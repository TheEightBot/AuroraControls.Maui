using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AuroraControls.TestApp;

public partial class ConfettiViewTestPage : ContentPage, INotifyPropertyChanged
{
    public ConfettiViewTestPage()
    {
        InitializeComponent();
        UpdateConfigLabel();
    }

    private void OnStartClicked(object sender, EventArgs e)
    {
        ConfettiView.Start();
        StatusLabel.Text = "Confetti animation started!";
    }

    private void OnStopClicked(object sender, EventArgs e)
    {
        ConfettiView.Stop();
        StatusLabel.Text = "Confetti animation stopped.";
    }

    private void OnMaxParticlesChanged(object sender, ValueChangedEventArgs e)
    {
        if (ConfettiView != null)
        {
            ConfettiView.MaxParticles = (int)e.NewValue;
            UpdateConfigLabel();
            StatusLabel.Text = $"Max particles updated to {(int)e.NewValue}";
        }
    }

    private void OnShapeChanged(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            if (button.Text == "Rectangular")
            {
                ConfettiView.ConfettiShape = ConfettiShape.Rectangular;
                RectangularButton.BackgroundColor = Colors.Blue;
                RectangularButton.TextColor = Colors.White;
                CircularButton.BackgroundColor = Colors.LightGray;
                CircularButton.TextColor = Colors.Black;
                StatusLabel.Text = "Shape changed to Rectangular";
            }
            else if (button.Text == "Circular")
            {
                ConfettiView.ConfettiShape = ConfettiShape.Circular;
                CircularButton.BackgroundColor = Colors.Blue;
                CircularButton.TextColor = Colors.White;
                RectangularButton.BackgroundColor = Colors.LightGray;
                RectangularButton.TextColor = Colors.Black;
                StatusLabel.Text = "Shape changed to Circular";
            }

            UpdateConfigLabel();
        }
    }

    private void OnContinuousToggled(object sender, ToggledEventArgs e)
    {
        ConfettiView.Continuous = e.Value;
        UpdateConfigLabel();
        StatusLabel.Text = e.Value ? "Continuous mode enabled" : "One-time mode enabled";
    }

    private void OnLightShowClicked(object sender, EventArgs e)
    {
        // Light show preset: fewer particles, circular, continuous
        MaxParticlesSlider.Value = 150;
        ConfettiView.ConfettiShape = ConfettiShape.Circular;
        ConfettiView.Continuous = true;
        ContinuousSwitch.IsToggled = true;

        // Update button states
        CircularButton.BackgroundColor = Colors.Blue;
        CircularButton.TextColor = Colors.White;
        RectangularButton.BackgroundColor = Colors.LightGray;
        RectangularButton.TextColor = Colors.Black;

        ConfettiView.Start();
        StatusLabel.Text = "Light Show preset applied and started!";
        UpdateConfigLabel();
    }

    private void OnCelebrationClicked(object sender, EventArgs e)
    {
        // Celebration preset: many particles, rectangular, one-time burst
        MaxParticlesSlider.Value = 400;
        ConfettiView.ConfettiShape = ConfettiShape.Rectangular;
        ConfettiView.Continuous = false;
        ContinuousSwitch.IsToggled = false;

        // Update button states
        RectangularButton.BackgroundColor = Colors.Blue;
        RectangularButton.TextColor = Colors.White;
        CircularButton.BackgroundColor = Colors.LightGray;
        CircularButton.TextColor = Colors.Black;

        ConfettiView.Start();
        StatusLabel.Text = "Celebration preset applied and started!";
        UpdateConfigLabel();
    }

    private void OnPartyModeClicked(object sender, EventArgs e)
    {
        // Party mode preset: maximum particles, mixed shapes (start with circular), continuous
        MaxParticlesSlider.Value = 500;
        ConfettiView.ConfettiShape = ConfettiShape.Circular;
        ConfettiView.Continuous = true;
        ContinuousSwitch.IsToggled = true;

        // Update button states
        CircularButton.BackgroundColor = Colors.Blue;
        CircularButton.TextColor = Colors.White;
        RectangularButton.BackgroundColor = Colors.LightGray;
        RectangularButton.TextColor = Colors.Black;

        ConfettiView.Start();
        StatusLabel.Text = "Party Mode preset applied and started!";
        UpdateConfigLabel();
    }

    private void UpdateConfigLabel()
    {
        if (ConfettiView != null && ConfigLabel != null)
        {
            var shapeText = ConfettiView.ConfettiShape == ConfettiShape.Rectangular ? "Rectangular" : "Circular";
            var modeText = ConfettiView.Continuous ? "Continuous mode" : "One-time mode";
            ConfigLabel.Text = $"Current: {ConfettiView.MaxParticles} particles, {shapeText} shape, {modeText}";
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
