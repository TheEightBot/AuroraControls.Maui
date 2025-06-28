using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace AuroraControls;

public partial class NumericEntryHandler : EntryHandler
{
    protected override void ConnectHandler(MauiTextField platformView)
    {
        base.ConnectHandler(platformView);

        platformView.ShouldChangeCharacters += OnShouldChangeCharacters;
    }

    protected override void DisconnectHandler(MauiTextField platformView)
    {
        platformView.ShouldChangeCharacters -= OnShouldChangeCharacters;

        base.DisconnectHandler(platformView);
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
}
