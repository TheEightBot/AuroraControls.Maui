using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuroraControls;

public abstract class AuroraViewBase : SKCanvasView, IAuroraView
{
    private readonly VisualEffects.VisualEffectCollection _visualEffects = new();
    private readonly Stopwatch _renderStopwatch = new();

    private bool _isPaintingSurface;
    private bool _needsDelayedPaint;
    private bool _attachmentClear;

    private IEnumerable<VisualEffects.VisualEffect> _enabledVisualEffects = Array.Empty<VisualEffects.VisualEffect>();

    protected float _scale;
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
                width = (int)this._overrideDrawableArea.Width;
                height = height = (int)this._overrideDrawableArea.Height;
                break;
            case > 0 when maxHeight <= 0:
                var scaleAmount = maxWidth / this._overrideDrawableArea.Width;

                width = (int)this._overrideDrawableArea.Width;
                height = (int)(this._overrideDrawableArea.Height * scaleAmount);
                break;
            default:
                if (maxHeight > 0 && maxWidth <= 0)
                {
                    var defaultScaleAmount = maxHeight / this._overrideDrawableArea.Height;

                    height = (int)this._overrideDrawableArea.Height;
                    width = (int)(this._overrideDrawableArea.Width * defaultScaleAmount);
                }
                else
                {
                    var scaledHeightAmount = maxHeight / this._overrideDrawableArea.Height;
                    var scaledWidthAmount = maxWidth / this._overrideDrawableArea.Width;

                    var minScale = Math.Min(scaledHeightAmount, scaledWidthAmount);

                    height = (int)(this._overrideDrawableArea.Height * minScale);
                    width = (int)(this._overrideDrawableArea.Width * minScale);
                }

                break;
        }

        var imageInfo = new SKImageInfo(width, height, colorType);

        using var surface = SKSurface.Create(imageInfo);
        this.PaintSurfaceInternal(surface, imageInfo);

        var snapshot = surface?.Snapshot();
        if (snapshot == null)
        {
            return Stream.Null;
        }

        var encoded = snapshot.Encode(imageFormat, quality)?.AsStream() ?? Stream.Null;

        snapshot?.Dispose();
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

        _renderStopwatch.Restart();

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

            var paintControlStart = _renderStopwatch.ElapsedMilliseconds;
            PaintControl(surface, info);
            var paintControlDuration = _renderStopwatch.ElapsedMilliseconds - paintControlStart;
            Debug.WriteLine($"PaintControl took {paintControlDuration}ms");

            _enabledVisualEffects = VisualEffects.Where(x => x.Enabled).ToList();

            if (!_enabledVisualEffects.Any())
            {
                return;
            }

            if (!IsAttached)
            {
                return;
            }

            var effectsStart = _renderStopwatch.ElapsedMilliseconds;
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

                    var effectStart = _renderStopwatch.ElapsedMilliseconds;
                    var tmpImage = visualEffect.ApplyEffect(image, surface, info, _overrideDrawableArea);
                    image?.Dispose();
                    image = tmpImage;
                    var effectDuration = _renderStopwatch.ElapsedMilliseconds - effectStart;
                    Debug.WriteLine($"Visual effect {visualEffect.GetType().Name} took {effectDuration}ms");
                }

                if (!IsAttached)
                {
                    return;
                }

                surface.Canvas.DrawImage(image, SKPoint.Empty);
            }

            var effectsTotalDuration = _renderStopwatch.ElapsedMilliseconds - effectsStart;
            Debug.WriteLine($"All visual effects took {effectsTotalDuration}ms");
        }
        finally
        {
            _isPaintingSurface = false;

            if (_needsDelayedPaint)
            {
                _needsDelayedPaint = false;
                InvalidateSurface();
            }

            _renderStopwatch.Stop();
            var totalDuration = _renderStopwatch.ElapsedMilliseconds;
            Debug.WriteLine($"Total render time: {totalDuration}ms");
        }
    }

    private void VisualEffects_EffectPropertyChanged(object? sender, PropertyChangedEventArgs e) => InvalidateSurface();

    private void VisualEffects_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => InvalidateSurface();
}
