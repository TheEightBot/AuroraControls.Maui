using System.Reflection;
using System.Text;
using Svg.Skia;

namespace AuroraControls;

public abstract class IconCacheBase : IIconCache, IDisposable
{
    private readonly char _underscore = '_';

    private readonly char[] _invalidFilenameChars;

    private readonly SemaphoreSlim _iconLock = new(1, 1);

    private readonly Dictionary<string, string> _resolvedIcons = new();

    private readonly SKPaint _paint = new() { BlendMode = SKBlendMode.SrcATop, Style = SKPaintStyle.Fill };

    private readonly float _platformScalingFactor;

    private readonly SKColorSpace _colorspace = SKColorSpace.CreateSrgb();

    private bool _disposedValue;

    public IconCacheBase()
    {
        _invalidFilenameChars = Path.GetInvalidFileNameChars();
        _platformScalingFactor = (float)PlatformInfo.ScalingFactor;
    }

    public Task<Image> IconFromSvg(string svgName, double squareSize = 22d, string additionalCacheKey = "", Color? colorOverride = null) => IconFromSvg(svgName, new Size(squareSize, squareSize), additionalCacheKey, colorOverride);

    public async Task<Image> IconFromSvg(string svgName, Size size, string additionalCacheKey = "", Color? colorOverride = null) =>
        new()
        {
            Source = await ImageSourceFromSvg(svgName, size, additionalCacheKey, colorOverride),
        };

    public Task<ImageSource> ImageSourceFromRawSvg(string svgName, string svgValue, double squareSize = 22d, string additionalCacheKey = "", Color? colorOverride = null) => ImageSourceFromRawSvg(svgName, svgValue, new Size(squareSize, squareSize), additionalCacheKey, colorOverride);

    public async Task<ImageSource> ImageSourceFromRawSvg(string svgName, string svgValue, Size size, string additionalCacheKey = "", Color? colorOverride = null)
    {
        try
        {
            await _iconLock.WaitAsync().ConfigureAwait(false);

            string key = CreateIconKey(svgName, size, additionalCacheKey, colorOverride);

            if (_resolvedIcons.ContainsKey(key))
            {
                return GetPlatformImageSource(_resolvedIcons[key]);
            }

            string? diskCachedImage = GetImagePathFromDiskCache(key);

            if (!string.IsNullOrEmpty(diskCachedImage))
            {
                _resolvedIcons[key] = diskCachedImage;
                return GetPlatformImageSource(diskCachedImage);
            }

            await GenerateImageFromRaw(key, svgValue, size, colorOverride).ConfigureAwait(false);

            diskCachedImage = GetImagePathFromDiskCache(key);

            _resolvedIcons[key] = diskCachedImage;

            return GetPlatformImageSource(diskCachedImage);
        }
        finally
        {
            _iconLock.Release();
        }
    }

    public Task<ImageSource> ImageSourceFromSvg(string svgName, double squareSize = 22d, string additionalCacheKey = "", Color? colorOverride = null) =>
        ImageSourceFromSvg(svgName, new Size(squareSize, squareSize), additionalCacheKey, colorOverride);

    public async Task<ImageSource> ImageSourceFromSvg(string svgName, Size size, string additionalCacheKey = "", Color? colorOverride = null)
    {
        try
        {
            await _iconLock.WaitAsync().ConfigureAwait(false);

            string key = CreateIconKey(svgName, size, additionalCacheKey, colorOverride);

            if (_resolvedIcons.ContainsKey(key))
            {
                return GetPlatformImageSource(_resolvedIcons[key]);
            }

            string? diskCachedImage = GetImagePathFromDiskCache(key);

            if (!string.IsNullOrEmpty(diskCachedImage))
            {
                _resolvedIcons[key] = diskCachedImage;
                return GetPlatformImageSource(diskCachedImage);
            }

            await GenerateImageFromEmbedded(key, svgName, size, colorOverride).ConfigureAwait(false);

            diskCachedImage = GetImagePathFromDiskCache(key);

            _resolvedIcons[key] = diskCachedImage;

            return GetPlatformImageSource(diskCachedImage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SVG Exception]\t {ex}");

            return GetPlatformImageSource();
        }
        finally
        {
            _iconLock.Release();
        }
    }

