using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Svg.Skia;

namespace AuroraControls;

[ContentProperty(nameof(Segments))]
public class SegmentedControl : AuroraViewBase
{
    private readonly ObservableCollection<Segment> _segments = new ObservableCollection<Segment>();

    private SKRect _controlRect;
    private int _segmentCount, _previousIndex;

    public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;

    public IList<Segment> Segments { get => _segments; }

    public static BindableProperty SegmentControlStyleProperty =
        BindableProperty.Create(nameof(SegmentControlStyle), typeof(SegmentedControlStyle), typeof(SegmentedControl), SegmentedControlStyle.Cupertino);

    public SegmentedControlStyle SegmentControlStyle
    {
        get => (SegmentedControlStyle)GetValue(SegmentControlStyleProperty);
        set => SetValue(SegmentControlStyleProperty, value);
    }

    public static BindableProperty ControlForegroundColorProperty =
        BindableProperty.Create(nameof(ControlForegroundColor), typeof(Color), typeof(SegmentedControl), Color.FromArgb("#FF007AFF"),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public Color ControlForegroundColor
    {
        get => (Color)GetValue(ControlForegroundColorProperty);
        set => SetValue(ControlForegroundColorProperty, value);
    }

    public static BindableProperty ControlBackgroundColorProperty =
        BindableProperty.Create(nameof(ControlBackgroundColor), typeof(Color), typeof(SegmentedControl), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public Color ControlBackgroundColor
    {
        get => (Color)GetValue(ControlBackgroundColorProperty);
        set => SetValue(ControlBackgroundColorProperty, value);
    }

    public static BindableProperty BorderSizeProperty =
        BindableProperty.Create(nameof(BorderSize), typeof(double), typeof(SegmentedControl), 2d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public double BorderSize
    {
        get => (double)GetValue(BorderSizeProperty);
        set => SetValue(BorderSizeProperty, value);
    }

    public static BindableProperty ForegroundTextColorProperty =
        BindableProperty.Create(nameof(ForegroundTextColor), typeof(Color), typeof(SegmentedControl), Colors.White);

    public Color ForegroundTextColor
    {
        get => (Color)GetValue(ForegroundTextColorProperty);
        set => SetValue(ForegroundTextColorProperty, value);
    }

    public static BindableProperty BackgroundTextColorProperty =
        BindableProperty.Create(nameof(BackgroundTextColor), typeof(Color), typeof(SegmentedControl), Colors.Transparent);

    public Color BackgroundTextColor
    {
        get => (Color)GetValue(BackgroundTextColorProperty);
        set => SetValue(BackgroundTextColorProperty, value);
    }

    public static BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(SegmentedControl), PlatformInfo.DefaultButtonFontSize,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static BindableProperty FontFamilyProperty =
        BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(SegmentedControl), default(string),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public string FontFamily
    {
        get => (string)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public static BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(SegmentedControl), 4,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public int CornerRadius
    {
        get => (int)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static BindableProperty SelectedIndexProperty =
        BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(SegmentedControl), -1,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is SegmentedControl sc)
                {
                    sc?.InvalidateSurface();
                    sc.SelectedItem = sc.Segments?.ElementAtOrDefault((int)newValue)?.Value;
                }
            });

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SegmentedControl), null, BindingMode.OneWayToSource,
        propertyChanged:
            (bindable, oldValue, newValue) =>
            {
                if (bindable is SegmentedControl sc)
                {
                    for (int i = 0; i < sc.Segments.Count(); i++)
                    {
                        var segment = sc.Segments[i];

                        if (segment?.Value?.Equals(newValue) ?? false)
                        {
                            sc.SelectedIndex = i;
                            sc.ItemSelected?.Invoke(bindable, new SelectedItemChangedEventArgs(newValue, i));
                            return;
                        }
                    }
                }
            });

    public object SelectedItem
    {
        get => (object)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public SegmentedControl()
    {
        MinimumHeightRequest = IAuroraView.StandardControlHeight;
    }

    protected override void Attached()
    {
        this.EnableTouchEvents = true;

        _segments.CollectionChanged -= SegmentsCollectionChanged;
        _segments.CollectionChanged += SegmentsCollectionChanged;

        foreach (INotifyPropertyChanged item in _segments)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            item.PropertyChanged += Item_PropertyChanged;
        }

        base.Attached();
    }

