using AuroraControls.Effects;

namespace AuroraControls;

[ContentProperty(nameof(Content))]
public class CardViewLayout : ContentView
{
    /// <summary>
    /// The corner radius property.
    /// </summary>
    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(CardViewLayout),
            ShadowEffect.CornerRadiusProperty.DefaultValue,
            propertyChanged: (bindable, _, newValue) =>
            {
                if (bindable is not CardViewLayout clv)
                {
                    return;
                }

                if (clv.Content != null)
                {
                    RoundedCornersEffect.SetCornerRadius(clv.Content, (double)newValue);
                }

                ShadowEffect.SetCornerRadius(clv, (double)newValue);
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
            ShadowEffect.ElevationProperty.DefaultValue,
            propertyChanged: (bindable, _, newValue) =>
            {
                if (bindable is CardViewLayout clv)
                {
                    ShadowEffect.SetElevation(clv, (double)newValue);
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
                    ShadowEffect.SetShadowColor(clv, (Color)newValue);
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
        BindableProperty.Create(nameof(Content), typeof(View), typeof(CardViewLayout), default(View),
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
    /// <value>The content to displa.</value>
    public new View Content
    {
        get => (View)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// The card background color property.
    /// </summary>
    public static new readonly BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(CardViewLayout), default(Color),
            propertyChanged: (bindable, _, newValue) =>
            {
                if (bindable is CardViewLayout clv && clv.Content != null)
                {
                    clv.Content.BackgroundColor = (Color)newValue;
                }
            });

    /// <summary>
    /// Gets or sets the color of the card background.
    /// </summary>
    /// <value>The color of the card background.</value>
    public new Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
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

                if (clv.Content != null)
                {
                    RoundedCornersEffect.SetBorderColor(clv.Content, (Color)newValue);
                }
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

                if (clv.Content != null)
                {
                    RoundedCornersEffect.SetBorderSize(clv.Content, (double)newValue);
                }
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
        base.BackgroundColor = Colors.Transparent;

        this.Effects.Add(new ShadowEffect());
        ShadowEffect.SetCornerRadius(this, CornerRadius);
        ShadowEffect.SetElevation(this, Elevation);
        ShadowEffect.SetShadowColor(this, ShadowColor);

        base.Content = this.Content;
    }

    private void SetContent(View view)
    {
        view.Effects.Add(new RoundedCornersEffect());
        RoundedCornersEffect.SetCornerRadius(view, CornerRadius);

        view.BackgroundColor = this.BackgroundColor;

        base.Content = view;
    }
}
