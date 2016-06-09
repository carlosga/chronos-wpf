using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Chronos.Extensions;
using Chronos.Presentation.Core.ViewModel;

namespace Chronos.Presentation.ViewModel
{
    /// <summary>
    /// ViewModel Collection
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    public class CollectionViewModel<TSource, TTarget>
        : IList<TTarget>, ICollection<TTarget>, IEnumerable<TTarget>, ICollectionItemChanged<TSource, TTarget>, IList, ICollection, IEnumerable, INotifyCollectionChanged, IDisposable
        where TSource : INotifyPropertyChanged
        where TTarget : INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event EventHandler<ItemChangedEventArgs<TSource, TTarget>> ItemChanged;

        private bool _disposed;
        private ObservableCollection<TSource> _sourceCollection;
        private ObservableCollection<TTarget> _items;
        private readonly Func<TSource, TTarget> _setup;
        private readonly Func<TTarget, TSource> _coerce;
        private readonly Action<TTarget> _teardown;

        /// <summary>
        /// Gets the list of target items
        /// </summary>
        protected ObservableCollection<TTarget> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ObservableCollection<TTarget>();
                }

                return _items;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewModel&lt;TSource, TTarget&gt;"/> class.
        /// </summary>
        /// <param name="sourceCollection">The source collection.</param>
        /// <param name="setup">The setup function.</param>
        /// <param name="coerce">The coerce function.</param>
        /// <param name="teardown">The teardown function.</param>
        public CollectionViewModel(
            ObservableCollection<TSource> sourceCollection, Func<TSource, TTarget> setup, Func<TTarget, TSource> coerce, Action<TTarget> teardown = null)
        {
            _setup = setup;
            _coerce = coerce;
            _teardown = teardown;
            _sourceCollection = sourceCollection;

            List<TTarget> targetItems = new List<TTarget>(sourceCollection.Select(setup));

            targetItems.ForEach(i => i.PropertyChanged += (x, y) => this.NotifyItemChanged(_coerce(i), i));

            this.Items.AddRange(targetItems);

            var notifyCollectionChanged = this.Items as INotifyCollectionChanged;

            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += this.OnSourceCollectionChanged;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Cleanup managed resources
                    this.Clear();

                    var notifyCollectionChanged = _sourceCollection as INotifyCollectionChanged;

                    if (notifyCollectionChanged != null)
                    {
                        notifyCollectionChanged.CollectionChanged -= this.OnSourceCollectionChanged;
                    }
                }

                // Cleanup unmanaged resources

                // Mark the object as disposed
                _disposed = true;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TTarget"/> at the specified index.
        /// </summary>
        /// <value></value>
        public TTarget this[int index]
        {
            get { return this.Items[index]; }
            set { this.Items[index] = value; }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { return this.Items.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(TTarget item)
        {
            return this.Items.Contains(item);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Add(TTarget item)
        {
            TSource source = _coerce(item);

            item.PropertyChanged += (x, y) => this.NotifyItemChanged(source, item);

            _sourceCollection.Add(source);
            this.Items.Add(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void Insert(int index, TTarget item)
        {
            TSource source = _coerce(item);

            item.PropertyChanged += (x, y) => this.NotifyItemChanged(source, item);

            _sourceCollection.Insert(index, source);
            this.Items.Insert(index, item);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(TTarget item)
        {
            return this.Items.IndexOf(item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public bool Remove(TTarget item)
        {
            if (_teardown != null)
            {
                _teardown(item);
            }

            item.PropertyChanged -= (x, y) => this.NotifyItemChanged(_coerce(item), item);

            this.Items.Remove(item);
            _sourceCollection.Remove(_coerce(item));

            return true;
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void RemoveAt(int index)
        {
            if (_teardown != null)
            {
                _teardown(this.Items[index]);
            }

            TTarget target = this[index];

            target.PropertyChanged -= (x, y) => this.NotifyItemChanged(_coerce(target), target);

            this.Items.RemoveAt(index);
            _sourceCollection.RemoveAt(index);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Clear()
        {
            if (_teardown != null)
            {
                this.Items.ForEach(target => _teardown(target));
            }

            this.Items.ForEach(target => target.PropertyChanged -= (x, y) => this.NotifyItemChanged(_coerce(target), target));

            _sourceCollection.Clear();
            this.Items.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TTarget> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// -or-
        /// Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        void ICollection<TTarget>.CopyTo(TTarget[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to add to the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// The position into which the new element was inserted.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.IList"/> is read-only.
        /// -or-
        /// The <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </exception>
        int IList.Add(object value)
        {
            this.Add((TTarget)value);

            return this.IndexOf((TTarget)value);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        void IList.Clear()
        {
            this.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Object"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, false.
        /// </returns>
        bool IList.Contains(object value)
        {
            return this.Contains((TTarget)value);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// The index of <paramref name="value"/> if found in the list; otherwise, -1.
        /// </returns>
        int IList.IndexOf(object value)
        {
            return this.IndexOf((TTarget)value);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to insert into the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.IList"/> is read-only.
        /// -or-
        /// The <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </exception>
        /// <exception cref="T:System.NullReferenceException">
        /// 	<paramref name="value"/> is null reference in the <see cref="T:System.Collections.IList"/>.
        /// </exception>
        void IList.Insert(int index, object value)
        {
            this.Insert(index, (TTarget)value);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false.
        /// </returns>
        bool IList.IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        bool IList.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to remove from the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.IList"/> is read-only.
        /// -or-
        /// The <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </exception>
        void IList.Remove(object value)
        {
            this.Remove((TTarget)value);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <value></value>
        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (TTarget)value; }
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is less than zero.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        int ICollection.Count
        {
            get { return this.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.
        /// </returns>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        object ICollection.SyncRoot
        {
            get { return null; }
        }

        private void NotifyItemChanged(TSource source, TTarget target)
        {
            if (this.ItemChanged != null)
            {
                this.ItemChanged(this, new ItemChangedEventArgs<TSource, TTarget>(source, target));
            }
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }
    }
}
