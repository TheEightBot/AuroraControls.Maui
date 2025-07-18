﻿namespace AuroraControls.VisualEffects;

public class Gradient : VisualEffect
{
    /// <summary>
    /// The gradient rotation angle property.
    /// </summary>
    public static readonly BindableProperty GradientRotationAngleProperty =
        BindableProperty.Create(nameof(GradientRotationAngle), typeof(double), typeof(Gradient), 0d);

    /// <summary>
    /// Gets or sets the rotation angle of the gradient.
    /// </summary>
    /// <value>Rotation angle as a double. Default is 0d.</value>
    public double GradientRotationAngle
    {
        get => (double)GetValue(GradientRotationAngleProperty);
        set => SetValue(GradientRotationAngleProperty, value.Clamp(-360, 360));
    }

    /// <summary>
    /// The gradient start color property.
    /// </summary>
    public static readonly BindableProperty GradientStartColorProperty =
        BindableProperty.Create(nameof(GradientStartColor), typeof(Color), typeof(Gradient));

    /// <summary>
    /// Gets or sets the starting color.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default is Xamarin.Forms.Color.Default.</value>
    public Color GradientStartColor
    {
        get => (Color)GetValue(GradientStartColorProperty);
        set => SetValue(GradientStartColorProperty, value);
    }

    /// <summary>
    /// The gradient stop color property.
    /// </summary>
    public static readonly BindableProperty GradientStopColorProperty =
        BindableProperty.Create(nameof(GradientStopColor), typeof(Color), typeof(Gradient));

    /// <summary>
    /// Gets or sets the stop color.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default is Xamarin.Forms.Color.Default.</value>
    public Color GradientStopColor
    {
        get => (Color)GetValue(GradientStopColorProperty);
        set => SetValue(GradientStopColorProperty, value);
    }

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, SKImageInfo info, SKRect overrideRect) => ApplyEffectInternal(surface, info.Rect);

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, GRBackendRenderTarget info, SKRect overrideRect) => ApplyEffectInternal(surface, info.Rect);

    private SKImage ApplyEffectInternal(SKSurface surface, SKRect rect)
    {
        var canvas = surface.Canvas;

        using (var overlayPaint = new SKPaint())
        using (
            var shader =
                SKShader
                    .CreateLinearGradient(
                        new SKPoint(0, 0), new SKPoint(rect.Width, 0),
                        [GradientStartColor.ToSKColor(), GradientStopColor.ToSKColor(),],
                        [0, 1,],
                        SKShaderTileMode.Clamp))
        {
            overlayPaint.BlendMode = SKBlendMode.SrcATop;
            overlayPaint.Shader = shader;
            overlayPaint.IsAntialias = true;

            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.RotateDegrees((float)GradientRotationAngle, rect.Width / 2f, rect.Height / 2f);

                canvas.DrawPaint(overlayPaint);
            }
        }

        return surface.Snapshot();
    }
}
