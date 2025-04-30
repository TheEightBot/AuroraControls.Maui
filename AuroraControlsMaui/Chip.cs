using Microsoft.Maui.Controls.Internals;
using Svg.Skia;

namespace AuroraControls;

public enum ChipShape
{
    Standard,
    Rectangle,
    RoundedRectangle,
}

public enum ChipState
{
    Interactable,
    ReadOnly,
}

public class Chip : AuroraViewBase, IDisposable
{
    private static readonly Size _minSize = new(52, 32);

    private readonly SKPath _backgroundPath = new();
    private readonly bool _cantHandleTouch;
    private bool _disposed;

    public event EventHandler<bool> Toggled;

    public event EventHandler Tapped;

    public event EventHandler Removed;

    private SKRect _rect, _removeRect;
    private SKSvg? _leadingSvg, _trailingSvg;

    private double _calculatedWidth;

    /// <summary>
    /// The state of the toggle.
    /// </summary>
    public static BindableProperty IsToggledProperty =
        BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(Chip), false,
            propertyChanged:
            (bindable, _, newValue) =>
            {
                if (bindable is not Chip chip || newValue is not bool toggled)
                {
                    return;
                }

                chip.InvalidateSurface();

                chip.Toggled?.Invoke(chip, toggled);

                if (!toggled)
                {
                    return;
                }

                if (chip.Command?.CanExecute(chip.CommandParameter) ?? false)
                {
                    chip.Command.Execute(chip.CommandParameter);
                }
            });

    /// <summary>
    /// Gets or sets a value indicating whether the box is toggled or not.
    /// </summary>
    /// <value><c>false</c> if not toggled, otherwise <c>true</c>.</value>
    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    public static BindableProperty StateProperty =
        BindableProperty.Create(nameof(State), typeof(ChipState), typeof(Chip), ChipState.Interactable,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public ChipState State
    {
        get => (ChipState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public static BindableProperty IsRemovableProperty =
        BindableProperty.Create(nameof(IsRemovable), typeof(bool), typeof(Chip), default(bool),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public bool IsRemovable
    {
        get => (bool)GetValue(IsRemovableProperty);
        set => SetValue(IsRemovableProperty, value);
    }

    public static BindableProperty IsSingleSelectionProperty =
        BindableProperty.Create(nameof(IsSingleSelection), typeof(bool), typeof(Chip), default(bool));

    public bool IsSingleSelection
    {
        get => (bool)GetValue(IsSingleSelectionProperty);
        set => SetValue(IsSingleSelectionProperty, value);
    }

    /// <summary>
    /// The command property. Fires on tap.
    /// </summary>
    public static BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(Chip));

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>Takes a System.Windows.Input.ICommand. Default value is default(ICommand).</value>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// The command parameter property.
    /// </summary>
    public static BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(Chip));

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    /// <value>The command parameter.</value>
    public object CommandParameter
    {
        get => this.GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// The Tapped command property. Fires on tap.
    /// </summary>
    public static BindableProperty TappedCommandProperty =
        BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(Chip));

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>Takes a System.Windows.Input.ICommand. Default value is default(ICommand).</value>
    public ICommand TappedCommand
    {
        get => (ICommand)GetValue(TappedCommandProperty);
        set => SetValue(TappedCommandProperty, value);
    }

    /// <summary>
    /// The command parameter property.
    /// </summary>
    public static BindableProperty TappedCommandParameterProperty =
        BindableProperty.Create(nameof(TappedCommandParameter), typeof(object), typeof(Chip));

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    /// <value>The command parameter.</value>
    public object TappedCommandParameter
    {
        get => this.GetValue(TappedCommandParameterProperty);
        set => SetValue(TappedCommandParameterProperty, value);
    }

    /// <summary>
    /// The removed command property. Fires on tap.
    /// </summary>
    public static BindableProperty RemovedCommandProperty =
        BindableProperty.Create(nameof(RemovedCommand), typeof(ICommand), typeof(Chip));

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>Takes a System.Windows.Input.ICommand. Default value is default(ICommand).</value>
    public ICommand RemovedCommand
    {
        get => (ICommand)GetValue(RemovedCommandProperty);
        set => SetValue(RemovedCommandProperty, value);
    }

    /// <summary>
    /// The command parameter property.
    /// </summary>
    public static BindableProperty RemovedCommandParameterProperty =
        BindableProperty.Create(nameof(RemovedCommandParameter), typeof(object), typeof(Chip));

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    /// <value>The command parameter.</value>
    public object RemovedCommandParameter
    {
        get => (object)GetValue(RemovedCommandParameterProperty);
        set => SetValue(RemovedCommandParameterProperty, value);
    }

    /// <summary>
    /// The border color.
    /// </summary>
    public static BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(Chip), Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the border color.
    /// </summary>
    /// <value>Color of the border.</value>
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    public static BindableProperty ToggledBorderColorProperty =
        BindableProperty.Create(nameof(ToggledBorderColor), typeof(Color), typeof(Chip), Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public Color ToggledBorderColor
    {
        get => (Color)GetValue(ToggledBorderColorProperty);
        set => SetValue(ToggledBorderColorProperty, value);
    }

    /// <summary>
    /// The border color when the chip is ReadOnly.
    /// </summary>
    public static BindableProperty ReadOnlyBorderColorProperty =
        BindableProperty.Create(nameof(ReadOnlyBorderColor), typeof(Color), typeof(Chip), Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the border color when the chip is ReadOnly.
    /// </summary>
    /// <value>Color of the border.</value>
    public Color ReadOnlyBorderColor
    {
        get => (Color)GetValue(ReadOnlyBorderColorProperty);
        set => SetValue(ReadOnlyBorderColorProperty, value);
    }

    /// <summary>
    /// The color of the background.
    /// </summary>
    public static new BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(Chip), Color.FromArgb("#EAEAEA"),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    /// <value>Expects a Color. Default value is #EAEAEA.</value>
    public new Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }

    /// <summary>
    /// The color of the background when toggled.
    /// </summary>
    public static BindableProperty ToggledBackgroundColorProperty =
        BindableProperty.Create(nameof(ToggledBackgroundColor), typeof(Color), typeof(Chip), Color.FromArgb("#D2D2D2"),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the background color when toggled.
    /// </summary>
    /// <value>Expects a Color. Default value is #D2D2D2.</value>
    public Color ToggledBackgroundColor
    {
        get => (Color)GetValue(ToggledBackgroundColorProperty);
        set => SetValue(ToggledBackgroundColorProperty, value);
    }

    /// <summary>
    /// The color of the background when the chip is ReadOnly.
    /// </summary>
    public static BindableProperty ReadOnlyBackgroundColorProperty =
        BindableProperty.Create(nameof(ReadOnlyBackgroundColor), typeof(Color), typeof(Chip), Colors.DarkGray,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the background color when the chip is ReadOnly.
    /// </summary>
    /// <value>Expects a Color. Default Color.DarkGray.</value>
    public Color ReadOnlyBackgroundColor
    {
        get => (Color)GetValue(ReadOnlyBackgroundColorProperty);
        set => SetValue(ReadOnlyBackgroundColorProperty, value);
    }

    public static BindableProperty BorderSizeProperty =
        BindableProperty.Create(nameof(BorderSize), typeof(double), typeof(Chip), default(double),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public double BorderSize
    {
        get => (double)GetValue(BorderSizeProperty);
        set => SetValue(BorderSizeProperty, value);
    }

    public static BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(Chip), 8d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// The shape of the chip.
    /// </summary>
    public static BindableProperty ShapeProperty =
        BindableProperty.Create(nameof(Shape), typeof(ChipShape), typeof(Chip), ChipShape.Standard,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets shape of the chip.
    /// </summary>
    /// <value>Takes a ChipShape. Default is ChipShape.Circular.</value>
    public ChipShape Shape
    {
        get => (ChipShape)GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    /// <summary>
    /// The value of the chip.
    /// </summary>
    public static BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(object), typeof(Chip));

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>object.</value>
    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// The text on the button.
    /// </summary>
    public static BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(Chip),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>string value for text. Default is default(string).</value>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// The font color of the text.
    /// </summary>
    public static BindableProperty FontColorProperty =
        BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(Chip), Color.FromArgb("#272727"),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the font.
    /// </summary>
    /// <value>Expects a Color. Default value is #272727.</value>
    public Color FontColor
    {
        get => (Color)GetValue(FontColorProperty);
        set => SetValue(FontColorProperty, value);
    }

    public static BindableProperty ToggledFontColorProperty =
        BindableProperty.Create(nameof(ToggledFontColor), typeof(Color), typeof(Chip),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public Color ToggledFontColor
    {
        get => (Color)GetValue(ToggledFontColorProperty);
        set => SetValue(ToggledFontColorProperty, value);
    }

    /// <summary>
    /// The font color of the text when chip is ReadOnly.
    /// </summary>
    public static BindableProperty ReadOnlyFontColorProperty =
        BindableProperty.Create(nameof(ReadOnlyFontColor), typeof(Color), typeof(Chip), Color.FromArgb("#272727"),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the font when chip is ReadOnly.
    /// </summary>
    /// <value>Expects a Color. Default value is #272727.</value>
    public Color ReadOnlyFontColor
    {
        get => (Color)GetValue(ReadOnlyFontColorProperty);
        set => SetValue(ReadOnlyFontColorProperty, value);
    }

    /// <summary>
    /// The font size of the text.
    /// </summary>
    public static BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(Chip), PlatformInfo.DefaultButtonFontSize,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets size of the font.
    /// </summary>
    /// <value>The size of the font as a double. Default is 12d.</value>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// The typeface of the text.
    /// </summary>
    public static BindableProperty TypefaceProperty =
        BindableProperty.Create(nameof(Typeface), typeof(SKTypeface), typeof(Chip),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the typeface.
    /// </summary>
    /// <value>Expects a SKTypeface. Default default(SKTypeface).</value>
    public SKTypeface Typeface
    {
        get => (SKTypeface)GetValue(TypefaceProperty);
        set => SetValue(TypefaceProperty, value);
    }

    public static BindableProperty IsIconifiedTextProperty =
        BindableProperty.Create(nameof(IsIconifiedText), typeof(bool), typeof(Chip), default(bool),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public bool IsIconifiedText
    {
        get => (bool)GetValue(IsIconifiedTextProperty);
        set => SetValue(IsIconifiedTextProperty, value);
    }

    /// <summary>
    /// The embedded svg image name property.
    /// </summary>
    public static BindableProperty LeadingEmbeddedImageNameProperty =
        BindableProperty.Create(nameof(LeadingEmbeddedImageName), typeof(string), typeof(Chip),
            propertyChanged:
            async (bindable, oldValue, newValue) =>
            {
                if (bindable is Chip c)
                {
                    await c.SetLeadingSvgResource().ConfigureAwait(false);
                    c.InvalidateSurface();
                }
            });

    /// <summary>
    /// Gets or sets the name of the embedded image.
    /// </summary>
    /// <value>Takes a string. Default value is null.</value>
    public string LeadingEmbeddedImageName
    {
        get => (string)GetValue(LeadingEmbeddedImageNameProperty);
        set => SetValue(LeadingEmbeddedImageNameProperty, value);
    }

    /// <summary>
    /// The embedded svg image name property.
    /// </summary>
    public static BindableProperty TrailingEmbeddedImageNameProperty =
        BindableProperty.Create(nameof(TrailingEmbeddedImageName), typeof(string), typeof(Chip),
            propertyChanged:
            async (bindable, oldValue, newValue) =>
            {
                if (bindable is Chip c)
                {
                    await c.SetTrailingSvgResource().ConfigureAwait(false);
                    c.InvalidateSurface();
                }
            });

    /// <summary>
    /// Gets or sets the name of the embedded image.
    /// </summary>
    /// <value>Takes a string. Default value is null.</value>
    public string TrailingEmbeddedImageName
    {
        get => (string)GetValue(TrailingEmbeddedImageNameProperty);
        set => SetValue(TrailingEmbeddedImageNameProperty, value);
    }

    public static BindableProperty EmbeddedImageColorMatchesTextProperty =
        BindableProperty.Create(nameof(EmbeddedImageColorMatchesText), typeof(bool), typeof(Chip), true,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public bool EmbeddedImageColorMatchesText
    {
        get => (bool)GetValue(EmbeddedImageColorMatchesTextProperty);
        set => SetValue(EmbeddedImageColorMatchesTextProperty, value);
    }

    public static BindableProperty SpacingProperty =
        BindableProperty.Create(nameof(Spacing), typeof(double), typeof(Chip), 8d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName is null)
        {
            return;
        }

        if (propertyName.Equals(HeightProperty.PropertyName) ||
            propertyName.Equals(WidthProperty.PropertyName) ||
            propertyName.Equals(MarginProperty.PropertyName))
        {
            this.InvalidateSurface();
        }
    }

    public Chip()
    {
        this._cantHandleTouch = DeviceInfo.Platform == DevicePlatform.Android;

        HeightRequest = _minSize.Height;
        WidthRequest = _minSize.Width;
    }

    protected override void Attached() => this.EnableTouchEvents = true;

    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        var xOffset = 0f;

        this._rect = new SKRect(0f, 0f, info.Width, info.Height);

        var spacing = (float)this.Spacing * _scale;

        var verticalSpacing = new Thickness(0d, 8d);

        var halfHeight = this._rect.MidY;
        var quarterHeight = info.Height * .25f;

        var scaledBorderSize = (float)this.BorderSize * _scale;
        var halfScaledBorderSize = scaledBorderSize * .5f;

        xOffset += halfScaledBorderSize;

        canvas.Clear();

        var isToggled = this.IsToggled;
        var isReadonly = this.State == ChipState.ReadOnly;

        using var backgroundPaint = new SKPaint();

        backgroundPaint.IsAntialias = true;
        backgroundPaint.Style = SKPaintStyle.Fill;
        backgroundPaint.Color =
            (isToggled
                ? this.ToggledBackgroundColor
                : isReadonly
                    ? this.ReadOnlyBackgroundColor
                    : this.BackgroundColor)
            .ToSKColor();

        var fontColor =
            (isToggled && this.ToggledFontColor != default(Color)
                ? this.ToggledFontColor
                : isReadonly
                    ? this.ReadOnlyFontColor
                    : this.FontColor)
            .ToSKColor();

        var borderColor =
            (isToggled && this.ToggledBorderColor != default(Color)
                ? this.ToggledBorderColor
                : isReadonly
                    ? this.ReadOnlyBorderColor
                    : this.BorderColor)
            .ToSKColor();

        using (new SKAutoCanvasRestore(canvas))
        {
            if (this.BorderSize > default(double) && borderColor != SKColors.Transparent)
            {
                using var borderPaint = new SKPaint();
                borderPaint.IsAntialias = true;
                borderPaint.Style = SKPaintStyle.Stroke;
                borderPaint.StrokeWidth = scaledBorderSize;
                borderPaint.Color = borderColor;

                this._rect = this._rect.Subtract(scaledBorderSize);

                switch (this.Shape)
                {
                    case ChipShape.Standard:
                        canvas.DrawRoundRect(this._rect, this._rect.Height / 2f, this._rect.Height / 2f, borderPaint);
                        break;
                    case ChipShape.Rectangle:
                        canvas.DrawRect(this._rect, borderPaint);
                        break;
                    case ChipShape.RoundedRectangle:
                        var cornerRadius = (float)this.CornerRadius * _scale;
                        canvas.DrawRoundRect(this._rect, cornerRadius, cornerRadius, borderPaint);
                        break;
                }
            }

            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.SaveLayer(null);

                canvas.Clear();
                switch (this.Shape)
                {
                    case ChipShape.Standard:
                        canvas.DrawRoundRect(this._rect, this._rect.Height / 2f, this._rect.Height / 2f, backgroundPaint);
                        break;
                    case ChipShape.Rectangle:
                        canvas.DrawRect(this._rect, backgroundPaint);
                        break;
                    case ChipShape.RoundedRectangle:
                        var cornerRadius = (float)this.CornerRadius * _scale;
                        canvas.DrawRoundRect(this._rect, cornerRadius, cornerRadius, backgroundPaint);
                        break;
                }
            }

            xOffset += quarterHeight + spacing;

            if (!string.IsNullOrEmpty(this.LeadingEmbeddedImageName) && this._leadingSvg != null)
            {
                var thumbnailContainer = new SKRect(this._rect.Left, this._rect.Top, this._rect.Height, this._rect.Height);

                var imageSize = thumbnailContainer.AspectFit(this._leadingSvg.Picture.CullRect.Size).Subtract(verticalSpacing);

                var imageWidth = imageSize.Width;
                imageSize.Left = xOffset;
                imageSize.Right = imageWidth + xOffset;

                var scaleAmount = Math.Min(imageSize.Width / this._leadingSvg.Picture.CullRect.Width, imageSize.Height / this._leadingSvg.Picture.CullRect.Height);

                var svgScale = SKMatrix.CreateScale(scaleAmount, scaleAmount);

                var translation = SKMatrix.CreateTranslation(imageSize.Left, imageSize.Top + halfScaledBorderSize);

                svgScale = svgScale.PostConcat(translation);

                if (fontColor != SKColors.Transparent && this.EmbeddedImageColorMatchesText)
                {
                    using (new SKAutoCanvasRestore(canvas))
                    {
                        using var overlayPaint = new SKPaint();
                        overlayPaint.IsAntialias = true;
                        overlayPaint.BlendMode = SKBlendMode.SrcATop;
                        overlayPaint.Color = fontColor;

                        canvas.SaveLayer(null);

                        canvas.Clear();
                        canvas.DrawPicture(this._leadingSvg.Picture, ref svgScale);
                        canvas.DrawPaint(overlayPaint);
                    }
                }
                else
                {
                    canvas.DrawPicture(this._leadingSvg.Picture, ref svgScale);
                }

                xOffset = imageSize.Right + spacing;
            }

            if (!string.IsNullOrEmpty(this.Text))
            {
                using var fontPaint = new SKPaint();
                fontPaint.Color = fontColor;
                fontPaint.TextSize = (float)this.FontSize * _scale;
                fontPaint.IsAntialias = true;
                fontPaint.Typeface = this.Typeface ?? PlatformInfo.DefaultTypeface;

                if (this.IsIconifiedText)
                {
                    var iconifiedSize = canvas.IconifiedTextSize(this.Text, fontPaint);
                    canvas.DrawCenteredIconifiedText(this.Text, xOffset + ((float)iconifiedSize.Width / 2f), this._rect.MidY, fontPaint);
                    xOffset += (float)iconifiedSize.Width + spacing;
                }
                else
                {
                    fontPaint.EnsureHasValidFont(this.Text);
                    var textSize = canvas.TextSize(this.Text, fontPaint);
                    canvas.DrawCenteredText(this.Text, xOffset + ((float)textSize.Width / 2f), this._rect.MidY, fontPaint);
                    xOffset += (float)textSize.Width + spacing;
                }
            }

            if (!string.IsNullOrEmpty(this.TrailingEmbeddedImageName) && this._trailingSvg != null)
            {
                var thumbnailContainer = new SKRect(this._rect.Left, this._rect.Top, this._rect.Height, this._rect.Height);

                var imageSize = thumbnailContainer.AspectFit(this._trailingSvg.Picture.CullRect.Size).Subtract(verticalSpacing);

                var imageWidth = imageSize.Width;
                imageSize.Left = xOffset;
                imageSize.Right = xOffset + imageWidth;

                var scaleAmount = Math.Min(imageSize.Width / this._trailingSvg.Picture.CullRect.Width, imageSize.Height / this._trailingSvg.Picture.CullRect.Height);

                var svgScale = SKMatrix.CreateScale(scaleAmount, scaleAmount);

                var translation = SKMatrix.CreateTranslation(imageSize.Left, imageSize.Top + halfScaledBorderSize);

                svgScale = svgScale.PostConcat(translation);

                if (fontColor != SKColors.Transparent && this.EmbeddedImageColorMatchesText)
                {
                    using (new SKAutoCanvasRestore(canvas))
                    {
                        using var overlayPaint = new SKPaint();
                        overlayPaint.IsAntialias = true;
                        overlayPaint.BlendMode = SKBlendMode.SrcATop;
                        overlayPaint.Color = fontColor;

                        canvas.SaveLayer(null);

                        canvas.Clear();
                        canvas.DrawPicture(this._trailingSvg.Picture, ref svgScale);
                        canvas.DrawPaint(overlayPaint);
                    }
                }
                else
                {
                    canvas.DrawPicture(this._trailingSvg.Picture, ref svgScale);
                }

                xOffset = imageSize.Right + spacing;
            }

            if (this.IsRemovable)
            {
                var size = this._rect.Height / 2f;
                var top = this._rect.Top + ((this._rect.Height - size) / 2f);
                this._removeRect = new SKRect(xOffset, top, xOffset + size, top + size).Subtract(8d);

                using (new SKAutoCanvasRestore(canvas))
                {
                    using var overlayPaint = new SKPaint();
                    overlayPaint.IsAntialias = true;
                    overlayPaint.Style = SKPaintStyle.Fill;
                    overlayPaint.Color = fontColor;

                    canvas.SaveLayer(null);

                    canvas.Clear();
                    canvas.DrawCircle(this._removeRect.MidX, this._removeRect.MidY, size / 2f, overlayPaint);

                    overlayPaint.Style = SKPaintStyle.Stroke;
                    overlayPaint.StrokeCap = SKStrokeCap.Round;
                    overlayPaint.BlendMode = SKBlendMode.DstIn;
                    overlayPaint.Color = SKColors.Transparent;
                    overlayPaint.StrokeWidth = 3f;

                    var crossRect = this._removeRect.Subtract(this._removeRect.Width * .25f);

                    canvas.DrawLine(crossRect.Left, crossRect.Top + crossRect.Height, crossRect.Left + crossRect.Width, crossRect.Top, overlayPaint);
                    canvas.DrawLine(crossRect.Left + crossRect.Width, crossRect.Top + crossRect.Height, crossRect.Left, crossRect.Top, overlayPaint);
                }

                xOffset = this._removeRect.Right + spacing;
            }

            xOffset += quarterHeight + halfScaledBorderSize;
        }

        var scaledXOffset = xOffset / _scale;

        if (Math.Abs(scaledXOffset - _calculatedWidth) > .001f)
        {
            _calculatedWidth = scaledXOffset;
            Dispatcher.Dispatch(
                () =>
                {
                    this.InvalidateMeasureNonVirtual(InvalidationTrigger.HorizontalOptionsChanged);
                });
        }
    }

    protected override void OnTouch(SKTouchEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"{e.ActionType}");

        if (!this._cantHandleTouch)
        {
            e.Handled = true;
        }

        if (e.ActionType != SKTouchAction.Released && (!this._cantHandleTouch || e.ActionType != SKTouchAction.Pressed))
        {
            return;
        }

        this.Tapped?.Invoke(this, EventArgs.Empty);

        if (this.TappedCommand?.CanExecute(this.TappedCommandParameter) ?? false)
        {
            this.TappedCommand.Execute(this.TappedCommandParameter);
        }

        if (this.State == ChipState.ReadOnly)
        {
            return;
        }

        if (this.IsRemovable && this._removeRect.Contains(e.Location))
        {
            if (this.RemovedCommand?.CanExecute(this.RemovedCommandParameter) ?? false)
            {
                this.RemovedCommand.Execute(this.RemovedCommandParameter);
            }

            this.Removed?.Invoke(this, EventArgs.Empty);

            return;
        }

        if (!this._rect.Contains(e.Location))
        {
            return;
        }

        this.IsToggled = !this.IsToggled;
        this.InvalidateSurface();
    }

    /*
    protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
    {
        var sr = base.OnMeasure(widthConstraint, heightConstraint);

        var neededWidth = this._calculatedWidth > sr.Request.Width ? this._calculatedWidth : sr.Request.Width;
        var neededHeight = _minSize.Height < sr.Request.Height ? _minSize.Height : sr.Request.Height;

        // TODO: Need to add sizing provider
        return new SizeRequest(new Size(neededWidth, neededHeight), _minSize);
    }
    */

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        var size = base.MeasureOverride(widthConstraint, heightConstraint);

        if (_calculatedWidth > size.Width)
        {
            return new Size(this._calculatedWidth + Margin.HorizontalThickness, size.Height);
        }

        return size;
    }

    /// <summary>
    /// Sets the svg resource.
    /// </summary>
    private async Task SetLeadingSvgResource()
    {
        if (string.IsNullOrEmpty(this.LeadingEmbeddedImageName))
        {
            this._leadingSvg = new SKSvg();
            return;
        }

        this._leadingSvg = new SKSvg();

        await using var imageStream = EmbeddedResourceLoader.Load(this.LeadingEmbeddedImageName);
        this._leadingSvg.Load(imageStream);
        await imageStream.FlushAsync().ConfigureAwait(false);
    }

    private async Task SetTrailingSvgResource()
    {
        if (string.IsNullOrEmpty(this.TrailingEmbeddedImageName))
        {
            this._trailingSvg = new SKSvg();
            return;
        }

        this._trailingSvg = new SKSvg();

        await using var imageStream = EmbeddedResourceLoader.Load(this.TrailingEmbeddedImageName);
        this._trailingSvg.Load(imageStream);
        await imageStream.FlushAsync().ConfigureAwait(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        // Free unmanaged resources and set fields to null
        this._disposed = true;

        if (!disposing)
        {
            return;
        }

        // Dispose managed resources
        this._backgroundPath.Dispose();
        this._leadingSvg?.Dispose();
        this._trailingSvg?.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Chip()
    {
        Dispose(false);
    }
}
