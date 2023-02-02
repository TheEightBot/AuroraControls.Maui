using System;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Animations;
using Microsoft.Maui;
using System.Runtime.CompilerServices;


#if IOS
using UIKit;
using CoreGraphics;
using SKCanvasView = SkiaSharp.Views.iOS.SKCanvasView;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.iOS.SKPaintSurfaceEventArgs;
#endif

namespace AuroraControls;

public partial class FloatLabelEntryHandler : EntryHandler
{
    private SKCanvasView _canvas;

#if IOS
    private UITapGestureRecognizer _canvasTapped;

    private UIView _commandView;

    private UITapGestureRecognizer _commandViewTapped;
#endif

    public FloatLabelEntryHandler()
        : base(FloatLabelEntryMapper)
    {
    }

#if IOS
    protected override MauiTextField CreatePlatformView()
    {
        var platformView = new AuroraControls.Platforms.iOS.AuroraTextField();

        platformView.BackgroundColor = UIColor.Clear;
        platformView.BorderStyle = UITextBorderStyle.None;

        return platformView;
    }

    protected override void ConnectHandler(MauiTextField platformView)
    {
        base.ConnectHandler(platformView);

        if (_canvas == null)
        {
            _canvas = new SKCanvasView { Opaque = false };

            _canvas.PaintSurface += OnPaintSurface;
            _canvas.AddGestureRecognizer(
                _canvasTapped = new UITapGestureRecognizer(() =>
                {
                    VirtualView?.Focus();
                }));
            platformView.InsertSubview(_canvas, 0);

            Invalidate(this);
        }
    }

    protected override void DisconnectHandler(MauiTextField platformView)
    {
        if (_canvas != null)
        {
            _canvas.RemoveGestureRecognizer(_canvasTapped);
            _canvas.PaintSurface -= OnPaintSurface;
            _canvas.RemoveFromSuperview();
            _canvas?.Dispose();

            _canvasTapped?.Dispose();
            _canvasTapped = null;
        }

        if (_commandView != null)
        {
            _commandView.RemoveGestureRecognizer(_commandViewTapped);
            _commandView.RemoveFromSuperview();
            _commandView?.Dispose();
            _commandView = null;

            _commandView?.Dispose();
            _commandView = null;
        }

        base.DisconnectHandler(platformView);
    }

    //public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
    //{
    //    var sizeRequest = base.GetDesiredSize(widthConstraint, heightConstraint);

    //    if (VirtualView is IUnderlayDrawable ud)
    //    {
    //        return ud.GetDesiredSize(sizeRequest);
    //    }

    //    return sizeRequest;
    //}

    public override void PlatformArrange(Rect rect)
    {
        base.PlatformArrange(rect);

        if (_canvas != null)
        {
            _canvas.Frame = new CGRect(0, 0, (float)rect.Width, (float)rect.Height);
        }

        if (_commandView != null)
        {
            _commandView.Frame = new CGRect(0, 0, (float)rect.Width, (float)rect.Height);
        }
    }

    private void OnCommandSet()
    {
        if (VirtualView is IUnderlayDrawable ud)
        {
            if (ud.Command != null)
            {
                if (_commandView == null)
                {
                    _commandView =
                        new UIView
                        {
                            BackgroundColor = UIColor.Clear,
                            UserInteractionEnabled = true,
                            Frame = new CGRect(0, 0, (float)VirtualView.Width, (float)VirtualView.Height)
                        };
                    _commandView.AddGestureRecognizer(
                        _commandViewTapped = new UITapGestureRecognizer(() =>
                        {
                            if (VirtualView is IUnderlayDrawable ude && (ude.Command?.CanExecute(ude.CommandParameter) ?? false))
                            {
                                ude.Command.Execute(ude.CommandParameter);
                            }
                        }));
                }

                PlatformView.AddSubview(_commandView);
                PlatformView.BringSubviewToFront(_commandView);
            }
            else if (ud.Command == null && _commandView != null)
            {
                _commandView?.RemoveFromSuperview();
            }
        }
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        if (VirtualView is IUnderlayDrawable ud && VirtualView is View view)
        {
            var bounds = PlatformView.EditingRect(PlatformView.Bounds);

            ud.DrawUnderlay(view, bounds.ToRectangle(), e.Surface, e.Info);
        }
    }
#endif

    private void UpdateLayoutInsets(InsetsF inset)
    {
#if IOS
        if (PlatformView is AuroraControls.Platforms.iOS.AuroraTextField atf)
        {
            atf.Inset = inset.ToNative();
        }
#endif
    }

    private static void Invalidate(FloatLabelEntryHandler handler)
    {
#if IOS
        handler._canvas.SetNeedsDisplay();
#endif
    }
}