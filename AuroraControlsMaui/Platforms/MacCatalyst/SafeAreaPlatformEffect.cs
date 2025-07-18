using AuroraControls.Effects;
using Foundation;
using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace AuroraControls;

public class SafeAreaPlatformEffect : PlatformEffect
{
    private Thickness _initialMargin;
    private NSObject? _orientationDidChangeNotificationObserver;

    private new View Element => (View)base.Element;

    private bool IsEligibleToConsumeEffect
        => Element != null
           && UIDevice.CurrentDevice.CheckSystemVersion(11, 0)
           && UIApplication.SharedApplication.Windows.Any();

    protected override void OnAttached()
    {
        if (!IsEligibleToConsumeEffect)
        {
            return;
        }

        _orientationDidChangeNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            UIDevice.OrientationDidChangeNotification, _ => UpdateInsets());

        _initialMargin = Element.Margin;
        UpdateInsets();
    }

    protected override void OnDetached()
    {
        if (!IsEligibleToConsumeEffect)
        {
            return;
        }

        if (_orientationDidChangeNotificationObserver != null)
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(_orientationDidChangeNotificationObserver);
            _orientationDidChangeNotificationObserver?.Dispose();
            _orientationDidChangeNotificationObserver = null;
        }

        Element.Margin = _initialMargin;
    }

    private void UpdateInsets()
    {
        // Double-check eligability something might've changed in regard to the windows
        if (!IsEligibleToConsumeEffect)
        {
            return;
        }

        var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;
        var safeArea = SafeAreaEffect.GetSafeArea(Element);

        Element.Margin = new Thickness(
            _initialMargin.Left + CalculateInsets(insets.Left, safeArea.Left),
            _initialMargin.Top + CalculateInsets(insets.Top, safeArea.Top),
            _initialMargin.Right + CalculateInsets(insets.Right, safeArea.Right),
            _initialMargin.Bottom + CalculateInsets(insets.Bottom, safeArea.Bottom));
    }

    private double CalculateInsets(double insetsComponent, bool shouldUseInsetsComponent) => shouldUseInsetsComponent ? insetsComponent : 0;
}
