using Android.Views.InputMethods;
using Android.Widget;
using AuroraControls.AttachedProperties;
using Microsoft.Maui.Controls.Platform;

namespace AuroraControls;

public class DroidKeyboardReturnKeyTypeNameEffect : PlatformEffect
{
    private ImeAction _startingAction;
    private string _startingImeActionLabel;

    protected override void OnAttached()
    {
        var editText = Control as EditText;

        if (editText == null)
        {
            return;
        }

        _startingAction = editText.ImeOptions;
        _startingImeActionLabel = editText.ImeActionLabel;

        editText.EditorAction += EditText_EditorAction;

        SetReturnType();
    }

    protected override void OnDetached()
    {
        var editText = Control as EditText;

        if (editText == null || editText.Handle == IntPtr.Zero)
        {
            return;
        }

        editText.EditorAction -= EditText_EditorAction;

        editText.ImeOptions = _startingAction;
        editText.SetImeActionLabel(_startingImeActionLabel, _startingAction);
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
        var editText = Control as EditText;

        if (editText == null)
        {
            return;
        }

        var keyboardType = KeyboardInput.GetReturnKey(Element);

        switch (keyboardType)
        {
            case KeyboardReturnKeyType.Go:
                editText.ImeOptions = ImeAction.Go;
                editText.SetImeActionLabel("Go", ImeAction.Go);
                break;
            case KeyboardReturnKeyType.Next:
                editText.ImeOptions = ImeAction.Next;
                editText.SetImeActionLabel("Next", ImeAction.Next);
                break;
            case KeyboardReturnKeyType.Send:
                editText.ImeOptions = ImeAction.Send;
                editText.SetImeActionLabel("Send", ImeAction.Send);
                break;
            case KeyboardReturnKeyType.Search:
                editText.ImeOptions = ImeAction.Search;
                editText.SetImeActionLabel("Search", ImeAction.Search);
                break;
            case KeyboardReturnKeyType.Default:
                editText.ImeOptions = _startingAction;
                editText.SetImeActionLabel(_startingImeActionLabel, _startingAction);
                break;
        }
    }

    private void EditText_EditorAction(object sender, TextView.EditorActionEventArgs e)
    {
        var nextElement = KeyboardInput.GetNextVisualElement(this.Element);
        nextElement?.Focus();
    }
}
