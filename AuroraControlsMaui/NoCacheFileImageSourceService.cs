using Microsoft.Extensions.Logging;

namespace AuroraControls;

internal partial class NoCacheFileImageSourceService : ImageSourceService, IImageSourceService<NoCacheFileImageSource>
{
    public NoCacheFileImageSourceService()
        : this(null)
    {
    }

    public NoCacheFileImageSourceService(ILogger<NoCacheFileImageSourceService>? logger = null)
        : base(logger)
    {
    }
}
