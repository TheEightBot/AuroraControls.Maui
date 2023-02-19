using System;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AuroraControls;

public static class AuroraControlBuilder
{

    public static MauiAppBuilder ConfigureAuroraControls(this MauiAppBuilder mauiAppBuilder)
    {
        return mauiAppBuilder
            .UseSkiaSharp()
            .ConfigureMauiHandlers(
                mauiHandlersCollection =>
                {
                    mauiHandlersCollection.AddHandler(typeof(StyledInputLayout), typeof(StyledInputLayoutHandler));
                });
    }
}

