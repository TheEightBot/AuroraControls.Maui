// <copyright file="CalendarViewDemoPage.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using AuroraControls.TestApp.Controls;

namespace AuroraControls.TestApp.Pages.Calendar;

/// <summary>
/// Demo page for CalendarView control with interactive property editors.
/// </summary>
public partial class CalendarViewDemoPage : ContentPage, INotifyPropertyChanged
{
    public CalendarViewDemoPage()
    {
        InitializeComponent();
        SetupPresets();
        SetupEditors();
        UpdateMonthYearLabel();
        UpdateEventCount();
    }

    private void SetupPresets()
    {
        var presets = new ObservableCollection<PresetItem>
        {
            new PresetItem { Name = "Dark Mode", OnSelected = ApplyDarkModePreset },
            new PresetItem { Name = "Light Mode", OnSelected = ApplyLightModePreset },
            new PresetItem { Name = "Purple", OnSelected = ApplyPurpleAccentPreset },
            new PresetItem { Name = "Ocean", OnSelected = ApplyOceanBluePreset },
            new PresetItem { Name = "Coral", OnSelected = ApplyCoralPreset },
        };
        PresetSelector.Presets = presets;
    }

    private void SetupEditors()
    {
        // Initialize color pickers with current values
        HeaderTextColorPicker.SelectedColor = PreviewCalendar.HeaderTextColor;
        SeparatorColorPicker.SelectedColor = PreviewCalendar.SeparatorColor;
        DateTextColorPicker.SelectedColor = PreviewCalendar.DateTextColor;
        SelectedDateColorPicker.SelectedColor = PreviewCalendar.SelectedDateColor;
        SelectedDateTextColorPicker.SelectedColor = PreviewCalendar.SelectedDateTextColor;
        DateBackgroundColorPicker.SelectedColor = PreviewCalendar.DateBackgroundColor;
        UnavailableDateColorPicker.SelectedColor = PreviewCalendar.UnavailableDateColor;

        // Initialize pickers
        DayOfWeekPicker.SelectedIndex = 1; // Abbreviated
    }

    private void UpdateMonthYearLabel()
    {
        var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(PreviewCalendar.CurrentMonth);
        CurrentMonthYearLabel.Text = $"{monthName} {PreviewCalendar.CurrentYear}";
    }

    private void UpdateEventCount()
    {
        var count = PreviewCalendar.Events.Count;
        EventCountLabel.Text = count == 0 ? "No events" : $"{count} event{(count == 1 ? string.Empty : "s")}";
    }

    private void UpdateSelectedDatesLabel()
    {
        var dates = PreviewCalendar.SelectedDates;
        if (dates == null || dates.Count == 0)
        {
            SelectedDatesLabel.Text = "No date selected";
        }
        else if (dates.Count == 1)
        {
            SelectedDatesLabel.Text = dates[0].ToString("MMMM d, yyyy");
        }
        else
        {
            SelectedDatesLabel.Text = $"{dates.Count} dates selected";
        }
    }

    private void OnPresetSelected(object? sender, PresetItem e)
    {
        // PresetItem.OnSelected is automatically called
        // Update color pickers to reflect new values
        SetupEditors();
    }

    private void ApplyDarkModePreset()
    {
        PreviewCalendar.HeaderTextColor = Color.FromArgb("#FFFFFF");
        PreviewCalendar.SeparatorColor = Color.FromArgb("#404040");
        PreviewCalendar.DateTextColor = Color.FromArgb("#E5E7EB");
        PreviewCalendar.SelectedDateColor = Color.FromArgb("#7C3AED");
        PreviewCalendar.SelectedDateTextColor = Color.FromArgb("#FFFFFF");
        PreviewCalendar.DateBackgroundColor = Color.FromArgb("#262626");
        PreviewCalendar.UnavailableDateColor = Color.FromArgb("#4B5563");
    }

    private void ApplyLightModePreset()
    {
        PreviewCalendar.HeaderTextColor = Color.FromArgb("#1F2937");
        PreviewCalendar.SeparatorColor = Color.FromArgb("#E5E7EB");
        PreviewCalendar.DateTextColor = Color.FromArgb("#374151");
        PreviewCalendar.SelectedDateColor = Color.FromArgb("#3B82F6");
        PreviewCalendar.SelectedDateTextColor = Color.FromArgb("#FFFFFF");
        PreviewCalendar.DateBackgroundColor = Color.FromArgb("#FFFFFF");
        PreviewCalendar.UnavailableDateColor = Color.FromArgb("#D1D5DB");
    }

