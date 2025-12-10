// <copyright file="GradientCircularButtonDemoPage.xaml.cs" company="Velocity Systems">
// Copyright (c) Velocity Systems. All rights reserved.
// </copyright>

#nullable enable

namespace AuroraControls.TestApp.Pages.Buttons;

/// <summary>
/// Demo page for the GradientCircularButton control.
/// </summary>
public partial class GradientCircularButtonDemoPage : ContentPage
{
    private int _clickCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="GradientCircularButtonDemoPage"/> class.
    /// </summary>
    public GradientCircularButtonDemoPage()
    {
        InitializeComponent();
        UpdateCodeExample();
    }

    private void OnButtonClicked(object? sender, EventArgs e)
    {
        _clickCount++;
        ClickFeedback.Text = $"Button clicked {_clickCount} time{(_clickCount == 1 ? string.Empty : "s")}!";
    }

    private void OnPresetDefault(object? sender, EventArgs e)
    {
        PreviewButton.ButtonBackgroundColor = Color.FromArgb("#7C3AED");
        PreviewButton.BorderWidth = 0;
        PreviewButton.FontColor = Colors.White;
        PreviewButton.FontSize = 32;
        PreviewButton.Text = "+";
        PreviewButton.Ripples = true;
        PreviewButton.GradientAngle = 135;

        SizeSlider.Value = 80;
        BorderWidthSlider.Value = 0;
        FontSizeSlider.Value = 32;
        GradientAngleSlider.Value = 135;
        TextEntry.Text = "+";
        RipplesSwitch.IsToggled = true;
        UpdateCodeExample();
    }

    private void OnPresetGradient(object? sender, EventArgs e)
    {
        PreviewButton.ButtonBackgroundColor = Color.FromArgb("#EC4899");
        PreviewButton.BorderWidth = 0;
        PreviewButton.FontColor = Colors.White;
        PreviewButton.FontSize = 28;
        PreviewButton.Text = "★";
        PreviewButton.Ripples = true;
        PreviewButton.GradientAngle = 45;

        SizeSlider.Value = 100;
        BorderWidthSlider.Value = 0;
        FontSizeSlider.Value = 28;
        GradientAngleSlider.Value = 45;
        TextEntry.Text = "★";
        RipplesSwitch.IsToggled = true;
        UpdateCodeExample();
    }

    private void OnPresetBordered(object? sender, EventArgs e)
    {
        PreviewButton.ButtonBackgroundColor = Color.FromArgb("#3B82F6");
        PreviewButton.BorderWidth = 4;
        PreviewButton.BorderColor = Colors.White;
        PreviewButton.FontColor = Colors.White;
        PreviewButton.FontSize = 24;
        PreviewButton.Text = "✓";
        PreviewButton.Ripples = true;
        PreviewButton.GradientAngle = 180;

        SizeSlider.Value = 72;
        BorderWidthSlider.Value = 4;
        FontSizeSlider.Value = 24;
        GradientAngleSlider.Value = 180;
        TextEntry.Text = "✓";
        RipplesSwitch.IsToggled = true;
        UpdateCodeExample();
    }

    private void OnPresetMinimal(object? sender, EventArgs e)
    {
        PreviewButton.ButtonBackgroundColor = Color.FromArgb("#14B8A6");
        PreviewButton.BorderWidth = 0;
        PreviewButton.FontColor = Colors.White;
        PreviewButton.FontSize = 20;
        PreviewButton.Text = "→";
        PreviewButton.Ripples = false;
        PreviewButton.GradientAngle = 90;

        SizeSlider.Value = 56;
        BorderWidthSlider.Value = 0;
        FontSizeSlider.Value = 20;
        GradientAngleSlider.Value = 90;
        TextEntry.Text = "→";
        RipplesSwitch.IsToggled = false;
        UpdateCodeExample();
    }

    private void OnSizeChanged(object? sender, ValueChangedEventArgs e)
    {
        var size = Math.Round(e.NewValue);
        PreviewButton.WidthRequest = size;
        PreviewButton.HeightRequest = size;
        SizeLabel.Text = size.ToString();
        UpdateCodeExample();
    }

