// <copyright file="NumericFormatExtension.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Globalization;

namespace AuroraControls.Converters;

/// <summary>
/// A markup extension that provides easy access to common numeric format converters.
/// </summary>
/// <example>
/// XAML usage:
/// <code>
/// &lt;!-- Currency format --&gt;
/// &lt;Entry Text="{Binding Price, Converter={aurora:NumericFormat Type=Currency}}" /&gt;
///
/// &lt;!-- Percent format --&gt;
/// &lt;Entry Text="{Binding Discount, Converter={aurora:NumericFormat Type=Percent, DecimalDigits=1}}" /&gt;
///
/// &lt;!-- Decimal format --&gt;
/// &lt;Entry Text="{Binding Amount, Converter={aurora:NumericFormat Type=Decimal, DecimalDigits=0}}" /&gt;
///
/// &lt;!-- Custom format --&gt;
/// &lt;Entry Text="{Binding Value, Converter={aurora:NumericFormat Format='#,##0.00'}}" /&gt;
/// </code>
/// </example>
[ContentProperty(nameof(Format))]
public class NumericFormatExtension : IMarkupExtension<IValueConverter>
{
    /// <summary>
    /// Gets or sets the type of numeric format to use.
    /// </summary>
    public NumericFormatType Type { get; set; } = NumericFormatType.Decimal;

    /// <summary>
    /// Gets or sets the number of decimal digits.
    /// Defaults to 2 for Currency and Decimal, 0 for Percent.
    /// </summary>
    public int DecimalDigits { get; set; } = -1;

    /// <summary>
    /// Gets or sets a custom format string.
    /// When specified, overrides the Type property.
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Gets or sets the culture to use for formatting.
    /// When null, uses the current UI culture.
    /// </summary>
    public CultureInfo? Culture { get; set; }

    /// <summary>
    /// Gets or sets the text to display for null values.
    /// </summary>
    public string NullPlaceholder { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the percent display mode (only applies when Type is Percent).
    /// </summary>
    public PercentDisplayMode PercentDisplayMode { get; set; } = PercentDisplayMode.Compute;

    /// <inheritdoc/>
    public IValueConverter ProvideValue(IServiceProvider serviceProvider)
    {
        // If custom format is specified, use CustomNumericFormatConverter
        if (!string.IsNullOrEmpty(Format))
        {
            return new CustomNumericFormatConverter
            {
                Format = Format,
                Culture = Culture,
                NullPlaceholder = NullPlaceholder,
            };
        }

        // Otherwise, create the appropriate converter based on Type
        return Type switch
        {
            NumericFormatType.Currency => CreateCurrencyConverter(),
            NumericFormatType.Percent => CreatePercentConverter(),
            NumericFormatType.Decimal => CreateDecimalConverter(),
            NumericFormatType.Integer => CreateIntegerConverter(),
            _ => CreateDecimalConverter(),
        };
    }

    /// <inheritdoc/>
    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }

    private CurrencyConverter CreateCurrencyConverter()
    {
        return new CurrencyConverter
        {
            DecimalDigits = DecimalDigits >= 0 ? DecimalDigits : 2,
            Culture = Culture,
            NullPlaceholder = NullPlaceholder,
        };
    }

    private PercentConverter CreatePercentConverter()
    {
        return new PercentConverter
        {
            DecimalDigits = DecimalDigits >= 0 ? DecimalDigits : 0,
            DisplayMode = PercentDisplayMode,
            Culture = Culture,
            NullPlaceholder = NullPlaceholder,
        };
    }

    private DecimalFormatConverter CreateDecimalConverter()
    {
        return new DecimalFormatConverter
        {
            DecimalDigits = DecimalDigits >= 0 ? DecimalDigits : 2,
            UseGroupSeparator = true,
            Culture = Culture,
            NullPlaceholder = NullPlaceholder,
        };
    }

    private DecimalFormatConverter CreateIntegerConverter()
    {
        return new DecimalFormatConverter
        {
            DecimalDigits = 0,
            UseGroupSeparator = true,
            Culture = Culture,
            NullPlaceholder = NullPlaceholder,
        };
    }
}

/// <summary>
/// Specifies the type of numeric format.
/// </summary>
public enum NumericFormatType
{
    /// <summary>
    /// Format as currency (e.g., $1,234.56).
    /// </summary>
    Currency,

    /// <summary>
    /// Format as percentage (e.g., 50%).
    /// </summary>
    Percent,

    /// <summary>
    /// Format as decimal number with group separators (e.g., 1,234.56).
    /// </summary>
    Decimal,

    /// <summary>
    /// Format as integer with group separators (e.g., 1,234).
    /// </summary>
    Integer,
}
