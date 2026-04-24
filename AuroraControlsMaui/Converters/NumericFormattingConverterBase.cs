// <copyright file="NumericFormattingConverterBase.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Globalization;

namespace AuroraControls.Converters;

/// <summary>
/// Abstract base class for numeric formatting value converters.
/// Provides common functionality for type conversion, null handling, culture support, and error handling.
/// </summary>
public abstract class NumericFormattingConverterBase : IValueConverter
{
    private CultureInfo? _culture;

    /// <summary>
    /// Gets or sets the culture to use for formatting and parsing.
    /// When null, the current UI culture is used.
    /// </summary>
    public CultureInfo? Culture
    {
        get => _culture;
        set => _culture = value;
    }

    /// <summary>
    /// Gets or sets the text to display when the value is null or empty.
    /// Defaults to an empty string.
    /// </summary>
    public string NullPlaceholder { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the fallback value to return when conversion fails.
    /// When null, the converter returns the original value on failure for Convert,
    /// or 0 for ConvertBack.
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
    /// Gets the effective culture to use for formatting and parsing.
    /// </summary>
    protected CultureInfo EffectiveCulture => _culture ?? CultureInfo.CurrentUICulture;

    /// <summary>
    /// Gets the <see cref="MidpointRounding"/> equivalent for the current <see cref="RoundingMode"/>.
    /// </summary>
    protected MidpointRounding MidpointRounding => RoundingMode switch
    {
        NumericRoundingMode.ToEven => MidpointRounding.ToEven,
        NumericRoundingMode.AwayFromZero => MidpointRounding.AwayFromZero,
        NumericRoundingMode.Ceiling => MidpointRounding.ToPositiveInfinity,
        NumericRoundingMode.Floor => MidpointRounding.ToNegativeInfinity,
        NumericRoundingMode.Truncate => MidpointRounding.ToZero,
        _ => MidpointRounding.ToEven,
    };

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            // Use provided culture parameter if available, otherwise use configured culture
            var effectiveCulture = GetEffectiveCulture(culture);

            // Handle null values
            if (value is null)
            {
                return NullPlaceholder;
            }

            // Try to convert the value to a double
            var numericValue = ToDouble(value);
            if (numericValue is null)
            {
                return HandleConvertError(value, $"Unable to convert value of type {value.GetType().Name} to a numeric type.");
            }

            // Perform the actual formatting
            return FormatValue(numericValue.Value, parameter, effectiveCulture);
        }
        catch (Exception ex) when (!ThrowOnError)
        {
            return HandleConvertError(value, ex.Message);
        }
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            // Use provided culture parameter if available, otherwise use configured culture
            var effectiveCulture = GetEffectiveCulture(culture);

            // Handle null or empty string values
            if (value is null || (value is string s && string.IsNullOrWhiteSpace(s)))
            {
                return GetTypedDefaultValue(targetType);
            }

