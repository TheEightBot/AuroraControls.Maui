// <copyright file="PresetSelector.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace AuroraControls.TestApp.Controls;

/// <summary>
/// A horizontal preset selector control for quick configuration changes.
/// Displays a row of selectable preset buttons.
/// </summary>
public partial class PresetSelector : ContentView
{
    /// <summary>
    /// Bindable property for the label text.
    /// </summary>
    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(
            nameof(Label),
            typeof(string),
            typeof(PresetSelector),
            "Presets");

    /// <summary>
    /// Bindable property for whether to show the label.
    /// </summary>
    public static readonly BindableProperty ShowLabelProperty =
        BindableProperty.Create(
            nameof(ShowLabel),
            typeof(bool),
            typeof(PresetSelector),
            true);

    /// <summary>
    /// Bindable property for the presets collection.
    /// </summary>
    public static readonly BindableProperty PresetsProperty =
        BindableProperty.Create(
            nameof(Presets),
            typeof(ObservableCollection<PresetItem>),
            typeof(PresetSelector),
            null);

    /// <summary>
    /// Bindable property for the selected preset.
    /// </summary>
    public static readonly BindableProperty SelectedPresetProperty =
        BindableProperty.Create(
            nameof(SelectedPreset),
            typeof(PresetItem),
            typeof(PresetSelector),
            null,
            BindingMode.TwoWay,
            propertyChanged: OnSelectedPresetChanged);

    /// <summary>
    /// Bindable property for the preset selected command.
    /// </summary>
    public static readonly BindableProperty PresetSelectedCommandProperty =
        BindableProperty.Create(
            nameof(PresetSelectedCommand),
            typeof(ICommand),
            typeof(PresetSelector));

    /// <summary>
    /// Initializes a new instance of the <see cref="PresetSelector"/> class.
    /// </summary>
    public PresetSelector()
    {
        InitializeComponent();
        SelectPresetCommand = new Command<PresetItem>(OnPresetSelected);
        Presets = new ObservableCollection<PresetItem>();
    }

    /// <summary>
    /// Event raised when a preset is selected.
    /// </summary>
    public event EventHandler<PresetItem>? PresetChanged;

    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the label.
    /// </summary>
    public bool ShowLabel
    {
        get => (bool)GetValue(ShowLabelProperty);
        set => SetValue(ShowLabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of presets.
    /// </summary>
    public ObservableCollection<PresetItem> Presets
    {
        get => (ObservableCollection<PresetItem>)GetValue(PresetsProperty);
        set => SetValue(PresetsProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently selected preset.
    /// </summary>
    public PresetItem? SelectedPreset
    {
        get => (PresetItem?)GetValue(SelectedPresetProperty);
        set => SetValue(SelectedPresetProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to execute when a preset is selected.
    /// </summary>
    public ICommand? PresetSelectedCommand
    {
        get => (ICommand?)GetValue(PresetSelectedCommandProperty);
        set => SetValue(PresetSelectedCommandProperty, value);
    }

    /// <summary>
    /// Gets the internal command for selecting a preset.
    /// </summary>
    public ICommand SelectPresetCommand { get; }

    private static void OnSelectedPresetChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PresetSelector selector && newValue is PresetItem preset)
        {
            selector.UpdateSelection(preset);
            selector.PresetChanged?.Invoke(selector, preset);
            selector.PresetSelectedCommand?.Execute(preset);
        }
    }

    private void OnPresetSelected(PresetItem preset)
    {
        SelectedPreset = preset;

        // Call the preset's OnSelected action if defined
        preset.OnSelected?.Invoke();
    }

    private void UpdateSelection(PresetItem selectedPreset)
    {
        foreach (var preset in Presets)
        {
            preset.IsSelected = preset == selectedPreset;
        }
    }
}

/// <summary>
/// Represents a preset configuration item.
/// </summary>
public class PresetItem : BindableObject
{
    private bool _isSelected;

    /// <summary>
    /// Gets or sets the preset name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the preset key/identifier.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this preset is selected.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets additional data associated with the preset.
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets the action to execute when this preset is selected.
    /// </summary>
    public Action? OnSelected { get; set; }
}

/// <summary>
/// Converter for preset selection to stroke color.
/// </summary>
public class BoolToPresetStrokeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return Color.FromArgb("#7C3AED"); // Aurora Purple
        }

        return Application.Current?.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#404040")
            : Color.FromArgb("#E5E7EB");
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converter for preset selection to background color.
/// </summary>
public class BoolToPresetBackgroundConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return Color.FromArgb("#7C3AED").WithAlpha(0.15f); // Aurora Purple with alpha
        }

        return Colors.Transparent;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converter for preset selection to text color.
/// </summary>
public class BoolToPresetTextColorConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return Color.FromArgb("#7C3AED"); // Aurora Purple
        }

        return Application.Current?.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#F9FAFB")
            : Color.FromArgb("#111827");
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
