# Keyboard Toolbar Overhaul — Implementation Checklist

> **Purpose**: Track all implementation work for replacing MAUI's default keyboard done-button toolbar with a fully configurable, iOS-26-compatible, per-control or globally-suppressed alternative.
>
> **How to use**: Check off items as work is completed. Sub-tasks are indented under their parent. Sections map to phases of implementation.

---

## Background & Context

### Root Issues Being Solved
- MAUI unconditionally calls `AddMauiDoneAccessoryView()` on every `Entry` and `Editor` via their iOS handlers (`EntryHandler.CreatePlatformView`, `EditorHandler.CreatePlatformView`), creating a `UIToolbar` with `UIBarButtonSystemItem.Done`.
- `UIBarButtonSystemItem.Done` on iOS 26 renders as a blue checkbox icon that clips inside the standard `UIToolbar` frame — but this checkmark style **should also be supported** as an intentional developer choice.
- The toolbar height is hardcoded to 44pt and does not respect Dynamic Type or accessibility sizing.
- The toolbar occludes the input area when the keyboard is shown on short views or in compact height.
- There is no built-in way to globally suppress it or per-control override it in Aurora.
- Aurora's existing `AppleShowKeyboardDoneButtonEffect` only handles `UITextField` (Entry), not `UITextView` (Editor), and stacks on top of MAUI's toolbar.

---

## Phase 1 — Global Configuration

### 1.1 — `KeyboardToolbarDoneButtonStyle` Enum
- [ ] Create `AuroraControlsMaui/KeyboardToolbarDoneButtonStyle.cs`
  - [ ] `Text` — renders a `UIButton` with a custom text label (default); immune to iOS 26 icon regression; supports `Title`, `TitleColor`, `Font`
  - [ ] `SystemCheckmark` — renders the iOS 26+ native blue checkmark via `UIBarButtonSystemItem.Done` (explicitly opt-in); clip-safe because the outer `UIView` sizes to contain it correctly, unlike MAUI's fixed-frame `UIToolbar`

### 1.2 — `KeyboardToolbarOptions` Configuration Type
- [ ] Create `AuroraControlsMaui/KeyboardToolbarOptions.cs`
  - [ ] `IsGloballyHidden` — `bool`, default `false`; when `true`, suppress MAUI's `MauiDoneAccessoryView` on all controls
  - [ ] `DefaultButtonStyle` — `KeyboardToolbarDoneButtonStyle`, default `Text`; selects between text button and system checkmark
  - [ ] `DefaultTitle` — `string`, default `"Done"`; button text when `ButtonStyle == Text`
  - [ ] `DefaultTitleColor` — `Color?`, default `null` (system tint); used as `UIButton.TintColor` when `ButtonStyle == Text`
  - [ ] `DefaultBackgroundColor` — `Color?`, default `null` (transparent/system material); fills the accessory view background
  - [ ] `DefaultHeight` — `double`, default `0` (auto-size); when `0`, view self-sizes via `intrinsicContentSize`
  - [ ] `DefaultFontFamily` — `string?`, default `null` (system font); font for the button label when `ButtonStyle == Text`
  - [ ] `DefaultFontSize` — `double`, default `0` (platform default)
  - [ ] Static singleton `Default` property
  - [ ] Optional fluent builder methods (e.g., `KeyboardToolbarOptions.Default.WithTitle("Close").WithIsGloballyHidden(true)`)

### 1.3 — `AuroraControlBuilder` Integration
- [ ] Add `UseAuroraControls(…, KeyboardToolbarOptions options)` overload
- [ ] When `options.IsGloballyHidden == true`, register handler modifiers that null out `InputAccessoryView` for `Entry` and `Editor` on iOS/macOS Catalyst
  - [ ] `EntryHandler.Mapper.AppendToMapping(nameof(IEntry.IsEnabled), SuppressMauiDoneButton)`
  - [ ] `EditorHandler.Mapper.AppendToMapping(nameof(IEditor.IsEnabled), SuppressMauiDoneButton)`
  - [ ] Helper: check `InputAccessoryView` is `MauiDoneAccessoryView` type before nulling (avoid clobbering custom views set by Aurora effect)
- [ ] Register new `KeyboardToolbarEffect` for iOS and macOS Catalyst

---

## Phase 2 — Custom Accessory View (iOS & macOS Catalyst)

