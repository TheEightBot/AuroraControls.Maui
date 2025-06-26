using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AuroraControls.TestApp;

public partial class GradientCircularButtonTestPage : ContentPage, INotifyPropertyChanged
{
    public GradientCircularButtonTestPage()
    {
        InitializeComponent();
        TestCommand = new Command<string>(OnTestCommandExecuted);
        BindingContext = this;
    }

    public ICommand TestCommand { get; }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        if (sender is AuroraControls.GradientCircularButton button)
        {
            var text = button.Text ?? "Unknown";
            var backgroundColor = button.ButtonBackgroundColor;
            var ripples = button.Ripples ? "with ripples" : "without ripples";
            var angle = button.GradientAngle;
            StatusLabel.Text = $"Clicked: '{text}' button ({backgroundColor}, {ripples}, {angle}°)";
        }
    }

    private void OnTestCommandExecuted(string parameter)
    {
        StatusLabel.Text = $"Command executed: {parameter}";
    }

    private void OnAngleChanged(object sender, ValueChangedEventArgs e)
    {
        AngleLabel.Text = $"Gradient Angle: {e.NewValue:F0}°";
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
