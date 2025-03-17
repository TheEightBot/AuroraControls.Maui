using System;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;

namespace AuroraControls;

public enum ToggleBoxShape
{
    Square,
    Circular,
    RoundedSquare,
}

public enum ToggleBoxCheckType
{
    Cross,
    Check,
    RoundedCheck,
    Circular,
}

[ContentProperty(nameof(IsToggled))]
public class ToggleBox : AuroraViewBase
{
    private SKRect _rect;
    private bool _holding;

    public event EventHandler<bool> Toggled;

    /// <summary>
    /// The state of the toggle.
    /// </summary>
    public static readonly BindableProperty IsToggledProperty =
        BindableProperty.Create(
            nameof(IsToggled),
            typeof(bool),
            typeof(ToggleBox),
            false,
            BindingMode.TwoWay,
            propertyChanged:
                (bindable, _, newValue) =>
                {
                    if (bindable is not ToggleBox tb || newValue is not bool boolVal)
                    {
                        return;
                    }

                    tb.InvalidateSurface();
                    tb.Toggled?.Invoke(tb, boolVal);
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

    /// <summary>
    /// The border color.
    /// </summary>
    public static readonly BindableProperty BorderColorProperty =
        BindableProperty.Create(
            nameof(BorderColor),
            typeof(Color),
            typeof(ToggleBox),
            Colors.Transparent,
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

    /// <summary>
    /// The color of the background.
    /// </summary>
    public static new BindableProperty BackgroundColorProperty =
        BindableProperty.Create(
            nameof(BackgroundColor),
            typeof(Color),
            typeof(ToggleBox),
            Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    /// <value>Expects a Color. Default Color.Transparent.</value>
    public new Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }

    /// <summary>
    /// The color of the background when toggled.
    /// </summary>
    public static readonly BindableProperty ToggledBackgroundColorProperty =
        BindableProperty.Create(
            nameof(ToggledBackgroundColor),
            typeof(Color),
            typeof(ToggleBox),
            Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the background color when toggled.
    /// </summary>
    /// <value>Expects a Color. Default Color.Transparent.</value>
    public Color ToggledBackgroundColor
    {
        get => (Color)GetValue(ToggledBackgroundColorProperty);
        set => SetValue(ToggledBackgroundColorProperty, value);
    }

    /// <summary>
    /// The color of the checkmark.
    /// </summary>
    public static readonly BindableProperty CheckColorProperty =
        BindableProperty.Create(
            nameof(CheckColor),
            typeof(Color),
            typeof(ToggleBox),
            Colors.Black,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the checkmark color.
    /// </summary>
    /// <value>Expects a Color. Default Color.Black.</value>
    public Color CheckColor
    {
        get => (Color)GetValue(CheckColorProperty);
        set => SetValue(CheckColorProperty, value);
    }

    /// <summary>
    /// The border width.
    /// </summary>
    public static readonly BindableProperty BorderWidthProperty =
        BindableProperty.Create(
            nameof(BorderWidth),
            typeof(int),
            typeof(ToggleBox),
            6,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the border width.
    /// </summary>
    /// <value>The border width as an int. Default is 6.</value>
    public int BorderWidth
    {
        get => (int)GetValue(BorderWidthProperty);
        set => SetValue(BorderWidthProperty, value);
    }

    /// <summary>
    /// The width of the checkmark.
    /// </summary>
    public static readonly BindableProperty MarkWidthProperty =
        BindableProperty.Create(
            nameof(MarkWidth),
            typeof(int),
            typeof(ToggleBox),
            6,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the width of the checkmark.
    /// </summary>
    /// <value>The border width as an int. Default is 6.</value>
    public int MarkWidth
    {
        get => (int)GetValue(MarkWidthProperty);
        set => SetValue(MarkWidthProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(
            nameof(CornerRadius),
            typeof(double),
            typeof(ToggleBox),
            8d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// The shape of the checkbox.
    /// </summary>
    public static readonly BindableProperty ShapeProperty =
        BindableProperty.Create(
            nameof(Shape),
            typeof(ToggleBoxShape),
            typeof(ToggleBox),
            ToggleBoxShape.Circular,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets shape of the checkbox.
    /// </summary>
    /// <value>Takes a CheckBoxShape. Default is CheckBoxShape.Circular.</value>
    public ToggleBoxShape Shape
    {
        get => (ToggleBoxShape)GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    /// <summary>
    /// The type of checkmark used.
    /// </summary>
    public static readonly BindableProperty CheckTypeProperty =
        BindableProperty.Create(
            nameof(ToggleBoxCheckType),
            typeof(ToggleBoxCheckType),
            typeof(ToggleBox),
            ToggleBoxCheckType.Check,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets type of checkmark.
    /// </summary>
    /// <value>Takes a CheckBoxCheckType. Default is CheckBoxCheckType.Check.</value>
    public ToggleBoxCheckType CheckType
    {
        get => (ToggleBoxCheckType)GetValue(CheckTypeProperty);
        set => SetValue(CheckTypeProperty, value);
    }

    /// <summary>
    /// The value of the checkbox.
    /// </summary>
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(object), typeof(ToggleBox));

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>object's value.</value>
    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public ToggleBox()
    {
        this.EnableTouchEvents = true;
        MinimumHeightRequest = IAuroraView.SmallControlHeight;
        MinimumWidthRequest = IAuroraView.SmallControlWidth;
    }

    public override Size CustomMeasuredSize(double widthConstraint, double heightConstraint)
    {
        return new Size(IAuroraView.SmallControlWidth, IAuroraView.SmallControlHeight);
    }

    protected override void PaintControl(SKSurface surface, SKImageInfo info)
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
                case ToggleBoxShape.Circular:
                    canvas.DrawOval(_rect, backgroundPaint);
                    break;
                case ToggleBoxShape.RoundedSquare:
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
                        case ToggleBoxCheckType.Check:
                            canvas.DrawLine(_rect.Left + (_rect.Width * .3f), _rect.Top + (_rect.Height * .5f),
                                _rect.Left + (_rect.Width * .5f), _rect.Top + (_rect.Height * .7f), checkPaint);
                            canvas.DrawLine(_rect.Left + (_rect.Width * .5f), _rect.Top + (_rect.Height * .7f),
                                _rect.Left + (_rect.Width * .75f), _rect.Top + (_rect.Height * .3f), checkPaint);
                            break;
                        case ToggleBoxCheckType.RoundedCheck:
                            checkPaint.StrokeCap = SKStrokeCap.Round;
                            canvas.DrawLine(_rect.Left + (_rect.Width * .3f), _rect.Top + (_rect.Height * .5f),
                                _rect.Left + (_rect.Width * .5f), _rect.Top + (_rect.Height * .7f), checkPaint);
                            canvas.DrawLine(_rect.Left + (_rect.Width * .5f), _rect.Top + (_rect.Height * .7f),
                                _rect.Left + (_rect.Width * .75f), _rect.Top + (_rect.Height * .3f), checkPaint);
                            break;
                        case ToggleBoxCheckType.Cross:
                            canvas.DrawLine(_rect.Left + MarkWidth, _rect.Top + _rect.Height - MarkWidth,
                                _rect.Left + _rect.Width - MarkWidth, _rect.Top + MarkWidth, checkPaint);
                            canvas.DrawLine(_rect.Left + _rect.Width - MarkWidth, _rect.Top + _rect.Height - MarkWidth,
                                _rect.Left + MarkWidth, _rect.Top + MarkWidth, checkPaint);
                            break;
                        case ToggleBoxCheckType.Circular:
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
                case ToggleBoxShape.Circular:
                    canvas.DrawOval(_rect, backgroundPaint);
                    break;
                case ToggleBoxShape.RoundedSquare:
                    canvas.DrawRoundRect(_rect, cornerRadius, backgroundPaint);
                    break;
                default:
                    canvas.DrawRect(_rect, backgroundPaint);
                    break;
            }
        }
    }

    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        if (!e.InContact)
        {
            this._holding = false;
            return;
        }

        if (this._holding == false && _rect.Contains(e.Location))
        {
            IsToggled = !IsToggled;
            this._holding = e.InContact;
            this.InvalidateSurface();
        }
    }
}
