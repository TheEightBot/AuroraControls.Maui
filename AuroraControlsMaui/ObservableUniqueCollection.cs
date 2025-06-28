using System.Collections.ObjectModel;

namespace AuroraControls;

internal class ObservableUniqueCollection<T> : ObservableCollection<T>
{
    private readonly object _lock = new object();

    /// <summary>
    /// Inserts the item into the observable collection.
    /// </summary>
    /// <param name="index">index to insert at.</param>
    /// <param name="item">Item to insert.</param>
    protected override void InsertItem(int index, T item)
    {
        lock (_lock)
        {
            bool exists = false;

            foreach (var myItem in Items.Where(myItem => myItem.Equals(item)))
            {
                exists = true;
            }

            if (!exists)
            {
                base.InsertItem(index, item);
            }
        }
    }

    /// <summary>
    /// Adds the range of items to the collection.
    /// </summary>
    /// <param name="range">IEnumerable of items to add.</param>
    public void AddRange(IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        foreach (var item in range)
        {
            Add(item);
        }
    }
}
