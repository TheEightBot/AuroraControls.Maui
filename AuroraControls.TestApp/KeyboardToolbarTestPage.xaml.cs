using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AuroraControls;

namespace AuroraControls.TestApp;

public partial class KeyboardToolbarTestPage : ContentPage
{
    public KeyboardToolbarTestPage()
    {
        InitializeComponent();
        BindingContext = new KeyboardToolbarViewModel();
    }
}

public class KeyboardToolbarViewModel : INotifyPropertyChanged
{
    // ── Picker option lists ───────────────────────────────────────────────
    public ObservableCollection<string> ButtonStyleOptions { get; } = new(
        new[] { "Text (recommended)", "System Checkmark" });

    public ObservableCollection<string> TitleColorOptions { get; } = new(
        new[] { "Default (system tint)", "Blue", "Red", "Green", "Orange", "Purple", "Black" });

    public ObservableCollection<string> BackgroundColorOptions { get; } = new(
        new[] { "Default (SystemBackground)", "White", "Dark", "Light Gray", "Teal", "Transparent-ish" });

    // ── Raw backing fields ────────────────────────────────────────────────

    // Start with effect ON and a visually distinct appearance so it's
    // immediately obvious it's different from MAUI's built-in Done bar.
    private bool _showToolbar = true;
    private int _buttonStyleIndex;
    private string _doneTitle = "Dismiss";
    private int _titleColorIndex = 2; // Red
    private int _backgroundColorIndex;
    private double _toolbarHeight;
    private double _liveFontSize;
    private string _liveEntryText = string.Empty;
    private string _liveEditorText = string.Empty;

    // ── Observable properties ──────────────────────────────────────────────

    /// <summary>Gets or sets a value indicating whether the keyboard toolbar effect is shown on the live test controls.</summary>
    public bool ShowToolbar
    {
        get => _showToolbar;
        set
        {
            if (SetProperty(ref _showToolbar, value))
            {
                OnPropertyChanged(nameof(StatusText));
                OnPropertyChanged(nameof(StatusColor));
                OnPropertyChanged(nameof(StatusBackground));
            }
        }
    }

    /// <summary>Gets or sets the picker index for the button style.</summary>
    public int ButtonStyleIndex
    {
        get => _buttonStyleIndex;
        set
        {
            if (SetProperty(ref _buttonStyleIndex, value))
            {
                OnPropertyChanged(nameof(SelectedButtonStyle));
            }
        }
    }

    /// <summary>Gets the enum value mapped from <see cref="ButtonStyleIndex"/>.</summary>
    public KeyboardToolbarDoneButtonStyle SelectedButtonStyle =>
        _buttonStyleIndex == 1
            ? KeyboardToolbarDoneButtonStyle.SystemCheckmark
            : KeyboardToolbarDoneButtonStyle.Text;

    /// <summary>Gets or sets the text shown on the Done button (Text style only).</summary>
    public string DoneTitle
    {
        get => _doneTitle;
        set => SetProperty(ref _doneTitle, value);
    }

    /// <summary>Gets or sets the picker index for the title color.</summary>
    public int TitleColorIndex
    {
        get => _titleColorIndex;
        set
        {
            if (SetProperty(ref _titleColorIndex, value))
            {
                OnPropertyChanged(nameof(SelectedTitleColor));
            }
        }
    }

    /// <summary>Gets the <see cref="Color"/> mapped from <see cref="TitleColorIndex"/>.</summary>
    public Color? SelectedTitleColor => TitleColorIndex switch
    {
        1 => Colors.Blue,
        2 => Colors.Red,
        3 => Colors.Green,
        4 => Colors.Orange,
        5 => Colors.Purple,
        6 => Colors.Black,
        _ => null,
    };

    /// <summary>Gets or sets the picker index for the toolbar background color.</summary>
    public int BackgroundColorIndex
    {
        get => _backgroundColorIndex;
        set
        {
            if (SetProperty(ref _backgroundColorIndex, value))
            {
                OnPropertyChanged(nameof(SelectedBackgroundColor));
            }
        }
    }

    /// <summary>Gets the <see cref="Color"/> mapped from <see cref="BackgroundColorIndex"/>.</summary>
    public Color? SelectedBackgroundColor => BackgroundColorIndex switch
    {
        1 => Colors.White,
        2 => Color.FromArgb("#CC222222"),
        3 => Color.FromArgb("#FFD3D3D3"),
        4 => Colors.Teal,
        5 => Color.FromArgb("#99FFFFFF"),
        _ => null,
    };

    /// <summary>Gets or sets the toolbar height in points (0 = auto).</summary>
    public double ToolbarHeight
    {
        get => _toolbarHeight;
        set
        {
            if (SetProperty(ref _toolbarHeight, value))
            {
                OnPropertyChanged(nameof(HeightLabel));
            }
        }
    }

    /// <summary>Gets a formatted label for the height slider.</summary>
    public string HeightLabel => ToolbarHeight < 1.0
        ? "Height: auto"
        : $"Height: {ToolbarHeight:F0} pt";

    /// <summary>Gets or sets the Done button font size (0 = auto).</summary>
    public double LiveFontSize
    {
        get => _liveFontSize;
        set
        {
            if (SetProperty(ref _liveFontSize, value))
            {
                OnPropertyChanged(nameof(FontSizeLabel));
            }
        }
    }

    /// <summary>Gets a formatted label for the font-size slider.</summary>
    public string FontSizeLabel => LiveFontSize < 1.0
        ? "Font size: auto"
        : $"Font size: {LiveFontSize:F0} pt";

    /// <summary>Gets or sets text entered in the live test Entry.</summary>
    public string LiveEntryText
    {
        get => _liveEntryText;
        set => SetProperty(ref _liveEntryText, value);
    }

    /// <summary>Gets or sets text entered in the live test Editor.</summary>
    public string LiveEditorText
    {
        get => _liveEditorText;
        set => SetProperty(ref _liveEditorText, value);
    }

    // ── Status ─────────────────────────────────────────────────────────────

    /// <summary>Gets a short status string indicating whether the effect is active.</summary>
    public string StatusText => ShowToolbar ? "✅  Effect is ON" : "⛔  Effect is OFF";

    /// <summary>Gets the color for the status text.</summary>
    public Color StatusColor => ShowToolbar ? Color.FromArgb("#2E7D32") : Color.FromArgb("#B71C1C");

    /// <summary>Gets the background color for the status indicator chip.</summary>
    public Color StatusBackground => ShowToolbar ? Color.FromArgb("#E8F5E9") : Color.FromArgb("#FFEBEE");

    // ── INotifyPropertyChanged ────────────────────────────────────────────
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(name);
        return true;
    }
}
