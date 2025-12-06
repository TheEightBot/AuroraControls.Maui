// <copyright file="StepIndicatorDemoPage.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using AuroraControls.TestApp.Controls;

namespace AuroraControls.TestApp.Pages.Gauges;

/// <summary>
/// Demo page for the StepIndicator control.
/// </summary>
public partial class StepIndicatorDemoPage : ContentPage
{
    private readonly Dictionary<string, Button> _shapeButtons;
    private StepIndicatorShape _currentShape = StepIndicatorShape.Circle;

    /// <summary>
    /// Initializes a new instance of the <see cref="StepIndicatorDemoPage"/> class.
    /// </summary>
    public StepIndicatorDemoPage()
    {
        InitializeComponent();

        _shapeButtons = new Dictionary<string, Button>
        {
            { "Circle", CircleShapeBtn },
            { "Square", SquareShapeBtn },
            { "RoundedSquare", RoundedSquareShapeBtn },
            { "Diamond", DiamondShapeBtn },
            { "Hexagon", HexagonShapeBtn },
            { "Star", StarShapeBtn },
            { "Triangle", TriangleShapeBtn },
            { "Pentagon", PentagonShapeBtn },
        };

        InitializePresets();
        UpdateStepLabel();
    }

    private void InitializePresets()
    {
        PresetSelector.Presets = new ObservableCollection<PresetItem>
        {
            new PresetItem { Name = "Default", Key = "default" },
            new PresetItem { Name = "Wizard", Key = "wizard" },
            new PresetItem { Name = "Checkout", Key = "checkout" },
            new PresetItem { Name = "Minimal", Key = "minimal" },
            new PresetItem { Name = "Playful", Key = "playful" },
            new PresetItem { Name = "Corporate", Key = "corporate" },
        };
    }

    private void UpdateStepLabel()
    {
        StepLabel.Text = $"Step {StepIndicatorPreview.CurrentStep + 1} of {StepIndicatorPreview.NumberOfSteps}";

        // Update button states
        PrevStepButton.IsEnabled = StepIndicatorPreview.CurrentStep > 0;
        NextStepButton.IsEnabled = StepIndicatorPreview.CurrentStep < StepIndicatorPreview.NumberOfSteps - 1;

        // Update button colors based on enabled state
        PrevStepButton.BackgroundColor = PrevStepButton.IsEnabled
            ? Color.FromArgb("#2A2A2A")
            : Color.FromArgb("#1A1A1A");
        PrevStepButton.TextColor = PrevStepButton.IsEnabled
            ? Colors.White
            : Color.FromArgb("#4A4A4A");
    }

    private void OnPrevStepClicked(object sender, EventArgs e)
    {
        if (StepIndicatorPreview.CurrentStep > 0)
        {
            StepIndicatorPreview.CurrentStep--;
            CurrentStepSlider.Value = StepIndicatorPreview.CurrentStep;
            UpdateStepLabel();
        }
    }

    private void OnNextStepClicked(object sender, EventArgs e)
    {
        if (StepIndicatorPreview.CurrentStep < StepIndicatorPreview.NumberOfSteps - 1)
        {
            StepIndicatorPreview.CurrentStep++;
            CurrentStepSlider.Value = StepIndicatorPreview.CurrentStep;
            UpdateStepLabel();
        }
    }

    private void OnNumberOfStepsChanged(object? sender, ValueChangedEventArgs e)
    {
        var steps = (int)e.NewValue;
        StepIndicatorPreview.NumberOfSteps = steps;

        // Adjust current step if needed
        CurrentStepSlider.Maximum = steps - 1;
        if (StepIndicatorPreview.CurrentStep >= steps)
        {
            StepIndicatorPreview.CurrentStep = steps - 1;
            CurrentStepSlider.Value = StepIndicatorPreview.CurrentStep;
        }

        UpdateStepLabel();
    }

    private void OnCurrentStepChanged(object? sender, ValueChangedEventArgs e)
    {
        StepIndicatorPreview.CurrentStep = (int)e.NewValue;
        UpdateStepLabel();
    }

    private void OnLineWidthChanged(object? sender, ValueChangedEventArgs e)
    {
        StepIndicatorPreview.LineWidth = e.NewValue;
    }

    private void OnShapeSelected(object sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        // Determine which shape was selected
        var shapeName = button.Text switch
        {
            "● Circle" => "Circle",
            "■ Square" => "Square",
            "▢ Rounded" => "RoundedSquare",
            "◆ Diamond" => "Diamond",
            "⬡ Hexagon" => "Hexagon",
            "★ Star" => "Star",
            "▲ Triangle" => "Triangle",
            "⬠ Pentagon" => "Pentagon",
            _ => "Circle",
        };

        // Update shape
        _currentShape = Enum.Parse<StepIndicatorShape>(shapeName);
        StepIndicatorPreview.Shape = _currentShape;

        // Update button styles
        UpdateShapeButtonStyles(shapeName);
    }

    private void UpdateShapeButtonStyles(string selectedShape)
    {
        foreach (var kvp in _shapeButtons)
        {
            if (kvp.Key == selectedShape)
            {
                kvp.Value.BackgroundColor = Color.FromArgb("#00D4AA");
                kvp.Value.TextColor = Color.FromArgb("#0F0F0F");
            }
            else
            {
                kvp.Value.BackgroundColor = Color.FromArgb("#2A2A2A");
                kvp.Value.TextColor = Colors.White;
            }
        }
    }

