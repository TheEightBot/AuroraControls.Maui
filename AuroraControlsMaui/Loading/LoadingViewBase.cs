namespace AuroraControls.Loading;

public class LoadingViewBase : AuroraViewBase
{
    private readonly string _animationName;

    public event EventHandler<ValueChangedEventArgs> AnimatingPercentageChanged;

    /// <summary>
    /// The animating percentage property.
    /// </summary>
    public static readonly BindableProperty AnimatingPercentageProperty =
        BindableProperty.Create(nameof(AnimatingPercentage), typeof(double), typeof(LoadingViewBase), 0d, BindingMode.OneWayToSource,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the animating percentage.
    /// </summary>
    /// <value>The animating percentage as a double. Default is 0d.</value>
    public double AnimatingPercentage
    {
        get
        {
            return (double)GetValue(AnimatingPercentageProperty);
        }

        set
        {
            var clampedValue = value.Clamp(0d, 1d);
            SetValue(AnimatingPercentageProperty, clampedValue);
            AnimatingPercentageChanged?.Invoke(this, new ValueChangedEventArgs(AnimatingPercentage, clampedValue));
        }
    }

    /// <summary>
    /// The animating property.
    /// </summary>
    public static readonly BindableProperty AnimatingProperty =
        BindableProperty.Create(nameof(Animating), typeof(bool), typeof(LoadingViewBase), default(bool), BindingMode.OneWayToSource);

    public bool Animating
    {
        get { return (bool)GetValue(AnimatingProperty); }
        private set { SetValue(AnimatingProperty, value); }
    }

    public LoadingViewBase()
    {
        _animationName = $"{this.GetType().Name}_{Guid.NewGuid().ToString()}";
    }

    protected override void Detached()
    {
        base.Detached();

        Stop();
    }

    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
    }

    protected virtual void StartAnimationValues()
    {
    }

    protected virtual void UpdateAnimationValues()
    {
    }

    /// <summary>
    /// Starts the step animation.
    /// </summary>
    /// <param name="rate">The time, in milliseconds, between frames.</param>
    /// <param name="length">The number of milliseconds over which to interpolate the animation.</param>
    /// <param name="easing">The easing function to use to transision in, out, or in and out of the animation.</param>
    public void Start(uint rate = 16, uint length = 250, Easing easing = null)
    {
        Stop();

        Animating = true;
        CreateAnimationNextStep(rate, length, easing);
    }

    /// <summary>
    /// Creates the animation for the next step.
    /// </summary>
    /// <param name="rate">Rate.</param>
    /// <param name="length">Length.</param>
    /// <param name="easing">Easing.</param>
    private void CreateAnimationNextStep(uint rate = 16, uint length = 250, Easing easing = null)
    {
        var primaryAnimation = new Animation(x => this.AnimatingPercentage = x, 0, 1);

        primaryAnimation
            .Commit(this, _animationName, rate, length, easing,
            repeat: () =>
            {
                if (!Animating)
                {
                    return false;
                }

                AnimatingPercentage = 0d;
                UpdateAnimationValues();
                return true;
            });
    }

    /// <summary>
    /// Stops the step animation.
    /// </summary>
    public void Stop()
    {
        Animating = false;
        this.AbortAnimation(_animationName);
    }
}
