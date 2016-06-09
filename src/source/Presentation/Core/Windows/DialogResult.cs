// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Chronos.Presentation.Core.Windows
{
    /// <summary>
    /// Specifies which dialog box button that a user clicks. 
    /// </summary>
    [Serializable]
    public enum DialogResult
    {
        /// <summary>
        /// The message box returns no result.
        /// </summary>
        None = 0,
        /// <summary>
        /// The result value of the message box is OK.
        /// </summary>
        Ok = 1,
        /// <summary>
        /// The result value of the message box is Cancel.
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// The result value of the message box is Yes.
        /// </summary>
        Yes = 6,
        /// <summary>
        /// The result value of the message box is No.
        /// </summary>
        No = 7,
    }
}