    private void ApplyPurpleAccentPreset()
    {
        PreviewCalendar.HeaderTextColor = Color.FromArgb("#FFFFFF");
        PreviewCalendar.SeparatorColor = Color.FromArgb("#7C3AED");
        PreviewCalendar.DateTextColor = Color.FromArgb("#E5E7EB");
        PreviewCalendar.SelectedDateColor = Color.FromArgb("#8B5CF6");
        PreviewCalendar.SelectedDateTextColor = Color.FromArgb("#FFFFFF");
        PreviewCalendar.DateBackgroundColor = Color.FromArgb("#1E1B4B");
        PreviewCalendar.UnavailableDateColor = Color.FromArgb("#6B7280");
    }

    private void ApplyOceanBluePreset()
    {
        PreviewCalendar.HeaderTextColor = Color.FromArgb("#FFFFFF");
        PreviewCalendar.SeparatorColor = Color.FromArgb("#0EA5E9");
        PreviewCalendar.DateTextColor = Color.FromArgb("#E0F2FE");
        PreviewCalendar.SelectedDateColor = Color.FromArgb("#0EA5E9");
        PreviewCalendar.SelectedDateTextColor = Color.FromArgb("#FFFFFF");
        PreviewCalendar.DateBackgroundColor = Color.FromArgb("#0C4A6E");
        PreviewCalendar.UnavailableDateColor = Color.FromArgb("#64748B");
    }

    private void ApplyCoralPreset()
    {
        PreviewCalendar.HeaderTextColor = Color.FromArgb("#FFFFFF");
        PreviewCalendar.SeparatorColor = Color.FromArgb("#FB7185");
        PreviewCalendar.DateTextColor = Color.FromArgb("#FEE2E2");
        PreviewCalendar.SelectedDateColor = Color.FromArgb("#F43F5E");
        PreviewCalendar.SelectedDateTextColor = Color.FromArgb("#FFFFFF");
        PreviewCalendar.DateBackgroundColor = Color.FromArgb("#4C1D29");
        PreviewCalendar.UnavailableDateColor = Color.FromArgb("#9CA3AF");
    }

    private void OnPreviousMonthClicked(object? sender, EventArgs e)
    {
        if (PreviewCalendar.CurrentMonth == 1)
        {
            PreviewCalendar.CurrentMonth = 12;
            PreviewCalendar.CurrentYear--;
        }
        else
        {
            PreviewCalendar.CurrentMonth--;
        }

        UpdateMonthYearLabel();
    }

    private void OnNextMonthClicked(object? sender, EventArgs e)
    {
        if (PreviewCalendar.CurrentMonth == 12)
        {
            PreviewCalendar.CurrentMonth = 1;
            PreviewCalendar.CurrentYear++;
        }
        else
        {
            PreviewCalendar.CurrentMonth++;
        }

        UpdateMonthYearLabel();
    }

    private void OnGoToTodayClicked(object? sender, EventArgs e)
    {
        PreviewCalendar.CurrentMonth = DateTime.Now.Month;
        PreviewCalendar.CurrentYear = DateTime.Now.Year;
        UpdateMonthYearLabel();
    }

    private void OnSelectionTypeChanged(object? sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        // Reset all button styles
        SingleSelectionBtn.BackgroundColor = Color.FromArgb("#3A3A3A");
        SpanSelectionBtn.BackgroundColor = Color.FromArgb("#3A3A3A");
        MultipleSelectionBtn.BackgroundColor = Color.FromArgb("#3A3A3A");

        // Highlight selected button and set selection type
        button.BackgroundColor = Color.FromArgb("#7C3AED");

        if (button == SingleSelectionBtn)
        {
            PreviewCalendar.SelectionType = CalendarSelectionType.Single;
        }
        else if (button == SpanSelectionBtn)
        {
            PreviewCalendar.SelectionType = CalendarSelectionType.Span;
        }
        else if (button == MultipleSelectionBtn)
        {
            PreviewCalendar.SelectionType = CalendarSelectionType.Multiple;
        }
    }

    private void OnClearSelectionClicked(object? sender, EventArgs e)
    {
        PreviewCalendar.SelectedDates.Clear();
        UpdateSelectedDatesLabel();
    }

    private void OnDayOfWeekDisplayChanged(object? sender, EventArgs e)
    {
        PreviewCalendar.DayOfWeekDisplayType = DayOfWeekPicker.SelectedIndex switch
        {
            0 => CalendarDayOfWeekDisplayType.Full,
            1 => CalendarDayOfWeekDisplayType.Abbreviated,
            2 => CalendarDayOfWeekDisplayType.AbbreviatedUppercase,
            3 => CalendarDayOfWeekDisplayType.Shortest,
            _ => CalendarDayOfWeekDisplayType.Abbreviated,
        };
    }

