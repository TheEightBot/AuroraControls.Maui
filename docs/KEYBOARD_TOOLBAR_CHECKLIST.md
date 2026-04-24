# Keyboard Toolbar Overhaul — Implementation Checklist

> **Purpose**: Track all implementation work for replacing MAUI's default keyboard done-button toolbar with a fully configurable, iOS-26-compatible, per-control or globally-suppressed alternative.
>
> **How to use**: Check off items as work is completed. Sub-tasks are indented under their parent. Sections map to phases of implementation.

---

## Background & Context

### Root Issues Being Solved
- MAUI unconditionally calls `AddMauiDoneAccessoryView()` on every `Entry` and `Editor` via their iOS handlers, creating a `UIToolbar` with `UIBarButtonSystemItem.Done`.
- `UIBarButtonSystemItem.Done` on iOS 26 renders as a blue checkbox icon that clips inside the standard `UIToolbar` frame — but this checkmark style **should also be supported** as an intentional developer choice.
- The toolbar height is hardcoded to 44pt and does not respect Dynamic Type or accessibility sizing.
- The toolbar occludes the input area when the keyboard is shown on short views or in compact height.
- There is no built-in way to globally suppress it or per-control override it in Aurora.
- Aurora's existing `AppleShowKeyboardDoneButtonEffect` only handles `UITextField` (Entry), not `UITextView` (Editor), and stacks on top of MAUI's toolbar.

---

## Phase 1 — Global Configuration ✅ COMPLETE

### 1.1 — `KeyboardToolbarDoneButtonStyle` Enum ✅
- [x] Create `AuroraControlsMaui/KeyboardToolbarDoneButtonStyle.cs`
  - [x] `Text` — renders a `UIButton` with a custom text label (default); immune to iOS 26 icon regression
  - [x] `SystemCheckmark` — renders the iOS 26+ native blue checkmark via `UIBarButtonSystemItem.Done` (explicitly opt-in); clip-safe

### 1.2 — `KeyboardToolbarOptions` Configuration Type ✅
- [x] Create `AuroraControlsMaui/KeyboardToolbarOptions.cs`
  - [x] `IsGloballyHidden` — `bool`, default `false`
  - [x] `DefaultButtonStyle` — `KeyboardToolbarDoneButtonStyle`, default `Text`
  - [x] `DefaultTitle` — `string`, default `"Done"`
  - [x] `DefaultTitleColor` — `Color?`, default `null` (system tint)
  - [x] `DefaultBackgroundColor` — `Color?`, default `null`
  - [x] `DefaultHeight` — `double`, default `0` (auto-size)
  - [x] `DefaultFontFamily` — `string?`, default `null`
  - [x] `DefaultFontSize` — `double`, default `0`
  - [x] Static singleton `Default` property
  - [x] Fluent builder methods (`WithTitle`, `WithIsGloballyHidden`, etc.)

### 1.3 — `AuroraControlBuilder` Integration ✅
- [x] Add `UseAuroraControls<T>(…, Action<KeyboardToolbarOptions>? configure)` overload
- [x] When `IsGloballyHidden == true`, register handler mapper that nulls out `InputAccessoryView` (typed cast via `UITextField`/`UITextView`)
  - [x] `EntryHandler.Mapper.AppendToMapping("AuroraGlobalKeyboardDoneButtonSuppress", …)`
  - [x] `EditorHandler.Mapper.AppendToMapping("AuroraGlobalKeyboardDoneButtonSuppress", …)`
  - [x] Guard by type name `"MauiDoneAccessoryView"` to avoid clobbering custom views
- [x] Register new `KeyboardToolbarEffect` for iOS and macOS Catalyst

---

## Phase 2 — Custom Accessory View (iOS & macOS Catalyst) ✅ COMPLETE

### 2.1 — `AuroraKeyboardAccessoryView` (iOS) ✅
- [x] Create `AuroraControlsMaui/Platforms/iOS/AuroraKeyboardAccessoryView.cs`
  - [x] Inherits `UIView` (NOT `UIToolbar`)
  - [x] `Configure(style, title, titleColor, backgroundColor, fontFamily, fontSize, height)` method
  - [x] **`Text` style**: `UIButton` constrained to trailing, 16pt padding, ≥44pt tap target
  - [x] **`SystemCheckmark` style**: `UIToolbar` pinned to all 4 edges — no iOS 26 clipping
  - [x] `IntrinsicContentSize` override: returns fixed height or content-derived height
  - [x] Auto Layout constraints internally (no fixed `CGRect` frames)
  - [x] `UpdateTitle`, `UpdateTitleColor`, `UpdateFont`, `UpdateBackgroundColor` live-update methods
  - [x] `UIContentSizeCategoryChangedNotification` observer for Dynamic Type
  - [x] `DoneAction` using `WeakReference<UIView>` to break retain cycles
  - [x] `Dispose(bool)` disposes `_fontSizeObserver`, `_textButton`, `_systemToolbar`

