using System.ComponentModel;
using System.Runtime.InteropServices;
using CoreGraphics;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using UIKit;

namespace AuroraControls;

public class RoundedCornersPlatformEffect : PlatformEffect
{
    private NFloat _originalCornerRadius;
    private bool _originalMasksToBounds;

    protected override void OnAttached()
    {
        var view = Container;

        if (view == null)
        {
            return;
        }

        _originalCornerRadius = view.Layer.CornerRadius;
        _originalMasksToBounds = view.Layer.MasksToBounds;

        view.Layer.MasksToBounds = true;

        SetCornerRadius();
    }

    protected override void OnDetached()
    {
        var view = Container;

        if (view == null)
        {
            return;
        }

        view.Layer.CornerRadius = _originalCornerRadius;
        view.Layer.MasksToBounds = _originalMasksToBounds;
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
        var view = Container;
        var ve = Element as VisualElement;

        if (view == null || ve == null)
        {
            return;
        }

        view.BackgroundColor = UIColor.Clear;
        view.Layer.BackgroundColor = ve.BackgroundColor?.ToCGColor() ?? UIColor.Clear.CGColor;
        view.Layer.EdgeAntialiasingMask = CoreAnimation.CAEdgeAntialiasingMask.All;
        view.Layer.BorderWidth = (NFloat)Effects.RoundedCornersEffect.GetBorderSize(this.Element);
        view.Layer.BorderColor = Effects.RoundedCornersEffect.GetBorderColor(this.Element)?.ToCGColor() ?? UIColor.Clear.CGColor;
        view.Layer.CornerRadius = (NFloat)Effects.RoundedCornersEffect.GetCornerRadius(this.Element);
    }
}
