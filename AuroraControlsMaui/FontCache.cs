using Topten.RichTextKit;

namespace AuroraControls;

/// <summary>
/// Font cache.
/// Ported from https://github.com/toptensoftware/RichTextKit/issues/7.
/// </summary>
public class FontCache : FontMapper
{
    /// <summary>
    /// Gets a dictionary containing a string representing the resource name and the SKTypeface resource.
    /// </summary>
    public Dictionary<string, List<SKTypeface>> RegisteredFonts { get; } = new();

    public static FontCache Instance { get; } = new();

    private FontCache() => Default = this;

    public void Add(string embeddedResourceName, string shortNameOverride = null)
    {
        using var stream = EmbeddedResourceLoader.Load(embeddedResourceName);
        this.Add(SKTypeface.FromStream(stream), shortNameOverride);
    }

    public void Add(SKTypeface typeface, string shortNameOverride = null)
    {
        string? familyName = shortNameOverride ?? typeface.FamilyName;

        if (!RegisteredFonts.ContainsKey(familyName))
        {
            RegisteredFonts.Add(familyName, new List<SKTypeface>());
        }

        RegisteredFonts[familyName].Add(typeface);
    }

    public void Remove(SKTypeface typeface, string shortNameOverride = null)
    {
        string? familyName = shortNameOverride ?? typeface.FamilyName;

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
        string? qualifiedName = style.FontFamily;
        if (style.FontItalic)
        {
            qualifiedName += "-Italic";
        }

        // Look up custom fonts
        List<SKTypeface> listFonts;

        if (qualifiedName is not null && RegisteredFonts.TryGetValue(qualifiedName, out listFonts))
        {
            // Find the closest weight
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
            bool fontExists = await FileSystem.Current.AppPackageFileExistsAsync(fontDescriptor.Filename).ConfigureAwait(false);

            if (!fontExists)
            {
                continue;
            }

            await using var asset = await FileSystem.Current.OpenAppPackageFileAsync(fontDescriptor.Filename).ConfigureAwait(false);

            FontCache.Instance.Add(SKTypeface.FromStream(asset), fontDescriptor.Alias);
        }
    }
}
