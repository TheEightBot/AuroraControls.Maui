namespace AuroraControls;

internal interface INoCacheFileImageSource : IImageSource
{
    string File { get; }

    bool HardwareAcceleration { get; }
}
