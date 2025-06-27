using System.Runtime.InteropServices;
using CoreGraphics;
using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace AuroraControls;

public class ShadowPlatformEffect : PlatformEffect
{
    private NFloat _originalCornerRadius, _originalShadowOpacity, _originalShadowRadius;
    private bool _originalMasksToBounds;
    private CGColor _originalShadowColor;
    private CGSize _originalShadowOffset;
    private CGPath _originalShadowPath;

    protected override void OnAttached()
    {
        var view = this.Container;

        if (view == null)
        {
            return;
        }

        _originalCornerRadius = view.Layer.CornerRadius;
        _originalMasksToBounds = view.Layer.MasksToBounds;
        _originalShadowColor = view.Layer.ShadowColor;
        _originalShadowOffset = view.Layer.ShadowOffset;
        _originalShadowOpacity = view.Layer.ShadowOpacity;
        _originalShadowPath = view.Layer.ShadowPath;
        _originalShadowRadius = view.Layer.ShadowRadius;

        // defaults for shadows
        view.Layer.MasksToBounds = false;
        view.Layer.ShadowColor = UIColor.Black.CGColor;
        view.Layer.ShadowOpacity = .4f;
        view.Layer.ShouldRasterize = true;
        view.Layer.RasterizationScale = UIScreen.MainScreen.Scale;

        SetShadowDistance();
    }

    protected override void OnDetached()
    {
        var view = this.Container;

        if (view == null)
        {
            return;
        }

        if (view.Layer.ShadowPath != null && view.Layer.ShadowPath != _originalShadowPath)
        {
            var sp = view.Layer.ShadowPath;
            view.Layer.ShadowPath = null;
            sp?.Dispose();
        }

        view.Layer.CornerRadius = _originalCornerRadius;
        view.Layer.MasksToBounds = _originalMasksToBounds;
        view.Layer.ShadowColor = _originalShadowColor;
        view.Layer.ShadowOffset = _originalShadowOffset;
        view.Layer.ShadowOpacity = (float)_originalShadowOpacity;
        view.Layer.ShadowPath = _originalShadowPath;
        view.Layer.ShadowRadius = _originalShadowRadius;
    }

    protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
    {
        base.OnElementPropertyChanged(args);

        if (args.PropertyName.Equals(Effects.ShadowEffect.ElevationProperty.PropertyName))
        {
            SetShadowDistance();
        }
        else if (args.PropertyName.Equals(Effects.ShadowEffect.CornerRadiusProperty.PropertyName) ||
                 args.PropertyName.Equals(VisualElement.HeightProperty.PropertyName) ||
                 args.PropertyName.Equals(VisualElement.WidthProperty.PropertyName))
        {
            UpdateShadowPath();
        }
    }

    private void SetShadowDistance()
    {
        var view = Container;
        var ve = Element as VisualElement;

        if (view == null || ve == null)
        {
            return;
        }

        double shadowDistance = Effects.ShadowEffect.GetElevation(this.Element);
        view.Layer.ShadowOffset = new CGSize(0f, shadowDistance);

        UpdateShadowPath();
    }

    private void UpdateShadowPath()
    {
        var view = this.Container;
        var ve = Element as VisualElement;

        if (view == null || ve == null)
        {
            return;
        }

        double cornerRadius = Effects.ShadowEffect.GetCornerRadius(this.Element);
        double elevation = Effects.ShadowEffect.GetElevation(this.Element);

        if (ve.Bounds == Rect.Zero)
        {
            return;
        }

        if (view.Layer.ShadowPath != null && view.Layer.ShadowPath != this._originalShadowPath)
        {
            var sp = view.Layer.ShadowPath;
            view.Layer.ShadowPath = null;
            sp?.Dispose();
        }

        view.Layer.ShadowPath = UIBezierPath
            .FromRoundedRect(new CGRect(0f, 0f, ve.Width, ve.Height), (float)cornerRadius).CGPath;
        view.Layer.ShadowOffset = new CGSize(0, elevation);
        view.Layer.ShadowRadius = (float)elevation * .8f;
    }
}
