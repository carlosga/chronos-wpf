// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Windows;
using Chronos.Presentation.Core.ViewModel;
using Chronos.Presentation.Core.Windows;

namespace Chronos.Presentation.Core.VirtualDesktops
{
    /// <summary>
    /// Interface for virtual desktop implementations
    /// </summary>
    public interface IVirtualDesktop
    {
        /// <summary>
        /// Gets the desktop identifier
        /// </summary>
        Guid Id
        {
            get;
        }

        /// <summary>
        /// Gets the list of active Windows
        /// </summary>
        ReadOnlyObservableCollection<INavigationViewModel> ActiveWindows
        {
            get;
        }

        /// <summary>
        /// Activates the desktop instance
        /// </summary>
        void Activate();

        /// <summary>
        /// Deactivates the desktop instance
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Shows the desktop
        /// </summary>
        void ShowDesktop();

        /// <summary>
        /// Saves the desktop to disk
        /// </summary>
        void Save();

        /// <summary>
        /// Creates a new shortcut with the given title and target
        /// </summary>
        /// <param name="title"></param>
        /// <param name="target"></param>
        void CreateShortcut<T>(string title, string target) where T : IShortcutViewModel, new();

        /// <summary>
        /// Creates a new shortcut with the given title, target and position
        /// </summary>
        /// <param name="title"></param>
        /// <param name="target"></param>
        void CreateShortcut<T>(string title, string target, Point position) where T : IShortcutViewModel, new();

        /// <summary>
        /// Shows a new instance of the given element type
        /// </summary>
        void Show<T>() where T : IDesktopElement, new();

        /// <summary>
        /// Shows a new instance of the given element type
        /// </summary>
        void Show<T>(Point position) where T : IDesktopElement, new();

        /// <summary>
        /// Shows the given <see cref="IDesktopElement"/> instance
        /// </summary>
        /// <param name="instance"></param>
        void Show(IDesktopElement instance);

        /// <summary>
        /// Shows the given <see cref="IDesktopElement"/> instance
        /// </summary>
        /// <param name="instance"></param>
        void Show(IDesktopElement instance, Point position);

        /// <summary>
        /// Shows the given <see cref="IWindow"/> instance
        /// </summary>
        /// <param name="window">A <see cref="IWindow"/> instance</param>
        void Show(IWindow window);

        /// <summary>
        /// Closes the element with given identifier
        /// </summary>
        /// <param name="id"></param>
        void Close(Guid id);

        /// <summary>
        /// Closes all the elements
        /// </summary>
        /// <param name="id">The identifier.</param>
        void CloseAll();

        /// <summary>
        /// Restores the window with the given identifier
        /// </summary>
        /// <param name="id">The identifier.</param>
        void Restore(Guid id);
    }
}
