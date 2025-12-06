namespace AuroraControls.TestApp.Pages;

public partial class EffectsPage : ContentPage
{
    public EffectsPage()
    {
        InitializeComponent();
    }

    private async void OnImageProcessingTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ImageProcessing());
    }

    private async void OnKeyboardEffectTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new KeyboardReturnKeyTypeEffectTestPage());
    }

    private async void OnDoneButtonTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ShowKeyboardDoneButtonEffectTestPage());
    }

    private async void OnSafeAreaTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new SafeAreaTestPage());
    }
}
