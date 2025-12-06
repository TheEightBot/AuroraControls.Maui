// <copyright file="ChipGroupDemoPage.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.ComponentModel;
using AuroraControls.TestApp.Controls;

namespace AuroraControls.TestApp.Pages.Chips;

/// <summary>
/// Demo page for ChipGroup control with interactive property editors.
/// </summary>
public partial class ChipGroupDemoPage : ContentPage, INotifyPropertyChanged
{
    private int _chipCounter = 6;

    public ChipGroupDemoPage()
    {
        InitializeComponent();
        SetupPresets();
        SetupColorPickers();
        UpdateOverflowStatus();
    }

    private void SetupPresets()
    {
        var presets = new ObservableCollection<PresetItem>
        {
            new PresetItem { Name = "Dark", OnSelected = ApplyDarkPreset },
            new PresetItem { Name = "Purple", OnSelected = ApplyPurplePreset },
            new PresetItem { Name = "Colorful", OnSelected = ApplyColorfulPreset },
            new PresetItem { Name = "Pill", OnSelected = ApplyPillPreset },
            new PresetItem { Name = "Outline", OnSelected = ApplyOutlinePreset },
        };
        PresetSelector.Presets = presets;
    }

    private void SetupColorPickers()
    {
        ChipBackgroundPicker.SelectedColor = Color.FromArgb("#3A3A3A");
        ChipToggledBackgroundPicker.SelectedColor = Color.FromArgb("#8B5CF6");
        ChipTextColorPicker.SelectedColor = Color.FromArgb("#E5E7EB");
        ChipToggledTextColorPicker.SelectedColor = Colors.White;
    }

    private void UpdateSelectionLabel()
    {
        var selectedChips = PreviewChipGroup.SelectedChips.ToList();
        if (selectedChips.Count == 0)
        {
            SelectedChipsLabel.Text = "None";
        }
        else
        {
            SelectedChipsLabel.Text = string.Join(", ", selectedChips.Select(c => c.Text));
        }
    }

    private void UpdateOverflowStatus()
    {
        OverflowLabel.Text = PreviewChipGroup.IsOverflow ? "Yes" : "No";
        OverflowLabel.TextColor = PreviewChipGroup.IsOverflow
            ? Color.FromArgb("#F59E0B")
            : Color.FromArgb("#22C55E");
    }

    private void OnPresetSelected(object sender, PresetItem e)
    {
        // PresetItem.OnSelected is automatically called
        SetupColorPickers();
    }

    private void ApplyDarkPreset()
    {
        foreach (var chip in PreviewChipGroup.Chips)
        {
            chip.BackgroundColor = Color.FromArgb("#3A3A3A");
            chip.ToggledBackgroundColor = Color.FromArgb("#8B5CF6");
            chip.TextColor = Color.FromArgb("#E5E7EB");
            chip.ToggledFontColor = Colors.White;
            chip.CornerRadius = 16;
            chip.BorderColor = Colors.Transparent;
        }
    }

    private void ApplyPurplePreset()
    {
        foreach (var chip in PreviewChipGroup.Chips)
        {
            chip.BackgroundColor = Color.FromArgb("#4C1D95");
            chip.ToggledBackgroundColor = Color.FromArgb("#7C3AED");
            chip.TextColor = Color.FromArgb("#DDD6FE");
            chip.ToggledFontColor = Colors.White;
            chip.CornerRadius = 12;
            chip.BorderColor = Color.FromArgb("#7C3AED");
        }
    }

    private void ApplyColorfulPreset()
    {
        var colors = new[]
        {
            ("#EF4444", "#DC2626"), // Red
            ("#F59E0B", "#D97706"), // Amber
            ("#22C55E", "#16A34A"), // Green
            ("#3B82F6", "#2563EB"), // Blue
            ("#8B5CF6", "#7C3AED"), // Purple
        };

        int i = 0;
        foreach (var chip in PreviewChipGroup.Chips)
        {
            var (bg, toggled) = colors[i % colors.Length];
            chip.BackgroundColor = Color.FromArgb(bg);
            chip.ToggledBackgroundColor = Color.FromArgb(toggled);
            chip.TextColor = Colors.White;
            chip.ToggledFontColor = Colors.White;
            chip.CornerRadius = 16;
            chip.BorderColor = Colors.Transparent;
            i++;
        }
    }

    private void ApplyPillPreset()
    {
        foreach (var chip in PreviewChipGroup.Chips)
        {
            chip.BackgroundColor = Color.FromArgb("#1F2937");
            chip.ToggledBackgroundColor = Color.FromArgb("#14B8A6");
            chip.TextColor = Color.FromArgb("#9CA3AF");
            chip.ToggledFontColor = Colors.White;
            chip.CornerRadius = 24;
            chip.BorderColor = Colors.Transparent;
        }
    }

    private void ApplyOutlinePreset()
    {
        foreach (var chip in PreviewChipGroup.Chips)
        {
            chip.BackgroundColor = Colors.Transparent;
            chip.ToggledBackgroundColor = Color.FromArgb("#3B82F6");
            chip.TextColor = Color.FromArgb("#3B82F6");
            chip.ToggledFontColor = Colors.White;
            chip.CornerRadius = 8;
            chip.BorderColor = Color.FromArgb("#3B82F6");
            chip.BorderSize = 2;
        }
    }

