namespace AuroraControls.VisualEffects;

public class Skew : VisualEffect
{
    /// <summary>
    /// The skew degrees X property.
    /// </summary>
    public static readonly BindableProperty SkewDegreesXProperty =
        BindableProperty.Create(nameof(SkewDegreesX), typeof(double), typeof(Skew), default(double));

    /// <summary>
    /// Gets or sets the X-axis skew.
    /// </summary>
    /// <value>Degrees as a double. Default is default(double).</value>
    public double SkewDegreesX
    {
        get => (double)GetValue(SkewDegreesXProperty);
        set => SetValue(SkewDegreesXProperty, value);
    }

    /// <summary>
    /// The skew degrees Y property.
    /// </summary>
    public static readonly BindableProperty SkewDegreesYProperty =
        BindableProperty.Create(nameof(SkewDegreesY), typeof(double), typeof(Skew), default(double));

    /// <summary>
    /// Gets or sets the Y-axis skew.
    /// </summary>
    /// <value>Degrees as a double. Default is default(double).</value>
    public double SkewDegreesY
    {
        get => (double)GetValue(SkewDegreesYProperty);
        set => SetValue(SkewDegreesYProperty, value);
    }

    /// <summary>
    /// The location of the skew as a percentage of the canvas
    /// Values are valid between 0..1.
    /// </summary>
    public static readonly BindableProperty LocationProperty =
        BindableProperty.Create(nameof(Location), typeof(Point), typeof(Skew), new Point(.5d, .5d));

    /// <summary>
    /// Gets or sets the location of the skew.
    /// </summary>
    /// <value>Takes a Point. Default is Point(.5d, .5d).</value>
    public Point Location
    {
        get => (Point)GetValue(LocationProperty);
        set => SetValue(LocationProperty, value);
    }

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, SKImageInfo info, SKRect overrideRect) => InternalApplyEffect(surface, info.Rect);

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, GRBackendRenderTarget info, SKRect overrideRect) => InternalApplyEffect(surface, info.Rect);

    private SKImage InternalApplyEffect(SKSurface surface, SKRect rect)
    {
        var canvas = surface.Canvas;

        using (var surfaceSnapshot = surface.Snapshot())
        {
            canvas.Clear();

            var skewLocation = this.Location;

            var locationX = (float)skewLocation.X * rect.Width;
            var locationY = (float)skewLocation.Y * rect.Height;

            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.Translate(locationX, locationY);
                SkewDegrees(canvas, this.SkewDegreesX, this.SkewDegreesY);
                canvas.Translate(-locationX, -locationY);

                canvas.DrawImage(surfaceSnapshot, rect);
            }
        }

        return surface.Snapshot();
    }

    private void SkewDegrees(SKCanvas canvas, double xDegrees, double yDegrees) =>
        canvas.Skew(
            (float)Math.Tan(Math.PI * xDegrees / 180f),
            (float)Math.Tan(Math.PI * yDegrees / 180f));
}
