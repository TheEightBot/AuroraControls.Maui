namespace AuroraControls;

[ContentProperty(nameof(Date))]
public class CalendarPicker : DatePicker
{
    public new event EventHandler<NullableDateChangedEventArgs> DateSelected;

    /// <summary>
    /// The date property.
    /// </summary>
    public static readonly new BindableProperty DateProperty = BindableProperty.Create(
        nameof(Date), typeof(DateTime?), typeof(CalendarPicker), null, BindingMode.TwoWay,
        coerceValue: CoerceNullableDate,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var picker = bindable as CalendarPicker;

            picker?.DateSelected?.Invoke(
                picker,
                new NullableDateChangedEventArgs((DateTime?)oldValue, (DateTime?)newValue));
        },
        defaultValueCreator: (bindable) => null);

    /// <summary>
    /// Gets or sets the date.
    /// </summary>
    /// <value>The date.</value>
    public new DateTime? Date
    {
        get => (DateTime?)GetValue(DateProperty);

        set
        {
            if (this.Date == value)
            {
                return;
            }

            if (value.HasValue)
            {
                base.Date = value.Value;
            }

            this.SetValue(DateProperty, value);
        }
    }

    /// <summary>
    /// Method that is called when the property that is specified by propertyName is changed.
    /// </summary>
    /// <param name="propertyName">The name of the bound property that changed.</param>
    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == VisualElement.IsFocusedProperty.PropertyName ||
            (this.IsFocused && propertyName == DatePicker.DateProperty.PropertyName))
        {
            this.Date = base.Date;
        }
    }

    /// <summary>
    /// Coerces the nullable date.
    /// </summary>
    /// <returns>The nullable date.</returns>
    /// <param name="bindable">Bindable.</param>
    /// <param name="value">Expects a DateTime?.</param>
    private static object CoerceNullableDate(BindableObject bindable, object value)
    {
        var picker = (DatePicker)bindable;
        DateTime? dateValue = (DateTime?)value;

        if (!dateValue.HasValue)
        {
            return dateValue;
        }

        if (dateValue > picker.MaximumDate)
        {
            dateValue = picker.MaximumDate;
        }

        if (dateValue < picker.MinimumDate)
        {
            dateValue = picker.MinimumDate;
        }

        return dateValue;
    }
}
