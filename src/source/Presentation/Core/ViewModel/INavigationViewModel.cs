// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using nRoute.Components;
using nRoute.Navigation;

namespace Chronos.Presentation.Core.ViewModel
{
    /// <summary>
    /// Interface for navigation viewmodel implementations
    /// </summary>
    public interface INavigationViewModel 
        : IClosableViewModel, ISupportNavigationLifecycle, ISupportNavigationState
    {
        #region · Commands ·

        /// <summary>
        /// Gets the open new window command
        /// </summary>
        ActionCommand NewWindowCommand
        {
            get;
        }

        /// <summary>
        /// Gets the restore window command.
        /// </summary>
        /// <value>The restore window command.</value>
        ActionCommand RestoreCommand
        {
            get;
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the navigation route
        /// </summary>
        string NavigationRoute
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating wheter the viewmodel has available relations
        /// </summary>
        bool HasRelations
        {
            get;
        }

        #endregion
    }
}
