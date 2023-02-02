using System;
namespace AuroraControls;

/// <summary>
/// Color extensions.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Lerp the specified color, to and amount.
    /// </summary>
    /// <returns>The lerp.</returns>
    /// <param name="color">Color to calculate from</param>
    /// <param name="to">Color to calculate to</param>
    /// <param name="amount">Amount amount to applys</param>
    public static Color Lerp(this Color color, Color to, float amount)
    {
        var returnColor = Color.FromRgba(
            color.Red.Lerp(to.Red, amount),
            color.Green.Lerp(to.Green, amount),
            color.Blue.Lerp(to.Blue, amount),
            color.Alpha.Lerp(to.Alpha, amount));

        return returnColor;
    }

    public static SKColor Lerp(this SKColor color, SKColor to, double amount)
    {
        var returnColor = new SKColor(
            (color.Red).Lerp(to.Red, amount),
            (color.Green).Lerp(to.Green, amount),
            (color.Blue).Lerp(to.Blue, amount),
            (color.Alpha).Lerp(to.Alpha, amount));

        return returnColor;
    }

    public static SKColor WithAlpha(this SKColor color, float alpha)
    {
        var alphaByte = Math.Max(Math.Min(byte.MaxValue, (int)(byte.MaxValue * alpha)), byte.MinValue);

        return color.WithAlpha((byte)alphaByte);
    }
}