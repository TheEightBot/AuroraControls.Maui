namespace AuroraControls.VisualEffects;

public class Rotate : VisualEffect
{
    /// <summary>
    /// The rotation degrees property.
    /// </summary>
    public static readonly BindableProperty RotationDegreesProperty =
        BindableProperty.Create(nameof(RotationDegrees), typeof(float), typeof(Rotate), default(float));

    /// <summary>
    /// Gets or sets the rotation degrees.
    /// </summary>
    /// <value>Degress as a float. Default is default(float).</value>
    public float RotationDegrees
    {
        get { return (float)GetValue(RotationDegreesProperty); }
        set { SetValue(RotationDegreesProperty, value.Clamp(-360, 360)); }
    }

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, SKImageInfo info, SKRect overrideRect)
    {
        return InternalApplyEffect(surface, info.Rect);
    }

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, GRBackendRenderTarget info, SKRect overrideRect)
    {
        return InternalApplyEffect(surface, info.Rect);
    }

    private SKImage InternalApplyEffect(SKSurface surface, SKRect rect)
    {
        var canvas = surface.Canvas;

        using (var surfaceSnapshot = surface.Snapshot())
        {
            canvas.Clear();

            using (new SKAutoCanvasRestore(canvas))
            {
                var matrix = SKMatrix.CreateRotationDegrees(RotationDegrees, rect.MidX, rect.MidY);

                canvas.SetMatrix(matrix);

                canvas.DrawImage(surfaceSnapshot, rect);
            }
        }

        return surface.Snapshot();
    }
}
