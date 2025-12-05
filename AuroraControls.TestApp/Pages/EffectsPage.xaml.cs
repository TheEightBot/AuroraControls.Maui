namespace AuroraControls.TestApp.Pages;

/// <summary>
/// Effects page displaying image processing and platform effects.
/// </summary>
public partial class EffectsPage : ContentPage
{
    public EffectsPage()
    {
        InitializeComponent();
    }

    private async void OnImageProcessingTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Coming Soon", "The Image Processing demo page is coming in a future update.", "OK");
    }

    private async void OnPlatformEffectsTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Coming Soon", "The Platform Effects demo page is coming in a future update.", "OK");
    }
}
