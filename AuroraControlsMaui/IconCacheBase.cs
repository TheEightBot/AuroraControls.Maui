using System.Reflection;
using System.Text;
using Svg.Skia;

namespace AuroraControls;

public abstract class IconCacheBase : IIconCache, IDisposable
{
    private readonly char _underscore = '_';

    private readonly char[] _invalidFilenameChars;

    private readonly SemaphoreSlim _iconLock = new SemaphoreSlim(1, 1);

    private readonly Dictionary<string, string> _resolvedIcons = new Dictionary<string, string>();

    private readonly SKPaint _paint = new SKPaint { BlendMode = SKBlendMode.SrcATop, Style = SKPaintStyle.Fill };

    private readonly float _platformScalingFactor;

    private readonly SKColorSpace _colorspace = SKColorSpace.CreateSrgb();

    private bool _disposedValue;

    public IconCacheBase()
    {
        _invalidFilenameChars = System.IO.Path.GetInvalidFileNameChars();
        _platformScalingFactor = (float)PlatformInfo.ScalingFactor;
    }

    public Task<Image> IconFromSvg(string svgName, double squareSize = 22d, string additionalCacheKey = "", Color? colorOverride = null) => IconFromSvg(svgName, new Size(squareSize, squareSize), additionalCacheKey, colorOverride);

    public async Task<Image> IconFromSvg(string svgName, Size size, string additionalCacheKey = "", Color? colorOverride = null) =>
        new()
        {
            Source = await SourceFromSvg(svgName, size, additionalCacheKey, colorOverride),
        };

    public Task<ImageSource> SourceFromSvg(string svgName, double squareSize = 22d, string additionalCacheKey = "", Color? colorOverride = null) => SourceFromSvg(svgName, new Size(squareSize, squareSize), additionalCacheKey, colorOverride);

