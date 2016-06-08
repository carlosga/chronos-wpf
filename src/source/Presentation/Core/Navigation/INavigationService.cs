// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using nRoute.Navigation;

namespace Chronos.Presentation.Core.Navigation
{
    /// <summary>
    /// Interface for view navigation services
    /// </summary>
    public interface INavigationService
    {
        #region · Methods ·

        /// <summary>
        /// Performs the navigation to the given target
        /// </summary>
        /// <param name="target"></param>
        void Navigate(string target);

        /// <summary>
        /// Performs the navigation to the given target
        /// </summary>
        /// <param name="target"></param>
        void Navigate(NavigateMode mode, string target);
        
        /// <summary>
        /// Performs the navigation to the given target
        /// </summary>
        /// <param name="target"></param>
        void Navigate(string target, params object[] args);

        /// <summary>
        /// Performs the navigation to the given target
        /// </summary>
        /// <param name="target"></param>
        void Navigate(NavigateMode mode, string target, params object[] args);
        
        #endregion
    }
}
