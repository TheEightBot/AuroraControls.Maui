using System.Linq.Expressions;
using System.Reflection;

namespace AuroraControls;

public static class ImageSourceExtensions
{
    private static readonly object _iconCacheLock = new();

    private static HttpClient _httpClient = new HttpClient();

    private static IIconCache? _iconCache;

    private static IIconCache IconCache
    {
        get
        {
            if (_iconCache is not null)
            {
                return _iconCache;
            }

            lock (_iconCacheLock)
            {
                _iconCache = IPlatformApplication.Current?.Services.GetService<IIconCache>();
            }

            return _iconCache;
        }
    }

    public static Image AsAsyncSourceFor<T>(this Task<T> imageSourceTask, Image image)
        where T : ImageSource
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && image != null)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => image.Source = imageSource);
                    }
                });

        return image;
    }

    public static ToolbarItem AsAsyncSourceFor(this Task<ImageSource> imageSourceTask, ToolbarItem toolbarItem)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && toolbarItem != null)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => toolbarItem.IconImageSource = imageSource);
                    }
                });

        return toolbarItem;
    }

    public static MenuItem AsAsyncSourceFor(this Task<ImageSource> imageSourceTask, MenuItem menuItem)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && menuItem != null)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => menuItem.IconImageSource = imageSource);
                    }
                });

        return menuItem;
    }

    public static Page AsAsyncSourceFor(this Task<ImageSource> imageSourceTask, Page page)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && page != null)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => page.IconImageSource = imageSource);
                    }
                });

        return page;
    }

    public static Button AsAsyncSourceFor(this Task<ImageSource> imageSourceTask, Button button)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && button != null)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => button.ImageSource = imageSource);
                    }
                });

        return button;
    }

    public static ImageButton AsAsyncSourceFor(this Task<ImageSource> imageSourceTask, ImageButton button)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && button != null)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => button.Source = imageSource);
                    }
                });

        return button;
    }

    public static ImageCell AsAsyncSourceFor(this Task<ImageSource> imageSourceTask, ImageCell imageCell)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && imageCell != null)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => imageCell.ImageSource = imageSource);
                    }
                });

        return imageCell;
    }

    public static void AsAsyncSourceFor(this Task<ImageSource> imageSourceTask, Action<ImageSource> assignImageSource) =>
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => assignImageSource?.Invoke(imageSource));
                    }
                });

    public static TElement AsAsyncSourceFor<TElement>(this Task<ImageSource> imageSourceTask, TElement element, Expression<Func<TElement, ImageSource>> source)
        where TElement : VisualElement
    {
        imageSourceTask
            .ContinueWith(
            async result =>
            {
                if (result.IsCompleted && element != null)
                {
                    var imageSource = await result.ConfigureAwait(false);

                    var member = (MemberExpression)source.Body;
                    var property = member.Member as PropertyInfo;
                    MainThread.BeginInvokeOnMainThread(() => property.SetValue(element, imageSource));
                }
            });

        return element;
    }

    public static Image AsAsyncSourceFor<T>(this Task<T> imageSourceTask)
        where T : ImageSource
    {
        var image = new Image();

        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && image != null)
                    {
                        var lateSource = await result.ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => image.Source = lateSource);
                    }
                });

        return image;
    }

    public static T AsAsyncSourceFor<T>(this Task<T> imageSourceTask, Action<T, T> assignSource)
        where T : ImageSource, new()
    {
        var imageSource = new T();

        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && imageSource != null)
                    {
                        var lateSource = await result.ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => assignSource(imageSource, lateSource));
                    }
                });

        return imageSource;
    }

    public static FileImageSource AsAsyncImageSource(this Task<FileImageSource> imageSourceTask)
    {
        var imageSource = new FileImageSource();

        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && imageSource != null)
                    {
                        var lateSource = await result.ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => imageSource.File = lateSource?.File);
                    }
                });

        return imageSource;
    }

    public static ImageSource AsAsyncImageSource(this Task<FileImageSource> imageSourceTask, FileImageSource updatableSource)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && updatableSource != null)
                    {
                        var lateSource = await result.ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => updatableSource.File = lateSource?.File);
                    }
                });

        return updatableSource;
    }

    public static UriImageSource AsAsyncUriImageSource(this Task<UriImageSource> imageSourceTask)
    {
        var imageSource = new UriImageSource();

        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && imageSource != null)
                    {
                        var lateSource = await result.ConfigureAwait(false);
                        MainThread.BeginInvokeOnMainThread(() => imageSource.Uri = lateSource?.Uri);
                    }
                });

        return imageSource;
    }

    public static Button SetSvgIcon(this Button imageElement, string svgName, double squareSize = 24d, Color? colorOverride = null) =>
        IconCache
            .ImageSourceFromSvg(svgName, squareSize, colorOverride: colorOverride, hardwareAcceleration: imageElement.SupportsHardwareAcceleration())
            .AsAsyncSourceFor(imageElement);

    public static ImageButton SetSvgIcon(this ImageButton imageButton, string svgName, double squareSize = 24d, Color? colorOverride = null)
    {
        IconCache
            .ImageSourceFromSvg(svgName, squareSize, colorOverride: colorOverride, hardwareAcceleration: imageButton.SupportsHardwareAcceleration())
            .AsAsyncSourceFor(imageButton);

        return imageButton;
    }

    public static ToolbarItem SetSvgIcon(this ToolbarItem toolbarItem, string svgName, double squareSize = 24d, Color? colorOverride = null) =>
        IconCache
            .ImageSourceFromSvg(svgName, squareSize, colorOverride: colorOverride, hardwareAcceleration: toolbarItem.SupportsHardwareAcceleration())
            .AsAsyncSourceFor(toolbarItem);

    public static MenuItem SetSvgIcon(this MenuItem menuItem, string svgName, double squareSize = 24d, Color? colorOverride = null) =>
        IconCache
            .ImageSourceFromSvg(svgName, squareSize, colorOverride: colorOverride, hardwareAcceleration: menuItem.SupportsHardwareAcceleration())
            .AsAsyncSourceFor(menuItem);

    public static Image SetSvgIcon(this Image image, string svgName, double squareSize = 24d, Color? colorOverride = null) =>
        IconCache
            .ImageSourceFromSvg(svgName, squareSize, colorOverride: colorOverride, hardwareAcceleration: image.SupportsHardwareAcceleration())
            .AsAsyncSourceFor(image);

    public static Page SetSvgIcon(this Page page, string svgName, double squareSize = 24d, Color? colorOverride = null) =>
        IconCache
            .ImageSourceFromSvg(svgName, squareSize, colorOverride: colorOverride, hardwareAcceleration: page.SupportsHardwareAcceleration())
            .AsAsyncSourceFor(page);

    public static async Task<SKBitmap?> ToSKBitmapAsync(this ImageSource imageSource)
    {
        if (imageSource == null)
        {
            return null;
        }

        Stream? stream = null;

        switch (imageSource)
        {
            case FileImageSource fileImageSource:
                stream = File.OpenRead(fileImageSource.File);
                break;
            case StreamImageSource streamImageSource:
                stream = await streamImageSource.Stream.Invoke(System.Threading.CancellationToken.None).ConfigureAwait(false);
                break;
            case UriImageSource uriImageSource:

                stream = await _httpClient.GetStreamAsync(uriImageSource.Uri).ConfigureAwait(false);
                break;
        }

        if (stream == null)
        {
            return null;
        }

        await using (stream)
        {
            return SKBitmap.Decode(stream);
        }
    }

    private static bool SupportsHardwareAcceleration(this Element view)
    {
        bool supportsHardwareAcceleration = true;
/*
#if ANDROID
        supportsHardwareAcceleration =
            view.Handler?.PlatformView is Android.Views.View
            {
                IsHardwareAccelerated: true,
            };
#endif
*/
        return supportsHardwareAcceleration;
    }
}
