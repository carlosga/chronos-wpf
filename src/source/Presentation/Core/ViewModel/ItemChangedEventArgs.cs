// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chronos.Presentation.Core.ViewModel
{
    /// <summary>
    /// Event argument for the currentchanged eventhandler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public sealed class ItemChangedEventArgs<T, V> 
        : EventArgs
    {
        #region · Properties ·

        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        /// <value>The current item.</value>
        public T Item
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the current viewmodel.
        /// </summary>
        /// <value>The current viewmodel.</value>
        public V ItemViewModel
        {
            get;
            private set;
        }

        #endregion
        
        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemChangedArgs&lt;T, V&gt;"/> class.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        /// <param name="currentViewModel">The current viewmodel.</param>
        public ItemChangedEventArgs(T item, V itemViewModel)
            : base()
        {
            this.Item           = item;
            this.ItemViewModel  = itemViewModel;
        }

        #endregion
    }
}
