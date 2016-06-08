// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Chronos.Presentation.Windows
{
    /// <summary>
    /// Common interface for items that can be grouped
    /// on the <see cref="Desktop" />; used by <see cref="DesktopItem" />
    /// </summary>
    public interface IGroupable
    {
        #region · Properties ·

        /// <summary>
        /// Gets the item id.
        /// </summary>
        /// <value>The id.</value>
        Guid Id
        {
            get;
        }

        /// <summary>
        /// Gets or sets the item parent id.
        /// </summary>
        /// <value>The parent id.</value>
        Guid ParentId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is a group.
        /// </summary>
        /// <value><c>true</c> if this instance is group; otherwise, <c>false</c>.</value>
        bool IsGroup
        {
            get;
            set;
        }

        #endregion
    }
}
