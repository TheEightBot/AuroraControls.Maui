// <copyright file="PercentConverter.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Globalization;

namespace AuroraControls.Converters;

/// <summary>
/// A value converter that formats numeric values as percentage strings.
/// Supports two display modes: Value (show as-is) and Compute (multiply by 100).
/// </summary>
/// <example>
/// XAML usage:
/// <code>
/// &lt;!-- For a value of 0.5, displays "50%" --&gt;
/// &lt;Entry Text="{Binding Discount, Converter={StaticResource PercentConverter}}" /&gt;
///
/// &lt;!-- For a value of 50, displays "50%" (using Value mode) --&gt;
/// &lt;Entry Text="{Binding Discount, Converter={StaticResource PercentValueConverter}}" /&gt;
///
/// &lt;ContentPage.Resources&gt;
///     &lt;aurora:PercentConverter x:Key="PercentConverter" DisplayMode="Compute" DecimalDigits="0" /&gt;
///     &lt;aurora:PercentConverter x:Key="PercentValueConverter" DisplayMode="Value" DecimalDigits="0" /&gt;
/// &lt;/ContentPage.Resources&gt;
/// </code>
/// </example>
public class PercentConverter : NumericFormattingConverterBase
{
    /// <summary>
    /// Gets or sets the number of decimal digits to display.
    /// Defaults to 0. Use -1 to use the culture's default.
    /// </summary>
    public int DecimalDigits { get; set; }

    /// <summary>
    /// Gets or sets the display mode for percentage values.
    /// <see cref="PercentDisplayMode.Compute"/> multiplies the value by 100 (e.g., 0.5 -> "50%").
    /// <see cref="PercentDisplayMode.Value"/> displays the value as-is with a percent symbol (e.g., 50 -> "50%").
    /// Defaults to <see cref="PercentDisplayMode.Compute"/>.
    /// </summary>
    public PercentDisplayMode DisplayMode { get; set; } = PercentDisplayMode.Compute;

    /// <summary>
    /// Gets or sets a custom percent symbol to use instead of the culture's default.
    /// When null or empty, the culture's default percent symbol is used.
    /// </summary>
    public string? PercentSymbol { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use the format string from the ConverterParameter.
    /// When true, the ConverterParameter is used as the format string (e.g., "P0", "P3").
    /// When false, the DecimalDigits property is used.
    /// </summary>
    public bool UseParameterAsFormat { get; set; }

    /// <inheritdoc/>
    protected override string FormatValue(double value, object? parameter, CultureInfo culture)
    {
        var numberFormat = GetNumberFormat(culture);

        // Adjust value based on display mode
        var displayValue = DisplayMode == PercentDisplayMode.Value
            ? value / 100.0 // Divide by 100 because .NET's P format will multiply by 100
            : value;

        // Check if parameter should be used as format string
        if (UseParameterAsFormat && parameter is string formatParam && !string.IsNullOrEmpty(formatParam))
        {
            return displayValue.ToString(formatParam, numberFormat);
        }

        // Build format string based on DecimalDigits
        var format = DecimalDigits >= 0 ? $"P{DecimalDigits}" : "P";
        return displayValue.ToString(format, numberFormat);
    }

    /// <inheritdoc/>
    protected override double? ParseValue(string value, object? parameter, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var numberFormat = GetNumberFormat(culture);

        // Remove the percent symbol for parsing
        var cleanedValue = value.Trim();
        var percentSymbol = string.IsNullOrEmpty(PercentSymbol)
            ? numberFormat.PercentSymbol
            : PercentSymbol;

        cleanedValue = cleanedValue.Replace(percentSymbol, string.Empty).Trim();

        // Also try removing the culture's default percent symbol if custom one was set
        if (!string.IsNullOrEmpty(PercentSymbol))
        {
            cleanedValue = cleanedValue.Replace(culture.NumberFormat.PercentSymbol, string.Empty).Trim();
        }

        // Remove group separators
        cleanedValue = cleanedValue.Replace(numberFormat.PercentGroupSeparator, string.Empty);

        if (double.TryParse(cleanedValue, NumberStyles.Any, numberFormat, out var result))
        {
            // Adjust result based on display mode
            // If Compute mode, the displayed value was already multiplied by 100, so divide by 100
            // If Value mode, the displayed value was the actual value, so keep as-is
            return DisplayMode == PercentDisplayMode.Compute
                ? result / 100.0
                : result;
        }

        return null;
    }

    /// <summary>
    /// Gets the number format info, optionally with a custom percent symbol.
    /// </summary>
    private NumberFormatInfo GetNumberFormat(CultureInfo culture)
    {
        if (string.IsNullOrEmpty(PercentSymbol))
        {
            return culture.NumberFormat;
        }

        // Clone the number format to avoid modifying the culture's format
        var numberFormat = (NumberFormatInfo)culture.NumberFormat.Clone();
        numberFormat.PercentSymbol = PercentSymbol;
        return numberFormat;
    }
}