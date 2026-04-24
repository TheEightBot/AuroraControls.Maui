# Aurora Keyboard Toolbar

A replacement for MAUI's built-in keyboard "Done" accessory bar. Fully customizable, properly sized, and free of the iOS 26 checkmark clipping bug.

## Why?

MAUI unconditionally adds a `UIToolbar` with `UIBarButtonSystemItem.Done` to every `Entry` and `Editor` on iOS/macOS Catalyst. This causes four problems:

| Problem | Aurora solution |
|---------|----------------|
| Toolbar blocks input — fixed 44 pt bar covers content | Auto Layout view sizes to content |
| iOS 26 renders Done as a clipped blue checkbox | `Text` style uses `UIButton` (not a system icon) |
| Height is always 44 pt regardless of Dynamic Type | Height is content-driven; explicit override supported |
| No way to suppress MAUI's toolbar globally | `IsGloballyHidden = true` removes it on every control |

---

## Setup

### 1. Register at startup

```csharp
// MauiProgram.cs
builder.UseAuroraControls<App>();
```

With options (e.g., suppress MAUI's toolbar globally):

```csharp
builder.UseAuroraControls<App>(opts =>
{
    opts.IsGloballyHidden = true;          // Hide MAUI's done bar on all Entry/Editor
    opts.DefaultTitle = "Dismiss";         // Default button title for all Aurora toolbars
    opts.DefaultTitleColor = Colors.Blue;  // Default button color
});
```

### 2. Opt individual controls in (per-control)

```xml
xmlns:ap="clr-namespace:AuroraControls.AttachedProperties;assembly=AuroraControlsMaui"

<Entry ap:KeyboardToolbar.Show="True" />
```

Global suppression and per-control opt-in compose: set `IsGloballyHidden = true` at startup, then add `KeyboardToolbar.Show="True"` only to controls where you want the custom toolbar.

---

## Attached Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `KeyboardToolbar.Show` | `bool` | `false` | Attaches the Aurora toolbar to this control |
| `KeyboardToolbar.ButtonStyle` | `KeyboardToolbarDoneButtonStyle` | `Text` | `Text` or `SystemCheckmark` |
| `KeyboardToolbar.Title` | `string?` | `null` → global default | Button title (Text style only) |
| `KeyboardToolbar.TitleColor` | `Color?` | `null` → global default | Button text color (Text style only) |
| `KeyboardToolbar.ToolbarBackgroundColor` | `Color?` | `null` → global default | Background color of the accessory view |
| `KeyboardToolbar.ToolbarHeight` | `double` | `0` (auto) | Explicit height in points; 0 = size to content |
| `KeyboardToolbar.ToolbarFontFamily` | `string?` | `null` → system | Font family for the button label (Text style only) |
| `KeyboardToolbar.ToolbarFontSize` | `double` | `0` → system | Font size in points (Text style only) |

---

## Button Styles

### `Text` (default)

A native `UIButton` with `SetTitle()`. Fully customizable and immune to system icon changes on any iOS version.

```xml
<Entry ap:KeyboardToolbar.Show="True"
       ap:KeyboardToolbar.Title="Close"
       ap:KeyboardToolbar.TitleColor="OrangeRed"
       ap:KeyboardToolbar.ToolbarFontSize="16" />
```

### `SystemCheckmark`

Uses `UIBarButtonSystemItem.Done` inside a `UIToolbar` that is **not** frame-locked to 44 pt. The iOS 26 checkmark icon is displayed at its natural size without clipping.

```xml
<Entry ap:KeyboardToolbar.Show="True"
       ap:KeyboardToolbar.ButtonStyle="SystemCheckmark" />
```

---

## Global Options Reference

Configure via `UseAuroraControls<T>(opts => ...)`:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsGloballyHidden` | `bool` | `false` | Suppress MAUI's built-in Done bar on all Entry/Editor |
| `DefaultButtonStyle` | `KeyboardToolbarDoneButtonStyle` | `Text` | Default style for all Aurora toolbars |
| `DefaultTitle` | `string` | `"Done"` | Default button title |
| `DefaultTitleColor` | `Color?` | `null` | Default button color; `null` = system tint |
| `DefaultBackgroundColor` | `Color?` | `null` | Default toolbar background; `null` = system material |
| `DefaultHeight` | `double` | `0` | Default height; `0` = auto |
| `DefaultFontFamily` | `string?` | `null` | Default font family |
| `DefaultFontSize` | `double` | `0` | Default font size; `0` = system default |

---

## Platform Support

| Platform | Support |
|----------|---------|
| iOS | ✅ Full |
| macOS Catalyst | ✅ Full |
| Android | ✅ No-op (safe to set, does nothing) |
| Windows | ✅ No-op |

---

## Behaviour Notes

- **`IsEnabled = false`** or **`IsReadOnly = true`**: The toolbar is not shown, matching MAUI's own behaviour fix for [#21059](https://github.com/dotnet/maui/issues/21059).
- **Orientation changes**: The view uses Auto Layout — width adapts automatically with no frame calculations.
- **Dynamic Type**: The button's font uses `AdjustsFontForContentSizeCategory = true` and the view responds to `UIContentSizeCategoryDidChangeNotification`.
- **Memory**: A `WeakReference` breaks the `UITextField → InputAccessoryView → DoneAction → UITextField` retain cycle.

---

## Migration from `ShowKeyboardDoneButtonEffect`

`ShowKeyboardDoneButtonEffect` and `KeyboardDoneButton` are deprecated but remain functional. Migrate at your own pace:

**Before:**
```xml
<Entry>
    <Entry.Effects>
        <aurora:ShowKeyboardDoneButtonEffect />
    </Entry.Effects>
</Entry>
```

**After:**
```xml
<Entry ap:KeyboardToolbar.Show="True" />
```

The old `KeyboardDoneButton.Show` attached property now internally delegates to `KeyboardToolbar.Show`, so no XAML rename is required for that property — but the class is marked `[Obsolete]`.
