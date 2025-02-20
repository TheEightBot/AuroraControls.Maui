using Microsoft.Maui.Controls.Platform;

#if ANDROID
using Android.Graphics.Drawables;
using Android.Widget;
using SkiaSharp.Views.Android;
#endif

#if IOS || MACCATALYST
using SkiaSharp.Views.iOS;
using UIKit;
#endif

namespace AuroraControls.Effects;

/// <summary>
/// Image processing effect.
/// </summary>
public class ImageProcessingEffect : RoutingEffect
{
    /// <summary>
    /// Gets the image processing effects.
    /// </summary>
    /// <value>The image processing effects.</value>
    public ImageProcessing.ImageProcessingCollection ImageProcessingEffects { get; private set; }
        = new ImageProcessing.ImageProcessingCollection();

    /// <summary>
    /// The processor changed property.
    /// </summary>
    public static readonly BindablePropertyKey ProcessorChangedProperty =
        BindableProperty.CreateReadOnly("ProcessorChanged", typeof(object), typeof(ImageProcessingEffect), null);

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageProcessingEffect"/> class.
    /// </summary>
    public ImageProcessingEffect()
    {
    }
}

#if ANDROID
public class ImageProcessingPlatformEffect : PlatformEffect
{
    private SKBitmap _image;

    private bool _processing;

    protected override async void OnAttached()
    {
        var view = Control as ImageView;

        if (view == null)
        {
            return;
        }

        if (this.Element?.Effects?.FirstOrDefault(e => e is ImageProcessingEffect) is ImageProcessingEffect effect)
        {
            effect.ImageProcessingEffects.PropertyChanged += ImageProcessingEffects_PropertyChanged;
        }

        var drawable = view?.Drawable as BitmapDrawable;

        var androidBitmap = drawable?.Bitmap;

        _image = androidBitmap?.ToSKBitmap();

        await ProcessImage(view, Element);
    }

    protected override void OnDetached()
    {
        if (this.Element?.Effects?.FirstOrDefault(e => e is ImageProcessingEffect) is ImageProcessingEffect effect)
        {
            effect.ImageProcessingEffects.PropertyChanged -= ImageProcessingEffects_PropertyChanged;
        }
    }

    private async void ImageProcessingEffects_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs args)
    {
        var view = Control as ImageView;

        if (view == null)
        {
            return;
        }

        if (args.PropertyName.Equals(ImageProcessingEffect.ProcessorChangedProperty.BindableProperty.PropertyName))
        {
            await ProcessImage(view, Element);
        }
    }

    protected override async void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
    {
        var view = Control as ImageView;

        if (view == null)
        {
            return;
        }

        if (args.PropertyName.Equals(Image.SourceProperty.PropertyName) ||
            args.PropertyName.Equals(Image.IsLoadingProperty.PropertyName))
        {
            var drawable = view?.Drawable as BitmapDrawable;

            var androidBitmap = drawable?.Bitmap;

            _image = androidBitmap?.ToSKBitmap();

            await ProcessImage(view, Element);
        }

        base.OnElementPropertyChanged(args);
    }

    private async Task ProcessImage(ImageView view, Element element)
    {
        if (_processing)
        {
            return;
        }

        _processing = true;

        try
        {
            if (element == null)
            {
                return;
            }

            if (_image == null)
            {
                view.SetImageBitmap(null);
                return;
            }

            var effect = Element?.Effects?.FirstOrDefault(e => e is ImageProcessingEffect) as ImageProcessingEffect;

            var processingEffects = effect?.ImageProcessingEffects;

            if (!processingEffects?.Any() ?? true)
            {
                return;
            }

            SKBitmap processingImage = null;

            try
            {
                processingImage = _image.Copy();

                await Task.Run(() =>
                {
                    foreach (var processingEffect in processingEffects)
                    {
                        var imageProcessor = ImageProcessing.RegisteredImageProcessors.GetProcessor(processingEffect.Key);

                        if (imageProcessor != null)
                        {
                            var tempImage = imageProcessor.ProcessImage(processingImage, processingEffect);

                            if (tempImage != processingImage)
                            {
                                processingImage?.Dispose();
                                processingImage = null;
                            }

                            processingImage = tempImage;
                        }
                    }
                });

                using var native = processingImage.ToBitmap();
                (view?.Drawable as BitmapDrawable)?.Bitmap?.Recycle();
                view.SetImageBitmap(native);
            }
            finally
            {
                processingImage?.Dispose();
                processingImage = null;
            }
        }
        finally
        {
            _processing = false;
        }
    }
}
#endif

