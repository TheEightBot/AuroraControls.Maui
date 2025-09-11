using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;

namespace AuroraControls;

public class ShadowPlatformEffect : PlatformEffect
{
    private double _scalingFactor;

    protected override void OnAttached()
    {
        var view = this.Container;

        if (view == null || view.Handle == IntPtr.Zero || Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
        {
            return;
        }

        _scalingFactor = PlatformInfo.ScalingFactor;

        SetElevation();
        SetShadowColor();
    }

    protected override void OnDetached()
    {
        var view = this.Container;

        if (view == null || view.Handle == IntPtr.Zero || Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
        {
            return;
        }
    }

    protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
    {
        base.OnElementPropertyChanged(args);

        if (args.PropertyName.Equals(Effects.ShadowEffect.ElevationProperty.PropertyName))
        {
            SetElevation();
        }
        else if (args.PropertyName.Equals(Effects.ShadowEffect.ShadowColorProperty.PropertyName))
        {
            SetShadowColor();
        }
    }

    private void SetElevation()
    {
        var view = Container as Android.Views.View;

        if (view == null || view.Handle == IntPtr.Zero || Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
        {
            return;
        }

        float elevation = (float)Effects.ShadowEffect.GetElevation(this.Element);

        view.Elevation = elevation;
        view.TranslationZ = elevation;
    }

    private void SetShadowColor()
    {
        var view = this.Container;

        if (view == null || view.Handle == IntPtr.Zero || Build.VERSION.SdkInt < BuildVersionCodes.P)
        {
            return;
        }

        var shadowColor = Effects.ShadowEffect.GetShadowColor(this.Element);

        if (Equals(shadowColor, Colors.Transparent))
        {
            return;
        }

        var androidShadowColor = shadowColor?.ToAndroidColor() ?? Android.Graphics.Color.Black;

        view.SetOutlineAmbientShadowColor(androidShadowColor);
        view.SetOutlineSpotShadowColor(androidShadowColor);
    }
}
