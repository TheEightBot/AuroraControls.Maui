using System;
namespace AuroraControls;

/// <summary>
/// Font cache.
/// </summary>
public static class FontCache
{
    private static SKTypeface[] _registeredTypefaces;
    public static SKTypeface[] RegisteredTypefaces
    {
        get
        {
            if (_registeredTypefaces != null)
            {
                return _registeredTypefaces;
            }

            _registeredTypefaces = _fontDictionary?.Values?.ToArray();
            return _registeredTypefaces;
        }
    }

    /// <summary>
    /// A dictionary containing a string representing the resource name and the SKTypeface resource.
    /// </summary>
    readonly static Dictionary<string, SKTypeface> _fontDictionary = new Dictionary<string, SKTypeface>();

    public static SKTypeface Add(string resourceName, string shortNameOverride = null)
    {
        using (var stream = EmbeddedResourceLoader.Load(resourceName))
        {
            var typeface = SKTypeface.FromStream(stream);
            _fontDictionary.Add(!string.IsNullOrEmpty(shortNameOverride) ? shortNameOverride : resourceName, typeface);
        }

        _registeredTypefaces = null;

        return _fontDictionary[resourceName];
    }

    public static void Remove(string resourceName, string shortNameOverride = null)
    {
        var cacheName = !string.IsNullOrEmpty(shortNameOverride) ? shortNameOverride : resourceName;

        if (_fontDictionary.TryGetValue(cacheName, out SKTypeface value))
        {
            var typeFace = value;

            _fontDictionary.Remove(cacheName);

            typeFace?.Dispose();
            typeFace = null;

            _registeredTypefaces = null;
        }
    }

    /// <summary>
    /// Gets the typeface.
    /// </summary>
    /// <returns>The typeface.</returns>
    /// <param name="resourceName">Resource name.</param>
    /// <param name="shortNameOverride">Optional short name provided for ease of use</param>
    public static SKTypeface GetTypeface(string resourceName, string shortNameOverride = null)
    {
        var cacheName = !string.IsNullOrEmpty(shortNameOverride) ? shortNameOverride : resourceName;

        if (_fontDictionary.TryGetValue(cacheName, out SKTypeface value))
        {
            return value;
        }

        return Add(resourceName, shortNameOverride);
    }
}