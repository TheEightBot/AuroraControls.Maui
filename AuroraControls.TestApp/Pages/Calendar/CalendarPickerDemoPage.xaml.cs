// <copyright file="CalendarPickerDemoPage.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.ComponentModel;
using AuroraControls.TestApp.Controls;

namespace AuroraControls.TestApp.Pages.Calendar;

/// <summary>
/// Demo page for CalendarPicker control with interactive property editors.
/// </summary>
public partial class CalendarPickerDemoPage : ContentPage, INotifyPropertyChanged
{
    public CalendarPickerDemoPage()
    {
        InitializeComponent();
        SetupPresets();
        UpdateDateLabels();
    }

    private void SetupPresets()
    {
        var presets = new ObservableCollection<PresetItem>
        {
            new PresetItem { Name = "Today", OnSelected = () => ApplyTodayPreset() },
            new PresetItem { Name = "This Week", OnSelected = () => ApplyThisWeekPreset() },
            new PresetItem { Name = "This Month", OnSelected = () => ApplyThisMonthPreset() },
            new PresetItem { Name = "Clear All", OnSelected = () => ApplyClearAllPreset() },
        };
        PresetSelector.Presets = presets;
    }

    private void UpdateDateLabels()
    {
        // Update selected date label
        if (PreviewPicker.Date.HasValue)
        {
            SelectedDateLabel.Text = PreviewPicker.Date.Value.ToString("MMMM d, yyyy");
            SelectedDateLabel.TextColor = Color.FromArgb("#3B82F6");
        }
        else
        {
            SelectedDateLabel.Text = "No date selected";
            SelectedDateLabel.TextColor = Color.FromArgb("#6B7280");
        }

        // Update constraint labels
        if (PreviewPicker.MinimumDate > DateTime.MinValue.AddYears(100))
        {
            MinDateLabel.Text = PreviewPicker.MinimumDate.ToString("MMM d, yyyy");
        }
        else
        {
            MinDateLabel.Text = "Not set";
        }

        if (PreviewPicker.MaximumDate < DateTime.MaxValue.AddYears(-100))
        {
            MaxDateLabel.Text = PreviewPicker.MaximumDate.ToString("MMM d, yyyy");
        }
        else
        {
            MaxDateLabel.Text = "Not set";
        }
    }

    private void OnPresetSelected(object? sender, PresetItem e)
    {
        // PresetItem.OnSelected is automatically called
    }

    private void ApplyTodayPreset()
    {
        PreviewPicker.Date = DateTime.Today;
        UpdateDateLabels();
    }

    private void ApplyThisWeekPreset()
    {
        var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(6);
        PreviewPicker.MinimumDate = startOfWeek;
        PreviewPicker.MaximumDate = endOfWeek;
        EnableMinDateToggle.Value = true;
        EnableMaxDateToggle.Value = true;
        UpdateDateLabels();
    }

    private void ApplyThisMonthPreset()
    {
        var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
        PreviewPicker.MinimumDate = startOfMonth;
        PreviewPicker.MaximumDate = endOfMonth;
        EnableMinDateToggle.Value = true;
        EnableMaxDateToggle.Value = true;
        UpdateDateLabels();
    }

    private void ApplyClearAllPreset()
    {
        PreviewPicker.Date = null;
        PreviewPicker.MinimumDate = DateTime.MinValue;
        PreviewPicker.MaximumDate = DateTime.MaxValue;
        EnableMinDateToggle.Value = false;
        EnableMaxDateToggle.Value = false;
        UpdateDateLabels();
    }

    private void OnDateSelected(object? sender, NullableDateChangedEventArgs e)
    {
        UpdateDateLabels();
    }

    private void OnUpdateModeChanged(object? sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        // Reset button styles
        WhenDoneBtn.BackgroundColor = Color.FromArgb("#3A3A3A");
        ImmediatelyBtn.BackgroundColor = Color.FromArgb("#3A3A3A");

        // Highlight selected and apply
        button.BackgroundColor = Color.FromArgb("#3B82F6");

        if (button == WhenDoneBtn)
        {
            PreviewPicker.UpdateMode = CalendarPickerUpdateMode.WhenDone;
        }
        else if (button == ImmediatelyBtn)
        {
            PreviewPicker.UpdateMode = CalendarPickerUpdateMode.Immediately;
        }
    }

    private void OnMinDateToggleChanged(object? sender, bool e)
    {
        if (e)
        {
            PreviewPicker.MinimumDate = DateTime.Today;
        }
        else
        {
            PreviewPicker.MinimumDate = DateTime.MinValue;
        }

        UpdateDateLabels();
    }

    private void OnMaxDateToggleChanged(object? sender, bool e)
    {
        if (e)
        {
            PreviewPicker.MaximumDate = DateTime.Today.AddDays(30);
        }
        else
        {
            PreviewPicker.MaximumDate = DateTime.MaxValue;
        }

        UpdateDateLabels();
    }

    private void OnSetTodayClicked(object? sender, EventArgs e)
    {
        PreviewPicker.Date = DateTime.Today;
        UpdateDateLabels();
    }

    private void OnSetTomorrowClicked(object? sender, EventArgs e)
    {
        PreviewPicker.Date = DateTime.Today.AddDays(1);
        UpdateDateLabels();
    }

    private void OnSetNextWeekClicked(object? sender, EventArgs e)
    {
        PreviewPicker.Date = DateTime.Today.AddDays(7);
        UpdateDateLabels();
    }

    private void OnClearDateClicked(object? sender, EventArgs e)
    {
        PreviewPicker.Date = null;
        UpdateDateLabels();
    }
}
