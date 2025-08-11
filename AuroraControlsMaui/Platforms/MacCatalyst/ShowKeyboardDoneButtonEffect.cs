using System.Drawing;
using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace AuroraControls;

public class ShowKeyboardDoneButtonEffect : PlatformEffect
{
    protected override void OnAttached()
    {
        var textField = this.Control as UITextField;

        if (textField == null && textField.InputAccessoryView != null)
        {
            return;
        }

        var width = (float)UIScreen.MainScreen.Bounds.Width;
        var toolbar = new UIToolbar(new RectangleF(0, 0, width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };
        var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
        var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) => textField.ResignFirstResponder());

        toolbar.SetItems(new[] { spacer, doneButton }, false);

        textField.InputAccessoryView = toolbar;
    }

    protected override void OnDetached()
    {
        var textField = this.Control as UITextField;

        if (textField == null && textField.InputAccessoryView != null)
        {
            return;
        }

        var iav = textField.InputAccessoryView;
        textField.InputAccessoryView = null;
        iav?.Dispose();
    }
}
