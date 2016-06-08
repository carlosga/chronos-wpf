// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Chronos.Presentation.Core.Windows
{
    /// <summary>
    /// Used by the ViewModel to set the state of all
    /// the data of a particular views data to the correct state
    /// </summary>
    [Serializable]
    public enum ViewModeType
    {
        /// <summary>
        /// Default mode
        /// </summary>
        Default,
        /// <summary>
        /// Adding new
        /// </summary>
        Add,
        /// <summary>
        /// Edit mode
        /// </summary>
        Edit,
        /// <summary>
        /// View only mode
        /// </summary>
        ViewOnly,
        /// <summary>
        /// Busy mode
        /// </summary>
        Busy
    }
}
