using System;
using CoreGraphics;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using UIKit;

namespace AuroraControls.Platforms.iOS
{
    public class AuroraTextView : MauiTextView, IHaveInsets
    {
        private UIEdgeInsets inset = new UIEdgeInsets(0f, 0f, 0f, 0f);

        public UIEdgeInsets Inset
        {
            get => inset;
            set
            {
                inset = value;
                ContentInset = inset;
                this.SetNeedsDisplay();
            }
        }

        public AuroraTextView(CGRect frame) : base(frame)
        {
        }

        public AuroraTextView() : base()
        {
        }

        public Rect GetViewLocation()
        {
            //TODO: ???
            return this.ContentInset.InsetRect(this.Frame).ToRectangle();
        }
    }
}

