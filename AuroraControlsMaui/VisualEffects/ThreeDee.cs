namespace AuroraControls.VisualEffects;

public class ThreeDee : VisualEffect
{
    /// <summary>
    /// The x-axis rotation property.
    /// </summary>
    public static readonly BindableProperty RotationXDegreesProperty =
        BindableProperty.Create(nameof(RotationXDegrees), typeof(double), typeof(ThreeDee), default(double));

    /// <summary>
    /// Gets or sets x-axis rotation degree.
    /// </summary>
    /// <value>Degress as a double. Default is default(double).</value>
    public double RotationXDegrees
    {
        get => (double)GetValue(RotationXDegreesProperty);
        set => SetValue(RotationXDegreesProperty, value.Clamp(-360, 360));
    }

    /// <summary>
    /// The y-axis rotation property.
    /// </summary>
    public static readonly BindableProperty RotationYDegreesProperty =
        BindableProperty.Create(nameof(RotationYDegrees), typeof(double), typeof(ThreeDee), default(double));

    /// <summary>
    /// Gets or sets y-axis rotation degree.
    /// </summary>
    /// <value>Degress as a double. Default is default(double).</value>
    public double RotationYDegrees
    {
        get => (double)GetValue(RotationYDegreesProperty);
        set => SetValue(RotationYDegreesProperty, value.Clamp(-360, 360));
    }

    /// <summary>
    /// The z-axis rotation property.
    /// </summary>
    public static readonly BindableProperty RotationZDegreesProperty =
        BindableProperty.Create(nameof(RotationZDegrees), typeof(double), typeof(ThreeDee), default(double));

    /// <summary>
    /// Gets or sets z-axis rotation degree.
    /// </summary>
    /// <value>Degress as a double. Default is default(double).</value>
    public double RotationZDegrees
    {
        get => (double)GetValue(RotationZDegreesProperty);
        set => SetValue(RotationZDegreesProperty, value.Clamp(-360, 360));
    }

    /// <summary>
    /// The location of the skew as a percentage of the canvas
    /// Values are valid between 0..1.
    /// </summary>
    public static readonly BindableProperty LocationProperty =
        BindableProperty.Create(nameof(Location), typeof(Point), typeof(ThreeDee), new Point(.5d, .5d));

    /// <summary>
    /// Gets or sets the location point.
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

            using (var rotationView = new SK3dView())
            using (new SKAutoCanvasRestore(canvas))
            {
                var skewLocation = this.Location;

                var locationX = (float)skewLocation.X * rect.Width;
                var locationY = (float)skewLocation.Y * rect.Height;

                var matrix = SKMatrix.CreateTranslation(-locationX, -locationY);

                rotationView.RotateXDegrees((float)RotationXDegrees);
                rotationView.RotateYDegrees((float)RotationYDegrees);
                rotationView.RotateZDegrees((float)RotationZDegrees);

                matrix =
                    matrix
                        .PostConcat(rotationView.Matrix)
                        .PostConcat(SKMatrix.CreateTranslation(locationX, locationY));

                canvas.SetMatrix(matrix);
                canvas.DrawImage(surfaceSnapshot, locationX - (surfaceSnapshot.Width / 2f), locationY - (surfaceSnapshot.Height / 2f));
            }
        }

        return surface.Snapshot();
    }
}
