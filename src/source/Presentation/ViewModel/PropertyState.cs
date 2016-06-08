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
        : ObservableObject where T: class
    {
        #region � PropertyChangedEventArgs Cached Instances �

        private static readonly PropertyChangedEventArgs IsEditableChangedArgs = CreateArgs<PropertyState<T>>(x => x.IsEditable);
        private static readonly PropertyChangedEventArgs IsReadOnlyChangedArgs = CreateArgs<PropertyState<T>>(x => x.IsReadOnly);

        #endregion

        #region � Fields �

        private string	name;
        private bool    isEditable;
        
        #endregion

        #region � Properties �

        /// <summary>
        /// Gets the property name
        /// </summary>
        public string PropertyName
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the property is editable
        /// </summary>
        public bool IsEditable
        {
            get { return this.isEditable; }
            set
            {
                this.isEditable = value;
                this.NotifyPropertyChanged(IsEditableChangedArgs);
                this.NotifyPropertyChanged(IsReadOnlyChangedArgs);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the property is readonly
        /// </summary>
        public bool IsReadOnly
        {
            get { return !this.isEditable; }
        }

        #endregion

        #region � Constructors �

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyState&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public PropertyState(string propertyName)
        {
            this.name = propertyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyState&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        public PropertyState(Expression<Func<T, object>> propertyExpression)
        {
            this.name = propertyExpression.GetPropertyName();
        }

        #endregion
    }
}