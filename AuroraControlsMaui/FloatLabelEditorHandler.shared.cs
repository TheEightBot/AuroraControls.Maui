using System;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace AuroraControls
{
    public partial class FloatLabelEntryHandler : EntryHandler
    {
        public bool PreviousHasValue { get; set; }

        public static PropertyMapper FloatLabelEntryMapper =
            new PropertyMapper<FloatLabelEntry, FloatLabelEntryHandler>(EntryHandler.Mapper)
            {
                [nameof(IView.IsFocused)] = MapFloatLabelFocus,
                [nameof(IView.Background)] = MapFloatLabelBackground,
                [nameof(VisualElement.BackgroundColor)] = MapFloatLabelBackground,
                [nameof(Entry.Text)] = MapFloatLabelText,
                [nameof(Entry.Placeholder)] = MapFloatLabelPlaceholder,
                [nameof(Entry.PlaceholderColor)] = MapFloatLabelPlaceholder,
                [nameof(IUnderlayDrawable.ActivePlaceholderFontSize)] = MapFloatLabelActivePlaceholderFontSize,
                [nameof(IUnderlayDrawable.BorderSize)] = MapFloatLabelBorderSize,
                [nameof(IUnderlayDrawable.BorderStyle)] = MapFloatLabelContainerBorderStyle,
                [nameof(IUnderlayDrawable.InternalMargin)] = MapFloatLabelInternalMargin,
                [nameof(IUnderlayDrawable.FocusAnimationPercentage)] = MapFocusAnimationPercentage,
                [nameof(IUnderlayDrawable.HasValueAnimationPercentage)] = MapHasValueAnimationPercentage,
            };

        private static void MapFloatLabelFocus(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
            var virtualView = handler.VirtualView;

            if (virtualView is IUnderlayDrawable ud && virtualView is IAnimatable ia)
            {
                ud.AnimateFocus(virtualView.IsFocused, ia);
            }
        }

        private static void MapFloatLabelText(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
            MapText(handler, view);

            var virtualView = handler.VirtualView;

            if (virtualView is IUnderlayDrawable ud && virtualView is IAnimatable ia && ud.HasValue != handler.PreviousHasValue)
            {
                handler.PreviousHasValue = ud.HasValue;
                ud.AnimateHasValue(ia);
            }
        }

        private static void MapFloatLabelPlaceholder(IElementHandler elementHandler, IUnderlayDrawable underlayDrawable)
        {
            Invalidate(elementHandler, underlayDrawable);
        }

        private static void MapFloatLabelBackground(IElementHandler elementHandler, IUnderlayDrawable underlayDrawable)
        {
            Invalidate(elementHandler, underlayDrawable);
        }

        private static void MapFloatLabelActivePlaceholderFontSize(IElementHandler elementHandler, IUnderlayDrawable underlayDrawable)
        {
            UpdateLayoutInsets(elementHandler, underlayDrawable);
        }

        private static void MapFloatLabelBorderSize(IElementHandler elementHandler, IUnderlayDrawable underlayDrawable)
        {
            UpdateLayoutInsets(elementHandler, underlayDrawable);
        }

        private static void MapFloatLabelContainerBorderStyle(IElementHandler elementHandler, IUnderlayDrawable underlayDrawable)
        {
            UpdateLayoutInsets(elementHandler, underlayDrawable);
        }

        private static void MapFloatLabelInternalMargin(IElementHandler elementHandler, IUnderlayDrawable underlayDrawable)
        {
            UpdateLayoutInsets(elementHandler, underlayDrawable);
        }

        private static void MapFocusAnimationPercentage(IElementHandler elementHandler, IUnderlayDrawable underlayDrawable)
        {
            Invalidate(elementHandler, underlayDrawable);
        }

        private static void MapHasValueAnimationPercentage(IElementHandler elementHandler, IUnderlayDrawable underlayDrawable)
        {
            Invalidate(elementHandler, underlayDrawable);
        }

        private static void UpdateLayoutInsets(IElementHandler elementHandler, IUnderlayDrawable underlayDrawable)
        {
            (elementHandler as IHavePlatformUnderlayDrawable)?.PlatformUnderlayDrawable?.UpdateLayoutInsets(underlayDrawable.GetLayoutInset());
        }

        private static void Invalidate(IElementHandler elementHandler, IUnderlayDrawable underlayDrawable)
        {
            (elementHandler as IHavePlatformUnderlayDrawable)?.PlatformUnderlayDrawable?.Invalidate();
        }
    }
}