// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace Chronos.Presentation.Core.ViewModel
{
    /// <summary>
    /// Interface for entity viewmodel implementations
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEntityViewModel<TEntity> : 
        IDataErrorInfo where TEntity : class, new()
    {
        #region · Properties ·

        /// <summary>
        /// Gets a value indicating wheter this instance is valid
        /// </summary>
        bool IsValid
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating wheter this instance has changes
        /// </summary>
        bool HasChanges
        {
            get;
        }

        #endregion
    }
}
