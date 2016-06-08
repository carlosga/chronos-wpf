// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using nRoute.Components;

namespace Chronos.Presentation.Core.ViewModel
{
    /// <summary>
    /// Interface for closable ViewModel implementations
    /// </summary>
    public interface IClosableViewModel
        : IObservableObject
    {
        #region · Commands ·

        /// <summary>
        /// Gets the close command
        /// </summary>
        ActionCommand CloseCommand
        {
            get;
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the viewmodel identifier
        /// </summary>
        Guid Id
        {
            get;
        }

        /// <summary>
        /// Gets or sets the viewmodel title
        /// </summary>
        string Title
        {
            get;
            set;
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Closes the viewmodel
        /// </summary>
        void Close();

        #endregion
    }
}
