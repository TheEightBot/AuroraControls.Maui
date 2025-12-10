// <copyright file="ToggleBoxDemoPage.xaml.cs" company="Velocity Systems">
// Copyright (c) Velocity Systems. All rights reserved.
// </copyright>

#nullable enable

namespace AuroraControls.TestApp.Pages.Inputs;

/// <summary>
/// Demo page for the ToggleBox control.
/// </summary>
public partial class ToggleBoxDemoPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToggleBoxDemoPage"/> class.
    /// </summary>
    public ToggleBoxDemoPage()
    {
        InitializeComponent();
        ShapePicker.SelectedIndex = 2; // RoundedSquare
        CheckTypePicker.SelectedIndex = 1; // Check
        UpdateCodeExample();
    }

    private void OnToggleChanged(object? sender, bool e)
    {
        ToggleStatus.Text = $"Toggle is: {(e ? "ON" : "OFF")}";
        ToggledSwitch.IsToggled = e;
        UpdateCodeExample();
    }

    private void OnPresetCheckbox(object? sender, EventArgs e)
    {
        PreviewToggle.Shape = ToggleBoxShape.RoundedSquare;
        PreviewToggle.CheckType = ToggleBoxCheckType.Check;
        PreviewToggle.BorderColor = Color.FromArgb("#7C3AED");
        PreviewToggle.ToggledBackgroundColor = Color.FromArgb("#7C3AED");
        PreviewToggle.CheckColor = Colors.White;
        PreviewToggle.BorderWidth = 3;

        ShapePicker.SelectedIndex = 2;
        CheckTypePicker.SelectedIndex = 1;
        SizeSlider.Value = 48;
        BorderWidthSlider.Value = 3;
        UpdateCodeExample();
    }

    private void OnPresetRadio(object? sender, EventArgs e)
    {
        PreviewToggle.Shape = ToggleBoxShape.Circular;
        PreviewToggle.CheckType = ToggleBoxCheckType.Circular;
        PreviewToggle.BorderColor = Color.FromArgb("#7C3AED");
        PreviewToggle.ToggledBackgroundColor = Colors.Transparent;
        PreviewToggle.CheckColor = Color.FromArgb("#7C3AED");
        PreviewToggle.BorderWidth = 3;

        ShapePicker.SelectedIndex = 1;
        CheckTypePicker.SelectedIndex = 3;
        SizeSlider.Value = 32;
        BorderWidthSlider.Value = 3;
        UpdateCodeExample();
    }

    private void OnPresetSquare(object? sender, EventArgs e)
    {
        PreviewToggle.Shape = ToggleBoxShape.Square;
        PreviewToggle.CheckType = ToggleBoxCheckType.Cross;
        PreviewToggle.BorderColor = Color.FromArgb("#6B7280");
        PreviewToggle.ToggledBackgroundColor = Colors.Transparent;
        PreviewToggle.CheckColor = Color.FromArgb("#3B82F6");
        PreviewToggle.BorderWidth = 2;

        ShapePicker.SelectedIndex = 0;
        CheckTypePicker.SelectedIndex = 0;
        SizeSlider.Value = 40;
        BorderWidthSlider.Value = 2;
        UpdateCodeExample();
    }

    private void OnPresetFilled(object? sender, EventArgs e)
    {
        PreviewToggle.Shape = ToggleBoxShape.RoundedSquare;
        PreviewToggle.CheckType = ToggleBoxCheckType.RoundedCheck;
        PreviewToggle.BorderColor = Colors.Transparent;
        PreviewToggle.ToggledBackgroundColor = Color.FromArgb("#22C55E");
        PreviewToggle.BackgroundColor = Color.FromArgb("#E5E7EB");
        PreviewToggle.CheckColor = Colors.White;
        PreviewToggle.BorderWidth = 0;

        ShapePicker.SelectedIndex = 2;
        CheckTypePicker.SelectedIndex = 2;
        SizeSlider.Value = 48;
        BorderWidthSlider.Value = 0;
        UpdateCodeExample();
    }

    private void OnShapeChanged(object? sender, EventArgs e)
    {
        if (ShapePicker.SelectedIndex < 0)
        {
            return;
        }

        PreviewToggle.Shape = ShapePicker.SelectedIndex switch
        {
            0 => ToggleBoxShape.Square,
            1 => ToggleBoxShape.Circular,
            2 => ToggleBoxShape.RoundedSquare,
            _ => ToggleBoxShape.RoundedSquare,
        };
        UpdateCodeExample();
    }

    private void OnCheckTypeChanged(object? sender, EventArgs e)
    {
        if (CheckTypePicker.SelectedIndex < 0)
        {
            return;
        }

        PreviewToggle.CheckType = CheckTypePicker.SelectedIndex switch
        {
            0 => ToggleBoxCheckType.Cross,
            1 => ToggleBoxCheckType.Check,
            2 => ToggleBoxCheckType.RoundedCheck,
            3 => ToggleBoxCheckType.Circular,
            _ => ToggleBoxCheckType.Check,
        };
        UpdateCodeExample();
    }

    private void OnSizeChanged(object? sender, ValueChangedEventArgs e)
    {
        var size = Math.Round(e.NewValue);
        PreviewToggle.WidthRequest = size;
        PreviewToggle.HeightRequest = size;
        SizeLabel.Text = size.ToString();
        UpdateCodeExample();
    }

    private void OnBorderWidthChanged(object? sender, ValueChangedEventArgs e)
    {
        var width = (int)Math.Round(e.NewValue);
        PreviewToggle.BorderWidth = width;
        BorderWidthLabel.Text = width.ToString();
        UpdateCodeExample();
    }

    private void OnToggledBgPurple(object? sender, EventArgs e)
    {
        PreviewToggle.ToggledBackgroundColor = Color.FromArgb("#7C3AED");
        UpdateCodeExample();
    }

    private void OnToggledBgPink(object? sender, EventArgs e)
    {
        PreviewToggle.ToggledBackgroundColor = Color.FromArgb("#EC4899");
        UpdateCodeExample();
    }

    private void OnToggledBgBlue(object? sender, EventArgs e)
    {
        PreviewToggle.ToggledBackgroundColor = Color.FromArgb("#3B82F6");
        UpdateCodeExample();
    }

    private void OnToggledBgTeal(object? sender, EventArgs e)
    {
        PreviewToggle.ToggledBackgroundColor = Color.FromArgb("#14B8A6");
        UpdateCodeExample();
    }

    private void OnToggledBgGreen(object? sender, EventArgs e)
    {
        PreviewToggle.ToggledBackgroundColor = Color.FromArgb("#22C55E");
        UpdateCodeExample();
    }

    private void OnCheckColorWhite(object? sender, EventArgs e)
    {
        PreviewToggle.CheckColor = Colors.White;
        UpdateCodeExample();
    }

    private void OnCheckColorBlack(object? sender, EventArgs e)
    {
        PreviewToggle.CheckColor = Colors.Black;
        UpdateCodeExample();
    }

    private void OnCheckColorPurple(object? sender, EventArgs e)
    {
        PreviewToggle.CheckColor = Color.FromArgb("#7C3AED");
        UpdateCodeExample();
    }

    private void OnBorderColorPurple(object? sender, EventArgs e)
    {
        PreviewToggle.BorderColor = Color.FromArgb("#7C3AED");
        UpdateCodeExample();
    }

    private void OnBorderColorGray(object? sender, EventArgs e)
    {
        PreviewToggle.BorderColor = Color.FromArgb("#6B7280");
        UpdateCodeExample();
    }

    private void OnBorderColorBlue(object? sender, EventArgs e)
    {
        PreviewToggle.BorderColor = Color.FromArgb("#3B82F6");
        UpdateCodeExample();
    }

    private void OnIsToggledChanged(object? sender, ToggledEventArgs e)
    {
        PreviewToggle.IsToggled = e.Value;
        UpdateCodeExample();
    }

    private void OnEnabledToggled(object? sender, ToggledEventArgs e)
    {
        PreviewToggle.IsEnabled = e.Value;
        UpdateCodeExample();
    }

    private async void OnCopyCode(object? sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(GenerateCodeExample());
        if (sender is Button button)
        {
            var originalText = button.Text;
            button.Text = "Copied!";
            await Task.Delay(1500);
            button.Text = originalText;
        }
    }

    private void UpdateCodeExample()
    {
        CodeLabel.Text = GenerateCodeExample();
    }

    private string GenerateCodeExample()
    {
        var borderColor = PreviewToggle.BorderColor.ToArgbHex();
        var toggledBgColor = PreviewToggle.ToggledBackgroundColor.ToArgbHex();
        var checkColor = PreviewToggle.CheckColor.ToArgbHex();

        var code = $@"<aurora:ToggleBox
    WidthRequest=""{PreviewToggle.WidthRequest}""
    HeightRequest=""{PreviewToggle.HeightRequest}""
    Shape=""{PreviewToggle.Shape}""
    CheckType=""{PreviewToggle.CheckType}""
    IsToggled=""{PreviewToggle.IsToggled}""
    BorderColor=""{borderColor}""
    ToggledBackgroundColor=""{toggledBgColor}""
    CheckColor=""{checkColor}""
    BorderWidth=""{PreviewToggle.BorderWidth}""
    Toggled=""OnToggled""/>";

        return code;
    }
}
