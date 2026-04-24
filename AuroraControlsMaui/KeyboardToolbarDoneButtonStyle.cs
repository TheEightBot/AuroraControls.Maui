namespace AuroraControls;

/// <summary>
/// Controls the visual style of the Done button in the custom keyboard accessory toolbar.
/// </summary>
public enum KeyboardToolbarDoneButtonStyle
{
    /// <summary>
    /// A plain text button using <see cref="UIKit.UIButton"/> (iOS/macOS Catalyst only).
    /// Fully configurable: title, color, font. Immune to system icon changes (e.g., iOS 26 checkmark).
    /// This is the default.
    /// </summary>
    Text = 0,

    /// <summary>
    /// Uses the system <see cref="UIKit.UIBarButtonItem"/> with <see cref="UIKit.UIBarButtonSystemItem.Done"/>
    /// embedded inside an unconstrained <see cref="UIKit.UIToolbar"/>. On iOS 26+, this renders as a
    /// system checkbox icon. The outer view is NOT frame-locked to 44 pt, so the icon is never clipped.
    /// Opt in to this only when you want the OS-native Done affordance.
    /// </summary>
    SystemCheckmark = 1,
}
