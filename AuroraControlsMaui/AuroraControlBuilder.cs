using System;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AuroraControls;

public static class AuroraControlBuilder
{
    public static MauiAppBuilder ConfigureAuroraControls(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder
            .UseSkiaSharp()
            .ConfigureMauiHandlers(
                mauiHandlersCollection =>
                {
                    mauiHandlersCollection.AddHandler(typeof(StyledInputLayout), typeof(StyledInputLayoutHandler));
                });

#if ANDROID
        mauiAppBuilder.Services.AddSingleton<IIconCache, AuroraControls.Platforms.Android.IconCache>();
#elif IOS
        mauiAppBuilder.Services.AddSingleton<IIconCache, AuroraControls.Platforms.iOS.IconCache>();
#endif

        return mauiAppBuilder;
    }
}