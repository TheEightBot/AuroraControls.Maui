using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Bumptech.Glide.Request.Target;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Platform;
using Path = System.IO.Path;

namespace AuroraControls;

internal partial class NoCacheFileImageSourceService
{
    public override async Task<IImageSourceServiceResult?> LoadDrawableAsync(IImageSource imageSource, ImageView imageView,
        CancellationToken cancellationToken = default)
    {
        var fileImageSource = (INoCacheFileImageSource)imageSource;

        if (!fileImageSource.IsEmpty)
        {
            var file = fileImageSource.File;

            try
            {
                if (!Path.IsPathRooted(file) || !File.Exists(file))
                {
                    var id = imageView.Context?.GetDrawableId(file) ?? -1;
                    if (id > 0)
                    {
                        imageView.SetImageResource(id);
                        return new ImageSourceServiceLoadResult();
                    }
                }

                using var pathBitmap = await BitmapFactory.DecodeFileAsync(file);
                using var pathDrawable = new BitmapDrawable(Platform.AppContext.Resources, pathBitmap);
                imageView.SetImageDrawable(pathDrawable);

                return new ImageSourceServiceLoadResult();
            }
            catch (Exception ex)
            {
                Logger?.LogWarning(ex, "Unable to load image file '{File}'.", file);
                throw;
            }
        }

        return null;
    }

    public override async Task<IImageSourceServiceResult<Drawable>?> GetDrawableAsync(IImageSource imageSource, Context context,
        CancellationToken cancellationToken = default)
    {
        var fileImageSource = (INoCacheFileImageSource)imageSource;
        if (!fileImageSource.IsEmpty)
        {
            var file = fileImageSource.File;

            try
            {
                if (!Path.IsPathRooted(file) || !File.Exists(file))
                {
                    var id = context?.GetDrawableId(file) ?? -1;
                    if (id > 0)
                    {
                        var d = context?.GetDrawable(id);
                        if (d is not null)
                        {
                            return new ImageSourceServiceResult(d);
                        }
                    }
                }

                var pathBitmap = await BitmapFactory.DecodeFileAsync(file);
                var pathDrawable = new BitmapDrawable(Platform.AppContext.Resources, pathBitmap);
                return new ImageSourceServiceResult(pathDrawable);
            }
            catch (Exception ex)
            {
                Logger?.LogWarning(ex, "Unable to load image file '{File}'.", file);
                throw;
            }
        }

        return null;
    }
}

internal class AuroraImageLoaderCallback : AuroraImageLoaderCallbackBase<IImageSourceServiceResult>
{
    protected override IImageSourceServiceResult? OnSuccess(Drawable? drawable, Action? dispose) =>
        new ImageSourceServiceLoadResult(dispose);
}

internal class AuroraImageLoaderResultCallback : AuroraImageLoaderCallbackBase<IImageSourceServiceResult<Drawable>>
{
    protected override IImageSourceServiceResult<Drawable>? OnSuccess(Drawable? drawable, Action? dispose) =>
        drawable is not null
            ? new ImageSourceServiceResult(drawable, dispose)
            : default;
}

internal abstract class AuroraImageLoaderCallbackBase<T> : Java.Lang.Object, IImageLoaderCallback
    where T : IImageSourceServiceResult
{
    private readonly TaskCompletionSource<T?> _tcsResult = new();

    public Task<T?> Result => _tcsResult.Task;

    public void OnComplete(Java.Lang.Boolean? success, Drawable? drawable, Java.Lang.IRunnable? dispose)
    {
        try
        {
            Action? disposeWrapper = dispose != null
                ? dispose.Run
                : null;

            var result = success?.BooleanValue() == true
                ? OnSuccess(drawable, disposeWrapper)
                : OnFailure(drawable, disposeWrapper);

            _tcsResult.SetResult(result);
        }
        catch (Exception ex)
        {
            _tcsResult.SetException(ex);
        }
    }

    protected abstract T? OnSuccess(Drawable? drawable, Action? dispose);

    protected virtual T? OnFailure(Drawable? errorDrawable, Action? dispose) => default;
}
