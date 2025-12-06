using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AuroraControls.TestApp.Controls;

namespace AuroraControls.TestApp.Pages.Inputs;

/// <summary>
/// Demo page for SegmentedControl with live property editing.
/// </summary>
public partial class SegmentedControlDemoPage : ContentPage, INotifyPropertyChanged
{
    private AuroraControls.SegmentedControlStyle _selectedStyle = AuroraControls.SegmentedControlStyle.Cupertino;
    private Color _foregroundColorValue = Color.FromArgb("#7C3AED");
    private Color _backgroundColorValue = Color.FromArgb("#262626");
    private Color _foregroundTextColorValue = Colors.White;
    private Color _backgroundTextColorValue = Colors.Transparent;
    private int _cornerRadius = 4;
    private double _borderSize = 2;
    private double _fontSize = 14;

    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentedControlDemoPage"/> class.
    /// </summary>
    public SegmentedControlDemoPage()
    {
        InitializeComponent();
        BindingContext = this;

        StylePicker.SelectedIndex = 0; // Cupertino

        SetupEditors();
        SetupPresets();
    }

    /// <summary>Gets or sets the selected style.</summary>
    public AuroraControls.SegmentedControlStyle SelectedStyle
    {
        get => _selectedStyle;
        set => SetProperty(ref _selectedStyle, value);
    }

    /// <summary>Gets or sets the foreground color.</summary>
    public Color ForegroundColorValue
    {
        get => _foregroundColorValue;
        set => SetProperty(ref _foregroundColorValue, value);
    }

    /// <summary>Gets or sets the background color.</summary>
    public Color BackgroundColorValue
    {
        get => _backgroundColorValue;
        set => SetProperty(ref _backgroundColorValue, value);
    }

    /// <summary>Gets or sets the foreground text color.</summary>
    public Color ForegroundTextColorValue
    {
        get => _foregroundTextColorValue;
        set => SetProperty(ref _foregroundTextColorValue, value);
    }

    /// <summary>Gets or sets the background text color.</summary>
    public Color BackgroundTextColorValue
    {
        get => _backgroundTextColorValue;
        set => SetProperty(ref _backgroundTextColorValue, value);
    }

    /// <summary>Gets or sets the corner radius.</summary>
    public int CornerRadius
    {
        get => _cornerRadius;
        set => SetProperty(ref _cornerRadius, value);
    }

    /// <summary>Gets or sets the border size.</summary>
    public double BorderSize
    {
        get => _borderSize;
        set => SetProperty(ref _borderSize, value);
    }

    /// <summary>Gets or sets the font size.</summary>
    public double FontSize
    {
        get => _fontSize;
        set => SetProperty(ref _fontSize, value);
    }

    private void SetupEditors()
    {
        // Dimension sliders
        CornerRadiusSlider.ValueChanged += (s, e) => CornerRadius = (int)e;
        BorderSizeSlider.ValueChanged += (s, e) => BorderSize = e;
        FontSizeSlider.ValueChanged += (s, e) => FontSize = e;

        // Color pickers
        ForegroundColorPicker.SelectedColor = _foregroundColorValue;
        ForegroundColorPicker.ColorChanged += (s, c) => ForegroundColorValue = c;

        BackgroundColorPicker.SelectedColor = _backgroundColorValue;
        BackgroundColorPicker.ColorChanged += (s, c) => BackgroundColorValue = c;

        ForegroundTextColorPicker.SelectedColor = _foregroundTextColorValue;
        ForegroundTextColorPicker.ColorChanged += (s, c) => ForegroundTextColorValue = c;

        BackgroundTextColorPicker.SelectedColor = _backgroundTextColorValue;
        BackgroundTextColorPicker.ColorChanged += (s, c) => BackgroundTextColorValue = c;
    }

