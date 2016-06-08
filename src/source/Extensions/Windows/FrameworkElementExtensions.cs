// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Chronos.Extensions.Windows
{
    public static class FrameworkElementExtensions
    {
        /// <summary>
        /// Set focus to UIElement
        /// </summary>
        /// <param name="element">The element to set focus on</param>
        public static void SetFocus(this FrameworkElement element)
        {
            //Focus in a callback to run on another thread, ensuring the main 
            //UI thread is initialized by the time focus is set
            ThreadPool.QueueUserWorkItem
            (
                delegate(object theElement)
                {
                    FrameworkElement elem = (FrameworkElement)theElement;

                    elem.InvokeAsynchronouslyInBackground
                    (
                        () =>
                        {
                            bool result = elem.Focus();
                            Keyboard.Focus(elem);
                        }
                    );
                }, element
            );
        }

        public static void MoveFocus(this IInputElement element, FocusNavigationDirection direction)
        {
            //Focus in a callback to run on another thread, ensuring the main 
            //UI thread is initialized by the time focus is set
            ThreadPool.QueueUserWorkItem
            (
                delegate(object theElement)
                {
                    FrameworkElement elem = (FrameworkElement)theElement;

                    elem.InvokeAsynchronouslyInBackground
                    (
                        () =>
                        {
                            elem.Focus();
                            Keyboard.Focus(elem);
                            elem.MoveFocus(new TraversalRequest(direction));
                        }
                    );
                }, element
            );
        }
    }
}
