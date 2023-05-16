using System;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AuroraControls;

public static class AuroraControlBuilder
{
    public static MauiAppBuilder UseAuroraControls(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder
            .UseSkiaSharp()
            .ConfigureMauiHandlers(
                mauiHandlersCollection =>
                {
                    mauiHandlersCollection.AddHandler(typeof(StyledInputLayout), typeof(StyledInputLayoutHandler));
                    mauiHandlersCollection.AddHandler(typeof(NumericEntry), typeof(NumericEntryHandler));
                });

#if ANDROID
        mauiAppBuilder.Services.AddSingleton<IIconCache, AuroraControls.Platforms.Android.IconCache>();
#elif IOS
        mauiAppBuilder.Services.AddSingleton<IIconCache, AuroraControls.Platforms.iOS.IconCache>();
#elif MACCATALYST
        mauiAppBuilder.Services.AddSingleton<IIconCache, AuroraControls.Platforms.MacCatalyst.IconCache>();
#endif

        return mauiAppBuilder;
    }
}