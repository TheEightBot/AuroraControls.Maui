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
        get => (double)GetValue(AnimatingPercentageProperty);

        set
        {
            double clampedValue = value.Clamp(0d, 1d);
            SetValue(AnimatingPercentageProperty, clampedValue);
            AnimatingPercentageChanged?.Invoke(this, new ValueChangedEventArgs(AnimatingPercentage, clampedValue));
        }
    }

    /// <summary>
    /// The is running property. Similar to ActivityIndicator's IsRunning.
    /// </summary>
    public static readonly BindableProperty IsRunningProperty =
        BindableProperty.Create(
            nameof(IsRunning),
            typeof(bool),
            typeof(LoadingViewBase),
            default(bool),
            BindingMode.TwoWay,
            propertyChanged: OnIsRunningChanged);

    /// <summary>
    /// Gets or sets a value indicating whether the loading indicator is running (animating).
    /// </summary>
    /// <value><c>true</c> if the indicator is running; otherwise, <c>false</c>.</value>
    public bool IsRunning
    {
        get => (bool)GetValue(IsRunningProperty);
        set => SetValue(IsRunningProperty, value);
    }

    /// <summary>
    /// The color property. Similar to ActivityIndicator's Color.
    /// </summary>
    public static readonly BindableProperty ColorProperty =
        BindableProperty.Create(
            nameof(Color),
            typeof(Color),
            typeof(LoadingViewBase),
            Colors.Gray,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the loading indicator.
    /// </summary>
    /// <value>The color of the indicator. Default is Gray.</value>
    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// The animation rate property. Time in milliseconds between frames.
    /// </summary>
    public static readonly BindableProperty AnimationRateProperty =
        BindableProperty.Create(
            nameof(AnimationRate),
            typeof(uint),
            typeof(LoadingViewBase),
            16u);

    /// <summary>
    /// Gets or sets the animation rate (time in milliseconds between frames).
    /// </summary>
    /// <value>The animation rate. Default is 16ms.</value>
    public uint AnimationRate
    {
        get => (uint)GetValue(AnimationRateProperty);
        set => SetValue(AnimationRateProperty, value);
    }

    /// <summary>
    /// The animation length property. Number of milliseconds over which to interpolate the animation.
    /// </summary>
    public static readonly BindableProperty AnimationLengthProperty =
        BindableProperty.Create(
            nameof(AnimationLength),
            typeof(uint),
            typeof(LoadingViewBase),
            1600u);

    /// <summary>
    /// Gets or sets the animation length (number of milliseconds for one animation cycle).
    /// </summary>
    /// <value>The animation length. Default is 1600ms.</value>
    public uint AnimationLength
    {
        get => (uint)GetValue(AnimationLengthProperty);
        set => SetValue(AnimationLengthProperty, value);
    }

    /// <summary>
    /// The animation easing property.
    /// </summary>
    public static readonly BindableProperty AnimationEasingProperty =
        BindableProperty.Create(
            nameof(AnimationEasing),
            typeof(Easing),
            typeof(LoadingViewBase),
            null);

    /// <summary>
    /// Gets or sets the animation easing function.
    /// </summary>
    /// <value>The easing function. Default is null (linear).</value>
    public Easing? AnimationEasing
    {
        get => (Easing?)GetValue(AnimationEasingProperty);
        set => SetValue(AnimationEasingProperty, value);
    }

    /// <summary>
    /// The animating property.
    /// </summary>
    [Obsolete("Use IsRunning instead. This property will be removed in a future version.")]
    public static readonly BindableProperty AnimatingProperty =
        BindableProperty.Create(nameof(Animating), typeof(bool), typeof(LoadingViewBase), default(bool), BindingMode.OneWayToSource);

    /// <summary>
    /// Gets a value indicating whether the loading indicator is currently animating.
    /// </summary>
    /// <value><c>true</c> if animating; otherwise, <c>false</c>.</value>
    [Obsolete("Use IsRunning instead. This property will be removed in a future version.")]
    public bool Animating
    {
        get => (bool)GetValue(AnimatingProperty);
        private set => SetValue(AnimatingProperty, value);
    }

    private static void OnIsRunningChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LoadingViewBase loadingView && newValue is bool isRunning)
        {
            if (isRunning)
            {
                loadingView.StartAnimation();
            }
            else
            {
                loadingView.StopAnimation();
            }
        }
    }

    public LoadingViewBase() => _animationName = $"{this.GetType().Name}_{Guid.NewGuid().ToString()}";

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
    /// Starts the animation. Sets IsRunning to true.
    /// </summary>
    public void Start()
    {
        IsRunning = true;
    }

    /// <summary>
    /// Stops the animation. Sets IsRunning to false.
    /// </summary>
    public void Stop()
    {
        IsRunning = false;
    }

    /// <summary>
    /// Called when IsRunning is set to true. Starts the internal animation.
    /// </summary>
    protected virtual void StartAnimation()
    {
        StopAnimation();

#pragma warning disable CS0618 // Type or member is obsolete
        Animating = true;
#pragma warning restore CS0618

        StartAnimationValues();
        CreateAnimationNextStep();
    }

    /// <summary>
    /// Called when IsRunning is set to false. Stops the internal animation.
    /// </summary>
    protected virtual void StopAnimation()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        Animating = false;
#pragma warning restore CS0618

        this.AbortAnimation(_animationName);
    }

    /// <summary>
    /// Creates the animation for the next step.
    /// </summary>
    private void CreateAnimationNextStep()
    {
        var primaryAnimation = new Animation(x => this.AnimatingPercentage = x);

        primaryAnimation
            .Commit(this, _animationName, AnimationRate, AnimationLength, AnimationEasing,
            repeat: () =>
            {
                if (!IsRunning)
                {
                    return false;
                }

                AnimatingPercentage = 0d;
                UpdateAnimationValues();
                return true;
            });
    }
}
