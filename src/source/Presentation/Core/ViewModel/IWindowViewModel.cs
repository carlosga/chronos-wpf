// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Chronos.Presentation.Core.Windows;
using nRoute.Components;

namespace Chronos.Presentation.Core.ViewModel
{
    public interface IWindowViewModel
        : INavigationViewModel
    {
        /// <summary>
        /// Gets the inquiry data command
        /// </summary>
        ActionCommand InquiryCommand
        {
            get;
        }

        /// <summary>
        /// Gets or sets the <see cref="Chronos.Presentation.Core.Windows.ViewModeType"/>
        /// </summary>
        ViewModeType ViewMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the status message text
        /// </summary>
        string StatusMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the notification message text
        /// </summary>
        string NotificationMessage
        {
            get;
            set;
        }
    }
}
