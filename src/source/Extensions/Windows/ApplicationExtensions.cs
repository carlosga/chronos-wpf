// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Permissions;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;

namespace Chronos.Extensions.Windows
{
    /// <summary>
    /// Application extensions
    /// </summary>
    public static class ApplicationExtensions
    {
        public static RenderTier GetRenderTier(this Application application)
        {
            return (RenderTier)(RenderCapability.Tier / 0x10000);
        }

        /// <summary>
        /// Forces the WPF message pump to process all enqueued messages
        /// that are above the input parameter DispatcherPriority.
        /// </summary>
        /// <param name="priority">The DispatcherPriority to use
        /// as the lowest level of messages to get processed</param>
        [SecurityPermissionAttribute(SecurityAction.Demand,
            Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents(this Application application, DispatcherPriority priority)
        {
            DispatcherFrame frame = new DispatcherFrame();
            DispatcherOperation dispatcherOperation =
                Dispatcher.CurrentDispatcher.BeginInvoke(priority,
                    new DispatcherOperationCallback(ExitFrameOperation), frame);

            Dispatcher.PushFrame(frame);

            if (dispatcherOperation.Status != DispatcherOperationStatus.Completed)
            {
                dispatcherOperation.Abort();
            }
        }

        /// <summary>
        /// Forces the WPF message pump to process all enqueued messages
        /// that are DispatcherPriority.Background or above
        /// </summary>
        [SecurityPermissionAttribute(SecurityAction.Demand,
            Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents(this Application application)
        {
            application.DoEvents(DispatcherPriority.Background);
        }

        /// <summary>
        /// Stops the dispatcher from continuing
        /// </summary>
        private static object ExitFrameOperation(object obj)
        {
            ((DispatcherFrame)obj).Continue = false;

            return null;
        }
    }
}
