namespace AuroraControls.Svg;

internal class SKText : IEnumerable<SKTextSpan>
{
    private readonly List<SKTextSpan> _spans = new List<SKTextSpan>();

    public SKText(SKPoint location, SKTextAlign textAlign)
    {
        Location = location;
        TextAlign = textAlign;
    }

    public void Append(SKTextSpan span)
    {
        _spans.Add(span);
    }

    public SKPoint Location { get; }

    public SKTextAlign TextAlign { get; }

    public float MeasureTextWidth() => _spans.Sum(x => x.MeasureTextWidth());

    public IEnumerator<SKTextSpan> GetEnumerator() => _spans.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}