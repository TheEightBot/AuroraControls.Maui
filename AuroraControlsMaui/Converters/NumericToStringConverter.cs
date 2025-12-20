// <copyright file="NumericToStringConverter.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Globalization;

namespace AuroraControls.Converters;

/// <summary>
/// A lightweight value converter for simple numeric-to-string conversion.
/// Ideal for scenarios where minimal formatting is needed.
/// </summary>
/// <example>
/// XAML usage:
/// <code>
/// &lt;!-- Simple conversion with default format --&gt;
/// &lt;Entry Text="{Binding Value, Converter={StaticResource NumericToString}}" /&gt;
///
/// &lt;!-- With format parameter --&gt;
/// &lt;Entry Text="{Binding Value, Converter={StaticResource NumericToString}, ConverterParameter='F2'}" /&gt;
///
/// &lt;ContentPage.Resources&gt;
///     &lt;aurora:NumericToStringConverter x:Key="NumericToString" /&gt;
/// &lt;/ContentPage.Resources&gt;
/// </code>
/// </example>
public class NumericToStringConverter : NumericFormattingConverterBase
{
    /// <summary>
    /// Gets or sets the default format string to use when no parameter is provided.
    /// Leave null or empty for general ("G") formatting.
    /// Common values: "F0", "F2", "N0", "N2", "G", "R".
    /// </summary>
    public string? DefaultFormat { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to trim trailing zeros from decimal values.
    /// Defaults to false.
    /// </summary>
    public bool TrimTrailingZeros { get; set; }

    /// <inheritdoc/>
    protected override string FormatValue(double value, object? parameter, CultureInfo culture)
    {
        // Use parameter as format if provided, otherwise use default format
        var format = parameter as string;

        if (string.IsNullOrEmpty(format))
        {
            format = DefaultFormat;
        }

        string formattedValue;

        if (string.IsNullOrEmpty(format))
        {
            // Use general format
            formattedValue = value.ToString(culture);
        }
        else
        {
            formattedValue = value.ToString(format, culture);
        }

        // Optionally trim trailing zeros
        if (TrimTrailingZeros && formattedValue.Contains(culture.NumberFormat.NumberDecimalSeparator))
        {
            formattedValue = formattedValue.TrimEnd('0').TrimEnd(
                culture.NumberFormat.NumberDecimalSeparator.ToCharArray());
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

        // Try parsing with standard number styles
        if (double.TryParse(value, NumberStyles.Any, culture, out var result))
        {
            return result;
        }

        // Try cleaning and parsing
        var cleanedValue = CleanNumericString(value, culture);
        if (double.TryParse(cleanedValue, NumberStyles.Any, culture, out result))
        {
            return result;
        }

        return null;
    }
}
