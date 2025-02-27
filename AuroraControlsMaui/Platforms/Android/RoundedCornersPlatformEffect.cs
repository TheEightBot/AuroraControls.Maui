using System.ComponentModel;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace AuroraControls;

public class RoundedCornersPlatformEffect : PlatformEffect
{
    private bool _originalClipToOutline;
    private ViewOutlineProvider _originalOutlineProvider;
    private double _scalingFactor;
    private Drawable _originalBackground;

    protected override void OnAttached()
    {
        var view = Container;

        if (view == null || Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
        {
            return;
        }

        _originalClipToOutline = view.ClipToOutline;
        _originalOutlineProvider = view.OutlineProvider;
        _originalBackground = view.Background;

        view.ClipToOutline = true;
        view.OutlineProvider = ViewOutlineProvider.Bounds;

        _scalingFactor = PlatformInfo.ScalingFactor;

        SetCornerRadius();
    }

    protected override void OnDetached()
    {
        var view = Container;

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
            view.Background = _originalBackground;
        }
        catch (ObjectDisposedException)
        {
        }
    }

    protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
    {
        base.OnElementPropertyChanged(args);

        if (args.PropertyName.Equals(Effects.RoundedCornersEffect.CornerRadiusProperty.PropertyName) ||
            args.PropertyName.Equals(Effects.RoundedCornersEffect.BorderColorProperty.PropertyName) ||
            args.PropertyName.Equals(Effects.RoundedCornersEffect.BorderSizeProperty.PropertyName) ||
            args.PropertyName.Equals(VisualElement.BackgroundColorProperty.PropertyName))
        {
            SetCornerRadius();
        }
    }

    private void SetCornerRadius()
    {
        var view = Container as Android.Views.View;

        if (view == null || view.Handle == IntPtr.Zero || Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
        {
            return;
        }

        var cornerRadius = Effects.RoundedCornersEffect.GetCornerRadius(this.Element);

        if (view.OutlineProvider != null & view.OutlineProvider.Handle != IntPtr.Zero &&
            view.OutlineProvider != _originalOutlineProvider)
        {
            var op = view.OutlineProvider;
            view.OutlineProvider = null;
            op.Dispose();
        }

        var scaledCornerRadius = (int)(cornerRadius * _scalingFactor);

        if (this.Element is VisualElement ve)
        {
            var shape = new GradientDrawable() { };
            shape.SetColor(ve.BackgroundColor.ToAndroid());
            shape.SetCornerRadius(scaledCornerRadius);

            var borderColor = Effects.RoundedCornersEffect.GetBorderColor(this.Element);
            var scaledBorderSize = (int)(Effects.RoundedCornersEffect.GetBorderSize(this.Element) * _scalingFactor);

            shape.SetStroke(scaledBorderSize, borderColor.ToAndroid());

            var background = view.Background;
            view.SetBackground(shape);
            background?.Dispose();
        }

        view.OutlineProvider = new RoundedViewOutline(scaledCornerRadius);
    }

    internal class RoundedViewOutline(int radius) : ViewOutlineProvider
    {
        public override void GetOutline(Android.Views.View view, Outline outline)
        {
            outline.SetRoundRect(0, 0, view.Width, view.Height, radius);
        }
    }
}
