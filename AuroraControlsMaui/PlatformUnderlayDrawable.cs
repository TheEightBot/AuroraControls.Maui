using Microsoft.Maui.Animations;
using Microsoft.Maui.Platform;

#if IOS || MACCATALYST
using CoreGraphics;
using UIKit;
using PlatformView = UIKit.UIView;
using SKCanvasView = SkiaSharp.Views.iOS.SKCanvasView;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.iOS.SKPaintSurfaceEventArgs;
#elif ANDROID
using Android.Graphics.Drawables;
using Android.Widget;
using PlatformView = Android.Views.ViewGroup;
using SKCanvasView = SkiaSharp.Views.Android.SKCanvasView;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.Android.SKPaintSurfaceEventArgs;
#endif

namespace AuroraControls;

public class PlatformUnderlayDrawable : IDisposable
{
#if IOS || MACCATALYST || ANDROID
    private IView _virtualView;

    private PlatformView _platformView;

    private View _content;

    private SKPaint _borderPaint, _backgroundPaint, _placeholderPaint;
#endif
    private bool _isDrawing;

    private bool _needsDraw;

    private bool _hadValue;

    private StyledContentTypeRegistration? _typeRegistration;

    private bool _hadFocus;

    private bool _disposedValue;

#if IOS || MACCATALYST
    private SKCanvasView _canvas;

    private UITapGestureRecognizer _canvasTapped;

    private UIView _commandView;

    private UITapGestureRecognizer _commandViewTapped;

    public void ConnectHandler(PlatformView platformView, IView virtualView)
    {
        _platformView = platformView;
        _virtualView = virtualView;

        if (_canvas is null)
        {
            _canvas =
                new SKCanvasView
                {
                    Opaque = false,
                };

            _canvas.PaintSurface += OnPaintSurface;
            _canvas.AddGestureRecognizer(
                _canvasTapped = new UITapGestureRecognizer(() =>
                {
                    if (_virtualView is Microsoft.Maui.Controls.ContentView cv && cv.Content is IView view)
                    {
                        view.Focus();
                    }
                }));
        }
    }

    public void DisconnectHandler()
    {
        _borderPaint?.Dispose();
        _backgroundPaint?.Dispose();
        _placeholderPaint?.Dispose();

        if (_virtualView is Microsoft.Maui.Controls.ContentView cv)
        {
            var content = cv.Content;

            if (content is not null)
            {
                content.PropertyChanged -= Content_PropertyChanged;
            }
        }

        if (_canvas is not null)
        {
            _canvas.RemoveGestureRecognizer(_canvasTapped);
            _canvas.PaintSurface -= OnPaintSurface;
            _canvas.RemoveFromSuperview();
            _canvas?.Dispose();

            _canvasTapped?.Dispose();
            _canvasTapped = null;
        }

        if (_commandView is not null)
        {
            _commandView.RemoveGestureRecognizer(_commandViewTapped);
            _commandView.RemoveFromSuperview();
            _commandView?.Dispose();
            _commandView = null;

            _commandView?.Dispose();
            _commandView = null;
        }
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        if (_virtualView is IUnderlayDrawable ud && _virtualView is Microsoft.Maui.Controls.ContentView cv && cv.Content is not null)
        {
            DrawUnderlay(ud, cv, cv.Content.Frame, e.Surface, e.Info);
        }
    }

    private void OnCommandSet()
    {
        if (_virtualView is IUnderlayDrawable ud)
        {
            if (ud.Command is not null)
            {
                if (_commandView is null)
                {
                    _commandView =
                        new UIView
                        {
                            BackgroundColor = UIColor.Clear,
                            UserInteractionEnabled = true,
                            Frame = new CGRect(0f, 0f, (float)_virtualView.Width, (float)_virtualView.Height),
                        };
                    _commandView.AddGestureRecognizer(
                        _commandViewTapped = new UITapGestureRecognizer(() =>
                        {
                            if (_virtualView is IUnderlayDrawable ude && (ude.Command?.CanExecute(ude.CommandParameter) ?? false))
                            {
                                ude.Command.Execute(ude.CommandParameter);
                            }
                        }));
                }

                _platformView.AddSubview(_commandView);
                _platformView.BringSubviewToFront(_commandView);
            }
            else if (ud.Command is null && _commandView is not null)
            {
                _commandView?.RemoveFromSuperview();
            }
        }
    }

#elif ANDROID
    private SKCanvasView _canvas;

