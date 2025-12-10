// <copyright file="CutoutOverlayDemoPage.xaml.cs" company="Velocity Systems">
// Copyright (c) Velocity Systems. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.Windows.Input;
using AuroraControls.TestApp.Controls;

namespace AuroraControls.TestApp.Pages.Imaging;

/// <summary>
/// Demo page for CutoutOverlayView control showcasing spotlight and mask overlay effects.
/// </summary>
public partial class CutoutOverlayDemoPage : ContentPage
{
    private static readonly IReadOnlyList<string> ShapeOptions = new[]
    {
        "Circular",
        "Oval",
        "Square",
        "Rectangular",
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="CutoutOverlayDemoPage"/> class.
    /// </summary>
    public CutoutOverlayDemoPage()
    {
        this.InitializeComponent();
        this.BindingContext = this;
        this.InitializeControls();
    }

    /// <summary>
    /// Gets the command executed when the cutout is tapped.
    /// </summary>
    public ICommand TapCommand => new Command(this.OnCutoutTapped);

    private void InitializeControls()
    {
        // Setup shape picker
        this.ShapePicker.SelectedIndex = 0;

        // Setup presets
        var presets = new ObservableCollection<PresetItem>
        {
            new() { Name = "Spotlight", OnSelected = this.ApplySpotlightPreset },
            new() { Name = "Camera", OnSelected = this.ApplyCameraPreset },
            new() { Name = "Document", OnSelected = this.ApplyDocumentPreset },
            new() { Name = "Avatar", OnSelected = this.ApplyAvatarPreset },
            new() { Name = "Vignette", OnSelected = this.ApplyVignettePreset },
        };
        this.PresetSelector.Presets = presets;
    }

    private void OnCutoutTapped()
    {
        this.TapFeedbackLabel.Text = "✓ Tapped!";
        this.Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(1.5), () =>
        {
            this.TapFeedbackLabel.Text = string.Empty;
        });
    }

    private void OnShapeChanged(object? sender, EventArgs e)
    {
        if (this.ShapePicker.SelectedIndex < 0)
        {
            return;
        }

        var shapeName = ShapeOptions[this.ShapePicker.SelectedIndex];
        if (Enum.TryParse<CutoutOverlayShape>(shapeName, out var shape))
        {
            this.CutoutOverlay.CutoutShape = shape;
        }
    }

    private void OnCornerRadiusChanged(object? sender, ValueChangedEventArgs e)
    {
        this.CutoutOverlay.CornerRadius = e.NewValue;
    }

    private void OnUniformInsetChanged(object? sender, ValueChangedEventArgs e)
    {
        if (!this.IndividualInsetsToggle.Value)
        {
            this.CutoutOverlay.CutoutInset = new Thickness(e.NewValue);
            this.UpdateIndividualInsetSliders(e.NewValue);
        }
    }

    private void OnIndividualInsetsToggled(object? sender, bool isEnabled)
    {
        this.IndividualInsetsPanel.IsVisible = isEnabled;
        this.UniformInsetEditor.IsEnabled = !isEnabled;

        if (!isEnabled)
        {
            // Reset to uniform when toggling off
            var uniform = this.UniformInsetEditor.Value;
            this.CutoutOverlay.CutoutInset = new Thickness(uniform);
            this.UpdateIndividualInsetSliders(uniform);
        }
    }

    private void OnInsetChanged(object? sender, ValueChangedEventArgs e)
    {
        if (this.IndividualInsetsToggle.Value)
        {
            this.CutoutOverlay.CutoutInset = new Thickness(
                this.LeftInsetEditor.Value,
                this.TopInsetEditor.Value,
                this.RightInsetEditor.Value,
                this.BottomInsetEditor.Value);
        }
    }

    private void UpdateIndividualInsetSliders(double value)
    {
        this.LeftInsetEditor.Value = value;
        this.TopInsetEditor.Value = value;
        this.RightInsetEditor.Value = value;
        this.BottomInsetEditor.Value = value;
    }

