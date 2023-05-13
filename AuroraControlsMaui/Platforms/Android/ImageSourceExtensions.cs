using Microsoft.Maui.Controls.Compatibility.Platform.Android;

namespace AuroraControls.Platforms.Android;

public static class ImageSourceExtensions
{
    public static IImageSourceHandler GetHandler(this ImageSource source)
    {
        // Image source handler to return
        IImageSourceHandler returnValue = null;

        // check the specific source type and return the correct image source handler
        if (source is UriImageSource)
        {
            returnValue = new ImageLoaderSourceHandler();
        }
        else if (source is FileImageSource)
        {
            returnValue = new FileImageSourceHandler();
        }
        else if (source is StreamImageSource sis)
        {
            returnValue = new StreamImagesourceHandler();
        }

        return returnValue;
    }
}