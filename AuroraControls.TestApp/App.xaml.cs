namespace AuroraControls.TestApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Set dark mode as default
        UserAppTheme = AppTheme.Dark;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
