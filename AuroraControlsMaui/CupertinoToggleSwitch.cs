using System.ComponentModel;
using Microsoft.Maui.Animations;

namespace AuroraControls;

/// <summary>
/// Cupertino (iOS style) Toggle Switch.
/// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class CupertinoToggleSwitch : AuroraViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private readonly SKPath _backgroundPath = new SKPath();

    private readonly string _animateToggleAnimationName;

    private double _toggleAnimationPercentage;

    private SKPaint _backgroundPaint;
    private SKPaint _thumbPaint;

    public event EventHandler<ToggledEventArgs> Toggled;

    /// <summary>
    /// The color of the thumb switch.
    /// </summary>
    public static BindableProperty ThumbColorProperty =
        BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(CupertinoToggleSwitch), Colors.White,
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
    public static BindableProperty TrackEnabledColorProperty =
        BindableProperty.Create(nameof(TrackEnabledColor), typeof(Color), typeof(CupertinoToggleSwitch), Color.FromRgba("#5bcd58"),
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
    public static BindableProperty TrackDisabledColorProperty =
        BindableProperty.Create(nameof(TrackDisabledColor), typeof(Color), typeof(CupertinoToggleSwitch), Color.FromRgba("#e9e9ea"),
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
    public static BindableProperty BorderWidthProperty =
        BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(CupertinoToggleSwitch), 4d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the width of the border.
    /// </summary>
    /// <value>Expects a double value. Default is 4d.</value>
    public double BorderWidth
    {
        get { return (double)GetValue(BorderWidthProperty); }
        set { SetValue(BorderWidthProperty, value); }
    }

    /// <summary>
    /// The duration of the toggle animation.
    /// </summary>
    public static BindableProperty ToggleAnimationDurationProperty =
        BindableProperty.Create(nameof(ToggleAnimationDuration), typeof(uint), typeof(CupertinoToggleSwitch), 240u,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the duration of the toggle animation.
    /// </summary>
    /// <value>Animation duration as an uint. Default is 240u.</value>
    [TypeConverter(typeof(UIntTypeConverter))]
    public uint ToggleAnimationDuration
    {
        get { return (uint)GetValue(ToggleAnimationDurationProperty); }
        set { SetValue(ToggleAnimationDurationProperty, value); }
    }

    /// <summary>
    /// The command property. Fires on tap.
    /// </summary>
    public static BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(CupertinoToggleSwitch), default(ICommand));

    /// <summary>
    /// The toggle maximum width property.
    /// </summary>
    public static BindableProperty ToggleMaxWidthProperty =
        BindableProperty.Create(nameof(ToggleMaxWidth), typeof(double), typeof(CupertinoToggleSwitch), 56d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the maximum width toggle.
    /// </summary>
    /// <value>Maximum width as a double. Default is 56d.</value>
    public double ToggleMaxWidth
    {
        get => (double)GetValue(ToggleMaxWidthProperty);
        set => SetValue(ToggleMaxWidthProperty, value);
    }

    /// <summary>
    /// The toggle maximum height property.
    /// </summary>
    public static BindableProperty ToggleMaxHeightProperty =
        BindableProperty.Create(nameof(ToggleMaxHeight), typeof(double), typeof(CupertinoToggleSwitch), 32d,
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
    /// Gets or sets the command.
    /// </summary>
    /// <value>The command.</value>
    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    /// <summary>
    /// The command parameter property.
    /// </summary>
    public static BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(CupertinoToggleSwitch), default(object));

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    /// <value>The command parameter. default value is default(object).</value>
    public object CommandParameter
    {
        get { return (object)GetValue(CommandParameterProperty); }
        set { SetValue(CommandParameterProperty, value); }
    }

    /// <summary>
    /// The state of the toggle.
    /// </summary>
    public static BindableProperty IsToggledProperty =
        BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(CupertinoToggleSwitch), false,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether the switch is toggled or not.
    /// </summary>
    /// <value><c>false</c> if not toggled, otherwise <c>true</c>.</value>
    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set
        {
            SetValue(IsToggledProperty, value);
            AnimateToggle(value);
            Toggled?.Invoke(this, new ToggledEventArgs(value));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CupertinoToggleSwitch"/> class.
    /// </summary>
    public CupertinoToggleSwitch()
    {
        _animateToggleAnimationName = $"{nameof(AnimateToggle)}_{Guid.NewGuid()}";
        HeightRequest = ToggleMaxHeight;
        WidthRequest = ToggleMaxWidth;
    }

    protected override void Attached()
    {
        this.EnableTouchEvents = true;

        _backgroundPaint = new SKPaint();
        _thumbPaint = new SKPaint();

        if (IsToggled)
        {
            _toggleAnimationPercentage = 1.0d;
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

        var scale = info.Height / (float)Height;

        var toggleHeight = (float)ToggleMaxHeight * _scale;
        var toggleWidth = (float)ToggleMaxWidth * _scale;
        var halfToggleHeight = toggleHeight * .5f;
        var halfToggleWidth = toggleWidth * .5f;

        var rect = new SKRect(
            info.Rect.MidX - halfToggleWidth, info.Rect.MidY - halfToggleHeight,
            info.Rect.MidX + halfToggleWidth, info.Rect.MidY + halfToggleHeight);

        var borderWidth = (float)BorderWidth;

        var thumbSize = rect.Height - (borderWidth * 2f);
        var thumbLocation = rect.Left + borderWidth + ((toggleWidth - thumbSize - (borderWidth * 2f)) * (float)_toggleAnimationPercentage);
        var thumbRect = new SKRect(thumbLocation, rect.Top + borderWidth, thumbLocation + thumbSize, rect.Top + borderWidth + thumbSize);

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

        _backgroundPath.AddRoundRect(rect, rect.Height / 2f, rect.Height / 2f);

        using (new SKAutoCanvasRestore(canvas))
        {
            canvas.ClipPath(_backgroundPath, SKClipOperation.Intersect, true);

            _backgroundPaint.Style = SKPaintStyle.Fill;
            canvas.DrawPath(_backgroundPath, _backgroundPaint);

            using (var thumbClipPath = new SKPath())
            {
                var thumbClip = new SKRect(borderWidth, borderWidth, rect.Right - borderWidth, rect.Bottom - borderWidth);
                thumbClipPath.AddRoundRect(thumbClip, thumbClip.Height * .5f, thumbClip.Height * .5f);

                _thumbPaint.Color = ThumbColor.ToSKColor();
                _thumbPaint.Style = SKPaintStyle.Fill;
                _thumbPaint.IsAntialias = true;
                _thumbPaint.ImageFilter =
                    SKImageFilter
                        .CreateDropShadow(
                            0, 0, 4f, 4f,
                            SKColors.DarkGray);

                canvas.DrawOval(thumbRect, _thumbPaint);
            }
        }
    }

    /// <summary>
    /// SKCanvas method that fires on touch.
    /// </summary>
    /// <param name="e">Provides data for the SKCanvasView.Touch or SKGLView.Touch event.</param>
    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        var isTapInside = _backgroundPath.Contains(e.Location.X, e.Location.Y);

        if (e.ActionType == SKTouchAction.Released && isTapInside)
        {
            IsToggled = !IsToggled;
            AnimateToggle(IsToggled);

            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }
        }
    }

    private void AnimateToggle(bool toggled)
    {
        this.TransitionTo(
            nameof(_toggleAnimationPercentage),
            x =>
            {
                _toggleAnimationPercentage = x;
                this.InvalidateSurface();
            },
            () => _toggleAnimationPercentage,
            toggled ? 1d : 0d,
            easing: Easing.CubicInOut,
            length: ToggleAnimationDuration);
    }
}
