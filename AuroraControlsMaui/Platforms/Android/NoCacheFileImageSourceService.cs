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
    private static readonly BitmapFactory.Options _bitmapFactoryOptions =
        new()
        {
            InSampleSize = 1,
            InPreferredConfig = Bitmap.Config.Argb8888, // Uses less memory than ARGB_8888
            InDither = false,
            InTempStorage = new byte[32 * 1024], // 32KB buffer for decoding
        };

    // Cache for hardware bitmap capability detection
    private static readonly object _hardwareBitmapCacheLock = new();
    private static bool? _cachedHardwareBitmapSupport;

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

            var pathDrawable = CreateDrawableModern(file, imageView.Context!, fileImageSource.HardwareAcceleration);
            if (pathDrawable != null)
            {
                imageView.SetImageDrawable(pathDrawable);
                return new ImageSourceServiceLoadResult(() => pathDrawable.Dispose());
            }

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

            var pathDrawable = CreateDrawableModern(file, context!, fileImageSource.HardwareAcceleration);
            if (pathDrawable != null)
            {
                return new ImageSourceServiceResult(pathDrawable, () => pathDrawable.Dispose());
            }

            return null;
        }
        catch (Exception ex)
        {
            this.Logger?.LogWarning(ex, "Unable to load image file '{File}'.", file);
            throw;
        }
    }

    private static Drawable? CreateDrawableModern(string file, Context context, bool hardwareAcceleration = true)
    {
        try
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.P)
            {
                // Use modern ImageDecoder for Android API 28+
                var source = ImageDecoder.CreateSource(new Java.IO.File(file));
                var bitmap = ImageDecoder.DecodeBitmap(
                    source,
                    new ImageDecoderOnHeaderDecodedListener(
                        decoder =>
                        {
                            decoder.SetTargetColorSpace(ColorSpace.Get(ColorSpace.Named.Srgb)!);
                            decoder.MemorySizePolicy = ImageDecoderMemoryPolicy.Default;

                            // Determine if we should use hardware or software allocation
                            decoder.Allocator =
                                hardwareAcceleration && ShouldUseHardwareBitmap(context)
                                    ? ImageDecoderAllocator.Hardware
                                    : ImageDecoderAllocator.Software;
                        }));
                return new BitmapDrawable(context.Resources, bitmap);
            }
            else
            {
                // Fallback to optimized BitmapFactory for older devices
                var bitmap = BitmapFactory.DecodeFile(file, _bitmapFactoryOptions);
                return new BitmapDrawable(context.Resources, bitmap);
            }
        }
        catch
        {
            return null;
        }
    }

    private static bool ShouldUseHardwareBitmap(Context context)
    {
        // Only available on API 28+
        if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.P)
        {
            return false;
        }

        // Check the cached value first with double-checked locking pattern
        if (_cachedHardwareBitmapSupport.HasValue && !_cachedHardwareBitmapSupport.Value)
        {
            return false;
        }

        lock (_hardwareBitmapCacheLock)
        {
            // Double-check inside the lock to avoid race conditions
            if (_cachedHardwareBitmapSupport.HasValue && !_cachedHardwareBitmapSupport.Value)
            {
                return _cachedHardwareBitmapSupport.Value;
            }

            if (!_cachedHardwareBitmapSupport.HasValue)
            {
                try
                {
                    // Check if hardware acceleration is enabled for the application
                    var activity = context as Android.App.Activity;
                    if (activity?.Window?.Attributes?.Flags.HasFlag(Android.Views.WindowManagerFlags.HardwareAccelerated) == false)
                    {
                        _cachedHardwareBitmapSupport = false;
                        return false;
                    }

                    // Check if the device supports hardware bitmaps
                    // Hardware bitmaps require sufficient GPU memory and capabilities
                    var activityManager = context.GetSystemService(Context.ActivityService) as Android.App.ActivityManager;

                    // OpenGL ES 2.0 requirement
                    if (activityManager?.DeviceConfigurationInfo?.ReqGlEsVersion < 0x20000)
                    {
                        _cachedHardwareBitmapSupport = false;
                        return false;
                    }
                }
                catch
                {
                    // If any check fails, default to software bitmap for safety
                    return false;
                }
            }

            var memoryActivityManager = context.GetSystemService(Context.ActivityService) as Android.App.ActivityManager;

            // Check available memory - avoid hardware bitmaps on low memory devices
            var memoryInfo = new Android.App.ActivityManager.MemoryInfo();
            memoryActivityManager?.GetMemoryInfo(memoryInfo);

            // If available memory is less than 512MB, prefer software bitmaps for stability
            if (memoryInfo.AvailMem < 512 * 1024 * 1024)
            {
                return false;
            }

            // Additional check: avoid hardware bitmaps if we're in a software rendering context
            // This is a heuristic based on the current thread and context
            return !IsLikelySoftwareRenderingContext();
        }
    }

    private static bool IsLikelySoftwareRenderingContext()
    {
        try
        {
            // Check if we're potentially in a Canvas drawing operation or similar software context
            // This is a heuristic - if we're on the main UI thread and there are no obvious indicators
            // of hardware acceleration being actively used, prefer software bitmaps
            var currentThread = Java.Lang.Thread.CurrentThread();
            var threadName = currentThread?.Name;

            // If we're on a background thread, it's likely for caching/processing, use software
            if (threadName != null && (
                threadName.Contains("Background") ||
                threadName.Contains("Cache") ||
                threadName.Contains("Worker") ||
                !threadName.Contains("main")))
            {
                return true;
            }

            return false;
        }
        catch
        {
            return true; // Default to software rendering context if unsure
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
