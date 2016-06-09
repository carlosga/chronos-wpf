// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Chronos.Presentation.Core.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWindow
        : IDesktopElement
    {
        /// <summary>
        /// Occurs when the window state has changed
        /// </summary>
        event EventHandler WindowStateChanged;

        /// <summary>
        /// Occurs when the window is about to be closed
        /// </summary>
        event CancelEventHandler Closing;

        /// <summary>
        /// Occurs when the window is closed
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Gets the close window command
        /// </summary>
        ICommand CloseCommand
        {
            get;
        }

        /// <summary>
        /// Gets the maximize window command
        /// </summary>
        ICommand MaximizeCommand
        {
            get;
        }

        /// <summary>
        /// Gets the minimize window command
        /// </summary>
        ICommand MinimizeCommand
        {
            get;
        }

        /// <summary>
        /// Gets or sets the window title
        /// </summary>
        string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the maximize button is shown
        /// </summary>
        bool ShowMaximizeButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the minimize button is shown
        /// </summary>
        bool ShowMinimizeButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the close button is shown
        /// </summary>
        bool ShowCloseButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current <see cref="Chronos.Presentation.Core.Windows.ViewModeType"/>
        /// </summary>
        ViewModeType ViewMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Chronos.Presentation.Core.Windows.WindowState"/>
        /// </summary>
        WindowState WindowState
        {
            get;
            set;
        }

        /// <summary>
        /// Shows the window
        /// </summary>
        void Show();

        /// <summary>
        /// Closes the window
        /// </summary>
        void Close();

        /// <summary>
        /// Hides the window
        /// </summary>
        void Hide();
    }
}
