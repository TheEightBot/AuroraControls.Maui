using System;
using Microsoft.Maui.Platform;

#if IOS
using UIKit;
using CoreGraphics;
using AuroraControls.Platforms.iOS;
using SKCanvasView = SkiaSharp.Views.iOS.SKCanvasView;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.iOS.SKPaintSurfaceEventArgs;
using PlatformView = UIKit.UIView;
#endif

namespace AuroraControls;

public class PlatformUnderlayDrawable
{
    private IView _virtualView;

#if IOS
    private SKCanvasView _canvas;

    private PlatformView _platformView;

    private UITapGestureRecognizer _canvasTapped;

    private UIView _commandView;

    private UITapGestureRecognizer _commandViewTapped;


    public void ConnectHandler(PlatformView platformView, IView virtualView)
    {
        _platformView = platformView;
        _virtualView = virtualView;

        if (_canvas == null)
        {
            _canvas = new SKCanvasView { Opaque = false };

            _canvas.PaintSurface += OnPaintSurface;
            _canvas.AddGestureRecognizer(
                _canvasTapped = new UITapGestureRecognizer(() =>
                {
                    _virtualView?.Focus();
                }));
            _platformView.InsertSubview(_canvas, 0);

            Invalidate();
        }
    }

    public void DisconnectHandler()
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
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        if (_virtualView is IUnderlayDrawable ud && _virtualView is View view && _platformView is IHaveInsets hi)
        {
            ud.DrawUnderlay(view, hi.GetViewLocation(), e.Surface, e.Info);
        }
    }


    private void OnCommandSet()
    {
        if (_virtualView is IUnderlayDrawable ud)
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
                            Frame = new CGRect(0, 0, (float)_virtualView.Width, (float)_virtualView.Height)
                        };
                    _commandView.AddGestureRecognizer(
                        _commandViewTapped = new UITapGestureRecognizer(() =>
                        {
                            if (_virtualView is IUnderlayDrawable ude && (ude.Command?.CanExecute(ude.CommandParameter) ?? false))
                            {
                                ude.Command.Execute(ude.CommandParameter);
                            }
                        }));
                }

                _platformView.AddSubview(_commandView);
                _platformView.BringSubviewToFront(_commandView);
            }
            else if (ud.Command == null && _commandView != null)
            {
                _commandView?.RemoveFromSuperview();
            }
        }
    }
#endif

    public void PlatformArrange(Rect rect)
    {
#if IOS
        if (_canvas != null)
        {
            _canvas.Frame = new CGRect(0, 0, (float)rect.Width, (float)rect.Height);
        }

        if (_commandView != null)
        {
            _commandView.Frame = new CGRect(0, 0, (float)rect.Width, (float)rect.Height);
        }
#endif
    }

    public void Invalidate()
    {
#if IOS
        _canvas.SetNeedsDisplay();
#endif
    }

    public void UpdateLayoutInsets(InsetsF inset)
    {
#if IOS
        if (_platformView is IHaveInsets hi)
        {
            hi.Inset = inset.ToNative();
        }
#endif
    }
}