    private Android.Widget.Button _commandButton;

    public void ConnectHandler(PlatformView platformView, IView virtualView)
    {
        _platformView = platformView;
        _virtualView = virtualView;

        if (_canvas is null)
        {
            _canvas = new SKCanvasView(platformView.Context);

            _canvas.PaintSurface += OnPaintSurface;
            _canvas.FocusChange += Canvas_FocusChange;
            _canvas.Layout(0, 0, platformView.Width, platformView.Height);
            _canvas.Invalidate();

            Invalidate();
        }
    }

    public void DisconnectHandler()
    {
        _borderPaint?.Dispose();
        _backgroundPaint?.Dispose();
        _placeholderPaint?.Dispose();

        if (_canvas is not null)
        {
            _canvas.PaintSurface -= OnPaintSurface;
            _canvas.FocusChange -= Canvas_FocusChange;
            _canvas.RemoveFromParent();
            _canvas?.Dispose();
            _canvas = null;
        }

        if (_commandButton is not null)
        {
            _commandButton.Click -= CommandClicked;
            _commandButton.RemoveFromParent();
            _commandButton?.Dispose();
            _commandButton = null;
        }
    }

    private void Canvas_FocusChange(object? sender, Android.Views.View.FocusChangeEventArgs e)
    {
        if (e.HasFocus && _virtualView is Microsoft.Maui.Controls.ContentView cv && cv.Content is IView view)
        {
            view.Focus();
        }
    }

    private void CommandClicked(object? sender, EventArgs e)
    {
        if (_virtualView is IUnderlayDrawable ude && (ude.Command?.CanExecute(ude.CommandParameter) ?? false))
        {
            ude.Command.Execute(ude.CommandParameter);
        }
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        if (_virtualView is IUnderlayDrawable ud && _virtualView is Microsoft.Maui.Controls.ContentView cv && cv.Content is not null)
        {
            DrawUnderlay(ud, cv, cv.Content.Frame, e.Surface, e.Info);
        }
    }

    private void OnCommandSet()
    {
        if (_virtualView is IUnderlayDrawable ud)
        {
            if (ud.Command is not null)
            {
                if (_commandButton is null)
                {
                    _commandButton =
                        new Android.Widget.Button(_platformView.Context)
                        {
                            Background = new ColorDrawable(Android.Graphics.Color.Transparent),
                        };

                    _commandButton.Click += CommandClicked;
                }

                _platformView.AddView(_commandButton);
                _commandButton.Layout(0, 0, _platformView.Width, _platformView.Height);
            }
            else if (ud.Command is null && _commandButton is not null)
            {
                _commandButton?.RemoveFromParent();
            }
        }
    }
#endif

    public void ClearSubviews()
    {
#if IOS || MACCATALYST || ANDROID
        if (_platformView is not null)
        {
#if IOS || MACCATALYST
            _platformView.ClearSubviews();
#elif ANDROID
            _platformView.RemoveAllViews();
#endif
            Invalidate();
        }
#endif
    }

