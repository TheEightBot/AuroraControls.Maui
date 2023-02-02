using Microsoft.Maui.Animations;

namespace AuroraControls;

public enum ContainerBorderStyle
{
    Underline,
    RoundedUnderline,
    Rectangle,
    RoundedRectangle,
    RoundedRectanglePlaceholderThrough,
}

public interface IHavePlaceholder
{
    string Placeholder { get; set; }

    Color PlaceholderColor { get; set; }

    double FontSize { get; }

    public Point PlaceholderOffset { get; set; }
}

public static class HavePlaceholderElement
{
    public static Color DefaultPlaceholderColor = Color.FromArgb("#808080");

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(IHavePlaceholder.Placeholder), typeof(string), typeof(IHavePlaceholder), default(string));

    public static readonly BindableProperty PlaceholderColorProperty =
        BindableProperty.Create(nameof(IHavePlaceholder.PlaceholderColor), typeof(Microsoft.Maui.Graphics.Color), typeof(IHavePlaceholder), default(Microsoft.Maui.Graphics.Color));

    public static readonly BindableProperty PlaceholderOffsetProperty =
        BindableProperty.Create(nameof(IHavePlaceholder.PlaceholderOffset), typeof(Point), typeof(IHavePlaceholder), default(Point));
}

public interface IUnderlayDrawable : IHavePlaceholder
{
    bool IsDrawing { get; set; }

    Thickness InternalMargin { get; }

    bool HasValue { get; }

    ContainerBorderStyle BorderStyle { get; }

    Color ActiveColor { get; }

    Color InactiveColor { get; }

    Color DisabledColor { get; }

    float BorderSize { get; }

    float CornerRadius { get; }

    float ActivePlaceholderFontSize { get; }

    bool IsError { get; set; }

    string ErrorText { get; set; }

    Color ErrorColor { get; set; }

    bool AlwaysShowPlaceholder { get; }

    bool AlignPlaceholderToTop { get; }

    uint FocusAnimationDuration { get; }

    double FocusAnimationPercentage { get; set; }

    double HasValueAnimationPercentage { get; set; }

    ICommand Command { get; }

    object CommandParameter { get; }
}

public static class UnderlayDrawableElement
{
    public static readonly BindableProperty ContainerBorderStyleProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.BorderStyle), typeof(ContainerBorderStyle), typeof(IUnderlayDrawable), ContainerBorderStyle.Underline);

    public static readonly BindableProperty ActiveColorProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.ActiveColor), typeof(Microsoft.Maui.Graphics.Color), typeof(IUnderlayDrawable), Colors.Black);

    public static readonly BindableProperty InactiveColorProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.InactiveColor), typeof(Microsoft.Maui.Graphics.Color), typeof(IUnderlayDrawable), Colors.DimGray);

    public static readonly BindableProperty DisabledColorProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.DisabledColor), typeof(Microsoft.Maui.Graphics.Color), typeof(IUnderlayDrawable), Colors.LightGray);

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.CornerRadius), typeof(float), typeof(IUnderlayDrawable), 4.0f);

    public static readonly BindableProperty BorderSizeProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.BorderSize), typeof(float), typeof(IUnderlayDrawable), 1.0f);

    public static readonly BindableProperty ActivePlaceholderFontSizeProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.ActivePlaceholderFontSize), typeof(float), typeof(IUnderlayDrawable), 12.0f);

    public static readonly BindableProperty AlwaysShowPlaceholderProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.AlwaysShowPlaceholder), typeof(bool), typeof(IUnderlayDrawable), false);

    public static readonly BindableProperty FocusAnimationDurationProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.FocusAnimationDuration), typeof(uint), typeof(IUnderlayDrawable), 250u);

    public static readonly BindableProperty FocusAnimationPercentageProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.FocusAnimationPercentage), typeof(double), typeof(IUnderlayDrawable), 0.0d);

    public static readonly BindableProperty HasValueAnimationPercentageProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.HasValueAnimationPercentage), typeof(double), typeof(IUnderlayDrawable), 0.0d);

    public static readonly BindableProperty IsErrorProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.IsError), typeof(bool), typeof(IUnderlayDrawable), false);

    public static readonly BindableProperty ErrorTextProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.ErrorText), typeof(string), typeof(IUnderlayDrawable), null);

    public static readonly BindableProperty ErrorColorProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.ErrorColor), typeof(Microsoft.Maui.Graphics.Color), typeof(IUnderlayDrawable), Colors.Red);

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.Command), typeof(ICommand), typeof(IUnderlayDrawable), default(ICommand));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.CommandParameter), typeof(object), typeof(IUnderlayDrawable), default(object));

    public static readonly BindableProperty InternalMarginProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.InternalMargin), typeof(Thickness), typeof(IUnderlayDrawable), new Thickness(2),
            propertyChanged:
                (bindable, oldValue, newValue) =>
                {
                    if (bindable is IVisualElementController v)
                    {
                        //TODO: Verify if this is right
                        v.PlatformSizeChanged();
                    }
                });
}

