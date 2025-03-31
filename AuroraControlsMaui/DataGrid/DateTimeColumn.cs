using System.Reflection;

namespace AuroraControls.DataGrid;

/// <summary>
/// A column type for displaying date and time values.
/// </summary>
public class DateTimeColumn : TextColumn
{
    /// <summary>
    /// Property for the date/time format string.
    /// </summary>
    public static readonly BindableProperty FormatStringProperty =
        BindableProperty.Create(nameof(FormatString), typeof(string), typeof(DateTimeColumn), "g");

    /// <summary>
    /// Gets or sets the format string for date/time values.
    /// </summary>
    public string FormatString
    {
        get => (string)GetValue(FormatStringProperty);
        set => SetValue(FormatStringProperty, value);
    }

    public DateTimeColumn()
    {
        // Center align by default
        TextAlignment = TextAlignment.Center;
    }

    /// <summary>
    /// Gets the formatted date/time value.
    /// </summary>
    public override object GetCellValue(object item)
    {
        var value = base.GetCellValue(item);
        if (value == null)
        {
            return null;
        }

        if (value is DateTime dateTime)
        {
            return dateTime.ToString(FormatString);
        }
        else if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString(FormatString);
        }

        return value;
    }
}
