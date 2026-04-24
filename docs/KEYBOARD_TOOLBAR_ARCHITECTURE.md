# Keyboard Toolbar — Architecture & Design Reference

> This document describes the design decisions and technical architecture for the Aurora keyboard toolbar overhaul. It is a companion to `KEYBOARD_TOOLBAR_CHECKLIST.md`.

---

## Problem Analysis

### MAUI Default Toolbar (What We're Replacing)

MAUI's `MauiDoneAccessoryView` is a `UIToolbar` subclass created inside:
- `EntryHandler.CreatePlatformView()` → `platformEntry.AddMauiDoneAccessoryView(this)`
- `EditorHandler.CreatePlatformView()` → `platformEditor.AddMauiDoneAccessoryView(this)`

It sets `UITextField.InputAccessoryView` / `UITextView.InputAccessoryView` to a `UIToolbar` containing:
- `UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)`
- `UIBarButtonItem(UIBarButtonSystemItem.Done, handler)`

**Issues with this approach:**

| Issue | Root Cause | Impact |
|-------|-----------|--------|
| Blue checkbox icon on iOS 26 | `UIBarButtonSystemItem.Done` changed visual in iOS 26 | Clipped/broken UI |
| Fixed 44pt height | Hardcoded `CGRect(0, 0, width, 44)` | Doesn't adapt to Dynamic Type |
| Blocks input tapping | `UIToolbar` floats above keyboard and sits over content | Hard to tap behind toolbar |
| No customization | No Aurora API to change title, color, font | Developer frustration |
| No global suppress | MAUI always adds it | Cannot opt-out globally |

### Aurora's Existing Effect (What We're Upgrading)

`AppleShowKeyboardDoneButtonEffect` adds *another* `UIToolbar` on top of MAUI's — stacking two toolbars. It also:
- Only supports `UITextField`, not `UITextView`
- Uses the same `UIBarButtonSystemItem.Done` (same iOS 26 bug)
- Has the same fixed 44pt height
- The macOS Catalyst version has a null-check logic bug (`&&` should be `||`)

---

## Design Decisions

### 1. `UIView` not `UIToolbar`

The new `AuroraKeyboardAccessoryView` inherits from `UIView` instead of `UIToolbar`. This avoids:
- Any system interpretation of `UIBarButtonSystemItem` values
- iOS 26 icon rendering changes in `UIToolbar`
- Background blur material conflicts

We render our own background (or none), our own button, our own layout.

### 2. `UIButton` with custom title, not `UIBarButtonItem`

Instead of `UIBarButtonItem(UIBarButtonSystemItem.Done, …)`, we create a `UIButton` with:
```csharp
var btn = UIButton.FromType(UIButtonType.System);
btn.SetTitle(configuration.Title, UIControlState.Normal);
btn.TitleLabel.Font = /* configured or system default */;
btn.TintColor = /* configured color */;
```
This renders a text-only button that is immune to iOS 26 icon changes.

### 3. Auto-Layout, not frame-based sizing

The old code used:
```csharp
new UIToolbar(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 44))
```
Problems:
- `UIScreen.MainScreen.Bounds.Width` is not meaningful on macOS Catalyst
- Orientation changes require manual frame update
- Hardcoded 44pt

New approach: use Auto Layout constraints inside `AuroraKeyboardAccessoryView`, let the system size it. Override `IntrinsicContentSize` to return the desired height, or use `systemLayoutSizeFittingSize(UILayoutFittingCompressedSize)`.

### 4. Global suppress via Handler Modifier, not subclassing

To suppress MAUI's done button globally, we use the `Handler.Mapper` pattern:

```csharp
EntryHandler.Mapper.AppendToMapping("AuroraGlobalDoneButtonSuppress", (handler, view) =>
{
    if (handler.PlatformView?.InputAccessoryView is MauiDoneAccessoryView mauiBar)
    {
        handler.PlatformView.InputAccessoryView = null;
        mauiBar.Dispose();
    }
});
```

This runs *after* MAUI sets the accessory view, cleanly removes it, and doesn't affect any other accessory views (like ones set by `AuroraKeyboardAccessoryView`).

### 5. Per-control opt-in via Effect + Attached Property

The existing `KeyboardDoneButton.Show` + `ShowKeyboardDoneButtonEffect` pattern is correct in spirit. We extend it with full configuration:

