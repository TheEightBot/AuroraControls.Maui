using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AuroraControls.TestApp.Controls;

namespace AuroraControls.TestApp.Pages.Inputs;

/// <summary>
/// Demo page for NumericEntry control with live property editing.
/// </summary>
public partial class NumericEntryDemoPage : ContentPage, INotifyPropertyChanged
{
    private AuroraControls.NumericEntryValueType _selectedValueType = AuroraControls.NumericEntryValueType.Double;
    private Color _textColorValue = Color.FromArgb("#F9FAFB");
    private Color _placeholderColorValue = Color.FromArgb("#6B7280");
    private Color _backgroundColorValue = Color.FromArgb("#1A1A1A");

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericEntryDemoPage"/> class.
    /// </summary>
    public NumericEntryDemoPage()
    {
        InitializeComponent();
        BindingContext = this;

        ValueTypePicker.SelectedIndex = 0; // Double

        SetupEditors();
        SetupPresets();
    }

    /// <summary>Gets or sets the selected value type.</summary>
    public AuroraControls.NumericEntryValueType SelectedValueType
    {
        get => _selectedValueType;
        set
        {
            if (SetProperty(ref _selectedValueType, value))
            {
                UpdateValueLabel();
            }
        }
    }

    /// <summary>Gets or sets the text color value.</summary>
    public Color TextColorValue
    {
        get => _textColorValue;
        set => SetProperty(ref _textColorValue, value);
    }

    /// <summary>Gets or sets the placeholder color value.</summary>
    public Color PlaceholderColorValue
    {
        get => _placeholderColorValue;
        set => SetProperty(ref _placeholderColorValue, value);
    }

    /// <summary>Gets or sets the background color value.</summary>
    public Color BackgroundColorValue
    {
        get => _backgroundColorValue;
        set => SetProperty(ref _backgroundColorValue, value);
    }

    private void SetupEditors()
    {
        // Color pickers
        TextColorPicker.SelectedColor = _textColorValue;
        TextColorPicker.ColorChanged += (s, c) =>
        {
            TextColorValue = c;
            PreviewNumericEntry.TextColor = c;
        };

        PlaceholderColorPicker.SelectedColor = _placeholderColorValue;
        PlaceholderColorPicker.ColorChanged += (s, c) =>
        {
            PlaceholderColorValue = c;
            PreviewNumericEntry.PlaceholderColor = c;
        };

        BackgroundColorPicker.SelectedColor = _backgroundColorValue;
        BackgroundColorPicker.ColorChanged += (s, c) =>
        {
            BackgroundColorValue = c;
            PreviewNumericEntry.BackgroundColor = c;
        };
    }

    private void SetupPresets()
    {
        var presets = new ObservableCollection<PresetItem>
        {
            new PresetItem
            {
                Name = "Double",
                OnSelected = () => ApplyPreset(AuroraControls.NumericEntryValueType.Double, "3.14159"),
            },
            new PresetItem
            {
                Name = "Integer",
                OnSelected = () => ApplyPreset(AuroraControls.NumericEntryValueType.Int, "Enter whole number"),
            },
            new PresetItem
            {
                Name = "Currency",
                OnSelected = () => ApplyPreset(AuroraControls.NumericEntryValueType.Decimal, "0.00"),
            },
            new PresetItem
            {
                Name = "Float",
                OnSelected = () => ApplyPreset(AuroraControls.NumericEntryValueType.Float, "Enter decimal"),
            },
            new PresetItem
            {
                Name = "Long",
                OnSelected = () => ApplyPreset(AuroraControls.NumericEntryValueType.Long, "Large number"),
            },
        };

        PresetSelector.Presets = presets;
    }

    private void ApplyPreset(AuroraControls.NumericEntryValueType valueType, string placeholder)
    {
        SelectedValueType = valueType;
        PreviewNumericEntry.Placeholder = placeholder;
        PlaceholderEntry.Text = placeholder;

        // Update picker
        ValueTypePicker.SelectedIndex = valueType switch
        {
            AuroraControls.NumericEntryValueType.Double => 0,
            AuroraControls.NumericEntryValueType.Decimal => 1,
            AuroraControls.NumericEntryValueType.Float => 2,
            AuroraControls.NumericEntryValueType.Int => 3,
            AuroraControls.NumericEntryValueType.Long => 4,
            _ => 0,
        };
    }

    private void OnValueTypeChanged(object sender, EventArgs e)
    {
        if (ValueTypePicker.SelectedIndex >= 0)
        {
            SelectedValueType = ValueTypePicker.SelectedIndex switch
            {
                0 => AuroraControls.NumericEntryValueType.Double,
                1 => AuroraControls.NumericEntryValueType.Decimal,
                2 => AuroraControls.NumericEntryValueType.Float,
                3 => AuroraControls.NumericEntryValueType.Int,
                4 => AuroraControls.NumericEntryValueType.Long,
                _ => AuroraControls.NumericEntryValueType.Double,
            };
        }
    }

    private void OnPlaceholderChanged(object sender, TextChangedEventArgs e)
    {
        PreviewNumericEntry.Placeholder = e.NewTextValue;
    }

    private void UpdateValueLabel()
    {
        if (ValueLabel != null)
        {
            ValueLabel.Text = $"Value Type: {SelectedValueType}";
        }
    }

    /// <summary>
    /// Sets a property value and raises PropertyChanged if the value changed.
    /// </summary>
    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return false;
        }

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>Event raised when a property value changes.</summary>
    public new event PropertyChangedEventHandler PropertyChanged;

    /// <summary>Raises the PropertyChanged event.</summary>
    protected new void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