    protected override void Detached()
    {
        _segments.CollectionChanged -= SegmentsCollectionChanged;

        foreach (INotifyPropertyChanged item in _segments)
        {
            item.PropertyChanged -= Item_PropertyChanged;
        }

        base.Detached();
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName.Equals(VisualElement.HeightProperty.PropertyName) ||
           propertyName.Equals(VisualElement.WidthProperty.PropertyName) ||
           propertyName.Equals(View.MarginProperty.PropertyName))
        {
            this.InvalidateSurface();
        }
    }

    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        using (new SKAutoCanvasRestore(canvas))
        using (var paint = new SKPaint())
        using (var drawingClip = new SKPath())
        {
            paint.IsAntialias = true;
            paint.Style = SKPaintStyle.Fill;

            var scaledCornerRadius =
                this.SegmentControlStyle switch
                {
                    SegmentedControlStyle.Rectangular => 0f,
                    SegmentedControlStyle.Pill => info.Height * .5f,
                    _ => CornerRadius * _scale,
                };

            var foregroundColor = ControlForegroundColor.ToSKColor();
            var backgroundColor = ControlBackgroundColor.ToSKColor();
            var style = this.SegmentControlStyle;

            var borderSize = (float)this.BorderSize * _scale;
            var halfBorderSize = borderSize * .5f;

            var segments = Segments.ToList();

            var segmentSize = info.Width / (float)(segments.Any() ? segments.Count : 1);
            var halfSegmentSize = segmentSize * .5f;

            _segmentCount = segments.Count;
            _controlRect = info.Rect;

            canvas.Clear();

            if (style != SegmentedControlStyle.Underline)
            {
                drawingClip.AddRoundRect(info.Rect, scaledCornerRadius, scaledCornerRadius);
                canvas.ClipPath(drawingClip, SKClipOperation.Intersect, true);
            }

            canvas.DrawColor(backgroundColor);

            for (int i = 0; i < Segments.Count(); i++)
            {
                var segment = segments.ElementAtOrDefault(i);

                if (segment != null)
                {
                    var selected = i == SelectedIndex;

                    var segmentForegroundColor =
                        segment.ForegroundColor != default(Color)
                            ? segment.ForegroundColor.ToSKColor()
                            : foregroundColor;

                    paint.Color = segmentForegroundColor;

                    if (selected)
                    {
                        var selectedX = segmentSize * i;

                        switch (style)
                        {
                            case SegmentedControlStyle.Filled:
                            case SegmentedControlStyle.Pill:
                                canvas
                                    .DrawRoundRect(
                                        selectedX + borderSize, borderSize, segmentSize - (borderSize * 2f), info.Height - (borderSize * 2f),
                                        scaledCornerRadius, scaledCornerRadius,
                                        paint);
                                break;
                            case SegmentedControlStyle.Underline:
                                canvas.DrawRect(selectedX, info.Height - borderSize, segmentSize, borderSize, paint);
                                break;
                            default:
                                canvas.DrawRect(selectedX, 0, segmentSize, info.Height, paint);
                                break;
                        }
                    }

                    if (segment.SVG != default(SKSvg))
                    {
                        using (new SKAutoCanvasRestore(canvas))
                        {
                            var contentRect = new SKRect(segmentSize * i, borderSize + halfBorderSize, segmentSize * (i + 1), info.Height - borderSize - halfBorderSize);

                            var imageSize = contentRect.AspectFit(segment.SVG.Picture.CullRect.Size);

                            var scaleAmount = (float)Math.Max(0, Math.Min(imageSize.Width / segment.SVG.Picture.CullRect.Width, imageSize.Height / segment.SVG.Picture.CullRect.Height));

                            var svgScale = SKMatrix.CreateScale(scaleAmount, scaleAmount);

                            var translation = SKMatrix.CreateTranslation(imageSize.Left, imageSize.Top);

                            svgScale = svgScale.PostConcat(translation);

                            using (new SKAutoCanvasRestore(canvas))
                            using (var overlayPaint = new SKPaint())
                            {
                                overlayPaint.BlendMode = SKBlendMode.SrcATop;
                                overlayPaint.IsAntialias = true;
                                overlayPaint.Color = selected ? backgroundColor : segmentForegroundColor;

                                canvas.SaveLayer(null);
                                canvas.Clear();
                                canvas.DrawPicture(segment.SVG.Picture, ref svgScale);
                                canvas.DrawPaint(overlayPaint);
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(segment.Text))
                    {
                        using (var fontPaint = new SKPaint())
                        {
                            fontPaint.Color =
                                selected
                                    ? ForegroundTextColor != default(Color) ? ForegroundTextColor.ToSKColor() : backgroundColor
                                    : BackgroundTextColor != default(Color) ? BackgroundTextColor.ToSKColor() : segmentForegroundColor;
                            fontPaint.TextSize = (float)FontSize * _scale;
                            fontPaint.Typeface = FontCache.Instance.TypefaceFromFontFamily(FontFamily);
                            fontPaint.IsAntialias = true;

                            var textMid = (segmentSize * i) + halfSegmentSize;

                            if (segment.IsIconifiedText)
                            {
                                var textBounds = SKRect.Empty;
                                textBounds = fontPaint.MeasureIconifiedText(segment.Text);

                                var maxSegmentSize = segmentSize - (borderSize * 4f);

                                while (textBounds.Width > 0 && fontPaint.TextSize > 0 && textBounds.Width > maxSegmentSize)
                                {
                                    fontPaint.TextSize--;
                                    textBounds = fontPaint.MeasureIconifiedText(segment.Text);
                                }

                                canvas.DrawCenteredIconifiedText(segment.Text, textMid, info.Rect.MidY, fontPaint);
                            }
                            else
                            {
                                fontPaint.EnsureHasValidFont(segment.Text);

                                var textBounds = SKRect.Empty;
                                fontPaint.MeasureText(segment.Text, ref textBounds);

                                while (textBounds.Width > 0 && fontPaint.TextSize > 0 && textBounds.Width > segmentSize - borderSize)
                                {
                                    fontPaint.TextSize--;
                                    fontPaint.MeasureText(segment.Text, ref textBounds);
                                }

                                canvas.DrawCenteredText(segment.Text, textMid, info.Rect.MidY, fontPaint);
                            }
                        }
                    }
                }
            }

            if (style == SegmentedControlStyle.Cupertino)
            {
                paint.Color = foregroundColor;

                for (int i = 0; i < segments.Count; i++)
                {
                    var x = (segmentSize * (i + 1)) - halfBorderSize;
                    canvas.DrawRect(x, borderSize, borderSize, info.Height - (borderSize * 2f), paint);
                }
            }

            if (style != SegmentedControlStyle.Underline)
            {
                paint.Color = foregroundColor;

                using (var outerPath = new SKPath())
                using (var innerPath = new SKPath())
                {
                    outerPath.AddRect(info.Rect);
                    innerPath
                        .AddRoundRect(
                            new SKRect(borderSize, borderSize, info.Width - borderSize, info.Height - borderSize),
                            scaledCornerRadius, scaledCornerRadius);
                    using (var finalPath = outerPath.Op(innerPath, SKPathOp.Difference))
                    {
                        canvas.DrawPath(finalPath, paint);
                    }
                }
            }
        }
    }

    protected override void OnTouch(SKTouchEventArgs e)
    {
        if (_controlRect == SKRect.Empty || _segmentCount == 0)
        {
            return;
        }

        var segmentSize = _controlRect.Width / (float)_segmentCount;

        e.Handled = true;

        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                _previousIndex = SelectedIndex;

                var newIndex = (int)Math.Floor(e.Location.X / segmentSize);

                if (_previousIndex != newIndex && !_segments.ElementAt(newIndex).IsSpacer)
                {
                    SelectedIndex = newIndex;
                }

                break;
            default:
                break;
        }
    }

    private void SegmentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems != null)
        {
            foreach (INotifyPropertyChanged item in args.OldItems)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        if (args.NewItems != null)
        {
            foreach (INotifyPropertyChanged item in args.NewItems)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        this.InvalidateSurface();
    }

    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        this.InvalidateSurface();
    }
}

