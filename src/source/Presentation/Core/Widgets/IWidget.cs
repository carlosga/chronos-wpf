// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;

namespace Chronos.Presentation.Core.Widgets
{
    /// <summary>
    /// Interface for widget definition implementations
    /// </summary>
    public interface IWidget
    {
        #region · Properties ·

        /// <summary>
        /// Gets the widget title
        /// </summary>
        string Title
        {
            get;
        }

        /// <summary>
        /// Gets the widget description
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Gets the widget group
        /// </summary>
        string Group
        {
            get;
        }

        /// <summary>
        /// Gets the widget icon style
        /// </summary>
        string IconStyle
        {
            get;
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Creates the widget view
        /// </summary>
        /// <returns></returns>
        FrameworkElement CreateView();

        #endregion
    }
}
