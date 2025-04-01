namespace AuroraControls.TestApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(SignaturePadPage), typeof(SignaturePadPage));
    }
}
