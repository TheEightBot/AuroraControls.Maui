// <copyright file="SignaturePadDemoPage.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.ComponentModel;
using AuroraControls.TestApp.Controls;
using SkiaSharp;

namespace AuroraControls.TestApp.Pages.Imaging;

/// <summary>
/// Demo page for SignaturePad control with interactive property editors.
/// </summary>
public partial class SignaturePadDemoPage : ContentPage, INotifyPropertyChanged
{
    public SignaturePadDemoPage()
    {
        InitializeComponent();
        SetupPresets();
        SignaturePadControl.PropertyChanged += OnSignaturePadPropertyChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        SignaturePadControl.PropertyChanged -= OnSignaturePadPropertyChanged;
    }

    private void OnSignaturePadPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SignaturePad.IsSigned))
        {
            UpdateSignedStatus();
        }
    }

    private void UpdateSignedStatus()
    {
        if (SignaturePadControl.IsSigned)
        {
            SignedStatusLabel.Text = "Signed";
            SignedIndicator.Text = "●";
            SignedIndicator.TextColor = Color.FromArgb("#22C55E");
        }
        else
        {
            SignedStatusLabel.Text = "Not signed";
            SignedIndicator.Text = "○";
            SignedIndicator.TextColor = Color.FromArgb("#EF4444");
        }
    }

    private void SetupPresets()
    {
        var presets = new ObservableCollection<PresetItem>
        {
            new PresetItem
            {
                Name = "Default",
                OnSelected = () =>
                {
                    SignaturePadControl.PenColor = Colors.Black;
                    SignaturePadControl.MinWidth = 2;
                    SignaturePadControl.MaxWidth = 6;
                    PenColorPicker.SelectedColor = Colors.Black;
                },
            },
            new PresetItem
            {
                Name = "Blue Ink",
                OnSelected = () =>
                {
                    SignaturePadControl.PenColor = Color.FromArgb("#1E40AF");
                    SignaturePadControl.MinWidth = 2;
                    SignaturePadControl.MaxWidth = 5;
                    PenColorPicker.SelectedColor = Color.FromArgb("#1E40AF");
                },
            },
            new PresetItem
            {
                Name = "Fine Tip",
                OnSelected = () =>
                {
                    SignaturePadControl.PenColor = Colors.Black;
                    SignaturePadControl.MinWidth = 1;
                    SignaturePadControl.MaxWidth = 3;
                    PenColorPicker.SelectedColor = Colors.Black;
                },
            },
            new PresetItem
            {
                Name = "Bold",
                OnSelected = () =>
                {
                    SignaturePadControl.PenColor = Colors.Black;
                    SignaturePadControl.MinWidth = 4;
                    SignaturePadControl.MaxWidth = 10;
                    PenColorPicker.SelectedColor = Colors.Black;
                },
            },
            new PresetItem
            {
                Name = "Red Ink",
                OnSelected = () =>
                {
                    SignaturePadControl.PenColor = Color.FromArgb("#DC2626");
                    SignaturePadControl.MinWidth = 2;
                    SignaturePadControl.MaxWidth = 6;
                    PenColorPicker.SelectedColor = Color.FromArgb("#DC2626");
                },
            },
        };

        PresetControl.Presets = presets;
    }

    private void OnPresetChanged(object? sender, PresetItem e)
    {
        e.OnSelected?.Invoke();
    }

    private void OnClearClicked(object? sender, EventArgs e)
    {
        SignaturePadControl.Clear();
        UpdateSignedStatus();
    }

    private async void OnExportPngClicked(object? sender, EventArgs e)
    {
        try
        {
            using var stream = SignaturePadControl.GetSignatureImageStream(SKEncodedImageFormat.Png);

            if (stream != null && stream.Length > 0)
            {
                await DisplayAlert("Export Complete", $"PNG image: {stream.Length:N0} bytes", "OK");
            }
            else
            {
                await DisplayAlert("Export", "No signature to export", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Export Error", ex.Message, "OK");
        }
    }

    private async void OnExportSvgClicked(object? sender, EventArgs e)
    {
        try
        {
            // Get transparent signature as a bitmap demonstration
            using var bitmap = SignaturePadControl.GetTransparentSignatureBitmap();

            if (bitmap != null)
            {
                await DisplayAlert("Export Info", $"Signature bitmap: {bitmap.Width}x{bitmap.Height} pixels", "OK");
            }
            else
            {
                await DisplayAlert("Export", "No signature to export", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Export Error", ex.Message, "OK");
        }
    }

    private async void OnCopyCodeClicked(object? sender, EventArgs e)
    {
        var penColorHex = SignaturePadControl.PenColor.ToArgbHex();

        var xaml = $@"<aurora:SignaturePad PenColor=""{penColorHex}""
                    MinWidth=""{SignaturePadControl.MinWidth}""
                    MaxWidth=""{SignaturePadControl.MaxWidth}""
                    VelocityFilterWeight=""{SignaturePadControl.VelocityFilterWeight}""
                    ClearOnDoubleClick=""{SignaturePadControl.ClearOnDoubleClick.ToString().ToLowerInvariant()}""/>";

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
