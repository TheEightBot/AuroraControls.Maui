using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AuroraControls.TestApp.Controls;
using AuroraControls.TestApp.Controls.PropertyEditors;

namespace AuroraControls.TestApp;

/// <summary>
/// Test page for ToggleBox control with live property editing.
/// </summary>
public partial class ToggleBoxTestPage : ContentPage, INotifyPropertyChanged
{
    private double _size = 48;
    private AuroraControls.ToggleBoxShape _selectedShape = AuroraControls.ToggleBoxShape.RoundedSquare;
    private AuroraControls.ToggleBoxCheckType _selectedCheckType = AuroraControls.ToggleBoxCheckType.Check;
    private bool _isToggled = true;
    private Color _borderColorValue = Color.FromArgb("#7C3AED");
    private Color _checkColorValue = Color.FromArgb("#7C3AED");
    private Color _toggledBgColorValue = Color.FromArgb("#7C3AED");
    private Color _untoggledBgColorValue = Colors.Transparent;
    private int _borderWidth = 3;
    private int _markWidth = 3;
    private double _cornerRadius = 8;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToggleBoxTestPage"/> class.
    /// </summary>
    public ToggleBoxTestPage()
    {
        InitializeComponent();
        BindingContext = this;

        // Set initial values for pickers
        ShapePicker.SelectedIndex = 1; // RoundedSquare
        CheckTypePicker.SelectedIndex = 0; // Check

        SetupEditors();
        SetupPresets();
        UpdateToggleStatus();
    }

    /// <summary>Gets or sets the size.</summary>
    public double Size
    {
        get => _size;
        set => SetProperty(ref _size, value);
    }

    /// <summary>Gets or sets the selected shape.</summary>
    public AuroraControls.ToggleBoxShape SelectedShape
    {
        get => _selectedShape;
        set => SetProperty(ref _selectedShape, value);
    }

    /// <summary>Gets or sets the selected check type.</summary>
    public AuroraControls.ToggleBoxCheckType SelectedCheckType
    {
        get => _selectedCheckType;
        set => SetProperty(ref _selectedCheckType, value);
    }

    /// <summary>Gets or sets a value indicating whether is toggled.</summary>
    public bool IsToggled
    {
        get => _isToggled;
        set
        {
            if (SetProperty(ref _isToggled, value))
            {
                UpdateToggleStatus();
            }
        }
    }

    /// <summary>Gets or sets the border color value.</summary>
    public Color BorderColorValue
    {
        get => _borderColorValue;
        set => SetProperty(ref _borderColorValue, value);
    }

    /// <summary>Gets or sets the check color value.</summary>
    public Color CheckColorValue
    {
        get => _checkColorValue;
        set => SetProperty(ref _checkColorValue, value);
    }

    /// <summary>Gets or sets the toggled background color value.</summary>
    public Color ToggledBgColorValue
    {
        get => _toggledBgColorValue;
        set => SetProperty(ref _toggledBgColorValue, value);
    }

    /// <summary>Gets or sets the untoggled background color value.</summary>
    public Color UntoggledBgColorValue
    {
        get => _untoggledBgColorValue;
        set => SetProperty(ref _untoggledBgColorValue, value);
    }

    /// <summary>Gets or sets the border width.</summary>
    public int BorderWidth
    {
        get => _borderWidth;
        set => SetProperty(ref _borderWidth, value);
    }

    /// <summary>Gets or sets the mark width.</summary>
    public int MarkWidth
    {
        get => _markWidth;
        set => SetProperty(ref _markWidth, value);
    }

    /// <summary>Gets or sets the corner radius.</summary>
    public double CornerRadius
    {
        get => _cornerRadius;
        set => SetProperty(ref _cornerRadius, value);
    }

    private void SetupEditors()
    {
        // Size slider
        SizeSlider.ValueChanged += (s, e) => Size = e.NewValue;

        // Border width slider
        BorderWidthSlider.ValueChanged += (s, e) => BorderWidth = (int)e.NewValue;

        // Mark width slider
        MarkWidthSlider.ValueChanged += (s, e) => MarkWidth = (int)e.NewValue;

        // Corner radius slider
        CornerRadiusSlider.ValueChanged += (s, e) => CornerRadius = e.NewValue;

        // Color pickers
        BorderColorPicker.SelectedColor = _borderColorValue;
        BorderColorPicker.ColorChanged += (s, c) => BorderColorValue = c;

        CheckColorPicker.SelectedColor = _checkColorValue;
        CheckColorPicker.ColorChanged += (s, c) => CheckColorValue = c;

        ToggledBgColorPicker.SelectedColor = _toggledBgColorValue;
        ToggledBgColorPicker.ColorChanged += (s, c) => ToggledBgColorValue = c;

        UntoggledBgColorPicker.SelectedColor = _untoggledBgColorValue;
        UntoggledBgColorPicker.ColorChanged += (s, c) => UntoggledBgColorValue = c;

        // Toggle editors
        IsToggledToggle.ValueChanged += (s, e) => IsToggled = e;
        IsEnabledToggle.ValueChanged += (s, e) => PreviewToggleBox.IsEnabled = e;
    }

