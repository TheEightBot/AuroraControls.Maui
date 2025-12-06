namespace AuroraControls.TestApp.Pages;

public partial class ControlsListPage : ContentPage
{
    public ControlsListPage()
    {
        InitializeComponent();
    }

    private async void OnGradientPillButtonTapped(object? sender, TappedEventArgs e)
    {
        // GradientPillButton doesn't have a test page yet, use GradientCircular
        await Navigation.PushAsync(new GradientCircularButtonTestPage());
    }

    private async void OnGradientCircularButtonTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new GradientCircularButtonTestPage());
    }

    private async void OnCupertinoButtonTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new CupertinoButtonTestPage());
    }

    private async void OnTileTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new TileTestPage());
    }

    private async void OnToggleBoxTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ToggleBoxTestPage());
    }

    private async void OnStyledInputTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new StyledInputLayoutTestPage());
    }

    private async void OnChipGroupTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ChipGroupPage());
    }

    private async void OnCalendarTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new CalendarViewPage());
    }

    private async void OnConfettiTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ConfettiViewTestPage());
    }

    private async void OnSvgImageTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new SvgImageViewTestPage());
    }

    private async void OnSignatureTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new SignaturePadPage());
    }
}
