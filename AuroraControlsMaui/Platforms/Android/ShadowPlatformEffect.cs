using Android.Graphics;
using Android.OS;
using Android.Views;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;

namespace AuroraControls;

public class ShadowPlatformEffect : PlatformEffect
{
    private bool _originalClipToOutline;
    private ViewOutlineProvider _originalOutlineProvider;
    private double _scalingFactor;

    protected override void OnAttached()
    {
        var view = this.Container;

        if (view == null || view.Handle == IntPtr.Zero || Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
        {
            return;
        }

        _originalClipToOutline = view.ClipToOutline;
        _originalOutlineProvider = view.OutlineProvider;

        view.ClipToOutline = true;
        view.OutlineProvider = ViewOutlineProvider.Bounds;

        _scalingFactor = PlatformInfo.ScalingFactor;

        SetElevation();
        SetCornerRadius();
        SetShadowColor();
    }

    protected override void OnDetached()
    {
        var view = this.Container;

        if (view == null || view.Handle == IntPtr.Zero || Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
        {
            return;
        }

        try
        {
            if (view.OutlineProvider != null && view.OutlineProvider != _originalOutlineProvider)
            {
                var op = view.OutlineProvider;
                view.OutlineProvider = null;
                op.Dispose();
            }

            view.ClipToOutline = _originalClipToOutline;
            view.OutlineProvider = _originalOutlineProvider;
        }
        catch (ObjectDisposedException)
        {
        }
    }

    protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
    {
        base.OnElementPropertyChanged(args);

        if (args.PropertyName.Equals(Effects.ShadowEffect.ElevationProperty.PropertyName))
        {
            SetElevation();
        }
        else if (args.PropertyName.Equals(Effects.ShadowEffect.CornerRadiusProperty.PropertyName))
        {
            SetCornerRadius();
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

    private void SetCornerRadius()
    {
        var view = this.Container;

        if (view == null || view.Handle == IntPtr.Zero || Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
        {
            return;
        }

        double cornerRadius = Effects.ShadowEffect.GetCornerRadius(this.Element);

        if (view.OutlineProvider != null && view.OutlineProvider != _originalOutlineProvider)
        {
            var op = view.OutlineProvider;
            view.OutlineProvider = null;
            op.Dispose();
        }

        view.OutlineProvider = new RoundedViewOutline((int)(cornerRadius * _scalingFactor));
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

        var androidShadowColor = shadowColor.ToAndroid();

        view.SetOutlineAmbientShadowColor(androidShadowColor);
        view.SetOutlineSpotShadowColor(androidShadowColor);
    }

    internal class RoundedViewOutline : ViewOutlineProvider
    {
        private int _radius;

        public RoundedViewOutline(int radius) => _radius = radius;

        public override void GetOutline(Android.Views.View view, Outline outline) => outline.SetRoundRect(0, 0, view.Width, view.Height, _radius);
    }
}
