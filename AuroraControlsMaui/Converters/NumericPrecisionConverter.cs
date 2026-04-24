// <copyright file="NumericPrecisionConverter.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Globalization;
using System.Text;

namespace AuroraControls.Converters;

/// <summary>
/// A value converter that formats numeric values with precise control over
/// minimum and maximum integer and fractional digits.
/// </summary>
/// <example>
/// XAML usage:
/// <code>
/// &lt;!-- Display with exactly 5 integer digits and 2-4 decimal places --&gt;
/// &lt;Entry Text="{Binding Value, Converter={StaticResource PrecisionConverter}}" /&gt;
///
/// &lt;ContentPage.Resources&gt;
///     &lt;aurora:NumericPrecisionConverter x:Key="PrecisionConverter"
///         MinimumIntegerDigits="5"
///         MinimumFractionDigits="2"
///         MaximumFractionDigits="4" /&gt;
/// &lt;/ContentPage.Resources&gt;
/// </code>
/// </example>
public class NumericPrecisionConverter : NumericFormattingConverterBase
{
    /// <summary>
    /// Gets or sets the minimum number of integer digits to display.
    /// Values with fewer integer digits will be zero-padded on the left.
    /// Defaults to 1.
    /// </summary>
    public int MinimumIntegerDigits { get; set; } = 1;

    /// <summary>
    /// Gets or sets the minimum number of fractional (decimal) digits to display.
    /// Values with fewer decimal digits will be zero-padded on the right.
    /// Defaults to 0.
    /// </summary>
    public int MinimumFractionDigits { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of fractional (decimal) digits to display.
    /// Values with more decimal digits will be rounded.
    /// Defaults to 2.
    /// </summary>
    public int MaximumFractionDigits { get; set; } = 2;

    /// <summary>
    /// Gets or sets a value indicating whether to include thousand separators.
    /// Defaults to false for precision formatting.
    /// </summary>
    public bool UseGroupSeparator { get; set; }

    /// <summary>
    /// Gets or sets an optional prefix to prepend (e.g., "$", "€").
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// Gets or sets an optional suffix to append (e.g., "%", " units").
    /// </summary>
    public string? Suffix { get; set; }

    /// <inheritdoc/>
    protected override string FormatValue(double value, object? parameter, CultureInfo culture)
    {
        // Build custom format string based on precision settings
        var format = BuildFormatString();

        // Round the value to the maximum fraction digits first
        var roundedValue = Math.Round(value, MaximumFractionDigits, MidpointRounding);

        var numberFormat = culture.NumberFormat;

        // Format the value
        string formattedValue;

        if (UseGroupSeparator)
        {
            // Use a custom number format that includes grouping
            var customFormat = (NumberFormatInfo)numberFormat.Clone();
            formattedValue = roundedValue.ToString(format, customFormat);
        }
        else
        {
            formattedValue = roundedValue.ToString(format, numberFormat);
        }

        // Apply prefix and suffix
        var result = new StringBuilder();

        if (!string.IsNullOrEmpty(Prefix))
        {
            result.Append(Prefix);
        }

        result.Append(formattedValue);

        if (!string.IsNullOrEmpty(Suffix))
        {
            result.Append(Suffix);
        }

        return result.ToString();
    }

    /// <inheritdoc/>
    protected override double? ParseValue(string value, object? parameter, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var cleanedValue = value.Trim();

        // Remove prefix if present
        if (!string.IsNullOrEmpty(Prefix) && cleanedValue.StartsWith(Prefix, StringComparison.Ordinal))
        {
            cleanedValue = cleanedValue.Substring(Prefix.Length);
        }

        // Remove suffix if present
        if (!string.IsNullOrEmpty(Suffix) && cleanedValue.EndsWith(Suffix, StringComparison.Ordinal))
        {
            cleanedValue = cleanedValue.Substring(0, cleanedValue.Length - Suffix.Length);
        }

        // Clean the value
        cleanedValue = CleanNumericString(cleanedValue, culture);

        if (double.TryParse(cleanedValue, NumberStyles.Any, culture, out var result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Builds a custom format string based on the precision settings.
    /// </summary>
    private string BuildFormatString()
    {
        var format = new StringBuilder();

        // Integer part: use "0" for minimum digits
        if (UseGroupSeparator && MinimumIntegerDigits > 0)
        {
            // With grouping, we need to specify the pattern differently
            // e.g., "#,##0" for grouping with at least 1 digit
            var integerPart = new string('0', Math.Max(1, MinimumIntegerDigits));
            if (MinimumIntegerDigits > 3)
            {
                // Insert grouping separators
                var insertPosition = integerPart.Length - 3;
                while (insertPosition > 0)
                {
                    integerPart = integerPart.Insert(insertPosition, ",");
                    insertPosition -= 3;
                }
            }
            else
            {
                integerPart = "#," + integerPart;
            }

            format.Append(integerPart);
        }
        else
        {
            format.Append(new string('0', Math.Max(1, MinimumIntegerDigits)));
        }

        // Fractional part
        if (MaximumFractionDigits > 0)
        {
            format.Append('.');

            // Minimum fraction digits use "0"
            if (MinimumFractionDigits > 0)
            {
                format.Append(new string('0', MinimumFractionDigits));
            }

            // Additional optional digits use "#"
            var optionalDigits = MaximumFractionDigits - MinimumFractionDigits;
            if (optionalDigits > 0)
            {
                format.Append(new string('#', optionalDigits));
            }
        }
        else if (MinimumFractionDigits > 0)
        {
            // Edge case: min > max, use min
            format.Append('.').Append(new string('0', MinimumFractionDigits));
        }

        return format.ToString();
    }
}
