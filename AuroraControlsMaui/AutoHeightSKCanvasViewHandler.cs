using System.ComponentModel;
using Microsoft.Maui.Platform;
using SkiaSharp.Views.Maui.Handlers;

#if IOS || MACCATALYST
using SKCanvasView = SkiaSharp.Views.iOS.SKCanvasView;
#endif

namespace AuroraControls;

internal class AutoHeightSKCanvasViewHandler : SKCanvasViewHandler
{
#if IOS || MACCATALYST
    protected override void ConnectHandler(SKCanvasView platformView)
    {
        base.ConnectHandler(platformView);

        if (VirtualView is AuroraViewBase avb)
        {
            avb.PropertyChanged += AvbOnPropertyChanged;
        }
    }

    protected override void DisconnectHandler(SKCanvasView platformView)
    {
        if (VirtualView is AuroraViewBase avb)
        {
            avb.PropertyChanged -= AvbOnPropertyChanged;
        }

        base.DisconnectHandler(platformView);
    }

    private void AvbOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if ((e.PropertyName == nameof(AuroraViewBase.IsVisible) ||
             e.PropertyName == nameof(AuroraViewBase.WidthRequest) ||
             e.PropertyName == nameof(AuroraViewBase.HeightRequest))
            && VirtualView is AuroraViewBase { IsVisible: true })
        {
            PlatformView?.Superview?.SetNeedsLayout();
        }
    }
#endif

    public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
    {
        var custom = (this.VirtualView as AuroraViewBase)?.CustomMeasuredSize(widthConstraint, heightConstraint);

        var size = custom.HasValue && custom.Value != Size.Zero
            ? custom.Value
            : base.GetDesiredSize(widthConstraint, heightConstraint);

        return size;
    }
}
