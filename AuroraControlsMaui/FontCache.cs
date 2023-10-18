using System.Collections.Generic;
using System.Windows.Markup;
using Topten.RichTextKit;

namespace AuroraControls;

/// <summary>
/// Font cache.
/// Ported from https://github.com/toptensoftware/RichTextKit/issues/7.
/// </summary>
public class FontCache : Topten.RichTextKit.FontMapper
{
    /// <summary>
    /// Gets a dictionary containing a string representing the resource name and the SKTypeface resource.
    /// </summary>
    public Dictionary<string, List<SKTypeface>> RegisteredFonts { get; } = new Dictionary<string, List<SKTypeface>>();

    public static FontCache Instance { get; } = new();

    private FontCache()
    {
        FontMapper.Default = this;
    }

    public void Add(string embeddedResourceName, string shortNameOverride = null)
    {
        using (var stream = EmbeddedResourceLoader.Load(embeddedResourceName))
        {
            Add(SKTypeface.FromStream(stream), shortNameOverride);
        }
    }

    public void Add(SKTypeface typeface, string shortNameOverride = null)
    {
        var familyName = shortNameOverride ?? typeface.FamilyName;

        if (!RegisteredFonts.ContainsKey(familyName))
        {
            RegisteredFonts.Add(familyName, new List<SKTypeface>());
        }

        RegisteredFonts[familyName].Add(typeface);
    }

    public void Remove(SKTypeface typeface, string shortNameOverride = null)
    {
        var familyName = shortNameOverride ?? typeface.FamilyName;

        if (RegisteredFonts.TryGetValue(familyName, out var values))
        {
            values.Remove(typeface);

            typeface?.Dispose();
        }
    }

    public SKTypeface TypefaceFromFontFamily(string fontFamily)
    {
        var style =
            new Topten.RichTextKit.Style
            {
                FontFamily = fontFamily,
            };

        return TypefaceFromStyle(style, true);
    }

    /// <summary>
    /// Map a RichTextKit style to an SKTypeface.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="ignoreFontVariants">True to ignore variants (super/subscript).</param>
    /// <returns>The mapped typeface.</returns>
    public override SKTypeface TypefaceFromStyle(IStyle style, bool ignoreFontVariants)
    {
        // Work out the qualified name
        var qualifiedName = style.FontFamily;
        if (style.FontItalic)
        {
            qualifiedName += "-Italic";
        }

        // Look up custom fonts
        List<SKTypeface> listFonts;

        if (qualifiedName is not null && RegisteredFonts.TryGetValue(qualifiedName, out listFonts))
        {
            // Find closest weight
            return listFonts.MinBy(x => Math.Abs(x.FontWeight - style.FontWeight));
        }

        // Do default mapping
        return base.TypefaceFromStyle(style, ignoreFontVariants);
    }
}

public static class FontCacheExtensions
{
    public static async void AddToAuroraFontCache(this IFontCollection fontDescriptors)
    {
        foreach (var fontDescriptor in fontDescriptors)
        {
            using (var asset = await FileSystem.OpenAppPackageFileAsync(fontDescriptor.Filename).ConfigureAwait(false))
            {
                FontCache.Instance.Add(SKTypeface.FromStream(asset), fontDescriptor.Alias);
            }
        }
    }
}
