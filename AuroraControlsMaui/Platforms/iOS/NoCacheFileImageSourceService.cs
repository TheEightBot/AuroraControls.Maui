using UIKit;

namespace AuroraControls;

internal partial class NoCacheFileImageSourceService
{
    public override Task<IImageSourceServiceResult<UIImage>?> GetImageAsync(IImageSource imageSource, float scale = 1, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
