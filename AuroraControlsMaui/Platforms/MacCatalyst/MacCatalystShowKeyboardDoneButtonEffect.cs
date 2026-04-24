using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace AuroraControls;

[Obsolete("Use KeyboardToolbarEffect with KeyboardToolbar attached properties instead. See docs/keyboard-toolbar.md for migration guidance.")]
public class MacCatalystShowKeyboardDoneButtonEffect : PlatformEffect
{
    protected override void OnAttached()
    {
        var textField = this.Control as UITextField;

        // Fixed: was `&&` (would NullReferenceException); now `||` correctly guards both conditions
        if (textField == null || textField.InputAccessoryView != null)
        {
            return;
        }

        var accessoryView = new Platforms.MacCatalyst.AuroraKeyboardAccessoryView();
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
        var textField = this.Control as UITextField;

        // Fixed: was `&&` (same bug); now `||`
        if (textField == null || textField.InputAccessoryView == null)
        {
            return;
        }

        var iav = textField.InputAccessoryView;
        textField.InputAccessoryView = null;
        iav?.Dispose();
    }
}
