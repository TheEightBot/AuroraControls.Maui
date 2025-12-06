namespace AuroraControls.TestApp.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Sync switch state with current theme
        if (Application.Current != null)
        {
            DarkModeSwitch.IsToggled = Application.Current.UserAppTheme == AppTheme.Dark ||
                                       (Application.Current.UserAppTheme == AppTheme.Unspecified &&
                                        Application.Current.RequestedTheme == AppTheme.Dark);
        }
    }

    private void OnDarkModeToggled(object? sender, ToggledEventArgs e)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;

            // Save preference
            Preferences.Default.Set("app_theme", (int)Application.Current.UserAppTheme);
        }
    }
}
