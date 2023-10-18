using System;
using System.Reflection;

namespace AuroraControls;

internal static class ObjectExtensions
{
    public static void SetPrivateProperty<TObj, TVal>(this TObj obj, string propertyName, TVal value)
    {
        var propertyInfo = typeof(TObj).GetProperty(propertyName);

        if (propertyInfo is null)
        {
            return;
        }

        propertyInfo.SetValue(obj, value);
    }

    /// <summary>
    /// Clamp the specified val, min and max.
    /// </summary>
    /// <returns>The clamp.</returns>
    /// <param name="val">Value.</param>
    /// <param name="min">Minimum.</param>
    /// <param name="max">Max.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T Clamp<T>(this T val, T min, T max)
        where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0)
        {
            return min;
        }

        if (val.CompareTo(max) > 0)
        {
            return max;
        }

        return val;
    }
}
