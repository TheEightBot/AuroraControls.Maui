﻿using System.Drawing;

namespace AuroraControls;

public static class DrawingExtensions
{
    public static System.Drawing.Point Center(this Rectangle rectangle) =>
        new(
            rectangle.Left + (rectangle.Width / 2),
            rectangle.Top + (rectangle.Height / 2));

    public static System.Drawing.PointF Center(this RectangleF rectangle) =>
        new(
            rectangle.Left + (rectangle.Width / 2f),
            rectangle.Top + (rectangle.Height / 2f));
}
