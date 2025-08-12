namespace AuroraControls;

/// <summary>
/// An overlay view that offers a definable cutout region to highlight or frame controls, images and views.
/// </summary>
#pragma warning disable CA1001
public class CutoutOverlayView : AuroraViewBase
#pragma warning restore CA1001
{
    private SKPath? _overlayPath = new SKPath();

    /// <summary>
    /// The cutout shape property, sets the shape for the cutout.
    /// </summary>
    public static BindableProperty CutoutShapeProperty =
        BindableProperty.Create(nameof(CutoutShape), typeof(CutoutOverlayShape), typeof(CutoutOverlayView), CutoutOverlayShape.Circular,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the cutout shape.
    /// </summary>
    /// <value>Expects Aurora.Controls.CutoutOverlayShape. Default is CutoutOverlayShape.Circular.</value>
    public CutoutOverlayShape CutoutShape
    {
        get { return (CutoutOverlayShape)GetValue(CutoutShapeProperty); }
        set { SetValue(CutoutShapeProperty, value); }
    }

    /// <summary>
    /// Defines the color of the overlay.
    /// </summary>
    public static BindableProperty CutoutOverlayColorProperty =
        BindableProperty.Create(nameof(CutoutOverlayColor), typeof(Color), typeof(CutoutOverlayView), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the cutout overlay.
    /// </summary>
    /// <value>Expects a Conlor. Default is Color.White.</value>
    public Color CutoutOverlayColor
    {
        get { return (Color)GetValue(CutoutOverlayColorProperty); }
        set { SetValue(CutoutOverlayColorProperty, value); }
    }

    /// <summary>
    /// Specifies the desired width of the border. Default width is 0.
    /// </summary>
    public static BindableProperty BorderWidthProperty =
        BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(CutoutOverlayView), 0d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the width of the cutout area's border.
    /// </summary>
    /// <value>The width of the border as a double. Default is 0d.</value>
    public double BorderWidth
    {
        get { return (double)GetValue(BorderWidthProperty); }
        set { SetValue(BorderWidthProperty, value); }
    }

    /// <summary>
    /// Specifies the desired color of the cutout area's border.
    /// </summary>
    public static BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(CutoutOverlayView), Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the border.
    /// </summary>
    /// <value>Expects a Color. Default is Color.Default.</value>
    public Color BorderColor
    {
        get { return (Color)GetValue(BorderColorProperty); }
        set { SetValue(BorderColorProperty, value); }
    }

    /// <summary>
    /// Defines the inset margin between the parent view and the beginning of the cutout area.
    /// </summary>
    public static BindableProperty CutoutInsetProperty =
        BindableProperty.Create(nameof(CutoutInset), typeof(Thickness), typeof(CutoutOverlayView), default(Thickness),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the cutout inset.
    /// </summary>
    /// <value>Expects a Thickness. Default is Thickness.Default.</value>
    public Thickness CutoutInset
    {
        get { return (Thickness)GetValue(CutoutInsetProperty); }
        set { SetValue(CutoutInsetProperty, value); }
    }

    /// <summary>
    /// The corner radius property for the cutout area.
    /// </summary>
    public static BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(CutoutOverlayView), 2d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    /// <value>Radius as a double. Default is 2d.</value>
    public double CornerRadius
    {
        get { return (double)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    /// <summary>
    /// The command property that fires when the view is tapped. Fires upon release.
    /// </summary>
    public static BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(CutoutOverlayView));

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>System.Windows.Input.</value>
    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    /// <summary>
    /// The command parameter property.
    /// </summary>
    public static BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(CutoutOverlayView));

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    public object CommandParameter
    {
        get { return (object)GetValue(CommandParameterProperty); }
        set { SetValue(CommandParameterProperty, value); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CutoutOverlayView"/> class.
    /// </summary>
    public CutoutOverlayView()
    {
    }

    protected override void Attached()
    {
        this.EnableTouchEvents = true;
        _overlayPath = new SKPath();
        base.Attached();
    }

    protected override void Detached()
    {
        _overlayPath?.Dispose();
        base.Detached();
    }

    /// <summary>
    /// Method that is called when the property that is specified by propertyName is changed.
    /// The surface is automatically invalidated/redrawn whenever <c>HeightProperty</c>, <c>WidthProperty</c> or <c>MarginProperty</c> gets updated.
    /// </summary>
    /// <param name="propertyName">The name of the bound property that changed.</param>
    protected override void OnPropertyChanged(string? propertyName = null)
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

    /// <summary>
    /// This is the method used to draw our control on the SKCanvas. This method is fired every time <c>this.InvalidateSurface();</c> is called, resulting in a "redrawing" of the control.
    /// </summary>
    /// <param name="surface">The skia surface to paint on the controls.</param>
    /// <param name="info">Information about the skia image.</param>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        if (_overlayPath is null)
        {
            return;
        }

        using var overlayPaint = new SKPaint();

        _overlayPath.Reset();

        overlayPaint.IsAntialias = true;
        overlayPaint.Style = SKPaintStyle.Fill;
        overlayPaint.Color = this.CutoutOverlayColor.ToSKColor();

        var scaledCornerRadius = (float)this.CornerRadius * _scale;

        switch (this.CutoutShape)
        {
            case CutoutOverlayShape.Circular:
                double size = Math.Min(info.Width - this.CutoutInset.Left - this.CutoutInset.Right, info.Height - this.CutoutInset.Top - this.CutoutInset.Bottom);

                float left = (float)((info.Width - size) / 2f);
                float top = (float)((info.Height - size) / 2f);
                float right = (float)(left + size);
                float bottom = (float)(top + size);

                var arcRect = new SKRect(left, top, right, bottom);
                _overlayPath.AddOval(arcRect);
                break;
            case CutoutOverlayShape.Oval:
                var ovalRect = new SKRect(
                    (float)this.CutoutInset.Left * _scale, (float)this.CutoutInset.Top * _scale,
                    info.Width - ((float)this.CutoutInset.Right * _scale), info.Height - ((float)this.CutoutInset.Bottom * _scale));
                _overlayPath.AddOval(ovalRect);
                break;
            case CutoutOverlayShape.Square:
                double sqSize = Math.Min(info.Width - this.CutoutInset.Left - this.CutoutInset.Right, info.Height - this.CutoutInset.Top - this.CutoutInset.Bottom);

                float sqLeft = (float)((info.Width - sqSize) / 2f);
                float sqTop = (float)((info.Height - sqSize) / 2f);
                float sqRight = (float)(sqLeft + sqSize);
                float sqBottom = (float)(sqTop + sqSize);

                var sqRect = new SKRect(sqLeft, sqTop, sqRight, sqBottom);
                _overlayPath.AddRoundRect(sqRect, scaledCornerRadius, scaledCornerRadius);
                break;
            case CutoutOverlayShape.Rectangular:
                var rectRect = new SKRect(
                    (float)this.CutoutInset.Left * _scale, (float)this.CutoutInset.Top * _scale,
                    info.Width - ((float)this.CutoutInset.Right * _scale), info.Height - ((float)this.CutoutInset.Bottom * _scale));
                _overlayPath.AddRoundRect(rectRect, scaledCornerRadius, scaledCornerRadius);
                break;
        }

        canvas.Clear();
        using (new SKAutoCanvasRestore(canvas))
        {
            canvas.ClipPath(_overlayPath, SKClipOperation.Difference, true);
            canvas.DrawPaint(overlayPaint);
        }

        if (this.BorderWidth > 0d && this.BorderColor is not null && !Equals(this.BorderColor, Colors.Transparent))
        {
            using var borderPaint = new SKPaint();
            borderPaint.IsAntialias = true;
            borderPaint.Style = SKPaintStyle.Stroke;
            borderPaint.Color = this.BorderColor.ToSKColor();
            borderPaint.StrokeWidth = (float)this.BorderWidth * _scale;

            canvas.DrawPath(_overlayPath, borderPaint);
        }
    }

    /// <summary>
    /// SKCanvas method that fires on touch.
    /// </summary>
    /// <param name="e">Provides data for the SKCanvasView.Touch or SKGLView.Touch event.</param>
    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        bool isTapInside = _overlayPath?.Contains(e.Location.X, e.Location.Y) ?? false;

        if (e.ActionType != SKTouchAction.Released || !isTapInside)
        {
            return;
        }

        if (this.Command?.CanExecute(this.CommandParameter) ?? false)
        {
            this.Command.Execute(this.CommandParameter);
        }
    }
}
