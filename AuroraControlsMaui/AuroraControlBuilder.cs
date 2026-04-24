using System.Reflection;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AuroraControls;

public static class AuroraControlBuilder
{
    /// <summary>
    /// Registers Aurora Controls with optional keyboard toolbar configuration.
    /// </summary>
    /// <typeparam name="T">The application type, used to locate embedded resources.</typeparam>
    /// <param name="configure">
    /// Optional delegate to configure <see cref="KeyboardToolbarOptions.Default"/>.
    /// Example: <c>opts => opts.IsGloballyHidden = true</c>.
    /// </param>
    public static MauiAppBuilder UseAuroraControls<T>(
        this MauiAppBuilder mauiAppBuilder,
        Action<KeyboardToolbarOptions>? configure = null)
        where T : Application
    {
        configure?.Invoke(KeyboardToolbarOptions.Default);
        return UseAuroraControls(mauiAppBuilder, typeof(T).Assembly);
    }

    public static MauiAppBuilder UseAuroraControls(this MauiAppBuilder mauiAppBuilder, params Assembly[] resourceAssemblies)
    {
        mauiAppBuilder
            .UseSkiaSharp()
            .ConfigureMauiHandlers(
                handlers =>
                {
                    handlers.AddHandler<AuroraViewBase, AutoHeightSKCanvasViewHandler>();
                    handlers.AddHandler<StyledInputLayout, StyledInputLayoutHandler>();
                    handlers.AddHandler<NumericEntry, NumericEntryHandler>();
                    handlers.AddHandler<CalendarPicker, CalendarPickerHandler>();
                })
            .ConfigureEffects(
                effects =>
                {
#if ANDROID
                    effects
                        .Add<Effects.ImageProcessingEffect, Effects.ImageProcessingPlatformEffect>()
                        .Add<Effects.ShadowEffect, ShadowPlatformEffect>()
                        .Add<Effects.RoundedCornersEffect, RoundedCornersPlatformEffect>()
                        .Add<Effects.KeyboardReturnKeyTypeNameEffect, DroidKeyboardReturnKeyTypeNameEffect>();
#elif IOS
                    effects
                        .Add<Effects.ImageProcessingEffect, Effects.ImageProcessingPlatformEffect>()
                        .Add<Effects.ShadowEffect, ShadowPlatformEffect>()
                        .Add<Effects.RoundedCornersEffect, RoundedCornersPlatformEffect>()
                        .Add<Effects.SafeAreaEffect, SafeAreaPlatformEffect>()
                        .Add<Effects.KeyboardReturnKeyTypeNameEffect, AppleKeyboardReturnKeyTypeNameEffect>()
                        .Add<Effects.ListViewHideEmptyCellsEffect, AppleListViewHideEmptyCellsEffect>()
                        .Add<Effects.ShowKeyboardDoneButtonEffect, AppleShowKeyboardDoneButtonEffect>()
                        .Add<Effects.KeyboardToolbarEffect, AppleKeyboardToolbarEffect>();
#elif MACCATALYST
                    effects
                        .Add<Effects.ImageProcessingEffect, Effects.ImageProcessingPlatformEffect>()
                        .Add<Effects.ShadowEffect, ShadowPlatformEffect>()
                        .Add<Effects.RoundedCornersEffect, RoundedCornersPlatformEffect>()
                        .Add<Effects.SafeAreaEffect, SafeAreaPlatformEffect>()
                        .Add<Effects.KeyboardReturnKeyTypeNameEffect, AppleKeyboardReturnKeyTypeNameEffect>()
                        .Add<Effects.ListViewHideEmptyCellsEffect, AppleListViewHideEmptyCellsEffect>()
                        .Add<Effects.ShowKeyboardDoneButtonEffect, MacCatalystShowKeyboardDoneButtonEffect>()
                        .Add<Effects.KeyboardToolbarEffect, MacCatalystKeyboardToolbarEffect>();
#endif
                })
            .ConfigureImageSources(
                services =>
                {
                    services.AddService<NoCacheFileImageSource>(svcs => new NoCacheFileImageSourceService(svcs.GetService<ILogger<NoCacheFileImageSourceService>>()));
                });

        foreach (var assembly in resourceAssemblies)
        {
            EmbeddedResourceLoader.LoadAssembly(assembly);
        }

        PlatformInfo.Init();

#if ANDROID
        mauiAppBuilder.Services.AddSingleton<IIconCache, AuroraControls.Platforms.Android.IconCache>();
#elif IOS
        mauiAppBuilder.Services.AddSingleton<IIconCache, Platforms.iOS.IconCache>();

        RegisterGlobalKeyboardSuppress();
#elif MACCATALYST
        mauiAppBuilder.Services.AddSingleton<IIconCache, Platforms.MacCatalyst.IconCache>();

        RegisterGlobalKeyboardSuppress();
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

#if IOS || MACCATALYST
    private static bool _suppressRegistered;

    private static void RegisterGlobalKeyboardSuppress()
    {
        if (_suppressRegistered || !KeyboardToolbarOptions.Default.IsGloballyHidden)
        {
            return;
        }

        _suppressRegistered = true;

        // AppendToMapping with a custom key is invoked once per handler instantiation,
        // which runs after MAUI has already set InputAccessoryView = MauiDoneAccessoryView.
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(
            "AuroraGlobalKeyboardDoneButtonSuppress",
            (handler, _) => SuppressMauiDoneAccessoryView(handler.PlatformView));

        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping(
            "AuroraGlobalKeyboardDoneButtonSuppress",
            (handler, _) => SuppressMauiDoneAccessoryView(handler.PlatformView));
    }

    private static void SuppressMauiDoneAccessoryView(UIKit.UIView? platformView)
    {
        if (platformView == null)
        {
            return;
        }

        UIKit.UIView? iav;
        if (platformView is UIKit.UITextField tf)
        {
            iav = tf.InputAccessoryView;

            // Guard by type name — we can't reference the internal MauiDoneAccessoryView type directly.
            if (iav?.GetType().Name == "MauiDoneAccessoryView")
            {
                tf.InputAccessoryView = null;
                iav.Dispose();
            }
        }
        else if (platformView is UIKit.UITextView tv)
        {
            iav = tv.InputAccessoryView;

            // Guard by type name — we can't reference the internal MauiDoneAccessoryView type directly.
            if (iav?.GetType().Name == "MauiDoneAccessoryView")
            {
                tv.InputAccessoryView = null;
                iav.Dispose();
            }
        }
    }
#endif
}
