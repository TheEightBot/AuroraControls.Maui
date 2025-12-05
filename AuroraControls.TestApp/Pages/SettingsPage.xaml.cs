namespace AuroraControls.TestApp.Pages;

/// <summary>
/// Settings page for theme and developer options.
/// </summary>
public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        // Initialize dark mode switch based on current theme
        DarkModeSwitch.IsToggled = Application.Current?.RequestedTheme == AppTheme.Dark;
    }

    private void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        }
    }

    private async void OnGitHubTapped(object sender, TappedEventArgs e)
    {
        try
        {
            await Launcher.OpenAsync("https://github.com/AuroraControls/AuroraControls.Maui");
        }
        catch
        {
            // Handle error silently
        }
    }

    private async void OnDocsTapped(object sender, TappedEventArgs e)
    {
        try
        {
            await Launcher.OpenAsync("https://github.com/AuroraControls/AuroraControls.Maui/wiki");
        }
        catch
        {
            // Handle error silently
        }
    }
}
