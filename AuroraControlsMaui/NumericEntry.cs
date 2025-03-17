using System.Globalization;

namespace AuroraControls;

public enum NumericEntryValueType
{
    Double = 1,
    Decimal = 2,
    Float = 3,
    Int = 4,
    Long = 5,
}

public class NumericEntry : Entry
{
    public static readonly BindableProperty CultureInfoProperty =
        BindableProperty.Create(nameof(CultureInfo), typeof(CultureInfo), typeof(NumericEntry), CultureInfo.CurrentUICulture);

    public CultureInfo CultureInfo
    {
        get { return (CultureInfo)GetValue(CultureInfoProperty); }
        set { SetValue(CultureInfoProperty, value); }
    }

    public static readonly BindableProperty ValueTypeProperty =
        BindableProperty.Create(nameof(ValueType), typeof(NumericEntryValueType), typeof(NumericEntry), NumericEntryValueType.Double);

    public NumericEntryValueType ValueType
    {
        get => (NumericEntryValueType)GetValue(ValueTypeProperty);
        set => SetValue(ValueTypeProperty, value);
    }
}
