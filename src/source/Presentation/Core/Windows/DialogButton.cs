// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Chronos.Presentation.Core.Windows
{
    /// <summary>
    /// Specifies the buttons that are displayed on a message window. 
    /// </summary>
    [Serializable]
    public enum DialogButton
    {
        /// <summary>
        /// The message box displays an OK button.
        /// </summary>
        Ok = 0,
        /// <summary>
        /// The message box displays OK and Cancel buttons.
        /// </summary>
        OkCancel = 1,
        /// <summary>
        /// The message box displays Yes and No buttons.
        /// </summary>
        YesNo = 4,
    }
}
