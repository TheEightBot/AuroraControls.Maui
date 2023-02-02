using System;
namespace AuroraControls;

public static class PlatformInfo
{
    private static SKTypeface _defaultTypeface;

    public static SKTypeface DefaultTypeface
    {
        get
        {
            if (_defaultTypeface != null)
                return _defaultTypeface;

#if ANDROID
            //This looks silly, but it should result in grabbing the system default font
            _defaultTypeface = SKTypeface.CreateDefault();
#elif IOS
                var fontFamily = UIKit.UIFont.SystemFontOfSize(UIKit.UIFont.ButtonFontSize);

                _defaultTypeface = SKTypeface.FromFamilyName(fontFamily.FamilyName, SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
#endif
            return _defaultTypeface;
        }
    }
}

