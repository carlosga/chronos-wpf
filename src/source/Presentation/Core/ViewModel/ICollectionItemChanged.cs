// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chronos.Presentation.Core.ViewModel
{
    /// <summary>
    /// Interface for collections that supports item change notifications
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public interface ICollectionItemChanged<T, V>
    {
        #region · Events ·

        /// <summary>
        /// Occurs when a property of an item collection has been changed
        /// </summary>
        event EventHandler<ItemChangedEventArgs<T, V>> ItemChanged;

        #endregion
    }
}
