namespace AuroraControls.TestApp.Pages;

public partial class ShowcasePage : ContentPage
{
    public ShowcasePage()
    {
        InitializeComponent();
    }

    private async void OnExploreClicked(object? sender, EventArgs e)
    {
        if (sender is View view)
        {
            AnimationExtensions.TriggerHaptic();
            await view.AnimatePressAsync();
        }

        // Navigate to GradientCircularButtonTestPage (GradientPillButton doesn't have a test page yet)
        await Navigation.PushAsync(new GradientCircularButtonTestPage());
    }

    private async void OnToggleBoxTapped(object? sender, TappedEventArgs e)
    {
        if (sender is View view)
        {
            AnimationExtensions.TriggerHaptic();
            await view.AnimatePressAsync();
        }

        await Navigation.PushAsync(new ToggleBoxTestPage());
    }

    private async void OnCalendarTapped(object? sender, TappedEventArgs e)
    {
        if (sender is View view)
        {
            AnimationExtensions.TriggerHaptic();
            await view.AnimatePressAsync();
        }

        await Navigation.PushAsync(new CalendarViewPage());
    }

    private async void OnConfettiTapped(object? sender, TappedEventArgs e)
    {
        if (sender is View view)
        {
            AnimationExtensions.TriggerHaptic();
            await view.AnimatePressAsync();
        }

        await Navigation.PushAsync(new ConfettiViewTestPage());
    }
}
