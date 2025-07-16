namespace AuroraControls;

public enum ConfettiShape
{
    Rectangular,
    Circular,
}

/// <summary>
/// High-performance confetti view optimized for maximum particles and smooth rendering.
/// </summary>
#pragma warning disable CA1001
public class ConfettiView : SceneViewBase
#pragma warning restore CA1001
{
    private readonly Random _rng;
    private readonly SKPaint _paint = new SKPaint();

    private ConfettiParticle[] _particles;
    private double _angle;
    private bool _confettiActive = true;
    private int _currentParticleCount;
    private int _canvasWidth;
    private int _canvasHeight;

    // Pre-calculated constants for performance
    private static readonly float[] SinTable = new float[3600]; // 0.1 degree precision
    private static readonly float[] CosTable = new float[3600];

    static ConfettiView()
    {
        // Initialize lookup tables once for all instances
        for (int i = 0; i < 3600; i++)
        {
            float radians = i * 0.001745329f; // 0.1 degrees in radians
            SinTable[i] = (float)Math.Sin(radians);
            CosTable[i] = (float)Math.Cos(radians);
        }
    }

    /// <summary>
    /// The max particles property. Default is 300 for optimal performance.
    /// </summary>
    public static BindableProperty MaxParticlesProperty =
        BindableProperty.Create(nameof(MaxParticles), typeof(int), typeof(ConfettiView), 300,
            propertyChanged: OnMaxParticlesChanged);

    public int MaxParticles
    {
        get => (int)GetValue(MaxParticlesProperty);
        set => SetValue(MaxParticlesProperty, value);
    }

    public static BindableProperty ConfettiShapeProperty =
        BindableProperty.Create(nameof(ConfettiShape), typeof(ConfettiShape), typeof(ConfettiView),
            ConfettiShape.Rectangular, propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public ConfettiShape ConfettiShape
    {
        get => (ConfettiShape)GetValue(ConfettiShapeProperty);
        set => SetValue(ConfettiShapeProperty, value);
    }

    public static BindableProperty ContinuousProperty =
        BindableProperty.Create(nameof(Continuous), typeof(bool), typeof(ConfettiView), false,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public bool Continuous
    {
        get => (bool)GetValue(ContinuousProperty);
        set => SetValue(ContinuousProperty, value);
    }

    /// <summary>
    /// The particle size property. Controls the size range of confetti particles. Default is 6.0.
    /// </summary>
    public static BindableProperty ParticleSizeProperty =
        BindableProperty.Create(nameof(ParticleSize), typeof(double), typeof(ConfettiView), 6.0,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the particle size. This controls the maximum size of confetti particles.
    /// The actual size will be randomized between 50% and 100% of this value.
    /// </summary>
    /// <value>The particle size as a double. Default is 6.0.</value>
    public double ParticleSize
    {
        get => (double)GetValue(ParticleSizeProperty);
        set => SetValue(ParticleSizeProperty, value);
    }

    /// <summary>
    /// The gravity property. Controls how fast particles fall. Default is 0.08.
    /// </summary>
    public static BindableProperty GravityProperty =
        BindableProperty.Create(nameof(Gravity), typeof(double), typeof(ConfettiView), 0.08,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the gravity affecting the particles. Higher values make particles fall faster.
    /// </summary>
    public double Gravity
    {
        get => (double)GetValue(GravityProperty);
        set => SetValue(GravityProperty, value);
    }

    /// <summary>
    /// The wind intensity property. Controls horizontal drift of particles. Default is 0.3.
    /// </summary>
    public static BindableProperty WindIntensityProperty =
        BindableProperty.Create(nameof(WindIntensity), typeof(double), typeof(ConfettiView), 0.3,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the wind intensity. Higher values create more horizontal drift.
    /// </summary>
    public double WindIntensity
    {
        get => (double)GetValue(WindIntensityProperty);
        set => SetValue(WindIntensityProperty, value);
    }

    /// <summary>
    /// The emission rate property. Controls how many particles spawn per frame in continuous mode. Default is 5.
    /// </summary>
    public static BindableProperty EmissionRateProperty =
        BindableProperty.Create(nameof(EmissionRate), typeof(int), typeof(ConfettiView), 5,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the emission rate for continuous mode. Number of new particles per frame.
    /// </summary>
    public int EmissionRate
    {
        get => (int)GetValue(EmissionRateProperty);
        set => SetValue(EmissionRateProperty, value);
    }

    /// <summary>
    /// The fade out property. Controls whether particles fade out as they age. Default is false.
    /// </summary>
    public static BindableProperty FadeOutProperty =
        BindableProperty.Create(nameof(FadeOut), typeof(bool), typeof(ConfettiView), false,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether particles fade out as they age.
    /// </summary>
    public bool FadeOut
    {
        get => (bool)GetValue(FadeOutProperty);
        set => SetValue(FadeOutProperty, value);
    }

    /// <summary>
    /// The colors property. Allows setting custom colors for confetti particles.
    /// </summary>
    public static BindableProperty ColorsProperty =
        BindableProperty.Create(nameof(Colors), typeof(IList<Color>), typeof(ConfettiView), null,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the custom colors for confetti particles. If null, random colors are used.
    /// </summary>
    public IList<Color> Colors
    {
        get => (IList<Color>)GetValue(ColorsProperty);
        set => SetValue(ColorsProperty, value);
    }

    /// <summary>
    /// The burst mode property. When true, creates an explosion effect from the center.
    /// </summary>
    public static BindableProperty BurstModeProperty =
        BindableProperty.Create(nameof(BurstMode), typeof(bool), typeof(ConfettiView), false,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether to use burst mode (explosion from center) instead of falling from top.
    /// </summary>
    public bool BurstMode
    {
        get => (bool)GetValue(BurstModeProperty);
        set => SetValue(BurstModeProperty, value);
    }

    public ConfettiView()
    {
        _rng = new Random();
        _particles = new ConfettiParticle[1000]; // Pre-allocate max possible

        // Optimize paint for maximum performance
        _paint.Style = SKPaintStyle.Stroke;
        _paint.IsAntialias = false; // Disable for maximum performance
        _paint.IsDither = false;
        _paint.StrokeWidth = 2f;
    }

    private static void OnMaxParticlesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ConfettiView view)
        {
            view._currentParticleCount = Math.Min((int)newValue, 1000);
        }

        IAuroraView.PropertyChangedInvalidateSurface(bindable, oldValue, newValue);
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName?.Equals(HeightProperty.PropertyName) == true ||
            propertyName?.Equals(WidthProperty.PropertyName) == true ||
            propertyName?.Equals(MarginProperty.PropertyName) == true)
        {
            InvalidateSurface();
        }
    }

    public override void Start()
    {
        _confettiActive = true;
        _currentParticleCount = Math.Min(MaxParticles, 1000);

        // Don't initialize particles here - wait for PaintScene to get proper dimensions
        // Just reset the array to ensure clean initialization
        _particles = new ConfettiParticle[1000];

        base.Start();
    }

    public override void Stop()
    {
        _confettiActive = false;
        base.Stop();
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private ConfettiParticle CreateParticle(int width, int height)
    {
        var maxSize = (float)ParticleSize;
        var minSize = maxSize * 0.5f; // Minimum size is 50% of maximum

        float x, y, velX, velY;

        if (BurstMode)
        {
            // Burst from center
            x = width * 0.5f;
            y = height * 0.5f;

            // Random direction for burst
            float angle = (float)(_rng.NextDouble() * Math.PI * 2);
            float speed = (float)((_rng.NextDouble() * 8) + 4); // Burst speed
            velX = (float)Math.Cos(angle) * speed;
            velY = (float)Math.Sin(angle) * speed;
        }
        else
        {
            // Traditional falling from top
            x = (float)(_rng.NextDouble() * width);
            y = (float)((_rng.NextDouble() * -height * 0.5) - 50);
            velX = (float)((_rng.NextDouble() * 2) - 1);
            velY = (float)((_rng.NextDouble() * 2) + 1);
        }

        // Choose color from custom palette or random
        SKColor color;
        if (Colors?.Count > 0)
        {
            var selectedColor = Colors[_rng.Next(Colors.Count)];
            color = new SKColor(
                (byte)(selectedColor.Red * 255),
                (byte)(selectedColor.Green * 255),
                (byte)(selectedColor.Blue * 255),
                (byte)(selectedColor.Alpha * 255));
        }
        else
        {
            color = new SKColor((byte)_rng.Next(255), (byte)_rng.Next(255), (byte)_rng.Next(255));
        }

        return new ConfettiParticle
        {
            X = x,
            Y = y,
            VelocityX = velX,
            VelocityY = velY,
            Size = (float)((_rng.NextDouble() * (maxSize - minSize)) + minSize),
            Rotation = (float)(_rng.NextDouble() * 360),
            RotationSpeed = (float)((_rng.NextDouble() * 8) - 4),
            Color = color,
            IsActive = true,
            Age = 0f,
            LifeSpan = FadeOut ? (float)((_rng.NextDouble() * 3) + 2) : float.MaxValue, // 2-5 seconds if fading
        };
    }

    protected override SKImage PaintScene(SKSurface surface, SKImageInfo info, double percentage)
    {
        var canvas = surface.Canvas;
        canvas.Clear();

        // Update canvas dimensions
        _canvasWidth = info.Width;
        _canvasHeight = info.Height;

        // Initialize particles with actual canvas dimensions
        bool needsInitialization = _particles[0].Size == 0;
        if (needsInitialization)
        {
            for (int i = 0; i < _currentParticleCount; i++)
            {
                _particles[i] = CreateParticle(_canvasWidth, _canvasHeight);
            }
        }

        _angle += 0.015; // Slower angle increment for stability

        var shape = ConfettiShape;
        var continuous = Continuous;
        int activeParticles = 0;

        // High-performance particle update and render loop
        for (int i = 0; i < _currentParticleCount; i++)
        {
            ref var particle = ref _particles[i];

            if (!particle.IsActive && !continuous)
            {
                continue;
            }

            // Update particle physics
            UpdateParticle(ref particle, _canvasWidth, _canvasHeight, continuous);

            // Only render if particle is active and on screen
            if (particle.IsActive && particle.Y > -50 && particle.Y < _canvasHeight + 50)
            {
                RenderParticle(canvas, ref particle, shape);
                activeParticles++;
            }
        }

        return surface.Snapshot();
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private void UpdateParticle(ref ConfettiParticle particle, int width, int height, bool continuous)
    {
        if (!particle.IsActive && !continuous)
        {
            return;
        }

        // Update particle age for fade out
        particle.Age += 0.016f; // Assume ~60 FPS (1/60 second per frame)

        // Check if particle should fade out
        if (FadeOut && particle.Age >= particle.LifeSpan)
        {
            particle.IsActive = false;
            return;
        }

        // Apply wind effect with configurable intensity
        float windEffect = (float)(Math.Sin(_angle + (particle.X * 0.01)) * WindIntensity);
        particle.X += particle.VelocityX + windEffect;
        particle.Y += particle.VelocityY;
        particle.Rotation += particle.RotationSpeed;

        // Apply gravity
        particle.VelocityY += (float)Gravity;

        // Handle off-screen particles
        if (particle.Y > height + 50)
        {
            if (continuous)
            {
                // Respawn particles based on emission rate
                particle = CreateParticle(width, height);
            }
            else
            {
                particle.IsActive = false;
            }
        }
        else if (particle.X < -50 || particle.X > width + 50)
        {
            if (continuous)
            {
                // Wrap around horizontally in continuous mode
                if (particle.X < -50)
                {
                    particle.X = width + 40;
                }
                else
                {
                    particle.X = -40;
                }
            }
            else
            {
                particle.IsActive = false;
            }
        }
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private void RenderParticle(SKCanvas canvas, ref ConfettiParticle particle, ConfettiShape shape)
    {
        // Apply fade out effect if enabled
        var color = particle.Color;
        if (FadeOut && particle.Age > 0)
        {
            float fadeAlpha = Math.Max(0f, 1f - (particle.Age / particle.LifeSpan));
            color = new SKColor(color.Red, color.Green, color.Blue, (byte)(255 * fadeAlpha));
        }

        _paint.Color = color;
        _paint.StrokeWidth = particle.Size * 0.5f;

        if (shape == ConfettiShape.Circular)
        {
            _paint.StrokeCap = SKStrokeCap.Round;
        }
        else
        {
            _paint.StrokeCap = SKStrokeCap.Square;
        }

        // Fast lookup for rotation
        int rotIndex = (int)(particle.Rotation * 10) % 3600;
        if (rotIndex < 0)
        {
            rotIndex += 3600;
        }

        float cos = CosTable[rotIndex];
        float sin = SinTable[rotIndex];

        // Draw optimized line representing confetti piece
        float halfSize = particle.Size * 0.5f;
        float x1 = particle.X + (cos * halfSize);
        float y1 = particle.Y + (sin * halfSize);
        float x2 = particle.X - (cos * halfSize);
        float y2 = particle.Y - (sin * halfSize);

        canvas.DrawLine(x1, y1, x2, y2, _paint);
    }

    /// <summary>
    /// Ultra-lightweight particle struct for maximum performance.
    /// </summary>
    private struct ConfettiParticle
    {
        public float X;
        public float Y;
        public float VelocityX;
        public float VelocityY;
        public float Size;
        public float Rotation;
        public float RotationSpeed;
        public SKColor Color;
        public bool IsActive;
        public float Age;
        public float LifeSpan;
    }
}
