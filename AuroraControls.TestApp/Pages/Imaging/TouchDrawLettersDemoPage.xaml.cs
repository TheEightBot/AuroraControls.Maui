// <copyright file="TouchDrawLettersDemoPage.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.ComponentModel;
using AuroraControls.TestApp.Controls;
using SkiaSharp;

namespace AuroraControls.TestApp.Pages.Imaging;

/// <summary>
/// Demo page for TouchDrawLettersImage control with interactive property editors.
/// </summary>
public partial class TouchDrawLettersDemoPage : ContentPage, INotifyPropertyChanged
{
    private readonly Random _random = new(Guid.NewGuid().GetHashCode());
    private char _nextLetter = 'A';

    public TouchDrawLettersDemoPage()
    {
        InitializeComponent();
        SetupPresets();
        UpdateInfoLabel();
    }

    private void SetupPresets()
    {
        var presets = new ObservableCollection<PresetItem>
        {
            new PresetItem
            {
                Name = "Blue",
                OnSelected = () =>
                {
                    PreviewControl.DrawItemForegroundColor = Colors.White;
                    PreviewControl.DrawItemBackgroundColor = Color.FromArgb("#3B82F6");
                    ForegroundColorPicker.SelectedColor = Colors.White;
                    BackgroundColorPicker.SelectedColor = Color.FromArgb("#3B82F6");
                },
            },
            new PresetItem
            {
                Name = "Green",
                OnSelected = () =>
                {
                    PreviewControl.DrawItemForegroundColor = Colors.White;
                    PreviewControl.DrawItemBackgroundColor = Color.FromArgb("#22C55E");
                    ForegroundColorPicker.SelectedColor = Colors.White;
                    BackgroundColorPicker.SelectedColor = Color.FromArgb("#22C55E");
                },
            },
            new PresetItem
            {
                Name = "Red",
                OnSelected = () =>
                {
                    PreviewControl.DrawItemForegroundColor = Colors.White;
                    PreviewControl.DrawItemBackgroundColor = Color.FromArgb("#EF4444");
                    ForegroundColorPicker.SelectedColor = Colors.White;
                    BackgroundColorPicker.SelectedColor = Color.FromArgb("#EF4444");
                },
            },
            new PresetItem
            {
                Name = "Purple",
                OnSelected = () =>
                {
                    PreviewControl.DrawItemForegroundColor = Colors.White;
                    PreviewControl.DrawItemBackgroundColor = Color.FromArgb("#A855F7");
                    ForegroundColorPicker.SelectedColor = Colors.White;
                    BackgroundColorPicker.SelectedColor = Color.FromArgb("#A855F7");
                },
            },
            new PresetItem
            {
                Name = "Random",
                OnSelected = () =>
                {
                    var bg = Color.FromRgb(_random.Next(256), _random.Next(256), _random.Next(256));
                    PreviewControl.DrawItemForegroundColor = Colors.White;
                    PreviewControl.DrawItemBackgroundColor = bg;
                    ForegroundColorPicker.SelectedColor = Colors.White;
                    BackgroundColorPicker.SelectedColor = bg;
                },
            },
        };

        PresetControl.Presets = presets;
    }

    private void OnPresetChanged(object? sender, PresetItem e)
    {
        e.OnSelected?.Invoke();
    }

    private void OnAddLetterClicked(object? sender, EventArgs e)
    {
        // Add a letter at a random position (using percentage values 0.0-1.0)
        var letter = new TouchDrawLetter
        {
            Value = _nextLetter.ToString(),
            Location = new Point((_random.NextDouble() * 0.8) + 0.1, (_random.NextDouble() * 0.8) + 0.1),
            ForegroundColorOverride = PreviewControl.DrawItemForegroundColor,
            BackgroundColorOverride = PreviewControl.DrawItemBackgroundColor,
        };

        PreviewControl.TouchDrawLetters.Add(letter);
        _nextLetter = _nextLetter >= 'Z' ? 'A' : (char)(_nextLetter + 1);

        UpdateInfoLabel();
        UpdateLettersList();
    }

    private void OnClearAllClicked(object? sender, EventArgs e)
    {
        PreviewControl.TouchDrawLetters.Clear();
        _nextLetter = 'A';
        UpdateInfoLabel();
        UpdateLettersList();
    }

    private async void OnExportClicked(object? sender, EventArgs e)
    {
        try
        {
            using var stream = PreviewControl.ExportImage(SKEncodedImageFormat.Png);

            if (stream != null && stream.Length > 0)
            {
                await DisplayAlert("Export Complete", $"Exported image: {stream.Length:N0} bytes", "OK");
            }
            else
            {
                await DisplayAlert("Export", "No image data to export", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Export Error", ex.Message, "OK");
        }
    }

    private void OnRandomPhotoClicked(object? sender, EventArgs e)
    {
        PreviewControl.Source = $"https://picsum.photos/400/300?random={_random.Next(1000)}";
    }

    private void OnNaturePhotoClicked(object? sender, EventArgs e)
    {
        PreviewControl.Source = $"https://picsum.photos/400/300?nature&random={_random.Next(1000)}";
    }

    private void OnArchitecturePhotoClicked(object? sender, EventArgs e)
    {
        PreviewControl.Source = $"https://picsum.photos/400/300?architecture&random={_random.Next(1000)}";
    }

    private void OnTechPhotoClicked(object? sender, EventArgs e)
    {
        PreviewControl.Source = $"https://picsum.photos/400/300?tech&random={_random.Next(1000)}";
    }

    private void UpdateInfoLabel()
    {
        var count = PreviewControl.TouchDrawLetters.Count;
        InfoLabel.Text = count == 1 ? "1 letter on canvas" : $"{count} letters on canvas";
    }

    private void UpdateLettersList()
    {
        if (PreviewControl.TouchDrawLetters.Count == 0)
        {
            LettersListLabel.Text = "No letters added yet";
        }
        else
        {
            var letters = string.Join(", ", PreviewControl.TouchDrawLetters.Select(l => l.Value));
            LettersListLabel.Text = $"Letters: {letters}";
        }
    }

    private async void OnCopyCodeClicked(object? sender, EventArgs e)
    {
        var fgHex = PreviewControl.DrawItemForegroundColor.ToArgbHex();
        var bgHex = PreviewControl.DrawItemBackgroundColor.ToArgbHex();

        var xaml = $@"<aurora:TouchDrawLettersImage Source=""your-image-source.jpg""
                           DrawItemForegroundColor=""{fgHex}""
                           DrawItemBackgroundColor=""{bgHex}""
                           BorderSize=""{PreviewControl.BorderSize}""
                           FontSize=""{PreviewControl.FontSize}""/>";

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