### 2.1 — `AuroraKeyboardAccessoryView` (iOS)
- [ ] Create `AuroraControlsMaui/Platforms/iOS/AuroraKeyboardAccessoryView.cs`
  - [ ] Inherit from `UIView` (NOT `UIToolbar` — gives us full control of sizing and button rendering)
  - [ ] Accept configuration: `buttonStyle`, `title`, `titleColor`, `backgroundColor`, `font`, `height`
  - [ ] **`Text` style** (default): create a `UIButton` with a custom title text label
    - [ ] Set `UIButton.TitleLabel` with the configured font/color
    - [ ] Constrain button to right side with 16pt trailing padding
    - [ ] Minimum tap target 44×44pt
  - [ ] **`SystemCheckmark` style**: embed a `UIToolbar` sub-view containing `UIBarButtonItem(UIBarButtonSystemItem.Done, …)`
    - [ ] The outer `AuroraKeyboardAccessoryView` (`UIView`) sizes itself to the `UIToolbar`'s intrinsic content size — this is the key fix that prevents the iOS 26 clipping: the outer `UIView` is not frame-constrained to 44pt, so the `UIToolbar` and its checkmark are fully visible
    - [ ] `UIToolbar` set to `TranslatesAutoresizingMaskIntoConstraints = false`; pinned to all four edges of the outer `UIView`
  - [ ] Override `IntrinsicContentSize` — when `DefaultHeight == 0`, use compressed fitting size of inner content; otherwise return configured height
  - [ ] Use Auto Layout constraints internally — do NOT use fixed `CGRect` frame
  - [ ] Support `UpdateConfiguration(AuroraKeyboardToolbarConfiguration)` method for live style/title/color swaps without recreating the view
  - [ ] Handle `UIContentSizeCategoryDidChangeNotification` to re-size `Text`-style button label on Dynamic Type changes
  - [ ] Wire `Done` action: `UIResponder.ResignFirstResponder()` on the attached `UIView` / text field / text view
  - [ ] Expose `DoneAction` `Action<UIView>?` property for custom dismiss logic
  - [ ] Properly dispose `NSNotificationCenter` observer in `Dispose(bool)`
  - [ ] Expose `DoneAction` `Action<UIView>?` property so callers can inject custom dismiss logic
  - [ ] Properly dispose `NSNotificationCenter` observer in `Dispose(bool)`

### 2.2 — `AuroraKeyboardAccessoryView` (macOS Catalyst)
- [ ] Create `AuroraControlsMaui/Platforms/MacCatalyst/AuroraKeyboardAccessoryView.cs`
  - [ ] Identical structure to iOS version (can share via `#if` or a shared partial)
  - [ ] macOS Catalyst note: `UIScreen.MainScreen.Bounds.Width` is not meaningful on macOS; use AutoLayout and `SystemLayoutSizeFittingSize` instead

### 2.3 — Height Auto-Sizing
- [ ] Toolbar view uses `systemLayoutSizeFittingSize(UILayoutFittingCompressedSize)` to self-size
- [ ] When `DefaultHeight > 0`, use as fixed height
- [ ] When `DefaultHeight == 0`, compress to content (button label + vertical padding)
- [ ] Add 8pt top + 8pt bottom padding around the button for breathing room
- [ ] Verify with Dynamic Type Extra Large — button should expand gracefully

---

## Phase 3 — Platform Effects

### 3.1 — `KeyboardToolbarEffect` Routing Effect
- [ ] Create `AuroraControlsMaui/Effects/KeyboardToolbarEffect.cs`
  - [ ] Inherit `RoutingEffect`
  - [ ] Expose attached configuration properties (forwarded from `KeyboardToolbar` attached property class)

