namespace AuroraControls.TestApp;

public static class Extensions
{
    public static T RandomEnum<T>(Random random = null)
    {
        random ??= new Random(Guid.NewGuid().GetHashCode());

        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(random.Next(values.Length));
    }

    public static T Next<T>(this T src)
        where T : struct
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");
        }

        T[] arr = (T[])Enum.GetValues(src.GetType());

        int j = (Array.IndexOf<T>(arr, src) + 1) % arr.Length; // <- Modulo % Arr.Length added

        return arr[j];
    }
}
