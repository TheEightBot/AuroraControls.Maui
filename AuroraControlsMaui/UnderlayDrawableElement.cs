namespace AuroraControls;

public static class UnderlayDrawableElement
{
    public static readonly BindableProperty BorderStyleProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.BorderStyle), typeof(ContainerBorderStyle), typeof(IUnderlayDrawable), ContainerBorderStyle.Underline);

    public static readonly BindableProperty ActiveColorProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.ActiveColor), typeof(Color), typeof(IUnderlayDrawable), Colors.Black);

    public static readonly BindableProperty InactiveColorProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.InactiveColor), typeof(Color), typeof(IUnderlayDrawable), Colors.DimGray);

    public static readonly BindableProperty DisabledColorProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.DisabledColor), typeof(Color), typeof(IUnderlayDrawable), Colors.LightGray);

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
        BindableProperty.Create(nameof(IUnderlayDrawable.IsError), typeof(bool), typeof(IUnderlayDrawable), false,
            propertyChanged:
                static (bindable, oldValue, newValue) =>
                {
                    if (bindable is IUnderlayDrawable ud && oldValue is bool oldv && newValue is bool newv && !oldv && newv)
                    {
                        ud.HasValueAnimationPercentage = 1.0d;
                    }
                });

    public static readonly BindableProperty ErrorTextProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.ErrorText), typeof(string), typeof(IUnderlayDrawable));

    public static readonly BindableProperty ErrorColorProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.ErrorColor), typeof(Color), typeof(IUnderlayDrawable), Colors.Red);

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.Command), typeof(ICommand), typeof(IUnderlayDrawable));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.CommandParameter), typeof(object), typeof(IUnderlayDrawable));

    public static readonly BindableProperty InternalMarginProperty =
        BindableProperty.Create(nameof(IUnderlayDrawable.InternalMargin), typeof(Thickness), typeof(IUnderlayDrawable), new Thickness(2),
            propertyChanged:
                static (bindable, _, _) =>
                {
                    if (bindable is IVisualElementController v)
                    {
                        // TODO: Verify if this is right
                        v.PlatformSizeChanged();
                    }
                });
}

public static class UnderlayDrawableExtensions
{
    public static SizeRequest GetDesiredSize(this IUnderlayDrawable underlayDrawable, SizeRequest sizeRequest, float scale = 1.0f)
    {
        double yOffset = underlayDrawable.ActivePlaceholderFontSize + ((underlayDrawable.BorderSize + underlayDrawable.InternalMargin.Top) * 2f);

        yOffset *= scale;

        sizeRequest.Minimum = new Size(sizeRequest.Minimum.Width, sizeRequest.Minimum.Height + yOffset);
        sizeRequest.Request = new Size(sizeRequest.Request.Width, sizeRequest.Request.Height + yOffset);

        return sizeRequest;
    }

    public static InsetsF GetLayoutInset(this IUnderlayDrawable underlayDrawable)
    {
        float yOffset = underlayDrawable.ActivePlaceholderFontSize;
        float borderSize = underlayDrawable.BorderSize;
        var internalMargin = underlayDrawable.InternalMargin;

        switch (underlayDrawable.BorderStyle)
        {
            case ContainerBorderStyle.Underline:
                return new(
                    yOffset + (float)internalMargin.Top,
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
}

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

    double FontSize { get; set; }

    public Point PlaceholderOffset { get; set; }
}

public static class HavePlaceholderElement
{
    internal static readonly Color DefaultPlaceholderColor = Color.FromArgb("#808080");

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(IHavePlaceholder.Placeholder), typeof(string), typeof(IHavePlaceholder));

    public static readonly BindableProperty InheritPlaceholderFromContentProperty =
        BindableProperty.Create(nameof(IHavePlaceholder.Placeholder), typeof(bool), typeof(IHavePlaceholder), true);

    public static readonly BindableProperty PlaceholderColorProperty =
        BindableProperty.Create(nameof(IHavePlaceholder.PlaceholderColor), typeof(Color), typeof(IHavePlaceholder), DefaultPlaceholderColor);

    public static readonly BindableProperty PlaceholderOffsetProperty =
        BindableProperty.Create(nameof(IHavePlaceholder.PlaceholderOffset), typeof(Point), typeof(IHavePlaceholder), default(Point));

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(IHavePlaceholder.FontSize), typeof(double), typeof(IHavePlaceholder), default(double));
}

public interface IUnderlayDrawable : IElement, IHavePlaceholder
{
    Thickness InternalMargin { get; }

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

    uint FocusAnimationDuration { get; }

    double FocusAnimationPercentage { get; set; }

    double HasValueAnimationPercentage { get; set; }

    ICommand Command { get; }

    object CommandParameter { get; }
}
