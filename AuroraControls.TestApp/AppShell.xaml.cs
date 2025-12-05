namespace AuroraControls.TestApp;

/// <summary>
/// Main application shell with TabBar navigation.
/// All detail page routes must be registered here for Shell.GoToAsync() to work.
/// </summary>
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        RegisterRoutes();
    }

    /// <summary>
    /// Registers all detail page routes for Shell navigation.
    /// IMPORTANT: Every page that uses Shell.GoToAsync() must be registered here.
    /// </summary>
    private void RegisterRoutes()
    {
        // ═══════════════════════════════════════════════════════════════════════
        // BUTTON CONTROLS
        // ═══════════════════════════════════════════════════════════════════════
        Routing.RegisterRoute("gradientpillbutton", typeof(Pages.Buttons.GradientPillButtonDemoPage));
        Routing.RegisterRoute("gradientcircularbutton", typeof(Pages.Buttons.GradientCircularButtonDemoPage));

        // TODO: Register more routes as demo pages are created
        // Routing.RegisterRoute("cupertinobutton", typeof(Pages.Buttons.CupertinoButtonDemoPage));
        // Routing.RegisterRoute("tile", typeof(Pages.Buttons.TileDemoPage));
        // Routing.RegisterRoute("svgimagebutton", typeof(Pages.Buttons.SvgImageButtonDemoPage));
    }
}