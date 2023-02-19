namespace AuroraControls;

public abstract class AuroraViewBase : SKCanvasView, IAuroraView
{
    private bool _isPaintingSurface;
    private bool _needsDelayedPaint;
    private bool _attachmentClear;

    protected float _scale;

    protected bool IsAttached { get; private set; }

    public AuroraViewBase()
    {
        _scale = (float)DeviceDisplay.Current.MainDisplayInfo.Density;
        MinimumHeightRequest = 1;
    }

    /// <summary>
    /// Invoked whenever the Parent of an element is set.
    /// </summary>
    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (Parent is not null)
        {
            this.InvalidateSurface();
        }
    }

    protected abstract void PaintControl(SKSurface surface, SKImageInfo info);

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);

        if (args.NewHandler is null)
        {
            IsAttached = false;

            Detached();

            return;
        }

        IsAttached = true;
        _attachmentClear = true;

        Attached();

        this.InvalidateSurface();
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        PaintSurfaceInternal(e.Surface, e.Info);
    }

    protected virtual void Attached()
    {
    }

    protected virtual void Detached()
    {
    }

    private void PaintSurfaceInternal(SKSurface surface, SKImageInfo info)
    {
        if (!IsAttached)
        {
            return;
        }

        if (_attachmentClear)
        {
            surface.Canvas.Clear(SKColors.Transparent);
            _attachmentClear = false;
        }

        if (info.Size == SKSize.Empty)
        {
            return;
        }

        if (_isPaintingSurface)
        {
            _needsDelayedPaint = true;
            return;
        }

        _isPaintingSurface = true;

        try
        {
            if (!IsAttached)
            {
                return;
            }

            PaintControl(surface, info);
        }
        finally
        {
            _isPaintingSurface = false;

            if (_needsDelayedPaint)
            {
                _needsDelayedPaint = false;
                this.InvalidateSurface();
            }
        }
    }
}