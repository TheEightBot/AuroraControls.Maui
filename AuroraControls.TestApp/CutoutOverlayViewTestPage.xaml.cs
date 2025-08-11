using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AuroraControls.TestApp;

public partial class CutoutOverlayViewTestPage : ContentPage
{
    public CutoutOverlayViewTestPage()
    {
        InitializeComponent();
        BindingContext = new CutoutOverlayViewTestViewModel();
    }
}

public class CutoutOverlayViewTestViewModel : INotifyPropertyChanged
{
    private CutoutOverlayShape _selectedShape = CutoutOverlayShape.Circular;
    private double _borderWidth = 2.0;
    private double _cornerRadius = 10.0;
    private double _insetValue = 20.0;
    private string _selectedOverlayColor = "Black (50% opacity)";
    private string _selectedBorderColor = "Red";
    private string _tapMessage = string.Empty;
    private bool _showTapMessage;

    public CutoutOverlayViewTestViewModel()
    {
        TapCommand = new Command<string>(OnTapped);

        // Initialize collections
        ShapeOptions = new ObservableCollection<string>
        {
            "Circular",
            "Oval",
            "Square",
            "Rectangular",
        };

        OverlayColorOptions = new ObservableCollection<string>
        {
            "Black (50% opacity)",
            "White (50% opacity)",
            "Red (50% opacity)",
            "Blue (50% opacity)",
            "Green (50% opacity)",
            "Purple (50% opacity)",
            "Black (solid)",
            "White (solid)",
        };

        BorderColorOptions = new ObservableCollection<string>
        {
            "Red",
            "Blue",
            "Green",
            "White",
            "Black",
            "Yellow",
            "Purple",
            "Orange",
            "Transparent",
        };
    }

    public ObservableCollection<string> ShapeOptions { get; }

    public ObservableCollection<string> OverlayColorOptions { get; }

    public ObservableCollection<string> BorderColorOptions { get; }

    public CutoutOverlayShape SelectedShape
    {
        get => _selectedShape;
        set
        {
            _selectedShape = value;
            OnPropertyChanged();
        }
    }

    public string SelectedShapeString
    {
        get => _selectedShape.ToString();
        set
        {
            if (Enum.TryParse<CutoutOverlayShape>(value, out var shape))
            {
                SelectedShape = shape;
            }
        }
    }

    public double BorderWidth
    {
        get => _borderWidth;
        set
        {
            _borderWidth = value;
            OnPropertyChanged();
        }
    }

    public double CornerRadius
    {
        get => _cornerRadius;
        set
        {
            _cornerRadius = value;
            OnPropertyChanged();
        }
    }

    public double InsetValue
    {
        get => _insetValue;
        set
        {
            _insetValue = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CutoutInset));
        }
    }

    public Thickness CutoutInset => new Thickness(_insetValue);

    public string SelectedOverlayColor
    {
        get => _selectedOverlayColor;
        set
        {
            _selectedOverlayColor = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(OverlayColor));
        }
    }

    public Color OverlayColor
    {
        get
        {
            return _selectedOverlayColor switch
            {
                "Black (50% opacity)" => Color.FromArgb("#80000000"),
                "White (50% opacity)" => Color.FromArgb("#80FFFFFF"),
                "Red (50% opacity)" => Color.FromArgb("#80FF0000"),
                "Blue (50% opacity)" => Color.FromArgb("#800000FF"),
                "Green (50% opacity)" => Color.FromArgb("#8000FF00"),
                "Purple (50% opacity)" => Color.FromArgb("#80800080"),
                "Black (solid)" => Colors.Black,
                "White (solid)" => Colors.White,
                _ => Color.FromArgb("#80000000"),
            };
        }
    }

    public string SelectedBorderColor
    {
        get => _selectedBorderColor;
        set
        {
            _selectedBorderColor = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(BorderColor));
        }
    }

    public Color BorderColor
    {
        get
        {
            return _selectedBorderColor switch
            {
                "Red" => Colors.Red,
                "Blue" => Colors.Blue,
                "Green" => Colors.Green,
                "White" => Colors.White,
                "Black" => Colors.Black,
                "Yellow" => Colors.Yellow,
                "Purple" => Colors.Purple,
                "Orange" => Colors.Orange,
                "Transparent" => Colors.Transparent,
                _ => Colors.Red,
            };
        }
    }

    public string TapMessage
    {
        get => _tapMessage;
        set
        {
            _tapMessage = value;
            OnPropertyChanged();
        }
    }

    public bool ShowTapMessage
    {
        get => _showTapMessage;
        set
        {
            _showTapMessage = value;
            OnPropertyChanged();
        }
    }

    public ICommand TapCommand { get; }

    private async void OnTapped(string parameter)
    {
        TapMessage = $"Tapped! {parameter} - Shape: {SelectedShape}";
        ShowTapMessage = true;

        // Hide the message after 3 seconds
        await Task.Delay(3000);
        ShowTapMessage = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
