// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Chronos.Presentation.Core.Windows;

namespace Chronos.Presentation.Core.Services
{
    /// <summary>
    /// Interface for modal messages view services
    /// </summary>
    public interface IShowMessageViewService
    {
        #region · Properties ·

        /// <summary>
        /// Gets or sets the message caption
        /// </summary>
        string Caption
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message text
        /// </summary>
        string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message buttons
        /// </summary>
        DialogButton ButtonSetup
        {
            get;
            set;
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Shows a new message window
        /// </summary>
        /// <returns></returns>
        DialogResult ShowMessage();

        #endregion
    }
}
