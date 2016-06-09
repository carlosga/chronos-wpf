// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Chronos.Presentation.ViewModel;

namespace Chronos.Presentation.Widgets
{
    /// <summary>
    /// Calendar Widget View Model
    /// </summary>
    public sealed class CalendarWidgetViewModel
        : WidgetViewModel
    {
        /// <summary>
        /// Gets the current selected date
        /// </summary>
        public DateTime SelectedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarWidget"/> class
        /// </summary>
        public CalendarWidgetViewModel()
            : base()
        {
        }
    }
}