public enum SegmentedControlStyle
{
    Cupertino,
    Filled,
    Rectangular,
    Underline,
    Pill,
}

public class Segment : BindableObject, IDisposable
{
    private SKSvg _svg;

    private bool _disposedValue;

    public static BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(Segment), default(string));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static BindableProperty IsIconifiedTextProperty =
        BindableProperty.Create(nameof(IsIconifiedText), typeof(bool), typeof(Segment), default(bool));

    public bool IsIconifiedText
    {
        get => (bool)GetValue(IsIconifiedTextProperty);
        set => SetValue(IsIconifiedTextProperty, value);
    }

    public static BindableProperty EmbeddedImageNameProperty =
        BindableProperty.Create(nameof(EmbeddedImageName), typeof(string), typeof(Segment), string.Empty,
            propertyChanged:
            (bindable, oldValue, newValue) =>
            {
                if (bindable is Segment s)
                {
                    s.SetSvgResource();
                }
            });

    public string EmbeddedImageName
    {
        get => (string)GetValue(EmbeddedImageNameProperty);
        set => SetValue(EmbeddedImageNameProperty, value);
    }

    public SKSvg SVG => _svg;

    public static BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(object), typeof(Segment), default(object));

    public object Value
    {
        get => (object)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static BindableProperty ForegroundColorProperty =
        BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(Segment), default(Color));

    public Color ForegroundColor
    {
        get => (Color)GetValue(ForegroundColorProperty);
        set => SetValue(ForegroundColorProperty, value);
    }

    public static BindableProperty IsSpacerProperty =
        BindableProperty.Create(nameof(IsSpacer), typeof(bool), typeof(Segment), default(bool));

    public bool IsSpacer
    {
        get => (bool)GetValue(IsSpacerProperty);
        set => SetValue(IsSpacerProperty, value);
    }

    private void SetSvgResource()
    {
        if (string.IsNullOrEmpty(EmbeddedImageName))
        {
            _svg = new SKSvg();
            return;
        }

        _svg = new SKSvg();

        using (var imageStream = EmbeddedResourceLoader.Load(EmbeddedImageName))
        {
            _svg.Load(imageStream);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _svg?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}