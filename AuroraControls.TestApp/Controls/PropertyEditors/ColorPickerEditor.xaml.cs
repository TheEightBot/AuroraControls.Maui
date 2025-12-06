// <copyright file="ColorPickerEditor.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Windows.Input;

namespace AuroraControls.TestApp.Controls.PropertyEditors;

/// <summary>
/// A color picker editor control for property editing in demo pages.
/// Provides a color swatch, hex input, and preset color selection.
/// </summary>
public partial class ColorPickerEditor : ContentView
{
    /// <summary>
    /// Bindable property for the label text.
    /// </summary>
    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(
            nameof(Label),
            typeof(string),
            typeof(ColorPickerEditor),
            "Color");

    /// <summary>
    /// Bindable property for the selected color.
    /// </summary>
    public static readonly BindableProperty SelectedColorProperty =
        BindableProperty.Create(
            nameof(SelectedColor),
            typeof(Color),
            typeof(ColorPickerEditor),
            Colors.Purple,
            BindingMode.TwoWay,
            propertyChanged: OnSelectedColorChanged);

    /// <summary>
    /// Bindable property for the hex value string.
    /// </summary>
    public static readonly BindableProperty HexValueProperty =
        BindableProperty.Create(
            nameof(HexValue),
            typeof(string),
            typeof(ColorPickerEditor),
            "#7C3AED",
            BindingMode.TwoWay);

    /// <summary>
    /// Bindable property for the color changed command.
    /// </summary>
    public static readonly BindableProperty ColorChangedCommandProperty =
        BindableProperty.Create(
            nameof(ColorChangedCommand),
            typeof(ICommand),
            typeof(ColorPickerEditor));

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorPickerEditor"/> class.
    /// </summary>
    public ColorPickerEditor()
    {
        InitializeComponent();
        SelectPresetCommand = new Command<Color>(OnPresetColorSelected);
    }

    /// <summary>
    /// Event raised when the selected color changes.
    /// </summary>
    public event EventHandler<Color>? ColorChanged;

    /// <summary>
    /// Gets or sets the label text for the color picker.
    /// </summary>
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected color.
    /// </summary>
    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the hex value representation of the color.
    /// </summary>
    public string HexValue
    {
        get => (string)GetValue(HexValueProperty);
        set => SetValue(HexValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to execute when the color changes.
    /// </summary>
    public ICommand? ColorChangedCommand
    {
        get => (ICommand?)GetValue(ColorChangedCommandProperty);
        set => SetValue(ColorChangedCommandProperty, value);
    }

    /// <summary>
    /// Gets the command for selecting a preset color.
    /// </summary>
    public ICommand SelectPresetCommand { get; }

    private static void OnSelectedColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ColorPickerEditor editor && newValue is Color color)
        {
            editor.HexValue = color.ToArgbHex();
            editor.ColorChanged?.Invoke(editor, color);
            editor.ColorChangedCommand?.Execute(color);
        }
    }

    private void OnPresetColorSelected(Color color)
    {
        SelectedColor = color;
    }

    private void OnSwatchTapped(object? sender, TappedEventArgs e)
    {
        // TODO: Could show a full color picker popup here
        // For now, cycle through preset colors
        var presets = new[]
        {
            Color.FromArgb("#7C3AED"), // Aurora Purple
            Color.FromArgb("#EC4899"), // Aurora Pink
            Color.FromArgb("#3B82F6"), // Aurora Blue
            Color.FromArgb("#14B8A6"), // Aurora Teal
            Color.FromArgb("#22C55E"), // Success
            Color.FromArgb("#F59E0B"), // Warning
            Color.FromArgb("#EF4444"), // Error
        };

        var currentIndex = Array.FindIndex(presets, c => c.ToArgbHex() == SelectedColor.ToArgbHex());
        var nextIndex = (currentIndex + 1) % presets.Length;
        SelectedColor = presets[nextIndex];
    }

    private void OnHexEntryCompleted(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(HexValue))
        {
            return;
        }

        try
        {
            var hex = HexValue.StartsWith("#") ? HexValue : $"#{HexValue}";
            SelectedColor = Color.FromArgb(hex);
        }
        catch
        {
            // Invalid hex, revert to current color
            HexValue = SelectedColor.ToArgbHex();
        }
    }
}
