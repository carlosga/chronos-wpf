// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Chronos.Presentation.Core.Windows;
using nRoute.Components;

namespace Chronos.Presentation.Core.ViewModel
{
    /// <summary>
    /// Interface for workspace viewmodel implementations
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IWorkspaceViewModel<TEntity>
        : IWindowViewModel, IEntityViewModel<TEntity>, INavigationViewModel, IBookmarkViewModel where TEntity : class, new()
    {
        /// <summary>
        /// Occurs when the view mode has changed
        /// </summary>
        event EventHandler ViewModeChanged;

        /// <summary>
        /// Gets the add new command
        /// </summary>
        ActionCommand AddNewCommand
        {
            get;
        }

        /// <summary>
        /// Gets the delete command
        /// </summary>
        ActionCommand DeleteCommand
        {
            get;
        }

        /// <summary>
        /// Gets the discard changes command
        /// </summary>
        ActionCommand DiscardCommand
        {
            get;
        }

        /// <summary>
        /// Gets the edit command
        /// </summary>
        ActionCommand EditCommand
        {
            get;
        }

        /// <summary>
        /// Gets the save changes command
        /// </summary>
        ActionCommand SaveCommand
        {
            get;
        }

        /// <summary>
        /// Gets the print command
        /// </summary>
        ActionCommand PrintCommand
        {
            get;
        }

        /// <summary>
        /// Gets the print preview command
        /// </summary>
        ActionCommand PrintPreviewCommand
        {
            get;
        }

        /// <summary>
        /// Gets the show form help command
        /// </summary>
        ActionCommand ShowFormHelpCommand
        {
            get;
        }

        /// <summary>
        /// Gets the show zoom window command
        /// </summary>
        ActionCommand ShowZoomWindowCommand
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the zoom window is shown
        /// </summary>
        bool ShowZoomWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the zoom level
        /// </summary>
        double ZoomLevel
        {
            get;
            set;
        }
    }
}
