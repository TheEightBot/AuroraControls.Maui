using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Animations;
using Microsoft.Maui.Platform;

namespace AuroraControls;

public abstract class SceneViewBase : AuroraViewBase
{
    private readonly object _surfaceLocker = new object();
    private readonly object _imageLocker = new object();

    private ITicker _ticker;

    private SKImage _image;
    private SKImageInfo _imageInfo;
    private SKSurface _surface;

    private Task _processingTask;

    private IntPtr _lastImageId;
    private IntPtr _currentImageId;

    private bool _started;

    private uint _currentPosition;

    private uint _length;

    private uint _rate;

    private Easings.Functions _easingFunction;

    private int _processing;

    public bool Processing
    {
        get
        {
            return Interlocked.CompareExchange(ref _processing, 1, 1) == 1;
        }

        set
        {
            if (value)
            {
                Interlocked.CompareExchange(ref _processing, 1, 0);
            }
            else
            {
                Interlocked.CompareExchange(ref _processing, 0, 1);
            }
        }
    }

    public static BindableProperty LengthProperty =
        BindableProperty
            .Create(
                nameof(Length), typeof(uint), typeof(SceneViewBase), 1600u,
                propertyChanged:
                    static (bindable, _, newValue) =>
                    {
                        if (bindable is SceneViewBase x)
                        {
                            x._length = (uint)newValue;
                        }
                    });

    public uint Length
    {
        get => (uint)GetValue(LengthProperty);
        set => SetValue(LengthProperty, value);
    }

    public static BindableProperty RateProperty =
        BindableProperty
            .Create(
                nameof(Rate), typeof(uint), typeof(SceneViewBase), 16u,
                propertyChanged:
                    static (bindable, _, newValue) =>
                    {
                        if (bindable is SceneViewBase x)
                        {
                            x._rate = (uint)newValue;
                        }
                    });

    public uint Rate
    {
        get => (uint)GetValue(RateProperty);
        set => SetValue(RateProperty, value);
    }

    public bool IsRunning
    {
        get => _ticker?.IsRunning ?? false;
    }

    public static BindableProperty EasingFunctionProperty =
        BindableProperty.Create(nameof(EasingFunction), typeof(Easings.Functions), typeof(SceneViewBase), default(Easings.Functions),
                propertyChanged:
                    static (bindable, _, newValue) =>
                    {
                        if (bindable is SceneViewBase x)
                        {
                            x._easingFunction = (Easings.Functions)newValue;
                        }
                    });

    public Easings.Functions EasingFunction
    {
        get => (Easings.Functions)GetValue(EasingFunctionProperty);
        set => SetValue(EasingFunctionProperty, value);
    }

    public SceneViewBase()
    {
        _rate = this.Rate;
        _length = this.Length;
    }

    protected override void Detached()
    {
        this.Stop();
        base.Detached();
    }

    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        if (_surface == null || _imageInfo.IsEmpty ||
           _imageInfo.Width != info.Width || _imageInfo.Height != info.Height)
        {
            canvas.Clear();

            lock (_surfaceLocker)
            {
                _imageInfo = new SKImageInfo(info.Width, info.Height);
                _surface = SKSurface.Create(_imageInfo);
            }
        }
        else if (_currentImageId == IntPtr.Zero)
        {
            canvas.Clear();
        }

        lock (_imageLocker)
        {
            if (_image == null)
            {
                return;
            }

            canvas.Clear();
            canvas.DrawImage(_image, 0, 0);

            _lastImageId = _image.Handle;
        }
    }

    public virtual void Start()
    {
        if (_started)
        {
            Stop();
        }

        _started = true;
        _lastImageId = IntPtr.Zero;

        if (IsAttached)
        {
            this.InvalidateSurface();
        }

        _ticker =
#if ANDROID
            new PlatformTicker(this.Handler.MauiContext.Services.GetRequiredService<IEnergySaverListenerManager>());
#else
            new PlatformTicker();
#endif

        _ticker.Fire =
            () =>
            {
                if (IsAttached)
                {
                    if (_image != null && _lastImageId != _currentImageId)
                    {
                        this.InvalidateSurface();
                    }

                    if (!Processing)
                    {
                        _processingTask = Task.Run(() => ProcessScene());
                    }
                }
            };

        _ticker.Start();

        OnPropertyChanged(nameof(IsRunning));
    }

    public virtual void Stop()
    {
        _started = false;

        lock (_imageLocker)
        {
            SKImage tmp = _image;
            _image = null;
            _processingTask = null;
            tmp?.Dispose();
        }

        if (_ticker is null)
        {
            return;
        }

        _ticker?.Stop();
        OnPropertyChanged(nameof(IsRunning));
    }

    protected abstract SKImage PaintScene(SKSurface surface, SKImageInfo info, double percentage);

    private void ProcessScene()
    {
        if (!IsAttached || Processing)
        {
            return;
        }

        try
        {
            Processing = true;

            SKImage image = null;

            lock (_surfaceLocker)
            {
                if (_surface != null)
                {
                    _currentPosition += _rate;

                    if (_currentPosition > _length)
                    {
                        _currentPosition -= _length;
                    }

                    var percentage = Easings.Interpolate(_currentPosition / (double)_length, _easingFunction);

                    image = PaintScene(_surface, _imageInfo, percentage);
                }
            }

            SKImage tmpImage;

            lock (_imageLocker)
            {
                tmpImage = _image;
                _image = image;
                _currentImageId = _image?.Handle ?? IntPtr.Zero;
            }

            tmpImage?.Dispose();
            tmpImage = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{ex}");
        }
        finally
        {
            Processing = false;
        }
    }
}
