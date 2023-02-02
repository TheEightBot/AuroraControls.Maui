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
                //[nameof(Entry.FontSize)] = MapDrawableFontSize,
                [nameof(IUnderlayDrawable.ActivePlaceholderFontSize)] = MapFloatLabelActivePlaceholderFontSize,
                [nameof(IUnderlayDrawable.BorderSize)] = MapFloatLabelBorderSize,
                [nameof(IUnderlayDrawable.BorderStyle)] = MapFloatLabelContainerBorderStyle,
                [nameof(IUnderlayDrawable.InternalMargin)] = MapFloatLabelInternalMargin,
                [nameof(IUnderlayDrawable.FocusAnimationPercentage)] = MapFocusAnimationPercentage,
                [nameof(IUnderlayDrawable.HasValueAnimationPercentage)] = MapHasValueAnimationPercentage,
            };

        public static PropertyMapper IUnderlayDrawableMapper =
            new PropertyMapper<IUnderlayDrawable>
            {
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

        private static void MapFloatLabelPlaceholder(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
            handler._underlayDrawable.Invalidate();
        }

        private static void MapFloatLabelBackground(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
            handler._underlayDrawable.Invalidate();
        }

        private static void MapFloatLabelActivePlaceholderFontSize(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
            UpdateLayoutInsets(handler);
        }

        private static void MapFloatLabelBorderSize(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
            UpdateLayoutInsets(handler);
        }
        private static void MapFloatLabelContainerBorderStyle(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
            UpdateLayoutInsets(handler);
        }

        private static void MapFloatLabelInternalMargin(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
            UpdateLayoutInsets(handler);
        }

        private static void MapFocusAnimationPercentage(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
            handler._underlayDrawable.Invalidate();
        }

        private static void MapHasValueAnimationPercentage(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
            handler._underlayDrawable.Invalidate();
        }

        private static void UpdateLayoutInsets(FloatLabelEntryHandler handler)
        {
            if (handler.VirtualView is IUnderlayDrawable ud)
            {
                handler._underlayDrawable.UpdateLayoutInsets(ud.GetLayoutInset());
            }
        }
    }
}