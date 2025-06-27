namespace AuroraControls;

public class TouchDrawLettersImage : AuroraViewBase
{
    private readonly object _lock = new object();
    private readonly ObservableUniqueCollection<TouchDrawLetter> _touchDrawLetters = new();

    private SKBitmap _imageBitmap;

    /// <summary>
    /// The source property.
    /// </summary>
    public static readonly BindableProperty SourceProperty =
        BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(TouchDrawLettersImage),
            propertyChanged: async (bindable, _, newValue) =>
            {
                if (bindable is not TouchDrawLettersImage tbi || newValue is not ImageSource source)
                {
                    return;
                }

                tbi._imageBitmap = await source.BitmapFromSource();
                tbi.InvalidateSurface();
            });

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.ImageSource. Default value is null.</value>
    public ImageSource Source
    {
        get => (ImageSource)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly BindableProperty DrawItemForegroundColorProperty =
        BindableProperty.Create(nameof(DrawItemForegroundColor), typeof(Color), typeof(TouchDrawLettersImage),
            Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public Color DrawItemForegroundColor
    {
        get => (Color)GetValue(DrawItemForegroundColorProperty);
        set => SetValue(DrawItemForegroundColorProperty, value);
    }

    public static readonly BindableProperty DrawItemBackgroundColorProperty =
        BindableProperty.Create(nameof(DrawItemBackgroundColor), typeof(Color), typeof(TouchDrawLettersImage),
            Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public Color DrawItemBackgroundColor
    {
        get => (Color)GetValue(DrawItemBackgroundColorProperty);
        set => SetValue(DrawItemBackgroundColorProperty, value);
    }

    public static readonly BindableProperty BorderSizeProperty =
        BindableProperty.Create(nameof(BorderSize), typeof(double), typeof(TouchDrawLettersImage), 2d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public double BorderSize
    {
        get => (double)GetValue(BorderSizeProperty);
        set => SetValue(BorderSizeProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(TouchDrawLettersImage),
            PlatformInfo.DefaultButtonFontSize,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty TypefaceProperty =
        BindableProperty.Create(nameof(Typeface), typeof(SKTypeface), typeof(TouchDrawLettersImage),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public SKTypeface Typeface
    {
        get => (SKTypeface)GetValue(TypefaceProperty);
        set => SetValue(TypefaceProperty, value);
    }

    /// <summary>
    /// Gets the user-selected TouchDrawLetter.
    /// </summary>h
    /// <value>A collection of <see cref="TouchDrawLetter"/>.</value>
    public IList<TouchDrawLetter> TouchDrawLetters => _touchDrawLetters;

    /// <summary>
    /// Initializes a new instance of the <see cref="TouchDrawLettersImage"/> class.
    /// </summary>
    public TouchDrawLettersImage()
    {
    }

    protected override void Attached()
    {
        this.EnableTouchEvents = true;

        _touchDrawLetters.CollectionChanged -= SelectedTouchDrawLetters_CollectionChanged;
        _touchDrawLetters.CollectionChanged += SelectedTouchDrawLetters_CollectionChanged;

        base.Attached();
    }

    protected override void Detached()
    {
        _touchDrawLetters.CollectionChanged -= SelectedTouchDrawLetters_CollectionChanged;
        base.Detached();
    }

    private void SelectedTouchDrawLetters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        lock (_lock)
        {
            InvalidateSurface();
        }
    }

    /// <summary>
    /// This is the method used to draw our control on the SKCanvas. This method is fired every time <c>this.InvalidateSurface();</c> is called, resulting in a "redrawing" of the control.
    /// </summary>
    /// <param name="surface">The skia surface to paint on the controls.</param>
    /// <param name="info">Information about the skia image.</param>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        DrawOverlay(canvas, info.Width, info.Height);
    }

    /// <summary>
    /// SKCanvas method that fires on touch.
    /// </summary>
    /// <param name="e">Provides data for the SKCanvasView.Touch or SKGLView.Touch event.</param>
    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        if (!e.InContact || (e.ActionType != SKTouchAction.Pressed && e.ActionType != SKTouchAction.Moved))
        {
            return;
        }

        if (!this._overrideDrawableArea.Contains(e.Location))
        {
            return;
        }

        float percentX = (e.Location.X - this._overrideDrawableArea.Location.X) / this._overrideDrawableArea.Width;
        float percentY = (e.Location.Y - this._overrideDrawableArea.Location.Y) / this._overrideDrawableArea.Height;

        var lastPoint = this.TouchDrawLetters.LastOrDefault();

        if (lastPoint != null)
        {
            lastPoint.Location = new Point(percentX, percentY);
            this.InvalidateSurface();
        }
    }

    private void DrawOverlay(SKCanvas canvas, int width, int height)
    {
        canvas.Clear();

        if (_imageBitmap == null)
        {
            return;
        }

        float factor = height / (float)_imageBitmap.Info.Height;

        if (_imageBitmap.Info.Width * factor > width)
        {
            factor = width / (float)_imageBitmap.Info.Width;
        }

        float scaledWidth = _imageBitmap.Info.Width * factor;
        float scaledHeight = _imageBitmap.Info.Height * factor;

        float horizontalBuffer = (width - scaledWidth) / 2f;
        float verticalBuffer = (height - scaledHeight) / 2f;

        var imageRect = new SKRect(horizontalBuffer, verticalBuffer, scaledWidth + horizontalBuffer, scaledHeight + verticalBuffer);

        _overrideDrawableArea = imageRect;

        using (new SKAutoCanvasRestore(canvas))
        {
            canvas.ClipRect(imageRect);

            canvas.DrawBitmap(_imageBitmap, imageRect);

            float scaledTouchSize = 44f * _scale;
            float halfScaledTouchSize = scaledTouchSize / 2f;

            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;

                foreach (var touchDrawLetter in TouchDrawLetters)
                {
                    if (touchDrawLetter.Location.X <= Point.Zero.X && touchDrawLetter.Location.Y <= Point.Zero.Y)
                    {
                        continue;
                    }

                    double touchLocationX = imageRect.Location.X + (imageRect.Width * touchDrawLetter.Location.X);
                    double touchLocationY = imageRect.Location.Y + (imageRect.Height * touchDrawLetter.Location.Y);

                    var drawLocationRect = new SKRect(
                        (float)touchLocationX - halfScaledTouchSize, (float)touchLocationY - halfScaledTouchSize,
                        (float)touchLocationX + halfScaledTouchSize, (float)touchLocationY + halfScaledTouchSize);

                    paint.Color = !Equals(touchDrawLetter.BackgroundColorOverride, Colors.Transparent)
                        ? touchDrawLetter.BackgroundColorOverride.ToSKColor()
                        : this.DrawItemBackgroundColor.ToSKColor();
                    paint.Style = SKPaintStyle.Fill;
                    canvas.DrawOval(drawLocationRect, paint);

                    paint.Color = !Equals(touchDrawLetter.ForegroundColorOverride, Colors.Transparent)
                        ? touchDrawLetter.ForegroundColorOverride.ToSKColor()
                        : this.DrawItemForegroundColor.ToSKColor();
                    paint.Style = SKPaintStyle.Stroke;
                    paint.StrokeWidth = (float)this.BorderSize * _scale;

                    canvas.DrawOval(drawLocationRect, paint);

                    if (!string.IsNullOrEmpty(touchDrawLetter.Value))
                    {
                        using (var drawPath = new SKPath())
                        using (new SKAutoCanvasRestore(canvas))
                        {
                            drawPath.AddOval(drawLocationRect);
                            canvas.ClipPath(drawPath);

                            paint.Color = !Equals(touchDrawLetter.ForegroundColorOverride, Colors.Transparent)
                                ? touchDrawLetter.ForegroundColorOverride.ToSKColor()
                                : this.DrawItemForegroundColor.ToSKColor();
                            paint.Style = SKPaintStyle.Fill;
                            paint.TextSize = halfScaledTouchSize;
                            paint.Typeface = Typeface ?? PlatformInfo.DefaultTypeface;

                            SKRect textBounds = SKRect.Empty;
                            paint.MeasureText(touchDrawLetter.Value, ref textBounds);

                            canvas
                                .DrawText(
                                    touchDrawLetter.Value,
                                    drawLocationRect.MidX - textBounds.MidX,
                                    drawLocationRect.MidY + (textBounds.Height / 2f),
                                    paint);
                        }
                    }
                }
            }
        }
    }
}

public class TouchDrawLetter : BindableObject
{
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(string), typeof(TouchDrawLetter), string.Empty);

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly BindableProperty LocationProperty =
        BindableProperty.Create(nameof(Location), typeof(Point), typeof(TouchDrawLetter), default(Point));

    public Point Location
    {
        get => (Point)GetValue(LocationProperty);
        set => SetValue(LocationProperty, value);
    }

    public static readonly BindableProperty ForegroundColorOverrideProperty =
        BindableProperty.Create(nameof(ForegroundColorOverride), typeof(Color), typeof(TouchDrawLetter),
            Colors.Transparent);

    public Color ForegroundColorOverride
    {
        get => (Color)GetValue(ForegroundColorOverrideProperty);
        set => SetValue(ForegroundColorOverrideProperty, value);
    }

    public static readonly BindableProperty BackgroundColorOverrideProperty =
        BindableProperty.Create(nameof(BackgroundColorOverride), typeof(Color), typeof(TouchDrawLetter),
            Colors.Transparent);

    public Color BackgroundColorOverride
    {
        get => (Color)GetValue(BackgroundColorOverrideProperty);
        set => SetValue(BackgroundColorOverrideProperty, value);
    }
}
