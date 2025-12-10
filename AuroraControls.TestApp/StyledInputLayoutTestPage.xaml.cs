using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AuroraControls.TestApp.Controls;

namespace AuroraControls.TestApp;

/// <summary>
/// Test page for StyledInputLayout control with live property editing.
/// </summary>
public partial class StyledInputLayoutTestPage : ContentPage, INotifyPropertyChanged
{
    private string _placeholderText = "Enter text";
    private Color _placeholderColor = Color.FromArgb("#9CA3AF");
    private Color _activeColor = Color.FromArgb("#7C3AED");
    private Color _inactiveColor = Color.FromArgb("#4B5563");
    private Color _errorColor = Color.FromArgb("#EF4444");
    private AuroraControls.ContainerBorderStyle _selectedBorderStyle = AuroraControls.ContainerBorderStyle.RoundedRectangle;
    private float _cornerRadius = 8;
    private float _borderSize = 2;
    private bool _isError;
    private string _errorText = "This field is required";

    /// <summary>
    /// Initializes a new instance of the <see cref="StyledInputLayoutTestPage"/> class.
    /// </summary>
    public StyledInputLayoutTestPage()
    {
        InitializeComponent();
        BindingContext = this;
        StylePicker.SelectedIndex = 3; // RoundedRectangle
        SetupPresets();
    }

    /// <summary>
    /// Gets or sets the placeholder text.
    /// </summary>
    public string PlaceholderText
    {
        get => _placeholderText;
        set => SetProperty(ref _placeholderText, value);
    }

    /// <summary>
    /// Gets or sets the placeholder color.
    /// </summary>
    public Color PlaceholderColor
    {
        get => _placeholderColor;
        set => SetProperty(ref _placeholderColor, value);
    }

    /// <summary>
    /// Gets or sets the active color.
    /// </summary>
    public Color ActiveColor
    {
        get => _activeColor;
        set => SetProperty(ref _activeColor, value);
    }

    /// <summary>
    /// Gets or sets the inactive color.
    /// </summary>
    public Color InactiveColor
    {
        get => _inactiveColor;
        set => SetProperty(ref _inactiveColor, value);
    }

    /// <summary>
    /// Gets or sets the error color.
    /// </summary>
    public Color ErrorColor
    {
        get => _errorColor;
        set => SetProperty(ref _errorColor, value);
    }

    /// <summary>
    /// Gets or sets the selected border style.
    /// </summary>
    public AuroraControls.ContainerBorderStyle SelectedBorderStyle
    {
        get => _selectedBorderStyle;
        set => SetProperty(ref _selectedBorderStyle, value);
    }

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    public float CornerRadius
    {
        get => _cornerRadius;
        set => SetProperty(ref _cornerRadius, value);
    }

    /// <summary>
    /// Gets or sets the border size.
    /// </summary>
    public float BorderSize
    {
        get => _borderSize;
        set => SetProperty(ref _borderSize, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the input is in error state.
    /// </summary>
    public bool IsError
    {
        get => _isError;
        set => SetProperty(ref _isError, value);
    }

    /// <summary>
    /// Gets or sets the error text.
    /// </summary>
    public string ErrorText
    {
        get => _errorText;
        set => SetProperty(ref _errorText, value);
    }

    private void OnStyleChanged(object sender, EventArgs e)
    {
        if (StylePicker.SelectedIndex >= 0)
        {
            var styleName = StylePicker.Items[StylePicker.SelectedIndex];
            SelectedBorderStyle = styleName switch
            {
                "Underline" => AuroraControls.ContainerBorderStyle.Underline,
                "RoundedUnderline" => AuroraControls.ContainerBorderStyle.RoundedUnderline,
                "Rectangle" => AuroraControls.ContainerBorderStyle.Rectangle,
                "RoundedRectangle" => AuroraControls.ContainerBorderStyle.RoundedRectangle,
                "RoundedRectanglePlaceholderThrough" => AuroraControls.ContainerBorderStyle.RoundedRectanglePlaceholderThrough,
                _ => AuroraControls.ContainerBorderStyle.Underline,
            };
        }
    }

    private void SetupPresets()
    {
        var presets = new ObservableCollection<PresetItem>
        {
            new PresetItem
            {
                Name = "Material",
                OnSelected = () => ApplyPreset(
                    style: AuroraControls.ContainerBorderStyle.Underline,
                    activeColor: "#7C3AED",
                    inactiveColor: "#4B5563",
                    cornerRadius: 0),
            },
            new PresetItem
            {
                Name = "Outlined",
                OnSelected = () => ApplyPreset(
                    style: AuroraControls.ContainerBorderStyle.RoundedRectangle,
                    activeColor: "#3B82F6",
                    inactiveColor: "#4B5563",
                    cornerRadius: 8),
            },
            new PresetItem
            {
                Name = "Rounded",
                OnSelected = () => ApplyPreset(
                    style: AuroraControls.ContainerBorderStyle.RoundedRectangle,
                    activeColor: "#22C55E",
                    inactiveColor: "#4B5563",
                    cornerRadius: 16),
            },
            new PresetItem
            {
                Name = "Floating",
                OnSelected = () => ApplyPreset(
                    style: AuroraControls.ContainerBorderStyle.RoundedRectanglePlaceholderThrough,
                    activeColor: "#EC4899",
                    inactiveColor: "#4B5563",
                    cornerRadius: 8),
            },
            new PresetItem
            {
                Name = "Sharp",
                OnSelected = () => ApplyPreset(
                    style: AuroraControls.ContainerBorderStyle.Rectangle,
                    activeColor: "#F59E0B",
                    inactiveColor: "#4B5563",
                    cornerRadius: 0),
            },
        };

        PresetSelector.Presets = presets;
    }

    private void ApplyPreset(
        AuroraControls.ContainerBorderStyle style,
        string activeColor,
        string inactiveColor,
        float cornerRadius)
    {
        SelectedBorderStyle = style;
        ActiveColor = Color.FromArgb(activeColor);
        InactiveColor = Color.FromArgb(inactiveColor);
        CornerRadius = cornerRadius;

        // Update style picker to match
        StylePicker.SelectedIndex = style switch
        {
            AuroraControls.ContainerBorderStyle.Underline => 0,
            AuroraControls.ContainerBorderStyle.RoundedUnderline => 1,
            AuroraControls.ContainerBorderStyle.Rectangle => 2,
            AuroraControls.ContainerBorderStyle.RoundedRectangle => 3,
            AuroraControls.ContainerBorderStyle.RoundedRectanglePlaceholderThrough => 4,
            _ => 0,
        };
    }

    /// <summary>
    /// Sets a property value and raises PropertyChanged if the value changed.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="backingStore">Reference to the backing field.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">The property name (auto-filled).</param>
    /// <returns>True if the value changed, false otherwise.</returns>
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

    /// <summary>
    /// Event raised when a property value changes.
    /// </summary>
    public new event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected new void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