    private void SetupPresets()
    {
        var presets = new ObservableCollection<PresetItem>
        {
            new PresetItem
            {
                Name = "Cupertino",
                OnSelected = () => ApplyPreset(
                    style: AuroraControls.SegmentedControlStyle.Cupertino,
                    foreground: "#7C3AED",
                    background: "#262626",
                    fgText: "White",
                    bgText: "Transparent",
                    radius: 8),
            },
            new PresetItem
            {
                Name = "Filled",
                OnSelected = () => ApplyPreset(
                    style: AuroraControls.SegmentedControlStyle.Filled,
                    foreground: "#22C55E",
                    background: "#262626",
                    fgText: "White",
                    bgText: "Transparent",
                    radius: 4),
            },
            new PresetItem
            {
                Name = "Underline",
                OnSelected = () => ApplyPreset(
                    style: AuroraControls.SegmentedControlStyle.Underline,
                    foreground: "#EC4899",
                    background: "Transparent",
                    fgText: "#EC4899",
                    bgText: "#6B7280",
                    radius: 0),
            },
            new PresetItem
            {
                Name = "Pill",
                OnSelected = () => ApplyPreset(
                    style: AuroraControls.SegmentedControlStyle.Pill,
                    foreground: "#14B8A6",
                    background: "#262626",
                    fgText: "White",
                    bgText: "Transparent",
                    radius: 16),
            },
            new PresetItem
            {
                Name = "Rectangular",
                OnSelected = () => ApplyPreset(
                    style: AuroraControls.SegmentedControlStyle.Rectangular,
                    foreground: "#3B82F6",
                    background: "#262626",
                    fgText: "White",
                    bgText: "Transparent",
                    radius: 0),
            },
        };

        PresetSelector.Presets = presets;
    }

    private void ApplyPreset(AuroraControls.SegmentedControlStyle style, string foreground, string background, string fgText, string bgText, int radius)
    {
        SelectedStyle = style;
        ForegroundColorValue = Color.FromArgb(foreground);
        BackgroundColorValue = background == "Transparent" ? Colors.Transparent : Color.FromArgb(background);
        ForegroundTextColorValue = fgText == "White" ? Colors.White : Color.FromArgb(fgText);
        BackgroundTextColorValue = bgText == "Transparent" ? Colors.Transparent : Color.FromArgb(bgText);
        CornerRadius = radius;

        // Update picker
        StylePicker.SelectedIndex = style switch
        {
            AuroraControls.SegmentedControlStyle.Cupertino => 0,
            AuroraControls.SegmentedControlStyle.Filled => 1,
            AuroraControls.SegmentedControlStyle.Rectangular => 2,
            AuroraControls.SegmentedControlStyle.Underline => 3,
            AuroraControls.SegmentedControlStyle.Pill => 4,
            _ => 0,
        };

        // Update color pickers
        ForegroundColorPicker.SelectedColor = ForegroundColorValue;
        BackgroundColorPicker.SelectedColor = BackgroundColorValue;
        ForegroundTextColorPicker.SelectedColor = ForegroundTextColorValue;
        BackgroundTextColorPicker.SelectedColor = BackgroundTextColorValue;

        // Update sliders
        CornerRadiusSlider.Value = radius;
    }

    private void OnStyleChanged(object sender, EventArgs e)
    {
        if (StylePicker.SelectedIndex >= 0)
        {
            SelectedStyle = StylePicker.SelectedIndex switch
            {
                0 => AuroraControls.SegmentedControlStyle.Cupertino,
                1 => AuroraControls.SegmentedControlStyle.Filled,
                2 => AuroraControls.SegmentedControlStyle.Rectangular,
                3 => AuroraControls.SegmentedControlStyle.Underline,
                4 => AuroraControls.SegmentedControlStyle.Pill,
                _ => AuroraControls.SegmentedControlStyle.Cupertino,
            };
        }
    }

    private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (SelectionLabel != null && e.SelectedItem != null)
        {
            SelectionLabel.Text = $"Selected: {e.SelectedItem}";
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
