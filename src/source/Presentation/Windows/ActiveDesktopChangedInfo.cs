// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Chronos.Presentation.Core.VirtualDesktops;

namespace Chronos.Presentation.Windows
{
    /// <summary>
    /// Actived desktop changed payload
    /// </summary>
    public sealed class ActiveDesktopChangedInfo
    {
        private readonly IVirtualDesktop _newActiveDesktop;

        /// <summary>
        /// Gets the new active desktop.
        /// </summary>
        /// <value>The new active desktop.</value>
        public IVirtualDesktop NewActiveDesktop
        {
            get { return _newActiveDesktop; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDesktopChangedInfo"/> class.
        /// </summary>
        /// <param name="newActiveDesktop">The new active desktop.</param>
        public ActiveDesktopChangedInfo(IVirtualDesktop newActiveDesktop)
        {
            _newActiveDesktop = newActiveDesktop;
        }
    }
}
