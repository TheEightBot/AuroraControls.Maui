// <copyright file="ValueConverters.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Globalization;

namespace AuroraControls.TestApp.Controls;

/// <summary>
/// Converter that returns true if the string is not null or empty.
/// </summary>
public class StringNotEmptyConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value as string);
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converter that inverts a boolean value.
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }

        return value;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }

        return value;
    }
}

/// <summary>
/// Converter that returns a color based on a boolean value.
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets the color to use when the value is true.
    /// </summary>
    public Color TrueColor { get; set; } = Colors.Green;

    /// <summary>
    /// Gets or sets the color to use when the value is false.
    /// </summary>
    public Color FalseColor { get; set; } = Colors.Red;

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? TrueColor : FalseColor;
        }

        return FalseColor;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converter that returns visibility based on null check.
/// </summary>
public class NullToBoolConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets a value indicating whether to invert the result.
    /// </summary>
    public bool Invert { get; set; }

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isNull = value == null;
        return Invert ? isNull : !isNull;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converter that formats a double value with specified format string.
/// </summary>
public class DoubleFormatConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets the format string (e.g., "F0", "F2", "P0").
    /// </summary>
    public string Format { get; set; } = "F0";

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            var format = parameter as string ?? Format;
            return d.ToString(format, culture);
        }

        if (value is float f)
        {
            var format = parameter as string ?? Format;
            return f.ToString(format, culture);
        }

        return value?.ToString();
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s && double.TryParse(s, NumberStyles.Any, culture, out var result))
        {
            return result;
        }

        return 0.0;
    }
}

/// <summary>
/// Converter that maps an index to a color from a predefined palette.
/// </summary>
public class IndexToColorConverter : IValueConverter
{
    private static readonly Color[] Palette =
    {
        Color.FromArgb("#7C3AED"), // Aurora Purple
        Color.FromArgb("#EC4899"), // Aurora Pink
        Color.FromArgb("#3B82F6"), // Aurora Blue
        Color.FromArgb("#14B8A6"), // Aurora Teal
        Color.FromArgb("#22C55E"), // Success
        Color.FromArgb("#F59E0B"), // Warning
        Color.FromArgb("#EF4444"), // Error
    };

    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index)
        {
            return Palette[index % Palette.Length];
        }

        return Palette[0];
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converter that converts between double and float values.
/// </summary>
public class DoubleToFloatConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return (float)d;
        }

        if (value is float f)
        {
            return f;
        }

        return 0f;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is float f)
        {
            return (double)f;
        }

        if (value is double d)
        {
            return d;
        }

        return 0.0;
    }
}
