using System;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

#if IOS
using UIKit;
using CoreGraphics;
#endif

namespace AuroraControls
{
    public partial class FloatLabelEntryHandler : EntryHandler
    {
        public static PropertyMapper FloatLabelEntryMapper =
            new PropertyMapper<FloatLabelEntry, FloatLabelEntryHandler>(EntryHandler.Mapper)
            {
                [nameof(VisualElement.Background)] = MapDrawableBackground,
                [nameof(VisualElement.BackgroundColor)] = MapDrawableBackground,
                [nameof(VisualElement.Height)] = MapDrawableSize,
                [nameof(VisualElement.Width)] = MapDrawableSize,
                [nameof(Entry.Text)] = MapDrawableText,
                [nameof(Entry.Placeholder)] = MapDrawablePlaceholder,
                [nameof(Entry.PlaceholderColor)] = MapDrawablePlaceholder,
                [nameof(Entry.FontSize)] = MapDrawableFontSize,
            };

#if IOS
        private PlatformGraphicsView _graphicsView;
        private UITapGestureRecognizer _graphicsViewTapped;
#endif

        private BackgroundDrawable _backgroundDrawable;

        public FloatLabelEntryHandler()
            : base(FloatLabelEntryMapper)
        {
        }

        private static void MapDrawableBackground(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
#if IOS
            handler._backgroundDrawable.BackgroundColor = view.BackgroundColor;
            handler._graphicsView.InvalidateDrawable();
#endif
        }

        private static void MapDrawableSize(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
#if IOS
            if (handler._graphicsView != null && view.Height > 0d && view.Width > 0d)
            {
                handler._graphicsView.Frame = new CGRect(0f, 0f, (float)view.Width, (float)view.Height);
                handler._graphicsView.InvalidateDrawable();
            }
#endif
        }

        private static void MapDrawableText(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
            MapText(handler, view);

#if IOS
            handler._backgroundDrawable.HasText = !string.IsNullOrEmpty(view.Text);
            handler._graphicsView.InvalidateDrawable();
#endif
        }

        private static void MapDrawablePlaceholder(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
#if IOS
            handler._backgroundDrawable.Placeholder = view.Placeholder;
            handler._graphicsView.InvalidateDrawable();
#endif
        }

        private static void MapDrawableFontSize(FloatLabelEntryHandler handler, FloatLabelEntry view)
        {
            MapFont(handler, view);
#if IOS
            handler._backgroundDrawable.FontSize = view.FontSize;
            handler._graphicsView.InvalidateDrawable();
#endif
        }

#if IOS
        protected override void ConnectHandler(MauiTextField platformView)
        {
            base.ConnectHandler(platformView);

            platformView.BackgroundColor = UIColor.Clear;
            platformView.BorderStyle = UITextBorderStyle.None;

            _graphicsView =
                new PlatformGraphicsView
                {
                    Drawable = _backgroundDrawable = new BackgroundDrawable(),
                    BackgroundColor = UIColor.Clear,
                };

            _graphicsView
                .AddGestureRecognizer(
                    _graphicsViewTapped = new UITapGestureRecognizer(() =>
                    {
                        VirtualView?.Focus();
                    }));
            platformView.InsertSubview(_graphicsView, 0);

            _graphicsView.InvalidateDrawable();
        }

        protected override void DisconnectHandler(MauiTextField platformView)
        {
            _graphicsView.RemoveFromSuperview();

            base.DisconnectHandler(platformView);
        }
#endif
    }
}

public class BackgroundDrawable : IDrawable
{
    public Color BackgroundColor { get; set; }

    public IFont Font { get; set; } = Microsoft.Maui.Graphics.Font.Default;

    public double FontSize { get; set; } = (double)Entry.FontSizeProperty.DefaultValue;

    public string Placeholder { get; set; }

    public bool HasText { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var fontSize = (float)FontSize;

        canvas.FillColor = BackgroundColor;

        canvas.FillRoundedRectangle(
            new Rect(0, 0, dirtyRect.Width, dirtyRect.Height), 2d);

        //canvas.Font = Font;
        //canvas.FontSize = (float)FontSize;
        canvas.FontColor = Colors.White;

        if(!HasText)
        {
            var stringSize = canvas.GetStringSize(Placeholder, Font, fontSize);

            var stringCenter = (dirtyRect.Height - stringSize.Height) * .5f;

            canvas.DrawString(Placeholder, 0, stringCenter, stringSize.Width, stringSize.Height, HorizontalAlignment.Left, VerticalAlignment.Top, TextFlow.OverflowBounds);

            canvas.FillColor = Colors.Chartreuse;
            canvas.DrawRectangle(0, stringCenter, stringSize.Width, stringSize.Height);
        }

        canvas.SaveState();
    }
}