// <copyright file="CustomNumericFormatConverter.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Globalization;
using System.Text.RegularExpressions;

namespace AuroraControls.Converters;

/// <summary>
/// A value converter that formats numeric values using custom format strings.
/// Supports standard .NET numeric format strings including "0" and "#" placeholders.
/// </summary>
/// <example>
/// XAML usage:
/// <code>
/// &lt;!-- Custom format with currency prefix --&gt;
/// &lt;Entry Text="{Binding Price, Converter={StaticResource CustomFormat}}" /&gt;
///
/// &lt;ContentPage.Resources&gt;
///     &lt;aurora:CustomNumericFormatConverter x:Key="CustomFormat" Format="$#,##0.00" /&gt;
///     &lt;aurora:CustomNumericFormatConverter x:Key="PercentFormat" Format="00.00%" /&gt;
///     &lt;aurora:CustomNumericFormatConverter x:Key="PaddedFormat" Format="00000.00" /&gt;
/// &lt;/ContentPage.Resources&gt;
/// </code>
/// </example>
/// <remarks>
/// Format specifiers:
/// <list type="bullet">
/// <item><description>"0" - Zero placeholder: displays the digit or zero if no digit is present</description></item>
/// <item><description>"#" - Digit placeholder: displays the digit or nothing if no digit is present</description></item>
/// <item><description>"." - Decimal point</description></item>
/// <item><description>"," - Group separator (thousands)</description></item>
/// <item><description>"%" - Percent placeholder (multiplies value by 100)</description></item>
/// </list>
/// </remarks>
public partial class CustomNumericFormatConverter : NumericFormattingConverterBase
{
    /// <summary>
    /// Gets or sets the custom format string.
    /// Supports standard .NET numeric format specifiers.
    /// Examples: "$#,##0.00", "00.00%", "00000.00", "#.00##".
    /// </summary>
    public string Format { get; set; } = "#,##0.00";

    /// <summary>
    /// Gets or sets a value indicating whether the format string contains a percent specifier
    /// that should be handled specially during parsing.
    /// When true, the parsed value will be divided by 100 during ConvertBack.
    /// </summary>
    public bool IsPercentFormat { get; set; }

    /// <summary>
    /// Gets or sets the prefix to prepend to the formatted value.
    /// This is applied after formatting and stripped before parsing.
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// Gets or sets the suffix to append to the formatted value.
    /// This is applied after formatting and stripped before parsing.
    /// </summary>
    public string? Suffix { get; set; }

    /// <inheritdoc/>
    protected override string FormatValue(double value, object? parameter, CultureInfo culture)
    {
        // Use parameter as format if provided
        var format = parameter as string ?? Format;

        if (string.IsNullOrEmpty(format))
        {
            format = "#,##0.00";
        }

        var formattedValue = value.ToString(format, culture);

        // Apply prefix and suffix
        if (!string.IsNullOrEmpty(Prefix))
        {
            formattedValue = Prefix + formattedValue;
        }

        if (!string.IsNullOrEmpty(Suffix))
        {
            formattedValue = formattedValue + Suffix;
        }

        return formattedValue;
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

        // Detect if the format or value contains percent
        var format = parameter as string ?? Format;
        var hasPercent = IsPercentFormat ||
                         format.Contains('%') ||
                         cleanedValue.Contains(culture.NumberFormat.PercentSymbol);

        // Clean the value for parsing
        cleanedValue = CleanNumericString(cleanedValue, culture);

        // Try standard parsing
        if (double.TryParse(cleanedValue, NumberStyles.Any, culture, out var result))
        {
            // If percent format, divide by 100 since the display value was multiplied
            if (hasPercent)
            {
                result /= 100.0;
            }

            return result;
        }

        return null;
    }
}
