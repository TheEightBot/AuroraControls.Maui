using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace AuroraControls.Platforms.iOS;

/// <summary>
/// Custom keyboard accessory view that replaces MAUI's fixed-height UIToolbar.
/// Supports two button styles:
/// <list type="bullet">
///   <item><see cref="KeyboardToolbarDoneButtonStyle.Text"/> — a <see cref="UIButton"/> with configurable title, color, and font.</item>
///   <item><see cref="KeyboardToolbarDoneButtonStyle.SystemCheckmark"/> — a <see cref="UIToolbar"/> pinned inside an unconstrained outer view; not clipped on iOS 26+.</item>
/// </list>
/// </summary>
internal sealed class AuroraKeyboardAccessoryView : UIView
{
    private UIButton? _textButton;
    private UIToolbar? _systemToolbar;
    private NSObject? _fontSizeObserver;
    private nfloat _fixedHeight;
    private NSLayoutConstraint? _heightConstraint;

    /// <summary>Gets or sets the action invoked when the Done button is tapped.</summary>
    public Action? DoneAction { get; set; }

    public AuroraKeyboardAccessoryView()
        : base(CGRect.Empty)
    {
        TranslatesAutoresizingMaskIntoConstraints = false;
        ClipsToBounds = false;
    }

    /// <summary>
    /// Configures the accessory view. Call once after creation.
    /// </summary>
    public void Configure(
        KeyboardToolbarDoneButtonStyle style,
        string? title,
        Color? titleColor,
        Color? backgroundColor,
        string? fontFamily,
        double fontSize,
        double height)
    {
        foreach (var sub in Subviews)
        {
            sub.RemoveFromSuperview();
        }

        _textButton = null;
        _systemToolbar = null;
        _fontSizeObserver?.Dispose();
        _fontSizeObserver = null;
        _fixedHeight = height > 0 ? (nfloat)height : 0;

        // Deactivate the previous self-height constraint before adding a new one.
        // Without this, every Configure() call stacks constraints and AutoLayout conflicts.
        _heightConstraint?.Active = false;
        _heightConstraint = null;

        // Default to transparent so the button floats above the keyboard's rounded corners
        // without drawing a harsh straight-edged background bar.
        BackgroundColor = backgroundColor != null
            ? backgroundColor.ToPlatform()
            : UIColor.Clear;

        if (style == KeyboardToolbarDoneButtonStyle.SystemCheckmark)
        {
            ConfigureSystemCheckmark();
        }
        else
        {
            ConfigureTextButton(title, titleColor, fontFamily, fontSize);
        }

        InvalidateIntrinsicContentSize();

        _fontSizeObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            UIApplication.ContentSizeCategoryChangedNotification,
            _ =>
            {
                InvalidateIntrinsicContentSize();
                SetNeedsLayout();
            });
    }

    private void ConfigureTextButton(string? title, Color? titleColor, string? fontFamily, double fontSize)
    {
        _textButton = new UIButton(UIButtonType.System);
        _textButton.TranslatesAutoresizingMaskIntoConstraints = false;
        _textButton.SetTitle(title ?? KeyboardToolbarOptions.Default.DefaultTitle, UIControlState.Normal);
        _textButton.AddTarget((_, _) => DoneAction?.Invoke(), UIControlEvent.TouchUpInside);
        _textButton.ContentEdgeInsets = new UIEdgeInsets(4, 16, 4, 16);

        UpdateTextButtonColor(_textButton, titleColor);
        UpdateTextButtonFont(_textButton, fontFamily, fontSize);

        AddSubview(_textButton);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            _textButton.TrailingAnchor.ConstraintEqualTo(TrailingAnchor, -16),
            _textButton.CenterYAnchor.ConstraintEqualTo(CenterYAnchor),
            _textButton.HeightAnchor.ConstraintGreaterThanOrEqualTo(44),
            _textButton.WidthAnchor.ConstraintGreaterThanOrEqualTo(44),
        });

        if (_fixedHeight <= 0)
        {
            _heightConstraint = HeightAnchor.ConstraintGreaterThanOrEqualTo(60);
        }
        else
        {
            _heightConstraint = HeightAnchor.ConstraintEqualTo(_fixedHeight);
        }

        _heightConstraint.Active = true;
    }

    private static void UpdateTextButtonColor(UIButton button, Color? titleColor)
    {
        var uiColor = titleColor?.ToPlatform() ?? UIColor.SystemBlue;
        button.SetTitleColor(uiColor, UIControlState.Normal);
        button.TintColor = uiColor;
    }

    private static void UpdateTextButtonFont(UIButton button, string? fontFamily, double fontSize)
    {
        UIFont font;
        if (!string.IsNullOrEmpty(fontFamily))
        {
            var pointSize = fontSize > 0 ? (nfloat)fontSize : UIFont.ButtonFontSize;
            font = UIFont.FromName(fontFamily, pointSize) ?? UIFont.SystemFontOfSize(pointSize, UIFontWeight.Semibold);
        }
        else if (fontSize > 0)
        {
            font = UIFont.SystemFontOfSize((nfloat)fontSize, UIFontWeight.Semibold);
        }
        else
        {
            font = UIFont.SystemFontOfSize(UIFont.ButtonFontSize, UIFontWeight.Semibold);
        }

        button.TitleLabel!.Font = font;
        button.TitleLabel.AdjustsFontForContentSizeCategory = true;
    }

    private void ConfigureSystemCheckmark()
    {
        _systemToolbar = new UIToolbar();
        _systemToolbar.TranslatesAutoresizingMaskIntoConstraints = false;
        _systemToolbar.BarStyle = UIBarStyle.Default;
        _systemToolbar.Translucent = true;
        _systemToolbar.ClipsToBounds = false;

        var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
        var doneItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, (_, _) => DoneAction?.Invoke());
        _systemToolbar.SetItems(new[] { spacer, doneItem }, false);

        AddSubview(_systemToolbar);

        // Allow the outer view to grow beyond the UIToolbar's own intrinsic height so that
        // iOS 26's larger circular Done button is not clipped at the bottom.
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            _systemToolbar.TopAnchor.ConstraintEqualTo(TopAnchor),
            _systemToolbar.LeadingAnchor.ConstraintEqualTo(LeadingAnchor),
            _systemToolbar.TrailingAnchor.ConstraintEqualTo(TrailingAnchor),
            BottomAnchor.ConstraintGreaterThanOrEqualTo(_systemToolbar.BottomAnchor),
        });

        if (_fixedHeight > 0)
        {
            _heightConstraint = HeightAnchor.ConstraintEqualTo(_fixedHeight);
        }
        else
        {
            // 56 pt is the minimum needed to show the iOS 26 circular Done button without clipping.
            _heightConstraint = HeightAnchor.ConstraintGreaterThanOrEqualTo(56);
        }

        _heightConstraint.Active = true;
    }

    /// <inheritdoc/>
    public override CGSize IntrinsicContentSize
    {
        get
        {
            if (_fixedHeight > 0)
            {
                return new CGSize(UIView.NoIntrinsicMetric, _fixedHeight);
            }

            if (_systemToolbar != null)
            {
                var h = _systemToolbar.IntrinsicContentSize.Height;

                // iOS 26 circular Done button needs at least 56 pt; older iOS is fine with this too.
                return new CGSize(UIView.NoIntrinsicMetric, h < 56 ? (nfloat)56 : h);
            }

            nfloat buttonHeight = 44;
            if (_textButton?.TitleLabel?.Font is { } font)
            {
                buttonHeight = (nfloat)Math.Max(44, Math.Ceiling(font.LineHeight) + 8);
            }

            return new CGSize(UIView.NoIntrinsicMetric, buttonHeight + 16);
        }
    }

    /// <inheritdoc/>
    public override UIView? HitTest(CGPoint point, UIEvent? uiEvent)
    {
        var hit = base.HitTest(point, uiEvent);

        // Pass touches through the toolbar background so users can interact with content that
        // may be partially obscured by the accessory bar. Only subview hits (the Done button)
        // are returned; hits on this view itself fall through to views below.
        return hit == this ? null : hit;
    }

    /// <summary>Updates the title text of the Done button.</summary>
    public void UpdateTitle(string? title)
    {
        if (_textButton == null)
        {
            return;
        }

        _textButton.SetTitle(title ?? KeyboardToolbarOptions.Default.DefaultTitle, UIControlState.Normal);
    }

    /// <summary>Updates the title color of the Done button.</summary>
    public void UpdateTitleColor(Color? titleColor) =>
        UpdateTextButtonColor(_textButton!, titleColor);

    /// <summary>Updates the font of the Done button label.</summary>
    public void UpdateFont(string? fontFamily, double fontSize)
    {
        UpdateTextButtonFont(_textButton!, fontFamily, fontSize);

        // Re-evaluate auto height since a larger font requires more space.
        InvalidateIntrinsicContentSize();
        SetNeedsLayout();
    }

    /// <summary>Updates the background color of the accessory view.</summary>
    public void UpdateBackgroundColor(Color? backgroundColor)
    {
        BackgroundColor = backgroundColor?.ToPlatform() ?? UIColor.Clear;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fontSizeObserver?.Dispose();
            _fontSizeObserver = null;
            _heightConstraint?.Dispose();
            _heightConstraint = null;
            _textButton?.Dispose();
            _textButton = null;
            _systemToolbar?.Dispose();
            _systemToolbar = null;
            DoneAction = null;
        }

        base.Dispose(disposing);
    }
}
