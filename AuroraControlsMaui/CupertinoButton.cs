namespace AuroraControls;

#pragma warning disable CA1001
public class CupertinoButton : AuroraViewBase
#pragma warning restore CA1001
{
    /// <summary>
    /// The button background start color property.
    /// </summary>
    public static readonly BindableProperty ButtonBackgroundColorProperty =
        BindableProperty.Create(nameof(ButtonBackgroundColor), typeof(Color), typeof(CupertinoButton), Color.FromHex("#006FFF"),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// The border color property.
    /// </summary>
    public static readonly BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(CupertinoButton), Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// The shadow color property.
    /// </summary>
    public static readonly BindableProperty ShadowColorProperty =
        BindableProperty.Create(nameof(ShadowColor), typeof(Color), typeof(CupertinoButton), Color.FromRgba(0d, 0d, 0d, .33d),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// The shadow location property.
    /// </summary>
    public static readonly BindableProperty ShadowLocationProperty =
        BindableProperty.Create(nameof(ShadowLocation), typeof(Point), typeof(CupertinoButton), new Point(0, 1),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// The shadow blur radius property.
    /// </summary>
    public static readonly BindableProperty ShadowBlurRadiusProperty =
        BindableProperty.Create(nameof(ShadowBlurRadius), typeof(double), typeof(CupertinoButton), 0d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// The border width property.
    /// </summary>
    public static readonly BindableProperty BorderWidthProperty =
        BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(CupertinoButton), 0d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// The border radius property.
    /// </summary>
    public static readonly BindableProperty BorderRadiusProperty =
        BindableProperty.Create(nameof(BorderRadius), typeof(double), typeof(CupertinoButton), 4d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// The text property.
    /// </summary>
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(CupertinoButton),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// The font color property.
    /// </summary>
    public static readonly BindableProperty FontColorProperty =
        BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(CupertinoButton), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(CupertinoButton), PlatformInfo.DefaultButtonFontSize,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// The typeface property.
    /// </summary>
    public static readonly BindableProperty TypefaceProperty =
        BindableProperty.Create(nameof(Typeface), typeof(SKTypeface), typeof(CupertinoButton),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public static readonly BindableProperty IsIconifiedTextProperty =
        BindableProperty.Create(nameof(IsIconifiedText), typeof(bool), typeof(CupertinoButton), false,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// The command property. Fires on tap.
    /// </summary>
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(CupertinoButton));

    /// <summary>
    /// The command parameter property.
    /// </summary>
    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(CupertinoButton));

    private SKPath _backgroundPath = new SKPath();

    private bool _tapped;

    /// <summary>
    /// Initializes a new instance of the <see cref="CupertinoButton"/> class.
    /// </summary>
    public CupertinoButton()
    {
    }

    /// <summary>
    /// Gets or sets the start color of the button background.
    /// </summary>
    /// <value>Expects a Color. Default value is Colors.Transparent.</value>
    public Color ButtonBackgroundColor
    {
        get => (Color)GetValue(ButtonBackgroundColorProperty);
        set => SetValue(ButtonBackgroundColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the border.
    /// </summary>
    /// <value>Expects a Color. Default value is Colors.Transparent.</value>
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the shadow.
    /// </summary>
    /// <value>Expects a Color. Default value is Color.FromRgba(0d, 0d, 0d, .33d).</value>
    public Color ShadowColor
    {
        get => (Color)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the shadow location.
    /// </summary>
    /// <value>Takes a point with x and y offsets. Default value is new Point(0, 1).</value>
    public Point ShadowLocation
    {
        get => (Point)GetValue(ShadowLocationProperty);
        set => SetValue(ShadowLocationProperty, value);
    }

    /// <summary>
    /// Gets or sets the shadow blur radius.
    /// </summary>
    /// <value>The shadow blur radius. Default value is default(double).</value>
    public double ShadowBlurRadius
    {
        get => (double)GetValue(ShadowBlurRadiusProperty);
        set => SetValue(ShadowBlurRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the border.
    /// </summary>
    /// <value>Expects a double value. Default is 0d.</value>
    public double BorderWidth
    {
        get => (double)GetValue(BorderWidthProperty);
        set => SetValue(BorderWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the radius of the border.
    /// </summary>
    /// <value>Expects a double value. Default is 4d.</value>
    public double BorderRadius
    {
        get => (double)GetValue(BorderRadiusProperty);
        set => SetValue(BorderRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the text for the button.
    /// </summary>
    /// <value>string value for text. Default is default(string).</value>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the font.
    /// </summary>
    /// <value>Expects a Color. Default value is Color.White.</value>
    public Color FontColor
    {
        get => (Color)GetValue(FontColorProperty);
        set => SetValue(FontColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the typeface for the button.
    /// </summary>
    /// <value>Expects a SKTypeface. Default default(SKTypeface).</value>
    public SKTypeface Typeface
    {
        get => (SKTypeface)GetValue(TypefaceProperty);
        set => SetValue(TypefaceProperty, value);
    }

    public bool IsIconifiedText
    {
        get => (bool)GetValue(IsIconifiedTextProperty);
        set => SetValue(IsIconifiedTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>The command.</value>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    /// <value>The command parameter. default value is default(object).</value>
    public object CommandParameter
    {
        get => (object)GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Aurora.Controls.CupertinoButton"/> is tapped.
    /// </summary>
    /// <value><c>true</c> if tapped; otherwise, <c>false</c>.</value>
    public bool Tapped
    {
        get => _tapped;

        set
        {
            if (_tapped == value)
            {
                return;
            }

            _tapped = value;
            this.InvalidateSurface();
        }
    }

    public event EventHandler Clicked;

    protected override void Attached()
    {
        this.EnableTouchEvents = true;
        _backgroundPath = new SKPath();
        base.Attached();
    }

    protected override void Detached()
    {
        _backgroundPath?.Dispose();
        base.Detached();
    }

    /// <summary>
    /// Method that is called when the property that is specified by propertyName is changed.
    /// The surface is automatically invalidated/redrawn whenever <c>HeightProperty</c>, <c>WidthProperty</c> or <c>MarginProperty</c> gets updated.
    /// </summary>
    /// <param name="propertyName">The name of the bound property that changed.</param>
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

    /// <summary>
    /// This is the method used to draw our control on the SKCanvas. This method is fired every time <c>this.InvalidateSurface();</c> is called, resulting in a "redrawing" of the control.
    /// </summary>
    /// <param name="surface">The skia surface to paint on the controls.</param>
    /// <param name="info">Information about the skia image.</param>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        using (var backgroundPaint = new SKPaint())
        {
            var scale = info.Height / (float)Height;
            var rect = new SKRect((float)ShadowLocation.X + (float)ShadowBlurRadius, (float)ShadowLocation.Y + (float)ShadowBlurRadius,
                info.Width - (float)ShadowLocation.X - (float)ShadowBlurRadius, info.Height - (float)ShadowLocation.Y - (float)ShadowBlurRadius);

            backgroundPaint.Color = this.ButtonBackgroundColor.ToSKColor();
            backgroundPaint.IsAntialias = true;
            backgroundPaint.Style = SKPaintStyle.Fill;

            var borderRadius = (float)this.BorderRadius * scale;

            canvas.Clear();
            _backgroundPath.Reset();

            if (ShadowColor != Colors.Transparent && ShadowLocation != Point.Zero)
            {
                using (var shadowPaint = new SKPaint())
                using (new SKAutoCanvasRestore(canvas))
                {
                    shadowPaint.IsAntialias = true;
                    shadowPaint.Color = ShadowColor.ToSKColor();
                    shadowPaint.Style = SKPaintStyle.Fill;
                    shadowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, SKMaskFilter.ConvertRadiusToSigma((float)ShadowBlurRadius));

                    canvas.Translate(ShadowLocation.ToSKPoint());
                    canvas.DrawRoundRect(rect, borderRadius, borderRadius, shadowPaint);
                }
            }

            using (new SKAutoCanvasRestore(canvas))
            {
                if (Tapped)
                {
                    canvas.Translate(ShadowLocation.ToSKPoint());
                }

                _backgroundPath.AddRoundRect(rect, borderRadius, borderRadius);
                canvas.DrawPath(_backgroundPath, backgroundPaint);

                if (BorderWidth > 0d && BorderColor != Colors.Transparent)
                {
                    backgroundPaint.StrokeWidth = (float)BorderWidth;
                    backgroundPaint.Color = BorderColor.ToSKColor();
                    backgroundPaint.Shader = null;
                    backgroundPaint.Style = SKPaintStyle.Stroke;

                    canvas.DrawPath(_backgroundPath, backgroundPaint);
                }

                if (!string.IsNullOrEmpty(Text))
                {
                    using (var fontPaint = new SKPaint())
                    {
                        fontPaint.Color = FontColor.ToSKColor();
                        fontPaint.TextSize = (float)FontSize * scale;
                        fontPaint.IsAntialias = true;
                        fontPaint.Typeface = Typeface ?? PlatformInfo.DefaultTypeface;

                        if (IsIconifiedText)
                        {
                            canvas.DrawCenteredIconifiedText(Text, rect.MidX, rect.MidY, fontPaint);
                        }
                        else
                        {
                            canvas.DrawCenteredText(Text, rect.MidX, rect.MidY, fontPaint);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// SKCanvas method that fires on touch.
    /// </summary>
    /// <param name="e">Provides data for the SKCanvasView.Touch or SKGLView.Touch event.</param>
    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        if (e.ActionType == SKTouchAction.Cancelled || e.ActionType == SKTouchAction.Exited)
        {
            Tapped = false;
            return;
        }

        var isTapInside = _backgroundPath.Contains(e.Location.X, e.Location.Y);

        if (e.ActionType == SKTouchAction.Released && isTapInside)
        {
            Tapped = false;

            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }

            Clicked?.Invoke(this, EventArgs.Empty);

            return;
        }

        Tapped = e.InContact && isTapInside;
    }
}
