using SkiaSharp.Views.iOS;

namespace AuroraControls.Platforms.iOS;

public class IconCache : IconCacheBase
{
    public override async Task<SKBitmap> SKBitmapFromSource(ImageSource imageSource)
    {
        if (imageSource == null)
        {
            return null;
        }

        using var image = await imageSource.GetHandler().LoadImageAsync(imageSource).ConfigureAwait(false);

        return image?.ToSKBitmap() ?? new SKBitmap();
    }

    public override async Task<byte[]> ByteArrayFromSource(ImageSource imageSource)
    {
        if (imageSource == null)
        {
            return null;
        }

        using var image = await imageSource.GetHandler().LoadImageAsync(imageSource).ConfigureAwait(false);
        using var data = image.AsPNG();
        byte[] dataBytes = new byte[data.Length];
        System.Runtime.InteropServices.Marshal.Copy(data.Bytes, dataBytes, 0, (int)data.Length);
        return dataBytes;
    }

    public override async Task<Stream> StreamFromSource(ImageSource imageSource)
    {
        if (imageSource == null)
        {
            return null;
        }

        using var image = await imageSource.GetHandler().LoadImageAsync(imageSource).ConfigureAwait(false);
        return image.AsPNG().AsStream();
    }
}
