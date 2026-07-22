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
using Microsoft.Extensions.Options;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Platform;
using Path = System.IO.Path;

namespace AuroraControls;

internal partial class NoCacheFileImageSourceService
{
    // TEMPORARY diagnostic tracing for the disappearing-icon investigation (bucket 1).
    // Remove or replace with proper logging once the root cause is confirmed.
    private static int _traceLoadCounter;

    private static void Trace(string message) =>
        Console.WriteLine($"[AuroraSvgTrace] {message}");

    public override async Task<IImageSourceServiceResult?> LoadDrawableAsync(IImageSource imageSource, ImageView imageView,
        CancellationToken cancellationToken = default)
    {
        var fileImageSource = (INoCacheFileImageSource)imageSource;

        if (fileImageSource.IsEmpty)
        {
            return null;
        }

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

            var loadId = Interlocked.Increment(ref _traceLoadCounter);
            Trace($"Load #{loadId} (LoadDrawableAsync) file='{Path.GetFileName(file)}' exists={File.Exists(file)} hwAccel={fileImageSource.HardwareAcceleration}");

            var pathDrawable = await CreateDrawableWithHealingAsync(fileImageSource, imageView.Context!, loadId);
            if (pathDrawable != null)
            {
                imageView.SetImageDrawable(pathDrawable);

                // Intentionally no drawable/bitmap disposal here: the bitmap may still be
                // referenced by the ImageView (or shared through the in-memory cache) when
                // MAUI releases the load result during navigation. The GC reclaims it once
                // nothing references it.
                return new ImageSourceServiceLoadResult(() =>
                    Trace($"Load #{loadId} release callback invoked (no-op) file='{Path.GetFileName(file)}'"));
            }

            Trace($"Load #{loadId} FAILED to create drawable (LoadDrawableAsync) file='{Path.GetFileName(file)}'");
            return null;
        }
        catch (Exception ex)
        {
            this.Logger?.LogWarning(ex, "Unable to load image file '{File}'.", file);
            throw;
        }
    }

    public override async Task<IImageSourceServiceResult<Drawable>?> GetDrawableAsync(IImageSource imageSource, Context context,
        CancellationToken cancellationToken = default)
    {
        var fileImageSource = (INoCacheFileImageSource)imageSource;
        if (fileImageSource.IsEmpty)
        {
            return null;
        }

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

            var loadId = Interlocked.Increment(ref _traceLoadCounter);
            Trace($"Load #{loadId} (GetDrawableAsync) file='{Path.GetFileName(file)}' exists={File.Exists(file)} hwAccel={fileImageSource.HardwareAcceleration}");

            var pathDrawable = await CreateDrawableWithHealingAsync(fileImageSource, context!, loadId);
            if (pathDrawable != null)
            {
                return new ImageSourceServiceResult(pathDrawable, () =>
                    Trace($"Load #{loadId} release callback invoked (no-op) file='{Path.GetFileName(file)}'"));
            }

            Trace($"Load #{loadId} FAILED to create drawable (GetDrawableAsync) file='{Path.GetFileName(file)}'");
            return null;
        }
        catch (Exception ex)
        {
            this.Logger?.LogWarning(ex, "Unable to load image file '{File}'.", file);
            throw;
        }
    }

    /// <summary>
    /// Creates a drawable for the source file, serving the bitmap from the in-memory cache
    /// when possible. If the file is missing or fails to decode (e.g. the OS trimmed the
    /// app cache directory), invokes the source's <see cref="INoCacheFileImageSource.Regenerate"/>
    /// callback to re-render the icon and retries once.
    /// </summary>
    private static async Task<Drawable?> CreateDrawableWithHealingAsync(INoCacheFileImageSource source, Context context, int loadId)
    {
        var drawable = TryCreateDrawable(source.File, context);

        if (drawable is not null)
        {
            return drawable;
        }

        if (source.Regenerate is null)
        {
            return null;
        }

        Trace($"Load #{loadId} file missing or undecodable; regenerating '{Path.GetFileName(source.File)}'");

        var regeneratedPath = await source.Regenerate().ConfigureAwait(true);

        if (string.IsNullOrEmpty(regeneratedPath))
        {
            return null;
        }

        drawable = TryCreateDrawable(regeneratedPath, context);

        if (drawable is not null)
        {
            Trace($"Load #{loadId} regeneration succeeded for '{Path.GetFileName(regeneratedPath)}'");
        }

        return drawable;
    }

    private static Drawable? TryCreateDrawable(string file, Context context)
    {
        var bitmap = BitmapCache.Get(file) ?? DecodeAndCacheBitmap(file);

        return bitmap is not null
            ? new BitmapDrawable(context.Resources, bitmap)
            : null;
    }

    private static Bitmap? DecodeAndCacheBitmap(string file)
    {
        try
        {
            if (!File.Exists(file))
            {
                return null;
            }

            Bitmap? bitmap;

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.P)
            {
                using var source = ImageDecoder.CreateSource(new Java.IO.File(file));
                bitmap = ImageDecoder.DecodeBitmap(
                    source,
                    new ImageDecoderOnHeaderDecodedListener(
                        decoder =>
                        {
                            decoder.MemorySizePolicy = ImageDecoderMemoryPolicy.Default;

                            // Software allocation: these bitmaps may be drawn into software
                            // canvases (navigation transitions, software layers), where
                            // hardware bitmaps throw and render blank.
                            decoder.MutableRequired = true;
                            decoder.Allocator = ImageDecoderAllocator.Software;
                        }));
            }
            else
            {
                var options = new BitmapFactory.Options
                {
                    InSampleSize = 1,
                    InPreferredConfig = Bitmap.Config.Argb8888,
                };
                bitmap = BitmapFactory.DecodeFile(file, options);
            }

            if (bitmap is not null)
            {
                BitmapCache.Put(file, bitmap);
            }

            return bitmap;
        }
        catch (Exception ex)
        {
            Trace($"DecodeAndCacheBitmap EXCEPTION for file='{Path.GetFileName(file)}': {ex.GetType().Name}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Bounded in-memory bitmap cache keyed by file path. Icon bitmaps are shared across
    /// drawables (bitmaps are never explicitly recycled; the GC reclaims them after they
    /// leave the cache and no view references them), which makes repeat loads during
    /// navigation/tab churn instant and removes the disk round-trip.
    /// </summary>
    private static class BitmapCache
    {
        private const int MaxSizeBytes = 12 * 1024 * 1024;

        private static readonly object _lock = new();
        private static readonly Dictionary<string, LinkedListNode<(string Key, Bitmap Bitmap, int Size)>> _entries = new();
        private static readonly LinkedList<(string Key, Bitmap Bitmap, int Size)> _lruOrder = new();
        private static int _currentSizeBytes;

        public static Bitmap? Get(string key)
        {
            lock (_lock)
            {
                if (!_entries.TryGetValue(key, out var node))
                {
                    return null;
                }

                var bitmap = node.Value.Bitmap;

                if (bitmap.Handle == IntPtr.Zero || bitmap.IsRecycled)
                {
                    RemoveNode(node);
                    return null;
                }

                _lruOrder.Remove(node);
                _lruOrder.AddFirst(node);

                return bitmap;
            }
        }

        public static void Put(string key, Bitmap bitmap)
        {
            var size = bitmap.ByteCount;

            lock (_lock)
            {
                if (_entries.TryGetValue(key, out var existing))
                {
                    RemoveNode(existing);
                }

                var node = _lruOrder.AddFirst((key, bitmap, size));
                _entries[key] = node;
                _currentSizeBytes += size;

                while (_currentSizeBytes > MaxSizeBytes && _lruOrder.Last is not null)
                {
                    RemoveNode(_lruOrder.Last);
                }
            }
        }

        private static void RemoveNode(LinkedListNode<(string Key, Bitmap Bitmap, int Size)> node)
        {
            _entries.Remove(node.Value.Key);
            _lruOrder.Remove(node);
            _currentSizeBytes -= node.Value.Size;
        }
    }

    private class ImageDecoderOnHeaderDecodedListener : Java.Lang.Object, ImageDecoder.IOnHeaderDecodedListener
    {
        private readonly Action<ImageDecoder> _onHeaderDecoded;

        public ImageDecoderOnHeaderDecodedListener(Action<ImageDecoder> onHeaderDecoded)
        {
            _onHeaderDecoded = onHeaderDecoded;
        }

        public void OnHeaderDecoded(ImageDecoder decoder, ImageDecoder.ImageInfo info, ImageDecoder.Source source)
        {
            _onHeaderDecoded(decoder);
        }
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
