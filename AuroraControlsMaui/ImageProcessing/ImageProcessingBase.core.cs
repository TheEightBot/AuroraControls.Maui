namespace AuroraControls.ImageProcessing;

/// <summary>
/// Image processing base class.
/// </summary>
public abstract class ImageProcessingBase : BindableObject
{
    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The key for processor.</value>
    public abstract string Key { get; }
}
