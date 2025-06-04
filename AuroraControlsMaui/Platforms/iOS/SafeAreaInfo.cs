using UIKit;

namespace AuroraControls;

public static class SafeAreaInfo
{
    public static UIEdgeInsets GetSafeArea()
    {
        var windowScene = UIApplication.SharedApplication.ConnectedScenes.ToArray().FirstOrDefault() as UIWindowScene;

        return windowScene?.Windows?.FirstOrDefault()?.SafeAreaInsets ?? UIEdgeInsets.Zero;
    }
}
