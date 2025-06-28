using Microsoft.Maui.Controls;

namespace AuroraControls.TestApp;

public partial class StepIndicatorTestPage : ContentPage
{
    private readonly Color[] _colorOptions =
    {
        Colors.Red, Colors.Blue, Colors.Green, Colors.Orange, Colors.Purple,
        Colors.Teal, Colors.Pink, Colors.Brown, Colors.Gray, Colors.Black,
    };

    private int _currentColorIndex = 0;

    public StepIndicatorTestPage()
    {
        InitializeComponent();
    }

    private void OnNumberOfStepsChanged(object sender, ValueChangedEventArgs e)
    {
        var newValue = (int)e.NewValue;
        NumberOfStepsLabel.Text = newValue.ToString();
        BasicStepIndicator.NumberOfSteps = newValue;

        // Update current step slider maximum
        CurrentStepSlider.Maximum = Math.Max(0, newValue - 1);
        if (BasicStepIndicator.CurrentStep >= newValue)
        {
            BasicStepIndicator.CurrentStep = Math.Max(0, newValue - 1);
            CurrentStepSlider.Value = BasicStepIndicator.CurrentStep;
            CurrentStepLabel.Text = BasicStepIndicator.CurrentStep.ToString();
        }
    }

    private void OnCurrentStepChanged(object sender, ValueChangedEventArgs e)
    {
        var newValue = (int)e.NewValue;
        CurrentStepLabel.Text = newValue.ToString();
        BasicStepIndicator.CurrentStep = newValue;
    }

    private void OnLineWidthChanged(object sender, ValueChangedEventArgs e)
    {
        var newValue = (int)e.NewValue;
        LineWidthLabel.Text = newValue.ToString();
        BasicStepIndicator.LineWidth = newValue;
    }

    private void OnDisplayStepNumberChanged(object sender, CheckedChangedEventArgs e)
    {
        BasicStepIndicator.DisplayStepNumber = e.Value;
    }

    private void OnDrawConnectingLineChanged(object sender, CheckedChangedEventArgs e)
    {
        BasicStepIndicator.DrawConnectingLine = e.Value;
    }

    private void OnSwitchOnStepTapChanged(object sender, CheckedChangedEventArgs e)
    {
        BasicStepIndicator.SwitchOnStepTap = e.Value;
    }

    private void OnLineColorClicked(object sender, EventArgs e)
    {
        var newColor = GetNextColor();
        BasicStepIndicator.LineColor = newColor;
        if (sender is Button button)
        {
            button.BackgroundColor = newColor;
            button.TextColor = GetContrastColor(newColor);
        }
    }

    private void OnHighlightColorClicked(object sender, EventArgs e)
    {
        var newColor = GetNextColor();
        BasicStepIndicator.HighlightColor = newColor;
        if (sender is Button button)
        {
            button.BackgroundColor = newColor;
            button.TextColor = GetContrastColor(newColor);
        }
    }

    private void OnInactiveColorClicked(object sender, EventArgs e)
    {
        var newColor = GetNextColor();
        BasicStepIndicator.InactiveColor = newColor;
        if (sender is Button button)
        {
            button.BackgroundColor = newColor;
            button.TextColor = GetContrastColor(newColor);
        }
    }

    private void OnFontColorClicked(object sender, EventArgs e)
    {
        var newColor = GetNextColor();
        BasicStepIndicator.FontColor = newColor;
        if (sender is Button button)
        {
            button.BackgroundColor = newColor;
            button.TextColor = GetContrastColor(newColor);
        }
    }

    private void OnPreviousStepClicked(object sender, EventArgs e)
    {
        if (BasicStepIndicator.CurrentStep > 0)
        {
            BasicStepIndicator.CurrentStep--;
            CurrentStepSlider.Value = BasicStepIndicator.CurrentStep;
            CurrentStepLabel.Text = BasicStepIndicator.CurrentStep.ToString();
        }
    }

    private void OnNextStepClicked(object sender, EventArgs e)
    {
        if (BasicStepIndicator.CurrentStep < BasicStepIndicator.NumberOfSteps - 1)
        {
            BasicStepIndicator.CurrentStep++;
            CurrentStepSlider.Value = BasicStepIndicator.CurrentStep;
            CurrentStepLabel.Text = BasicStepIndicator.CurrentStep.ToString();
        }
    }

    private void OnResetClicked(object sender, EventArgs e)
    {
        BasicStepIndicator.CurrentStep = 0;
        CurrentStepSlider.Value = 0;
        CurrentStepLabel.Text = "0";

        // Reset interactive step indicator as well
        InteractiveStepIndicator.CurrentStep = 0;
    }

    private Color GetNextColor()
    {
        var color = _colorOptions[_currentColorIndex];
        _currentColorIndex = (_currentColorIndex + 1) % _colorOptions.Length;
        return color;
    }

    private static Color GetContrastColor(Color backgroundColor)
    {
        // Simple contrast calculation - if the color is dark, use white text, otherwise black
        var luminance = (0.299 * backgroundColor.Red) + (0.587 * backgroundColor.Green) + (0.114 * backgroundColor.Blue);
        return luminance > 0.5 ? Colors.Black : Colors.White;
    }
}
