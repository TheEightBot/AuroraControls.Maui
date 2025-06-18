using System.Collections;
using System.Collections.Specialized;

namespace AuroraControls.ImageProcessing;

/// <summary>
/// Image processing collection.
/// </summary>
public class ImageProcessingCollection : BindableObject, IList<ImageProcessingBase>, INotifyCollectionChanged
{
    /// <summary>
    /// The collection of image processors.
    /// </summary>
    private readonly List<ImageProcessingBase> _items = new List<ImageProcessingBase>();

    /// <summary>
    /// Occurs when collection changed.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    /// <summary>
    /// Finalizes an instance of the <see cref="ImageProcessingCollection"/> class.
    /// Releases unmanaged resources and performs other cleanup operations before the
    /// <see cref="T:AuroraControls.ImageProcessing.ImageProcessingCollection"/> is reclaimed by garbage collection.
    /// </summary>
    ~ImageProcessingCollection()
    {
        if (this._items == null)
        {
            return;
        }

        foreach (var item in this._items)
        {
            item.PropertyChanged -= this.HandlePropertyChangedEventHandler;
        }
    }

    /// <summary>
    /// Gets the index in the collection of a given processor.
    /// </summary>
    /// <returns>The index of the processor.</returns>
    /// <param name="item">Item.</param>
    public int IndexOf(ImageProcessingBase item) => this._items.IndexOf(item);

    /// <summary>
    /// Inserts the processor at the specified index.
    /// </summary>
    /// <param name="index">Index to insert at.</param>
    /// <param name="item">Image processor.</param>
    public void Insert(int index, ImageProcessingBase item)
    {
        this._items.Insert(index, item);

        item.PropertyChanged -= HandlePropertyChangedEventHandler;
        item.PropertyChanged += HandlePropertyChangedEventHandler;

        SetValue(Effects.ImageProcessingEffect.ProcessorChangedProperty, item);
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
    }

    /// <summary>
    /// Removes processor at index.
    /// </summary>
    /// <param name="index">Index.</param>
    public void RemoveAt(int index)
    {
        var oldItem = this[index];
        this._items.RemoveAt(index);
        oldItem.PropertyChanged -= HandlePropertyChangedEventHandler;
        SetValue(Effects.ImageProcessingEffect.ProcessorChangedProperty, oldItem);
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
    }

    /// <summary>
    /// Gets or sets the <see cref="T:AuroraControls.ImageProcessing.ImageProcessingCollection"/> at the specified index.
    /// </summary>
    /// <param name="index">Index.</param>
    public ImageProcessingBase this[int index]
    {
        get => this._items[index];

        set
        {
            var oldItem = this[index];

            var imageProcessingBase = (ImageProcessingBase)value;

            this._items[index] = imageProcessingBase;

            imageProcessingBase.PropertyChanged -= HandlePropertyChangedEventHandler;
            imageProcessingBase.PropertyChanged += HandlePropertyChangedEventHandler;

            oldItem.PropertyChanged -= HandlePropertyChangedEventHandler;
            SetValue(Effects.ImageProcessingEffect.ProcessorChangedProperty, oldItem);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem));
        }
    }

    /// <summary>
    /// Add the specified processor to the list.
    /// </summary>
    /// <param name="item">Image processor.</param>
    public void Add(ImageProcessingBase item)
    {
        this._items.Add(item);

        item.PropertyChanged -= HandlePropertyChangedEventHandler;
        item.PropertyChanged += HandlePropertyChangedEventHandler;

        SetValue(Effects.ImageProcessingEffect.ProcessorChangedProperty, item);
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, Count - 1));
    }

    /// <summary>
    /// Clears the list.
    /// </summary>
    public void Clear()
    {
        foreach (var item in this._items)
        {
            item.PropertyChanged -= HandlePropertyChangedEventHandler;
        }

        this._items.Clear();

        SetValue(Effects.ImageProcessingEffect.ProcessorChangedProperty, null);
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Checks if the processor passed in already exists in the list.
    /// </summary>
    /// <returns>Returns <c>true</c> if the item exists in the list, otherwise <c>false</c>.</returns>
    /// <param name="item">Item.</param>
    public bool Contains(ImageProcessingBase item) => this._items.Contains(item);

    /// <summary>
    /// Copies items to list.
    /// </summary>
    /// <param name="array">Array of processors.</param>
    /// <param name="arrayIndex">Array index.</param>
    public void CopyTo(ImageProcessingBase[] array, int arrayIndex)
    {
        foreach (var item in array)
        {
            item.PropertyChanged -= HandlePropertyChangedEventHandler;
            item.PropertyChanged += HandlePropertyChangedEventHandler;
        }

        this._items.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Remove the specified item.
    /// </summary>
    /// <returns>The remove.</returns>
    /// <param name="item">Item.</param>
    public bool Remove(ImageProcessingBase item)
    {
        var oldIndex = IndexOf(item);

        if (!this._items.Remove(item))
        {
            return false;
        }

        item.PropertyChanged -= this.HandlePropertyChangedEventHandler;
        this.SetValue(Effects.ImageProcessingEffect.ProcessorChangedProperty, item);
        this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, oldIndex));
        return true;
    }

    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <value>The count.</value>
    public int Count => this._items.Count;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:AuroraControls.ImageProcessing.ImageProcessingCollection"/> is
    /// read only.
    /// </summary>
    /// <value><c>true</c> if is read only; otherwise, <c>false</c>.</value>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<ImageProcessingBase> GetEnumerator() => this._items.GetEnumerator();

    /// <summary>
    /// System.s the collections. IE numerable. get enumerator.
    /// </summary>
    /// <returns>The collections. IE numerable. get enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Handles the property changed event handler.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e"><c>PropertyChangedEventArgs</c> provides the <see cref="T:PropertyChangedEventArgs.PropertyName"/> property to get the name of the property that changed.</param>
    private void HandlePropertyChangedEventHandler(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        ClearValue(Effects.ImageProcessingEffect.ProcessorChangedProperty);
        SetValue(Effects.ImageProcessingEffect.ProcessorChangedProperty, sender);
    }
}
