using System;
using SkiaSharp;

namespace AuroraControls.ImageProcessing;

/// <summary>
/// Blur effect for images.
/// </summary>
public class Blur : ImageProcessingBase, IImageProcessor
{
    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The "Blur" key.</value>
    public override string Key => nameof(Blur);

    /// <summary>
    /// Blur location options.
    /// </summary>
    public enum BlurLocation
    {
        Full,
        Inside,
    }

    /// <summary>
    /// The blur amount property.
    /// </summary>
    public static BindableProperty BlurAmountProperty =
        BindableProperty.Create(nameof(BlurAmount), typeof(double), typeof(Blur), default(double));

    /// <summary>
    /// Gets or sets the blur amount.
    /// </summary>
    /// <value>A double value representing the blur amount. Default value is default(double).</value>
    public double BlurAmount
    {
        get { return (double)GetValue(BlurAmountProperty); }
        set { SetValue(BlurAmountProperty, value); }
    }

    /// <summary>
    /// The blurring location property.
    /// </summary>
    public static BindableProperty BlurringLocationProperty =
        BindableProperty.Create(nameof(BlurringLocation), typeof(object), typeof(BlurLocation), BlurLocation.Full);

    /// <summary>
    /// Gets or sets the blurring location.
    /// </summary>
    /// <value>Takes a BlurLocation enum. Default value is BlurLocation.Full.</value>
    public BlurLocation BlurringLocation
    {
        get { return (BlurLocation)GetValue(BlurringLocationProperty); }
        set { SetValue(BlurringLocationProperty, value); }
    }

    /// <summary>
    /// Processes the image and apply blur.
    /// </summary>
    /// <returns>The an SKBitmap image.</returns>
    /// <param name="processingImage">Processing SKBitmap image.</param>
    /// <param name="imageProcessor">Image processor.</param>
    public SKBitmap ProcessImage(SKBitmap processingImage, ImageProcessingBase imageProcessor)
    {
        if (imageProcessor is not AuroraControls.ImageProcessing.Blur)
        {
            return processingImage;
        }

        var blur = imageProcessor as AuroraControls.ImageProcessing.Blur;

        var blurAmount = (float)blur.BlurAmount;
        var blurLocation = blur.BlurringLocation;

        var bitmap = new SKBitmap(processingImage.Info);

        using var canvas = new SKCanvas(bitmap);
        using var paint = new SKPaint();
        paint.IsAntialias = true;
        paint.Style = SKPaintStyle.Fill;

        if (blurLocation == AuroraControls.ImageProcessing.Blur.BlurLocation.Inside)
        {
            paint.BlendMode = SKBlendMode.SrcOver;
        }

        paint.ImageFilter = SKImageFilter.CreateBlur((int)blurAmount, (int)blurAmount);

        canvas.Clear();

        if (blurLocation == AuroraControls.ImageProcessing.Blur.BlurLocation.Full)
        {
            canvas.DrawBitmap(processingImage, processingImage.Info.Rect);
        }

        canvas.DrawBitmap(processingImage, processingImage.Info.Rect, paint);

        canvas.Flush();

        return bitmap;
    }
}