            var stringValue = value.ToString();
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return GetTypedDefaultValue(targetType);
            }

            // Perform the actual parsing
            var parsedValue = ParseValue(stringValue, parameter, effectiveCulture);
            if (parsedValue is null)
            {
                return HandleConvertBackError(value, targetType, $"Unable to parse '{stringValue}' as a numeric value.");
            }

            // Convert to the target type
            return ConvertToTargetType(parsedValue.Value, targetType);
        }
        catch (Exception ex) when (!ThrowOnError)
        {
            return HandleConvertBackError(value, targetType, ex.Message);
        }
    }

    /// <summary>
    /// Formats a numeric value to a string. Override this method to implement custom formatting.
    /// </summary>
    /// <param name="value">The numeric value to format.</param>
    /// <param name="parameter">The converter parameter.</param>
    /// <param name="culture">The culture to use for formatting.</param>
    /// <returns>The formatted string.</returns>
    protected abstract string FormatValue(double value, object? parameter, CultureInfo culture);

    /// <summary>
    /// Parses a string value back to a numeric value. Override this method to implement custom parsing.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <param name="parameter">The converter parameter.</param>
    /// <param name="culture">The culture to use for parsing.</param>
    /// <returns>The parsed numeric value, or null if parsing fails.</returns>
    protected abstract double? ParseValue(string value, object? parameter, CultureInfo culture);

    /// <summary>
    /// Attempts to convert an object to a nullable double.
    /// Supports double, decimal, float, int, long, short, byte, and their nullable counterparts.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The double value, or null if conversion fails.</returns>
    protected virtual double? ToDouble(object? value)
    {
        return value switch
        {
            null => null,
            double d => d,
            decimal dec => (double)dec,
            float f => f,
            int i => i,
            long l => l,
            short s => s,
            byte b => b,
            uint ui => ui,
            ulong ul => ul,
            ushort us => us,
            sbyte sb => sb,
            string str when double.TryParse(str, NumberStyles.Any, EffectiveCulture, out var parsed) => parsed,
            IConvertible convertible => TryConvertToDouble(convertible),
            _ => null,
        };
    }

    /// <summary>
    /// Attempts to convert an object to a nullable decimal.
    /// Provides higher precision for financial calculations.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The decimal value, or null if conversion fails.</returns>
    protected virtual decimal? ToDecimal(object? value)
    {
        return value switch
        {
            null => null,
            decimal dec => dec,
            double d => (decimal)d,
            float f => (decimal)f,
            int i => i,
            long l => l,
            short s => s,
            byte b => b,
            uint ui => ui,
            ulong ul => ul,
            ushort us => us,
            sbyte sb => sb,
            string str when decimal.TryParse(str, NumberStyles.Any, EffectiveCulture, out var parsed) => parsed,
            IConvertible convertible => TryConvertToDecimal(convertible),
            _ => null,
        };
    }

    /// <summary>
    /// Attempts to parse a numeric string, handling common formatting characters.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="culture">The culture to use for parsing.</param>
    /// <param name="result">The parsed result.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    protected virtual bool TryParseNumeric(string? value, CultureInfo culture, out double result)
    {
        result = 0;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        // First, try standard parsing
        if (double.TryParse(value, NumberStyles.Any, culture, out result))
        {
            return true;
        }

        // Try cleaning the string and parsing again
        var cleanedValue = CleanNumericString(value, culture);
        return double.TryParse(cleanedValue, NumberStyles.Any, culture, out result);
    }

    /// <summary>
    /// Cleans a numeric string by removing common formatting characters while preserving the value.
    /// </summary>
    /// <param name="value">The string to clean.</param>
    /// <param name="culture">The culture to use for determining formatting characters.</param>
    /// <returns>The cleaned string.</returns>
    protected virtual string CleanNumericString(string value, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var numberFormat = culture.NumberFormat;
        var result = value.Trim();

        // Remove currency symbol
        result = result.Replace(numberFormat.CurrencySymbol, string.Empty);

        // Remove percent symbol
        result = result.Replace(numberFormat.PercentSymbol, string.Empty);

        // Remove group separators (thousands)
        result = result.Replace(numberFormat.NumberGroupSeparator, string.Empty);
        result = result.Replace(numberFormat.CurrencyGroupSeparator, string.Empty);
        result = result.Replace(numberFormat.PercentGroupSeparator, string.Empty);

        // Handle parentheses for negative numbers (common in accounting)
        if (result.StartsWith("(") && result.EndsWith(")"))
        {
            result = "-" + result.Substring(1, result.Length - 2);
        }

        return result.Trim();
    }

    /// <summary>
    /// Converts a double value to the specified target type.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="targetType">The target type.</param>
    /// <returns>The converted value.</returns>
    protected virtual object ConvertToTargetType(double value, Type targetType)
    {
        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        return underlyingType switch
        {
            Type t when t == typeof(double) => value,
            Type t when t == typeof(decimal) => (decimal)value,
            Type t when t == typeof(float) => (float)value,
            Type t when t == typeof(int) => (int)Math.Round(value, MidpointRounding),
            Type t when t == typeof(long) => (long)Math.Round(value, MidpointRounding),
            Type t when t == typeof(short) => (short)Math.Round(value, MidpointRounding),
            Type t when t == typeof(byte) => (byte)Math.Round(value, MidpointRounding),
            Type t when t == typeof(uint) => (uint)Math.Round(value, MidpointRounding),
            Type t when t == typeof(ulong) => (ulong)Math.Round(value, MidpointRounding),
            Type t when t == typeof(ushort) => (ushort)Math.Round(value, MidpointRounding),
            Type t when t == typeof(sbyte) => (sbyte)Math.Round(value, MidpointRounding),
            _ => value,
        };
    }

    /// <summary>
    /// Gets the default value for a given type.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <returns>The default value for the type.</returns>
    protected virtual object? GetTypedDefaultValue(Type targetType)
    {
        // Handle nullable types - return null
        if (Nullable.GetUnderlyingType(targetType) is not null)
        {
            return null;
        }

        var underlyingType = targetType;

        return underlyingType switch
        {
            Type t when t == typeof(double) => 0.0,
            Type t when t == typeof(decimal) => 0m,
            Type t when t == typeof(float) => 0f,
            Type t when t == typeof(int) => 0,
            Type t when t == typeof(long) => 0L,
            Type t when t == typeof(short) => (short)0,
            Type t when t == typeof(byte) => (byte)0,
            Type t when t == typeof(uint) => 0u,
            Type t when t == typeof(ulong) => 0ul,
            Type t when t == typeof(ushort) => (ushort)0,
            Type t when t == typeof(sbyte) => (sbyte)0,
            _ => null,
        };
    }

    /// <summary>
    /// Gets the effective culture to use, considering both the configured culture and the parameter.
    /// </summary>
    /// <param name="parameterCulture">The culture passed as a parameter.</param>
    /// <returns>The effective culture.</returns>
    protected CultureInfo GetEffectiveCulture(CultureInfo? parameterCulture)
    {
        // Priority: Configured Culture > Parameter Culture > Current UI Culture
        return _culture ?? parameterCulture ?? CultureInfo.CurrentUICulture;
    }

    /// <summary>
    /// Handles an error during Convert operation.
    /// </summary>
    private object? HandleConvertError(object? originalValue, string message)
    {
        if (ThrowOnError)
        {
            throw new InvalidOperationException(message);
        }

        return FallbackValue ?? originalValue?.ToString() ?? NullPlaceholder;
    }

    /// <summary>
    /// Handles an error during ConvertBack operation.
    /// </summary>
    private object? HandleConvertBackError(object? originalValue, Type targetType, string message)
    {
        if (ThrowOnError)
        {
            throw new InvalidOperationException(message);
        }

        return FallbackValue ?? GetTypedDefaultValue(targetType);
    }

    /// <summary>
    /// Tries to convert an IConvertible to double.
    /// </summary>
    private static double? TryConvertToDouble(IConvertible convertible)
    {
        try
        {
            return convertible.ToDouble(CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Tries to convert an IConvertible to decimal.
    /// </summary>
    private static decimal? TryConvertToDecimal(IConvertible convertible)
    {
        try
        {
            return convertible.ToDecimal(CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }
}
