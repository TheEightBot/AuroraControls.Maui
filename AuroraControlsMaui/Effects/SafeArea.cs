using System.ComponentModel;

namespace AuroraControls.Effects;

[TypeConverter(typeof(SafeAreaTypeConverter))]
public struct SafeArea
{
    private readonly bool _isParameterized;

    public bool Left { get; }

    public bool Top { get; }

    public bool Right { get; }

    public bool Bottom { get; }

    public bool IsEmpty
        => !Left && !Top && !Right && !Bottom;

    public SafeArea(bool uniformSafeArea)
        : this(uniformSafeArea, uniformSafeArea, uniformSafeArea, uniformSafeArea)
    {
    }

    public SafeArea(bool horizontal, bool vertical)
        : this(horizontal, vertical, horizontal, vertical)
    {
    }

    public SafeArea(bool left, bool top, bool right, bool bottom)
    {
        _isParameterized = true;

        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public static implicit operator SafeArea(bool uniformSafeArea)
        => new(uniformSafeArea);

    private bool Equals(SafeArea other)
        => (!_isParameterized &&
            !other._isParameterized) ||
           (Left == other.Left &&
            Top == other.Top &&
            Right == other.Right &&
            Bottom == other.Bottom);

    public override bool Equals(object? obj)
        => obj is not null
           && obj is SafeArea safeArea
           && Equals(safeArea);

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Left.GetHashCode();
            hashCode = (hashCode * 397) ^ Top.GetHashCode();
            hashCode = (hashCode * 397) ^ Right.GetHashCode();
            hashCode = (hashCode * 397) ^ Bottom.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(SafeArea left, SafeArea right)
        => left.Equals(right);

    public static bool operator !=(SafeArea left, SafeArea right)
        => !left.Equals(right);

    public void Deconstruct(out bool left, out bool top, out bool right, out bool bottom)
    {
        left = Left;
        top = Top;
        right = Right;
        bottom = Bottom;
    }
}