    private void OnHighlightColorChanged(object sender, Color color)
    {
        StepIndicatorPreview.HighlightColor = color;

        // Update Next button color to match
        NextStepButton.BackgroundColor = color;
    }

    private void OnInactiveColorChanged(object sender, Color color)
    {
        StepIndicatorPreview.InactiveColor = color;
    }

    private void OnLineColorChanged(object sender, Color color)
    {
        StepIndicatorPreview.LineColor = color;
    }

    private void OnFontColorChanged(object sender, Color color)
    {
        StepIndicatorPreview.FontColor = color;
    }

    private void OnDisplayStepNumberChanged(object sender, bool value)
    {
        StepIndicatorPreview.DisplayStepNumber = value;
    }

    private void OnDrawConnectingLineChanged(object sender, bool value)
    {
        StepIndicatorPreview.DrawConnectingLine = value;
    }

    private void OnSwitchOnStepTapChanged(object sender, bool value)
    {
        StepIndicatorPreview.SwitchOnStepTap = value;
    }

    private void OnPresetSelected(object sender, PresetItem preset)
    {
        switch (preset.Name)
        {
            case "Default":
                ApplyPreset(
                    steps: 5,
                    currentStep: 2,
                    highlightColor: "#00D4AA",
                    inactiveColor: "#2A2A2A",
                    lineColor: "#3A3A3A",
                    fontColor: "#FFFFFF",
                    lineWidth: 3,
                    shape: StepIndicatorShape.Circle,
                    displayNumber: true,
                    drawLine: true);
                break;

            case "Wizard":
                ApplyPreset(
                    steps: 4,
                    currentStep: 1,
                    highlightColor: "#4A90D9",
                    inactiveColor: "#1E3A5F",
                    lineColor: "#2A5A8A",
                    fontColor: "#FFFFFF",
                    lineWidth: 4,
                    shape: StepIndicatorShape.Circle,
                    displayNumber: true,
                    drawLine: true);
                break;

            case "Checkout":
                ApplyPreset(
                    steps: 4,
                    currentStep: 2,
                    highlightColor: "#4CAF50",
                    inactiveColor: "#2E7D32",
                    lineColor: "#1B5E20",
                    fontColor: "#FFFFFF",
                    lineWidth: 3,
                    shape: StepIndicatorShape.RoundedSquare,
                    displayNumber: true,
                    drawLine: true);
                break;

            case "Minimal":
                ApplyPreset(
                    steps: 5,
                    currentStep: 3,
                    highlightColor: "#FFFFFF",
                    inactiveColor: "#3A3A3A",
                    lineColor: "#2A2A2A",
                    fontColor: "#0F0F0F",
                    lineWidth: 2,
                    shape: StepIndicatorShape.Circle,
                    displayNumber: false,
                    drawLine: true);
                break;

            case "Playful":
                ApplyPreset(
                    steps: 6,
                    currentStep: 2,
                    highlightColor: "#FF6B6B",
                    inactiveColor: "#4A3A6A",
                    lineColor: "#6A4A8A",
                    fontColor: "#FFFFFF",
                    lineWidth: 3,
                    shape: StepIndicatorShape.Star,
                    displayNumber: false,
                    drawLine: true);
                break;

            case "Corporate":
                ApplyPreset(
                    steps: 4,
                    currentStep: 1,
                    highlightColor: "#3498DB",
                    inactiveColor: "#34495E",
                    lineColor: "#2C3E50",
                    fontColor: "#FFFFFF",
                    lineWidth: 2,
                    shape: StepIndicatorShape.Square,
                    displayNumber: true,
                    drawLine: true);
                break;
        }
    }

    private void ApplyPreset(
        int steps,
        int currentStep,
        string highlightColor,
        string inactiveColor,
        string lineColor,
        string fontColor,
        double lineWidth,
        StepIndicatorShape shape,
        bool displayNumber,
        bool drawLine)
    {
        // Update control
        StepIndicatorPreview.NumberOfSteps = steps;
        StepIndicatorPreview.CurrentStep = currentStep;
        StepIndicatorPreview.HighlightColor = Color.FromArgb(highlightColor);
        StepIndicatorPreview.InactiveColor = Color.FromArgb(inactiveColor);
        StepIndicatorPreview.LineColor = Color.FromArgb(lineColor);
        StepIndicatorPreview.FontColor = Color.FromArgb(fontColor);
        StepIndicatorPreview.LineWidth = lineWidth;
        StepIndicatorPreview.Shape = shape;
        StepIndicatorPreview.DisplayStepNumber = displayNumber;
        StepIndicatorPreview.DrawConnectingLine = drawLine;

        // Update editors
        NumberOfStepsSlider.Value = steps;
        CurrentStepSlider.Maximum = steps - 1;
        CurrentStepSlider.Value = currentStep;
        LineWidthSlider.Value = lineWidth;

        HighlightColorPicker.SelectedColor = Color.FromArgb(highlightColor);
        InactiveColorPicker.SelectedColor = Color.FromArgb(inactiveColor);
        LineColorPicker.SelectedColor = Color.FromArgb(lineColor);
        FontColorPicker.SelectedColor = Color.FromArgb(fontColor);

        DisplayStepNumberToggle.Value = displayNumber;
        DrawConnectingLineToggle.Value = drawLine;

        _currentShape = shape;
        UpdateShapeButtonStyles(shape.ToString());

        // Update Next button color to match highlight
        NextStepButton.BackgroundColor = Color.FromArgb(highlightColor);

        UpdateStepLabel();
    }
}