#if IOS || MACCATALYST
public class ImageProcessingPlatformEffect : PlatformEffect
{
    private SKBitmap _image;

    private bool _processing;

    private long _lastProcessingTime;

    protected override async void OnAttached()
    {
        var view = Control as UIImageView;

        if (view == null)
        {
            return;
        }

        if (this.Element?.Effects?.FirstOrDefault(e => e is Effects.ImageProcessingEffect) is ImageProcessingEffect effect)
        {
            effect.ImageProcessingEffects.PropertyChanged += ImageProcessingEffects_PropertyChanged;
        }

        _image = view?.Image?.ToSKBitmap();
        await ProcessImage(view, Element);
    }

    protected override void OnDetached()
    {
        if (this.Element?.Effects?.FirstOrDefault(e => e is ImageProcessingEffect) is ImageProcessingEffect effect)
        {
            effect.ImageProcessingEffects.PropertyChanged -= ImageProcessingEffects_PropertyChanged;
        }
    }

    private async void ImageProcessingEffects_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs args)
    {
        var view = Control as UIImageView;

        if (view == null)
        {
            return;
        }

        if (args.PropertyName.Equals(ImageProcessingEffect.ProcessorChangedProperty.BindableProperty.PropertyName))
        {
            _lastProcessingTime = DateTime.UtcNow.Ticks;
            await ProcessImage(view, Element);
        }
    }

    protected override async void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
    {
        var view = Control as UIImageView;

        if (view == null)
        {
            return;
        }

        if (args.PropertyName.Equals(Image.SourceProperty.PropertyName) ||
            args.PropertyName.Equals(Image.IsLoadingProperty.PropertyName))
        {
            _image = view?.Image?.ToSKBitmap();

            await ProcessImage(view, Element);
        }

        base.OnElementPropertyChanged(args);
    }

    private async Task ProcessImage(UIImageView view, Element element)
    {
        if (_processing)
        {
            return;
        }

        _processing = true;
        var currentProcessingTime = _lastProcessingTime;

        try
        {
            if (element == null)
            {
                return;
            }

            if (_image == null)
            {
                view.Image = null;
                return;
            }

            var effect = Element?.Effects?.FirstOrDefault(e => e is ImageProcessingEffect) as ImageProcessingEffect;

            var processingEffects = effect?.ImageProcessingEffects;

            if (!processingEffects?.Any() ?? true)
            {
                return;
            }

            SKBitmap processingImage = null;
            try
            {
                processingImage = _image.Copy();

                await Task.Run(() =>
                {
                    foreach (var processingEffect in processingEffects)
                    {
                        var imageProcessor = ImageProcessing.RegisteredImageProcessors.GetProcessor(processingEffect.Key);

                        if (imageProcessor != null)
                        {
                            var tempImage = imageProcessor.ProcessImage(processingImage, processingEffect);

                            if (tempImage != processingImage)
                            {
                                processingImage?.Dispose();
                                processingImage = null;
                            }

                            processingImage = tempImage;
                        }
                    }
                });

                view.Image?.Dispose();

                using var native = processingImage?.ToUIImage();
                view.Image = native;
            }
            finally
            {
                processingImage?.Dispose();
                processingImage = null;
            }
        }
        finally
        {
            _processing = false;
            if (_lastProcessingTime > currentProcessingTime)
            {
                await ProcessImage(view, element);
            }
        }
    }
}
#endif
