using System;
using Microsoft.Maui.Graphics;

namespace AuroraControls;

public enum CheckBoxShape
{
    Square,
    Circular,
    RoundedSquare,
}

public enum CheckBoxCheckType
{
    Cross,
    Check,
    RoundedCheck,
    Circular,
}

[ContentProperty(nameof(IsToggled))]
public class CheckBox : AuroraViewBase
{
    private SKRect _rect;

    public event EventHandler<bool> Toggled;

    /// <summary>
    /// The state of the toggle.
    /// </summary>
    public static BindableProperty IsToggledProperty =
        BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(CheckBox), false,
            propertyChanged:
                (bindable, oldValue, newValue) =>
                {
                    if (bindable is CheckBox cb && newValue is bool boolVal)
                    {
                        cb?.InvalidateSurface();
                        cb?.Toggled?.Invoke(cb, boolVal);
                    }
                });

    /// <summary>
    /// Gets or sets a value indicating whether the box is toggled or not.
    /// </summary>
    /// <value><c>false</c> if not toggled, otherwise <c>true</c>.</value>
    public bool IsToggled
    {
        get { return (bool)GetValue(IsToggledProperty); }
        set { SetValue(IsToggledProperty, value); }
    }

    /// <summary>
    /// The border color.
    /// </summary>
    public static BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(CheckBox), Colors.Transparent,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

    /// <summary>
    /// Gets or sets the border color.
    /// </summary>
    /// <value>Color of the border.</value>
    public Color BorderColor
    {
        get { return (Color)GetValue(BorderColorProperty); }
        set { SetValue(BorderColorProperty, value); }
    }

    /// <summary>
    /// The color of the background.
    /// </summary>
    public static new BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(CheckBox), Colors.Transparent,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    /// <value>Expects a Color. Default Color.Transparent.</value>
    public new Color BackgroundColor
    {
        get { return (Color)GetValue(BackgroundColorProperty); }
        set { SetValue(BackgroundColorProperty, value); }
    }

    /// <summary>
    /// The color of the background when toggled.
    /// </summary>
    public static BindableProperty ToggledBackgroundColorProperty =
        BindableProperty.Create(nameof(ToggledBackgroundColor), typeof(Color), typeof(CheckBox), Colors.Transparent,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

    /// <summary>
    /// Gets or sets the background color when toggled.
    /// </summary>
    /// <value>Expects a Color. Default Color.Transparent.</value>
    public Color ToggledBackgroundColor
    {
        get { return (Color)GetValue(ToggledBackgroundColorProperty); }
        set { SetValue(ToggledBackgroundColorProperty, value); }
    }

    /// <summary>
    /// The color of the checkmark.
    /// </summary>
    public static BindableProperty CheckColorProperty =
        BindableProperty.Create(nameof(CheckColor), typeof(Color), typeof(CheckBox), Colors.Black,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

    /// <summary>
    /// Gets or sets the checkmark color.
    /// </summary>
    /// <value>Expects a Color. Default Color.Black.</value>
    public Color CheckColor
    {
        get { return (Color)GetValue(CheckColorProperty); }
        set { SetValue(CheckColorProperty, value); }
    }

    /// <summary>
    /// The border width.
    /// </summary>
    public static BindableProperty BorderWidthProperty =
        BindableProperty.Create(nameof(BorderWidth), typeof(int), typeof(CheckBox), 6,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

    /// <summary>
    /// Gets or sets the border width.
    /// </summary>
    /// <value>The border width as an int. Default is 6.</value>
    public int BorderWidth
    {
        get { return (int)GetValue(BorderWidthProperty); }
        set { SetValue(BorderWidthProperty, value); }
    }

    /// <summary>
    /// The width of the checkmark.
    /// </summary>
    public static BindableProperty MarkWidthProperty =
        BindableProperty.Create(nameof(MarkWidth), typeof(int), typeof(CheckBox), 6,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

    /// <summary>
    /// Gets or sets the width of the checkmark.
    /// </summary>
    /// <value>The border width as an int. Default is 6.</value>
    public int MarkWidth
    {
        get { return (int)GetValue(MarkWidthProperty); }
        set { SetValue(MarkWidthProperty, value); }
    }

    public static BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(CheckBox), 8d,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// The shape of the checkbox.
    /// </summary>
    public static BindableProperty ShapeProperty =
        BindableProperty.Create(nameof(Shape), typeof(CheckBoxShape), typeof(CheckBox), CheckBoxShape.Circular,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

    /// <summary>
    /// Gets or sets shape of the checkbox.
    /// </summary>
    /// <value>Takes a CheckBoxShape. Default is CheckBoxShape.Circular.</value>
    public CheckBoxShape Shape
    {
        get { return (CheckBoxShape)GetValue(ShapeProperty); }
        set { SetValue(ShapeProperty, value); }
    }

    /// <summary>
    /// The type of checkmark used.
    /// </summary>
    public static BindableProperty CheckTypeProperty =
        BindableProperty.Create(nameof(CheckType), typeof(CheckBoxCheckType), typeof(CheckBox), CheckBoxCheckType.Check,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

    /// <summary>
    /// Gets or sets type of checkmark.
    /// </summary>
    /// <value>Takes a CheckBoxCheckType. Default is CheckBoxCheckType.Check.</value>
    public CheckBoxCheckType CheckType
    {
        get { return (CheckBoxCheckType)GetValue(CheckTypeProperty); }
        set { SetValue(CheckTypeProperty, value); }
    }

    /// <summary>
    /// The value of the checkbox.
    /// </summary>
    public static BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(object), typeof(CheckBox), default(object));

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>object's value.</value>
    public object Value
    {
        get { return (object)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public CheckBox()
    {
        this.EnableTouchEvents = true;
        MinimumHeightRequest = 36;
        MinimumWidthRequest = 36;
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName.Equals(VisualElement.HeightProperty.PropertyName) ||
            propertyName.Equals(VisualElement.WidthProperty.PropertyName) ||
            propertyName.Equals(View.MarginProperty.PropertyName))
        {
            this.InvalidateSurface();
        }
    }

    protected override void PaintControl(SkiaSharp.SKSurface surface, SkiaSharp.SKImageInfo info)
    {
        var canvas = surface.Canvas;

        canvas.Clear();

        var rect = new SKRect(0, 0, info.Height, info.Height);
        var halfBorderWidth = BorderWidth / 2f;

        var cornerRadius = new SKSize((float)CornerRadius, (float)CornerRadius);

        using (var backgroundPaint = new SKPaint())
        {
            var halfCheckSize = info.Height * .5f;

            _rect = new SKRect(
                info.Rect.MidX - halfCheckSize + halfBorderWidth, info.Rect.MidY - halfCheckSize + halfBorderWidth,
                info.Rect.MidX + halfCheckSize - halfBorderWidth, info.Rect.MidY + halfCheckSize - halfBorderWidth);

            backgroundPaint.Color =
                IsToggled
                    ? ToggledBackgroundColor.ToSKColor()
                    : BackgroundColor.ToSKColor();

            backgroundPaint.IsAntialias = true;
            backgroundPaint.Style = SKPaintStyle.Fill;

            // draw fill
            switch (this.Shape)
            {
                case CheckBoxShape.Circular:
                    canvas.DrawOval(_rect, backgroundPaint);
                    break;
                case CheckBoxShape.RoundedSquare:
                    canvas.DrawRoundRect(_rect, cornerRadius, backgroundPaint);
                    break;
                default:
                    canvas.DrawRect(_rect, backgroundPaint);
                    break;
            }

            var innerRect = new SKRect
            {
                Left = _rect.Left + halfBorderWidth,
                Right = _rect.Right - halfBorderWidth,
                Top = _rect.Top + halfBorderWidth,
                Bottom = _rect.Bottom - halfBorderWidth,
            };

            var outerRect = new SKRect
            {
                Left = _rect.Left - halfBorderWidth,
                Right = _rect.Right + halfBorderWidth,
                Top = _rect.Top - halfBorderWidth,
                Bottom = _rect.Bottom + halfBorderWidth,
            };

            if (IsToggled)
            {
                using (var checkPaint = new SKPaint())
                {
                    checkPaint.Style = SKPaintStyle.Stroke;
                    checkPaint.Color = CheckColor.ToSKColor();
                    checkPaint.StrokeWidth = MarkWidth;
                    checkPaint.StrokeCap = SKStrokeCap.Square;
                    checkPaint.IsAntialias = true;

                    switch (CheckType)
                    {
                        case CheckBoxCheckType.Check:
                            canvas.DrawLine(_rect.Left + (_rect.Width * .3f), _rect.Top + (_rect.Height * .5f),
                                _rect.Left + (_rect.Width * .5f), _rect.Top + (_rect.Height * .7f), checkPaint);
                            canvas.DrawLine(_rect.Left + (_rect.Width * .5f), _rect.Top + (_rect.Height * .7f),
                                _rect.Left + (_rect.Width * .75f), _rect.Top + (_rect.Height * .3f), checkPaint);
                            break;
                        case CheckBoxCheckType.RoundedCheck:
                            checkPaint.StrokeCap = SKStrokeCap.Round;
                            canvas.DrawLine(_rect.Left + (_rect.Width * .3f), _rect.Top + (_rect.Height * .5f),
                                _rect.Left + (_rect.Width * .5f), _rect.Top + (_rect.Height * .7f), checkPaint);
                            canvas.DrawLine(_rect.Left + (_rect.Width * .5f), _rect.Top + (_rect.Height * .7f),
                                _rect.Left + (_rect.Width * .75f), _rect.Top + (_rect.Height * .3f), checkPaint);
                            break;
                        case CheckBoxCheckType.Cross:
                            canvas.DrawLine(_rect.Left + MarkWidth, _rect.Top + _rect.Height - MarkWidth,
                                _rect.Left + _rect.Width - MarkWidth, _rect.Top + MarkWidth, checkPaint);
                            canvas.DrawLine(_rect.Left + _rect.Width - MarkWidth, _rect.Top + _rect.Height - MarkWidth,
                                _rect.Left + MarkWidth, _rect.Top + MarkWidth, checkPaint);
                            break;
                        case CheckBoxCheckType.Circular:
                            checkPaint.Style = SKPaintStyle.Fill;
                            canvas.DrawCircle(_rect.MidX, rect.MidY, innerRect.Width * .20f, checkPaint);
                            break;
                    }
                }
            }

            backgroundPaint.StrokeWidth = BorderWidth;
            backgroundPaint.Style = SKPaintStyle.Stroke;
            backgroundPaint.Color = BorderColor.ToSKColor();

            switch (this.Shape)
            {
                case CheckBoxShape.Circular:
                    canvas.DrawOval(_rect, backgroundPaint);
                    break;
                case CheckBoxShape.RoundedSquare:
                    canvas.DrawRoundRect(_rect, cornerRadius, backgroundPaint);
                    break;
                default:
                    canvas.DrawRect(_rect, backgroundPaint);
                    break;
            }
        }
    }

    protected bool holding = false;

    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        if (!e.InContact)
        {
            holding = false;
            return;
        }

        if (holding == false && _rect.Contains(e.Location))
        {
            IsToggled = !IsToggled;
            holding = e.InContact;
            this.InvalidateSurface();
        }
    }
}
