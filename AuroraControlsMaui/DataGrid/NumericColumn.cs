using System.Reflection;

namespace AuroraControls.DataGrid;

/// <summary>
/// A column type for displaying numeric data with formatting.
/// </summary>
public class NumericColumn : TextColumn
{
    /// <summary>
    /// Property for the numeric format string.
    /// </summary>
    public static readonly BindableProperty FormatStringProperty =
        BindableProperty.Create(nameof(FormatString), typeof(string), typeof(NumericColumn), "N2");

    /// <summary>
    /// Gets or sets the format string for numeric values.
    /// </summary>
    public string FormatString
    {
        get => (string)GetValue(FormatStringProperty);
        set => SetValue(FormatStringProperty, value);
    }

    public NumericColumn()
    {
        // Set default alignment to end (right)
        TextAlignment = TextAlignment.End;
    }

    /// <summary>
    /// Gets the formatted numeric value.
    /// </summary>
    public override object GetCellValue(object item)
    {
        var value = base.GetCellValue(item);
        if (value == null)
        {
            return null;
        }

        // Handle different numeric types
        if (value is IFormattable formattable)
        {
            return formattable.ToString(FormatString, System.Globalization.CultureInfo.CurrentCulture);
        }

        return value;
    }
}
