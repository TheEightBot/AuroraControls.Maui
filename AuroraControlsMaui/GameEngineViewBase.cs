using System.Diagnostics;

namespace AuroraControls;

/// <summary>
/// A base class for rendering views like a game engine with FPS display and rendering control.
/// </summary>
public abstract class GameEngineViewBase : AuroraViewBase
{
    private bool _isRendering;
    private Stopwatch _stopwatch;
    private int _frameCount;
    private double _fps;

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether the rendering is enabled.
    /// </summary>
    public bool IsRendering
    {
        get => _isRendering;
        set
        {
            if (_isRendering == value)
            {
                return;
            }

            _isRendering = value;

            if (_isRendering)
            {
                StartRendering();
            }
            else
            {
                StopRendering();
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameEngineViewBase"/> class.
    /// </summary>
    public GameEngineViewBase()
    {
        _stopwatch = new Stopwatch();
        _frameCount = 0;
        _fps = 0;
    }

    /// <summary>
    /// Starts the rendering loop.
    /// </summary>
    private void StartRendering()
    {
        _stopwatch.Start();
        Device.StartTimer(
            TimeSpan.FromMilliseconds(16.7),
            () =>
            {
                if (!_isRendering)
                {
                    return false;
                }

                InvalidateSurface();
                return true;
            });
    }

    /// <summary>
    /// Stops the rendering loop.
    /// </summary>
    private void StopRendering()
    {
        _stopwatch.Stop();
        _stopwatch.Reset();
    }

    /// <summary>
    /// Paints the control, including the FPS display.
    /// </summary>
    /// <param name="surface">The SkiaSharp surface to draw on.</param>
    /// <param name="info">Information about the SkiaSharp image.</param>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Black);

        // Call the abstract method for custom rendering logic
        RenderFrame(canvas, info);

        // Calculate FPS
        _frameCount++;
        if (_stopwatch.ElapsedMilliseconds >= 1000)
        {
            _fps = _frameCount / (_stopwatch.ElapsedMilliseconds / 1000.0);
            _frameCount = 0;
            _stopwatch.Restart();
        }

        // Draw FPS on the screen
        using var fpsPaint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = 24 * _scale,
            IsAntialias = true,
        };

        canvas.DrawText($"FPS: {_fps:F1}", 10, 30, fpsPaint);
    }

    /// <summary>
    /// Abstract method for rendering a single frame. Override this to implement custom rendering logic.
    /// </summary>
    /// <param name="canvas">The SkiaSharp canvas to draw on.</param>
    /// <param name="info">Information about the SkiaSharp image.</param>
    protected abstract void RenderFrame(SKCanvas canvas, SKImageInfo info);
}
