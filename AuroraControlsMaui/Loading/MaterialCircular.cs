using System;

namespace AuroraControls.Loading;

/// <summary>
/// Material circular loading animation.
/// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class MaterialCircular : SceneViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private SKPaint _progressPaint;
    private SKPath _progressPath;
    private SKPaint _progressBackgroundPaint;
    private SKPath _backgroundProgressPath;

    /// <summary>
    /// The foreground color property. Specifies the foreground color.
    /// </summary>
    public static BindableProperty ForegroundLoadingColorProperty =
        BindableProperty.Create(nameof(ForegroundLoadingColor), typeof(Color), typeof(MaterialCircular), Colors.LightBlue,
            propertyChanged:
                static (bindable, _, newValue) =>
                {
                    if (bindable is MaterialCircular mc && mc._progressPaint != null)
                    {
                        var value = (Color)newValue;
                        mc._progressPaint.Color = value.ToSKColor();
                    }
                });

    /// <summary>
    /// Gets or sets the color of the foreground.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is default(Xamarin.Forms.Color).</value>
    public Color ForegroundLoadingColor
    {
        get { return (Color)GetValue(ForegroundLoadingColorProperty); }
        set { SetValue(ForegroundLoadingColorProperty, value); }
    }

    /// <summary>
    /// The background color property.
    /// </summary>
    public static BindableProperty BackgroundLoadingColorProperty =
        BindableProperty.Create(nameof(BackgroundLoadingColor), typeof(Color), typeof(MaterialCircular), Colors.White,
            propertyChanged:
                static (bindable, _, newValue) =>
                {
                    if (bindable is MaterialCircular mc && mc._progressBackgroundPaint != null)
                    {
                        var value = (Color)newValue;
                        mc._progressBackgroundPaint.Color = value.ToSKColor();
                    }
                });

    /// <summary>
    /// Gets or sets the color of the background.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is default(Xamarin.Forms.Color).</value>
    public Color BackgroundLoadingColor
    {
        get { return (Color)GetValue(BackgroundLoadingColorProperty); }
        set { SetValue(BackgroundLoadingColorProperty, value); }
    }

    /// <summary>
    /// The end cap type property.
    /// </summary>
    public static BindableProperty EndCapTypeProperty =
        BindableProperty.Create(nameof(EndCapType), typeof(EndCapType), typeof(MaterialCircular), EndCapType.Rounded,
            propertyChanged:
                static (bindable, _, newValue) =>
                {
                    if (bindable is MaterialCircular mc && mc._progressPaint != null)
                    {
                        switch ((EndCapType)newValue)
                        {
                            case EndCapType.Square:
                                mc._progressPaint.StrokeCap = SKStrokeCap.Butt;
                                break;
                            case EndCapType.Rounded:
                                mc._progressPaint.StrokeCap = SKStrokeCap.Round;
                                break;
                        }
                    }
                });

    /// <summary>
    /// Gets or sets the end cap type.
    /// </summary>
    /// <value>Takes an EndCapType. Default is EndCapType.Rounded.</value>
    public EndCapType EndCapType
    {
        get { return (EndCapType)GetValue(EndCapTypeProperty); }
        set { SetValue(EndCapTypeProperty, value); }
    }

    /// <summary>
    /// The progress thickness property.
    /// </summary>
    public static BindableProperty ProgressThicknessProperty =
        BindableProperty.Create(nameof(ProgressThickness), typeof(double), typeof(MaterialCircular), 6d,
            propertyChanged:
                static (bindable, _, newValue) =>
                {
                    if (bindable is MaterialCircular mc && mc._progressPaint != null && mc._progressBackgroundPaint != null)
                    {
                        var value = (float)(double)newValue;
                        mc._progressPaint.StrokeWidth = value * mc._scale;
                        mc._progressBackgroundPaint.StrokeWidth = value * mc._scale;
                    }
                });

    /// <summary>
    /// Gets or sets the progress thickness.
    /// </summary>
    /// <value>Takes a double. Default value is 12d.</value>
    public double ProgressThickness
    {
        get { return (double)GetValue(ProgressThicknessProperty); }
        set { SetValue(ProgressThicknessProperty, value); }
    }

    public MaterialCircular()
    {
        this.MinimumHeightRequest = 44;
    }

    protected override void Attached()
    {
        _progressPaint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke };

        switch (EndCapType)
        {
            case EndCapType.Square:
                _progressPaint.StrokeCap = SKStrokeCap.Butt;
                break;
            case EndCapType.Rounded:
                _progressPaint.StrokeCap = SKStrokeCap.Round;
                break;
        }

        _progressPaint.Color = ForegroundLoadingColor.ToSKColor();
        _progressPaint.StrokeWidth = (float)ProgressThickness * _scale;

        _progressPath = new SKPath { };

        _progressBackgroundPaint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke };
        _progressBackgroundPaint.Color = BackgroundLoadingColor.ToSKColor();
        _progressBackgroundPaint.StrokeWidth = (float)ProgressThickness * _scale;

        _backgroundProgressPath = new SKPath { };

        base.Attached();
    }

    protected override void Detached()
    {
        base.Detached();

        var progressPaint = _progressPaint;
        var progressPath = _progressPath;
        var progressBackgroundPaint = _progressBackgroundPaint;
        var backgroundProgressPath = _backgroundProgressPath;

        _progressPaint = null;
        _progressPath = null;
        _progressBackgroundPaint = null;
        _backgroundProgressPath = null;

        progressPaint?.Dispose();
        progressPath?.Dispose();
        progressBackgroundPaint?.Dispose();
        backgroundProgressPath?.Dispose();
    }

    protected override SKImage PaintScene(SKSurface surface, SKImageInfo info, double percentage)
    {
        var canvas = surface.Canvas;

        if (_progressPaint == null || _progressBackgroundPaint == null)
        {
            return surface.Snapshot();
        }

        var size = Math.Min(info.Width, info.Height) - _progressBackgroundPaint.StrokeWidth;

        var left = (info.Width - size) / 2f;
        var top = (info.Height - size) / 2f;
        var right = left + size;
        var bottom = top + size;

        var arcRect = new SKRect((float)left, (float)top, (float)right, (float)bottom);

        var progressOfCircle = (float)percentage * 360f;

        var progressArcLength = percentage <= .5 ? progressOfCircle : 360f - progressOfCircle;

        if (progressArcLength < 1)
        {
            progressArcLength = 1;
        }

        _backgroundProgressPath.Reset();
        _backgroundProgressPath.AddArc(arcRect, 0, 360);

        _progressPath.Reset();
        _progressPath.AddArc(arcRect, progressOfCircle, progressArcLength);

        using (new SKAutoCanvasRestore(canvas))
        {
            var matrix = SKMatrix.CreateRotationDegrees(progressOfCircle - 90f, info.Rect.MidX, info.Rect.MidY);
            canvas.SetMatrix(matrix);
            canvas.Clear();
            canvas.DrawPath(_backgroundProgressPath, _progressBackgroundPaint);
            canvas.DrawPath(_progressPath, _progressPaint);
        }

        canvas.Flush();

        return surface.Snapshot();
    }
}