namespace AuroraControls;

/// <summary>
/// Math extensions.
/// </summary>
public static class MathExtensions
{
    /// <summary>
    /// Lerp the specified start, end and amount.
    /// </summary>
    /// <returns>The lerp.</returns>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="amount">Amount.</param>
    public static double Lerp(this double start, double end, double amount)
    {
        double difference = end - start;
        double adjusted = difference * amount;
        return start + adjusted;
    }

    public static float Lerp(this float start, float end, float amount)
    {
        float difference = end - start;
        float adjusted = difference * amount;
        return start + adjusted;
    }

    public static int Lerp(this int start, int end, double amount)
    {
        int difference = end - start;
        int adjusted = (int)(difference * amount);
        return start + adjusted;
    }

    public static byte Lerp(this byte start, byte end, double amount)
    {
        int difference = end - start;
        int adjusted = (int)(difference * amount);

        return (byte)(start + adjusted);
    }
}
