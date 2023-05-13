using System.Reflection;
using System.Text;
using System.Xml;
using Svg.Skia;

namespace AuroraControls;

public abstract class IconCacheBase : IIconCache, IDisposable
{
    private readonly char[] _invalidFilenameChars;

    private readonly SemaphoreSlim _iconLock = new SemaphoreSlim(1, 1);

    private readonly Dictionary<string, string> _resolvedIcons = new Dictionary<string, string>();

    private readonly SKPaint _paint = new SKPaint { BlendMode = SKBlendMode.SrcATop, Style = SKPaintStyle.Fill };

    private bool _disposedValue;

    public IconCacheBase()
    {
        _invalidFilenameChars = System.IO.Path.GetInvalidFileNameChars();
    }

    public Task<Image> IconFromSvg(string svgName, double squareSize = 22d, string additionalCacheKey = "", Color colorOverride = default(Color))
    {
        return IconFromSvg(svgName, new Size(squareSize, squareSize), additionalCacheKey, colorOverride);
    }

    public async Task<Image> IconFromSvg(string svgName, Size size, string additionalCacheKey = "", Color colorOverride = default(Color))
    {
        return
            new Image()
            {
                Source = await SourceFromSvg(svgName, size, additionalCacheKey, colorOverride),
            };
    }

    public Task<ImageSource> SourceFromSvg(string svgName, double squareSize = 22d, string additionalCacheKey = "", Color colorOverride = default(Color))
    {
        return SourceFromSvg(svgName, new Size(squareSize, squareSize), additionalCacheKey, colorOverride);
    }

    public async Task<ImageSource> SourceFromSvg(string svgName, Size size, string additionalCacheKey = "", Color colorOverride = default(Color))
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
        finally
        {
            _iconLock.Release();
        }
    }

    public Task<ImageSource> SourceFromRawSvg(string svgName, string svgValue, double squareSize = 22d, string additionalCacheKey = "", Color colorOverride = default(Color))
    {
        return SourceFromRawSvg(svgName, svgValue, new Size(squareSize, squareSize), additionalCacheKey, colorOverride);
    }

    public async Task<ImageSource> SourceFromRawSvg(string svgName, string svgValue, Size size, string additionalCacheKey = "", Color colorOverride = default(Color))
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

    public Task<FileImageSource> FileImageSourceFromSvg(string svgName, double squareSize = 22d, string additionalCacheKey = "", Color colorOverride = default(Color))
    {
        return FileImageSourceFromSvg(svgName, new Size(squareSize, squareSize), additionalCacheKey, colorOverride);
    }

    public async Task<FileImageSource> FileImageSourceFromSvg(string svgName, Size size, string additionalCacheKey = "", Color colorOverride = default(Color))
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
        finally
        {
            _iconLock.Release();
        }
    }

    public Image SourceFromRawSvg(string svgName, Size size, int quality = 80)
    {
        try
        {
            var scale = PlatformInfo.ScalingFactor;
            var devicePpi = (float)PlatformInfo.DevicePpi;

            var width = (int)Math.Floor(size.Width * scale);
            var height = (int)Math.Floor(size.Height * scale);

            return new Image
            {
                Source = new StreamImageSource
                {
                    Stream =
                        _ =>
                        {
                            var skSvg = new SKSvg();

                            using (var stream = EmbeddedResourceLoader.Load(svgName))
                            {
                                skSvg.Load(stream);
                            }

                            using (var image = SKImage.FromPicture(skSvg.Picture, skSvg.Picture.CullRect.Size.ToSizeI()))
                            {
                                return Task.FromResult(image.Encode(SKEncodedImageFormat.Png, quality).AsStream(true));
                            }
                        },
                },
            };
        }
        catch (Exception)
        {
            return default(Image);
        }
    }

    private string GetImagePathFromDiskCache(string key)
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

    private async Task GenerateImageFromEmbedded(string key, string svgName, Size size, Color colorOverride)
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

            var scale = (float)PlatformInfo.ScalingFactor;

            // TODO: Manage resizing...
            /*
            if (size != default(Size) && skSvg.Picture.CullRect != default)
            {
                var minSize = (float)Math.Min(size.Width, size.Height);

                var scaledCanvas = minSize / Math.Max(skSvg.Picture.CullRect.Width, skSvg.Picture.CullRect.Height);

                skSvg.Picture.CullRect = new SKSize(skSvg.Picture.CullRect.Width * scaledCanvas * scale, skSvg.Picture.CullRect.Height * scaledCanvas * scale);
            }
            else if (skSvg.Picture.CullRect != default)
            {
                skSvg.Picture.CullRect = new SKSize(skSvg.Picture.CullRect.Width * scale, skSvg.Picture.CullRect.Height * scale);
            }
            */

            var imageInfo = new SKImageInfo((int)skSvg.Picture.CullRect.Width, (int)skSvg.Picture.CullRect.Height, SKColorType.Rgba8888, SKAlphaType.Premul);

            using (var bmp = new SKBitmap(imageInfo))
            using (var canvas = new SKCanvas(bmp))
            {
                canvas.Clear();

                canvas.DrawPicture(skSvg.Picture);

                if (colorOverride != default(Color))
                {
                    _paint.Color = colorOverride.ToSKColor();
                    canvas.DrawPaint(_paint);
                }

                canvas.Flush();

                using (var image = SKImage.FromBitmap(bmp))
                using (var data = image.Encode())
                using (var stream = data.AsStream(false))
                {
                    await SaveIconToDiskCache(key, stream).ConfigureAwait(false);
                }
            }

