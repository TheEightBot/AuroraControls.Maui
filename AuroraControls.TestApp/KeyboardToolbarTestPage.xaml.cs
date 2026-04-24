using System.ComponentModel;
using System.Runtime.CompilerServices;

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
    private string _entryText = string.Empty;
    private string _editorText = string.Empty;
    private string _customEntry = string.Empty;
    private string _darkEntry = string.Empty;
    private string _systemEntry = string.Empty;
    private string _tallEntry = string.Empty;
    private bool _toggleToolbarShown = true;

    public string EntryText
    {
        get => _entryText;
        set => SetProperty(ref _entryText, value);
    }

    public string EditorText
    {
        get => _editorText;
        set => SetProperty(ref _editorText, value);
    }

    public string CustomEntry
    {
        get => _customEntry;
        set => SetProperty(ref _customEntry, value);
    }

    public string DarkEntry
    {
        get => _darkEntry;
        set => SetProperty(ref _darkEntry, value);
    }

    public string SystemEntry
    {
        get => _systemEntry;
        set => SetProperty(ref _systemEntry, value);
    }

    public string TallEntry
    {
        get => _tallEntry;
        set => SetProperty(ref _tallEntry, value);
    }

    public bool ToggleToolbarShown
    {
        get => _toggleToolbarShown;
        set => SetProperty(ref _toggleToolbarShown, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(name);
        return true;
    }
}
