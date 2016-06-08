// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows.Threading;

namespace Chronos.Extensions.Windows
{
    /// <summary>
    /// Provides a set of commonly used Dispatcher extension methods
    /// </summary>
    public static class DispatcherObjectExtensions
    {
        #region · Extensions ·

        /// <summary>
        /// Executes the specified <see cref="Action "/> at the <see cref="DispatcherPriority.ApplicationIdle"/> priority 
        /// on the thread on which the DispatcherObject is associated with. 
        /// </summary>
        /// <param name="dispatcherObject">The dispatcher object.</param>
        /// <param name="action">The action.</param>
        public static void InvokeAsynchronously(this DispatcherObject dispatcherObject, Action action)
        {
            dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }

        /// <summary>
        /// Executes the specified <see cref="Action "/> at the <see cref="DispatcherPriority.ApplicationIdle"/> priority 
        /// on the thread on which the DispatcherObject is associated with. 
        /// </summary>
        /// <param name="dispatcherObject">The dispatcher object.</param>
        /// <param name="action">The action.</param>
        public static void InvokeAsynchronouslyInBackground(this DispatcherObject dispatcherObject, Action action)
        {
            dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }

        /// <summary>
        /// Executes the specified <see cref="Action "/> at the <see cref="DispatcherPriority.ApplicationIdle"/> priority 
        /// on the thread on which the DispatcherObject is associated with. 
        /// </summary>
        /// <param name="dispatcherObject">The dispatcher object.</param>
        /// <param name="action">The action.</param>
        public static void Invoke(this DispatcherObject dispatcherObject, Action action)
        {
            dispatcherObject.Dispatcher.Invoke(action);
        }

        #endregion
    }
}