    private void OnDayDisplayLocationChanged(object? sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        // Reset button styles
        CenteredBtn.BackgroundColor = Color.FromArgb("#3A3A3A");
        UpperRightBtn.BackgroundColor = Color.FromArgb("#3A3A3A");

        // Highlight selected and apply
        button.BackgroundColor = Color.FromArgb("#7C3AED");

        if (button == CenteredBtn)
        {
            PreviewCalendar.CalendarDayDisplayLocation = CalendarDayDisplayLocationType.Centered;
        }
        else if (button == UpperRightBtn)
        {
            PreviewCalendar.CalendarDayDisplayLocation = CalendarDayDisplayLocationType.UpperRight;
        }
    }

    private void OnColorChanged(object? sender, Color e)
    {
        if (sender == HeaderTextColorPicker)
        {
            PreviewCalendar.HeaderTextColor = e;
        }
        else if (sender == SeparatorColorPicker)
        {
            PreviewCalendar.SeparatorColor = e;
        }
        else if (sender == DateTextColorPicker)
        {
            PreviewCalendar.DateTextColor = e;
        }
        else if (sender == SelectedDateColorPicker)
        {
            PreviewCalendar.SelectedDateColor = e;
        }
        else if (sender == SelectedDateTextColorPicker)
        {
            PreviewCalendar.SelectedDateTextColor = e;
        }
        else if (sender == DateBackgroundColorPicker)
        {
            PreviewCalendar.DateBackgroundColor = e;
        }
        else if (sender == UnavailableDateColorPicker)
        {
            PreviewCalendar.UnavailableDateColor = e;
        }
    }

    private void OnMinDateToggleChanged(object? sender, bool e)
    {
        if (e)
        {
            // Set minimum date to first of current month
            PreviewCalendar.MinimumDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }
        else
        {
            PreviewCalendar.MinimumDate = DateTime.MinValue;
        }
    }

    private void OnMaxDateToggleChanged(object? sender, bool e)
    {
        if (e)
        {
            // Set maximum date to 30 days from now
            PreviewCalendar.MaximumDate = DateTime.Now.AddDays(30);
        }
        else
        {
            PreviewCalendar.MaximumDate = DateTime.MaxValue;
        }
    }

    private void OnAddEventClicked(object? sender, EventArgs e)
    {
        // Add a single event for today
        PreviewCalendar.Events.Add(new CalendarEvent
        {
            Color = Color.FromArgb("#22C55E"),
            TextColor = Colors.White,
            DisplayText = "Event",
            EventDate = DateTime.Now,
        });
        UpdateEventCount();
    }

    private void OnAddSampleEventsClicked(object? sender, EventArgs e)
    {
        // Add sample events
        PreviewCalendar.Events.Add(new CalendarEvent
        {
            Color = Color.FromArgb("#22C55E"),
            TextColor = Colors.Black,
            DisplayText = "1",
            EventDate = DateTime.Now,
        });

        PreviewCalendar.Events.Add(new CalendarEvent
        {
            Color = Color.FromArgb("#8B5CF6"),
            TextColor = Colors.White,
            DisplayText = "2",
            EventDate = DateTime.Now,
        });

        PreviewCalendar.Events.Add(new CalendarEvent
        {
            CalendarEventDisplay = CalendarEventDisplayType.LargeEvent,
            Color = Color.FromArgb("#3B82F6"),
            TextColor = Colors.White,
            DisplayText = "$2,751\nMeeting",
            EventDate = DateTime.Now.AddDays(1),
        });

        PreviewCalendar.Events.Add(new CalendarEvent
        {
            CalendarEventDisplay = CalendarEventDisplayType.LargeEvent,
            Color = Color.FromArgb("#F59E0B"),
            TextColor = Colors.Black,
            DisplayText = "Deadline\nProject Due",
            EventDate = DateTime.Now.AddDays(3),
        });

        PreviewCalendar.Events.Add(new CalendarEvent
        {
            Color = Color.FromArgb("#EF4444"),
            TextColor = Colors.White,
            DisplayText = "!",
            EventDate = DateTime.Now.AddDays(7),
        });

        UpdateEventCount();
    }

    private void OnClearEventsClicked(object? sender, EventArgs e)
    {
        PreviewCalendar.Events.Clear();
        UpdateEventCount();
    }

    private void OnCalendarSelectedDatesChanged(object? sender, CalendarSelectedDatesChangedEventArgs e)
    {
        UpdateSelectedDatesLabel();
    }
}
