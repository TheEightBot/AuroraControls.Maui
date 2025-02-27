using System;
using System.Reflection;
using AuroraControls.Effects;
using AuroraControls.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AuroraControls;

public static class AuroraControlBuilder
{
    public static MauiAppBuilder UseAuroraControls<T>(this MauiAppBuilder mauiAppBuilder)
        where T : Application
    {
        return UseAuroraControls(mauiAppBuilder, typeof(T).Assembly);
    }

    public static MauiAppBuilder UseAuroraControls(this MauiAppBuilder mauiAppBuilder, params Assembly[] resourceAssemblies)
    {
        mauiAppBuilder
            .UseSkiaSharp()
            .ConfigureMauiHandlers(
                mauiHandlersCollection =>
                {
                    mauiHandlersCollection.AddHandler(typeof(StyledInputLayout), typeof(StyledInputLayoutHandler));
                    mauiHandlersCollection.AddHandler(typeof(NumericEntry), typeof(NumericEntryHandler));
                })
            .ConfigureEffects(
                effects =>
                {
#if ANDROID
                    effects
                        .Add<Effects.ImageProcessingEffect, Effects.ImageProcessingPlatformEffect>()
                        .Add<Effects.ShadowEffect, ShadowPlatformEffect>()
                        .Add<Effects.RoundedCornersEffect, RoundedCornersPlatformEffect>();
#elif IOS || MACCATALYST
                    effects
                        .Add<Effects.ImageProcessingEffect, Effects.ImageProcessingPlatformEffect>()
                        .Add<Effects.ShadowEffect, ShadowPlatformEffect>()
                        .Add<Effects.RoundedCornersEffect, RoundedCornersPlatformEffect>()
                        .Add<Effects.NullableCalendarDatePickerEffect, NullableCalendarDatePickerPlatormEffect>();
#endif
                });

        foreach (var assembly in resourceAssemblies)
        {
            EmbeddedResourceLoader.LoadAssembly(assembly);
        }

        PlatformInfo.Init();

#if ANDROID
        mauiAppBuilder.Services.AddSingleton<IIconCache, AuroraControls.Platforms.Android.IconCache>();
#elif IOS
        mauiAppBuilder.Services.AddSingleton<IIconCache, AuroraControls.Platforms.iOS.IconCache>();
#elif MACCATALYST
        mauiAppBuilder.Services.AddSingleton<IIconCache, AuroraControls.Platforms.MacCatalyst.IconCache>();
#endif

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterStyledInputLayout<TView>(this MauiAppBuilder mauiAppBuilder, string valueChangedPropertyName, Func<TView, bool> hasValue, bool alignPlaceholderToTop = false)
        where TView : IView
    {
        StyledInputLayout
            .StyledInputLayoutContentRegistrations
            .Add(
                typeof(TView),
                StyledContentTypeRegistration.Build(valueChangedPropertyName, hasValue, alignPlaceholderToTop));

        return mauiAppBuilder;
    }
}
