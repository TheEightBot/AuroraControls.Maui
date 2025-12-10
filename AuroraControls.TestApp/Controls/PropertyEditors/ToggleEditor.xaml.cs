// <copyright file="ToggleEditor.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Windows.Input;

namespace AuroraControls.TestApp.Controls.PropertyEditors;

/// <summary>
/// A toggle editor control for boolean property editing in demo pages.
/// Displays a labeled switch with optional description.
/// </summary>
public partial class ToggleEditor : ContentView
{
    /// <summary>
    /// Bindable property for the label text.
    /// </summary>
    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(
            nameof(Label),
            typeof(string),
            typeof(ToggleEditor),
            "Enabled");

    /// <summary>
    /// Bindable property for the description text.
    /// </summary>
    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(ToggleEditor),
            string.Empty);

    /// <summary>
    /// Bindable property for the toggle value.
    /// </summary>
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(
            nameof(Value),
            typeof(bool),
            typeof(ToggleEditor),
            false,
            BindingMode.TwoWay,
            propertyChanged: OnValueChanged);

    /// <summary>
    /// Bindable property for the value changed command.
    /// </summary>
    public static readonly BindableProperty ValueChangedCommandProperty =
        BindableProperty.Create(
            nameof(ValueChangedCommand),
            typeof(ICommand),
            typeof(ToggleEditor));

    /// <summary>
    /// Initializes a new instance of the <see cref="ToggleEditor"/> class.
    /// </summary>
    public ToggleEditor()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Event raised when the value changes.
    /// </summary>
    public event EventHandler<bool>? ValueChanged;

    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the description text.
    /// </summary>
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the toggle is on.
    /// </summary>
    public bool Value
    {
        get => (bool)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
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
        if (bindable is ToggleEditor editor && newValue is bool value)
        {
            editor.ValueChanged?.Invoke(editor, value);
            editor.ValueChangedCommand?.Execute(value);
        }
    }
}
