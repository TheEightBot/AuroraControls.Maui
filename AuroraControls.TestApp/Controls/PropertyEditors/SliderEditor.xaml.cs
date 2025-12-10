// <copyright file="SliderEditor.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Windows.Input;

namespace AuroraControls.TestApp.Controls.PropertyEditors;

/// <summary>
/// A slider editor control for numeric property editing in demo pages.
/// Displays a slider with min/max labels and formatted value display.
/// </summary>
public partial class SliderEditor : ContentView
{
    /// <summary>
    /// Bindable property for the label text.
    /// </summary>
    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(
            nameof(Label),
            typeof(string),
            typeof(SliderEditor),
            "Value");

    /// <summary>
    /// Bindable property for the current value.
    /// </summary>
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(
            nameof(Value),
            typeof(double),
            typeof(SliderEditor),
            0.0,
            BindingMode.TwoWay,
            propertyChanged: OnValueChanged);

    /// <summary>
    /// Bindable property for the minimum value.
    /// </summary>
    public static readonly BindableProperty MinimumProperty =
        BindableProperty.Create(
            nameof(Minimum),
            typeof(double),
            typeof(SliderEditor),
            0.0);

    /// <summary>
    /// Bindable property for the maximum value.
    /// </summary>
    public static readonly BindableProperty MaximumProperty =
        BindableProperty.Create(
            nameof(Maximum),
            typeof(double),
            typeof(SliderEditor),
            100.0);

    /// <summary>
    /// Bindable property for the step/increment value.
    /// </summary>
    public static readonly BindableProperty StepProperty =
        BindableProperty.Create(
            nameof(Step),
            typeof(double),
            typeof(SliderEditor),
            1.0);

    /// <summary>
    /// Bindable property for the unit suffix.
    /// </summary>
    public static readonly BindableProperty UnitProperty =
        BindableProperty.Create(
            nameof(Unit),
            typeof(string),
            typeof(SliderEditor),
            string.Empty);

    /// <summary>
    /// Bindable property for the string format.
    /// </summary>
    public static readonly BindableProperty FormatProperty =
        BindableProperty.Create(
            nameof(Format),
            typeof(string),
            typeof(SliderEditor),
            "F0",
            propertyChanged: OnFormatChanged);

    /// <summary>
    /// Bindable property for the display value (read-only).
    /// </summary>
    public static readonly BindableProperty DisplayValueProperty =
        BindableProperty.Create(
            nameof(DisplayValue),
            typeof(string),
            typeof(SliderEditor),
            "0");

    /// <summary>
    /// Bindable property for the value changed command.
    /// </summary>
    public static readonly BindableProperty ValueChangedCommandProperty =
        BindableProperty.Create(
            nameof(ValueChangedCommand),
            typeof(ICommand),
            typeof(SliderEditor));

    /// <summary>
    /// Initializes a new instance of the <see cref="SliderEditor"/> class.
    /// </summary>
    public SliderEditor()
    {
        InitializeComponent();
        UpdateDisplayValue();
    }

    /// <summary>
    /// Event raised when the value changes.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs>? ValueChanged;

    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// Gets or sets the step/increment value.
    /// </summary>
    public double Step
    {
        get => (double)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    /// <summary>
    /// Gets or sets the unit suffix (e.g., "px", "dp", "%").
    /// </summary>
    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    /// <summary>
    /// Gets or sets the string format for the value display (e.g., "F0", "F2").
    /// </summary>
    public string Format
    {
        get => (string)GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    /// <summary>
    /// Gets the formatted display value.
    /// </summary>
    public string DisplayValue
    {
        get => (string)GetValue(DisplayValueProperty);
        private set => SetValue(DisplayValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to execute when the value changes.
    /// </summary>
    public ICommand? ValueChangedCommand
    {
        get => (ICommand?)GetValue(ValueChangedCommandProperty);
        set => SetValue(ValueChangedCommandProperty, value);
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SliderEditor editor && newValue is double newDoubleValue && oldValue is double oldDoubleValue)
        {
            // Apply step rounding
            if (editor.Step > 0)
            {
                var steppedValue = Math.Round(newDoubleValue / editor.Step) * editor.Step;
                if (Math.Abs(steppedValue - newDoubleValue) > 0.0001)
                {
                    editor.Value = steppedValue;
                    return;
                }
            }

            editor.UpdateDisplayValue();
            editor.ValueChanged?.Invoke(editor, new ValueChangedEventArgs(oldDoubleValue, newDoubleValue));
            editor.ValueChangedCommand?.Execute(newDoubleValue);
        }
    }

    private static void OnFormatChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SliderEditor editor)
        {
            editor.UpdateDisplayValue();
        }
    }

    private void UpdateDisplayValue()
    {
        try
        {
            DisplayValue = Value.ToString(Format);
        }
        catch
        {
            DisplayValue = Value.ToString("F0");
        }
    }
}
