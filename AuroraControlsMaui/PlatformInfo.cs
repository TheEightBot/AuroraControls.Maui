using System;

#if IOS
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

    public static double ScalingFactor
    {
        get
        {
            if (_scalingFactor > 0d)
            {
                return _scalingFactor;
            }

#if ANDROID
            _scalingFactor = Android.App.Application.Context.Resources.DisplayMetrics.Density;
#elif IOS
            _scalingFactor = UIScreen.MainScreen.Scale;
#endif

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
#elif IOS
            using var fontFamily = UIKit.UIFont.SystemFontOfSize(UIKit.UIFont.ButtonFontSize);
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

#if IOS
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

#if IOS
            _devicePpi = ScalingFactor * BaselinePpi;
#elif ANDROID
            _devicePpi = (double)Android.App.Application.Context.Resources.DisplayMetrics.DensityDpi;
#endif

            return _devicePpi;
        }
    }
}