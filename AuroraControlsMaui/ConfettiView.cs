namespace AuroraControls;

public enum ConfettiShape
{
    Rectangular,
    Circular,
}

/// <summary>
/// Confetti view.
/// </summary>
#pragma warning disable CA1001
public class ConfettiView : SceneViewBase
#pragma warning restore CA1001
{
    private readonly List<ConfettiParticle> _particles = new List<ConfettiParticle>();
    private readonly object _listLock = new object();
    private readonly Random _rng;

    private readonly SKPaint _paint = new SKPaint();
    private readonly SKPath _path = new SKPath();

    private double _angle, _tiltAngle;
    private bool _confettiActive = true;

    /// <summary>
    /// The max particles property defines the maximum number of particles desired. Default is 250 particles.
    /// </summary>
    public static BindableProperty MaxParticlesProperty =
        BindableProperty.Create(nameof(MaxParticles), typeof(int), typeof(ConfettiView), 250,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the max particles.
    /// </summary>
    /// <value>Maximum amount of particles as an int. Default is 250.</value>
    public int MaxParticles
    {
        get { return (int)this.GetValue(MaxParticlesProperty); }
        set { this.SetValue(MaxParticlesProperty, value); }
    }

    public static BindableProperty ConfettiShapeProperty =
        BindableProperty.Create(nameof(ConfettiShape), typeof(ConfettiShape), typeof(ConfettiView), default(ConfettiShape),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public ConfettiShape ConfettiShape
    {
        get => (ConfettiShape)this.GetValue(ConfettiShapeProperty);
        set => this.SetValue(ConfettiShapeProperty, value);
    }

    /// <summary>
    /// The continuous property.
    /// </summary>
    public static BindableProperty ContinuousProperty =
        BindableProperty.Create(nameof(Continuous), typeof(bool), typeof(ConfettiView), default(bool),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:AuroraControls.ConfettiView"/> is continuous.
    /// </summary>
    /// <value><c>true</c> if continuous; otherwise, <c>false</c>.</value>
    public bool Continuous
    {
        get { return (bool)this.GetValue(ContinuousProperty); }
        set { this.SetValue(ContinuousProperty, value); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfettiView"/> class.
    /// </summary>
    public ConfettiView()
    {
        this._rng = new Random(Guid.NewGuid().GetHashCode());

        this._paint.Style = SKPaintStyle.StrokeAndFill;
        this._paint.IsDither = true;
        this._paint.IsAntialias = false;
        this._paint.HintingLevel = SKPaintHinting.NoHinting;
    }

    /// <summary>
    /// Method that is called when the property that is specified by propertyName is changed.
    /// The surface is automatically invalidated/redrawn whenever <c>HeightProperty</c>, <c>WidthProperty</c> or <c>MarginProperty</c> gets updated.
    /// </summary>
    /// <param name="propertyName">The name of the bound property that changed.</param>
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

    public override void Start()
    {
        lock (this._listLock)
        {
            this._particles.Clear();
        }

        base.Start();
    }

    /// <summary>
    /// Update the specified particle, particleIndex, width, height and remainingFlakes.
    /// </summary>
    /// <param name="particle">Particle.</param>
    /// <param name="particleIndex">Particle index.</param>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    /// <param name="remainingFlakes">Remaining flakes.</param>
    private void Update(ref ConfettiParticle particle, int particleIndex, int width, int height, bool continuous, ref int remainingFlakes)
    {
        this._angle += 0.01;
        this._tiltAngle += 0.1;

        if (!this._confettiActive && particle.YOffset < -15)
        {
            particle.YOffset = height + 100;
            return;
        }

        this.StepParticle(ref particle, particleIndex);

        if (continuous || particle.YOffset <= height)
        {
            ++remainingFlakes;

            this.CheckForReposition(ref particle, particleIndex, width, height);
        }
    }

    /// <summary>
    /// Steps the particle.
    /// </summary>
    /// <param name="particle">ConfettiParticle ref particle.</param>
    /// <param name="particleIndex">Particle index.</param>
    private void StepParticle(ref ConfettiParticle particle, int particleIndex)
    {
        particle.TiltAngle += particle.TiltAngleIncremental;
        particle.YOffset += (Math.Cos(this._angle + particle.Density) + 3d + (particle.Radius / 2d)) / 2d;
        particle.XOffset += Math.Sin(this._angle);
        particle.Tilt = Math.Sin(particle.TiltAngle - (particleIndex / 3d)) * 15d;
    }

    /// <summary>
    /// Checks for reposition.
    /// </summary>
    /// <param name="particle">Particle.</param>
    /// <param name="particleIndex">Particle index.</param>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    private void CheckForReposition(ref ConfettiParticle particle, int particleIndex, int width, int height)
    {
        if ((particle.XOffset > width + 20 || particle.XOffset < -20 || particle.YOffset > height) && this._confettiActive)
        {
            // 66.67% of the flakes
            if (particleIndex % 5 > 0 || particleIndex % 2 == 0)
            {
                this.RepositionParticle(particle, this._rng.NextDouble() * width, -10, Math.Floor(this._rng.NextDouble() * 10d) - 20);
            }
            else
            {
                if (Math.Sin(this._angle) > 0)
                {
                    // Enter from the left
                    this.RepositionParticle(particle, -20, this._rng.NextDouble() * height, Math.Floor(this._rng.NextDouble() * 10d) - 20);
                }
                else
                {
                    // Enter from the right
                    this.RepositionParticle(particle, width + 20, this._rng.NextDouble() * height, Math.Floor(this._rng.NextDouble() * 10d) - 20);
                }
            }
        }
    }

    /// <summary>
    /// Repositions the particle.
    /// </summary>
    /// <param name="particle">Particle.</param>
    /// <param name="xCoordinate">X coordinate.</param>
    /// <param name="yCoordinate">Y coordinate.</param>
    /// <param name="tilt">Tilt.</param>
    private void RepositionParticle(ConfettiParticle particle, double xCoordinate, double yCoordinate, double tilt)
    {
        particle.XOffset = xCoordinate;
        particle.YOffset = yCoordinate;
        particle.Tilt = tilt;
    }

    protected override SKImage PaintScene(SKSurface surface, SKImageInfo info, double percentage)
    {
        var canvas = surface.Canvas;

        if ((!this._particles?.Any() ?? false) || this._particles.Count != this.MaxParticles)
        {
            lock (this._listLock)
            {
                var particleCount = this._particles.Count;
                var maxParticles = this.MaxParticles;

                if (particleCount == 0)
                {
                    for (int i = 0; i < maxParticles; i++)
                    {
                        this._particles.Add(new ConfettiParticle(info.Width, info.Height, maxParticles, this._rng));
                    }
                }
                else if (particleCount < maxParticles)
                {
                    var particlesToAdd = maxParticles - particleCount;

                    for (int i = 0; i < particlesToAdd; i++)
                    {
                        this._particles.Add(new ConfettiParticle(info.Width, info.Height, maxParticles, this._rng));
                    }
                }
                else
                {
                    var particlesToRemove = particleCount - maxParticles;

                    for (int i = particleCount - 1; i >= maxParticles; i--)
                    {
                        this._particles.RemoveAt(i);
                    }
                }
            }
        }

        canvas.Clear();

        List<ConfettiParticle> tempParticles;

        lock (this._listLock)
        {
            tempParticles = this._particles.ToList();
        }

        var remainingFlakes = 0;
        var particleIndex = 0;

        var continuous = this.Continuous;
        var shape = this.ConfettiShape;
        for (int i = 0; i < tempParticles.Count; i++)
        {
            var particle = tempParticles[i];
            particle.Draw(canvas, this._paint, this._path, shape);
            this.Update(ref particle, particleIndex, info.Width, info.Height, continuous, ref remainingFlakes);
        }

        if (remainingFlakes <= 0)
        {
            canvas.Clear();
        }

        tempParticles = null;

        return surface.Snapshot();
    }

    /// <summary>
    /// Confetti particle class, represents a single confetti particle.
    /// </summary>
    private class ConfettiParticle
    {
        /// <summary>
        /// Gets or sets the XO ffset.
        /// </summary>
        /// <value>The XO ffset.</value>
        public double XOffset { get; set; }

        /// <summary>
        /// Gets or sets the YO ffset.
        /// </summary>
        /// <value>The YO ffset.</value>
        public double YOffset { get; set; }

        /// <summary>
        /// Gets the radius.
        /// </summary>
        /// <value>The radius.</value>
        public int Radius { get; private set; }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <value>The color.</value>
        public SKColor Color { get; private set; }

        /// <summary>
        /// Gets or sets the tilt.
        /// </summary>
        /// <value>The tilt.</value>
        public double Tilt { get; set; }

        /// <summary>
        /// Gets the tilt angle incremental.
        /// </summary>
        /// <value>The tilt angle incremental.</value>
        public double TiltAngleIncremental { get; private set; }

        /// <summary>
        /// Gets or sets the tilt angle.
        /// </summary>
        /// <value>The tilt angle.</value>
        public double TiltAngle { get; set; }

        /// <summary>
        /// Gets or sets the density.
        /// </summary>
        /// <value>The density.</value>
        public double Density { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfettiParticle"/> class.
        /// </summary>
        /// <param name="width">Width of the confetti particle.</param>
        /// <param name="height">Height of the confetti particle.</param>
        /// <param name="maxParticles">Max particles.</param>
        /// <param name="rng">random number gen.</param>
        public ConfettiParticle(int width, int height, int maxParticles, Random rng)
        {
            this.XOffset = rng.NextDouble() * width;
            this.YOffset = (rng.NextDouble() * height) - height;
            this.Radius = rng.Next(10, 30);
            this.Color = new SKColor((byte)rng.Next(0, 255), (byte)rng.Next(0, 255), (byte)rng.Next(0, 255), (byte)255);
            this.Tilt = Math.Floor(rng.NextDouble() * 10) - 10;
            this.TiltAngleIncremental = (rng.NextDouble() * 0.07) + 0.05;
            this.TiltAngle = 0d;
            this.Density = (rng.NextDouble() * maxParticles) + 10;
        }

        /// <summary>
        /// Draw the specified canvas, paint and path.
        /// </summary>
        /// <param name="canvas">Canvas to draw upon.</param>
        /// <param name="paint">SKPaint.</param>
        /// <param name="path">Particle drawing path.</param>
        public void Draw(SKCanvas canvas, SKPaint paint, SKPath path, ConfettiShape shape)
        {
            paint.Color = this.Color;
            paint.StrokeWidth = this.Radius / 2f;

            switch (shape)
            {
                case ConfettiShape.Circular:
                    paint.StrokeCap = SKStrokeCap.Round;
                    break;
                default:
                    paint.StrokeCap = SKStrokeCap.Square;
                    break;
            }

            path.Reset();
            path.MoveTo((float)(this.XOffset + this.Tilt + (this.Radius / 4f)), (float)this.YOffset);
            path.LineTo((float)(this.XOffset + this.Tilt), (float)(this.YOffset + this.Tilt + (this.Radius / 4f)));

            canvas.DrawPath(path, paint);
        }
    }
}