```xml
<Entry aurora:KeyboardToolbar.Show="True"
       aurora:KeyboardToolbar.Title="Close"
       aurora:KeyboardToolbar.TitleColor="Blue"
       aurora:KeyboardToolbar.Height="0" />
```

The attached property handler adds/removes `KeyboardToolbarEffect`, which is a `RoutingEffect`. The platform effect (`AppleKeyboardToolbarEffect`) creates and attaches `AuroraKeyboardAccessoryView`.

### 6. Configuration Fallback Chain

```
Per-control KeyboardToolbar.TitleColor
  → if null, fall back to KeyboardToolbarOptions.Default.DefaultTitleColor
    → if null, use UIView tintColor (system blue)
```

This means global defaults work "out of the box" and per-control overrides are additive.

### 7. `KeyboardToolbarDoneButtonStyle` Enum — Two Rendering Paths

The accessory view supports two explicit button styles:

| Style | Rendering | Best Use |
|-------|-----------|----------|
| `Text` (default) | `UIButton` with custom title string | Full customization; immune to iOS 26 icon changes |
| `SystemCheckmark` | `UIBarButtonItem(UIBarButtonSystemItem.Done)` embedded inside a `UIToolbar` sub-view | Intentionally adopts the iOS 26 blue checkmark; pre-iOS 26 renders as "Done" text |

**Key insight for `SystemCheckmark`**: The iOS 26 *clipping* bug in MAUI's implementation is caused by the outer `UIToolbar` having a hardcoded 44pt height frame. By embedding the `UIToolbar` inside our custom `AuroraKeyboardAccessoryView` (`UIView`) and sizing it with Auto Layout, the outer container can grow to fit the system checkmark — so `SystemCheckmark` is safe to use without clipping.

**`Text` is the default** because it:
- Is immune to future iOS icon rendering changes
- Supports full title/color/font customization
- Renders consistently on all iOS versions and macOS Catalyst

`SystemCheckmark` should be explicitly opted into when the developer wants the native platform appearance.

---

## Component Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│ MauiProgram.cs                                                   │
│  .UseAuroraControls(opts => opts.IsGloballyHidden = true)        │
└─────────────────────────┬───────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│ AuroraControlBuilder                                             │
│  - Registers KeyboardToolbarEffect                               │
│  - If IsGloballyHidden: appends EntryHandler/EditorHandler       │
│    mapper to null InputAccessoryView (only MauiDoneAccessoryView)│
└─────────────────────────────────────────────────────────────────┘

XAML ──► aurora:KeyboardToolbar.Show="True"
              │
              ▼
┌─────────────────────────────────────────────────────────────────┐
│ KeyboardToolbar (AttachedProperty)                               │
│  - ShowProperty, ButtonStyleProperty, TitleProperty,             │
│    TitleColorProperty, etc.                                      │
│  - OnShowChanged: adds/removes KeyboardToolbarEffect             │
└─────────────────────────┬───────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│ KeyboardToolbarEffect (RoutingEffect)                            │
└─────────────────────────┬───────────────────────────────────────┘
                          │
              ┌───────────┴────────────┐
              │                        │
              ▼                        ▼
┌─────────────────────┐   ┌─────────────────────────────────┐
│ AppleKeyboard-      │   │ MacCatalystKeyboard-            │
│ ToolbarEffect       │   │ ToolbarEffect                   │
│ (PlatformEffect)    │   │ (PlatformEffect)                │
│                     │   │                                 │
│ OnAttached:         │   │ OnAttached:                     │
│  - read config      │   │  - same as iOS                  │
│  - create Aurora-   │   │  - create AuroraKeyboard-       │
│    KeyboardAccessory│   │    AccessoryView                │
│    View             │   └─────────────────────────────────┘
│  - set as Input-    │
│    AccessoryView    │
│    on UITextField / │
│    UITextView       │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────────────────────────────────────────────────┐
│ AuroraKeyboardAccessoryView : UIView                             │
│                                                                  │
│  ButtonStyle == Text (default)      ButtonStyle == SystemCheckmark│
│  ┌──────────────────────────┐       ┌─────────────────────────┐  │
│  │ UIButton (custom title,  │       │ UIToolbar sub-view with │  │
│  │  color, font)            │       │ UIBarButtonSystemItem   │  │
│  │ constrained right side   │       │ .Done (iOS 26 checkmark)│  │
│  └──────────────────────────┘       └─────────────────────────┘  │
│                                                                  │
│  - Auto Layout — no hardcoded frames                             │
│  - IntrinsicContentSize returns configured or compressed height  │
│  - DoneAction: resignFirstResponder                              │
│  - UpdateConfiguration(config): live property updates            │
└─────────────────────────────────────────────────────────────────┘
```

---

## API Surface

### `KeyboardToolbarDoneButtonStyle` Enum

```csharp
/// <summary>
/// Controls how the Done button is rendered in the keyboard accessory toolbar.
/// </summary>
public enum KeyboardToolbarDoneButtonStyle
{
    /// <summary>
    /// Default. Renders a UIButton with a custom text label.
    /// Immune to iOS 26 icon changes. Supports Title, TitleColor, Font.
    /// </summary>
    Text,

