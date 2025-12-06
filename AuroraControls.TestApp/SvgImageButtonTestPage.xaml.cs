using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AuroraControls.TestApp.Controls;

namespace AuroraControls.TestApp;

/// <summary>
/// Test page for SvgImageButton control with live property editing.
/// </summary>
public partial class SvgImageButtonTestPage : ContentPage, INotifyPropertyChanged
{
    private string _selectedImage = "splatoon.svg";
    private AuroraControls.SvgImageButtonBackgroundShape _selectedShape = AuroraControls.SvgImageButtonBackgroundShape.RoundedSquare;
    private Color _buttonBackgroundColor = Color.FromArgb("#3B82F6");
    private Color _overlayColor = Colors.Transparent;
    private double _buttonSize = 80;
    private double _cornerRadius = 16;

    /// <summary>
    /// Initializes a new instance of the <see cref="SvgImageButtonTestPage"/> class.
    /// </summary>
    public SvgImageButtonTestPage()
    {
        InitializeComponent();
        BindingContext = this;
        ImagePicker.SelectedIndex = 0;
        ShapePicker.SelectedIndex = 3; // RoundedSquare
        SetupPresets();
    }

    /// <summary>
    /// Gets or sets the selected image name.
    /// </summary>
    public string SelectedImage
    {
        get => _selectedImage;
        set => SetProperty(ref _selectedImage, value);
    }

    /// <summary>
    /// Gets or sets the selected background shape.
    /// </summary>
    public AuroraControls.SvgImageButtonBackgroundShape SelectedShape
    {
        get => _selectedShape;
        set => SetProperty(ref _selectedShape, value);
    }

    /// <summary>
    /// Gets or sets the button background color.
    /// </summary>
    public Color ButtonBackgroundColor
    {
        get => _buttonBackgroundColor;
        set => SetProperty(ref _buttonBackgroundColor, value);
    }

    /// <summary>
    /// Gets or sets the overlay color.
    /// </summary>
    public Color OverlayColor
    {
        get => _overlayColor;
        set => SetProperty(ref _overlayColor, value);
    }

    /// <summary>
    /// Gets or sets the button size.
    /// </summary>
    public double ButtonSize
    {
        get => _buttonSize;
        set => SetProperty(ref _buttonSize, value);
    }

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    public double CornerRadius
    {
        get => _cornerRadius;
        set => SetProperty(ref _cornerRadius, value);
    }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        if (sender is AuroraControls.SvgImageButton button)
        {
            var imageName = button.EmbeddedImageName ?? "Unknown";
            var shape = button.BackgroundShape.ToString();
            StatusLabel.Text = $"Clicked: {imageName} ({shape})";
        }
    }

    private void OnImageChanged(object sender, EventArgs e)
    {
        if (ImagePicker.SelectedIndex >= 0)
        {
            SelectedImage = ImagePicker.Items[ImagePicker.SelectedIndex];
        }
    }

    private void OnShapeChanged(object sender, EventArgs e)
    {
        if (ShapePicker.SelectedIndex >= 0)
        {
            var shapeName = ShapePicker.Items[ShapePicker.SelectedIndex];
            SelectedShape = shapeName switch
            {
                "None" => AuroraControls.SvgImageButtonBackgroundShape.None,
                "Square" => AuroraControls.SvgImageButtonBackgroundShape.Square,
                "Circular" => AuroraControls.SvgImageButtonBackgroundShape.Circular,
                "RoundedSquare" => AuroraControls.SvgImageButtonBackgroundShape.RoundedSquare,
                _ => AuroraControls.SvgImageButtonBackgroundShape.None,
            };
        }
    }

    private void SetupPresets()
    {
        var presets = new ObservableCollection<PresetItem>
        {
            new PresetItem
            {
                Name = "Default",
                OnSelected = () => ApplyPreset(
                    bgColor: "#3B82F6",
                    overlayColor: Colors.Transparent,
                    shape: AuroraControls.SvgImageButtonBackgroundShape.RoundedSquare,
                    size: 80,
                    cornerRadius: 16),
            },
            new PresetItem
            {
                Name = "Circle",
                OnSelected = () => ApplyPreset(
                    bgColor: "#22C55E",
                    overlayColor: Colors.Transparent,
                    shape: AuroraControls.SvgImageButtonBackgroundShape.Circular,
                    size: 80,
                    cornerRadius: 0),
            },
            new PresetItem
            {
                Name = "Square",
                OnSelected = () => ApplyPreset(
                    bgColor: "#EC4899",
                    overlayColor: Colors.Transparent,
                    shape: AuroraControls.SvgImageButtonBackgroundShape.Square,
                    size: 80,
                    cornerRadius: 0),
            },
            new PresetItem
            {
                Name = "Tinted",
                OnSelected = () => ApplyPreset(
                    bgColor: "#1F2937",
                    overlayColor: Color.FromArgb("#7C3AED"),
                    shape: AuroraControls.SvgImageButtonBackgroundShape.Circular,
                    size: 80,
                    cornerRadius: 0),
            },
            new PresetItem
            {
                Name = "Large",
                OnSelected = () => ApplyPreset(
                    bgColor: "#F59E0B",
                    overlayColor: Colors.Transparent,
                    shape: AuroraControls.SvgImageButtonBackgroundShape.RoundedSquare,
                    size: 120,
                    cornerRadius: 24),
            },
        };

        PresetSelector.Presets = presets;
    }

    private void ApplyPreset(
        string bgColor,
        Color overlayColor,
        AuroraControls.SvgImageButtonBackgroundShape shape,
        double size,
        double cornerRadius)
    {
        ButtonBackgroundColor = Color.FromArgb(bgColor);
        OverlayColor = overlayColor;
        SelectedShape = shape;
        ButtonSize = size;
        CornerRadius = cornerRadius;

        // Update shape picker to match
        ShapePicker.SelectedIndex = shape switch
        {
            AuroraControls.SvgImageButtonBackgroundShape.None => 0,
            AuroraControls.SvgImageButtonBackgroundShape.Square => 1,
            AuroraControls.SvgImageButtonBackgroundShape.Circular => 2,
            AuroraControls.SvgImageButtonBackgroundShape.RoundedSquare => 3,
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
