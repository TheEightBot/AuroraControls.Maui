using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
#if ANDROID
using AndroidX.AppCompat.Widget;
#elif IOS
using Foundation;
using UIKit;
#endif

namespace AuroraControls;

public partial class NumericEntryHandler : EntryHandler
{
    public static PropertyMapper NumericEntryMapper =
         new PropertyMapper<NumericEntry, NumericEntryHandler>(Mapper)
         {
             // [nameof(Entry.Text)] = MapNumericEntryText,
         };

    public NumericEntryHandler()
        : base(NumericEntryMapper)
    {
    }

#if ANDROID
    protected override void ConnectHandler(AppCompatEditText platformView)
    {
        base.ConnectHandler(platformView);

        platformView.InputType = Android.Text.InputTypes.NumberFlagDecimal;
    }
#elif IOS
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
        var originalSource = replacementString;
        var originalDest = textField.Text;

        if (string.IsNullOrEmpty(originalDest) && string.IsNullOrEmpty(originalSource))
        {
            return true;
        }

        var final =
            originalDest.Substring(0, (int)range.Location) + replacementString + originalDest.Substring((int)range.Location + (int)range.Length);

        if (string.IsNullOrEmpty(final) || final.Equals(".") || final.Equals("-") || final.Equals("+"))
        {
            return true;
        }

        return double.TryParse(final, out var _);
    }
#endif

    private static void MapNumericEntryText(NumericEntryHandler handler, NumericEntry control)
    {
        var controlText = control.Text;

        if (string.IsNullOrEmpty(controlText) || controlText.Equals('.') || controlText.Equals('-') || controlText.Equals('+'))
        {
            return;
        }

        control.Text = NumericValidationRegex().IsMatch(controlText)
            ? controlText
            : null;

        System.Diagnostics.Debug.WriteLine($"PV: {handler.PlatformView.Text}\t-\tC: {control.Text}");
    }

    // [GeneratedRegex("^-?\\d*(\\.\\d+)?$")]
    [GeneratedRegex("^(?:[+-]?(?:\\d+(?:\\.\\d*)?|\\.\\d+))$")]
    private static partial Regex NumericValidationRegex();
}