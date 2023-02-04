using System;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Animations;
using Microsoft.Maui;
using System.Runtime.CompilerServices;


#if IOS
using UIKit;
using CoreGraphics;
#endif

namespace AuroraControls;

public partial class FloatLabelEntryHandler : EntryHandler, IHavePlatformUnderlayDrawable
{
    public PlatformUnderlayDrawable PlatformUnderlayDrawable { get; private set; }

    public FloatLabelEntryHandler()
        : base(FloatLabelEntryMapper)
    {
    }

#if IOS
    protected override MauiTextField CreatePlatformView()
    {
        var platformView = new AuroraControls.Platforms.iOS.AuroraTextField();

        platformView.BackgroundColor = UIColor.Clear;
        platformView.BorderStyle = UITextBorderStyle.None;

        return platformView;
    }

    protected override void ConnectHandler(MauiTextField platformView)
    {
        base.ConnectHandler(platformView);

        PlatformUnderlayDrawable = new PlatformUnderlayDrawable();
        PlatformUnderlayDrawable.ConnectHandler(platformView, VirtualView);
    }

    protected override void DisconnectHandler(MauiTextField platformView)
    {
        PlatformUnderlayDrawable?.DisconnectHandler();

        base.DisconnectHandler(platformView);
    }
#endif

    public override void PlatformArrange(Rect rect)
    {
        base.PlatformArrange(rect);

        PlatformUnderlayDrawable?.PlatformArrange(rect);
    }
}