namespace AuroraControls;

public class SKTextRunLookup : IDisposable
{
    private static readonly Lazy<SKTextRunLookup> instance = new Lazy<SKTextRunLookup>(() => new SKTextRunLookup(true));

    private readonly List<SKTextRunLookupEntry> _entries;
    private readonly bool _disposeEntries;

    public SKTextRunLookup()
        : this(false)
    {
    }

    public SKTextRunLookup(bool disposeEntries)
    {
        this._disposeEntries = disposeEntries;
        _entries = new List<SKTextRunLookupEntry>();
    }

    public static SKTextRunLookup Instance => instance.Value;

    public IEnumerable<SKTypeface> Typefaces => _entries.Select(l => l.Typeface);

    public void AddTypeface(SKTypeface typeface, IReadOnlyDictionary<string, string> characters)
    {
        if (typeface is null)
        {
            throw new ArgumentNullException(nameof(typeface));
        }

        if (characters is null)
        {
            throw new ArgumentNullException(nameof(characters));
        }

        _entries.Add(new SKTextRunLookupEntry(typeface, characters));
    }

    public void AddTypeface(SKTextRunLookupEntry entry)
    {
        if (entry is null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        if (!_entries.Contains(entry))
        {
            _entries.Add(entry);
        }
    }

    public void RemoveTypeface(SKTextRunLookupEntry entry)
    {
        if (entry is null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        if (_entries.Contains(entry))
        {
            _entries.Remove(entry);
        }
    }

    public bool TryLookup(string template, out SKTypeface typeface, out string character)
    {
        foreach (var entry in _entries)
        {
            if (entry.Characters.ContainsKey(template))
            {
                typeface = entry.Typeface;
                character = entry.Characters[template];
                return true;
            }
        }

        typeface = null;
        character = null;
        return false;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposeEntries)
        {
            foreach (var entry in _entries)
            {
                entry.Dispose();
            }
        }

        _entries.Clear();
    }

    public void Dispose()
    {
        Dispose(true);
    }
}
