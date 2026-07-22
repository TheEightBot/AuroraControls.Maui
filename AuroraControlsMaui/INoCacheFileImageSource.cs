namespace AuroraControls;

internal interface INoCacheFileImageSource : IImageSource
{
    string File { get; }

    /// <summary>
    /// Gets an optional callback that re-renders the source image to disk and returns the file path.
    /// Invoked by platform image services when the cached file has been deleted
    /// (e.g. the OS trimmed the app cache directory) so the icon can self-heal
    /// instead of rendering blank until app restart.
    /// </summary>
    Func<Task<string?>>? Regenerate { get; }
}
