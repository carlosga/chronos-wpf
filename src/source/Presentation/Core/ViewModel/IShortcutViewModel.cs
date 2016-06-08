// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using nRoute.Components;

namespace Chronos.Presentation.Core.ViewModel
{
    /// <summary>
    /// Interface for shortcut viewmodel implementations
    /// </summary>
    public interface IShortcutViewModel 
        : IClosableViewModel
    {
        #region · Commands ·

        /// <summary>
        /// Gets the open command
        /// </summary>
        ActionCommand OpenCommand
        {
            get;
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets or sets the shortcut target
        /// </summary>
        string Target
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the shortcut navigation parameters
        /// </summary>
        string Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the shortcut icon style
        /// </summary>
        string IconStyle
        {
            get;
            set;
        }

        #endregion
    }
}
