using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AuroraControls.TestApp.Controls;

namespace AuroraControls.TestApp;

/// <summary>
/// Test page for CupertinoButton control with live property editing.
/// </summary>
public partial class CupertinoButtonTestPage : ContentPage, INotifyPropertyChanged
{
    private string _buttonText = "Tap Me";
    private Color _buttonBackgroundColor = Color.FromArgb("#7C3AED");
    private Color _fontColor = Colors.White;
    private Color _borderColor = Colors.Transparent;
    private Color _shadowColor = Color.FromArgb("#7C3AED");
    private double _fontSize = 17;
    private double _borderWidth = 0;
    private double _borderRadius = 12;
    private double _shadowBlurRadius = 0;
    private bool _isIconifiedText = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="CupertinoButtonTestPage"/> class.
    /// </summary>
    public CupertinoButtonTestPage()
    {
        InitializeComponent();
        BindingContext = this;
        TestCommand = new Command<string>(OnTestCommandExecuted);
        SetupPresets();
    }

    /// <summary>
    /// Gets or sets the button text.
    /// </summary>
    public string ButtonText
    {
        get => _buttonText;
        set => SetProperty(ref _buttonText, value);
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
    /// Gets or sets the font color.
    /// </summary>
    public Color FontColor
    {
        get => _fontColor;
        set => SetProperty(ref _fontColor, value);
    }

    /// <summary>
    /// Gets or sets the border color.
    /// </summary>
    public Color BorderColor
    {
        get => _borderColor;
        set => SetProperty(ref _borderColor, value);
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
    /// Gets or sets the font size.
    /// </summary>
    public double FontSize
    {
        get => _fontSize;
        set => SetProperty(ref _fontSize, value);
    }

    /// <summary>
    /// Gets or sets the border width.
    /// </summary>
    public double BorderWidth
    {
        get => _borderWidth;
        set => SetProperty(ref _borderWidth, value);
    }

    /// <summary>
    /// Gets or sets the border radius.
    /// </summary>
    public double BorderRadius
    {
        get => _borderRadius;
        set => SetProperty(ref _borderRadius, value);
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
    /// Gets or sets a value indicating whether the text is iconified.
    /// </summary>
    public bool IsIconifiedText
    {
        get => _isIconifiedText;
        set => SetProperty(ref _isIconifiedText, value);
    }

    /// <summary>
    /// Gets the test command.
    /// </summary>
    public ICommand TestCommand { get; }

    private void OnTestCommandExecuted(string parameter)
    {
        System.Diagnostics.Debug.WriteLine($"CupertinoButton tapped: {parameter}");
    }

    private void SetupPresets()
    {
        var presets = new ObservableCollection<PresetItem>
        {
            new PresetItem
            {
                Name = "Default",
                OnSelected = () => ApplyPreset(
                    bgColor: "#7C3AED",
                    fontColor: Colors.White,
                    borderColor: Colors.Transparent,
                    borderWidth: 0,
                    borderRadius: 12,
                    shadowBlur: 0),
            },
            new PresetItem
            {
                Name = "Pill",
                OnSelected = () => ApplyPreset(
                    bgColor: "#22C55E",
                    fontColor: Colors.White,
                    borderColor: Colors.Transparent,
                    borderWidth: 0,
                    borderRadius: 25,
                    shadowBlur: 0),
            },
            new PresetItem
            {
                Name = "Outline",
                OnSelected = () => ApplyPreset(
                    bgColor: "Transparent",
                    fontColor: Color.FromArgb("#7C3AED"),
                    borderColor: Color.FromArgb("#7C3AED"),
                    borderWidth: 2,
                    borderRadius: 10,
                    shadowBlur: 0),
            },
            new PresetItem
            {
                Name = "Glow",
                OnSelected = () => ApplyPreset(
                    bgColor: "#EC4899",
                    fontColor: Colors.White,
                    borderColor: Colors.Transparent,
                    borderWidth: 0,
                    borderRadius: 12,
                    shadowBlur: 12,
                    shadowColor: "#EC4899"),
            },
            new PresetItem
            {
                Name = "Square",
                OnSelected = () => ApplyPreset(
                    bgColor: "#3B82F6",
                    fontColor: Colors.White,
                    borderColor: Colors.Transparent,
                    borderWidth: 0,
                    borderRadius: 0,
                    shadowBlur: 0),
            },
        };

        PresetSelector.Presets = presets;
    }

    private void ApplyPreset(
        string bgColor,
        Color fontColor,
        Color borderColor,
        double borderWidth,
        double borderRadius,
        double shadowBlur,
        string shadowColor = null)
    {
        ButtonBackgroundColor = bgColor == "Transparent" ? Colors.Transparent : Color.FromArgb(bgColor);
        FontColor = fontColor;
        BorderColor = borderColor;
        BorderWidth = borderWidth;
        BorderRadius = borderRadius;
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