    private ImageSource GetPlatformImageSource(string? file = null)
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            return new NoCacheFileImageSource { File = file };
        }

        return new FileImageSource { File = file };
    }

    private string? GetImagePathFromDiskCache(string key)
    {
        if (!Directory.Exists(PlatformInfo.IconCacheDirectory))
        {
            return null;
        }

        string filePath = Path.Combine(PlatformInfo.IconCacheDirectory, key);

        if (!File.Exists(filePath))
        {
            return null;
        }

        if (DeviceInfo.Current.Platform == DevicePlatform.iOS ||
            DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst)
        {
            return $"{filePath.Split('@')[0]}.png";
        }

        return filePath;
    }

    private async Task SaveIconToDiskCache(string key, Stream imageStream)
    {
        Directory.CreateDirectory(PlatformInfo.IconCacheDirectory);

        using var file = File.OpenWrite(Path.Combine(PlatformInfo.IconCacheDirectory, key));
        await imageStream.CopyToAsync(file).ConfigureAwait(false);
        await file.FlushAsync().ConfigureAwait(false);
    }

    private async Task GenerateImageFromEmbedded(string key, string svgName, Size size, Color? colorOverride = null)
    {
        try
        {
#if DEBUG
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
#endif

            using var skSvg = new SKSvg();

            using (var stream = EmbeddedResourceLoader.Load(svgName))
            {
                skSvg.Load(stream);
                await stream.FlushAsync().ConfigureAwait(false);
            }

            await RenderSvgAsync(skSvg, key, size, colorOverride).ConfigureAwait(false);

#if DEBUG
            stopWatch.Stop();

            System.Diagnostics.Debug.WriteLine(
                "Generating image \"{0}\" from SvG took {1}ms",
                svgName, stopWatch.ElapsedMilliseconds);
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SVG Exception]\t {ex}");
        }
    }

    private async Task GenerateImageFromRaw(string key, string svgValue, Size size, Color? colorOverride = null)
    {
        try
        {
#if DEBUG
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
#endif

            using var skSvg = new SKSvg();

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(svgValue)))
            {
                ms.Seek(0L, SeekOrigin.Begin);
                skSvg.Load(ms);
            }

            await RenderSvgAsync(skSvg, key, size, colorOverride).ConfigureAwait(false);

