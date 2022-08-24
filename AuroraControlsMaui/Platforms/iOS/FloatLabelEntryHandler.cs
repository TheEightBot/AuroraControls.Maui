//using System;
//using Microsoft.Maui.Graphics;
//using Microsoft.Maui.Graphics.Platform;
//using Microsoft.Maui.Handlers;
//using Microsoft.Maui.Platform;

//#if IOS
//using UIKit;
//using CoreGraphics;
//#endif

//namespace AuroraControls
//{
//    public partial class FloatLabelEntryHandler : EntryHandler
//    {
//        private PlatformGraphicsView _graphicsView;
//        private UITapGestureRecognizer _graphicsViewTapped;

//        private static void MapDrawableBackground(FloatLabelEntryHandler handler, FloatLabelEntry view)
//        {
//            handler._backgroundDrawable.BackgroundColor = view.BackgroundColor;
//            handler._graphicsView.InvalidateDrawable();
//        }

//        private static void MapDrawableSize(FloatLabelEntryHandler handler, FloatLabelEntry view)
//        {
//            if (handler._graphicsView != null && view.Height > 0d && view.Width > 0d)
//            {
//                handler._graphicsView.Frame = new CGRect(0f, 0f, (float)view.Width, (float)view.Height);
//                handler._graphicsView.InvalidateDrawable();
//            }
//        }

//        private static void MapDrawableText(FloatLabelEntryHandler handler, FloatLabelEntry view)
//        {
//            MapText(handler, view);

//            handler._backgroundDrawable.HasText = !string.IsNullOrEmpty(view.Text);
//            handler._graphicsView.InvalidateDrawable();
//        }

//        private static void MapDrawablePlaceholder(FloatLabelEntryHandler handler, FloatLabelEntry view)
//        {
//            handler._backgroundDrawable.Placeholder = view.Placeholder;
//            handler._graphicsView.InvalidateDrawable();
//        }

//        private static void MapDrawableFontSize(FloatLabelEntryHandler handler, FloatLabelEntry view)
//        {
//            MapFont(handler, view);
//            handler._backgroundDrawable.FontSize = view.FontSize;
//            handler._graphicsView.InvalidateDrawable();
//        }

//        protected override void ConnectHandler(MauiTextField platformView)
//        {
//            base.ConnectHandler(platformView);

//            platformView.BackgroundColor = UIColor.Clear;
//            platformView.BorderStyle = UITextBorderStyle.None;

//            _graphicsView =
//                new PlatformGraphicsView
//                {
//                    Drawable = _backgroundDrawable = new BackgroundDrawable(),
//                    BackgroundColor = UIColor.Clear,
//                };

//            _graphicsView
//                .AddGestureRecognizer(
//                    _graphicsViewTapped = new UITapGestureRecognizer(() =>
//                    {
//                        VirtualView?.Focus();
//                    }));
//            platformView.InsertSubview(_graphicsView, 0);

//            _graphicsView.InvalidateDrawable();
//        }

//        protected override void DisconnectHandler(MauiTextField platformView)
//        {
//            _graphicsView.RemoveFromSuperview();

//            base.DisconnectHandler(platformView);
//        }
//    }
//}