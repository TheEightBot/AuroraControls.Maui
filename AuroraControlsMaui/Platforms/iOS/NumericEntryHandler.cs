using System.Drawing;
using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace AuroraControls;

public partial class NumericEntryHandler : EntryHandler, IDisposable
{
    private UIBarButtonItem _doneButton;

    protected override void ConnectHandler(MauiTextField platformView)
    {
        base.ConnectHandler(platformView);

        var toolbar = new UIToolbar(new RectangleF(0, 0, (float)UIScreen.MainScreen.Bounds.Width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };
        _doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done);
        _doneButton.Clicked += DoneButtonOnClicked;

        toolbar.Items = [new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _doneButton,];
        platformView.InputAccessoryView = toolbar;

        platformView.ShouldChangeCharacters += OnShouldChangeCharacters;
    }

    protected override void DisconnectHandler(MauiTextField platformView)
    {
        platformView.ShouldChangeCharacters -= OnShouldChangeCharacters;
        _doneButton.Clicked -= DoneButtonOnClicked;

        PlatformView.InputAccessoryView?.Dispose();
        PlatformView.InputAccessoryView = null;

        base.DisconnectHandler(platformView);
    }

    private void DoneButtonOnClicked(object? sender, EventArgs e)
    {
        this.PlatformView.ResignFirstResponder();
    }

    private bool OnShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
    {
        string originalSource = replacementString;
        string? originalDest = textField.Text;

        if (string.IsNullOrEmpty(originalDest) && string.IsNullOrEmpty(originalSource))
        {
            return true;
        }

        string final =
            originalDest.Substring(0, (int)range.Location) + replacementString + originalDest.Substring((int)range.Location + (int)range.Length);

        return IsValid(final, NumericEntryVirtualView.CultureInfo, NumericEntryVirtualView.ValueType);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _doneButton.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
