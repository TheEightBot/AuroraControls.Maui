﻿using System.Reflection;

namespace AuroraControls;

/// <summary>
/// Icon cache interface.
/// </summary>
public interface IIconCache
{
    /// <summary>
    /// Fetches an SVG icon by name.
    /// </summary>
    /// <returns>The Icon as an Image.</returns>
    /// <param name="svgName">The name of the SVG.</param>
    /// <param name="squareSize">The square size of the icon.</param>
    /// <param name="additionalCacheKey">Allows for setting an addiitonal cache key.</param>
    /// <param name="colorOverride">Allows for setting the color of the icon.</param>
    Task<Image> IconFromSvg(string svgName, double squareSize = 22d, string additionalCacheKey = "", Color? colorOverride = default);

    /// <summary>
    /// Fetches an SVG icon by name.
    /// </summary>
    /// <returns>The Icon as an Image.</returns>
    /// <param name="svgName">The name of the SVG.</param>
    /// <param name="size">A Xamarin.Forms.Size representing the desired size of the icon.</param>
    /// <param name="additionalCacheKey">Allows for setting an addiitonal cache key.</param>
    /// <param name="colorOverride">Allows for setting the color of the icon.</param>
    Task<Image> IconFromSvg(string svgName, Size size, string additionalCacheKey = "", Color? colorOverride = default);

    /// <summary>
    /// Fetches an SVG source by name.
    /// </summary>
    /// <returns>The Icon as an ImageSource.</returns>
    /// <param name="svgName">The name of the SVG.</param>
    /// <param name="squareSize">The square size of the icon.</param>
    /// <param name="additionalCacheKey">Allows for setting an addiitonal cache key.</param>
    /// <param name="colorOverride">Allows for setting the color of the icon.</param>
    Task<ImageSource> ImageSourceFromSvg(string svgName, double squareSize = 22d, string additionalCacheKey = "", Color? colorOverride = default);

    /// <summary>
    /// Fetches an SVG icon by name.
    /// </summary>
    /// <returns>The Icon as an ImageSource.</returns>
    /// <param name="svgName">The name of the SVG.</param>
    /// <param name="size">A Xamarin.Forms.Size representing the desired size of the icon.</param>
    /// <param name="additionalCacheKey">Allows for setting an addiitonal cache key.</param>
    /// <param name="colorOverride">Allows for setting the color of the icon.</param>
    Task<ImageSource> ImageSourceFromSvg(string svgName, Size size, string additionalCacheKey = "", Color? colorOverride = default);

    /// <summary>
    /// Fetches an SVG icon by name.
    /// </summary>
    /// <returns>The Icon as an ImageSource.</returns>
    /// <param name="svgName">The name of the SVG.</param>
    /// <param name="svgValue">The SVG Value.</param>
    /// <param name="squareSize">A double representing the desired size of the icon.</param>
    /// <param name="additionalCacheKey">Allows for setting an addiitonal cache key.</param>
    /// <param name="colorOverride">Allows for setting the color of the icon.</param>
    Task<ImageSource> ImageSourceFromRawSvg(string svgName, string svgValue, double squareSize = 22d, string additionalCacheKey = "", Color? colorOverride = default);

    /// <summary>
    /// Fetches an SVG icon by name.
    /// </summary>
    /// <returns>The Icon as an ImageSource.</returns>
    /// <param name="svgName">The name of the SVG.</param>
    /// <param name="svgValue">The SVG Value.</param>
    /// <param name="size">A Xamarin.Forms.Size representing the desired size of the icon.</param>
    /// <param name="additionalCacheKey">Allows for setting an addiitonal cache key.</param>
    /// <param name="colorOverride">Allows for setting the color of the icon.</param>
    Task<ImageSource> ImageSourceFromRawSvg(string svgName, string svgValue, Size size, string additionalCacheKey = "", Color? colorOverride = default);

    /// <summary>
    /// Loads the assembly.
    /// </summary>
    /// <param name="assembly">Assembly.</param>
    void LoadAssembly(Assembly assembly);

    /// <summary>
    /// SKBs the itmap from source.
    /// </summary>
    /// <returns>The itmap from source.</returns>
    /// <param name="imageSource">Image source.</param>
    Task<SKBitmap> SKBitmapFromSource(ImageSource imageSource);

    Task<byte[]> ByteArrayFromSource(ImageSource imageSource);

    Task<Stream> StreamFromSource(ImageSource imageSource);

    /// <summary>
    /// Clears the cache.
    /// </summary>
    Task ClearCache();
}