    private void OnBorderWidthChanged(object? sender, ValueChangedEventArgs e)
    {
        var width = Math.Round(e.NewValue);
        PreviewButton.BorderWidth = width;
        BorderWidthLabel.Text = width.ToString();
        UpdateCodeExample();
    }

    private void OnBackgroundPurple(object? sender, EventArgs e)
    {
        PreviewButton.ButtonBackgroundColor = Color.FromArgb("#7C3AED");
        UpdateCodeExample();
    }

    private void OnBackgroundPink(object? sender, EventArgs e)
    {
        PreviewButton.ButtonBackgroundColor = Color.FromArgb("#EC4899");
        UpdateCodeExample();
    }

    private void OnBackgroundBlue(object? sender, EventArgs e)
    {
        PreviewButton.ButtonBackgroundColor = Color.FromArgb("#3B82F6");
        UpdateCodeExample();
    }

    private void OnBackgroundTeal(object? sender, EventArgs e)
    {
        PreviewButton.ButtonBackgroundColor = Color.FromArgb("#14B8A6");
        UpdateCodeExample();
    }

    private void OnBackgroundRed(object? sender, EventArgs e)
    {
        PreviewButton.ButtonBackgroundColor = Color.FromArgb("#EF4444");
        UpdateCodeExample();
    }

    private void OnBackgroundOrange(object? sender, EventArgs e)
    {
        PreviewButton.ButtonBackgroundColor = Color.FromArgb("#F59E0B");
        UpdateCodeExample();
    }

    private void OnFontColorWhite(object? sender, EventArgs e)
    {
        PreviewButton.FontColor = Colors.White;
        UpdateCodeExample();
    }

    private void OnFontColorBlack(object? sender, EventArgs e)
    {
        PreviewButton.FontColor = Colors.Black;
        UpdateCodeExample();
    }

    private void OnFontColorPurple(object? sender, EventArgs e)
    {
        PreviewButton.FontColor = Color.FromArgb("#7C3AED");
        UpdateCodeExample();
    }

    private void OnBorderColorWhite(object? sender, EventArgs e)
    {
        PreviewButton.BorderColor = Colors.White;
        UpdateCodeExample();
    }

    private void OnBorderColorPurple(object? sender, EventArgs e)
    {
        PreviewButton.BorderColor = Color.FromArgb("#7C3AED");
        UpdateCodeExample();
    }

    private void OnBorderColorPink(object? sender, EventArgs e)
    {
        PreviewButton.BorderColor = Color.FromArgb("#EC4899");
        UpdateCodeExample();
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        PreviewButton.Text = e.NewTextValue;
        UpdateCodeExample();
    }

    private void OnFontSizeChanged(object? sender, ValueChangedEventArgs e)
    {
        var size = Math.Round(e.NewValue);
        PreviewButton.FontSize = size;
        FontSizeLabel.Text = size.ToString();
        UpdateCodeExample();
    }

    private void OnGradientAngleChanged(object? sender, ValueChangedEventArgs e)
    {
        var angle = Math.Round(e.NewValue);
        PreviewButton.GradientAngle = angle;
        GradientAngleLabel.Text = $"{angle}°";
        UpdateCodeExample();
    }

    private void OnRipplesToggled(object? sender, ToggledEventArgs e)
    {
        PreviewButton.Ripples = e.Value;
        UpdateCodeExample();
    }

    private void OnEnabledToggled(object? sender, ToggledEventArgs e)
    {
        PreviewButton.IsEnabled = e.Value;
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
        var bgColor = PreviewButton.ButtonBackgroundColor.ToArgbHex();
        var fontColor = PreviewButton.FontColor.ToArgbHex();
        var borderColor = PreviewButton.BorderColor.ToArgbHex();

        var code = $@"<aurora:GradientCircularButton
    WidthRequest=""{PreviewButton.WidthRequest}""
    HeightRequest=""{PreviewButton.HeightRequest}""
    Text=""{PreviewButton.Text}""
    FontSize=""{PreviewButton.FontSize}""
    FontColor=""{fontColor}""
    ButtonBackgroundColor=""{bgColor}""
    BorderWidth=""{PreviewButton.BorderWidth}""
    BorderColor=""{borderColor}""
    GradientAngle=""{PreviewButton.GradientAngle}""
    Ripples=""{PreviewButton.Ripples}""
    Clicked=""OnClicked""/>";

        return code;
    }
}
