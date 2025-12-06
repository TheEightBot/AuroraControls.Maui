using AuroraControls.TestApp.Pages;

namespace AuroraControls.TestApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        UserAppTheme = AppTheme.Dark;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Use TabbedPage instead of Shell (Shell has compatibility issues with this project)
        var tabbedPage = new TabbedPage
        {
            Title = "Aurora Controls",
            BarBackgroundColor = Color.FromArgb("#1A1A1A"),
            BarTextColor = Colors.White,
            SelectedTabColor = Color.FromArgb("#7C3AED"),
            UnselectedTabColor = Color.FromArgb("#9CA3AF"),
        };

        // Wrap each page in NavigationPage for navigation support
        var showcaseNav = new NavigationPage(new ShowcasePage())
        {
            Title = "Showcase",
            IconImageSource = "icon_home.svg",
            BarBackgroundColor = Color.FromArgb("#1A1A1A"),
            BarTextColor = Colors.White,
        };

        var controlsNav = new NavigationPage(new ControlsListPage())
        {
            Title = "Controls",
            IconImageSource = "icon_controls.svg",
            BarBackgroundColor = Color.FromArgb("#1A1A1A"),
            BarTextColor = Colors.White,
        };

        var effectsNav = new NavigationPage(new EffectsPage())
        {
            Title = "Effects",
            IconImageSource = "icon_effects.svg",
            BarBackgroundColor = Color.FromArgb("#1A1A1A"),
            BarTextColor = Colors.White,
        };

        var settingsNav = new NavigationPage(new SettingsPage())
        {
            Title = "Settings",
            IconImageSource = "icon_settings.svg",
            BarBackgroundColor = Color.FromArgb("#1A1A1A"),
            BarTextColor = Colors.White,
        };

        tabbedPage.Children.Add(showcaseNav);
        tabbedPage.Children.Add(controlsNav);
        tabbedPage.Children.Add(effectsNav);
        tabbedPage.Children.Add(settingsNav);

        return new Window(tabbedPage);
    }
}
