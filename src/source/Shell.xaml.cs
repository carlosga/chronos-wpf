// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Input;
using Chronos.Interop;
using Chronos.ViewModel;
using WinInterop = System.Windows.Interop;

namespace Chronos
{
    /// <summary>
    /// Shell Window
    /// </summary>
    public partial class Shell 
        : Window
    {
        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        public Shell()
        {
            InitializeComponent();
        }

        #endregion

        #region · Event Handlers ·

        protected override void  OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.SystemKey == Key.Space && e.Key == Key.System)
            {
                // Disable Window's ControlBox Menu
                e.Handled = true;
            }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr handle = (new WinInterop.WindowInteropHelper(this)).Handle;
            WinInterop.HwndSource.FromHwnd(handle).AddHook(new WinInterop.HwndSourceHook(Win32Interop.WindowProc));

            ShellViewModel vm = new ShellViewModel();
            vm.WindowState = System.Windows.WindowState.Maximized;

            this.DataContext = vm;
        }

        #endregion
    }
}