    public void PrepareForDisplay()
    {
#if IOS || MACCATALYST || ANDROID
        if (_platformView is not null && _canvas is not null)
        {
#if IOS || MACCATALYST
            var addedView = _platformView.Subviews.ElementAtOrDefault(0);

            if (addedView is not null)
            {
                addedView.BackgroundColor = UIColor.Clear;

                if (addedView is UITextField tf)
                {
                    tf.BorderStyle = UITextBorderStyle.None;
                }
                else if (_virtualView is IUnderlayDrawable ud && addedView is UITextView tv)
                {
                    var edgeInsets = tv.TextContainerInset;
                    var lineFragmentPadding = tv.TextContainer.LineFragmentPadding;
                    ud.PlaceholderOffset = new Point(lineFragmentPadding, edgeInsets.Top);
                }
            }

            _platformView.InsertSubview(_canvas, 0);
#elif ANDROID
            var addedView = _platformView.GetChildAt(0);

            if (addedView is not null)
            {
                if (addedView is EditText et)
                {
                    et.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    et.SetIncludeFontPadding(false);
                    et.SetPadding(0, 0, 0, 0);
                }
            }

            _platformView.AddView(_canvas, 0);
#endif

            if (_virtualView is Microsoft.Maui.Controls.ContentView cv)
            {
                _content = cv.Content;

                if (_content is not null)
                {
                    _content.PropertyChanged -= Content_PropertyChanged;
                    _content.PropertyChanged += Content_PropertyChanged;

                    if (_virtualView is IUnderlayDrawable ud)
                    {
                        if (_content is InputView iv)
                        {
                            ud.Placeholder = iv.Placeholder;
                            ud.PlaceholderColor = iv.PlaceholderColor;
                            iv.PlaceholderColor = Colors.Transparent;
                        }

                        if (_content is Microsoft.Maui.Controls.Internals.IFontElement fe)
                        {
                            ud.FontSize = fe.FontSize;
                        }

                        _typeRegistration = GetRegistration(_content.GetType());
                    }
                }
            }

            AnimateHasValue();
            AnimateHasFocus();
            Invalidate();
        }
#endif
    }

    private static StyledContentTypeRegistration GetRegistration(Type type)
    {
        if (StyledInputLayout.StyledInputLayoutContentRegistrations.TryGetValue(type, out var match))
        {
            return match;
        }

        for (int i = StyledInputLayout.StyledInputLayoutContentRegistrations.Count - 1; i >= 0; i--)
        {
            var registration = StyledInputLayout.StyledInputLayoutContentRegistrations.ElementAt(i);
            if (type.IsAssignableTo(registration.Key))
            {
                return registration.Value;
            }
        }

        return StyledContentTypeRegistration.Default;
    }

