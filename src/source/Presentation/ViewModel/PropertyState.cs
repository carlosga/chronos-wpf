// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Chronos.Presentation.ViewModel
{
    /// <summary>
    /// Represents a property state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class PropertyState<T>
        : ObservableObject where T : class
    {
        private static readonly PropertyChangedEventArgs s_isEditableChangedArgs = CreateArgs<PropertyState<T>>(x => x.IsEditable);
        private static readonly PropertyChangedEventArgs s_isReadOnlyChangedArgs = CreateArgs<PropertyState<T>>(x => x.IsReadOnly);

        private string _name;
        private bool _isEditable;

        /// <summary>
        /// Gets the property name
        /// </summary>
        public string PropertyName
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the property is editable
        /// </summary>
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                this.NotifyPropertyChanged(s_isEditableChangedArgs);
                this.NotifyPropertyChanged(s_isReadOnlyChangedArgs);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the property is readonly
        /// </summary>
        public bool IsReadOnly
        {
            get { return !_isEditable; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyState&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public PropertyState(string propertyName)
        {
            _name = propertyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyState&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        public PropertyState(Expression<Func<T, object>> propertyExpression)
        {
            _name = propertyExpression.GetPropertyName();
        }
    }
}