    private void OnChipTapped(object sender, ChipTappedEventArgs e)
    {
        UpdateSelectionLabel();
    }

    private void OnAddChipClicked(object sender, EventArgs e)
    {
        var chip = new Chip
        {
            Text = $"New {_chipCounter++}",
            Value = $"new{_chipCounter}",
            BackgroundColor = ChipBackgroundPicker.SelectedColor,
            ToggledBackgroundColor = ChipToggledBackgroundPicker.SelectedColor,
            TextColor = ChipTextColorPicker.SelectedColor,
            ToggledFontColor = ChipToggledTextColorPicker.SelectedColor,
            CornerRadius = CornerRadiusSlider.Value,
            IsRemovable = RemovableToggle.Value,
        };

        chip.Removed += (s, args) =>
        {
            if (s is Chip removedChip)
            {
                PreviewChipGroup.Remove(removedChip);
                UpdateSelectionLabel();
            }
        };

        PreviewChipGroup.Add(chip);
        UpdateOverflowStatus();
    }

    private void OnSelectionModeChanged(object sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        MultiSelectBtn.BackgroundColor = Color.FromArgb("#3A3A3A");
        SingleSelectBtn.BackgroundColor = Color.FromArgb("#3A3A3A");

        button.BackgroundColor = Color.FromArgb("#8B5CF6");

        PreviewChipGroup.IsSingleSelection = button == SingleSelectBtn;

        // Clear selections when switching modes
        foreach (var chip in PreviewChipGroup.Chips)
        {
            chip.IsToggled = false;
        }

        UpdateSelectionLabel();
    }

    private void OnChipSelectionDetectionChanged(object sender, bool e)
    {
        PreviewChipGroup.ChipSelectionDetection = e;
    }

    private void OnClearSelectionsClicked(object sender, EventArgs e)
    {
        foreach (var chip in PreviewChipGroup.Chips)
        {
            chip.IsToggled = false;
        }

        UpdateSelectionLabel();
    }

    private void OnSpacingChanged(object sender, double e)
    {
        PreviewChipGroup.Spacing = e;
    }

    private void OnScrollableChanged(object sender, bool e)
    {
        PreviewChipGroup.Scrollable = e;
        if (e)
        {
            PreviewChipGroup.HeightRequest = 50;
        }
        else
        {
            PreviewChipGroup.HeightRequest = -1;
        }
    }

    private void OnMaxRowsChanged(object sender, double e)
    {
        PreviewChipGroup.MaxRowsBeforeOverflow = (int)e;
        UpdateOverflowStatus();
    }

    private void OnChipColorChanged(object sender, Color e)
    {
        foreach (var chip in PreviewChipGroup.Chips)
        {
            if (sender == ChipBackgroundPicker)
            {
                chip.BackgroundColor = e;
            }
            else if (sender == ChipToggledBackgroundPicker)
            {
                chip.ToggledBackgroundColor = e;
            }
            else if (sender == ChipTextColorPicker)
            {
                chip.TextColor = e;
            }
            else if (sender == ChipToggledTextColorPicker)
            {
                chip.ToggledFontColor = e;
            }
        }
    }

    private void OnCornerRadiusChanged(object sender, double e)
    {
        foreach (var chip in PreviewChipGroup.Chips)
        {
            chip.CornerRadius = e;
        }
    }

    private void OnRemovableChanged(object sender, bool e)
    {
        foreach (var chip in PreviewChipGroup.Chips)
        {
            chip.IsRemovable = e;

            if (e)
            {
                chip.Removed -= OnChipRemoved;
                chip.Removed += OnChipRemoved;
            }
        }
    }

    private void OnChipRemoved(object sender, EventArgs e)
    {
        if (sender is Chip chip)
        {
            PreviewChipGroup.Remove(chip);
            UpdateSelectionLabel();
            UpdateOverflowStatus();
        }
    }

    private void OnShapeChanged(object sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        StandardShapeBtn.BackgroundColor = Color.FromArgb("#3A3A3A");
        RectangleShapeBtn.BackgroundColor = Color.FromArgb("#3A3A3A");
        RoundedShapeBtn.BackgroundColor = Color.FromArgb("#3A3A3A");

        button.BackgroundColor = Color.FromArgb("#8B5CF6");

        ChipShape shape = ChipShape.Standard;
        if (button == RectangleShapeBtn)
        {
            shape = ChipShape.Rectangle;
        }
        else if (button == RoundedShapeBtn)
        {
            shape = ChipShape.RoundedRectangle;
        }

        foreach (var chip in PreviewChipGroup.Chips)
        {
            chip.Shape = shape;
        }
    }

    private async void OnScrollToStartClicked(object sender, EventArgs e)
    {
        var firstChip = PreviewChipGroup.Chips.FirstOrDefault();
        if (PreviewChipGroup.Scrollable && firstChip != null)
        {
            await PreviewChipGroup.ScrollToAsync(firstChip, ScrollToPosition.Start);
        }
    }

    private async void OnScrollToEndClicked(object sender, EventArgs e)
    {
        var lastChip = PreviewChipGroup.Chips.LastOrDefault();
        if (PreviewChipGroup.Scrollable && lastChip != null)
        {
            await PreviewChipGroup.ScrollToAsync(lastChip, ScrollToPosition.End);
        }
    }
}