public static class UnderlayDrawableExtensions
{
    public static SizeRequest GetDesiredSize(this IUnderlayDrawable underlayDrawable, SizeRequest sizeRequest, float scale = 1.0f)
    {
        var yOffset = underlayDrawable.ActivePlaceholderFontSize + ((underlayDrawable.BorderSize + underlayDrawable.InternalMargin.Top) * 2f);
        yOffset *= scale;

        sizeRequest.Minimum = new Microsoft.Maui.Graphics.Size(sizeRequest.Minimum.Width, sizeRequest.Minimum.Height + yOffset);
        sizeRequest.Request = new Microsoft.Maui.Graphics.Size(sizeRequest.Request.Width, sizeRequest.Request.Height + yOffset);

        return sizeRequest;
    }

    public static InsetsF GetLayoutInset(this IUnderlayDrawable underlayDrawable)
    {
        var yOffset = underlayDrawable.ActivePlaceholderFontSize;
        var borderSize = underlayDrawable.BorderSize;
        var internalMargin = underlayDrawable.InternalMargin;

        switch (underlayDrawable.BorderStyle)
        {
            case ContainerBorderStyle.Underline:
                return new(
                    (yOffset + (float)internalMargin.Top),
                    0f,
                    borderSize + (float)internalMargin.Bottom,
                    0f);
            default:
                return new(
                    yOffset + borderSize + (float)internalMargin.Top,
                    borderSize + (float)internalMargin.Left,
                    borderSize + (float)internalMargin.Bottom,
                    borderSize + (float)internalMargin.Right);
        }
    }

    public static void AnimateFocus(this IUnderlayDrawable underlayDrawable, bool isFocused, IAnimatable element)
    {
        var endFocused = isFocused ? 1d : 0d;

        element
            .TransitionTo(
                nameof(UnderlayDrawableElement.FocusAnimationPercentageProperty),
                x =>
                {
                    underlayDrawable.FocusAnimationPercentage = x;
                },
                underlayDrawable.FocusAnimationPercentage,
                endFocused,
                easing: Easing.CubicInOut,
                length: underlayDrawable.FocusAnimationDuration);
    }

    public static void AnimateHasValue(this IUnderlayDrawable underlayDrawable, IAnimatable element)
    {
        var endHasValue = underlayDrawable.HasValue ? 1d : 0d;

        element
            .TransitionTo(
                nameof(UnderlayDrawableElement.HasValueAnimationPercentageProperty),
                x =>
                {
                    underlayDrawable.HasValueAnimationPercentage = x;
                },
                underlayDrawable.HasValueAnimationPercentage,
                endHasValue,
                easing: Easing.CubicInOut,
                length: underlayDrawable.FocusAnimationDuration);
    }

