using System;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Animations;
using Microsoft.Maui;

#if IOS
using UIKit;
using CoreGraphics;
#endif

namespace AuroraControls;

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
        handler._backgroundDrawable.SetHasValue(
            !string.IsNullOrEmpty(view.Text),
            view, () => handler._graphicsView.InvalidateDrawable());
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
        handler._backgroundDrawable.FontSize = (float)view.FontSize;
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

    public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
    {
        var sizeRequest = base.GetDesiredSize(widthConstraint, heightConstraint);

        if (_backgroundDrawable != null)
        {
            return _backgroundDrawable.GetDesiredSize(sizeRequest);
        }

        return sizeRequest;
    }
#endif
}

public class BackgroundDrawable : IDrawable
{
    public Color BackgroundColor { get; set; }

    public Color BorderBackgroundColor { get; set; }

    public float BorderSize { get; set; }

    public IFont Font { get; set; }

    public float ActivePlaceholderFontSize { get; set; } = 10.0f;

    public float FontSize { get; set; } = 14.0f;

    public string Placeholder { get; set; }

    public bool HasValue { get; private set; }    

    public double HasValueAnimationPercentage { get; set; }

    public uint FloatAnimationDuration { get; set; } = 400;

    public Thickness InternalMargin { get; set; } = new Thickness(2);

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = BackgroundColor;

        canvas.FillRoundedRectangle(
            new Rect(0, 0, dirtyRect.Width, dirtyRect.Height), 2d);

        canvas.Font = Font;
        canvas.FontColor = Colors.White;

        var lerpedFont = FontSize.Lerp(ActivePlaceholderFontSize, HasValueAnimationPercentage);
        canvas.FontSize = lerpedFont;
        System.Diagnostics.Debug.WriteLine($"Active Font: {lerpedFont}");

        if (!HasValue)
        {
            var stringSize = canvas.GetStringSize(Placeholder, Font, lerpedFont);

            var placeholderOffset = (dirtyRect.Height - stringSize.Height) * .5f;

            canvas.DrawString(Placeholder, 0, placeholderOffset, dirtyRect.Width, dirtyRect.Height - placeholderOffset, HorizontalAlignment.Left, VerticalAlignment.Top);
        }
        else
        {
            canvas.FontSize = lerpedFont;
            canvas.DrawString(Placeholder, 0, 0, dirtyRect.Width, dirtyRect.Height, HorizontalAlignment.Left, VerticalAlignment.Top);
        }
    }

    public void SetHasValue(bool hasValue, View virtualView, Action invalidateAction)
    {
        if (HasValue == hasValue)
        {
            return;
        }

        HasValue = hasValue;

        var endHasValue = hasValue ? 1d : 0d;

        virtualView
            .TransitionTo(
                nameof(HasValueAnimationPercentage),
                x =>
                {
                    System.Diagnostics.Debug.WriteLine($"Anim Percentage: {x}");
                    HasValueAnimationPercentage = Math.Round(x, 3);
                    invalidateAction?.Invoke();
                },
                HasValueAnimationPercentage,
                endHasValue,
                easing: Easing.CubicInOut,
                length: FloatAnimationDuration);
    }

    public SizeRequest GetDesiredSize(SizeRequest sizeRequest, float scale = 1.0f)
    {
        var yOffset = ActivePlaceholderFontSize + ((BorderSize + InternalMargin.Top) * 2f);
        yOffset *= scale;

        sizeRequest.Minimum = new Size(sizeRequest.Minimum.Width, sizeRequest.Minimum.Height + yOffset);
        sizeRequest.Request = new Size(sizeRequest.Request.Width, sizeRequest.Request.Height + yOffset);

        return sizeRequest;
    }
}