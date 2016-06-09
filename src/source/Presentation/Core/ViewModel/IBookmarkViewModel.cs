// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using nRoute.Components;

namespace Chronos.Presentation.Core.ViewModel
{
    /// <summary>
    /// Interface for ViewModels that supports bookmarking
    /// </summary>
    public interface IBookmarkViewModel
    {
        /// <summary>
        /// Gets the bookmark current command
        /// </summary>
        ActionCommand BookmarkCurrentCommand
        {
            get;
        }

        /// <summary>
        /// Gets the clear bookmarks command
        /// </summary>
        ActionCommand ClearBookmarksCommand
        {
            get;
        }

        /// <summary>
        /// Gets the organize bookmarks command
        /// </summary>
        ActionCommand OrganizeBookmarksCommand
        {
            get;
        }

        /// <summary>
        /// Gets the create shortcut command
        /// </summary>
        ActionCommand CreateShortcutCommand
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating if there are available bookmarks
        /// </summary>
        bool HasBookMarks
        {
            get;
        }
    }
}
