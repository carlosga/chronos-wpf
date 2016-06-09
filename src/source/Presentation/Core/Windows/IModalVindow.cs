// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Chronos.Presentation.Core.Windows
{
    /// <summary>
    /// Interface for modal window implementations
    /// </summary>
    public interface IModalVindow
        : IWindow
    {
        /// <summary>
        /// Gets or sets the <see cref="Chronos.Presentation.Core.Windows.DialogResult"/>
        /// </summary>
        DialogResult DialogResult
        {
            get;
        }

        /// <summary>
        /// Shows the window as a modal dialog
        /// </summary>
        /// <returns></returns>
        DialogResult ShowDialog();
    }
}
