namespace AuroraControls;

public static class AndroidExtensions
{
    public static Android.Graphics.Color ToAndroidColor(this Microsoft.Maui.Graphics.Color color)
    {
        return Android.Graphics.Color.Argb(
            (int)(color.Alpha * 255),
            (int)(color.Red * 255),
            (int)(color.Green * 255),
            (int)(color.Blue * 255));
    }
}
