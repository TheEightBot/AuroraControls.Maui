using AuroraControls.Effects;
using Microsoft.Maui.Controls.Shapes;

namespace AuroraControls;

[ContentProperty(nameof(Content))]
public class CardViewLayout : Border
{
    private readonly Shadow _shadow = new Shadow();

    /// <summary>
    /// The corner radius property.
    /// </summary>
    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(CardViewLayout),
            RoundedCornersEffect.CornerRadiusProperty.DefaultValue,
            propertyChanged: (bindable, _, newValue) =>
            {
                if (bindable is not CardViewLayout clv || newValue is not double doubleValue)
                {
                    return;
                }

                if (clv.Content != null)
                {
                    RoundedCornersEffect.SetCornerRadius(clv.Content, doubleValue);
                }

                clv.StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(doubleValue), };

                // RoundedCornersEffect.SetCornerRadius(clv, doubleValue);
                clv._shadow.Radius = (float)doubleValue;
            });

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    /// <value>The corner radius.</value>
    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// The elevation property.
    /// </summary>
    public static readonly BindableProperty ElevationProperty =
        BindableProperty.Create(nameof(Elevation), typeof(double), typeof(CardViewLayout),
            2d,
            propertyChanged: (bindable, _, newValue) =>
            {
                if (bindable is CardViewLayout clv)
                {
                    clv._shadow.Offset = new Point(0, (double)newValue);
                }
            });

    /// <summary>
    /// The card shadow color property.
    /// </summary>
    public static readonly BindableProperty ShadowColorProperty =
        BindableProperty.Create(nameof(ShadowColor), typeof(Color), typeof(CardViewLayout), Colors.Transparent,
            propertyChanged: (bindable, _, newValue) =>
            {
                if (bindable is CardViewLayout clv)
                {
                    clv._shadow.Brush = (Color)newValue;
                }
            });

    /// <summary>
    /// Gets or sets the color of the card shadow.
    /// </summary>
    /// <value>The color of the card shadow.</value>
    public Color ShadowColor
    {
        get => (Color)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    /// <summary>
    /// The content property.
    /// </summary>
    public static new readonly BindableProperty ContentProperty =
        BindableProperty.Create(nameof(Content), typeof(View), typeof(CardViewLayout),
            propertyChanged: (bindable, _, newValue) =>
            {
                if (bindable is CardViewLayout clv)
                {
                    clv.SetContent((View)newValue);
                }
            });

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    /// <value>The content to display.</value>
    public new View Content
    {
        get => (View)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// The card border color property.
    /// </summary>
    public static readonly BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(CardViewLayout), Colors.Transparent,
            propertyChanged: (bindable, _, newValue) =>
            {
                if (bindable is not CardViewLayout clv)
                {
                    return;
                }

                clv.Stroke = (Color)newValue;
            });

    /// <summary>
    /// Gets or sets the color of the card border.
    /// </summary>
    /// <value>The color of the card border.</value>
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    /// <summary>
    /// The card border size property.
    /// </summary>
    public static readonly BindableProperty BorderSizeProperty =
        BindableProperty.Create(nameof(BorderSize), typeof(double), typeof(CardViewLayout), 0d,
            propertyChanged: (bindable, _, newValue) =>
            {
                if (bindable is not CardViewLayout clv)
                {
                    return;
                }

                if (clv.Content is not null)
                {
                    clv.Content.Margin = -(double)newValue;
                }

                clv.StrokeThickness = (double)newValue;
            });

    /// <summary>
    /// Gets or sets the size of the border.
    /// </summary>
    /// <value>The border size.</value>
    public double BorderSize
    {
        get => (double)GetValue(BorderSizeProperty);
        set => SetValue(BorderSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the elevation.
    /// </summary>
    /// <value>The evevation.</value>
    public double Elevation
    {
        get => (double)GetValue(ElevationProperty);
        set => SetValue(ElevationProperty, value);
    }

    public CardViewLayout()
    {
        this.Shadow = _shadow;
        _shadow.Brush = ShadowColor;
        _shadow.Offset = new Point(0, Elevation);
        _shadow.Radius = (float)CornerRadius;

        base.Content = this.Content;
    }

    private void SetContent(View view)
    {
        RoundedCornersEffect.SetHasRoundedCorners(view, true);
        RoundedCornersEffect.SetCornerRadius(view, CornerRadius);
        view.Margin = -BorderSize;
        base.Content = view;
    }
}
