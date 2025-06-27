using System.ComponentModel;
using System.Globalization;

namespace AuroraControls.Effects;

[TypeConverter(typeof(SafeArea))]
public class SafeAreaTypeConverter : TypeConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        string? strValue = value.ToString() ?? string.Empty;

        if (strValue != null)
        {
            strValue = strValue.Trim();

            if (strValue.Contains(","))
            {
                // XAML based definition
                string[] safeArea = strValue.Split(',');

                switch (safeArea.Length)
                {
                    case 2:
                        if (bool.TryParse(safeArea[0], out bool h)
                            && bool.TryParse(safeArea[1], out bool v))
                        {
                            return new SafeArea(h, v);
                        }

                        break;
                    case 4:
                        if (bool.TryParse(safeArea[0], out bool l)
                            && bool.TryParse(safeArea[1], out bool t)
                            && bool.TryParse(safeArea[2], out bool r)
                            && bool.TryParse(safeArea[3], out bool b))
                        {
                            return new SafeArea(l, t, r, b);
                        }

                        break;
                }
            }
            else
            {
                // Single uniform SafeArea
                if (bool.TryParse(strValue, out bool l))
                {
                    return new SafeArea(l);
                }
            }
        }

        throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(SafeArea)}");
    }
}