    /// <summary>
    /// Renders the native UIBarButtonSystemItem.Done.
    /// On iOS 26+ this is a blue checkmark. On earlier versions it shows "Done".
    /// Explicitly opt-in for developers who want the system appearance.
    /// The clip bug from MAUI's implementation is avoided by hosting it in
    /// AuroraKeyboardAccessoryView (UIView) with Auto Layout.
    /// </summary>
    SystemCheckmark
}
```

### `KeyboardToolbarOptions` (global singleton)

```csharp
public class KeyboardToolbarOptions
{
    public static KeyboardToolbarOptions Default { get; }

    /// <summary>When true, suppress MAUI's built-in done toolbar globally.</summary>
    public bool IsGloballyHidden { get; set; } = false;

    /// <summary>Default button style. Defaults to Text (custom label, immune to iOS 26 icon changes).</summary>
    public KeyboardToolbarDoneButtonStyle DefaultButtonStyle { get; set; } = KeyboardToolbarDoneButtonStyle.Text;

    /// <summary>Button title when ButtonStyle == Text. Defaults to "Done".</summary>
    public string DefaultTitle { get; set; } = "Done";

    /// <summary>Button text color when ButtonStyle == Text. Null = system tint color.</summary>
    public Color? DefaultTitleColor { get; set; }

    /// <summary>Toolbar background color. Null = transparent.</summary>
    public Color? DefaultBackgroundColor { get; set; }

    /// <summary>Toolbar height in points. 0 = auto-size to content.</summary>
    public double DefaultHeight { get; set; } = 0;

    /// <summary>Button font family when ButtonStyle == Text. Null = system font.</summary>
    public string? DefaultFontFamily { get; set; }

    /// <summary>Button font size in points when ButtonStyle == Text. 0 = system default size.</summary>
    public double DefaultFontSize { get; set; } = 0;
}
```

### `KeyboardToolbar` Attached Properties (XAML)

```xml
<!-- Text style (default) — full customization -->
aurora:KeyboardToolbar.Show="True|False"
aurora:KeyboardToolbar.ButtonStyle="Text"
aurora:KeyboardToolbar.Title="Done"
aurora:KeyboardToolbar.TitleColor="Blue"
aurora:KeyboardToolbar.BackgroundColor="Transparent"
aurora:KeyboardToolbar.Height="0"
aurora:KeyboardToolbar.FontFamily="Helvetica"
aurora:KeyboardToolbar.FontSize="16"

<!-- SystemCheckmark style — opt-in iOS 26 native checkmark -->
aurora:KeyboardToolbar.Show="True"
aurora:KeyboardToolbar.ButtonStyle="SystemCheckmark"
```

### `MauiProgram.cs` Usage

```csharp
// Suppress toolbar globally and set Text style defaults
builder.UseAuroraControls<App>(options =>
{
    options.IsGloballyHidden = true;
    options.DefaultButtonStyle = KeyboardToolbarDoneButtonStyle.Text;
    options.DefaultTitle = "Close";
    options.DefaultTitleColor = Colors.DarkGray;
});

// Or opt-in to iOS 26 system checkmark globally:
builder.UseAuroraControls<App>(options =>
{
    options.IsGloballyHidden = true;
    options.DefaultButtonStyle = KeyboardToolbarDoneButtonStyle.SystemCheckmark;
});

// Per-control opt-in when globally hidden:
// <Entry aurora:KeyboardToolbar.Show="True" />

