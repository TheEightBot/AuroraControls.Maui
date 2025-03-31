using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AuroraControls.Fluent;

/// <summary>
/// Represents a Fluent 2 design style rating display component based on AuroraViewBase.
/// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class FluentRatingDisplay : AuroraViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private readonly SKPaint _starPaint = new();
    private readonly SKPaint _textPaint = new();
    private readonly SKPath _starPath = new();

    /// <summary>
    /// The current rating value property.
    /// </summary>
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(float), typeof(FluentRatingDisplay), 0f,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the current rating value.
    /// </summary>
    public float Value
    {
        get => (float)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// The maximum rating property.
    /// </summary>
    public static readonly BindableProperty MaxProperty =
        BindableProperty.Create(nameof(Max), typeof(int), typeof(FluentRatingDisplay), 5,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the maximum rating value.
    /// </summary>
    public int Max
    {
        get => (int)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    /// <summary>
    /// The star size property.
    /// </summary>
    public static readonly BindableProperty StarSizeProperty =
        BindableProperty.Create(nameof(StarSize), typeof(RatingSize), typeof(FluentRatingDisplay), RatingSize.Medium,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the size of stars.
    /// </summary>
    public RatingSize StarSize
    {
        get => (RatingSize)GetValue(StarSizeProperty);
        set => SetValue(StarSizeProperty, value);
    }

    /// <summary>
    /// The color property.
    /// </summary>
    public static readonly BindableProperty ColorProperty =
        BindableProperty.Create(nameof(Color), typeof(Color), typeof(FluentRatingDisplay), null,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color for the filled stars.
    /// </summary>
    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// The rest color property (for unfilled stars).
    /// </summary>
    public static readonly BindableProperty RestColorProperty =
        BindableProperty.Create(nameof(RestColor), typeof(Color), typeof(FluentRatingDisplay), null,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color for unfilled stars.
    /// </summary>
    public Color RestColor
    {
        get => (Color)GetValue(RestColorProperty);
        set => SetValue(RestColorProperty, value);
    }

    /// <summary>
    /// The show count property.
    /// </summary>
    public static readonly BindableProperty ShowCountProperty =
        BindableProperty.Create(nameof(ShowCount), typeof(bool), typeof(FluentRatingDisplay), false,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether to display the count of ratings.
    /// </summary>
    public bool ShowCount
    {
        get => (bool)GetValue(ShowCountProperty);
        set => SetValue(ShowCountProperty, value);
    }

    /// <summary>
    /// The count property.
    /// </summary>
    public static readonly BindableProperty CountProperty =
        BindableProperty.Create(nameof(Count), typeof(int), typeof(FluentRatingDisplay), 0,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the number of ratings.
    /// </summary>
    public int Count
    {
        get => (int)GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    /// <summary>
    /// The precision property.
    /// </summary>
    public static readonly BindableProperty PrecisionProperty =
        BindableProperty.Create(nameof(Precision), typeof(RatingPrecision), typeof(FluentRatingDisplay),
            RatingPrecision.Half,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the precision for the rating display.
    /// </summary>
    public RatingPrecision Precision
    {
        get => (RatingPrecision)GetValue(PrecisionProperty);
        set => SetValue(PrecisionProperty, value);
    }

    /// <summary>
    /// The appearance property.
    /// </summary>
    public static readonly BindableProperty AppearanceProperty =
        BindableProperty.Create(nameof(Appearance), typeof(RatingAppearance), typeof(FluentRatingDisplay),
            RatingAppearance.Filled,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the appearance style of the rating display.
    /// </summary>
    public RatingAppearance Appearance
    {
        get => (RatingAppearance)GetValue(AppearanceProperty);
        set => SetValue(AppearanceProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentRatingDisplay"/> class.
    /// </summary>
    public FluentRatingDisplay()
    {
        MinimumHeightRequest = IAuroraView.StandardControlHeight;

        // Initialize the paints
        _starPaint.IsAntialias = true;
        _starPaint.Style = SKPaintStyle.Fill;

        _textPaint.IsAntialias = true;
        _textPaint.TextAlign = SKTextAlign.Left;
        _textPaint.Color = SKColors.Black;
        _textPaint.Typeface = PlatformInfo.DefaultTypeface;
    }

    /// <summary>
    /// Override to handle drawing of the control.
    /// </summary>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;
        canvas.Clear();

        // Set up star size based on control settings
        float starSizeValue = GetStarSizeValue();

        // Calculate spacing between stars (20% of star size)
        float spacing = starSizeValue * 0.2f;

        // Set up colors
        SKColor filledColor = GetFilledStarColor().ToSKColor();
        SKColor restColor = GetRestStarColor().ToSKColor();

        // Create the star shape if not already created
        if (_starPath.IsEmpty)
        {
            CreateStarPath(starSizeValue);
        }

        // Calculate total width needed for stars
        float totalStarsWidth = (Max * starSizeValue) + ((Max - 1) * spacing);

        // Start position for the first star
        float startX = 0;
        float startY = (info.Height - starSizeValue) / 2;

        // Draw each star
        for (int i = 0; i < Max; i++)
        {
            float currentStarX = startX + (i * (starSizeValue + spacing));
            float currentStarPosition = i + 1;

            // Determine star fill percentage based on value and precision
            float fillPercentage = DetermineFillPercentage(currentStarPosition);

            // Draw the star based on appearance
            DrawStar(canvas, currentStarX, startY, starSizeValue, fillPercentage, filledColor, restColor);
        }

        // Draw count text if enabled
        if (ShowCount && Count > 0)
        {
            float countTextX = startX + totalStarsWidth + spacing;

            // Set text size based on star size
            _textPaint.TextSize = starSizeValue * 0.7f;
            string countText = $"({Count})";

            // Measure the text to properly center it vertically
            var textBounds = default(SKRect);
            _textPaint.MeasureText(countText, ref textBounds);

            // Calculate the Y position to vertically center the text with the stars
            // This takes into account the full height of the text including ascent and descent
            float countTextY = (info.Height / 2f) + ((textBounds.Height / 2f) - textBounds.Bottom);

            canvas.DrawText(countText, countTextX, countTextY, _textPaint);
        }
    }

    private float GetStarSizeValue()
    {
        // Convert enum size to actual pixel values scaled for device
        switch (StarSize)
        {
            case RatingSize.Small:
                return 16f * _scale;
            case RatingSize.Large:
                return 32f * _scale;
            case RatingSize.Medium:
            default:
                return 24f * _scale;
        }
    }

    private Color GetFilledStarColor()
    {
        // Return custom color or default Fluent color
        if (Color != null)
        {
            return Color;
        }

        return Color.FromArgb("#E24C00"); // Fluent default star color
    }

    private Color GetRestStarColor()
    {
        // Return custom rest color or default gray color
        if (RestColor != null)
        {
            return RestColor;
        }

        return Color.FromArgb("#BDBDBD"); // Default gray color for unfilled stars
    }

    private float DetermineFillPercentage(float starPosition)
    {
        float value = Math.Min(Value, Max);

        // Calculate fill percentage based on precision setting
        switch (Precision)
        {
            case RatingPrecision.Full:
                // Either completely filled or completely empty
                return (value >= starPosition) ? 1.0f : 0.0f;

            case RatingPrecision.Quarter:
                // Can be filled in quarter increments
                if (value >= starPosition)
                {
                    return 1.0f;
                }
                else if (value <= starPosition - 1)
                {
                    return 0.0f;
                }
                else
                {
                    float fraction = value - (starPosition - 1);

                    // Round to nearest quarter
                    return (float)Math.Round(fraction * 4) / 4;
                }

            case RatingPrecision.Half:
            default:
                // Can be filled in half increments
                if (value >= starPosition)
                {
                    return 1.0f;
                }
                else if (value <= starPosition - 1)
                {
                    return 0.0f;
                }
                else
                {
                    float fraction = value - (starPosition - 1);

                    // Round to nearest half
                    return (float)Math.Round(fraction * 2) / 2;
                }
        }
    }

    private void CreateStarPath(float size)
    {
        _starPath.Reset();

        // Star points for a 5-pointed star
        const int numberOfPoints = 5;
        float outerRadius = size / 2;
        float innerRadius = outerRadius * 0.4f;
        float centerX = outerRadius;
        float centerY = outerRadius;

        // Start at the top point
        double angle = -Math.PI / 2; // -90 degrees
        float x = centerX;
        float y = centerY - outerRadius;
        _starPath.MoveTo(x, y);

        // Create the star shape
        for (int i = 0; i < numberOfPoints; i++)
        {
            // Outer point
            angle += Math.PI / numberOfPoints;
            x = centerX + (float)(Math.Cos(angle) * innerRadius);
            y = centerY + (float)(Math.Sin(angle) * innerRadius);
            _starPath.LineTo(x, y);

            // Inner point
            angle += Math.PI / numberOfPoints;
            x = centerX + (float)(Math.Cos(angle) * outerRadius);
            y = centerY + (float)(Math.Sin(angle) * outerRadius);
            _starPath.LineTo(x, y);
        }

        _starPath.Close();
    }

    private void DrawStar(SKCanvas canvas, float x, float y, float size, float fillPercentage, SKColor filledColor,
        SKColor restColor)
    {
        using (new SKAutoCanvasRestore(canvas))
        {
            canvas.Translate(x, y);

            switch (Appearance)
            {
                case RatingAppearance.Regular:
                    // For regular appearance, draw outline stars
                    _starPaint.Style = SKPaintStyle.Stroke;
                    _starPaint.StrokeWidth = size * 0.1f;
                    _starPaint.Color = (fillPercentage > 0) ? filledColor : restColor;
                    canvas.DrawPath(_starPath, _starPaint);
                    break;

                case RatingAppearance.Filled:
                default:
                    // For filled appearance with partial fill
                    if (fillPercentage > 0 && fillPercentage < 1.0f)
                    {
                        // Draw the unfilled part first
                        _starPaint.Style = SKPaintStyle.Fill;
                        _starPaint.Color = restColor;
                        canvas.DrawPath(_starPath, _starPaint);

                        // Then draw the filled part with clipping
                        using (new SKAutoCanvasRestore(canvas))
                        {
                            // Create clip rect for partial fill
                            canvas.ClipRect(new SKRect(0, 0, size * fillPercentage, size));

                            _starPaint.Color = filledColor;
                            canvas.DrawPath(_starPath, _starPaint);
                        }
                    }
                    else
                    {
                        // Draw completely filled or unfilled star
                        _starPaint.Style = SKPaintStyle.Fill;
                        _starPaint.Color = (fillPercentage >= 1.0f) ? filledColor : restColor;
                        canvas.DrawPath(_starPath, _starPaint);
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Cleanly dispose of resources.
    /// </summary>
    protected override void Detached()
    {
        _starPath.Dispose();
        _starPaint.Dispose();
        _textPaint.Dispose();

        base.Detached();
    }
}

/// <summary>
/// Defines the size options for rating stars.
/// </summary>
public enum RatingSize
{
    /// <summary>
    /// Small size stars.
    /// </summary>
    Small,

    /// <summary>
    /// Medium size stars.
    /// </summary>
    Medium,

    /// <summary>
    /// Large size stars.
    /// </summary>
    Large,
}

/// <summary>
/// Defines the precision options for rating display.
/// </summary>
public enum RatingPrecision
{
    /// <summary>
    /// Only full stars are displayed.
    /// </summary>
    Full,

    /// <summary>
    /// Stars can be filled in half increments.
    /// </summary>
    Half,

    /// <summary>
    /// Stars can be filled in quarter increments.
    /// </summary>
    Quarter,
}

/// <summary>
/// Defines the appearance styles for rating display.
/// </summary>
public enum RatingAppearance
{
    /// <summary>
    /// Stars are filled based on rating.
    /// </summary>
    Filled,

    /// <summary>
    /// Stars are outlined based on rating.
    /// </summary>
    Regular,
}
