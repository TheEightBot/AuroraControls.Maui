using System.ComponentModel;
using System.Globalization;

namespace AuroraControls;

public class UIntTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        => sourceType == typeof(string);

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(string);

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        string? strValue = value?.ToString();
        if (string.IsNullOrWhiteSpace(strValue))
        {
            return 0u;
        }

        return uint.Parse(strValue, CultureInfo.InvariantCulture);
    }

    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is not uint ui)
        {
            throw new NotSupportedException();
        }

        return ui.ToString(CultureInfo.InvariantCulture);
    }
}
