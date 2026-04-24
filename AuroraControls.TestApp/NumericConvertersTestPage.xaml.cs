// <copyright file="NumericConvertersTestPage.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Globalization;
using AuroraControls.Converters;

namespace AuroraControls.TestApp;

/// <summary>
/// Test page demonstrating all numeric value converters.
/// </summary>
public partial class NumericConvertersTestPage : ContentPage
{
    private readonly CurrencyConverter _currencyConverter = new() { DecimalDigits = 2 };
    private readonly PercentConverter _percentConverter = new() { DecimalDigits = 0 };
    private readonly DecimalFormatConverter _decimalConverter = new() { DecimalDigits = 2 };

    public NumericConvertersTestPage()
    {
        InitializeComponent();
    }

    private void OnTestValueChanged(object? sender, TextChangedEventArgs e)
    {
        if (double.TryParse(e.NewTextValue, NumberStyles.Any, CultureInfo.CurrentUICulture, out var value))
        {
            ParsedValueLabel.Text = $"Parsed: {value}";
            ParsedValueLabel.TextColor = Colors.Green;
        }
        else
        {
            ParsedValueLabel.Text = $"Could not parse: {e.NewTextValue}";
            ParsedValueLabel.TextColor = Colors.Red;
        }
    }

    private void OnTestNullClicked(object? sender, EventArgs e)
    {
        var result = _currencyConverter.Convert(null, typeof(string), null, CultureInfo.CurrentUICulture);
        EdgeCaseResultLabel.Text = $"Null → \"{result}\" (expected: empty or placeholder)";
        EdgeCaseResultLabel.TextColor = string.IsNullOrEmpty(result?.ToString()) ? Colors.Green : Colors.Orange;
    }

    private void OnTestLargeNumberClicked(object? sender, EventArgs e)
    {
        var largeNumber = 1234567890123.456;
        var result = _currencyConverter.Convert(largeNumber, typeof(string), null, CultureInfo.CurrentUICulture);
        EdgeCaseResultLabel.Text = $"Large: {largeNumber} → \"{result}\"";
        EdgeCaseResultLabel.TextColor = Colors.Green;
    }

    private void OnTestNegativeClicked(object? sender, EventArgs e)
    {
        var negativeNumber = -1234.56;
        var currencyResult = _currencyConverter.Convert(negativeNumber, typeof(string), null, CultureInfo.CurrentUICulture);
        var percentResult = _percentConverter.Convert(negativeNumber, typeof(string), null, CultureInfo.CurrentUICulture);
        EdgeCaseResultLabel.Text = $"Negative: Currency=\"{currencyResult}\", Percent=\"{percentResult}\"";
        EdgeCaseResultLabel.TextColor = Colors.Green;
    }
}