    public static void DrawUnderlay(this IUnderlayDrawable underlayDrawable, View element, Rect controlFrame, SKSurface surface, SKImageInfo imageInfo)
    {
        if (underlayDrawable.IsDrawing)
        {
            return;
        }

        try
        {
            underlayDrawable.IsDrawing = true;

            var scale = (float)DeviceDisplay.Current.MainDisplayInfo.Density;

            var canvas = surface.Canvas;
            var size = imageInfo.Size;

            var hasValue = underlayDrawable.HasValue;
            var isFocused = element.IsFocused;
            var isDisabled = !element.IsEnabled;
            var isError = underlayDrawable.IsError;
            var borderSize = (float)underlayDrawable.BorderSize * scale;
            var halfBorder = borderSize / 2f;
            var cornerRadius = underlayDrawable.CornerRadius * scale;
            var cornerRadiusSize = new SKSize(cornerRadius, cornerRadius);
            var internalMargin = underlayDrawable.InternalMargin;

            var placeholderFontSize = underlayDrawable.ActivePlaceholderFontSize * scale;
            var placeholderColor =
                underlayDrawable.PlaceholderColor != default(Color)
                    ? underlayDrawable.PlaceholderColor
                    : HavePlaceholderElement.DefaultPlaceholderColor;

            var fontSize = (float)underlayDrawable.FontSize * scale;

            var controlYCenter =
                underlayDrawable.AlignPlaceholderToTop
                ? (controlFrame.Top * scale) + (fontSize * .5f)
                : controlFrame.Center.Y * scale;

            var focusedPlaceholderCenterY = borderSize + ((float)internalMargin.Top * scale) + (placeholderFontSize * .5f);
            var controlXLeft = controlFrame.Left * scale;

            var placeholderOffset = underlayDrawable.PlaceholderOffset;
            if (placeholderOffset != default(Point))
            {
                controlYCenter += placeholderOffset.Y * scale;
                controlXLeft += placeholderOffset.X * scale;
            }

            var hasValueAnimationPercentage =
                underlayDrawable.AlwaysShowPlaceholder
                    ? 1d
                    : underlayDrawable.HasValueAnimationPercentage;

            var focusAnimationPercentage = underlayDrawable.FocusAnimationPercentage;

            using var borderPaint =
                new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = borderSize,
                    Color =
                        isError
                            ? underlayDrawable.ErrorColor.ToSKColor()
                            : underlayDrawable.InactiveColor.Lerp(underlayDrawable.ActiveColor, focusAnimationPercentage).ToSKColor(),
                };

            using var backgroundPaint =
                new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                };

            using var placeholderPaint =
                new SKPaint
                {
                    TextSize = placeholderFontSize,
                    TextAlign = SKTextAlign.Left,
                    IsAntialias = true,
                    //TODO: Port
                    //Typeface = Services.PlatformInfo.DefaultTypeface,
                };

            var placeholder = underlayDrawable.Placeholder;

            canvas.Clear(SKColors.Transparent);

