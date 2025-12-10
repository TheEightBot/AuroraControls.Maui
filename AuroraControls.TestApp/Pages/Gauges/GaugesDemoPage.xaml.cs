// <copyright file="GaugesDemoPage.xaml.cs" company="Velocity Systems">
// Copyright (c) Velocity Systems. All rights reserved.
// </copyright>

using AuroraControls.Gauges;

namespace AuroraControls.TestApp.Pages.Gauges;

/// <summary>
/// Demo page for Gauge controls and Loading indicators.
/// </summary>
public partial class GaugesDemoPage : ContentPage
{
    private bool _animationsRunning = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="GaugesDemoPage"/> class.
    /// </summary>
    public GaugesDemoPage()
    {
        this.InitializeComponent();
        this.InitializeControls();
    }

    private void InitializeControls()
    {
        // Setup orientation picker
        this.LinearOrientationPicker.SelectedIndex = 0;

        // Start loading animations
        this.RainbowRingControl.Start();
        this.MaterialCircularControl.Start();
        this.CupertinoControl.Start();
        this.NofriendoControl.Start();
        this.WavesControl.Start();
    }

    private void OnCircularProgressChanged(object? sender, ValueChangedEventArgs e)
    {
        this.CircularGaugeControl.EndingDegree = e.NewValue;
        var percentage = (int)((e.NewValue / 360.0) * 100);
        this.CircularGaugeLabel.Text = $"{percentage}%";
    }

    private void OnCircularThicknessChanged(object? sender, ValueChangedEventArgs e)
    {
        this.CircularGaugeControl.ProgressThickness = e.NewValue;
    }

    private void OnCircularProgressColorChanged(object? sender, Color e)
    {
        this.CircularGaugeControl.ProgressColor = e;
    }

    private void OnCircularBackgroundColorChanged(object? sender, Color e)
    {
        this.CircularGaugeControl.ProgressBackgroundColor = e;
    }

    private void OnCircularFillProgressChanged(object? sender, ValueChangedEventArgs e)
    {
        this.CircularFillGaugeControl.ProgressPercentage = e.NewValue;
        this.CircularFillGaugeLabel.Text = $"{(int)e.NewValue}%";
    }

    private void OnCircularFillColorChanged(object? sender, Color e)
    {
        this.CircularFillGaugeControl.ProgressColor = e;
    }

    private void OnLinearProgressChanged(object? sender, ValueChangedEventArgs e)
    {
        this.LinearGaugeControl.EndingPercent = e.NewValue;
        this.LinearGaugeLabel.Text = $"{(int)e.NewValue}%";
    }

    private void OnLinearThicknessChanged(object? sender, ValueChangedEventArgs e)
    {
        this.LinearGaugeControl.ProgressThickness = e.NewValue;
    }

    private void OnLinearOrientationChanged(object? sender, EventArgs e)
    {
        if (this.LinearOrientationPicker.SelectedIndex < 0)
        {
            return;
        }

        var orientations = new[]
        {
            LinearGaugeOrientation.Horizontal,
            LinearGaugeOrientation.ReverseHorizontal,
            LinearGaugeOrientation.Vertical,
            LinearGaugeOrientation.ReverseVertical,
        };

        this.LinearGaugeControl.Orientation = orientations[this.LinearOrientationPicker.SelectedIndex];
    }

    private void OnLinearProgressColorChanged(object? sender, Color e)
    {
        this.LinearGaugeControl.ProgressColor = e;
    }

    private void OnLoadingColorChanged(object? sender, Color e)
    {
        // Note: Different loading indicators have different color properties
        // MaterialCircular uses ForegroundLoadingColor
        // RainbowRing uses fixed rainbow colors
        // Others may have different properties
        this.MaterialCircularControl.ForegroundLoadingColor = e;
    }

    private void OnToggleAnimationsClicked(object? sender, EventArgs e)
    {
        this._animationsRunning = !this._animationsRunning;

        if (this._animationsRunning)
        {
            this.RainbowRingControl.Start();
            this.MaterialCircularControl.Start();
            this.CupertinoControl.Start();
            this.NofriendoControl.Start();
            this.WavesControl.Start();
        }
        else
        {
            this.RainbowRingControl.Stop();
            this.MaterialCircularControl.Stop();
            this.CupertinoControl.Stop();
            this.NofriendoControl.Stop();
            this.WavesControl.Stop();
        }

        this.AnimationToggleButton.Text = this._animationsRunning ? "⏸ Pause" : "▶ Play";
    }

    private async void OnCopyCodeClicked(object? sender, EventArgs e)
    {
        var xaml = $@"<!-- Circular Gauge -->
<gauges:CircularGauge
    StartingDegree=""0""
    EndingDegree=""{this.CircularGaugeControl.EndingDegree}""
    ProgressThickness=""{this.CircularGaugeControl.ProgressThickness}""
    ProgressColor=""{this.CircularGaugeControl.ProgressColor.ToArgbHex()}""
    ProgressBackgroundColor=""{this.CircularGaugeControl.ProgressBackgroundColor.ToArgbHex()}""
    EndCapType=""Rounded""/>

<!-- Circular Fill Gauge -->
<gauges:CircularFillGauge
    ProgressPercentage=""{this.CircularFillGaugeControl.ProgressPercentage}""
    ProgressColor=""{this.CircularFillGaugeControl.ProgressColor.ToArgbHex()}""
    ProgressBackgroundColor=""{this.CircularFillGaugeControl.ProgressBackgroundColor.ToArgbHex()}""/>

<!-- Linear Gauge -->
<gauges:LinearGauge
    StartingPercent=""0""
    EndingPercent=""{this.LinearGaugeControl.EndingPercent}""
    ProgressThickness=""{this.LinearGaugeControl.ProgressThickness}""
    ProgressColor=""{this.LinearGaugeControl.ProgressColor.ToArgbHex()}""
    Orientation=""{this.LinearGaugeControl.Orientation}""
    EndCapType=""Rounded""/>";

        await Clipboard.Default.SetTextAsync(xaml);
        await this.DisplayAlert("Copied", "XAML copied to clipboard", "OK");
    }
}
