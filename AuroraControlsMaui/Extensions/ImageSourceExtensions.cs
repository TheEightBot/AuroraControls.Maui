using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Maui.Controls;

namespace AuroraControls;

public static class ImageSourceExtensions
{
    private static readonly object _iconCacheLock = new object();

    private static IIconCache _iconCache;

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
                _iconCache = IPlatformApplication.Current.Services.GetService<IIconCache>();
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
                        imageSource.Dispatcher.Dispatch(() => image.Source = imageSource);
                    }
                });

        return image;
    }

    public static ToolbarItem AsAsyncSourceFor(this Task<FileImageSource> imageSourceTask, ToolbarItem toolbarItem)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && toolbarItem != null)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        imageSource.Dispatcher.Dispatch(() => toolbarItem.IconImageSource = imageSource);
                    }
                });

        return toolbarItem;
    }

    public static MenuItem AsAsyncSourceFor(this Task<FileImageSource> imageSourceTask, MenuItem menuItem)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && menuItem != null)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        imageSource.Dispatcher.Dispatch(() => menuItem.IconImageSource = imageSource);
                    }
                });

        return menuItem;
    }

    public static Page AsAsyncSourceFor(this Task<FileImageSource> imageSourceTask, Page page)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && page != null)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        imageSource.Dispatcher.Dispatch(() => page.IconImageSource = imageSource);
                    }
                });

        return page;
    }

    public static Button AsAsyncSourceFor(this Task<FileImageSource> imageSourceTask, Button button)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && button != null)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        imageSource.Dispatcher.Dispatch(() => button.ImageSource = imageSource);
                    }
                });

        return button;
    }

    public static ImageButton AsAsyncSourceFor(this Task<FileImageSource> imageSourceTask, ImageButton button)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && button != null)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        imageSource.Dispatcher.Dispatch(() => button.Source = imageSource);
                    }
                });

        return button;
    }

    /*
    public static ImageCell AsAsyncSourceFor(this Task<FileImageSource> imageSourceTask, ImageCell imageCell)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && imageCell != null)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        imageSource.Dispatcher.Dispatch(() => imageCell.Image = imageSource);
                    }
                });

        return imageCell;
    }
    */

    public static void AsAsyncSourceFor(this Task<FileImageSource> imageSourceTask, Action<ImageSource> assignImageSource) =>
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted)
                    {
                        var imageSource = await result.ConfigureAwait(false);
                        imageSource.Dispatcher.Dispatch(() => assignImageSource?.Invoke(imageSource));
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
                    element.Dispatcher.Dispatch(() => property.SetValue(element, imageSource));
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
                        lateSource.Dispatcher.Dispatch(() => image.Source = lateSource);
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
                        imageSource.Dispatcher.Dispatch(() => assignSource(imageSource, lateSource));
                    }
                });

        return imageSource;
    }

    public static FileImageSource AsAsyncFileImageSource(this Task<FileImageSource> imageSourceTask)
    {
        var imageSource = new FileImageSource();

        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && imageSource != null)
                    {
                        var lateSource = await result.ConfigureAwait(false);
                        imageSource.Dispatcher.Dispatch(() => imageSource.File = lateSource?.File);
                    }
                });

        return imageSource;
    }

    public static FileImageSource AsAsyncFileImageSource(this Task<FileImageSource> imageSourceTask, FileImageSource updatableSource)
    {
        imageSourceTask
            .ContinueWith(
                async result =>
                {
                    if (result.IsCompleted && updatableSource != null)
                    {
                        var lateSource = await result.ConfigureAwait(false);
                        updatableSource.Dispatcher.Dispatch(() => updatableSource.File = lateSource?.File);
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
                        imageSource.Dispatcher.Dispatch(() => imageSource.Uri = lateSource?.Uri);
                    }
                });

        return imageSource;
    }

    public static Button SetSvgIcon(this Button imageElement, string svgName, double squareSize = 24d, Color? colorOverride = default(Color)) =>
        IconCache
            .FileImageSourceFromSvg(svgName, squareSize, colorOverride: colorOverride)
            .AsAsyncSourceFor(imageElement);

    public static ImageButton SetSvgIcon(this ImageButton imageButton, string svgName, double squareSize = 24d, Color colorOverride = default(Color))
    {
        IconCache
            .FileImageSourceFromSvg(svgName, squareSize, colorOverride: colorOverride)
            .AsAsyncSourceFor(x => imageButton.Source = x);

        return imageButton;
    }

    public static ToolbarItem SetSvgIcon(this ToolbarItem toolbarItem, string svgName, double squareSize = 24d, Color colorOverride = default(Color)) =>
        IconCache
            .FileImageSourceFromSvg(svgName, squareSize, colorOverride: colorOverride)
            .AsAsyncSourceFor(toolbarItem);

    public static Image SetSvgIcon(this Image image, string svgName, double squareSize = 24d, Color colorOverride = default(Color)) =>
        IconCache
            .FileImageSourceFromSvg(svgName, squareSize, colorOverride: colorOverride)
            .AsAsyncSourceFor(image);

    public static Task<SKBitmap> BitmapFromSource(this ImageSource imageSource) => IconCache.SKBitmapFromSource(imageSource);

    public static Task<SKImage> ImageFromSource(this ImageSource imageSource) => IconCache.SKImageFromSource(imageSource);
}