            switch (underlayDrawable.BorderStyle)
            {
                case ContainerBorderStyle.Underline:
                    borderPaint.StrokeCap = SKStrokeCap.Square;
                    canvas.DrawLine(new SKPoint(0, size.Height - halfBorder), new SKPoint(size.Width, size.Height - halfBorder), borderPaint);
                    break;
                case ContainerBorderStyle.RoundedUnderline:
                    borderPaint.StrokeCap = SKStrokeCap.Round;
                    canvas.DrawLine(new SKPoint(halfBorder, size.Height - halfBorder), new SKPoint(size.Width - borderSize, size.Height - halfBorder), borderPaint);
                    break;
                case ContainerBorderStyle.Rectangle:
                    var rectBackground = SKRect.Create(halfBorder, halfBorder, size.Width - borderSize, size.Height - borderSize);

                    backgroundPaint.Color =
                        isDisabled
                            ? underlayDrawable.DisabledColor.Lerp(Colors.White, .2d).ToSKColor()
                            : element.BackgroundColor.ToSKColor();

                    canvas.DrawRect(rectBackground, backgroundPaint);

                    if (underlayDrawable.BorderSize > 0d)
                    {
                        canvas.DrawRect(rectBackground, borderPaint);
                    }
                    break;
                case ContainerBorderStyle.RoundedRectangle:
                    var roundedRectBackground = SKRect.Create(halfBorder, halfBorder, size.Width - borderSize, size.Height - borderSize);

                    backgroundPaint.Color =
                        isDisabled
                            ? underlayDrawable.DisabledColor.Lerp(Colors.White, .2d).ToSKColor()
                            : element.BackgroundColor.ToSKColor();

                    canvas.DrawRoundRect(roundedRectBackground, cornerRadiusSize, backgroundPaint);

                    if (borderSize > 0d)
                    {
                        canvas.DrawRoundRect(roundedRectBackground, cornerRadiusSize, borderPaint);
                    }
                    break;

                case ContainerBorderStyle.RoundedRectanglePlaceholderThrough:
                    var roundedRectBackgroundPlaceholderThrough = SKRect.Create(halfBorder, halfBorder + focusedPlaceholderCenterY, size.Width - borderSize, size.Height - focusedPlaceholderCenterY - borderSize);

                    backgroundPaint.Color =
                        isDisabled
                            ? underlayDrawable.DisabledColor.Lerp(Colors.White, .2d).ToSKColor()
                            : element.BackgroundColor.ToSKColor();

                    canvas.DrawRoundRect(roundedRectBackgroundPlaceholderThrough, cornerRadiusSize, backgroundPaint);

                    using (new SKAutoCanvasRestore(canvas))
                    {
                        canvas.Save();
                        canvas.Clear();

                        if (borderSize > 0d)
                        {
                            placeholderPaint.TextSize = placeholderFontSize;
                            var placeholderRectSize = canvas.GetTextContainerRectAt(isError ? underlayDrawable.ErrorText : placeholder, new SKPoint((float)controlXLeft, 0.0f), placeholderPaint);

                            canvas.DrawRoundRect(roundedRectBackgroundPlaceholderThrough, cornerRadiusSize, borderPaint);

                            System.Diagnostics.Debug.WriteLine($"HVAP: {hasValueAnimationPercentage}");

                            if (hasValueAnimationPercentage > 0.0d)
                            {
                                var bufferSize = 2f;
                                var top = roundedRectBackgroundPlaceholderThrough.Top - (borderPaint.StrokeWidth * .5f);

                                var startingBlendMode = backgroundPaint.BlendMode;

                                backgroundPaint.BlendMode = SKBlendMode.SrcIn;

                                backgroundPaint.Color =
                                    borderPaint.Color.Lerp(
                                        element.BackgroundColor.ToSKColor(),
                                        hasValueAnimationPercentage);

                                canvas.DrawRect(new SKRect(placeholderRectSize.Left - bufferSize, top, placeholderRectSize.Right + (bufferSize * 2f), top + placeholderRectSize.Height), backgroundPaint);

                                backgroundPaint.BlendMode = startingBlendMode;
                            }
                        }

                        canvas.Restore();
                    }
                    break;
            }

            if (isError)
            {
                placeholderPaint.TextSize = placeholderFontSize;
                placeholderPaint.Color = underlayDrawable.ErrorColor.ToSKColor();
                placeholderPaint.EnsureHasValidFont(underlayDrawable.ErrorText ?? placeholder);

                canvas.DrawTextCenteredVertically(underlayDrawable.ErrorText ?? placeholder, new SKPoint((float)controlXLeft, focusedPlaceholderCenterY), placeholderPaint);
            }

            else if (!string.IsNullOrEmpty(placeholder))
            {
                placeholderPaint.Color =
                    placeholderColor
                        .Lerp(
                            focusAnimationPercentage > 0d
                                ? underlayDrawable.ActiveColor
                                : placeholderColor,
                            focusAnimationPercentage > 0d ? focusAnimationPercentage : hasValueAnimationPercentage)
                        .ToSKColor();

                if (placeholderFontSize > 0d)
                {
                    var placeholderY = controlYCenter.Lerp(focusedPlaceholderCenterY, hasValueAnimationPercentage);

                    placeholderPaint.TextSize = fontSize.Lerp(placeholderFontSize, (float)hasValueAnimationPercentage);
                    placeholderPaint.Color = placeholderPaint.Color.WithAlpha((float)hasValueAnimationPercentage);

                    placeholderPaint.EnsureHasValidFont(placeholder);

                    canvas.DrawTextCenteredVertically(placeholder, new SKPoint((float)controlXLeft, (float)placeholderY), placeholderPaint);
                }

                placeholderPaint.Color = placeholderColor.ToSKColor().WithAlpha(1f - (float)hasValueAnimationPercentage);
                placeholderPaint.TextSize = fontSize;

                canvas.DrawTextCenteredVertically(placeholder, new SKPoint((float)controlXLeft, (float)controlYCenter), placeholderPaint);
            }
        }
        finally
        {
            underlayDrawable.IsDrawing = false;
        }
    }
}

