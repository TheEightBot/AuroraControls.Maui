using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AuroraControls.VisualEffects;

public class VisualEffectCollection : BindableObject, IList<VisualEffect>, INotifyCollectionChanged
{
    private readonly List<VisualEffect> _items = new();

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public event EventHandler<PropertyChangedEventArgs> EffectPropertyChanged;

    ~VisualEffectCollection()
    {
        if (_items != null)
        {
            foreach (var item in _items)
            {
                item.PropertyChanged -= HandlePropertyChangedEventHandler;
            }
        }
    }

    public int IndexOf(VisualEffect item) => _items.IndexOf(item);

    public void Insert(int index, VisualEffect item)
    {
        _items.Insert(index, item);

        item.PropertyChanged -= HandlePropertyChangedEventHandler;
        item.PropertyChanged += HandlePropertyChangedEventHandler;

        CollectionChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
    }

    public void RemoveAt(int index)
    {
        var oldItem = this[index];
        _items.RemoveAt(index);
        oldItem.PropertyChanged -= HandlePropertyChangedEventHandler;
        CollectionChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
    }

    public VisualEffect this[int index]
    {
        get => _items[index];

        set
        {
            var oldItem = this[index];

            var imageProcessingBase = (VisualEffect)value;

            _items[index] = imageProcessingBase;

            imageProcessingBase.PropertyChanged -= HandlePropertyChangedEventHandler;
            imageProcessingBase.PropertyChanged += HandlePropertyChangedEventHandler;

            oldItem.PropertyChanged -= HandlePropertyChangedEventHandler;
            CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem));
        }
    }

    public void Add(VisualEffect item)
    {
        if (item == null)
        {
            return;
        }

        _items.Add(item);
        item.PropertyChanged -= HandlePropertyChangedEventHandler;
        item.PropertyChanged += HandlePropertyChangedEventHandler;

        CollectionChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, Count - 1));
    }

    public void Clear()
    {
        foreach (var item in _items)
        {
            item.PropertyChanged -= HandlePropertyChangedEventHandler;
        }

        _items.Clear();

        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public bool Contains(VisualEffect item) => _items.Contains(item);

    public void CopyTo(VisualEffect[] array, int arrayIndex)
    {
        foreach (var item in array)
        {
            item.PropertyChanged -= HandlePropertyChangedEventHandler;
            item.PropertyChanged += HandlePropertyChangedEventHandler;
        }

        _items.CopyTo(array, arrayIndex);
    }

    public bool Remove(VisualEffect item)
    {
        int oldIndex = IndexOf(item);

        if (_items.Remove(item))
        {
            item.PropertyChanged -= HandlePropertyChangedEventHandler;
            CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, oldIndex));
            return true;
        }

        return false;
    }

    public int Count => _items.Count;

    public bool IsReadOnly => false;

    public IEnumerator<VisualEffect> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void HandlePropertyChangedEventHandler(object sender, PropertyChangedEventArgs e) => EffectPropertyChanged?.Invoke(sender, e);
}
