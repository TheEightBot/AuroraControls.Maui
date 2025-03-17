using System.ComponentModel;
using Microsoft.Maui.Controls.Internals;

namespace AuroraControls;

/// <summary>
/// Cupertino (iOS style) Toggle Switch with text.
/// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class CupertinoTextToggleSwitch : AuroraViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private readonly SKPath _backgroundPath = new SKPath();
    private readonly string _animateToggleAnimationName;

    private float _calculatedWidth;

    private float _toggleAnimationPercentage;

    private SKPaint _backgroundPaint;

    private SKPaint _thumbPaint;

    public event EventHandler<ToggledEventArgs> Toggled;

    /// <summary>
    /// The color of the thumb switch.
    /// </summary>
    public static readonly BindableProperty ThumbColorProperty =
        BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(CupertinoTextToggleSwitch), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the thumb switch.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default is Color.White.</value>
    public Color ThumbColor
    {
        get => (Color)GetValue(ThumbColorProperty);
        set => SetValue(ThumbColorProperty, value);
    }

    /// <summary>
    /// The color of the track when enabled.
    /// </summary>
    public static readonly BindableProperty TrackEnabledColorProperty =
        BindableProperty.Create(nameof(TrackEnabledColor), typeof(Color), typeof(CupertinoTextToggleSwitch), Color.FromRgba("#5bcd58"),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the track enabled color.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default is Color.Green.</value>
    public Color TrackEnabledColor
    {
        get => (Color)GetValue(TrackEnabledColorProperty);
        set => SetValue(TrackEnabledColorProperty, value);
    }

    /// <summary>
    /// The color of the track when disabled.
    /// </summary>
    public static readonly BindableProperty TrackDisabledColorProperty =
        BindableProperty.Create(nameof(TrackDisabledColor), typeof(Color), typeof(CupertinoTextToggleSwitch), Color.FromRgba("#e9e9ea"),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the track disabled color.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default is Color.Red.</value>
    public Color TrackDisabledColor
    {
        get => (Color)GetValue(TrackDisabledColorProperty);
        set => SetValue(TrackDisabledColorProperty, value);
    }

    /// <summary>
    /// The border width property.
    /// </summary>
    public static readonly BindableProperty BorderWidthProperty =
        BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(CupertinoTextToggleSwitch), 2d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the width of the border.
    /// </summary>
    /// <value>Expects a double value. Default is 2d.</value>
    public double BorderWidth
    {
        get => (double)GetValue(BorderWidthProperty);
        set => SetValue(BorderWidthProperty, value);
    }

    /// <summary>
    /// The duration of the toggle animation.
    /// </summary>
    public static readonly BindableProperty ToggleAnimationDurationProperty =
        BindableProperty.Create(nameof(ToggleAnimationDuration), typeof(uint), typeof(CupertinoTextToggleSwitch), 240u,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the duration of the toggle animation.
    /// </summary>
    /// <value>Animation duration as an uint. Default is 240u.</value>
    [TypeConverter(typeof(UIntTypeConverter))]
    public uint ToggleAnimationDuration
    {
        get => (uint)GetValue(ToggleAnimationDurationProperty);
        set => SetValue(ToggleAnimationDurationProperty, value);
    }

    /// <summary>
    /// The command property. Fires on tap.
    /// </summary>
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(CupertinoTextToggleSwitch), default(ICommand));

    /// <summary>
    /// The toggle maximum width property.
    /// </summary>
    public static readonly BindableProperty ToggleMinWidthProperty =
        BindableProperty.Create(nameof(ToggleMinWidth), typeof(double), typeof(CupertinoTextToggleSwitch), 56d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the minimum width toggle.
    /// </summary>
    /// <value>Minimum width as a double. Default is 56d.</value>
    public double ToggleMinWidth
    {
        get => (double)GetValue(ToggleMinWidthProperty);
        set => SetValue(ToggleMinWidthProperty, value);
    }

    /// <summary>
    /// The toggle maximum height property.
    /// </summary>
    public static readonly BindableProperty ToggleMaxHeightProperty =
        BindableProperty.Create(nameof(ToggleMaxHeight), typeof(double), typeof(CupertinoTextToggleSwitch), 32d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the maximum height toggle.
    /// </summary>
    /// <value>Maximum height as a double. Default is 32d.</value>
    public double ToggleMaxHeight
    {
        get => (double)GetValue(ToggleMaxHeightProperty);
        set => SetValue(ToggleMaxHeightProperty, value);
    }

    /// <summary>
    /// The text on the toggle switch.
    /// </summary>
    public static readonly BindableProperty EnabledTextProperty =
        BindableProperty.Create(nameof(EnabledText), typeof(string), typeof(CupertinoTextToggleSwitch), default(string),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>string value for text. Default is default(string).</value>
    public string EnabledText
    {
        get => (string)GetValue(EnabledTextProperty);
        set => SetValue(EnabledTextProperty, value);
    }

    /// <summary>
    /// The text on the toggle switch.
    /// </summary>
    public static readonly BindableProperty DisabledTextProperty =
        BindableProperty.Create(nameof(DisabledText), typeof(string), typeof(CupertinoTextToggleSwitch), default(string),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>string value for text. Default is default(string).</value>
    public string DisabledText
    {
        get => (string)GetValue(DisabledTextProperty);
        set => SetValue(DisabledTextProperty, value);
    }

    /// <summary>
    /// The font color of the text.
    /// </summary>
    public static readonly BindableProperty DisabledFontColorProperty =
        BindableProperty.Create(nameof(DisabledFontColor), typeof(Color), typeof(CupertinoTextToggleSwitch), Color.FromArgb("#272727"),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the font.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default value is Color.White.</value>
    public Color DisabledFontColor
    {
        get => (Color)GetValue(DisabledFontColorProperty);
        set => SetValue(DisabledFontColorProperty, value);
    }

    public static readonly BindableProperty EnabledFontColorProperty =
        BindableProperty.Create(nameof(EnabledFontColor), typeof(Color), typeof(CupertinoTextToggleSwitch), default(Color),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public Color EnabledFontColor
    {
        get => (Color)GetValue(EnabledFontColorProperty);
        set => SetValue(EnabledFontColorProperty, value);
    }

    /// <summary>
    /// The font size of the text.
    /// </summary>
    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(CupertinoTextToggleSwitch), PlatformInfo.DefaultButtonFontSize,
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
    public static readonly BindableProperty TypefaceProperty =
        BindableProperty.Create(nameof(Typeface), typeof(SKTypeface), typeof(CupertinoTextToggleSwitch), default(SKTypeface),
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

    public static readonly BindableProperty IsIconifiedTextProperty =
        BindableProperty.Create(nameof(IsIconifiedText), typeof(bool), typeof(CupertinoTextToggleSwitch), default(bool),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public bool IsIconifiedText
    {
        get => (bool)GetValue(IsIconifiedTextProperty);
        set => SetValue(IsIconifiedTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>The command.</value>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// The command parameter property.
    /// </summary>
    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(CupertinoTextToggleSwitch), default(object));

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    /// <value>The command parameter. default value is default(object).</value>
    public object CommandParameter
    {
        get => (object)GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// The state of the toggle.
    /// </summary>
    public static readonly BindableProperty IsToggledProperty =
        BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(CupertinoTextToggleSwitch), false,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged:
                (bindable, value, newValue) =>
                {
                    if (bindable is not CupertinoTextToggleSwitch ctts || newValue is not bool nvBool)
                    {
                        return;
                    }

                    ctts.Toggled?.Invoke(ctts, new ToggledEventArgs(nvBool));
                    ctts.AnimateToggle(nvBool);
                });

    /// <summary>
    /// Gets or sets a value indicating whether the switch is toggled or not.
    /// </summary>
    /// <value><c>false</c> if not toggled, otherwise <c>true</c>.</value>
    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CupertinoTextToggleSwitch"/> class.
    /// </summary>
    public CupertinoTextToggleSwitch()
    {
        _animateToggleAnimationName = $"{nameof(AnimateToggle)}_{Guid.NewGuid()}";
        HeightRequest = ToggleMaxHeight;
        MinimumWidthRequest = ToggleMinWidth;
    }

    protected override void Attached()
    {
        this.EnableTouchEvents = true;

        _backgroundPaint = new SKPaint();
        _thumbPaint = new SKPaint();

        if (IsToggled)
        {
            _toggleAnimationPercentage = 1.0f;
        }

        base.Attached();
    }

    protected override void Detached()
    {
        _backgroundPaint?.Dispose();
        _thumbPaint?.Dispose();

        base.Detached();
    }

    /// <summary>
    /// This is the method used to draw our control on the SKCanvas. This method is fired every time <c>this.InvalidateSurface();</c> is called, resulting in a "redrawing" of the control.
    /// </summary>
    /// <param name="surface">The skia surface to paint on the controls.</param>
    /// <param name="info">Information about the skia image.</param>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        if (_backgroundPaint == null || _thumbPaint == null)
        {
            return;
        }

        var canvas = surface.Canvas;

        var rect = info.Rect;

        var borderWidth = (float)BorderWidth * _scale;

        var thumbSize = rect.Height - (borderWidth * 2f);
        var thumbLocation = rect.Left + borderWidth + ((rect.Width - thumbSize - (borderWidth * 2f)) * (float)_toggleAnimationPercentage);
        var thumbRect = new SKRect(thumbLocation, rect.Top + borderWidth, thumbLocation + thumbSize, rect.Top + borderWidth + thumbSize);
        var halfThumbRectWidth = thumbRect.Width * .5f;
        var trackColor =
            TrackDisabledColor
                .Lerp(TrackEnabledColor, _toggleAnimationPercentage)
                .ToSKColor();

        _backgroundPaint.IsAntialias = true;
        _backgroundPaint.Style = SKPaintStyle.Stroke;
        _backgroundPaint.StrokeWidth = (borderWidth * 2f) + (info.Height * (float)_toggleAnimationPercentage);
        _backgroundPaint.Color = trackColor;

        canvas.Clear();
        _backgroundPath.Reset();

        _backgroundPath.AddRoundRect(info.Rect, info.Rect.Height * .5f, info.Rect.Height * .5f);

        float instanceCalculatedWidth = 0.0f;

        _backgroundPaint.Style = SKPaintStyle.Fill;
        canvas.DrawPath(_backgroundPath, _backgroundPaint);

        canvas.ClipPath(_backgroundPath, SKClipOperation.Intersect, true);

        using var thumbClipPath = new SKPath();
        var thumbClip = new SKRect(borderWidth, borderWidth, rect.Right - borderWidth, rect.Bottom - borderWidth);
        thumbClipPath.AddRoundRect(thumbClip, thumbClip.Height * .5f, thumbClip.Height * .5f);

        instanceCalculatedWidth += borderWidth * 2f;

        Size enabledTextSize = default;
        Size disabledTextSize = default;

        if (!string.IsNullOrEmpty(EnabledText))
        {
            var fontColor =
                (EnabledFontColor != default(Color)
                    ? EnabledFontColor
                    : DisabledFontColor)
                    .ToSKColor()
                    .WithAlpha(_toggleAnimationPercentage);

            using var fontPaint = new SKPaint();
            fontPaint.Color = fontColor;
            fontPaint.TextSize = (float)FontSize * _scale;
            fontPaint.IsAntialias = true;
            fontPaint.Typeface = Typeface ?? PlatformInfo.DefaultTypeface;

            fontPaint.EnsureHasValidFont(EnabledText);

            if (IsIconifiedText)
            {
                enabledTextSize = canvas.IconifiedTextSize(EnabledText, fontPaint);
                canvas.DrawCenteredIconifiedText(EnabledText, rect.MidX - (halfThumbRectWidth * _toggleAnimationPercentage) - (borderWidth * .35f), rect.MidY, fontPaint);
            }
            else
            {
                enabledTextSize = canvas.TextSize(EnabledText, fontPaint);
                canvas.DrawCenteredText(EnabledText, rect.MidX - (halfThumbRectWidth * _toggleAnimationPercentage) - (borderWidth * .35f), rect.MidY, fontPaint);
            }
        }

        if (!string.IsNullOrEmpty(DisabledText))
        {
            var fontColor =
                DisabledFontColor
                    .ToSKColor()
                    .WithAlpha(1f - _toggleAnimationPercentage);

            using var fontPaint = new SKPaint();
            fontPaint.Color = fontColor;
            fontPaint.TextSize = (float)FontSize * _scale;
            fontPaint.IsAntialias = true;
            fontPaint.Typeface = Typeface ?? PlatformInfo.DefaultTypeface;

            fontPaint.EnsureHasValidFont(DisabledText);

            if (IsIconifiedText)
            {
                disabledTextSize = canvas.IconifiedTextSize(DisabledText, fontPaint);
                canvas.DrawCenteredIconifiedText(DisabledText, rect.MidX + (halfThumbRectWidth * (1f - _toggleAnimationPercentage)) + (borderWidth * .35f), rect.MidY, fontPaint);
            }
            else
            {
                disabledTextSize = canvas.TextSize(DisabledText, fontPaint);
                canvas.DrawCenteredText(DisabledText, rect.MidX + (halfThumbRectWidth * (1f - _toggleAnimationPercentage)) + (borderWidth * .35f), rect.MidY, fontPaint);
            }
        }

        instanceCalculatedWidth += (float)Math.Max(enabledTextSize.Width, disabledTextSize.Width);

        var thumbColor = ThumbColor.ToSKColor();
        _thumbPaint.Color = thumbColor;
        _thumbPaint.Style = SKPaintStyle.Fill;
        _thumbPaint.IsAntialias = true;

        var shadowSigma = SKMaskFilter.ConvertRadiusToSigma(borderWidth);
        _thumbPaint.ImageFilter =
            SKImageFilter
                .CreateDropShadow(
                    0, 0, shadowSigma, shadowSigma,
                    trackColor.Lerp(SKColors.Black, .9d));

        canvas.DrawOval(thumbRect, _thumbPaint);

        _thumbPaint.Color = thumbColor.Lerp(SKColors.DarkGray, .6d);
        _thumbPaint.StrokeWidth = borderWidth * .2f;
        _thumbPaint.ImageFilter = null;
        _thumbPaint.Style = SKPaintStyle.Stroke;
        canvas.DrawOval(thumbRect, _thumbPaint);

        // This gets calculated 2x because we need it as a buffer for the text.
        instanceCalculatedWidth += (float)thumbRect.Width * 1.7f;

        var scaledInstanceCalculatedWidth = instanceCalculatedWidth / _scale;

        if (Math.Abs(scaledInstanceCalculatedWidth - _calculatedWidth) > .001f)
        {
            _calculatedWidth = scaledInstanceCalculatedWidth;
            Dispatcher.Dispatch(
                () =>
                {
                    this.InvalidateMeasureNonVirtual(InvalidationTrigger.HorizontalOptionsChanged);
                });
        }
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        var size = base.MeasureOverride(widthConstraint, heightConstraint);

        if (_calculatedWidth > size.Width)
        {
            return new Size(this._calculatedWidth, size.Height);
        }

        return size;
    }

    /// <summary>
    /// SKCanvas method that fires on touch.
    /// </summary>
    /// <param name="e">Provides data for the SKCanvasView.Touch or SKGLView.Touch event.</param>
    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        var isTapInside = _backgroundPath.Contains(e.Location.X, e.Location.Y);

        if (e.ActionType != SKTouchAction.Released || !isTapInside)
        {
            return;
        }

        this.IsToggled = !this.IsToggled;

        if (this.Command?.CanExecute(this.CommandParameter) ?? false)
        {
            this.Command.Execute(this.CommandParameter);
        }
    }

    private void AnimateToggle(bool toggled) =>
        this.TransitionTo(
            _animateToggleAnimationName,
            x =>
            {
                _toggleAnimationPercentage = x;
                this.InvalidateSurface();
            },
            () => _toggleAnimationPercentage,
            toggled ? 1f : 0f,
            easing: Easing.CubicInOut,
            length: ToggleAnimationDuration);
}
