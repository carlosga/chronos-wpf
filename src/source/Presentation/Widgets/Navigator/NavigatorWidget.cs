// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Chronos.Presentation.Core.Widgets;

namespace Chronos.Presentation.Widgets
{
    /// <summary>
    /// Navigator Widget Definition
    /// </summary>
    public sealed class NavigatorWidget
        : IWidget
    {
        #region · Properties ·

        /// <summary>
        /// Gets the widget title
        /// </summary>
        /// <value></value>
        public string Title
        {
            get { return "Navigator"; }
        }

        /// <summary>
        /// Gets the widget description
        /// </summary>
        /// <value></value>
        public string Description
        {
            get { return "Browse and search application functions"; }
        }

        /// <summary>
        /// Gets the widget group
        /// </summary>
        /// <value></value>
        public string Group
        {
            get { return "System"; }
        }

        /// <summary>
        /// Gets the widget icon style
        /// </summary>
        /// <value></value>
        public string IconStyle
        {
            get { return String.Empty; }
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Creates the widget view
        /// </summary>
        /// <returns></returns>
        public System.Windows.FrameworkElement CreateView()
        {
            return new NavigatorWidgetView();
        }

        #endregion
    }
}
