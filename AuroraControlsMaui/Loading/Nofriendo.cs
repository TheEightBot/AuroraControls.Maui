using Microsoft.Maui.Animations;

namespace AuroraControls.Loading;

public class Nofriendo : LoadingViewBase
{
    private int _previousAnimationStep = 0;

    /// <summary>
    /// The step count property.
    /// </summary>
    public static readonly BindableProperty CurrentAnimationStepProperty =
        BindableProperty.Create(nameof(CurrentAnimationStep), typeof(int), typeof(Nofriendo), 0, BindingMode.OneWayToSource,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets the current animation step.
    /// </summary>
    /// <value>Current step as an int. Default is 0.</value>
    public int CurrentAnimationStep
    {
        get => (int)GetValue(CurrentAnimationStepProperty);
        private set => SetValue(CurrentAnimationStepProperty, value);
    }

    /// <summary>
    /// The step count property.
    /// </summary>
    public static readonly BindableProperty StepCountProperty =
        BindableProperty.Create(nameof(StepCount), typeof(int), typeof(Nofriendo), 1,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the step count.
    /// </summary>
    /// <value>An int representing the step count. Default count is 1.</value>
    public int StepCount
    {
        get => (int)GetValue(StepCountProperty);
        set => SetValue(StepCountProperty, value);
    }

    /// <summary>
    /// The maximum animation steps property.
    /// </summary>
    public static readonly BindableProperty MaxAnimationStepsProperty =
        BindableProperty.Create(nameof(MaxAnimationSteps), typeof(int), typeof(Nofriendo), 3);

    /// <summary>
    /// Gets or sets the maximum animation steps.
    /// </summary>
    /// <value>Maximum animation steps as an int. Default is 3.</value>
    public int MaxAnimationSteps
    {
        get => (int)GetValue(MaxAnimationStepsProperty);
        set => SetValue(MaxAnimationStepsProperty, value);
    }

    /// <summary>
    /// The loading start color property.
    /// </summary>
    public static readonly BindableProperty LoadingStartColorProperty =
        BindableProperty.Create(nameof(LoadingStartColor), typeof(Color), typeof(Nofriendo),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the loading start.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default is default(Xamarin.Forms.Color).</value>
    public Color LoadingStartColor
    {
        get => (Color)GetValue(LoadingStartColorProperty);
        set => SetValue(LoadingStartColorProperty, value);
    }

    /// <summary>
    /// The loading end color property.
    /// </summary>
    public static readonly BindableProperty LoadingEndColorProperty =
        BindableProperty.Create(nameof(LoadingEndColor), typeof(Color), typeof(Nofriendo),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the loading end.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default is default(Xamarin.Forms.Color).</value>
    public Color LoadingEndColor
    {
        get => (Color)GetValue(LoadingEndColorProperty);
        set => SetValue(LoadingEndColorProperty, value);
    }

    /// <summary>
    /// This is the method used to draw our control on the SKCanvas. This method is fired every time <c>this.InvalidateSurface();</c> is called, resulting in a "redrawing" of the control.
    /// </summary>
    /// <param name="surface">The skia surface to paint on the controls.</param>
    /// <param name="info">Information about the skia image.</param>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        using var foregroundPaint = new SKPaint();
        using var foregroundPath = new SKPath();
        int animationStep = this.CurrentAnimationStep;
        double previousStepProgress = _previousAnimationStep / (double)this.MaxAnimationSteps;
        double currentStepProcess = animationStep / (double)this.MaxAnimationSteps;

        var previousStepColor = this.LoadingStartColor.Lerp(this.LoadingEndColor, previousStepProgress).ToSKColor();
        var currentStepColor = this.LoadingStartColor.Lerp(this.LoadingEndColor, currentStepProcess).ToSKColor();

        foregroundPaint.Style = SKPaintStyle.Fill;
        foregroundPaint.Color = currentStepColor;

        double animatingPercentage = this.AnimatingPercentage * 100;

        if (!this.Animating)
        {
            canvas.DrawColor(previousStepColor);
            return;
        }

        float stepHeight = (float)(info.Height / (double)this.StepCount);
        double individualStepCompletion = 100 / (double)this.StepCount;
        bool startsAtRight = animationStep % 2 == 0;

        for (int i = 0; i < this.StepCount; i++)
        {
            double stepProgressEnd = individualStepCompletion * (i + 1);

            SKRect drawRect;

            if (this.CurrentAnimationStep == 0)
            {
                canvas.Clear();
            }

            if (animatingPercentage > stepProgressEnd)
            {
                drawRect = SKRect.Create(0, stepHeight * i, info.Width, stepHeight);
            }
            else
            {
                double currentStepProgress = animatingPercentage / stepProgressEnd;

                float currentStepWidth = info.Width * (float)currentStepProgress;

                if (startsAtRight)
                {
                    drawRect = SKRect.Create(info.Width - currentStepWidth, stepHeight * i, currentStepWidth, stepHeight);
                }
                else
                {
                    drawRect = SKRect.Create(0, stepHeight * i, currentStepWidth, stepHeight);
                }
            }

            foregroundPath.AddRect(drawRect);
        }

        canvas.DrawPath(foregroundPath, foregroundPaint);
    }

    protected override void StartAnimationValues() => CurrentAnimationStep = 0;

    protected override void UpdateAnimationValues()
    {
        _previousAnimationStep = CurrentAnimationStep;

        CurrentAnimationStep++;

        if (CurrentAnimationStep > MaxAnimationSteps)
        {
            CurrentAnimationStep = 0;
        }
    }
}
