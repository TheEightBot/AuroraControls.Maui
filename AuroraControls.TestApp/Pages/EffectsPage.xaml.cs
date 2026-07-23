namespace AuroraControls.TestApp.Pages;

public partial class EffectsPage : ContentPage
{
    public EffectsPage()
    {
        InitializeComponent();
    }

    private async void OnImageProcessingTapped(object? sender, TappedEventArgs e)
    {
        if (sender is View view)
        {
            AnimationExtensions.TriggerHaptic();
            await view.AnimatePressAsync();
        }

        await Navigation.PushAsync(new ImageProcessing());
    }

    private async void OnKeyboardEffectTapped(object? sender, TappedEventArgs e)
    {
        if (sender is View view)
        {
            AnimationExtensions.TriggerHaptic();
            await view.AnimatePressAsync();
        }

        await Navigation.PushAsync(new KeyboardReturnKeyTypeEffectTestPage());
    }

    private async void OnDoneButtonTapped(object? sender, TappedEventArgs e)
    {
        if (sender is View view)
        {
            AnimationExtensions.TriggerHaptic();
            await view.AnimatePressAsync();
        }

        await Navigation.PushAsync(new ShowKeyboardDoneButtonEffectTestPage());
    }

    private async void OnKeyboardToolbarTapped(object? sender, TappedEventArgs e)
    {
        if (sender is View view)
        {
            AnimationExtensions.TriggerHaptic();
            await view.AnimatePressAsync();
        }

        await Navigation.PushAsync(new KeyboardToolbarTestPage());
    }

    private async void OnSafeAreaTapped(object? sender, TappedEventArgs e)
    {
        if (sender is View view)
        {
            AnimationExtensions.TriggerHaptic();
            await view.AnimatePressAsync();
        }

        await Navigation.PushAsync(new SafeAreaTestPage());
    }

    private async void OnNumericConvertersTapped(object? sender, TappedEventArgs e)
    {
        if (sender is View view)
        {
            AnimationExtensions.TriggerHaptic();
            await view.AnimatePressAsync();
        }

        await Navigation.PushAsync(new NumericConvertersTestPage());
    }
}
