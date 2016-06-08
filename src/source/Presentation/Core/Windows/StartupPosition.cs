// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Chronos.Presentation.Core.Windows
{
    /// <summary>
    /// Specifies the position that a <see cref="Chronos.Presentation.Core.Windows.IDesktopElement"/> 
    /// will be shown in when it is first opened. 
    /// </summary>
    [Serializable]
    public enum StartupPosition
    {
        /// <summary>
        /// The startup location of a <see cref="Chronos.Presentation.Core.Windows.IDesktopElement"/> 
        /// is the center of the parent control that owns it.
        /// </summary>
        CenterParent,
        /// <summary>
        /// The startup location of a <see cref="Chronos.Presentation.Core.Windows.IDesktopElement"/> 
        /// is set from code, or defers to the default Windows location.
        /// </summary>
        Manual,
        /// <summary>
        /// The startup location of a <see cref="Chronos.Presentation.Core.Windows.IDesktopElement"/> 
        /// is the center of the screen that contains the mouse cursor.
        /// </summary>
        WindowsDefaultLocation
    }
}