// Per-control SystemCheckmark override:
// <Entry aurora:KeyboardToolbar.Show="True"
//        aurora:KeyboardToolbar.ButtonStyle="SystemCheckmark" />
```

---

## File Map

```
AuroraControlsMaui/
├── KeyboardToolbarDoneButtonStyle.cs       ← NEW: enum (Text / SystemCheckmark)
├── KeyboardToolbarOptions.cs               ← NEW: global config
├── Effects/
│   ├── KeyboardToolbarEffect.cs            ← NEW: routing effect
│   └── ShowKeyboardDoneButtonEffect.cs     ← MODIFIED: [Obsolete] forward
├── AttachedProperties/
│   ├── KeyboardToolbar.cs                  ← NEW: full attached prop set
│   └── KeyboardDoneButton.cs              ← MODIFIED: [Obsolete] forward
├── Platforms/
│   ├── iOS/
│   │   ├── AuroraKeyboardAccessoryView.cs  ← NEW: UIView-based toolbar
│   │   ├── AppleKeyboardToolbarEffect.cs   ← NEW: platform effect
│   │   └── AppleShowKeyboardDoneButton-
│   │       Effect.cs                       ← MODIFIED: [Obsolete] forward
│   └── MacCatalyst/
│       ├── AuroraKeyboardAccessoryView.cs  ← NEW: UIView-based toolbar
│       ├── MacCatalystKeyboardToolbar-
│       │   Effect.cs                       ← NEW: platform effect
│       └── MacCatalystShowKeyboardDone-
│           ButtonEffect.cs                 ← MODIFIED: [Obsolete] + bug fix
└── AuroraControlBuilder.cs                ← MODIFIED: register + suppress

docs/
├── KEYBOARD_TOOLBAR_CHECKLIST.md          ← Implementation checklist
└── KEYBOARD_TOOLBAR_ARCHITECTURE.md       ← This file

AuroraControls.TestApp/
└── KeyboardToolbarTestPage.xaml[.cs]      ← NEW: test page
```

---

## Key Implementation Notes

### iOS 26 Compatibility

`UIBarButtonSystemItem.Done` in iOS 26 renders as a blue checkbox icon instead of the word "Done", and **clips** when the outer `UIToolbar` is constrained to 44pt (MAUI's root cause).

Aurora handles this in two ways:

**Default (`Text` style) — fully immune:**
```csharp
// ✅ Safe on all iOS versions — uses UIButton with text, not UIBarButtonSystemItem
var btn = UIButton.FromType(UIButtonType.System);
btn.SetTitle("Done", UIControlState.Normal);
```

**Opt-in (`SystemCheckmark` style) — intentionally adopts the iOS 26 checkmark:**
```csharp
// ✅ Safe in AuroraKeyboardAccessoryView because Auto Layout allows the outer
// UIView to grow — the clip bug only occurs in MAUI's hardcoded 44pt UIToolbar frame
var toolbar = new UIToolbar();
toolbar.Items = new[]
{
    new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
    new UIBarButtonItem(UIBarButtonSystemItem.Done, this, new ObjCRuntime.Selector("doneAction"))
};
// Pin toolbar to all four edges of outer UIView; UIView height is unconstrained
```

`SystemCheckmark` should only be chosen when the developer explicitly wants the iOS 26+ native appearance. On pre-iOS 26 it renders as the word "Done" in system font.

### Editor vs Entry Support

MAUI's `Editor` uses `UITextView`, while `Entry` uses `UITextField`. Both support `InputAccessoryView`. The Aurora effect must handle both:

```csharp
// In OnAttached:
if (Control is UITextField textField)
    textField.InputAccessoryView = accessoryView;
else if (Control is UITextView textView)
    textView.InputAccessoryView = accessoryView;
```

### Width on macOS Catalyst

On macOS Catalyst, `UIScreen.MainScreen.Bounds.Width` may return the full display width rather than the window width. Use Auto Layout so the toolbar fills its container naturally:

```csharp
TranslatesAutoresizingMaskIntoConstraints = false;
// Let the system size the width — don't set a fixed width.
```

### Disabled / ReadOnly Controls

When `IsEnabled = false` or `IsReadOnly = true`, the toolbar should NOT be shown (consistent with the fix for dotnet/maui #21059). Check these states in `OnAttached` and `OnElementPropertyChanged`.

---

*Last updated: 2026-04-24*