    public async Task<ImageSource> SourceFromSvg(string svgName, Size size, string additionalCacheKey = "", Color? colorOverride = null)
    {
        var key = CreateIconKey(svgName, size, additionalCacheKey, colorOverride);

        try
        {
            await _iconLock.WaitAsync().ConfigureAwait(false);

            if (_resolvedIcons.ContainsKey(key))
            {
                return new FileImageSource { File = _resolvedIcons[key] };
            }

            var diskCachedImage = GetImagePathFromDiskCache(key);

            if (!string.IsNullOrEmpty(diskCachedImage))
            {
                _resolvedIcons[key] = diskCachedImage;
                return new FileImageSource { File = diskCachedImage };
            }

            await GenerateImageFromEmbedded(key, svgName, size, colorOverride).ConfigureAwait(false);

            diskCachedImage = GetImagePathFromDiskCache(key);

            _resolvedIcons[key] = diskCachedImage;

            return new FileImageSource { File = diskCachedImage };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SVG Exception]\t {ex}");

            return new FileImageSource();
        }
        finally
        {
            _iconLock.Release();
        }
    }

    public Task<ImageSource> SourceFromRawSvg(string svgName, string svgValue, double squareSize = 22d, string additionalCacheKey = "", Color? colorOverride = null) => SourceFromRawSvg(svgName, svgValue, new Size(squareSize, squareSize), additionalCacheKey, colorOverride);

    public async Task<ImageSource> SourceFromRawSvg(string svgName, string svgValue, Size size, string additionalCacheKey = "", Color? colorOverride = null)
    {
        try
        {
            await _iconLock.WaitAsync().ConfigureAwait(false);

            var key = CreateIconKey(svgName, size, additionalCacheKey, colorOverride);

            if (_resolvedIcons.ContainsKey(key))
            {
                return new FileImageSource { File = _resolvedIcons[key] };
            }

            var diskCachedImage = GetImagePathFromDiskCache(key);

            if (!string.IsNullOrEmpty(diskCachedImage))
            {
                _resolvedIcons[key] = diskCachedImage;
                return new FileImageSource { File = diskCachedImage };
            }

            await GenerateImageFromRaw(key, svgValue, size, colorOverride).ConfigureAwait(false);

            diskCachedImage = GetImagePathFromDiskCache(key);

            _resolvedIcons[key] = diskCachedImage;

            return new FileImageSource { File = diskCachedImage };
        }
        finally
        {
            _iconLock.Release();
        }
    }

    public Task<FileImageSource> FileImageSourceFromSvg(string svgName, double squareSize = 22d, string additionalCacheKey = "", Color? colorOverride = null) => FileImageSourceFromSvg(svgName, new Size(squareSize, squareSize), additionalCacheKey, colorOverride);

    public async Task<FileImageSource> FileImageSourceFromSvg(string svgName, Size size, string additionalCacheKey = "", Color? colorOverride = null)
    {
        try
        {
            await _iconLock.WaitAsync().ConfigureAwait(false);

            var key = CreateIconKey(svgName, size, additionalCacheKey, colorOverride);

            if (_resolvedIcons.ContainsKey(key))
            {
                return new FileImageSource { File = _resolvedIcons[key] };
            }

            var diskCachedImage = GetImagePathFromDiskCache(key);

            if (!string.IsNullOrEmpty(diskCachedImage))
            {
                _resolvedIcons[key] = diskCachedImage;
                return new FileImageSource { File = diskCachedImage };
            }

            await GenerateImageFromEmbedded(key, svgName, size, colorOverride).ConfigureAwait(false);

            diskCachedImage = GetImagePathFromDiskCache(key);

            _resolvedIcons[key] = diskCachedImage;

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                return new FileImageSource { File = diskCachedImage };
            }

            return new FileImageSource { File = diskCachedImage };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SVG Exception]\t {ex}");

            return new FileImageSource();
        }
        finally
        {
            _iconLock.Release();
        }
    }

    private string? GetImagePathFromDiskCache(string key)
    {
        if (!Directory.Exists(PlatformInfo.IconCacheDirectory))
        {
            return null;
        }

        var filePath = System.IO.Path.Combine(PlatformInfo.IconCacheDirectory, key);

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

        using (var file = File.OpenWrite(Path.Combine(PlatformInfo.IconCacheDirectory, key)))
        {
            await imageStream.CopyToAsync(file).ConfigureAwait(false);
            await file.FlushAsync().ConfigureAwait(false);
        }
    }

    private async Task GenerateImageFromEmbedded(string key, string svgName, Size size, Color? colorOverride = null)
    {
        try
        {
#if DEBUG
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
#endif

            using var skSvg = new SKSvg();

            var devicePpi = (float)PlatformInfo.DevicePpi;

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
        var platformScalingFactor = _platformScalingFactor;

        var scaledCanvas = platformScalingFactor;

        SKRect resize = skSvg.Picture.CullRect;

        if (size != default(Size) && skSvg.Picture.CullRect != default)
        {
            var minSize = (float)Math.Min(size.Width, size.Height);

            scaledCanvas = (minSize / Math.Max(skSvg.Picture.CullRect.Width, skSvg.Picture.CullRect.Height)) *
                           platformScalingFactor;

            if (scaledCanvas > 1.0f)
            {
                scaledCanvas = (float)Math.Ceiling(scaledCanvas);
            }

            resize = new SKRect(0, 0, skSvg.Picture.CullRect.Width * scaledCanvas, skSvg.Picture.CullRect.Height * scaledCanvas);
        }
        else if (skSvg.Picture.CullRect != default)
        {
            resize = new SKRect(0, 0, skSvg.Picture.CullRect.Width * platformScalingFactor, skSvg.Picture.CullRect.Height * platformScalingFactor);
        }

        if (colorOverride == null)
        {
            using var memStream = new MemoryStream();

            skSvg.Picture.ToImage(memStream, SKColors.Empty, SKEncodedImageFormat.Png, 100, scaledCanvas, scaledCanvas, SKColorType.Rgba8888, SKAlphaType.Premul, SKColorSpace.CreateSrgb());

            // var img = SKImage.FromPicture(skSvg.Picture, resize.Size.ToSizeI());
            // using var encoded = img.Encode(SKEncodedImageFormat.Png, 100);
            memStream.Seek(0L, SeekOrigin.Begin);

            // encoded.SaveTo(memStream);
            await SaveIconToDiskCache(key, memStream).ConfigureAwait(false);
            return;
        }

        var imageInfo = new SKImageInfo((int)Math.Ceiling(resize.Width), (int)Math.Ceiling(resize.Height), SKColorType.Rgba8888, SKAlphaType.Premul);

        using var bmp = new SKBitmap(imageInfo);
        using var canvas = new SKCanvas(bmp);

        skSvg.Picture.Draw(SKColor.Empty, scaledCanvas, scaledCanvas, canvas);

        if (colorOverride != default(Color))
        {
            this._paint.Color = colorOverride.ToSKColor();
            canvas.DrawPaint(this._paint);
        }

        canvas.Flush();

        using (var image = SKImage.FromBitmap(bmp))
        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
        using (var stream = data.AsStream(false))
        {
            await this.SaveIconToDiskCache(key, stream).ConfigureAwait(false);
        }
    }

    public void LoadAssembly(Assembly assembly) => EmbeddedResourceLoader.LoadAssembly(assembly);

    public Task ClearCache()
    {
        _resolvedIcons.Clear();

        if (Directory.Exists(PlatformInfo.IconCacheDirectory))
        {
            var files = Directory.GetFiles(PlatformInfo.IconCacheDirectory);

            if (files?.Any() ?? false)
            {
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }

            Directory.Delete(PlatformInfo.IconCacheDirectory, true);
        }

        return Task.CompletedTask;
    }

    public abstract Task<SKBitmap> SKBitmapFromSource(ImageSource imageSource);

    public abstract Task<SKImage> SKImageFromSource(ImageSource imageSource);

    public abstract Task<byte[]> ByteArrayFromSource(ImageSource imageSource);

    public abstract Task<Stream> StreamFromSource(ImageSource imageSource);

    private string CreateIconKey(string svgName, Size size, string additionalCacheKey = "", Color colorOverride = null)
    {
        var key = $"{svgName}_{additionalCacheKey}_{size.Width}_{size.Height}_{colorOverride?.GetHashCode()}".Trim(_underscore);

        key = $"{key}@{_platformScalingFactor}x";

        key = string.Join(_underscore, key.Split(_invalidFilenameChars));

        return $"{key}.png";
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue || !disposing)
        {
            return;
        }

        this._iconLock?.Dispose();
        this._colorspace.Dispose();
        this._paint.Dispose();

        this._disposedValue = true;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
