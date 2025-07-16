using System.Runtime.CompilerServices;

namespace AuroraControls;

public abstract class AuroraViewBase : SKCanvasView, IAuroraView
{
    private readonly VisualEffects.VisualEffectCollection _visualEffects = new();

    private bool _isPaintingSurface;
    private bool _needsDelayedPaint;
    private bool _attachmentClear;

    private IEnumerable<VisualEffects.VisualEffect> _enabledVisualEffects;

    protected readonly float _scale;
    protected SKRect _overrideDrawableArea;

    public IList<VisualEffects.VisualEffect> VisualEffects => _visualEffects;

    protected bool IsAttached { get; private set; }

    public AuroraViewBase()
    {
        MinimumHeightRequest = 1;
        _scale = (float)PlatformInfo.ScalingFactor;
    }

    public virtual Size CustomMeasuredSize(double widthConstraint, double heightConstraint) => Size.Zero;

    public Stream ExportImage(SKEncodedImageFormat imageFormat, int quality = 85, int maxWidth = -1, int maxHeight = -1, SKColorType colorType = SKColorType.Rgba8888)
    {
        int width = 0;
        int height = 0;

        switch (maxWidth)
        {
            case <= 0 when maxHeight <= 0:
                width = (int)_overrideDrawableArea.Width;
                height = height = (int)_overrideDrawableArea.Height;
                break;
            case > 0 when maxHeight <= 0:
                float scaleAmount = maxWidth / _overrideDrawableArea.Width;

                width = (int)_overrideDrawableArea.Width;
                height = (int)(_overrideDrawableArea.Height * scaleAmount);
                break;
            default:
                if (maxHeight > 0 && maxWidth <= 0)
                {
                    float defaultScaleAmount = maxHeight / _overrideDrawableArea.Height;

                    height = (int)_overrideDrawableArea.Height;
                    width = (int)(_overrideDrawableArea.Width * defaultScaleAmount);
                }
                else
                {
                    float scaledHeightAmount = maxHeight / _overrideDrawableArea.Height;
                    float scaledWidthAmount = maxWidth / _overrideDrawableArea.Width;

                    float minScale = Math.Min(scaledHeightAmount, scaledWidthAmount);

                    height = (int)(_overrideDrawableArea.Height * minScale);
                    width = (int)(_overrideDrawableArea.Width * minScale);
                }

                break;
        }

        if (height == 0 && width == 0)
        {
            height = (int)this.CanvasSize.Height;
            width = (int)this.CanvasSize.Width;
        }

        var imageInfo = new SKImageInfo(width, height, colorType);

        using var surface = SKSurface.Create(imageInfo);
        this.PaintSurfaceInternal(surface, imageInfo);

        var snapshot = surface?.Snapshot();

        var encoded = snapshot?.Encode(imageFormat, quality)?.AsStream() ?? Stream.Null;

        snapshot.Dispose();
        snapshot = null;

        return encoded;
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

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e) => PaintSurfaceInternal(e.Surface, e.Info);

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

            if (!_enabledVisualEffects.Any())
            {
                return;
            }

            if (!this.IsAttached)
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
                    if (!this.IsAttached)
                    {
                        return;
                    }

                    var tmpImage = visualEffect.ApplyEffect(image, surface, info, _overrideDrawableArea);
                    image?.Dispose();
                    image = tmpImage;
                }

                if (!this.IsAttached)
                {
                    return;
                }

                surface.Canvas.DrawImage(image, SKPoint.Empty);
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

    private void VisualEffects_EffectPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => this.InvalidateSurface();

    private void VisualEffects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => this.InvalidateSurface();
}
