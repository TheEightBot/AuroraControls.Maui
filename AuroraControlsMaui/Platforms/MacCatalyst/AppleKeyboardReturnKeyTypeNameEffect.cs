using AuroraControls.AttachedProperties;
using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace AuroraControls;

public class AppleKeyboardReturnKeyTypeNameEffect : PlatformEffect
{
    private UIReturnKeyType _startingReturnKeyType;

    protected override void OnAttached()
    {
        if (this.Control is not UITextField textField)
        {
            return;
        }

        _startingReturnKeyType = textField.ReturnKeyType;

        textField.ShouldReturn += TextField_ShouldReturn;

        SetReturnType();
    }

    protected override void OnDetached()
    {
        if (this.Control is not UITextField textField)
        {
            return;
        }

        textField.ShouldReturn -= TextField_ShouldReturn;

        textField.ReturnKeyType = _startingReturnKeyType;
    }

    protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
    {
        if (args.PropertyName.Equals(KeyboardInput.ReturnKeyProperty.PropertyName))
        {
            SetReturnType();
        }
        else
        {
            base.OnElementPropertyChanged(args);
        }
    }

    private void SetReturnType()
    {
        if (this.Control is not UITextField textField)
        {
            return;
        }

        _startingReturnKeyType = textField.ReturnKeyType;

        var keyboardType = KeyboardInput.GetReturnKey(Element);

        switch (keyboardType)
        {
            case KeyboardReturnKeyType.Go:
                textField.ReturnKeyType = UIReturnKeyType.Go;
                break;
            case KeyboardReturnKeyType.Next:
                textField.ReturnKeyType = UIReturnKeyType.Next;
                break;
            case KeyboardReturnKeyType.Send:
                textField.ReturnKeyType = UIReturnKeyType.Send;
                break;
            case KeyboardReturnKeyType.Search:
                textField.ReturnKeyType = UIReturnKeyType.Search;
                break;
            case KeyboardReturnKeyType.Done:
                textField.ReturnKeyType = UIReturnKeyType.Done;
                break;
            case KeyboardReturnKeyType.Default:
                textField.ReturnKeyType = UIReturnKeyType.Default;
                break;
        }
    }

    private bool TextField_ShouldReturn(UITextField textField)
    {
        var nextElement = KeyboardInput.GetNextVisualElement(this.Element);

        nextElement?.Focus();

        return true;
    }
}
