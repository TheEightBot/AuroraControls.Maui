using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AuroraControls.TestApp;

public partial class SvgImageButtonTestPage : ContentPage, INotifyPropertyChanged
{
    public SvgImageButtonTestPage()
    {
        InitializeComponent();
        TestCommand = new Command<string>(OnTestCommandExecuted);
        BindingContext = this;
    }

    public ICommand TestCommand { get; }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        if (sender is AuroraControls.SvgImageButton button)
        {
            var imageName = button.EmbeddedImageName ?? "Unknown";
            var shape = button.BackgroundShape.ToString();
            StatusLabel.Text = $"Clicked: {imageName} ({shape} background)";
        }
    }

    private void OnTestCommandExecuted(string parameter)
    {
        StatusLabel.Text = $"Command executed: {parameter}";
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
