using System;
using CoreGraphics;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using UIKit;

namespace AuroraControls.Platforms.iOS
{
    public class AuroraTextField : MauiTextField
    {
        private UIEdgeInsets inset = new UIEdgeInsets(0f, 0f, 0f, 0f);

        public UIEdgeInsets Inset
        {
            get => inset;
            set
            {
                inset = value;
                this.SetNeedsDisplay();
            }
        }

        public AuroraTextField(CGRect frame) : base(frame)
        {
        }

        public AuroraTextField() : base()
        {
        }

        public override CGRect TextRect(CGRect forBounds)
        {
            var rect = base.TextRect(forBounds);

            return Inset.InsetRect(rect);
        }

        public override CGRect EditingRect(CGRect forBounds)
        {
            var rect = base.EditingRect(forBounds);

            return Inset.InsetRect(rect);
        }

        public override CGRect PlaceholderRect(CGRect forBounds)
        {
            var rect = base.PlaceholderRect(forBounds);

            return Inset.InsetRect(rect);
        }
    }
}

