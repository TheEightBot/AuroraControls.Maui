using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace AuroraControls;

[Obsolete("Use KeyboardToolbarEffect with KeyboardToolbar attached properties instead. See docs/keyboard-toolbar.md for migration guidance.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Disposable fields are cleaned up in OnDetached which is the correct MAUI PlatformEffect lifecycle.")]
public class AppleShowKeyboardDoneButtonEffect : PlatformEffect
{
    private Platforms.iOS.AuroraKeyboardAccessoryView? _accessoryView;
    private UIView? _previousAccessoryView;

    protected override void OnAttached()
    {
        if (this.Control is not UITextField textField)
        {
            return;
        }

        // MAUI installs its own MauiDoneAccessoryView on every Entry/Editor, so an existing
        // accessory view is the norm, not the exception. Replace it and restore on detach.
        _previousAccessoryView = textField.InputAccessoryView;

        _accessoryView = new Platforms.iOS.AuroraKeyboardAccessoryView();
        _accessoryView.Configure(
            KeyboardToolbarOptions.Default.DefaultButtonStyle,
            KeyboardToolbarOptions.Default.DefaultTitle,
            KeyboardToolbarOptions.Default.DefaultTitleColor,
            KeyboardToolbarOptions.Default.DefaultBackgroundColor,
            KeyboardToolbarOptions.Default.DefaultFontFamily,
            KeyboardToolbarOptions.Default.DefaultFontSize,
            KeyboardToolbarOptions.Default.DefaultHeight);

        var weakField = new WeakReference<UITextField>(textField);
        _accessoryView.DoneAction = () =>
        {
            if (weakField.TryGetTarget(out var tf))
            {
                tf.ResignFirstResponder();
            }
        };

        textField.InputAccessoryView = _accessoryView;
        textField.ReloadInputViews();
    }

    protected override void OnDetached()
    {
        if (this.Control is UITextField textField)
        {
            textField.InputAccessoryView = _previousAccessoryView;
            textField.ReloadInputViews();
        }

        _accessoryView?.Dispose();
        _accessoryView = null;
        _previousAccessoryView = null;
    }
}
