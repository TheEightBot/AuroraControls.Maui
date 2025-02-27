using System.Runtime.CompilerServices;

namespace AuroraControls;

public abstract class AuroraViewBase : SKCanvasView, IAuroraView
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

    public AuroraViewBase()
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

    protected abstract void PaintControl(SKSurface surface, SKImageInfo info);

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        PaintSurfaceInternal(e.Surface, e.Info);
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (string.IsNullOrEmpty(propertyName))
        {
            return;
        }

        if (propertyName.Equals(HeightProperty.PropertyName) ||
             propertyName.Equals(WidthProperty.PropertyName) ||
             propertyName.Equals(MarginProperty.PropertyName))
        {
            this.InvalidateSurface();
        }
        else if (propertyName.Equals(WindowProperty.PropertyName))
        {
            if (Window is null)
            {
                IsAttached = false;

                _visualEffects.EffectPropertyChanged -= VisualEffects_EffectPropertyChanged;

                _visualEffects.CollectionChanged -= VisualEffects_CollectionChanged;

                Detached();

                return;
            }

            IsAttached = true;
            _attachmentClear = true;

            _visualEffects.EffectPropertyChanged -= VisualEffects_EffectPropertyChanged;
            _visualEffects.EffectPropertyChanged += VisualEffects_EffectPropertyChanged;

            _visualEffects.CollectionChanged -= VisualEffects_CollectionChanged;
            _visualEffects.CollectionChanged += VisualEffects_CollectionChanged;

            Attached();

            this.InvalidateSurface();
        }
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

                        var tmpImage = visualEffect.ApplyEffect(image, surface, info, _overrideDrawableArea);
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

    private void VisualEffects_EffectPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        this.InvalidateSurface();
    }

    private void VisualEffects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        this.InvalidateSurface();
    }
}
