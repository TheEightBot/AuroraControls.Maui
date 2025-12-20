// <copyright file="DecimalFormatConverter.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Globalization;

namespace AuroraControls.Converters;

/// <summary>
/// A value converter that formats numeric values with thousand separators and decimal places.
/// Uses the "N" (numeric) format specifier for culture-aware number formatting.
/// </summary>
/// <example>
/// XAML usage:
/// <code>
/// &lt;!-- For a value of 1234567.89, displays "1,234,567.89" in en-US --&gt;
/// &lt;Entry Text="{Binding Amount, Converter={StaticResource DecimalFormatConverter}}" /&gt;
///
/// &lt;ContentPage.Resources&gt;
///     &lt;aurora:DecimalFormatConverter x:Key="DecimalFormatConverter" DecimalDigits="2" /&gt;
/// &lt;/ContentPage.Resources&gt;
/// </code>
/// </example>
public class DecimalFormatConverter : NumericFormattingConverterBase
{
    /// <summary>
    /// Gets or sets the number of decimal digits to display.
    /// Defaults to 2. Use -1 to use the culture's default.
    /// </summary>
    public int DecimalDigits { get; set; } = 2;

    /// <summary>
    /// Gets or sets a value indicating whether to include thousand separators.
    /// Defaults to true. When false, uses "F" format instead of "N".
    /// </summary>
    public bool UseGroupSeparator { get; set; } = true;

    /// <summary>
    /// Gets or sets a custom decimal separator to use instead of the culture's default.
    /// When null or empty, the culture's default decimal separator is used.
    /// </summary>
    public string? DecimalSeparator { get; set; }

    /// <summary>
    /// Gets or sets a custom group (thousands) separator to use instead of the culture's default.
    /// When null or empty, the culture's default group separator is used.
    /// </summary>
    public string? GroupSeparator { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use the format string from the ConverterParameter.
    /// When true, the ConverterParameter is used as the format string (e.g., "N0", "N3", "F2").
    /// When false, the DecimalDigits and UseGroupSeparator properties are used.
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

        // Build format string based on settings
        var formatSpecifier = UseGroupSeparator ? "N" : "F";
        var format = DecimalDigits >= 0 ? $"{formatSpecifier}{DecimalDigits}" : formatSpecifier;
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

        // Try parsing with number style that includes group separators
        var numberStyle = NumberStyles.Number | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;

        if (double.TryParse(value, numberStyle, numberFormat, out var result))
        {
            return result;
        }

        // Fall back to cleaning the string and parsing
        var cleanedValue = CleanNumericString(value, culture);

        // If we have custom separators, also clean those
        if (!string.IsNullOrEmpty(GroupSeparator))
        {
            cleanedValue = cleanedValue.Replace(GroupSeparator, string.Empty);
        }

        if (double.TryParse(cleanedValue, NumberStyles.Any, numberFormat, out result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Gets the number format info, optionally with custom separators.
    /// </summary>
    private NumberFormatInfo GetNumberFormat(CultureInfo culture)
    {
        var hasCustomDecimalSeparator = !string.IsNullOrEmpty(DecimalSeparator);
        var hasCustomGroupSeparator = !string.IsNullOrEmpty(GroupSeparator);

        if (!hasCustomDecimalSeparator && !hasCustomGroupSeparator)
        {
            return culture.NumberFormat;
        }

        // Clone the number format to avoid modifying the culture's format
        var numberFormat = (NumberFormatInfo)culture.NumberFormat.Clone();

        if (hasCustomDecimalSeparator)
        {
            numberFormat.NumberDecimalSeparator = DecimalSeparator!;
        }

        if (hasCustomGroupSeparator)
        {
            numberFormat.NumberGroupSeparator = GroupSeparator!;
        }

        return numberFormat;
    }
}
