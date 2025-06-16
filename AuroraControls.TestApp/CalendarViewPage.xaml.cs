using System.Collections.Specialized;

namespace AuroraControls.TestApp;

public partial class CalendarViewPage : ContentPage
{
    private bool _continueRunningTimer = true;

    public CalendarViewPage()
    {
        InitializeComponent();

        control.PropertyChanged += Control_PropertyChanged;

        if (control.SelectedDates is INotifyCollectionChanged incc)
        {
            incc.CollectionChanged += SelectedDates_CollectionChanged;
        }

        ChangeCurrentYear.Value = control.CurrentYear;
        ChangeCurrentMonth.Value = control.CurrentMonth;

        CurrentYear.Text = $"Current Year {control.CurrentYear}";
        CurrentMonth.Text = $"Current Month {control.CurrentMonth}";

        control
            .Events
            .Add(new CalendarEvent
            {
                Color = Colors.Green, TextColor = Colors.Black, DisplayText = "1", EventDate = DateTime.Now,
            });

        control
            .Events
            .Add(new CalendarEvent
            {
                Color = Colors.Purple, TextColor = Colors.White, DisplayText = "2", EventDate = DateTime.Now,
            });

        control
            .Events
            .Add(new CalendarEvent
            {
                CalendarEventDisplay = CalendarEventDisplayType.LargeEvent,
                Color = Colors.Green,
                TextColor = Colors.Black,
                DisplayText = "$2,751\nText Two\nOpen",
                EventDate = DateTime.Now.AddDays(1),
            });

        control
            .Events
            .Add(new CalendarEvent
            {
                CalendarEventDisplay = CalendarEventDisplayType.LargeEvent,
                Color = Colors.Green,
                TextColor = Colors.Black,
                DisplayText = "$2,751\n\nOpen\n\nMore Text",
                EventDate = DateTime.Now.AddDays(2),
            });

        control
            .Events
            .Add(new CalendarEvent
            {
                CalendarEventDisplay = CalendarEventDisplayType.LargeEvent,
                Color = Colors.Green,
                TextColor = Colors.Black,
                DisplayText = "$2,751\n\nOpen\n\nMore Text\n\nEven More Text",
                EventDate = DateTime.Now.AddDays(3),
            });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Device.StartTimer(
            TimeSpan.FromSeconds(1.5),
            () => { return _continueRunningTimer; });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _continueRunningTimer = false;
    }

    private async void UpdateValues_Clicked(object sender, System.EventArgs e)
    {
        var rngesus = new Random(Guid.NewGuid().GetHashCode());

        var separatorColor = Color.FromRgba(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255),
            rngesus.Next(0, 255));
        var headerTextColor = Color.FromRgba(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255),
            rngesus.Next(0, 255));
        var textColor = Color.FromRgba(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255),
            rngesus.Next(0, 255));
        var selectedColor = Color.FromRgba(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255),
            rngesus.Next(0, 255));
        var selectedTextColor = Color.FromRgba(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255),
            rngesus.Next(0, 255));
        var dateTextColor = Color.FromRgba(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255),
            rngesus.Next(0, 255));
        var dateTextBackgroundColor = Color.FromRgba(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255),
            rngesus.Next(0, 255));
        var availableDateColor = Color.FromRgba(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255),
            rngesus.Next(0, 255));
        var unavailableDateColor = Color.FromRgba(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255),
            rngesus.Next(0, 255));

        await Task.WhenAll(
            control.ColorTo(c => c.SeparatorColor, separatorColor),
            control.ColorTo(c => c.HeaderTextColor, textColor),
            control.ColorTo(c => c.DateColor, textColor),
            control.ColorTo(c => c.SelectedDateColor, selectedColor),
            control.ColorTo(c => c.SelectedDateTextColor, selectedTextColor),
            control.ColorTo(c => c.AvailableDateColor, availableDateColor),
            control.ColorTo(c => c.UnavailableDateColor, unavailableDateColor),
            control.ColorTo(c => c.DateTextColor, dateTextColor),
            control.ColorTo(c => c.DateBackgroundColor, dateTextBackgroundColor));
    }

    private void ToggleSingleSelection_Clicked(object sender, System.EventArgs e)
    {
        control.SelectionType = CalendarSelectionType.Single;
    }

    private void ToggleSpannedSelection_Clicked(object sender, System.EventArgs e)
    {
        control.SelectionType = CalendarSelectionType.Span;
    }

    private void ToggleMultipleSelection_Clicked(object sender, System.EventArgs e)
    {
        control.SelectionType = CalendarSelectionType.Multiple;
    }

    private void SwitchDayDisplay_Clicked(object sender, System.EventArgs e)
    {
        control.DayOfWeekDisplayType = control.DayOfWeekDisplayType.Next();
    }

    private void CurrentYear_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        control.CurrentYear = (int)e.NewValue;
        CurrentYear.Text = $"Current Year {control.CurrentYear}";
    }

    private void CurrentMonth_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        control.CurrentMonth = (int)e.NewValue;
        CurrentMonth.Text = $"Current Month {control.CurrentMonth}";
    }

    private void Control_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"Property Changed: {e.PropertyName}");

        if (e.PropertyName == CalendarView.SelectedDatesProperty.PropertyName)
        {
            foreach (var selectedDate in control.SelectedDates)
            {
                System.Diagnostics.Debug.WriteLine($"Selected Date: {selectedDate}");
            }
        }
    }

    private void SelectedDates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"Selected Date Changed\t{e.Action}");
    }
}
