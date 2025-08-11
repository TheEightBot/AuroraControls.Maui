using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AuroraControls.TestApp;

public partial class ShowKeyboardDoneButtonEffectTestPage : ContentPage
{
    public ShowKeyboardDoneButtonEffectTestPage()
    {
        InitializeComponent();
        BindingContext = new ShowKeyboardDoneButtonEffectViewModel();
    }
}

public class ShowKeyboardDoneButtonEffectViewModel : INotifyPropertyChanged
{
    private string _regularText = string.Empty;
    private string _enhancedText = string.Empty;
    private string _regularNumber = string.Empty;
    private string _enhancedNumber = string.Empty;
    private string _regularEmail = string.Empty;
    private string _enhancedEmail = string.Empty;
    private string _regularPhone = string.Empty;
    private string _enhancedPhone = string.Empty;
    private string _regularUrl = string.Empty;
    private string _enhancedUrl = string.Empty;
    private string _regularEditorText = string.Empty;
    private string _enhancedEditorText = string.Empty;
    private string _testFeedback = string.Empty;
    private bool _showTestFeedback;

    public ShowKeyboardDoneButtonEffectViewModel()
    {
        ShowInstructionsCommand = new Command(OnShowInstructions);
    }

    public string RegularText
    {
        get => _regularText;
        set
        {
            _regularText = value;
            OnPropertyChanged();
        }
    }

    public string EnhancedText
    {
        get => _enhancedText;
        set
        {
            _enhancedText = value;
            OnPropertyChanged();
        }
    }

    public string RegularNumber
    {
        get => _regularNumber;
        set
        {
            _regularNumber = value;
            OnPropertyChanged();
        }
    }

    public string EnhancedNumber
    {
        get => _enhancedNumber;
        set
        {
            _enhancedNumber = value;
            OnPropertyChanged();
        }
    }

    public string RegularEmail
    {
        get => _regularEmail;
        set
        {
            _regularEmail = value;
            OnPropertyChanged();
        }
    }

    public string EnhancedEmail
    {
        get => _enhancedEmail;
        set
        {
            _enhancedEmail = value;
            OnPropertyChanged();
        }
    }

    public string RegularPhone
    {
        get => _regularPhone;
        set
        {
            _regularPhone = value;
            OnPropertyChanged();
        }
    }

    public string EnhancedPhone
    {
        get => _enhancedPhone;
        set
        {
            _enhancedPhone = value;
            OnPropertyChanged();
        }
    }

    public string RegularUrl
    {
        get => _regularUrl;
        set
        {
            _regularUrl = value;
            OnPropertyChanged();
        }
    }

    public string EnhancedUrl
    {
        get => _enhancedUrl;
        set
        {
            _enhancedUrl = value;
            OnPropertyChanged();
        }
    }

    public string RegularEditorText
    {
        get => _regularEditorText;
        set
        {
            _regularEditorText = value;
            OnPropertyChanged();
        }
    }

    public string EnhancedEditorText
    {
        get => _enhancedEditorText;
        set
        {
            _enhancedEditorText = value;
            OnPropertyChanged();
        }
    }

    public string TestFeedback
    {
        get => _testFeedback;
        set
        {
            _testFeedback = value;
            OnPropertyChanged();
        }
    }

    public bool ShowTestFeedback
    {
        get => _showTestFeedback;
        set
        {
            _showTestFeedback = value;
            OnPropertyChanged();
        }
    }

    public string PlatformInfo =>
        $"{DeviceInfo.Platform} - Effect {(DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.macOS ? "ACTIVE" : "INACTIVE")}";

    public ICommand ShowInstructionsCommand { get; }

    private async void OnShowInstructions()
    {
        var platform = DeviceInfo.Platform;
        var isSupported = platform == DevicePlatform.iOS || platform == DevicePlatform.macOS;

        if (isSupported)
        {
            TestFeedback = "✅ Platform supported! Try tapping the fields on the right side. You should see a 'Done' button above the keyboard that dismisses it when tapped. Compare with the left side fields that don't have this button.";
        }
        else
        {
            TestFeedback = $"ℹ️ Current platform ({platform}) doesn't support this effect. The effect only works on iOS and macOS. On this platform, both sides will behave identically.";
        }

        ShowTestFeedback = true;

        // Hide the message after 8 seconds
        await Task.Delay(8000);
        ShowTestFeedback = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
