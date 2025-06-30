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

    private void OnParticleSizeChanged(object sender, ValueChangedEventArgs e)
    {
        if (ConfettiView != null)
        {
            ConfettiView.ParticleSize = e.NewValue;
            UpdateConfigLabel();
            StatusLabel.Text = $"Particle size updated to {e.NewValue:F1}";
        }
    }

    private void OnGravityChanged(object sender, ValueChangedEventArgs e)
    {
        if (ConfettiView != null)
        {
            ConfettiView.Gravity = e.NewValue;
            UpdateConfigLabel();
            StatusLabel.Text = $"Gravity updated to {e.NewValue:F2}";
        }
    }

    private void OnWindIntensityChanged(object sender, ValueChangedEventArgs e)
    {
        if (ConfettiView != null)
        {
            ConfettiView.WindIntensity = e.NewValue;
            UpdateConfigLabel();
            StatusLabel.Text = $"Wind intensity updated to {e.NewValue:F1}";
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

    private void OnFadeOutToggled(object sender, ToggledEventArgs e)
    {
        ConfettiView.FadeOut = e.Value;
        UpdateConfigLabel();
        StatusLabel.Text = e.Value ? "Fade out effect enabled" : "Fade out effect disabled";
    }

    private void OnBurstModeToggled(object sender, ToggledEventArgs e)
    {
        ConfettiView.BurstMode = e.Value;
        UpdateConfigLabel();
        StatusLabel.Text = e.Value ? "Burst mode enabled - particles explode from center" : "Normal falling mode enabled";
    }

    private void OnColorThemeChanged(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            // Reset all color theme buttons
            RandomColorsButton.BackgroundColor = Colors.LightGray;
            RandomColorsButton.TextColor = Colors.Black;
            RainbowButton.BackgroundColor = Colors.LightGray;
            RainbowButton.TextColor = Colors.Black;
            PastelButton.BackgroundColor = Colors.LightGray;
            PastelButton.TextColor = Colors.Black;
            GoldButton.BackgroundColor = Colors.LightGray;
            GoldButton.TextColor = Colors.Black;

            // Set selected button as active
            button.BackgroundColor = Colors.Blue;
            button.TextColor = Colors.White;

            // Apply color theme
            switch (button.Text)
            {
                case "Random":
                    ConfettiView.Colors = null; // Use random colors
                    StatusLabel.Text = "Color theme: Random colors";
                    break;

                case "Rainbow":
                    ConfettiView.Colors = new List<Color>
                    {
                        Colors.Red, Colors.Orange, Colors.Yellow,
                        Colors.Green, Colors.Blue, Colors.Purple,
                    };
                    StatusLabel.Text = "Color theme: Rainbow colors";
                    break;

                case "Pastel":
                    ConfettiView.Colors = new List<Color>
                    {
                        Color.FromRgb(255, 182, 193), // Light pink
                        Color.FromRgb(173, 216, 230), // Light blue
                        Color.FromRgb(144, 238, 144), // Light green
                        Color.FromRgb(255, 218, 185), // Peach
                        Color.FromRgb(221, 160, 221),  // Plum
                    };
                    StatusLabel.Text = "Color theme: Pastel colors";
                    break;

                case "Gold":
                    ConfettiView.Colors = new List<Color>
                    {
                        Color.FromRgb(255, 215, 0),   // Gold
                        Color.FromRgb(255, 165, 0),   // Orange
                        Color.FromRgb(255, 140, 0),   // Dark orange
                        Color.FromRgb(218, 165, 32),  // Goldenrod
                        Color.FromRgb(255, 228, 181),  // Moccasin
                    };
                    StatusLabel.Text = "Color theme: Gold colors";
                    break;
            }

            UpdateConfigLabel();
        }
    }

    private void OnLightShowClicked(object sender, EventArgs e)
    {
        // Light show preset: fewer particles, smaller size, circular, continuous
        MaxParticlesSlider.Value = 150;
        ParticleSizeSlider.Value = 4.0; // Smaller particles for light show
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
        // Celebration preset: many particles, medium size, rectangular, one-time burst
        MaxParticlesSlider.Value = 400;
        ParticleSizeSlider.Value = 8.0; // Medium-large particles for celebration
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
        // Party mode preset: maximum particles, large size, circular, continuous
        MaxParticlesSlider.Value = 500;
        ParticleSizeSlider.Value = 12.0; // Large particles for party mode
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
            var sizeText = ConfettiView.ParticleSize.ToString("F1");
            ConfigLabel.Text = $"Current: {ConfettiView.MaxParticles} particles, Size {sizeText}, {shapeText} shape, {modeText}";
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
