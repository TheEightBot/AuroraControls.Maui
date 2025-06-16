using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

#if IOS || MACCATALYST
using CoreGraphics;
using UIKit;
#elif ANDROID
using Android.Views;
using AndroidX.AppCompat.Widget;
#endif

namespace AuroraControls;

public class StyledInputLayoutHandler : ContentViewHandler, IHavePlatformUnderlayDrawable
{
    public static PropertyMapper StyledInputLayoutMapper =
        new PropertyMapper<StyledInputLayout, StyledInputLayoutHandler>(ContentViewHandler.Mapper)
        {
            [nameof(IContentView.Content)] = MapStyledInputContent,
            [nameof(IView.Background)] = MapStyledInputLayoutBackground,
            [nameof(IView.Opacity)] = MapStyledInputLayoutOpacity,
            [nameof(VisualElement.BackgroundColor)] = MapStyledInputLayoutBackground,
            [nameof(IUnderlayDrawable.Placeholder)] = MapStyledInputLayoutPlaceholder,
            [nameof(IUnderlayDrawable.PlaceholderColor)] = MapStyledInputLayoutPlaceholder,
            [nameof(IUnderlayDrawable.ActivePlaceholderFontSize)] = MapStyledInputLayoutActivePlaceholderFontSize,
            [nameof(IUnderlayDrawable.BorderSize)] = MapStyledInputLayoutBorderSize,
            [nameof(IUnderlayDrawable.BorderStyle)] = MapStyledInputLayoutContainerBorderStyle,
            [nameof(IUnderlayDrawable.InternalMargin)] = MapStyledInputLayoutInternalMargin,
            [nameof(IUnderlayDrawable.FocusAnimationPercentage)] = MapFocusAnimationPercentage,
            [nameof(IUnderlayDrawable.HasValueAnimationPercentage)] = MapHasValueAnimationPercentage,
            [nameof(IUnderlayDrawable.Command)] = MapCommand,
        };

    public bool PreviousHasValue { get; set; }

    public PlatformUnderlayDrawable PlatformUnderlayDrawable { get; private set; }

    public StyledInputLayoutHandler()
        : base(StyledInputLayoutMapper)
    {
    }

#if IOS || MACCATALYST
    protected override void ConnectHandler(Microsoft.Maui.Platform.ContentView platformView)
    {
        base.ConnectHandler(platformView);

        PlatformUnderlayDrawable = new PlatformUnderlayDrawable();
        PlatformUnderlayDrawable.ConnectHandler(platformView, VirtualView);
    }

    protected override void DisconnectHandler(Microsoft.Maui.Platform.ContentView platformView)
    {
        PlatformUnderlayDrawable?.DisconnectHandler();

        base.DisconnectHandler(platformView);
    }

#elif ANDROID

    protected override void ConnectHandler(ContentViewGroup platformView)
    {
        base.ConnectHandler(platformView);

        PlatformUnderlayDrawable = new PlatformUnderlayDrawable();
        PlatformUnderlayDrawable.ConnectHandler(platformView, VirtualView);
    }

    protected override void DisconnectHandler(ContentViewGroup platformView)
    {
        PlatformUnderlayDrawable?.DisconnectHandler();

        base.DisconnectHandler(platformView);
    }
#endif

    public override void PlatformArrange(Rect rect)
    {
        base.PlatformArrange(rect);

        PlatformUnderlayDrawable?.PlatformArrange(rect);
    }

    public override void UpdateValue(string property)
    {
        base.UpdateValue(property);

        if (property is nameof(IUnderlayDrawable.Command) or nameof(IUnderlayDrawable.CommandParameter))
        {
            this.PlatformUnderlayDrawable?.OnCommandSet();
        }

        this.PlatformUnderlayDrawable?.Invalidate();
    }

    private static void MapStyledInputLayoutPlaceholder(IContentViewHandler elementHandler, IUnderlayDrawable underlayDrawable) => Invalidate(elementHandler, underlayDrawable);

    private static void MapStyledInputLayoutBackground(IContentViewHandler elementHandler, IUnderlayDrawable underlayDrawable) => Invalidate(elementHandler, underlayDrawable);

    private static void MapStyledInputLayoutOpacity(IContentViewHandler elementHandler, IUnderlayDrawable underlayDrawable)
    {
        (elementHandler as IHavePlatformUnderlayDrawable)?.PlatformUnderlayDrawable?.UpdateOpacity();
        Invalidate(elementHandler, underlayDrawable);
    }

    private static void MapStyledInputLayoutActivePlaceholderFontSize(IContentViewHandler elementHandler, IUnderlayDrawable underlayDrawable) => UpdateLayoutInsets(elementHandler, underlayDrawable);

    private static void MapStyledInputLayoutBorderSize(IContentViewHandler elementHandler, IUnderlayDrawable underlayDrawable) => UpdateLayoutInsets(elementHandler, underlayDrawable);

    private static void MapStyledInputLayoutContainerBorderStyle(IContentViewHandler elementHandler, IUnderlayDrawable underlayDrawable) => UpdateLayoutInsets(elementHandler, underlayDrawable);

    private static void MapStyledInputLayoutInternalMargin(IContentViewHandler elementHandler, IUnderlayDrawable underlayDrawable) => UpdateLayoutInsets(elementHandler, underlayDrawable);

    private static void MapFocusAnimationPercentage(IContentViewHandler elementHandler, IUnderlayDrawable underlayDrawable) => Invalidate(elementHandler, underlayDrawable);

    private static void MapHasValueAnimationPercentage(IContentViewHandler elementHandler, IUnderlayDrawable underlayDrawable) => Invalidate(elementHandler, underlayDrawable);

    private static void UpdateLayoutInsets(IContentViewHandler elementHandler, IUnderlayDrawable underlayDrawable)
    {
        (elementHandler as IHavePlatformUnderlayDrawable)?.PlatformUnderlayDrawable?.UpdateLayoutInsets(underlayDrawable.GetLayoutInset());
        Invalidate(elementHandler, underlayDrawable);
    }

    private static void MapCommand(IContentViewHandler elementHandler, IUnderlayDrawable underlayDrawable) => (elementHandler as IHavePlatformUnderlayDrawable)?.PlatformUnderlayDrawable?.OnCommandSet();

    private static void MapStyledInputContent(IContentViewHandler elementHandler, IContentView view)
    {
        if (elementHandler is IHavePlatformUnderlayDrawable hpud)
        {
            hpud.PlatformUnderlayDrawable?.ClearSubviews();

            MapContent(elementHandler, view);

            hpud.PlatformUnderlayDrawable?.PrepareForDisplay();
        }

        if (view is StyledInputLayout { InheritPlaceholderFromContent: true } && view.Content is InputView inputView)
        {
            var placeholderBinding = new Binding(nameof(StyledInputLayout.Placeholder), mode: BindingMode.OneWayToSource, source: view);
            inputView.SetBinding(InputView.PlaceholderProperty, placeholderBinding);

            inputView.PlaceholderColor = Colors.Transparent;
        }
    }

    private static void Invalidate(IContentViewHandler elementHandler, IUnderlayDrawable underlayDrawable) => (elementHandler as IHavePlatformUnderlayDrawable)?.PlatformUnderlayDrawable?.Invalidate();
}
