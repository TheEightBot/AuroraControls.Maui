using System.Runtime.CompilerServices;

namespace AuroraControls;

public abstract class AuroraGLViewBase : SKGLView, IAuroraView
{
    private readonly VisualEffects.VisualEffectCollection _visualEffects = new();

    private bool _isPaintingSurface;
    private bool _needsDelayedPaint;
    private bool _attachmentClear;

    private IEnumerable<VisualEffects.VisualEffect> _enabledVisualEffects;

    protected float _scale;
    protected SKRect _overrideDrawableArea;

    public IList<VisualEffects.VisualEffect> VisualEffects => _visualEffects;

    protected bool IsAttached { get; private set; }

    public AuroraGLViewBase()
    {
        MinimumHeightRequest = 1;
        _scale = (float)PlatformInfo.ScalingFactor;
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

    protected abstract void PaintControl(SKSurface surface, GRBackendRenderTarget info);

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

    protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e) => PaintSurfaceInternal(e.Surface, e.BackendRenderTarget);

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName != null &&
            (propertyName.Equals(HeightProperty.PropertyName) ||
                propertyName.Equals(WidthProperty.PropertyName) ||
                propertyName.Equals(MarginProperty.PropertyName)))
        {
            this.InvalidateSurface();
        }
    }

    protected virtual void Attached()
    {
    }

    protected virtual void Detached()
    {
    }

    private void PaintSurfaceInternal(SKSurface surface, GRBackendRenderTarget renderTarget)
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

        if (renderTarget.Size == SKSize.Empty)
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

            PaintControl(surface, renderTarget);

            _enabledVisualEffects = VisualEffects.Where(x => x.Enabled).ToList();

            if (_enabledVisualEffects.Any())
            {
                if (!IsAttached)
                {
                    return;
                }

                using (new SKAutoCanvasRestore(surface.Canvas))
                {
                    var image = surface.Snapshot();

                    if (_overrideDrawableArea != default(SKRect))
                    {
                        surface.Canvas.ClipRect(_overrideDrawableArea);
                    }

                    foreach (var visualEffect in _enabledVisualEffects)
                    {
                        if (!IsAttached)
                        {
                            return;
                        }

                        var tmpImage = visualEffect.ApplyEffect(image, surface, renderTarget, _overrideDrawableArea);
                        image?.Dispose();
                        image = tmpImage;
                    }

                    if (!IsAttached)
                    {
                        return;
                    }

                    surface.Canvas.DrawImage(image, SKPoint.Empty);
                }
            }
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