### 2.2 — `AuroraKeyboardAccessoryView` (macOS Catalyst) ✅
- [x] Create `AuroraControlsMaui/Platforms/MacCatalyst/AuroraKeyboardAccessoryView.cs`
  - [x] Identical structure to iOS version; same Auto Layout approach

### 2.3 — Height Auto-Sizing ✅
- [x] `0` height → auto-size from content (≥60pt for text button, UIToolbar intrinsic for checkmark)
- [x] `> 0` height → fixed via `HeightAnchor.ConstraintEqualTo`
- [x] Dynamic Type handled via notification + `InvalidateIntrinsicContentSize`

---

## Phase 3 — Platform Effects ✅ COMPLETE

### 3.1 — `KeyboardToolbarEffect` Routing Effect ✅
- [x] Create `AuroraControlsMaui/Effects/KeyboardToolbarEffect.cs` (inherits `RoutingEffect`)

### 3.2 — `AppleKeyboardToolbarEffect` (iOS) ✅
- [x] Create `AuroraControlsMaui/Platforms/iOS/AppleKeyboardToolbarEffect.cs`
  - [x] Handles `UITextField` (Entry) and `UITextView` (Editor) via typed cast helpers
  - [x] Reads all `KeyboardToolbar.*` attached properties; falls back to `KeyboardToolbarOptions`
  - [x] Saves and restores previous `InputAccessoryView` on detach
  - [x] `OnElementPropertyChanged`: reacts to all `KeyboardToolbar.*` changes
  - [x] `SetInputAccessoryView` / `GetInputAccessoryView` typed cast helpers (net10.0-ios compat)
  - [x] CA1001 suppressed; lifecycle managed via `OnDetached`

### 3.3 — `MacCatalystKeyboardToolbarEffect` (macOS Catalyst) ✅
- [x] Create `AuroraControlsMaui/Platforms/MacCatalyst/MacCatalystKeyboardToolbarEffect.cs`
  - [x] Same logic as iOS version
  - [x] `&&`→`||` bug in deprecated effect also fixed

---

## Phase 4 — Attached Properties ✅ COMPLETE

### 4.1 — `KeyboardToolbar` Attached Property Class ✅
- [x] Create `AuroraControlsMaui/AttachedProperties/KeyboardToolbar.cs`
  - [x] `ShowProperty` — `bool`; adds/removes `KeyboardToolbarEffect`
  - [x] `ButtonStyleProperty` — `KeyboardToolbarDoneButtonStyle`
  - [x] `TitleProperty` — `string?`
  - [x] `TitleColorProperty` — `Color?`
  - [x] `BackgroundColorProperty` — `Color?` (registered as `"ToolbarBackgroundColor"`)
  - [x] `HeightProperty` — `double` (registered as `"ToolbarHeight"`)
  - [x] `FontFamilyProperty` — `string?` (registered as `"ToolbarFontFamily"`)
  - [x] `FontSizeProperty` — `double` (registered as `"ToolbarFontSize"`)
  - [x] XML doc comments on all public getters/setters

### 4.2 — XAML Namespace Availability ✅
- [x] `KeyboardToolbar` accessible via `aurora:` XAML namespace via test page

---

## Phase 5 — Backward Compatibility / Deprecation ✅ COMPLETE

### 5.1 — Deprecate Old Effect and Attached Property ✅
- [x] `ShowKeyboardDoneButtonEffect.cs` — `[Obsolete]` added
- [x] `KeyboardDoneButton.cs` — `[Obsolete]`; forwards to `KeyboardToolbar.SetShow()`
- [x] `AppleShowKeyboardDoneButtonEffect.cs` — marked obsolete; uses `AuroraKeyboardAccessoryView`
- [x] `MacCatalystShowKeyboardDoneButtonEffect.cs` — `[Obsolete]`; `&&`→`||` bug fixed

---

## Phase 6 — `AuroraControlBuilder` Updates ✅ COMPLETE