    private void Content_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_typeRegistration.HasValue && !string.IsNullOrEmpty(_typeRegistration.Value.ValueChangeProperty) && e.PropertyName == _typeRegistration.Value.ValueChangeProperty)
        {
            AnimateHasValue();
        }

        if (e.PropertyName == nameof(Microsoft.Maui.Controls.VisualElement.IsFocused))
        {
            AnimateHasFocus();
        }
    }

    private void AnimateHasFocus()
    {
#if IOS || MACCATALYST || ANDROID
        if (_virtualView is IUnderlayDrawable ud && _virtualView is Microsoft.Maui.Controls.VisualElement animatable)
        {
            var hasFocus = _content?.IsFocused ?? false;

            if (hasFocus == _hadFocus)
            {
                return;
            }

            var endFocused = hasFocus ? 1d : 0d;

            _hadFocus = hasFocus;

            animatable
                .TransitionTo(
                    nameof(UnderlayDrawableElement.FocusAnimationPercentageProperty),
                    x =>
                    {
                        ud.FocusAnimationPercentage = x;
                    },
                    ud.FocusAnimationPercentage,
                    endFocused,
                    easing: Easing.CubicInOut,
                    length: ud.FocusAnimationDuration);
        }
#endif
    }

    private void AnimateHasValue()
    {
#if IOS || MACCATALYST || ANDROID
        var hasValue = _typeRegistration?.HasValue?.Invoke(_content) ?? false;

        if (hasValue == _hadValue)
        {
            return;
        }

        if (_virtualView is IUnderlayDrawable ud && _virtualView is Microsoft.Maui.Controls.IAnimatable animatable)
        {
            var endHasValue = hasValue ? 1d : 0d;

            _hadValue = hasValue;

            animatable
                .TransitionTo(
                    nameof(UnderlayDrawableElement.HasValueAnimationPercentageProperty),
                    x =>
                    {
                        ud.HasValueAnimationPercentage = x;
                    },
                    ud.HasValueAnimationPercentage,
                    endHasValue,
                    easing: Easing.CubicInOut,
                    length: ud.FocusAnimationDuration);
        }
#endif
    }

    public void PlatformArrange(Rect rect)
    {
#if IOS || MACCATALYST
        if (_canvas is not null)
        {
            _canvas.Frame = new CGRect(0, 0, _platformView?.Bounds.Width ?? 0, _platformView?.Bounds.Height ?? 0);
        }

        if (_commandView is not null)
        {
            _commandView.Frame = new CGRect(0, 0, _platformView?.Bounds.Width ?? 0, _platformView?.Bounds.Height ?? 0);
        }
#elif ANDROID
        if (_canvas is not null)
        {
            _canvas?.Layout(0, 0, _platformView?.Width ?? 0, _platformView?.Height ?? 0);
        }

        if (_commandButton is not null)
        {
            _commandButton?.Layout(0, 0, _platformView?.Width ?? 0, _platformView?.Height ?? 0);
        }
#endif
        Invalidate();
    }

    public void Invalidate()
    {
#if IOS || MACCATALYST || ANDROID
        if (_canvas.CanvasSize == SKSize.Empty)
        {
            return;
        }

#if IOS || MACCATALYST
        if (_canvas.Frame == CGRect.Empty)
        {
            return;
        }

        _canvas?.SetNeedsDisplay();
#elif ANDROID

        _canvas.Invalidate();
#endif
#endif
    }

    public void UpdateLayoutInsets(InsetsF inset)
    {
#if IOS || MACCATALYST || ANDROID
        var scale = (float)DeviceDisplay.Current.MainDisplayInfo.Density;

        (_virtualView as Microsoft.Maui.Controls.ContentView).Padding = new Thickness(inset.Left, inset.Top, inset.Right, inset.Bottom);
#endif
    }

    public void UpdateOpacity()
    {
#if IOS || MACCATALYST || ANDROID
        _content.Opacity = _virtualView.Opacity;
#if IOS || MACCATALYST
        _canvas.Alpha = (nfloat)_virtualView.Opacity;
#elif ANDROID
        _canvas.Alpha = (float)_virtualView.Opacity;
#endif
#endif
    }

    private void DrawUnderlay(IUnderlayDrawable underlayDrawable, View element, Rect controlFrame, SKSurface surface, SKImageInfo imageInfo)
    {
        if (_isDrawing)
        {
            _needsDraw = true;
            return;
        }

#if IOS || MACCATALYST || ANDROID

        try
        {
            _isDrawing = true;

            var scale = (float)DeviceDisplay.Current.MainDisplayInfo.Density;

            var canvas = surface.Canvas;
            var size = imageInfo.Size;

            var hasValue = _typeRegistration?.HasValue?.Invoke(_content) ?? false;
            var isFocused = _content?.IsFocused ?? false;
            var isDisabled = !(_content?.IsEnabled ?? false);
            var isError = underlayDrawable.IsError;
            var borderSize = (float)underlayDrawable.BorderSize * scale;
            var halfBorder = borderSize / 2f;
            var cornerRadius = underlayDrawable.CornerRadius * scale;
            var cornerRadiusSize = new SKSize(cornerRadius, cornerRadius);
            var internalMargin = underlayDrawable.InternalMargin;

            var placeholderFontSize = underlayDrawable.ActivePlaceholderFontSize * scale;
            var placeholderColor =
                underlayDrawable.PlaceholderColor != default(Color)
                    ? underlayDrawable.PlaceholderColor
                    : HavePlaceholderElement.DefaultPlaceholderColor;

            var fontSize = (float)underlayDrawable.FontSize * scale;

            var controlYCenter =
                _typeRegistration?.AlignPlaceholderToTop ?? false
                    ? (controlFrame.Top * scale) + (fontSize * .5f)
                    : controlFrame.Center.Y * scale;

            var focusedPlaceholderCenterY = borderSize + ((float)internalMargin.Top * scale) + (placeholderFontSize * .5f);
            var controlXLeft = controlFrame.Left * scale;

            var placeholderOffset = underlayDrawable.PlaceholderOffset;
            if (placeholderOffset != default(Point))
            {
                controlYCenter += placeholderOffset.Y * scale;
                controlXLeft += placeholderOffset.X * scale;
            }

            var hasValueAnimationPercentage =
                underlayDrawable.AlwaysShowPlaceholder
                    ? 1d
                    : underlayDrawable.HasValueAnimationPercentage;

            var focusAnimationPercentage = underlayDrawable.FocusAnimationPercentage;

            _borderPaint =
                new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                };

            _borderPaint.Color =
                isError
                    ? underlayDrawable.ErrorColor.ToSKColor()
                    : underlayDrawable.InactiveColor.Lerp(underlayDrawable.ActiveColor, focusAnimationPercentage).ToSKColor();
            _borderPaint.StrokeWidth = borderSize;

            _backgroundPaint =
                new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                };

            _placeholderPaint =
                new SKPaint
                {
                    TextAlign = SKTextAlign.Left,
                    IsAntialias = true,
                };
            _placeholderPaint.TextSize = placeholderFontSize;
            _placeholderPaint.Typeface = PlatformInfo.DefaultTypeface;

            var placeholder = underlayDrawable.Placeholder;

            canvas.Clear(SKColors.Transparent);

            switch (underlayDrawable.BorderStyle)
            {
                case ContainerBorderStyle.Underline:
                    _borderPaint.StrokeCap = SKStrokeCap.Square;
                    canvas.DrawLine(new SKPoint(0, size.Height - halfBorder), new SKPoint(size.Width, size.Height - halfBorder), _borderPaint);
                    break;
                case ContainerBorderStyle.RoundedUnderline:
                    _borderPaint.StrokeCap = SKStrokeCap.Round;
                    canvas.DrawLine(new SKPoint(halfBorder, size.Height - halfBorder), new SKPoint(size.Width - borderSize, size.Height - halfBorder), _borderPaint);
                    break;
                case ContainerBorderStyle.Rectangle:
                    var rectBackground = SKRect.Create(halfBorder, halfBorder, size.Width - borderSize, size.Height - borderSize);

                    _backgroundPaint.Color =
                        isDisabled
                            ? underlayDrawable.DisabledColor.Lerp(Colors.White, .2d).ToSKColor()
                            : element.BackgroundColor.ToSKColor();

                    canvas.DrawRect(rectBackground, _backgroundPaint);

                    if (underlayDrawable.BorderSize > 0d)
                    {
                        canvas.DrawRect(rectBackground, _borderPaint);
                    }

                    break;
                case ContainerBorderStyle.RoundedRectangle:
                    var roundedRectBackground = SKRect.Create(halfBorder, halfBorder, size.Width - borderSize, size.Height - borderSize);

                    _backgroundPaint.Color =
                        isDisabled
                            ? underlayDrawable.DisabledColor.Lerp(Colors.White, .2d).ToSKColor()
                            : element.BackgroundColor.ToSKColor();

                    canvas.DrawRoundRect(roundedRectBackground, cornerRadiusSize, _backgroundPaint);

                    if (borderSize > 0d)
                    {
                        canvas.DrawRoundRect(roundedRectBackground, cornerRadiusSize, _borderPaint);
                    }

                    break;

                case ContainerBorderStyle.RoundedRectanglePlaceholderThrough:
                    var roundedRectBackgroundPlaceholderThrough = SKRect.Create(halfBorder, halfBorder + focusedPlaceholderCenterY, size.Width - borderSize, size.Height - focusedPlaceholderCenterY - borderSize);

                    _backgroundPaint.Color =
                        isDisabled
                            ? underlayDrawable.DisabledColor.Lerp(Colors.White, .2d).ToSKColor()
                            : element.BackgroundColor.ToSKColor();

                    canvas.DrawRoundRect(roundedRectBackgroundPlaceholderThrough, cornerRadiusSize, _backgroundPaint);

                    var restoreTo = canvas.SaveLayer();

                    using (new SKAutoCanvasRestore(canvas))
                    {
                        canvas.Clear();
                        if (borderSize > 0d)
                        {
                            _placeholderPaint.TextSize = placeholderFontSize;
                            var placeholderRectSize = canvas.GetTextContainerRectAt(isError ? underlayDrawable.ErrorText : placeholder, new SKPoint((float)controlXLeft, 0.0f), _placeholderPaint);

                            canvas.DrawRoundRect(roundedRectBackgroundPlaceholderThrough, cornerRadiusSize, _borderPaint);

                            if (hasValueAnimationPercentage > 0.0d)
                            {
                                var bufferSize = 2f;
                                var top = roundedRectBackgroundPlaceholderThrough.Top - (_borderPaint.StrokeWidth * .5f);

                                var startingBlendMode = _backgroundPaint.BlendMode;

                                _backgroundPaint.BlendMode = SKBlendMode.SrcIn;

                                _backgroundPaint.Color =
                                        _borderPaint.Color.Lerp(
                                            element.BackgroundColor.ToSKColor(),
                                            hasValueAnimationPercentage);

                                canvas.DrawRect(new SKRect(placeholderRectSize.Left - bufferSize, top, placeholderRectSize.Right + (bufferSize * 2f), top + placeholderRectSize.Height), _backgroundPaint);

                                _backgroundPaint.BlendMode = startingBlendMode;
                            }
                        }
                    }

                    canvas.RestoreToCount(restoreTo);

                    break;
            }

            if (isError)
            {
                _placeholderPaint.TextSize = placeholderFontSize;
                _placeholderPaint.Color = underlayDrawable.ErrorColor.ToSKColor();
                _placeholderPaint.EnsureHasValidFont(underlayDrawable.ErrorText ?? placeholder);

                canvas.DrawTextCenteredVertically(underlayDrawable.ErrorText ?? placeholder, new SKPoint((float)controlXLeft, focusedPlaceholderCenterY), _placeholderPaint);
            }
            else if (!string.IsNullOrEmpty(placeholder))
            {
                _placeholderPaint.Color =
                    placeholderColor
                        .Lerp(
                            underlayDrawable.ActiveColor,
                            focusAnimationPercentage > 0d ? focusAnimationPercentage : hasValueAnimationPercentage)
                        .ToSKColor();

                if (placeholderFontSize > 0d)
                {
                    var placeholderY = controlYCenter.Lerp(focusedPlaceholderCenterY, hasValueAnimationPercentage);

                    _placeholderPaint.TextSize = fontSize.Lerp(placeholderFontSize, (float)hasValueAnimationPercentage);
                    _placeholderPaint.Color = _placeholderPaint.Color.WithAlpha((float)hasValueAnimationPercentage);

                    _placeholderPaint.EnsureHasValidFont(placeholder);

                    canvas.DrawTextCenteredVertically(placeholder, new SKPoint((float)controlXLeft, (float)placeholderY), _placeholderPaint);
                }

                _placeholderPaint.Color = placeholderColor.ToSKColor().WithAlpha(1f - (float)hasValueAnimationPercentage);
                _placeholderPaint.TextSize = fontSize;

                canvas.DrawTextCenteredVertically(placeholder, new SKPoint((float)controlXLeft, (float)controlYCenter), _placeholderPaint);
            }
        }
        finally
        {
            _isDrawing = false;
        }
#endif

        if (_needsDraw)
        {
            _needsDraw = false;
            Invalidate();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
#if IOS || MACCATALYST || ANDROID
                _backgroundPaint?.Dispose();
                _borderPaint?.Dispose();
                _canvas?.Dispose();
                _placeholderPaint?.Dispose();

#if ANDROID
                _commandButton?.Dispose();
#elif IOS || MACCATALYST
                _canvasTapped?.Dispose();
                _commandViewTapped?.Dispose();
                _commandView?.Dispose();
#endif
#endif
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public interface IHavePlatformUnderlayDrawable
{
    PlatformUnderlayDrawable PlatformUnderlayDrawable { get; }
}
