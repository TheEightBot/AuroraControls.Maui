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
}