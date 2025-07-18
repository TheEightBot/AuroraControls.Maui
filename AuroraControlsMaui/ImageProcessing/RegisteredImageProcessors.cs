namespace AuroraControls.ImageProcessing;

/// <summary>
/// Registered image processors.
/// </summary>
public static class RegisteredImageProcessors
{
    /// <summary>
    /// A static dictionary of registered Image processors and their adjacent keys.
    /// </summary>
    private static readonly Dictionary<string, IImageProcessor> _imageProcessors = new();

    static RegisteredImageProcessors()
    {
        SetProcessor(nameof(Grayscale), new Grayscale());
        SetProcessor(nameof(Sepia), new Sepia());
        SetProcessor(nameof(Invert), new Invert());
        SetProcessor(nameof(Grayscale), new Grayscale());
        SetProcessor(nameof(Blur), new Blur());
        SetProcessor(nameof(Circular), new Circular());
        SetProcessor(nameof(Scale), new Scale());
        SetProcessor(nameof(ResizeImage), new ResizeImage());
    }

    /// <summary>
    /// Gets the processor by key.
    /// </summary>
    /// <returns>result as an IImageProcessor.</returns>
    /// <param name="key">Key/name of processor.</param>
    public static IImageProcessor GetProcessor(string key) => _imageProcessors.ContainsKey(key) ? _imageProcessors[key] : null;

    /// <summary>
    /// Sets the processor.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="imageProcessor">Image processor.</param>
    public static void SetProcessor(string key, IImageProcessor imageProcessor) => _imageProcessors[key] = imageProcessor;
}