### 3.2 — `AppleKeyboardToolbarEffect` (iOS)
- [ ] Create `AuroraControlsMaui/Platforms/iOS/AppleKeyboardToolbarEffect.cs`
  - [ ] Inherit `PlatformEffect`
  - [ ] Handle `UITextField` (Entry, NumericEntry, SearchBar) in `OnAttached`
  - [ ] Handle `UITextView` (Editor) in `OnAttached`
  - [ ] Guard: if `InputAccessoryView` is already an `AuroraKeyboardAccessoryView`, just update its configuration rather than replacing
  - [ ] On attach: read all `KeyboardToolbar.*` attached property values; build `AuroraKeyboardAccessoryView`; set as `InputAccessoryView`
  - [ ] On detach: restore previous `InputAccessoryView` (null if it was MAUI's default that was already removed, or the original value)
  - [ ] `OnElementPropertyChanged`: react to `KeyboardToolbar.*` property changes and call `UpdateConfiguration`
  - [ ] Properly store and restore `InputAccessoryView` reference on detach

### 3.3 — `MacCatalystKeyboardToolbarEffect` (macOS Catalyst)
- [ ] Create `AuroraControlsMaui/Platforms/MacCatalyst/MacCatalystKeyboardToolbarEffect.cs`
  - [ ] Same logic as iOS version (macOS Catalyst has same `UITextField`/`UITextView` APIs)
  - [ ] Fix existing `MacCatalystShowKeyboardDoneButtonEffect.cs` bug: current code uses `&&` instead of `||` in null check (`if (textField == null && textField.InputAccessoryView != null)` — should be `||`)

---

## Phase 4 — Attached Properties

### 4.1 — `KeyboardToolbar` Attached Property Class
- [ ] Create `AuroraControlsMaui/AttachedProperties/KeyboardToolbar.cs`
  - [ ] `ShowProperty` — `bool`, default `false`; adds/removes `KeyboardToolbarEffect`
  - [ ] `ButtonStyleProperty` — `KeyboardToolbarDoneButtonStyle`, default `Text`; selects button rendering style
  - [ ] `TitleProperty` — `string?`, default `null` (falls back to `KeyboardToolbarOptions.Default.DefaultTitle`); applies when `ButtonStyle == Text`
  - [ ] `TitleColorProperty` — `Color?`, default `null` (falls back to global); applies when `ButtonStyle == Text`
  - [ ] `BackgroundColorProperty` — `Color?`, default `null` (falls back to global)
  - [ ] `HeightProperty` — `double`, default `0` (auto)
  - [ ] `FontFamilyProperty` — `string?`, default `null` (falls back to global); applies when `ButtonStyle == Text`
  - [ ] `FontSizeProperty` — `double`, default `0` (platform default); applies when `ButtonStyle == Text`
  - [ ] All properties trigger `UpdateConfiguration` on the effect if already attached
  - [ ] `OnShowChanged`: add/remove `KeyboardToolbarEffect` like `KeyboardDoneButton.OnKeyboardDoneButtonChanged` currently does

### 4.2 — XAML Namespace Availability
- [ ] Confirm `KeyboardToolbar` is accessible via `aurora:` XAML namespace in test app
- [ ] Example XAML (Text style):
  ```xml
  <Entry aurora:KeyboardToolbar.Show="True"
         aurora:KeyboardToolbar.ButtonStyle="Text"
         aurora:KeyboardToolbar.Title="Close"
         aurora:KeyboardToolbar.TitleColor="Blue" />
  ```
- [ ] Example XAML (SystemCheckmark style):
  ```xml
  <Entry aurora:KeyboardToolbar.Show="True"
         aurora:KeyboardToolbar.ButtonStyle="SystemCheckmark" />
  ```

---

## Phase 5 — Backward Compatibility / Deprecation

### 5.1 — Deprecate Old Effect and Attached Property
- [ ] `ShowKeyboardDoneButtonEffect.cs` — add `[Obsolete("Use KeyboardToolbarEffect instead")]` attribute; keep routing for BC
- [ ] `KeyboardDoneButton.cs` — add `[Obsolete("Use KeyboardToolbar attached property instead")]`; internally forward to `KeyboardToolbar.ShowProperty`
- [ ] `AppleShowKeyboardDoneButtonEffect.cs` — mark obsolete; keep functional by forwarding to `AppleKeyboardToolbarEffect`
- [ ] `MacCatalystShowKeyboardDoneButtonEffect.cs` — same as above; also fix the `&&`/`||` null-check bug even in deprecated path

---

## Phase 6 — `AuroraControlBuilder` Updates

- [ ] Register `KeyboardToolbarEffect` for iOS in effects configuration
- [ ] Register `KeyboardToolbarEffect` for macOS Catalyst in effects configuration
- [ ] Add `UseAuroraControls(…, Action<KeyboardToolbarOptions> configure)` overload for fluent configuration
- [ ] Global suppress logic (Phase 1.2) wired here using handler modifier pattern
- [ ] Unit-test that handler modifier runs only on iOS/macOS Catalyst

---

## Phase 7 — Test App

### 7.1 — New Test Page (`KeyboardToolbarTestPage`)
- [ ] Create `AuroraControls.TestApp/KeyboardToolbarTestPage.xaml` + `.xaml.cs`
- [ ] Section A: **Global suppress demo** — show Entry/Editor without any toolbar (requires `IsGloballyHidden = true`)
- [ ] Section B: **Per-control opt-in** — Entry/Editor with `KeyboardToolbar.Show="True"` and default settings
- [ ] Section C: **Button style variants**:
  - [ ] `Text` style (default) — custom title "Dismiss", blue color
  - [ ] `SystemCheckmark` style — uses iOS native checkmark; verify renders without clipping on iOS 26+
  - [ ] Side-by-side comparison: one Entry with `Text`, one with `SystemCheckmark`
- [ ] Section D: **Custom styling** (Text style only — N/A for SystemCheckmark):
  - [ ] Custom title text ("Dismiss", "Close", "✓ Done")
  - [ ] Custom title color (green, red, orange)
  - [ ] Custom background color (light gray, system material-like)
  - [ ] Custom font / bold
- [ ] Section E: **Height variants**:
  - [ ] Auto-height (default)
  - [ ] Fixed 32pt (compact)
  - [ ] Fixed 60pt (tall)
- [ ] Section F: **Dynamic Type** — verify toolbar resizes gracefully as user-preferred font scale changes
- [ ] Section G: **Editor (multi-line)** — confirm toolbar works on `Editor` not just `Entry`
- [ ] Section H: **NumericEntry** — confirm toolbar works on numeric keyboard (no "Done" key on hardware keyboard)
- [ ] Section I: **Comparison with MAUI default** — side-by-side (left: MAUI default, right: Aurora) for visual regression reference
- [ ] Add viewmodel / MVVM bindings for live value display
- [ ] Add page to `MainPage.cs` navigation list

### 7.2 — Existing Test Page Backward Compat Check
- [ ] Open `ShowKeyboardDoneButtonEffectTestPage.xaml` — confirm existing markup still compiles with `[Obsolete]` attributes
- [ ] Verify runtime behavior is unchanged for existing `ShowKeyboardDoneButtonEffect` usage

---

## Phase 8 — Documentation

### 8.1 — `docs/keyboard-toolbar.md`
- [ ] Overview: what the keyboard toolbar is and why Aurora overrides it
- [ ] Global suppress setup (code snippet for `MauiProgram.cs`)
- [ ] Per-control usage (XAML and C# snippets)
- [ ] All `KeyboardToolbar.*` attached property reference table
- [ ] All `KeyboardToolbarOptions` property reference table
- [ ] iOS 26 / macOS compatibility notes
- [ ] Migration guide from `ShowKeyboardDoneButtonEffect` / `KeyboardDoneButton.Show`
- [ ] Known limitations (Android is no-op, macOS Catalyst physical keyboard behavior)

### 8.2 — Code Comments
- [ ] XML doc comments on all `public` members of `KeyboardToolbarOptions`
- [ ] XML doc comments on all `public` members of `KeyboardToolbar` attached property class
- [ ] Inline comment explaining the iOS 26 `UIBarButtonSystemItem.Done` rendering regression

---

## Phase 9 — Quality & Correctness

### 9.1 — Bug Fixes (existing code)
- [ ] Fix `MacCatalystShowKeyboardDoneButtonEffect.cs`: null check logic (`&&` → `||`)
- [ ] Fix `AppleShowKeyboardDoneButtonEffect.cs`: add support for `UITextView` (Editor), not just `UITextField`
- [ ] Guard `InputAccessoryView` assignment: check control is not `null` before setting
- [ ] Ensure MAUI's `MauiDoneAccessoryView` suppression only targets `MauiDoneAccessoryView` instances (don't clobber custom accessory views set by other code)

### 9.2 — Memory & Lifecycle
- [ ] `AuroraKeyboardAccessoryView` — dispose `NSNotificationCenter` observer in `Dispose(bool disposing)`
- [ ] `AppleKeyboardToolbarEffect` — `OnDetached`: restore original `InputAccessoryView`, do not leave dangling reference
- [ ] Verify no retain cycle between `AuroraKeyboardAccessoryView` → text field → done action block

### 9.3 — Edge Cases
- [ ] Control with `IsEnabled = false` — toolbar should NOT be shown (match `MauiDoneAccessoryView` fix from dotnet/maui #21059)
- [ ] Control with `IsReadOnly = true` — toolbar should NOT be shown
- [ ] Multiple `Effects` applied to same control — only one `AuroraKeyboardAccessoryView` should be created
- [ ] Effect detached and re-attached — should cleanly replace accessory view
- [ ] `KeyboardToolbar.Show` set to `false` after `true` — removes toolbar and restores original (null) accessory view
- [ ] `UITextField` embedded inside custom view hierarchy (e.g., `StyledInputLayoutHandler`) — toolbar must still attach to the deepest `UITextField`/`UITextView`
- [ ] Orientation change — toolbar should resize width with `frame.size.width` update or use `autoresizingMask` / AutoLayout to follow screen width

---

## Checklist Summary

| Phase | Description | Status |
|-------|-------------|--------|
| 1 | Global Configuration (`KeyboardToolbarOptions` + Builder) | ⬜ Not started |
| 2 | Custom Accessory View (iOS + macOS) | ⬜ Not started |
| 3 | Platform Effects | ⬜ Not started |
| 4 | Attached Properties (`KeyboardToolbar`) | ⬜ Not started |
| 5 | Backward Compatibility / Deprecations | ⬜ Not started |
| 6 | `AuroraControlBuilder` Updates | ⬜ Not started |
| 7 | Test App Page | ⬜ Not started |
| 8 | Documentation | ⬜ Not started |
| 9 | Bug Fixes, Memory, Edge Cases | ⬜ Not started |

---

*Last updated: 2026-04-24*
