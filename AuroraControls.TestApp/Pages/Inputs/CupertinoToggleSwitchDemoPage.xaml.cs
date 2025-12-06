using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AuroraControls.TestApp.Controls;

namespace AuroraControls.TestApp.Pages.Inputs;

/// <summary>
/// Demo page for CupertinoToggleSwitch control with live property editing.
/// </summary>
public partial class CupertinoToggleSwitchDemoPage : ContentPage, INotifyPropertyChanged
{
    private bool _isToggled = true;
    private double _toggleWidth = 60;
    private double _toggleHeight = 32;
    private double _borderWidth = 4;
    private Color _thumbColorValue = Colors.White;
    private Color _trackEnabledColorValue = Color.FromArgb("#22C55E");
    private Color _trackDisabledColorValue = Color.FromArgb("#404040");

    /// <summary>
    /// Initializes a new instance of the <see cref="CupertinoToggleSwitchDemoPage"/> class.
    /// </summary>
    public CupertinoToggleSwitchDemoPage()
    {
        InitializeComponent();
        BindingContext = this;

        SetupEditors();
        SetupPresets();
        UpdateToggleStatus();
    }

    /// <summary>Gets or sets a value indicating whether the toggle is on.</summary>
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

    /// <summary>Gets or sets the toggle width.</summary>
    public double ToggleWidth
    {
        get => _toggleWidth;
        set => SetProperty(ref _toggleWidth, value);
    }

    /// <summary>Gets or sets the toggle height.</summary>
    public double ToggleHeight
    {
        get => _toggleHeight;
        set => SetProperty(ref _toggleHeight, value);
    }

    /// <summary>Gets or sets the border width.</summary>
    public double BorderWidth
    {
        get => _borderWidth;
        set => SetProperty(ref _borderWidth, value);
    }

    /// <summary>Gets or sets the thumb color.</summary>
    public Color ThumbColorValue
    {
        get => _thumbColorValue;
        set => SetProperty(ref _thumbColorValue, value);
    }

    /// <summary>Gets or sets the track enabled color.</summary>
    public Color TrackEnabledColorValue
    {
        get => _trackEnabledColorValue;
        set => SetProperty(ref _trackEnabledColorValue, value);
    }

    /// <summary>Gets or sets the track disabled color.</summary>
    public Color TrackDisabledColorValue
    {
        get => _trackDisabledColorValue;
        set => SetProperty(ref _trackDisabledColorValue, value);
    }

    private void SetupEditors()
    {
        // Toggle editors
        IsToggledEditor.ValueChanged += (s, e) => IsToggled = e;
        IsEnabledEditor.ValueChanged += (s, e) => PreviewToggle.IsEnabled = e;

        // Size sliders
        WidthSlider.ValueChanged += (s, e) => ToggleWidth = e.NewValue;
        HeightSlider.ValueChanged += (s, e) => ToggleHeight = e.NewValue;
        BorderWidthSlider.ValueChanged += (s, e) => BorderWidth = e.NewValue;

        // Animation slider
        AnimationDurationSlider.ValueChanged += (s, e) => PreviewToggle.ToggleAnimationDuration = (uint)e.NewValue;

        // Color pickers
        ThumbColorPicker.SelectedColor = _thumbColorValue;
        ThumbColorPicker.ColorChanged += (s, c) => ThumbColorValue = c;

        TrackEnabledColorPicker.SelectedColor = _trackEnabledColorValue;
        TrackEnabledColorPicker.ColorChanged += (s, c) => TrackEnabledColorValue = c;

        TrackDisabledColorPicker.SelectedColor = _trackDisabledColorValue;
        TrackDisabledColorPicker.ColorChanged += (s, c) => TrackDisabledColorValue = c;
    }

    private void SetupPresets()
    {
        var presets = new ObservableCollection<PresetItem>
        {
            new PresetItem
            {
                Name = "iOS",
                OnSelected = () => ApplyPreset(
                    trackEnabled: "#22C55E",
                    trackDisabled: "#E5E7EB",
                    thumb: "White",
                    width: 51,
                    height: 31),
            },
            new PresetItem
            {
                Name = "Purple",
                OnSelected = () => ApplyPreset(
                    trackEnabled: "#7C3AED",
                    trackDisabled: "#404040",
                    thumb: "White",
                    width: 60,
                    height: 32),
            },
            new PresetItem
            {
                Name = "Blue",
                OnSelected = () => ApplyPreset(
                    trackEnabled: "#3B82F6",
                    trackDisabled: "#404040",
                    thumb: "White",
                    width: 60,
                    height: 32),
            },
            new PresetItem
            {
                Name = "Pink",
                OnSelected = () => ApplyPreset(
                    trackEnabled: "#EC4899",
                    trackDisabled: "#404040",
                    thumb: "White",
                    width: 60,
                    height: 32),
            },
            new PresetItem
            {
                Name = "Large",
                OnSelected = () => ApplyPreset(
                    trackEnabled: "#7C3AED",
                    trackDisabled: "#404040",
                    thumb: "White",
                    width: 80,
                    height: 44),
            },
        };

        PresetSelector.Presets = presets;
    }

    private void ApplyPreset(string trackEnabled, string trackDisabled, string thumb, double width, double height)
    {
        TrackEnabledColorValue = Color.FromArgb(trackEnabled);
        TrackDisabledColorValue = Color.FromArgb(trackDisabled);
        ThumbColorValue = thumb == "White" ? Colors.White : Color.FromArgb(thumb);
        ToggleWidth = width;
        ToggleHeight = height;

        // Update color pickers
        TrackEnabledColorPicker.SelectedColor = TrackEnabledColorValue;
        TrackDisabledColorPicker.SelectedColor = TrackDisabledColorValue;
        ThumbColorPicker.SelectedColor = ThumbColorValue;

        // Update sliders
        WidthSlider.Value = width;
        HeightSlider.Value = height;
    }

    private void OnToggleChanged(object sender, ToggledEventArgs e)
    {
        IsToggled = e.Value;
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
