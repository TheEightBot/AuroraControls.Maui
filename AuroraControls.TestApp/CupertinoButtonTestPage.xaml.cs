using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AuroraControls.TestApp;

public partial class CupertinoButtonTestPage : ContentPage, INotifyPropertyChanged
{
    private string _buttonText = "Live Button";
    private Color _buttonBackgroundColor = Color.FromArgb("#006FFF");
    private Color _fontColor = Colors.White;
    private Color _borderColor = Colors.Transparent;
    private double _fontSize = 16;
    private double _borderWidth;
    private double _borderRadius = 4;
    private double _shadowBlurRadius;
    private Color _shadowColor = Color.FromRgba(0, 0, 0, 0.33);
    private bool _isIconifiedText;

    public CupertinoButtonTestPage()
    {
        InitializeComponent();
        TestCommand = new Command<string>(OnTestCommandExecuted);
        BindingContext = this;
    }

    public ICommand TestCommand { get; }

    public string ButtonText
    {
        get => _buttonText;
        set => SetProperty(ref _buttonText, value);
    }

    public new Color BackgroundColor
    {
        get => _buttonBackgroundColor;
        set => SetProperty(ref _buttonBackgroundColor, value);
    }

    public Color FontColor
    {
        get => _fontColor;
        set => SetProperty(ref _fontColor, value);
    }

    public Color BorderColor
    {
        get => _borderColor;
        set => SetProperty(ref _borderColor, value);
    }

    public double FontSize
    {
        get => _fontSize;
        set => SetProperty(ref _fontSize, value);
    }

    public double BorderWidth
    {
        get => _borderWidth;
        set => SetProperty(ref _borderWidth, value);
    }

    public double BorderRadius
    {
        get => _borderRadius;
        set => SetProperty(ref _borderRadius, value);
    }

    public double ShadowBlurRadius
    {
        get => _shadowBlurRadius;
        set => SetProperty(ref _shadowBlurRadius, value);
    }

    public Color ShadowColor
    {
        get => _shadowColor;
        set => SetProperty(ref _shadowColor, value);
    }

    public bool IsIconifiedText
    {
        get => _isIconifiedText;
        set => SetProperty(ref _isIconifiedText, value);
    }

    private void OnTestCommandExecuted(string parameter)
    {
        var details = $"Button: {parameter}\n" +
                     $"Text: '{ButtonText}'\n" +
                     $"Background: {BackgroundColor}\n" +
                     $"Font: {FontColor}, Size: {FontSize:F1}\n" +
                     $"Border: {BorderColor}, Width: {BorderWidth:F1}, Radius: {BorderRadius:F1}\n" +
                     $"Shadow: Blur {ShadowBlurRadius:F1}\n" +
                     $"Iconified: {IsIconifiedText}";

        StatusLabel.Text = details;
    }

    private async void OnBackgroundColorClicked(object sender, EventArgs e)
    {
        var colorName = await ShowColorPicker("Select Background Color", BackgroundColor);
        if (colorName != null)
        {
            BackgroundColor = GetColorByName(colorName);
        }
    }

    private async void OnFontColorClicked(object sender, EventArgs e)
    {
        var colorName = await ShowColorPicker("Select Font Color", FontColor);
        if (colorName != null)
        {
            FontColor = GetColorByName(colorName);
        }
    }

    private async void OnBorderColorClicked(object sender, EventArgs e)
    {
        var colorName = await ShowColorPicker("Select Border Color", BorderColor);
        if (colorName != null)
        {
            BorderColor = GetColorByName(colorName);
        }
    }

    private async Task<string> ShowColorPicker(string title, Color currentColor)
    {
        var colorNames = new[]
        {
            "Red", "Blue", "Green", "Orange", "Purple", "Teal", "Brown",
            "Pink", "Yellow", "Gray", "Black", "White", "Transparent",
        };

        return await DisplayActionSheet(title, "Cancel", null, colorNames);
    }

    private Color GetColorByName(string colorName)
    {
        return colorName switch
        {
            "Red" => Colors.Red,
            "Blue" => Colors.Blue,
            "Green" => Colors.Green,
            "Orange" => Colors.Orange,
            "Purple" => Colors.Purple,
            "Teal" => Colors.Teal,
            "Brown" => Colors.Brown,
            "Pink" => Colors.Pink,
            "Yellow" => Colors.Yellow,
            "Gray" => Colors.Gray,
            "Black" => Colors.Black,
            "White" => Colors.White,
            "Transparent" => Colors.Transparent,
            _ => Colors.Blue,
        };
    }

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

    public new event PropertyChangedEventHandler PropertyChanged;

    protected new void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
