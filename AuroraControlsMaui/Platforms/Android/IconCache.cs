using Android.Graphics;
using Android.Runtime;
using SkiaSharp.Views.Android;

namespace AuroraControls.Platforms.Android;

public class IconCache : IconCacheBase
{
    public override async Task<SKBitmap> SKBitmapFromSource(ImageSource imageSource)
    {
        if (imageSource == null)
        {
            return null;
        }

        using (var image = await imageSource.GetHandler().LoadImageAsync(imageSource, Microsoft.Maui.ApplicationModel.Platform.CurrentActivity).ConfigureAwait(false))
        {
            return image.ToSKBitmap();
        }
    }

    public override async Task<byte[]> ByteArrayFromSource(ImageSource imageSource)
    {
        if (imageSource == null)
        {
            return null;
        }

        using (var image = await imageSource.GetHandler().LoadImageAsync(imageSource, Microsoft.Maui.ApplicationModel.Platform.CurrentActivity).ConfigureAwait(false))
        using (var stream = new MemoryStream())
        {
            await image.CompressAsync(Bitmap.CompressFormat.Png, 100, stream).ConfigureAwait(false);
            return stream.ToArray();
        }
    }

    public override async Task<Stream> StreamFromSource(ImageSource imageSource)
    {
        if (imageSource == null)
        {
            return null;
        }

        using (var image = await imageSource.GetHandler().LoadImageAsync(imageSource, Microsoft.Maui.ApplicationModel.Platform.CurrentActivity))
        {
            var stream = new MemoryStream();
            await image.CompressAsync(Bitmap.CompressFormat.Png, 100, stream).ConfigureAwait(false);
            return stream;
        }
    }
}
