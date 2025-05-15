using System.ComponentModel;
using System.Globalization;

namespace AuroraControls.Effects;

[TypeConverter(typeof(SafeArea))]
public class SafeAreaTypeConverter : TypeConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        var strValue = value.ToString() ?? string.Empty;

        if (strValue != null)
        {
            strValue = strValue.Trim();

            if (strValue.Contains(","))
            {
                // XAML based definition
                var safeArea = strValue.Split(',');

                switch (safeArea.Length)
                {
                    case 2:
                        if (bool.TryParse(safeArea[0], out var h)
                            && bool.TryParse(safeArea[1], out var v))
                        {
                            return new SafeArea(h, v);
                        }

                        break;
                    case 4:
                        if (bool.TryParse(safeArea[0], out var l)
                            && bool.TryParse(safeArea[1], out var t)
                            && bool.TryParse(safeArea[2], out var r)
                            && bool.TryParse(safeArea[3], out var b))
                        {
                            return new SafeArea(l, t, r, b);
                        }

                        break;
                }
            }
            else
            {
                // Single uniform SafeArea
                if (bool.TryParse(strValue, out var l))
                {
                    return new SafeArea(l);
                }
            }
        }

        throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(SafeArea)}");
    }
}
