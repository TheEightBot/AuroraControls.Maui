using AuroraControls.TestApp.Pages;

namespace AuroraControls.TestApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Load saved theme preference or default to dark
        LoadSavedTheme();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Use TabbedPage instead of Shell (Shell has compatibility issues with this project)
        var tabbedPage = new TabbedPage
        {
            Title = "Aurora Controls",
            SelectedTabColor = Color.FromArgb("#7C3AED"),
            UnselectedTabColor = Color.FromArgb("#9CA3AF"),
        };

        // Apply theme-aware colors to the tab bar
        UpdateTabbedPageColors(tabbedPage);

        // Subscribe to theme changes to update tab bar colors
        RequestedThemeChanged += (s, e) => UpdateTabbedPageColors(tabbedPage);

        // Wrap each page in NavigationPage for navigation support
        var showcaseNav = CreateNavigationPage(new ShowcasePage(), "Home", "icon_home.png");
        var controlsNav = CreateNavigationPage(new ControlsListPage(), "Controls", "icon_controls.png");
        var effectsNav = CreateNavigationPage(new EffectsPage(), "Effects", "icon_effects.png");
        var settingsNav = CreateNavigationPage(new SettingsPage(), "Settings", "icon_settings.png");

        tabbedPage.Children.Add(showcaseNav);
        tabbedPage.Children.Add(controlsNav);
        tabbedPage.Children.Add(effectsNav);
        tabbedPage.Children.Add(settingsNav);

        return new Window(tabbedPage);
    }

    private NavigationPage CreateNavigationPage(Page page, string title, string icon)
    {
        var navPage = new NavigationPage(page)
        {
            Title = title,
            IconImageSource = icon,
        };

        UpdateNavigationPageColors(navPage);

        // Subscribe to theme changes
        RequestedThemeChanged += (s, e) => UpdateNavigationPageColors(navPage);

        return navPage;
    }

    private void UpdateTabbedPageColors(TabbedPage tabbedPage)
    {
        var isDark = UserAppTheme == AppTheme.Dark ||
                     (UserAppTheme == AppTheme.Unspecified && RequestedTheme == AppTheme.Dark);

        tabbedPage.BarBackgroundColor = isDark
            ? Color.FromArgb("#1A1A1A")
            : Color.FromArgb("#FFFFFF");

        tabbedPage.BarTextColor = isDark
            ? Colors.White
            : Color.FromArgb("#111827");
    }

    private void UpdateNavigationPageColors(NavigationPage navPage)
    {
        var isDark = UserAppTheme == AppTheme.Dark ||
                     (UserAppTheme == AppTheme.Unspecified && RequestedTheme == AppTheme.Dark);

        navPage.BarBackgroundColor = isDark
            ? Color.FromArgb("#1A1A1A")
            : Color.FromArgb("#FFFFFF");

        navPage.BarTextColor = isDark
            ? Colors.White
            : Color.FromArgb("#111827");
    }

    private void LoadSavedTheme()
    {
        var savedTheme = Preferences.Default.Get("app_theme", (int)AppTheme.Dark);

        if (savedTheme != (int)AppTheme.Unspecified)
        {
            UserAppTheme = (AppTheme)savedTheme;
        }
        else
        {
            UserAppTheme = AppTheme.Dark;
        }
    }
}
