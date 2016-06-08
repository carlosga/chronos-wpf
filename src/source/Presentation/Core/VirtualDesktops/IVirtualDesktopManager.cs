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
    /// Interface for virtual desktop manager implementations
    /// </summary>
    public interface IVirtualDesktopManager
    {
        #region · Properties ·

        /// <summary>
        /// Gets the active desktop windows.
        /// </summary>
        /// <value>The active desktop windows.</value>
        ReadOnlyObservableCollection<INavigationViewModel> ActiveDesktopWindows
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether there is an active virtual desktop.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if there are an active virtual desktop; otherwise, <c>false</c>.
        /// </value>
        bool HasDesktopActive
        {
            get;
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Activates the default desktop
        /// </summary>
        void ActivateDefaultDesktop();

        /// <summary>
        /// Switches the active desktop
        /// </summary>
        void SwitchDesktop();

        /// <summary>
        /// Shows the desktop
        /// </summary>
        void ShowDesktop();

        /// <summary>
        /// Saves all the desktops to disk
        /// </summary>
        void SaveAllDesktops();

        /// <summary>
        /// Saves the active desktop to disk.
        /// </summary>
        void SaveCurrentDesktop();

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
        /// Shows a new instance of the specified type in the active desktop.
        /// </summary>
        void Show<T>() where T : IDesktopElement, new();

        /// <summary>
        /// Shows a new instance of the specified type in the active desktop.
        /// </summary>
        /// <param name="position">The position.</param>
        void Show<T>(Point position) where T : IDesktopElement, new();

        /// <summary>
        /// Shows the given <see cref="IDesktopElement"/>
        /// </summary>
        /// <param name="instance"></param>
        void Show(IDesktopElement instance);

        /// <summary>
        /// Shows the given <see cref="IClosableViewModel"/>
        /// </summary>
        /// <param name="instance"></param>
        void Show(IDesktopElement instance, Point position);

        /// <summary>
        /// Shows the given <see cref="IWindow"/> as a normal Window
        /// </summary>
        /// <param name="window"></param>
        void Show(IWindow element);

        /// <summary>
        /// Shows the given <see cref="IModalVindow"/> instance
        /// </summary>
        /// <param name="window"></param>
        DialogResult ShowDialog(IModalVindow element);

        /// <summary>
        /// Closes the object with the given identifier
        /// </summary>
        /// <param name="id">The element identifier.</param>
        void Close(Guid id);

        /// <summary>
        /// Closes all the elements
        /// </summary>
        void CloseAll();

        /// <summary>
        /// Closes the current dialog
        /// </summary>
        void CloseDialog();

        /// <summary>
        /// Restores the window with the given identifier
        /// </summary>
        /// <param name="id">The element identifier.</param>
        void Restore(Guid id);

        /// <summary>
        /// Registers the given desktop instance
        /// </summary>
        /// <param name="d"></param>
        void RegisterDesktop(DependencyObject d);

        /// <summary>
        /// Registers the given modal container instance
        /// </summary>
        /// <param name="d"></param>
        void RegisterModalContainer(DependencyObject d);

        #endregion
    }
}