#if DEBUG
            stopWatch.Stop();

            System.Diagnostics.Debug.WriteLine(
                "Generating image \"{0}\" from SvG took {1}ms",
                key, stopWatch.ElapsedMilliseconds);
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SVG Exception]\t {ex}");
        }
    }

    private async Task RenderSvgAsync(SKSvg skSvg, string key, Size size, Color? colorOverride)
    {
        float platformScalingFactor = _platformScalingFactor;

        // Calculate the final canvas size with platform scaling
        float targetCanvasWidth = (float)(size.Width * platformScalingFactor);
        float targetCanvasHeight = (float)(size.Height * platformScalingFactor);

        SKRect resize;
        SKMatrix scaleMatrix = SKMatrix.Identity;

        if (size != default && skSvg.Picture?.CullRect != default)
        {
            var svgRect = skSvg.Picture!.CullRect;

            // Calculate scale factors for both dimensions
            float scaleX = targetCanvasWidth / svgRect.Width;
            float scaleY = targetCanvasHeight / svgRect.Height;

            // Use the smaller scale factor to ensure the SVG fits entirely within the target size
            float uniformScale = Math.Min(scaleX, scaleY);

            // Calculate the actual canvas size that maintains the original aspect ratio
            float actualCanvasWidth = svgRect.Width * uniformScale;
            float actualCanvasHeight = svgRect.Height * uniformScale;

            // Create transformation matrix for scaling, no centering needed since canvas matches aspect ratio
            scaleMatrix = SKMatrix.CreateScale(uniformScale, uniformScale);
            scaleMatrix = scaleMatrix.PostConcat(SKMatrix.CreateTranslation(-(svgRect.Left * uniformScale), -(svgRect.Top * uniformScale)));

            resize = new SKRect(0, 0, actualCanvasWidth, actualCanvasHeight);
        }
        else if (skSvg.Picture?.CullRect != default)
        {
            resize = new SKRect(0, 0, (float)Math.Ceiling(skSvg.Picture!.CullRect.Width * platformScalingFactor), (float)Math.Ceiling(skSvg.Picture.CullRect.Height * platformScalingFactor));
        }
        else
        {
            resize = new SKRect(0, 0, targetCanvasWidth, targetCanvasHeight);
        }

        // Calculate final image dimensions once
        int finalWidth = (int)Math.Ceiling(resize.Width);
        int finalHeight = (int)Math.Ceiling(resize.Height);

        // Use a single rendering path to reduce code duplication and memory allocations
        var imageInfo = new SKImageInfo(finalWidth, finalHeight, SKColorType.Rgba8888, SKAlphaType.Premul, _colorspace);

        using var bitmap = new SKBitmap(imageInfo);
        using var canvas = new SKCanvas(bitmap);

        canvas.Clear(SKColors.Transparent);

        if (size != default && !scaleMatrix.IsIdentity)
        {
            canvas.SetMatrix(scaleMatrix);
        }

        canvas.DrawPicture(skSvg.Picture);

        // Apply color override if needed
        if (!Equals(colorOverride, Colors.Transparent))
        {
            _paint.Color = colorOverride.ToSKColor();
            canvas.DrawPaint(_paint);
        }

        canvas.Flush();

        // Encode directly to stream and save to cache
        using var image = SKImage.FromBitmap(bitmap);
        using var encodedData = image.Encode(SKEncodedImageFormat.Png, 85); // Reduced quality for smaller file size
        using var stream = encodedData.AsStream(false); // Don't transfer ownership to avoid extra allocation

        await SaveIconToDiskCache(key, stream).ConfigureAwait(false);
    }

    public void LoadAssembly(Assembly assembly) => EmbeddedResourceLoader.LoadAssembly(assembly);

    public Task ClearCache()
    {
        _resolvedIcons.Clear();

        if (Directory.Exists(PlatformInfo.IconCacheDirectory))
        {
            string[]? files = Directory.GetFiles(PlatformInfo.IconCacheDirectory);

            if (files?.Any() ?? false)
            {
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }

            Directory.Delete(PlatformInfo.IconCacheDirectory, true);
        }

        return Task.CompletedTask;
    }

    public abstract Task<SKBitmap> SKBitmapFromSource(ImageSource imageSource);

    public abstract Task<byte[]> ByteArrayFromSource(ImageSource imageSource);

    public abstract Task<Stream> StreamFromSource(ImageSource imageSource);

    private string CreateIconKey(string svgName, Size size, string additionalCacheKey = "", Color? colorOverride = null)
    {
        string key = $"{svgName.Replace(".svg", string.Empty, StringComparison.OrdinalIgnoreCase)}_{additionalCacheKey}_{size.Width}_{size.Height}_{colorOverride?.GetHashCode()}".Trim(_underscore);

        if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst || DeviceInfo.Platform == DevicePlatform.macOS)
        {
            key = $"{key}@{_platformScalingFactor}x";
        }

        key = string.Join(_underscore, key.Split(_invalidFilenameChars));

        return $"{key}.png";
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue || !disposing)
        {
            return;
        }

        _iconLock?.Dispose();
        _colorspace.Dispose();
        _paint.Dispose();

        _disposedValue = true;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
