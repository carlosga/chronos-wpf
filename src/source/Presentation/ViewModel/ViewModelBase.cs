// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using nRoute.Services;
using nRoute.ViewServices;

namespace Chronos.Presentation.ViewModel
{
    /// <summary>
    /// Provides a base class for Applications to inherit from. 
    /// </summary>
    public abstract class ViewModelBase 
        : ObservableObject
    {
        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        protected ViewModelBase()
            : base()
        {
        }

        #endregion

        #region · Protected Methods ·

        /// <summary>
        /// Gets the requested service instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        protected TService GetService<TService>() where TService : class
        {
            return ServiceLocator.GetService<TService>();
        }

        /// <summary>
        /// Gets the requested view service.
        /// </summary>
        /// <typeparam name="TViewService">The type of the view service.</typeparam>
        /// <returns></returns>
        protected TViewService GetViewService<TViewService>() where TViewService : class
        {
            return ViewServiceLocator.GetViewService<TViewService>();
        }

        #endregion
    }
}
