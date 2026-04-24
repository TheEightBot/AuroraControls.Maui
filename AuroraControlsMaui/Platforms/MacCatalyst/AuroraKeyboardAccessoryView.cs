using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace AuroraControls.Platforms.MacCatalyst;

/// <summary>
/// Custom keyboard accessory view that replaces MAUI's fixed-height UIToolbar.
/// Supports two button styles:
/// <list type="bullet">
///   <item><see cref="KeyboardToolbarDoneButtonStyle.Text"/> — a <see cref="UIButton"/> with configurable title, color, and font.</item>
///   <item><see cref="KeyboardToolbarDoneButtonStyle.SystemCheckmark"/> — a <see cref="UIToolbar"/> pinned inside an unconstrained outer view; not clipped on macOS Catalyst.</item>
/// </list>
/// </summary>
internal sealed class AuroraKeyboardAccessoryView : UIView
{
    private UIButton? _textButton;
    private UIToolbar? _systemToolbar;
    private NSObject? _fontSizeObserver;
    private nfloat _fixedHeight;

    /// <summary>Gets or sets the action invoked when the Done button is tapped.</summary>
    public Action? DoneAction { get; set; }

    public AuroraKeyboardAccessoryView()
        : base(CGRect.Empty)
    {
        TranslatesAutoresizingMaskIntoConstraints = false;
        AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
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

        BackgroundColor = backgroundColor != null
            ? backgroundColor.ToPlatform()
            : UIColor.SystemBackground;

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
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                HeightAnchor.ConstraintGreaterThanOrEqualTo(60),
            });
        }
        else
        {
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                HeightAnchor.ConstraintEqualTo(_fixedHeight),
            });
        }
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

        var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
        var doneItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, (_, _) => DoneAction?.Invoke());
        _systemToolbar.SetItems(new[] { spacer, doneItem }, false);

        AddSubview(_systemToolbar);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            _systemToolbar.TopAnchor.ConstraintEqualTo(TopAnchor),
            _systemToolbar.LeadingAnchor.ConstraintEqualTo(LeadingAnchor),
            _systemToolbar.TrailingAnchor.ConstraintEqualTo(TrailingAnchor),
            _systemToolbar.BottomAnchor.ConstraintEqualTo(BottomAnchor),
        });

        if (_fixedHeight > 0)
        {
            HeightAnchor.ConstraintEqualTo(_fixedHeight).Active = true;
        }
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
                return new CGSize(UIView.NoIntrinsicMetric, h > 0 ? h : 44);
            }

            nfloat buttonHeight = 44;
            if (_textButton?.TitleLabel?.Font is { } font)
            {
                buttonHeight = (nfloat)Math.Max(44, Math.Ceiling(font.LineHeight) + 8);
            }

            return new CGSize(UIView.NoIntrinsicMetric, buttonHeight + 16);
        }
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
    public void UpdateFont(string? fontFamily, double fontSize) =>
        UpdateTextButtonFont(_textButton!, fontFamily, fontSize);

    /// <summary>Updates the background color of the accessory view.</summary>
    public void UpdateBackgroundColor(Color? backgroundColor)
    {
        BackgroundColor = backgroundColor?.ToPlatform() ?? UIColor.SystemBackground;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fontSizeObserver?.Dispose();
            _fontSizeObserver = null;
            _textButton?.Dispose();
            _textButton = null;
            _systemToolbar?.Dispose();
            _systemToolbar = null;
            DoneAction = null;
        }

        base.Dispose(disposing);
    }
}
