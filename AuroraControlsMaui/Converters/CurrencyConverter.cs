// <copyright file="CurrencyConverter.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Globalization;

namespace AuroraControls.Converters;

/// <summary>
/// A value converter that formats numeric values as currency strings.
/// Supports culture-aware formatting and custom currency symbols.
/// </summary>
/// <example>
/// XAML usage:
/// <code>
/// &lt;Entry Text="{Binding Price, Converter={StaticResource CurrencyConverter}}" /&gt;
///
/// &lt;ContentPage.Resources&gt;
///     &lt;aurora:CurrencyConverter x:Key="CurrencyConverter" DecimalDigits="2" /&gt;
/// &lt;/ContentPage.Resources&gt;
/// </code>
/// </example>
public class CurrencyConverter : NumericFormattingConverterBase
{
    /// <summary>
    /// Gets or sets the number of decimal digits to display.
    /// Defaults to 2. Use -1 to use the culture's default.
    /// </summary>
    public int DecimalDigits { get; set; } = 2;

    /// <summary>
    /// Gets or sets a custom currency symbol to use instead of the culture's default.
    /// When null or empty, the culture's default currency symbol is used.
    /// </summary>
    public string? CurrencySymbol { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use the format string from the ConverterParameter.
    /// When true, the ConverterParameter is used as the format string (e.g., "C0", "C3").
    /// When false, the DecimalDigits property is used.
    /// </summary>
    public bool UseParameterAsFormat { get; set; }

    /// <inheritdoc/>
    protected override string FormatValue(double value, object? parameter, CultureInfo culture)
    {
        var numberFormat = GetNumberFormat(culture);

        // Check if parameter should be used as format string
        if (UseParameterAsFormat && parameter is string formatParam && !string.IsNullOrEmpty(formatParam))
        {
            return value.ToString(formatParam, numberFormat);
        }

        // Build format string based on DecimalDigits
        var format = DecimalDigits >= 0 ? $"C{DecimalDigits}" : "C";
        return value.ToString(format, numberFormat);
    }

    /// <inheritdoc/>
    protected override double? ParseValue(string value, object? parameter, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var numberFormat = GetNumberFormat(culture);

        // Try parsing with currency style first
        if (double.TryParse(value, NumberStyles.Currency, numberFormat, out var result))
        {
            return result;
        }

        // Fall back to cleaning the string and parsing
        var cleanedValue = CleanNumericString(value, culture);
        if (double.TryParse(cleanedValue, NumberStyles.Any, numberFormat, out result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Gets the number format info, optionally with a custom currency symbol.
    /// </summary>
    private NumberFormatInfo GetNumberFormat(CultureInfo culture)
    {
        if (string.IsNullOrEmpty(CurrencySymbol))
        {
            return culture.NumberFormat;
        }

        // Clone the number format to avoid modifying the culture's format
        var numberFormat = (NumberFormatInfo)culture.NumberFormat.Clone();
        numberFormat.CurrencySymbol = CurrencySymbol;
        return numberFormat;
    }
}
