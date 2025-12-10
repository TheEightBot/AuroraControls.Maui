// <copyright file="SvgImageViewDemoPage.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.ComponentModel;
using AuroraControls.TestApp.Controls;
using SkiaSharp.Views.Maui;

namespace AuroraControls.TestApp.Pages.Imaging;

/// <summary>
/// Demo page for SvgImageView control with interactive property editors.
/// </summary>
public partial class SvgImageViewDemoPage : ContentPage, INotifyPropertyChanged
{
    private readonly Dictionary<string, string> _imageMap = new()
    {
        { "SplatoonBtn", "splatoon.svg" },
        { "TriforceBtn", "triforce.svg" },
        { "MoreBtn", "more.svg" },
        { "CheckBtn", "check-filled.svg" },
    };

    private int _touchCount;
    private Button? _selectedImageButton;

    public SvgImageViewDemoPage()
    {
        InitializeComponent();
        SetupPresets();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _selectedImageButton = SplatoonBtn;
        UpdateImageButtonSelection();
    }

    private void SetupPresets()
    {
        var presets = new System.Collections.ObjectModel.ObservableCollection<PresetItem>
        {
            new PresetItem
            {
                Name = "Original",
                OnSelected = () =>
                {
                    PreviewSvg.OverlayColor = Colors.Transparent;
                    OverlayColorPickerEditor.SelectedColor = Colors.Transparent;
                },
            },
            new PresetItem
            {
                Name = "Blue",
                OnSelected = () =>
                {
                    PreviewSvg.OverlayColor = Color.FromArgb("#3B82F6");
                    OverlayColorPickerEditor.SelectedColor = Color.FromArgb("#3B82F6");
                },
            },
            new PresetItem
            {
                Name = "Green",
                OnSelected = () =>
                {
                    PreviewSvg.OverlayColor = Color.FromArgb("#22C55E");
                    OverlayColorPickerEditor.SelectedColor = Color.FromArgb("#22C55E");
                },
            },
            new PresetItem
            {
                Name = "Yellow",
                OnSelected = () =>
                {
                    PreviewSvg.OverlayColor = Color.FromArgb("#EAB308");
                    OverlayColorPickerEditor.SelectedColor = Color.FromArgb("#EAB308");
                },
            },
            new PresetItem
            {
                Name = "Red",
                OnSelected = () =>
                {
                    PreviewSvg.OverlayColor = Color.FromArgb("#EF4444");
                    OverlayColorPickerEditor.SelectedColor = Color.FromArgb("#EF4444");
                },
            },
            new PresetItem
            {
                Name = "White",
                OnSelected = () =>
                {
                    PreviewSvg.OverlayColor = Colors.White;
                    OverlayColorPickerEditor.SelectedColor = Colors.White;
                },
            },
        };

        PresetControl.Presets = presets;
    }

    private void OnPresetChanged(object? sender, PresetItem e)
    {
        e.OnSelected?.Invoke();
    }

    private void OnImageSelected(object? sender, EventArgs e)
    {
        if (sender is Button button && _imageMap.TryGetValue(button.StyleId ?? GetButtonId(button), out var imageName))
        {
            PreviewSvg.EmbeddedImageName = imageName;
            _selectedImageButton = button;
            UpdateImageButtonSelection();
        }
    }

    private string GetButtonId(Button button)
    {
        if (button == SplatoonBtn)
        {
            return "SplatoonBtn";
        }

        if (button == TriforceBtn)
        {
            return "TriforceBtn";
        }

        if (button == MoreBtn)
        {
            return "MoreBtn";
        }

        if (button == CheckBtn)
        {
            return "CheckBtn";
        }

        return string.Empty;
    }

    private void UpdateImageButtonSelection()
    {
        var buttons = new[] { SplatoonBtn, TriforceBtn, MoreBtn, CheckBtn };

        foreach (var btn in buttons)
        {
            btn.BackgroundColor = btn == _selectedImageButton
                ? Color.FromArgb("#3B82F6")
                : Color.FromArgb("#262626");
        }
    }

    private void OnQuickColorClicked(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            PreviewSvg.OverlayColor = button.BackgroundColor;
            OverlayColorPickerEditor.SelectedColor = button.BackgroundColor;
        }
    }

    private void OnSizeChanged(object? sender, double value)
    {
        PreviewSvg.WidthRequest = WidthEditor.Value;
        PreviewSvg.HeightRequest = HeightEditor.Value;
    }

    private void OnSquareClicked(object? sender, EventArgs e)
    {
        WidthEditor.Value = 100;
        HeightEditor.Value = 100;
        PreviewSvg.WidthRequest = 100;
        PreviewSvg.HeightRequest = 100;
    }

    private void OnLargeClicked(object? sender, EventArgs e)
    {
        WidthEditor.Value = 180;
        HeightEditor.Value = 180;
        PreviewSvg.WidthRequest = 180;
        PreviewSvg.HeightRequest = 180;
    }

    private void OnMaxSizeChanged(object? sender, double value)
    {
        var maxWidth = MaxWidthEditor.Value;
        var maxHeight = MaxHeightEditor.Value;

        PreviewSvg.MaxImageSize = maxWidth > 0 || maxHeight > 0
            ? new Size(maxWidth, maxHeight)
            : default;
    }

    private void OnResetMaxSizeClicked(object? sender, EventArgs e)
    {
        MaxWidthEditor.Value = 0;
        MaxHeightEditor.Value = 0;
        PreviewSvg.MaxImageSize = default;
    }

    private void OnSvgTouched(object? sender, SKTouchEventArgs e)
    {
        if (e.ActionType == SKTouchAction.Pressed)
        {
            _touchCount++;
            TouchEventLabel.Text = $"Touch at ({e.Location.X:F0}, {e.Location.Y:F0})";
            TouchCountLabel.Text = $"Touch count: {_touchCount}";

            // Animate the touchable SVG
            TouchableSvg.ScaleTo(0.9, 50, Easing.CubicOut)
                .ContinueWith(_ => MainThread.BeginInvokeOnMainThread(() =>
                    TouchableSvg.ScaleTo(1.0, 100, Easing.BounceOut)));

            e.Handled = true;
        }
    }

    private async void OnCopyCodeClicked(object? sender, EventArgs e)
    {
        var overlayColorHex = PreviewSvg.OverlayColor != null && PreviewSvg.OverlayColor != Colors.Transparent
            ? PreviewSvg.OverlayColor.ToArgbHex()
            : string.Empty;

        var overlayAttr = !string.IsNullOrEmpty(overlayColorHex)
            ? $"\n                     OverlayColor=\"{overlayColorHex}\""
            : string.Empty;

        var maxSizeAttr = PreviewSvg.MaxImageSize != default
            ? $"\n                     MaxImageSize=\"{PreviewSvg.MaxImageSize.Width},{PreviewSvg.MaxImageSize.Height}\""
            : string.Empty;

        var xaml = $@"<aurora:SvgImageView EmbeddedImageName=""{PreviewSvg.EmbeddedImageName}""
                     HeightRequest=""{PreviewSvg.HeightRequest}""
                     WidthRequest=""{PreviewSvg.WidthRequest}""{overlayAttr}{maxSizeAttr}/>";

        await Clipboard.SetTextAsync(xaml);

        if (sender is Button button)
        {
            var originalText = button.Text;
            button.Text = "✓";
            await Task.Delay(1500);
            button.Text = originalText;
        }
    }
}
