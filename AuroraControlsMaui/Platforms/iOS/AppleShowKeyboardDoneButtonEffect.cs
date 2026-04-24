using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace AuroraControls;

[Obsolete("Use KeyboardToolbarEffect with KeyboardToolbar attached properties instead. See docs/keyboard-toolbar.md for migration guidance.")]
public class AppleShowKeyboardDoneButtonEffect : PlatformEffect
{
    protected override void OnAttached()
    {
        if (this.Control is not UITextField textField || textField.InputAccessoryView != null)
        {
            return;
        }

        var accessoryView = new Platforms.iOS.AuroraKeyboardAccessoryView();
        accessoryView.Configure(
            KeyboardToolbarOptions.Default.DefaultButtonStyle,
            KeyboardToolbarOptions.Default.DefaultTitle,
            KeyboardToolbarOptions.Default.DefaultTitleColor,
            KeyboardToolbarOptions.Default.DefaultBackgroundColor,
            KeyboardToolbarOptions.Default.DefaultFontFamily,
            KeyboardToolbarOptions.Default.DefaultFontSize,
            KeyboardToolbarOptions.Default.DefaultHeight);

        var weakField = new WeakReference<UITextField>(textField);
        accessoryView.DoneAction = () =>
        {
            if (weakField.TryGetTarget(out var tf))
            {
                tf.ResignFirstResponder();
            }
        };

        textField.InputAccessoryView = accessoryView;
    }

    protected override void OnDetached()
    {
        if (this.Control is not UITextField textField || textField.InputAccessoryView == null)
        {
            return;
        }

        var iav = textField.InputAccessoryView;
        textField.InputAccessoryView = null;
        iav?.Dispose();
    }
}
