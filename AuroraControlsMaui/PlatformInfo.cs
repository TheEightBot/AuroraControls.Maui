using Microsoft.Maui.Platform;

#if ANDROID
using Android.Views;
#elif IOS || MACCATALYST
using UIKit;
#endif

namespace AuroraControls;

public static class PlatformInfo
{
    private const int BaselinePpi = 160;

    private static double _scalingFactor = -1d;

    private static double _devicePpi = -1d;

    private static SKTypeface _defaultTypeface;

    private static double _defaultButtonFontSize;

    private static string _iconCacheDirectory;

    private static Rect _unknownLocation = new Rect(-1d, -1d, -1d, -1d);

    public static void Init()
    {
        _ = IconCacheDirectory;
        _ = ScalingFactor;
        _ = DefaultTypeface;
        _ = DefaultButtonFontSize;
        _ = DevicePpi;
    }

    public static string IconCacheDirectory
    {
        get
        {
            if (!string.IsNullOrEmpty(_iconCacheDirectory))
            {
                return _iconCacheDirectory;
            }

            _iconCacheDirectory = Path.Combine(FileSystem.CacheDirectory, "aurora.icons");

            return _iconCacheDirectory;
        }
    }

    public static double ScalingFactor
    {
        get
        {
            if (_scalingFactor > 0d)
            {
                return _scalingFactor;
            }

            _scalingFactor = DeviceDisplay.Current.MainDisplayInfo.Density;

            return _scalingFactor;
        }
    }

    public static SKTypeface DefaultTypeface
    {
        get
        {
            if (_defaultTypeface is not null)
            {
                return _defaultTypeface;
            }

#if ANDROID
            // This looks silly, but it should result in grabbing the system default font
            _defaultTypeface = SKTypeface.CreateDefault();
#elif IOS || MACCATALYST

            using var fontFamily = UIFont.SystemFontOfSize(UIFont.ButtonFontSize);
            _defaultTypeface = SKTypeface.FromFamilyName(fontFamily.FamilyName, SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
#endif
            return _defaultTypeface;
        }
    }

    public static double DefaultButtonFontSize
    {
        get
        {
            if (Math.Abs(_defaultButtonFontSize - default(double)) > .01d)
            {
                return _defaultButtonFontSize;
            }

#if IOS || MACCATALYST
            _defaultButtonFontSize = (double)UIFont.ButtonFontSize;
#elif ANDROID
            using var btn = new Android.Widget.Button(Android.App.Application.Context);
            _defaultButtonFontSize = (double)btn.TextSize / ScalingFactor;
#endif

            return _defaultButtonFontSize;
        }
    }

    public static double DevicePpi
    {
        get
        {
            if (_devicePpi > 0d)
            {
                return _devicePpi;
            }

#if IOS || MACCATALYST
            _devicePpi = ScalingFactor * BaselinePpi;
#elif ANDROID
            _devicePpi = (double)Android.App.Application.Context.Resources.DisplayMetrics.DensityDpi;
#endif

            return _devicePpi;
        }
    }

    public static Rect GetLocationOfView(IElement view) => GetLocationOfView(view, view.Parent);

    public static Rect GetLocationOfView(IElement view, IElement parent)
    {
        if (view?.Handler?.MauiContext is null || parent?.Handler?.MauiContext is null)
        {
            return _unknownLocation;
        }

        var nativeChild = view.ToPlatform(view.Handler.MauiContext);
        var nativeParent = parent.ToPlatform(parent.Handler.MauiContext);

        if (nativeChild is null || nativeParent is null)
        {
            return _unknownLocation;
        }

#if ANDROID
        var location = new Android.Graphics.Rect();
        nativeChild.GetDrawingRect(location);

        if (nativeParent is ViewGroup vg)
        {
            vg.OffsetDescendantRectToMyCoords(nativeChild, location);

            return new Rect(location.Left, location.Top, location.Width(), location.Height());
        }

        return _unknownLocation;
#elif IOS || MACCATALYST
        var rect = nativeParent.ConvertRectFromView(nativeChild.Frame, nativeChild);
        return rect.ToRectangle();
#else
        return _unknownLocation;
#endif
    }
}
