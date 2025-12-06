namespace AuroraControls.TestApp.Pages;

public partial class ShowcasePage : ContentPage
{
    public ShowcasePage()
    {
        InitializeComponent();
    }

    private async void OnExploreClicked(object? sender, EventArgs e)
    {
        // Navigate to GradientCircularButtonTestPage (GradientPillButton doesn't have a test page yet)
        await Navigation.PushAsync(new GradientCircularButtonTestPage());
    }

    private async void OnToggleBoxTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ToggleBoxTestPage());
    }

    private async void OnCalendarTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new CalendarViewPage());
    }

    private async void OnConfettiTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ConfettiViewTestPage());
    }
}