    private void SetupPresets()
    {
        var presets = new ObservableCollection<PresetItem>
        {
            new PresetItem
            {
                Name = "Checkbox",
                OnSelected = () => ApplyPreset(
                    shape: AuroraControls.ToggleBoxShape.RoundedSquare,
                    checkType: AuroraControls.ToggleBoxCheckType.Check,
                    borderColor: "#7C3AED",
                    checkColor: "White",
                    toggledBg: "#7C3AED"),
            },
            new PresetItem
            {
                Name = "Radio",
                OnSelected = () => ApplyPreset(
                    shape: AuroraControls.ToggleBoxShape.Circular,
                    checkType: AuroraControls.ToggleBoxCheckType.Circular,
                    borderColor: "#3B82F6",
                    checkColor: "White",
                    toggledBg: "#3B82F6"),
            },
            new PresetItem
            {
                Name = "Square",
                OnSelected = () => ApplyPreset(
                    shape: AuroraControls.ToggleBoxShape.Square,
                    checkType: AuroraControls.ToggleBoxCheckType.Cross,
                    borderColor: "#EF4444",
                    checkColor: "#EF4444",
                    toggledBg: "Transparent"),
            },
            new PresetItem
            {
                Name = "Success",
                OnSelected = () => ApplyPreset(
                    shape: AuroraControls.ToggleBoxShape.RoundedSquare,
                    checkType: AuroraControls.ToggleBoxCheckType.Check,
                    borderColor: "#22C55E",
                    checkColor: "White",
                    toggledBg: "#22C55E"),
            },
            new PresetItem
            {
                Name = "Teal",
                OnSelected = () => ApplyPreset(
                    shape: AuroraControls.ToggleBoxShape.RoundedSquare,
                    checkType: AuroraControls.ToggleBoxCheckType.RoundedCheck,
                    borderColor: "#14B8A6",
                    checkColor: "White",
                    toggledBg: "#14B8A6"),
            },
        };

        PresetSelector.Presets = presets;
    }

    private void ApplyPreset(
        AuroraControls.ToggleBoxShape shape,
        AuroraControls.ToggleBoxCheckType checkType,
        string borderColor,
        string checkColor,
        string toggledBg)
    {
        SelectedShape = shape;
        SelectedCheckType = checkType;
        BorderColorValue = Color.FromArgb(borderColor);
        CheckColorValue = checkColor == "White" ? Colors.White : Color.FromArgb(checkColor);
        ToggledBgColorValue = toggledBg == "Transparent" ? Colors.Transparent : Color.FromArgb(toggledBg);

        // Update pickers
        ShapePicker.SelectedIndex = shape switch
        {
            AuroraControls.ToggleBoxShape.Square => 0,
            AuroraControls.ToggleBoxShape.RoundedSquare => 1,
            AuroraControls.ToggleBoxShape.Circular => 2,
            _ => 1,
        };

        CheckTypePicker.SelectedIndex = checkType switch
        {
            AuroraControls.ToggleBoxCheckType.Check => 0,
            AuroraControls.ToggleBoxCheckType.RoundedCheck => 1,
            AuroraControls.ToggleBoxCheckType.Cross => 2,
            AuroraControls.ToggleBoxCheckType.Circular => 3,
            _ => 0,
        };

        // Update color pickers
        BorderColorPicker.SelectedColor = BorderColorValue;
        CheckColorPicker.SelectedColor = CheckColorValue;
        ToggledBgColorPicker.SelectedColor = ToggledBgColorValue;
    }

    private void OnShapeChanged(object sender, EventArgs e)
    {
        if (ShapePicker.SelectedIndex >= 0)
        {
            SelectedShape = ShapePicker.SelectedIndex switch
            {
                0 => AuroraControls.ToggleBoxShape.Square,
                1 => AuroraControls.ToggleBoxShape.RoundedSquare,
                2 => AuroraControls.ToggleBoxShape.Circular,
                _ => AuroraControls.ToggleBoxShape.RoundedSquare,
            };
        }
    }

    private void OnCheckTypeChanged(object sender, EventArgs e)
    {
        if (CheckTypePicker.SelectedIndex >= 0)
        {
            SelectedCheckType = CheckTypePicker.SelectedIndex switch
            {
                0 => AuroraControls.ToggleBoxCheckType.Check,
                1 => AuroraControls.ToggleBoxCheckType.RoundedCheck,
                2 => AuroraControls.ToggleBoxCheckType.Cross,
                3 => AuroraControls.ToggleBoxCheckType.Circular,
                _ => AuroraControls.ToggleBoxCheckType.Check,
            };
        }
    }

    private void UpdateToggleStatus()
    {
        if (ToggleStatusLabel != null)
        {
            ToggleStatusLabel.Text = IsToggled ? "ON" : "OFF";
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
