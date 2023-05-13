using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Handlers;

namespace AuroraControls;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public partial class NumericEntryHandler : EntryHandler
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private NumericEntry NumericEntryVirtualView => VirtualView as NumericEntry;

    public static PropertyMapper NumericEntryMapper =
         new PropertyMapper<NumericEntry, NumericEntryHandler>(Mapper)
         {
         };

    public NumericEntryHandler()
        : base(NumericEntryMapper)
    {
    }

    protected static bool IsValid(string value, CultureInfo cultureInfo, NumericEntryValueType valueType)
    {
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }

        switch (valueType)
        {
            case NumericEntryValueType.Double:
            case NumericEntryValueType.Decimal:
            case NumericEntryValueType.Float:
                if (value.Equals(cultureInfo.NumberFormat.NumberDecimalSeparator))
                {
                    return true;
                }

                break;
        }

        if (value.Equals(cultureInfo.NumberFormat.NegativeSign) ||
            value.Equals(cultureInfo.NumberFormat.PositiveSign))
        {
            return true;
        }

        switch (valueType)
        {
            case NumericEntryValueType.Double:
                return double.TryParse(value, cultureInfo, out var _);
            case NumericEntryValueType.Decimal:
                return decimal.TryParse(value, cultureInfo, out var _);
            case NumericEntryValueType.Float:
                return float.TryParse(value, cultureInfo, out var _);
            case NumericEntryValueType.Int:
                return int.TryParse(value, cultureInfo, out var _);
            case NumericEntryValueType.Long:
                return long.TryParse(value, cultureInfo, out var _);
        }

        return false;
    }
}