- [x] `KeyboardToolbarEffect` registered for iOS
- [x] `KeyboardToolbarEffect` registered for macOS Catalyst
- [x] `UseAuroraControls<T>` overload accepting `Action<KeyboardToolbarOptions>?`
- [x] Global suppress logic wired via handler mapper (runs once per handler instantiation)

---

## Phase 7 — Test App ✅ COMPLETE

### 7.1 — New Test Page (`KeyboardToolbarTestPage`) ✅
- [x] Create `AuroraControls.TestApp/KeyboardToolbarTestPage.xaml` + `.xaml.cs`
- [x] Section: Per-control opt-in (default settings)
- [x] Section: `Text` style with custom title/color
- [x] Section: `SystemCheckmark` style (iOS 26 safe)
- [x] Section: Custom styling (title, color, font)
- [x] Section: Height variants (auto, compact, tall)
- [x] Section: Editor (multi-line)
- [x] Add page to `MainPage.cs` navigation list

### 7.2 — Remaining Test Scenarios
- [ ] Verify runtime behavior on device/simulator for all sections (manual QA)
- [ ] Verify iOS 26 simulator: `SystemCheckmark` style renders checkmark without clipping
- [ ] Verify Dynamic Type: toolbar resizes gracefully at Extra Large text scale
- [ ] `IsReadOnly = true` / `IsEnabled = false` — toolbar should not appear
- [ ] Effect detach/re-attach round-trip
- [ ] `StyledInputLayout` integration (innermost `UITextField` detection)

---

## Phase 8 — Documentation ✅ COMPLETE

### 8.1 — `docs/keyboard-toolbar.md` ✅
- [x] Overview section
- [x] Global suppress setup (code snippet)
- [x] Per-control usage (XAML and C# snippets)
- [x] `KeyboardToolbar.*` attached property reference table
- [x] `KeyboardToolbarOptions` property reference table
- [x] iOS 26 / macOS compatibility notes
- [x] Migration guide from `ShowKeyboardDoneButtonEffect`

### 8.2 — Code Comments ✅
- [x] XML doc on all public members of `KeyboardToolbarOptions`
- [x] XML doc on all public members of `KeyboardToolbar`
- [x] XML doc on platform effect classes

---

## Phase 9 — Quality & Correctness

### 9.1 — Bug Fixes ✅
- [x] Fix `MacCatalystShowKeyboardDoneButtonEffect.cs`: `&&` → `||`
- [x] Fix `InputAccessoryView` read-only (net10.0-ios): typed cast via `UITextField`/`UITextView`
- [x] Fix `UIColor.ColorWithAlphaComponent` removal (net10.0-ios): use `UIColor.SystemBackground`

### 9.2 — Memory & Lifecycle ✅
- [x] `AuroraKeyboardAccessoryView`: disposes `_fontSizeObserver`, `_textButton`, `_systemToolbar` in `Dispose(bool)`
- [x] `AppleKeyboardToolbarEffect`: `OnDetached` restores original `InputAccessoryView`
- [x] No retain cycle: `DoneAction` uses `WeakReference<UIView>`

### 9.3 — Edge Cases (Manual QA Remaining)
- [ ] `IsEnabled = false` — toolbar should not be shown
- [ ] `IsReadOnly = true` — toolbar should not be shown
- [ ] Multiple effects on same control — only one `AuroraKeyboardAccessoryView` created
- [ ] Effect detach and re-attach — clean replacement
- [ ] `KeyboardToolbar.Show` toggled `false` after `true` — restores original accessory view
- [ ] `StyledInputLayout` inner field integration
- [ ] Orientation change — toolbar width follows screen width via Auto Layout

---

## Checklist Summary

| Phase | Description | Status |
|-------|-------------|--------|
| 1 | Global Configuration (`KeyboardToolbarOptions` + Builder) | ✅ Complete |
| 2 | Custom Accessory View (iOS + macOS) | ✅ Complete |
| 3 | Platform Effects | ✅ Complete |
| 4 | Attached Properties (`KeyboardToolbar`) | ✅ Complete |
| 5 | Backward Compatibility / Deprecations | ✅ Complete |
| 6 | `AuroraControlBuilder` Updates | ✅ Complete |
| 7 | Test App Page | ✅ Complete (manual QA remaining) |
| 8 | Documentation | ✅ Complete |
| 9 | Bug Fixes, Memory, Edge Cases | ⬜ Manual QA remaining |

---

*Last updated: implementation complete; build passing 0 errors on net10.0-ios and net10.0-maccatalyst*
