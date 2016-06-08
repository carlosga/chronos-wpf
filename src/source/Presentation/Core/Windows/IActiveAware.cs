// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Chronos.Presentation.Core.Windows
{
    /// <summary>
    /// Allows an object to provide information about whether it is active or not.
    /// </summary>
    public interface IActiveAware
    {
        #region · Properties ·

        /// <summary>
        /// Gets or sets a value indicating whether the element is active.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        bool IsActive
        {
            get;
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Activates the element
        /// </summary>
        void Activate();

        /// <summary>
        /// Deactivates the element
        /// </summary>
        void Deactivate();

        #endregion
    }
}