    private void OnOverlayColorChanged(object? sender, Color e)
    {
        this.CutoutOverlay.CutoutOverlayColor = e;
    }

    private void OnBorderColorChanged(object? sender, Color e)
    {
        this.CutoutOverlay.BorderColor = e;
    }

    private void OnBorderWidthChanged(object? sender, ValueChangedEventArgs e)
    {
        this.CutoutOverlay.BorderWidth = (float)e.NewValue;
    }

    private async void OnCopyCodeClicked(object? sender, EventArgs e)
    {
        var xaml = $@"<aurora:CutoutOverlayView
    CutoutShape=""{this.CutoutOverlay.CutoutShape}""
    CutoutOverlayColor=""{this.CutoutOverlay.CutoutOverlayColor.ToArgbHex()}""
    BorderColor=""{this.CutoutOverlay.BorderColor.ToArgbHex()}""
    BorderWidth=""{this.CutoutOverlay.BorderWidth}""
    CutoutInset=""{this.CutoutOverlay.CutoutInset.Left},{this.CutoutOverlay.CutoutInset.Top},{this.CutoutOverlay.CutoutInset.Right},{this.CutoutOverlay.CutoutInset.Bottom}""
    CornerRadius=""{this.CutoutOverlay.CornerRadius}""/>";

        await Clipboard.Default.SetTextAsync(xaml);
        await this.DisplayAlert("Copied", "XAML copied to clipboard", "OK");
    }

    private void ApplyPreset(CutoutOverlayShape shape, Color overlayColor, Color borderColor, float borderWidth, double inset, double cornerRadius)
    {
        this.CutoutOverlay.CutoutShape = shape;
        this.CutoutOverlay.CutoutOverlayColor = overlayColor;
        this.CutoutOverlay.BorderColor = borderColor;
        this.CutoutOverlay.BorderWidth = borderWidth;
        this.CutoutOverlay.CutoutInset = new Thickness(inset);
        this.CutoutOverlay.CornerRadius = cornerRadius;

        // Update editor controls
        this.ShapePicker.SelectedIndex = (int)shape;
        this.CornerRadiusEditor.Value = cornerRadius;
        this.UniformInsetEditor.Value = inset;
        this.UpdateIndividualInsetSliders(inset);
        this.OverlayColorEditor.SelectedColor = overlayColor;
        this.BorderColorEditor.SelectedColor = borderColor;
        this.BorderWidthEditor.Value = borderWidth;
    }

    private void ApplySpotlightPreset()
    {
        this.ApplyPreset(
            shape: CutoutOverlayShape.Circular,
            overlayColor: Color.FromArgb("#CC000000"),
            borderColor: Color.FromArgb("#FFFFFF"),
            borderWidth: 4,
            inset: 60,
            cornerRadius: 0);
    }

    private void ApplyCameraPreset()
    {
        this.ApplyPreset(
            shape: CutoutOverlayShape.Rectangular,
            overlayColor: Color.FromArgb("#99000000"),
            borderColor: Color.FromArgb("#10B981"),
            borderWidth: 3,
            inset: 30,
            cornerRadius: 8);
    }

    private void ApplyDocumentPreset()
    {
        this.ApplyPreset(
            shape: CutoutOverlayShape.Rectangular,
            overlayColor: Color.FromArgb("#80000000"),
            borderColor: Color.FromArgb("#3B82F6"),
            borderWidth: 2,
            inset: 20,
            cornerRadius: 4);
    }

    private void ApplyAvatarPreset()
    {
        this.ApplyPreset(
            shape: CutoutOverlayShape.Circular,
            overlayColor: Color.FromArgb("#B3000000"),
            borderColor: Color.FromArgb("#EC4899"),
            borderWidth: 3,
            inset: 50,
            cornerRadius: 0);
    }

    private void ApplyVignettePreset()
    {
        this.ApplyPreset(
            shape: CutoutOverlayShape.Oval,
            overlayColor: Color.FromArgb("#66000000"),
            borderColor: Colors.Transparent,
            borderWidth: 0,
            inset: 10,
            cornerRadius: 0);
    }
}