#if DEBUG
            stopWatch.Stop();

            System.Diagnostics.Debug.WriteLine(
                "Generating image \"{0}\" from SvG took {1}ms",
                svgName, stopWatch.ElapsedMilliseconds);
#endif
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("[SVG Exception]" + Environment.NewLine + ex.ToString());
        }
    }

    private async Task GenerateImageFromRaw(string key, string svgValue, Size size, Color colorOverride)
    {
        try
        {
#if DEBUG
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
#endif

            using var skSvg = new SKSvg();

            var devicePpi = (float)PlatformInfo.DevicePpi;

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(svgValue)))
            {
                skSvg.Load(ms);
            }

            var scale = (float)PlatformInfo.ScalingFactor;

            // TODO: Figure out resizing
            /*
            if (size != default(Size) && skSvg.Picture.CullRect != default)
            {
                var minSize = (float)Math.Min(size.Width, size.Height);

                var scaledCanvas = minSize / Math.Max(skSvg.Picture.CullRect.Width, skSvg.Picture.CullRect.Height);

                skSvg.Picture.CullRect = new SKSize(skSvg.Picture.CullRect.Width * scaledCanvas * scale, skSvg.Picture.CullRect.Height * scaledCanvas * scale);
            }
            else if (skSvg.Picture.CullRect != default)
            {
                skSvg.Picture.CullRect = new SKSize(skSvg.Picture.CullRect.Width * scale, skSvg.Picture.CullRect.Height * scale);
            }
            */

            var imageInfo = new SKImageInfo((int)skSvg.Picture.CullRect.Width, (int)skSvg.Picture.CullRect.Height, SKColorType.Rgba8888, SKAlphaType.Premul);

            using (var bmp = new SKBitmap(imageInfo))
            using (var canvas = new SKCanvas(bmp))
            {
                canvas.Clear();

                canvas.DrawPicture(skSvg.Picture);

                if (colorOverride != default(Color))
                {
                    _paint.Color = colorOverride.ToSKColor();
                    canvas.DrawPaint(_paint);
                }

                canvas.Flush();

                using (var image = SKImage.FromBitmap(bmp))
                using (var data = image.Encode())
                using (var stream = data.AsStream(false))
                {
                    await SaveIconToDiskCache(key, stream).ConfigureAwait(false);
                }
            }

#if DEBUG
            stopWatch.Stop();

            System.Diagnostics.Debug.WriteLine(
                "Generating image \"{0}\" from SvG took {1}ms",
                key, stopWatch.ElapsedMilliseconds);
#endif
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("[SVG Exception]" + Environment.NewLine + ex.ToString());
        }
    }

    public void LoadAssembly(Assembly assembly)
    {
        EmbeddedResourceLoader.LoadAssembly(assembly);
    }

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

    public abstract Task<byte[]> ByteArrayFromSource(ImageSource imageSource);

    public abstract Task<Stream> StreamFromSource(ImageSource imageSource);

    private string CreateIconKey(string svgName, Size size, string additionalCacheKey = "", Color colorOverride = default(Color))
    {
        var key = string.Format("{0}_{1}_{2}_{3}_{4}", svgName, additionalCacheKey, size.Width, size.Height, colorOverride.GetHashCode());

        var scale = PlatformInfo.ScalingFactor;

        if (scale > 1.01d)
        {
            key = $"{key}@{scale}x";
        }

        key = string.Join("_", key.Split(_invalidFilenameChars));

        return $"{key}.png";
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _iconLock?.Dispose();
                _paint.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}