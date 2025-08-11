using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AuroraControls.AttachedProperties;

namespace AuroraControls.TestApp;

public partial class KeyboardReturnKeyTypeEffectTestPage : ContentPage
{
    public KeyboardReturnKeyTypeEffectTestPage()
    {
        InitializeComponent();
        BindingContext = new KeyboardReturnKeyTypeEffectViewModel();
    }
}

public class KeyboardReturnKeyTypeEffectViewModel : INotifyPropertyChanged
{
    private KeyboardReturnKeyType _selectedReturnKeyType = KeyboardReturnKeyType.Done;
    private string _entryText = string.Empty;
    private string _testResult = string.Empty;
    private bool _showTestResult;

    public KeyboardReturnKeyTypeEffectViewModel()
    {
        TestReturnKeyCommand = new Command(OnTestReturnKey);

        ReturnKeyTypes = new ObservableCollection<KeyboardReturnKeyType>
        {
            KeyboardReturnKeyType.Default,
            KeyboardReturnKeyType.Done,
            KeyboardReturnKeyType.Go,
            KeyboardReturnKeyType.Send,
            KeyboardReturnKeyType.Search,
            KeyboardReturnKeyType.Next,
        };
    }

    public ObservableCollection<KeyboardReturnKeyType> ReturnKeyTypes { get; }

    public KeyboardReturnKeyType SelectedReturnKeyType
    {
        get => _selectedReturnKeyType;
        set
        {
            _selectedReturnKeyType = value;
            OnPropertyChanged();
        }
    }

    public string EntryText
    {
        get => _entryText;
        set
        {
            _entryText = value;
            OnPropertyChanged();
        }
    }

    public string TestResult
    {
        get => _testResult;
        set
        {
            _testResult = value;
            OnPropertyChanged();
        }
    }

    public bool ShowTestResult
    {
        get => _showTestResult;
        set
        {
            _showTestResult = value;
            OnPropertyChanged();
        }
    }

    public ICommand TestReturnKeyCommand { get; }

    private async void OnTestReturnKey()
    {
        TestResult = $"Return key type '{SelectedReturnKeyType}' is active. Check the keyboard when focusing the entry field above.";
        ShowTestResult = true;

        await Task.Delay(5000);
        ShowTestResult = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
