using System;
using UIKit;

namespace AuroraControls.Platforms.iOS
{
    public interface IHaveInsets
    {
        public UIEdgeInsets Inset{ get; set; }

        public Rect GetViewLocation();
    }
}

