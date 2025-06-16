using UIKit;

namespace AuroraControls;

public static class UIViewExtensions
{
    public static UIEdgeInsets ToNative(this InsetsF inset) => new(inset.Top, inset.Left, inset.Bottom, inset.Right);
}
