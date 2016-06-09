// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace Chronos.Presentation.ViewModel
{
    /// <summary>
    /// <see cref="PropertyState&lt;T&gt;"/> collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class PropertyStateCollection<T>
        : IList<PropertyState<T>>, ICollection<PropertyState<T>>, IEnumerable<PropertyState<T>>, IList, ICollection, IEnumerable
            where T : class
    {
        private IList<PropertyState<T>> _innerList;

        /// <summary>
        /// Gets the <see cref="Chronos.Presentation.ViewModel.PropertyState&lt;T&gt;"/> with the specified property name.
        /// </summary>
        /// <value></value>
        public PropertyState<T> this[string propertyName]
        {
            get { return _innerList.FirstOrDefault(o => o.PropertyName == propertyName); }
        }

        /// <summary>
        /// Gets the <see cref="Chronos.Presentation.ViewModel.PropertyState&lt;T&gt;"/> with the specified property.
        /// </summary>
        /// <value></value>
        public PropertyState<T> this[Expression<Func<T, object>> property]
        {
            get { return _innerList.FirstOrDefault(o => o.PropertyName == property.GetPropertyName()); }
        }

        /// <summary>
        /// Gets the number of property states in the collection.
        /// </summary>
        /// <value>The number of property states in the collection.</value>
        public int Count
        {
            get { return _innerList.Count; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyStateCollection&lt;T&gt;"/> class.
        /// </summary>
        public PropertyStateCollection()
        {
            _innerList = new List<PropertyState<T>>();
        }

        /// <summary>
        /// Adds a new <see cref="PropertyState&lt;T&gt;"/> instance for the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public PropertyState<T> Add(string propertyName)
        {
            PropertyState<T> editor = new PropertyState<T>(propertyName);

            _innerList.Add(editor);

            return editor;
        }

        /// <summary>
        /// Adds the specified property expression.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns></returns>
        public PropertyState<T> Add(Expression<Func<T, object>> propertyExpression)
        {
            PropertyState<T> editor = new PropertyState<T>(propertyExpression);

            _innerList.Add(editor);

            return editor;
        }

        /// <summary>
        /// Removes all items from the <see cref="PropertyStateCollection&lt;T&gt;"/>.
        /// </summary>
        public void Clear()
        {
            if (_innerList != null)
            {
                _innerList.Clear();
                _innerList = null;
            }
        }

        public int IndexOf(PropertyState<T> item)
        {
            return _innerList.IndexOf(item);
        }

        public void Insert(int index, PropertyState<T> item)
        {
            _innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _innerList.RemoveAt(index);
        }

        public PropertyState<T> this[int index]
        {
            get { return _innerList[index]; }
            set { _innerList[index] = value; }
        }

        public void Add(PropertyState<T> item)
        {
            _innerList.Add(item);
        }

        public bool Contains(PropertyState<T> item)
        {
            return _innerList.Contains(item);
        }

        public void CopyTo(PropertyState<T>[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return _innerList.IsReadOnly; }
        }

        public bool Remove(PropertyState<T> item)
        {
            return _innerList.Remove(item);
        }

        public IEnumerator<PropertyState<T>> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        void ICollection<PropertyState<T>>.Add(PropertyState<T> item)
        {
            this.Add(item);
        }

        void ICollection<PropertyState<T>>.Clear()
        {
            this.Clear();
        }

        bool ICollection<PropertyState<T>>.Contains(PropertyState<T> item)
        {
            return this.Contains(item);
        }

        void ICollection<PropertyState<T>>.CopyTo(PropertyState<T>[] array, int arrayIndex)
        {
            this.CopyTo(array, arrayIndex);
        }

        int ICollection<PropertyState<T>>.Count
        {
            get { return _innerList.Count; }
        }

        bool ICollection<PropertyState<T>>.IsReadOnly
        {
            get { return this.IsReadOnly; }
        }

        bool ICollection<PropertyState<T>>.Remove(PropertyState<T> item)
        {
            return this.Remove(item);
        }

        IEnumerator<PropertyState<T>> IEnumerable<PropertyState<T>>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Add(object value)
        {
            _innerList.Add((PropertyState<T>)value);

            return this.IndexOf(value);
        }

        public bool Contains(object value)
        {
            return this.Contains((PropertyState<T>)value);
        }

        public int IndexOf(object value)
        {
            return this.IndexOf((PropertyState<T>)value);
        }

        public void Insert(int index, object value)
        {
            this.Insert(index, (PropertyState<T>)value);
        }

        public bool IsFixedSize
        {
            get { return ((IList)_innerList).IsFixedSize; }
        }

        public void Remove(object value)
        {
            this.Remove((PropertyState<T>)value);
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (PropertyState<T>)value; }
        }

        public void CopyTo(Array array, int index)
        {
            this.CopyTo((PropertyState<T>[])array, index);
        }

        public bool IsSynchronized
        {
            get { return ((IList)_innerList).IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return ((IList)_innerList).SyncRoot; }
        }
    }
}
