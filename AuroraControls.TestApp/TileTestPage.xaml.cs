using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AuroraControls.TestApp.Controls;

namespace AuroraControls.TestApp;

/// <summary>
/// Test page for Tile control with live property editing.
/// </summary>
public partial class TileTestPage : ContentPage, INotifyPropertyChanged
{
    private string _selectedImage = "triforce.svg";
    private string _tileText = "Tile Test";
    private bool _showText = true;
    private Color _tileBackgroundColor = Color.FromArgb("#2196F3");
    private Color _tileFontColor = Colors.White;
    private Color _shadowColor = Color.FromArgb("#80000000");
    private double _tileWidth = 200;
    private double _tileHeight = 200;
    private double _cornerRadius = 12;
    private double _shadowBlurRadius = 10;
    private double _maxImageWidth = 0;
    private double _maxImageHeight = 0;
    private double _paddingLeft = 8;
    private double _paddingTop = 8;
    private double _paddingRight = 8;
    private double _paddingBottom = 8;

    /// <summary>
    /// Initializes a new instance of the <see cref="TileTestPage"/> class.
    /// </summary>
    public TileTestPage()
    {
        InitializeComponent();
        BindingContext = this;
        ImagePicker.SelectedIndex = 0;
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
    /// Gets or sets the tile text.
    /// </summary>
    public string TileText
    {
        get => _tileText;
        set => SetProperty(ref _tileText, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show text.
    /// </summary>
    public bool ShowText
    {
        get => _showText;
        set
        {
            if (SetProperty(ref _showText, value))
            {
                TileText = value ? "Tile Test" : string.Empty;
            }
        }
    }

    /// <summary>
    /// Gets or sets the tile background color.
    /// </summary>
    public Color TileBackgroundColor
    {
        get => _tileBackgroundColor;
        set => SetProperty(ref _tileBackgroundColor, value);
    }

    /// <summary>
    /// Gets or sets the tile font color.
    /// </summary>
    public Color TileFontColor
    {
        get => _tileFontColor;
        set => SetProperty(ref _tileFontColor, value);
    }

    /// <summary>
    /// Gets or sets the shadow color.
    /// </summary>
    public Color ShadowColor
    {
        get => _shadowColor;
        set => SetProperty(ref _shadowColor, value);
    }

    /// <summary>
    /// Gets or sets the tile width.
    /// </summary>
    public double TileWidth
    {
        get => _tileWidth;
        set => SetProperty(ref _tileWidth, value);
    }

    /// <summary>
    /// Gets or sets the tile height.
    /// </summary>
    public double TileHeight
    {
        get => _tileHeight;
        set => SetProperty(ref _tileHeight, value);
    }

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    public double CornerRadius
    {
        get => _cornerRadius;
        set => SetProperty(ref _cornerRadius, value);
    }

    /// <summary>
    /// Gets or sets the shadow blur radius.
    /// </summary>
    public double ShadowBlurRadius
    {
        get => _shadowBlurRadius;
        set => SetProperty(ref _shadowBlurRadius, value);
    }

    /// <summary>
    /// Gets or sets the max image width.
    /// </summary>
    public double MaxImageWidth
    {
        get => _maxImageWidth;
        set
        {
            if (SetProperty(ref _maxImageWidth, value))
            {
                UpdateMaxImageSize();
            }
        }
    }

    /// <summary>
    /// Gets or sets the max image height.
    /// </summary>
    public double MaxImageHeight
    {
        get => _maxImageHeight;
        set
        {
            if (SetProperty(ref _maxImageHeight, value))
            {
                UpdateMaxImageSize();
            }
        }
    }

    /// <summary>
    /// Gets or sets the padding left.
    /// </summary>
    public double PaddingLeft
    {
        get => _paddingLeft;
        set
        {
            if (SetProperty(ref _paddingLeft, value))
            {
                UpdateContentPadding();
            }
        }
    }

    /// <summary>
    /// Gets or sets the padding top.
    /// </summary>
    public double PaddingTop
    {
        get => _paddingTop;
        set
        {
            if (SetProperty(ref _paddingTop, value))
            {
                UpdateContentPadding();
            }
        }
    }

    /// <summary>
    /// Gets or sets the padding right.
    /// </summary>
    public double PaddingRight
    {
        get => _paddingRight;
        set
        {
            if (SetProperty(ref _paddingRight, value))
            {
                UpdateContentPadding();
            }
        }
    }

    /// <summary>
    /// Gets or sets the padding bottom.
    /// </summary>
    public double PaddingBottom
    {
        get => _paddingBottom;
        set
        {
            if (SetProperty(ref _paddingBottom, value))
            {
                UpdateContentPadding();
            }
        }
    }

    private void UpdateMaxImageSize()
    {
        if (MaxImageWidth == 0 && MaxImageHeight == 0)
        {
            PreviewTile.MaxImageSize = Size.Zero;
        }
        else
        {
            PreviewTile.MaxImageSize = new Size(
                MaxImageWidth == 0 ? 999 : MaxImageWidth,
                MaxImageHeight == 0 ? 999 : MaxImageHeight);
        }
    }

    private void UpdateContentPadding()
    {
        PreviewTile.ContentPadding = new Thickness(
            PaddingLeft,
            PaddingTop,
            PaddingRight,
            PaddingBottom);
    }

    private void OnImageChanged(object sender, EventArgs e)
    {
        if (ImagePicker.SelectedIndex >= 0)
        {
            SelectedImage = ImagePicker.Items[ImagePicker.SelectedIndex];
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
                    bgColor: "#2196F3",
                    fontColor: Colors.White,
                    cornerRadius: 12,
                    shadowBlur: 10),
            },
            new PresetItem
            {
                Name = "Rounded",
                OnSelected = () => ApplyPreset(
                    bgColor: "#22C55E",
                    fontColor: Colors.White,
                    cornerRadius: 24,
                    shadowBlur: 0),
            },
            new PresetItem
            {
                Name = "Circle",
                OnSelected = () => ApplyPreset(
                    bgColor: "#7C3AED",
                    fontColor: Colors.White,
                    cornerRadius: 50,
                    shadowBlur: 15,
                    shadowColor: "#7C3AED"),
            },
            new PresetItem
            {
                Name = "Square",
                OnSelected = () => ApplyPreset(
                    bgColor: "#EC4899",
                    fontColor: Colors.White,
                    cornerRadius: 0,
                    shadowBlur: 0),
            },
            new PresetItem
            {
                Name = "Subtle",
                OnSelected = () => ApplyPreset(
                    bgColor: "#374151",
                    fontColor: Colors.White,
                    cornerRadius: 8,
                    shadowBlur: 5),
            },
        };

        PresetSelector.Presets = presets;
    }

    private void ApplyPreset(
        string bgColor,
        Color fontColor,
        double cornerRadius,
        double shadowBlur,
        string shadowColor = null)
    {
        TileBackgroundColor = Color.FromArgb(bgColor);
        TileFontColor = fontColor;
        CornerRadius = cornerRadius;
        ShadowBlurRadius = shadowBlur;
        if (shadowColor != null)
        {
            ShadowColor = Color.FromArgb(shadowColor);
        }
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
