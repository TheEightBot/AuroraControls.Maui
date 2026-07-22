using AuroraControls.AttachedProperties;
using AuroraControls.Platforms.MacCatalyst;
using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace AuroraControls;

/// <summary>
/// macOS Catalyst platform implementation of <see cref="KeyboardToolbarEffect"/>.
/// Attaches a configurable <see cref="AuroraKeyboardAccessoryView"/> to the underlying
/// <see cref="UITextField"/> or <see cref="UITextView"/>, replacing any existing MAUI accessory view.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Disposable fields are cleaned up in OnDetached which is the correct MAUI PlatformEffect lifecycle.")]
public class MacCatalystKeyboardToolbarEffect : PlatformEffect
{
    private AuroraKeyboardAccessoryView? _accessoryView;
    private UIView? _previousAccessoryView;

    /// <inheritdoc/>
    protected override void OnAttached()
    {
        var inputView = GetInputView();
        if (inputView == null)
        {
            return;
        }

        _previousAccessoryView = GetInputAccessoryView(inputView);

        _accessoryView = new AuroraKeyboardAccessoryView();
        ConfigureAccessoryView(_accessoryView);

        var weakInputView = new WeakReference<UIView>(inputView);
        _accessoryView.DoneAction = () =>
        {
            if (weakInputView.TryGetTarget(out var iv))
            {
                iv.ResignFirstResponder();
            }
        };

        SetInputAccessoryView(inputView, _accessoryView);

        // If the keyboard is already visible, reload immediately so the new accessory view appears
        // without requiring the user to dismiss and re-tap.
        inputView.ReloadInputViews();
    }

    /// <inheritdoc/>
    protected override void OnDetached()
    {
        var inputView = GetInputView();
        if (inputView != null)
        {
            SetInputAccessoryView(inputView, _previousAccessoryView);
            inputView.ReloadInputViews();
        }

        _accessoryView?.Dispose();
        _accessoryView = null;
        _previousAccessoryView = null;
    }

    /// <inheritdoc/>
    protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
    {
        base.OnElementPropertyChanged(args);

        if (_accessoryView == null)
        {
            return;
        }

        switch (args.PropertyName)
        {
            case "ButtonStyle":
            case "ToolbarHeight":
                ConfigureAccessoryView(_accessoryView);
                break;

            case "Title":
                _accessoryView.UpdateTitle(KeyboardToolbar.GetTitle(Element));
                break;

            case "TitleColor":
                _accessoryView.UpdateTitleColor(KeyboardToolbar.GetTitleColor(Element));
                break;

            case "ToolbarBackgroundColor":
                _accessoryView.UpdateBackgroundColor(KeyboardToolbar.GetBackgroundColor(Element));
                break;

            case "ToolbarFontFamily":
            case "ToolbarFontSize":
                _accessoryView.UpdateFont(
                    KeyboardToolbar.GetFontFamily(Element),
                    KeyboardToolbar.GetFontSize(Element));
                break;
        }
    }

    private void ConfigureAccessoryView(AuroraKeyboardAccessoryView view)
    {
        var opts = KeyboardToolbarOptions.Default;
        var style = KeyboardToolbar.GetButtonStyle(Element);
        var height = KeyboardToolbar.GetHeight(Element);
        var title = KeyboardToolbar.GetTitle(Element);
        var titleColor = KeyboardToolbar.GetTitleColor(Element);
        var bg = KeyboardToolbar.GetBackgroundColor(Element);
        var fontFamily = KeyboardToolbar.GetFontFamily(Element);
        var fontSize = KeyboardToolbar.GetFontSize(Element);

        view.Configure(
            style != default ? style : opts.DefaultButtonStyle,
            !string.IsNullOrEmpty(title) ? title : opts.DefaultTitle,
            titleColor ?? opts.DefaultTitleColor,
            bg ?? opts.DefaultBackgroundColor,
            !string.IsNullOrEmpty(fontFamily) ? fontFamily : opts.DefaultFontFamily,
            fontSize > 0 ? fontSize : opts.DefaultFontSize,
            height > 0 ? height : opts.DefaultHeight);
    }

    private UIView? GetInputView()
    {
        if (Control is UIView uiView)
        {
            return uiView;
        }

        return null;
    }

    private static void SetInputAccessoryView(UIView host, UIView? value)
    {
        if (host is UITextField tf)
        {
            tf.InputAccessoryView = value;
        }
        else if (host is UITextView tv)
        {
            tv.InputAccessoryView = value;
        }
    }

    private static UIView? GetInputAccessoryView(UIView host)
    {
        if (host is UITextField tf)
        {
            return tf.InputAccessoryView;
        }

        if (host is UITextView tv)
        {
            return tv.InputAccessoryView;
        }

        return null;
    }
}
