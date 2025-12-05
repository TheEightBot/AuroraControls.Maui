namespace AuroraControls.TestApp.Pages;

/// <summary>
/// Showcase page displaying featured controls and library highlights.
/// </summary>
public partial class ShowcasePage : ContentPage
{
    public ShowcasePage()
    {
        InitializeComponent();
    }

    private async void OnGradientButtonsTapped(object sender, TappedEventArgs e)
    {
        await NavigateToRoute("gradientpillbutton");
    }

    private async void OnCalendarTapped(object sender, TappedEventArgs e)
    {
        await ShowComingSoon("Calendar View");
    }

    private async void OnInputsTapped(object sender, TappedEventArgs e)
    {
        await ShowComingSoon("Styled Input Layout");
    }

    private async void OnChipsTapped(object sender, TappedEventArgs e)
    {
        await ShowComingSoon("Chip Group");
    }

    private async void OnProgressTapped(object sender, TappedEventArgs e)
    {
        await ShowComingSoon("Gauges");
    }

    private async void OnAnimationsTapped(object sender, TappedEventArgs e)
    {
        await ShowComingSoon("Confetti");
    }

    private async Task NavigateToRoute(string route)
    {
        try
        {
            await Shell.Current.GoToAsync(route);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Navigation Error", $"Could not navigate to {route}: {ex.Message}", "OK");
        }
    }

    private async Task ShowComingSoon(string controlName)
    {
        await DisplayAlert("Coming Soon", $"The {controlName} demo page is coming in a future update.", "OK");
    }
}
