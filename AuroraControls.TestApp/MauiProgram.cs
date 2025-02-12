using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using NPicker;
using SkiaSharp;

namespace AuroraControls.TestApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .UseNPicker()
            .RegisterStyledInputLayout<NPicker.DatePicker>(nameof(NPicker.DatePicker.Date), view => view.Date.HasValue)
            .ConfigureFonts(
                fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Clathing.otf", "Clathing");
                    fonts.AddFont("Font Awesome 6 Brands.otf", "FontAwesomeBrands");
                    fonts.AddFont("Font Awesome 6 Free Regular.otf", "FontAwesomeRegular");
                    fonts.AddFont("Font Awesome 6 Free Solid.otf", "FontAwesomeSolid");

                    fonts.AddToAuroraFontCache();
                })
            .UseAuroraControls<App>();

        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
