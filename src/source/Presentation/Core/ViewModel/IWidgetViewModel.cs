// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Chronos.Presentation.Core.ViewModel
{
    /// <summary>
    /// Interface for widget viewmodel implementations
    /// </summary>
    public interface IWidgetViewModel 
        : IClosableViewModel
    {
        #region · Properties ·

        /// <summary>
        /// Gets the widget description
        /// </summary>
        string Description
        {
            get;
        }

        #endregion
    }
}
