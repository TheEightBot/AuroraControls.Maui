using System.Reflection;

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
    /// <param name="hardwareAcceleration">Allows for setting the hardware acceleration.</param>
    Task<Image> IconFromSvg(string svgName, double squareSize = 22d, string additionalCacheKey = "", Color? colorOverride = default, bool hardwareAcceleration = true);

    /// <summary>
    /// Fetches an SVG icon by name.
    /// </summary>
    /// <returns>The Icon as an Image.</returns>
    /// <param name="svgName">The name of the SVG.</param>
    /// <param name="size">A Size representing the desired size of the icon.</param>
    /// <param name="additionalCacheKey">Allows for setting an addiitonal cache key.</param>
    /// <param name="colorOverride">Allows for setting the color of the icon.</param>
    /// <param name="hardwareAcceleration">Allows for setting the hardware acceleration.</param>
    Task<Image> IconFromSvg(string svgName, Size size, string additionalCacheKey = "", Color? colorOverride = default, bool hardwareAcceleration = true);

    /// <summary>
    /// Fetches an SVG source by name.
    /// </summary>
    /// <returns>The Icon as an ImageSource.</returns>
    /// <param name="svgName">The name of the SVG.</param>
    /// <param name="squareSize">The square size of the icon.</param>
    /// <param name="additionalCacheKey">Allows for setting an addiitonal cache key.</param>
    /// <param name="colorOverride">Allows for setting the color of the icon.</param>
    /// <param name="hardwareAcceleration">Allows for setting the hardware acceleration.</param>
    Task<ImageSource> ImageSourceFromSvg(string svgName, double squareSize = 22d, string additionalCacheKey = "", Color? colorOverride = default, bool hardwareAcceleration = true);

    /// <summary>
    /// Fetches an SVG icon by name.
    /// </summary>
    /// <returns>The Icon as an ImageSource.</returns>
    /// <param name="svgName">The name of the SVG.</param>
    /// <param name="size">A Size representing the desired size of the icon.</param>
    /// <param name="additionalCacheKey">Allows for setting an addiitonal cache key.</param>
    /// <param name="colorOverride">Allows for setting the color of the icon.</param>
    /// <param name="hardwareAcceleration">Allows for setting the hardware acceleration.</param>
    Task<ImageSource> ImageSourceFromSvg(string svgName, Size size, string additionalCacheKey = "", Color? colorOverride = default, bool hardwareAcceleration = true);

    /// <summary>
    /// Fetches an SVG icon by name.
    /// </summary>
    /// <returns>The Icon as an ImageSource.</returns>
    /// <param name="svgName">The name of the SVG.</param>
    /// <param name="svgValue">The SVG Value.</param>
    /// <param name="squareSize">A double representing the desired size of the icon.</param>
    /// <param name="additionalCacheKey">Allows for setting an addiitonal cache key.</param>
    /// <param name="colorOverride">Allows for setting the color of the icon.</param>
    /// <param name="hardwareAcceleration">Allows for setting the hardware acceleration.</param>
    Task<ImageSource> ImageSourceFromRawSvg(string svgName, string svgValue, double squareSize = 22d, string additionalCacheKey = "", Color? colorOverride = default, bool hardwareAcceleration = true);

    /// <summary>
    /// Fetches an SVG icon by name.
    /// </summary>
    /// <returns>The Icon as an ImageSource.</returns>
    /// <param name="svgName">The name of the SVG.</param>
    /// <param name="svgValue">The SVG Value.</param>
    /// <param name="size">A Size representing the desired size of the icon.</param>
    /// <param name="additionalCacheKey">Allows for setting an addiitonal cache key.</param>
    /// <param name="colorOverride">Allows for setting the color of the icon.</param>
    /// <param name="hardwareAcceleration">Allows for setting the hardware acceleration.</param>
    Task<ImageSource> ImageSourceFromRawSvg(string svgName, string svgValue, Size size, string additionalCacheKey = "", Color? colorOverride = default, bool hardwareAcceleration = true);

    /// <summary>
    /// Loads the assembly.
    /// </summary>
    /// <param name="assembly">Assembly.</param>
    void LoadAssembly(Assembly assembly);

    /// <summary>
    /// Clears the cache.
    /// </summary>
    Task ClearCache();
}
