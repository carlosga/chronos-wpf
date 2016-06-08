﻿// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Chronos.Presentation.Core.Widgets;

namespace Chronos.Presentation.Widgets
{
    /// <summary>
    /// Calendar widget definition
    /// </summary>
    public sealed class CalendarWidget
        : IWidget
    {
        #region · Properties ·

        /// <summary>
        /// Gets the widget title
        /// </summary>
        /// <value></value>
        public string Title
        {
            get { return "Calendar"; }
        }

        /// <summary>
        /// Gets the widget description
        /// </summary>
        /// <value></value>
        public string Description
        {
            get { return "Calendar"; }
        }

        /// <summary>
        /// Gets the widget group
        /// </summary>
        /// <value></value>
        public string Group
        {
            get { return "Tools"; }
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
            return new CalendarWidgetView();
        }

        #endregion
    }
}
