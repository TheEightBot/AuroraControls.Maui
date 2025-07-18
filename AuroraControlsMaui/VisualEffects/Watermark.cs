﻿namespace AuroraControls.VisualEffects;

public class Watermark : VisualEffect
{
    public enum WatermarkLocation
    {
        Start,
        Center,
        End,
    }

    public static readonly BindableProperty HorizontalWatermarkLocationProperty =
        BindableProperty.Create(nameof(HorizontalWatermarkLocation), typeof(WatermarkLocation), typeof(Watermark), WatermarkLocation.End);

    public WatermarkLocation HorizontalWatermarkLocation
    {
        get => (WatermarkLocation)GetValue(HorizontalWatermarkLocationProperty);
        set => SetValue(HorizontalWatermarkLocationProperty, value);
    }

    public static readonly BindableProperty VerticalWatermarkLocationProperty =
        BindableProperty.Create(nameof(VerticalWatermarkLocation), typeof(WatermarkLocation), typeof(Watermark), WatermarkLocation.End);

    public WatermarkLocation VerticalWatermarkLocation
    {
        get => (WatermarkLocation)GetValue(VerticalWatermarkLocationProperty);
        set => SetValue(VerticalWatermarkLocationProperty, value);
    }

    public static readonly BindableProperty WatermarkPaddingProperty =
        BindableProperty.Create(nameof(WatermarkPadding), typeof(double), typeof(Watermark), 8d);

    public double WatermarkPadding
    {
        get => (double)GetValue(WatermarkPaddingProperty);
        set => SetValue(WatermarkPaddingProperty, value);
    }

    public static readonly BindableProperty WatermarkTextProperty =
        BindableProperty.Create(nameof(WatermarkText), typeof(string), typeof(Watermark));

    public string WatermarkText
    {
        get => (string)GetValue(WatermarkTextProperty);
        set => SetValue(WatermarkTextProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(Watermark), 24d);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty TypefaceProperty =
        BindableProperty.Create(nameof(Typeface), typeof(SKTypeface), typeof(Watermark));

    public SKTypeface Typeface
    {
        get => (SKTypeface)GetValue(TypefaceProperty);
        set => SetValue(TypefaceProperty, value);
    }

    public static readonly BindableProperty ForegroundColorProperty =
        BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(Watermark), Colors.White.MultiplyAlpha(.5f));

    public Color ForegroundColor
    {
        get => (Color)GetValue(ForegroundColorProperty);
        set => SetValue(ForegroundColorProperty, value);
    }

    public static readonly BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(Watermark));

    public Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }

    public static readonly BindableProperty BackgroundCornerRadiusProperty =
        BindableProperty.Create(nameof(BackgroundCornerRadius), typeof(double), typeof(Watermark), 4d);

    public double BackgroundCornerRadius
    {
        get => (double)GetValue(BackgroundCornerRadiusProperty);
        set => SetValue(BackgroundCornerRadiusProperty, value);
    }

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, SKImageInfo info, SKRect overrideRect) => InternalApplyEffect(surface, overrideRect != default ? overrideRect : info.Rect);

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, GRBackendRenderTarget info, SKRect overrideRect) => InternalApplyEffect(surface, overrideRect != default ? overrideRect : info.Rect);

    private SKImage InternalApplyEffect(SKSurface surface, SKRect rect)
    {
        string text = this.WatermarkText;

        if (string.IsNullOrEmpty(text))
        {
            return surface.Snapshot();
        }

        var canvas = surface.Canvas;

        using (var paint = new SKPaint())
        {
            paint.IsAntialias = true;
            paint.Style = SKPaintStyle.Fill;
            paint.TextSize = (float)this.FontSize;
            paint.Typeface = Typeface ?? PlatformInfo.DefaultTypeface;

            paint.EnsureHasValidFont(text);

            var measuredText = SKRect.Empty;
            paint.MeasureText(text, ref measuredText);
            float x = 0f;
            float y = 0f;

            switch (this.HorizontalWatermarkLocation)
            {
                case WatermarkLocation.Center:
                    x = rect.MidX - measuredText.MidX;
                    break;
                case WatermarkLocation.End:
                    x = rect.Left + rect.Width - measuredText.Width - (float)this.WatermarkPadding;
                    break;
                case WatermarkLocation.Start:
                default:
                    x = rect.Left + (float)this.WatermarkPadding;
                    break;
            }

            switch (this.VerticalWatermarkLocation)
            {
                case WatermarkLocation.Center:
                    y = rect.MidY - (measuredText.Height / 2f);
                    break;
                case WatermarkLocation.End:
                    y = rect.Top + rect.Height - (measuredText.Height / 2f) - (float)this.WatermarkPadding;
                    break;
                case WatermarkLocation.Start:
                default:
                    y = rect.Top + (measuredText.Height / 2f) + (float)this.WatermarkPadding;
                    break;
            }

            paint.Color = this.BackgroundColor.ToSKColor();
            float halfPadding = (float)this.WatermarkPadding / 2f;
            canvas.DrawRoundRect(
                x - halfPadding, y - (measuredText.Height / 2f) - halfPadding,
                measuredText.Width + (float)this.WatermarkPadding, measuredText.Height + (float)this.WatermarkPadding,
                (float)this.BackgroundCornerRadius, (float)this.BackgroundCornerRadius,
                paint);

            paint.Color = this.ForegroundColor.ToSKColor();
            canvas.DrawTextCenteredVertically(text, new SKPoint(x, y), paint);

            canvas.Flush();
        }

        return surface.Snapshot();
    }
}
