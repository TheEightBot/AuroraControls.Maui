﻿namespace AuroraControls.Gauges;

/// <summary>
/// Circular fill gauge.
/// </summary>
public class CircularFillGauge : AuroraViewBase
{
    /// <summary>
    /// The progress percentage property.
    /// </summary>
    public static readonly BindableProperty ProgressPercentageProperty =
        BindableProperty.Create(nameof(ProgressPercentage), typeof(double), typeof(CircularFillGauge), default(double),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the progress percentage.
    /// </summary>
    /// <value>The progress percentage as a double. Default is default(double).</value>
    public double ProgressPercentage
    {
        get => (double)GetValue(ProgressPercentageProperty);
        set => SetValue(ProgressPercentageProperty, value.Clamp(0, 100));
    }

    /// <summary>
    /// The progress color property.
    /// </summary>
    public static readonly BindableProperty ProgressColorProperty =
        BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(CircularFillGauge), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the progress.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Color.Default.</value>
    public Color ProgressColor
    {
        get => (Color)GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }

    /// <summary>
    /// The progress background color property.
    /// </summary>
    public static readonly BindableProperty ProgressBackgroundColorProperty =
        BindableProperty.Create(nameof(ProgressBackgroundColor), typeof(Color), typeof(CircularFillGauge), Colors.Gray,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the progress background.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Color.Default.</value>
    public Color ProgressBackgroundColor
    {
        get => (Color)GetValue(ProgressBackgroundColorProperty);
        set => SetValue(ProgressBackgroundColorProperty, value);
    }

    public CircularFillGauge() => MinimumHeightRequest = IAuroraView.StandardControlHeight;

    /// <summary>
    /// Method that is called when the property that is specified by propertyName is changed.
    /// The surface is automatically invalidated/redrawn whenever <c>HeightProperty</c>, <c>WidthProperty</c> or <c>MarginProperty</c> gets updated.
    /// </summary>
    /// <param name="propertyName">The name of the bound property that changed.</param>
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName.Equals(HeightProperty.PropertyName) ||
           propertyName.Equals(WidthProperty.PropertyName) ||
           propertyName.Equals(MarginProperty.PropertyName))
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

        using var progressPaint = new SKPaint();
        using var progressPath = new SKPath();
        using var progressBackgroundPaint = new SKPaint();
        using var backgroundProgressPath = new SKPath();
        progressPaint.IsAntialias = true;
        progressPaint.Style = SKPaintStyle.Fill;
        progressPaint.Color = this.ProgressColor.ToSKColor();

        progressBackgroundPaint.IsAntialias = true;
        progressBackgroundPaint.Style = SKPaintStyle.Fill;
        progressBackgroundPaint.Color = this.ProgressBackgroundColor.ToSKColor();

        int size = Math.Min(info.Width, info.Height);

        float left = (info.Width - size) / 2f;
        float top = (info.Height - size) / 2f;
        float right = left + size;
        float bottom = top + size;

        var arcRect = new SKRect(left, top, right, bottom);

        backgroundProgressPath.AddOval(arcRect);

        float halfWidth = arcRect.Width * .5f;
        float halfHeight = arcRect.Height * .5f;
        var newRect = SKRect.Inflate(arcRect, -halfWidth + (((float)this.ProgressPercentage / 100f) * halfWidth), -halfHeight + (((float)this.ProgressPercentage / 100f) * halfHeight));

        progressPath.AddOval(newRect);

        canvas.Clear();
        canvas.DrawPath(backgroundProgressPath, progressBackgroundPaint);

        if (this.ProgressPercentage > 0)
        {
            canvas.DrawPath(progressPath, progressPaint);
        }
    }

    /// <summary>
    /// Transitions the progress percentage.
    /// </summary>
    /// <returns>A Task boolean when the Task is complete.</returns>
    /// <param name="progressPercentage">Progress percentage.</param>
    /// <param name="rate">The time, in milliseconds, between frames.</param>
    /// <param name="length">The number of milliseconds over which to interpolate the animation.</param>
    /// <param name="easing">The easing function to use to transision in, out, or in and out of the animation.</param>
    public Task<bool> TransitionTo(double progressPercentage, uint rate = 16, uint length = 250, Easing easing = null) => this.TransitionTo(x => x.ProgressPercentage, progressPercentage, rate, length, easing);
}
