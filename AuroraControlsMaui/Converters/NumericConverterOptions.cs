// <copyright file="NumericConverterOptions.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Globalization;

namespace AuroraControls.Converters;

/// <summary>
/// Specifies how percentage values should be displayed.
/// </summary>
public enum PercentDisplayMode
{
    /// <summary>
    /// Display the actual value with a percent symbol (e.g., 1000 -> "1000%").
    /// The value is shown as-is without multiplication.
    /// </summary>
    Value,

    /// <summary>
    /// Compute the percentage by multiplying by 100 (e.g., 0.5 -> "50%").
    /// This is the standard .NET behavior for percent format strings.
    /// </summary>
    Compute,
}

/// <summary>
/// Specifies the rounding strategy for numeric formatting.
/// </summary>
public enum NumericRoundingMode
{
    /// <summary>
    /// Round to the nearest value, with midpoint values rounded to the nearest even number.
    /// This is the default .NET rounding behavior.
    /// </summary>
    ToEven,

    /// <summary>
    /// Round to the nearest value, with midpoint values rounded away from zero.
    /// </summary>
    AwayFromZero,

    /// <summary>
    /// Round toward positive infinity.
    /// </summary>
    Ceiling,

    /// <summary>
    /// Round toward negative infinity.
    /// </summary>
    Floor,

    /// <summary>
    /// Truncate the value (round toward zero).
    /// </summary>
    Truncate,
}

/// <summary>
/// Provides shared configuration options for numeric value converters.
/// This class can be used to define common settings that are shared across multiple converters.
/// </summary>
public class NumericConverterOptions
{
    /// <summary>
    /// Gets or sets the culture to use for formatting and parsing.
    /// When null, the current UI culture is used.
    /// </summary>
    public CultureInfo? Culture { get; set; }

    /// <summary>
    /// Gets or sets the text to display when the value is null.
    /// Defaults to an empty string.
    /// </summary>
    public string NullPlaceholder { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the fallback value to return when conversion fails.
    /// When null, the converter returns the original value on failure.
    /// </summary>
    public object? FallbackValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to throw an exception when conversion fails.
    /// When false (default), the converter returns the fallback value on failure.
    /// </summary>
    public bool ThrowOnError { get; set; }

    /// <summary>
    /// Gets or sets the rounding mode to use when limiting decimal places.
    /// Defaults to <see cref="NumericRoundingMode.ToEven"/>.
    /// </summary>
    public NumericRoundingMode RoundingMode { get; set; } = NumericRoundingMode.ToEven;

    /// <summary>
    /// Gets the effective culture to use, falling back to CurrentUICulture if not specified.
    /// </summary>
    /// <returns>The culture to use for formatting and parsing.</returns>
    public CultureInfo GetEffectiveCulture() => Culture ?? CultureInfo.CurrentUICulture;

    /// <summary>
    /// Gets the <see cref="MidpointRounding"/> equivalent for the current <see cref="RoundingMode"/>.
    /// </summary>
    /// <returns>The midpoint rounding mode.</returns>
    public MidpointRounding GetMidpointRounding() => RoundingMode switch
    {
        NumericRoundingMode.ToEven => MidpointRounding.ToEven,
        NumericRoundingMode.AwayFromZero => MidpointRounding.AwayFromZero,
        NumericRoundingMode.Ceiling => MidpointRounding.ToPositiveInfinity,
        NumericRoundingMode.Floor => MidpointRounding.ToNegativeInfinity,
        NumericRoundingMode.Truncate => MidpointRounding.ToZero,
        _ => MidpointRounding.ToEven,
    };

    /// <summary>
    /// Creates a copy of this options instance.
    /// </summary>
    /// <returns>A new instance with the same values.</returns>
    public NumericConverterOptions Clone() => new()
    {
        Culture = Culture,
        NullPlaceholder = NullPlaceholder,
        FallbackValue = FallbackValue,
        ThrowOnError = ThrowOnError,
        RoundingMode = RoundingMode,
    };
}
