// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using System.Windows.Threading;
using Chronos.Authentication;
using Chronos.Modules.Navigation;
using Chronos.Presentation.Core.Navigation;
using Chronos.Presentation.Core.ViewModel;
using Chronos.Presentation.Core.VirtualDesktops;
using Chronos.Presentation.ViewModel;
using Chronos.Presentation.Windows;
using Chronos.Presentation.Windows.Navigation;
using Chronos.WidgetLibrary;
using NLog;
using nRoute.Components;
using nRoute.Components.Messaging;
using nRoute.Navigation;

namespace Chronos.ViewModel
{
    /// <summary>
    /// Shell Window ViewModel
    /// </summary>
    public sealed class ShellViewModel
        : ViewModelBase
    {
        #region · Logger ·

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region · Fields ·

        private WindowState _windowState;
        private string _userName;

        #region · Commands ·

        private ICommand _maximizeCommand;
        private ICommand _minimizeCommand;
        private ICommand _showWidgetLibraryCommand;
        private ICommand _shutdownCommand;
        private ICommand _closeSessionCommand;
        private ICommand _switchDesktopCommand;
        private ICommand _showDesktopCommand;
        private ICommand _saveCurrentDesktopCommand;
        private ICommand _saveAllDesktopsCommand;
        private ICommand _showAboutBoxCommand;

        #endregion

        #region · Observers ·

        private ChannelObserver<AuthenticationInfo> _authenticationObserver;
        private ChannelObserver<ActiveDesktopChangedInfo> _activeDesktopObserver;
        private ChannelObserver<NavigatedInfo> _navigatedObserver;

        #endregion

        #endregion

        #region · Commands ·

        /// <summary>
        /// Gets the maximize command.
        /// </summary>
        /// <value>The maximize command.</value>
        public ICommand MaximizeCommand
        {
            get
            {
                if (_maximizeCommand == null)
                {
                    _maximizeCommand = new ActionCommand(() => OnMaximizeWindow());
                }

                return _maximizeCommand;
            }
        }

        /// <summary>
        /// Gets the minimize command.
        /// </summary>
        /// <value>The minimize command.</value>
        public ICommand MinimizeCommand
        {
            get
            {
                if (_minimizeCommand == null)
                {
                    _minimizeCommand = new ActionCommand(() => OnMinimizeWindow());
                }

                return _minimizeCommand;
            }
        }

        /// <summary>
        /// Gets the switch desktop command
        /// </summary>
        public ICommand SwitchDesktopCommand
        {
            get
            {
                if (_switchDesktopCommand == null)
                {
                    _switchDesktopCommand = new ActionCommand(() => OnSwitchDesktop());
                }

                return _switchDesktopCommand;
            }
        }

        /// <summary>
        /// Gets the show desktop command
        /// </summary>
        public ICommand ShowDesktopCommand
        {
            get
            {
                if (_showDesktopCommand == null)
                {
                    _showDesktopCommand = new ActionCommand(() => OnShowDesktop());
                }

                return _showDesktopCommand;
            }
        }

        /// <summary>
        /// Gets the save current desktop command.
        /// </summary>
        /// <value>The save desktop command.</value>
        public ICommand SaveCurrentDesktopCommand
        {
            get
            {
                if (_saveCurrentDesktopCommand == null)
                {
                    _saveCurrentDesktopCommand = new ActionCommand(() => OnSaveCurrentDesktop());
                }

                return _saveCurrentDesktopCommand;
            }
        }

        /// <summary>
        /// Gets the save all desktops command.
        /// </summary>
        /// <value>The save desktop command.</value>
        public ICommand SaveAllDesktopsCommand
        {
            get
            {
                if (_saveAllDesktopsCommand == null)
                {
                    _saveAllDesktopsCommand = new ActionCommand(() => OnSaveAllDesktops());
                }

                return _saveAllDesktopsCommand;
            }
        }

        /// <summary>
        /// Gets the show widget library command.
        /// </summary>
        /// <value>The show widget library command.</value>
        public ICommand ShowWidgetLibraryCommand
        {
            get
            {
                if (_showWidgetLibraryCommand == null)
                {
                    _showWidgetLibraryCommand = new ActionCommand(() => OnShowWidgetLibrary());
                }

                return _showWidgetLibraryCommand;
            }
        }

        /// <summary>
        /// Gets the about box command.
        /// </summary>
        /// <value>The about box command.</value>
        public ICommand ShowAboutBoxCommand
        {
            get
            {
                if (_showAboutBoxCommand == null)
                {
                    _showAboutBoxCommand = new ActionCommand(() => OnShowAboutBoxCommand());
                }

                return _showAboutBoxCommand;
            }
        }

        /// <summary>
        /// Gets the shutdown command
        /// </summary>
        public ICommand ShutdownCommand
        {
            get
            {
                if (_shutdownCommand == null)
                {
                    _shutdownCommand = new ActionCommand(() => OnShutdown());
                }

                return _shutdownCommand;
            }
        }

        /// <summary>
        /// Gets the log off command
        /// </summary>
        public ICommand CloseSessionCommand
        {
            get
            {
                if (_closeSessionCommand == null)
                {
                    _closeSessionCommand = new ActionCommand(() => OnCloseSession());
                }

                return _closeSessionCommand;
            }
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        /// <value>The state of the window.</value>
        public WindowState WindowState
        {
            get { return _windowState; }
            set
            {
                if (_windowState != value)
                {
                    _windowState = value;
                    this.NotifyPropertyChanged(() => WindowState);
                }
            }
        }

        /// <summary>
        /// Gets the active windows.
        /// </summary>
        /// <value>The active windows.</value>
        public IList<INavigationViewModel> ActiveWindows
        {
            get
            {
                if (this.GetService<IVirtualDesktopManager>().HasDesktopActive)
                {
                    return this.GetService<IVirtualDesktopManager>().ActiveDesktopWindows;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the logged in user name
        /// </summary>
        public string UserName
        {
            get { return (!String.IsNullOrEmpty(_userName) ? _userName : "Sesi\u00F3n no iniciada"); }
            private set
            {
                _userName = value;
                this.NotifyPropertyChanged(() => UserName);
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        public ShellViewModel()
            : base()
        {
            this.InitializeObservers();
        }

        #endregion

        #region · Command Actions ·

        /// <summary>
        /// Handles the show widget library command action
        /// </summary>
        private void OnShowWidgetLibrary()
        {
            this.GetService<IVirtualDesktopManager>().Show<WidgetLibraryView>();
        }

        /// <summary>
        /// Handles the switch desktop command action
        /// </summary>
        private void OnSwitchDesktop()
        {
            this.GetService<IVirtualDesktopManager>().SwitchDesktop();
        }

        /// <summary>
        /// Handles the show desktop command
        /// </summary>
        private void OnShowDesktop()
        {
            this.GetService<IVirtualDesktopManager>().ShowDesktop();
        }

        /// <summary>
        /// Handles the save current desktop command action
        /// </summary>
        private void OnSaveCurrentDesktop()
        {
            this.GetService<IVirtualDesktopManager>().SaveCurrentDesktop();
        }

        /// <summary>
        /// Handles the save all desktops command action
        /// </summary>
        private void OnSaveAllDesktops()
        {
            this.GetService<IVirtualDesktopManager>().SaveAllDesktops();
        }

        /// <summary>
        /// Handles the shutdown command action
        /// </summary>
        private void OnShutdown()
        {
            s_logger.Debug("Finalizando la sesi\u00F3n");
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Closes the session
        /// </summary>
        private void OnCloseSession()
        {
            Channel<AuthenticationInfo>.Public.OnNext(
                new AuthenticationInfo { Action = AuthenticationAction.LogOut }, true);
        }

        /// <summary>
        /// Maximizes the window.
        /// </summary>
        private void OnMaximizeWindow()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }

            s_logger.Debug("Cambiado el estado de la ventana principal de la aplicaci\u00F3n ({0})", this.WindowState);
        }

        /// <summary>
        /// Handles the minimize window command action
        /// </summary>
        private void OnMinimizeWindow()
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OnShowAboutBoxCommand()
        {
            this.GetService<INavigationService>()
                .Navigate(NavigateMode.Modal, NavigationRoutes.About);
        }

        #endregion

        #region · Observer Initialization ·

        private void InitializeObservers()
        {
            s_logger.Debug("Inicializando observers de nroute");

            // Authentication Observer
            _authenticationObserver = new ChannelObserver<AuthenticationInfo>(
                (l) => OnAuthenticationAction(l));

            // Subscribe on the UI Thread
            _authenticationObserver.Subscribe(ThreadOption.BackgroundThread);

            // Active desktop changed observer
            _activeDesktopObserver = new ChannelObserver<ActiveDesktopChangedInfo>(
                (l) => OnActiveDesktopChanged(l));

            // Subscribe on the UI Thread
            _activeDesktopObserver.Subscribe(ThreadOption.UIThread);

            // Navigation observers

            // Navigated observer
            _navigatedObserver = new ChannelObserver<NavigatedInfo>(
                (l) => OnNavigated(l));

            _navigatedObserver.Subscribe(ThreadOption.BackgroundThread);
        }

        #endregion

        #region · Observer Actions ·

        private void OnAuthenticationAction(AuthenticationInfo info)
        {
            switch (info.Action)
            {
                case AuthenticationAction.LogOn:
                    break;

                case AuthenticationAction.LoggedIn:
                    this.Invoke(() => { this.UserName = info.UserId; });
                    break;

                case AuthenticationAction.LogOut:
                    this.Invoke(() => { this.UserName = info.UserId; });
                    break;
            }
        }

        private void OnActiveDesktopChanged(ActiveDesktopChangedInfo info)
        {
            s_logger.Debug("Cambiando el escritorio activo");

            this.NotifyPropertyChanged(() => ActiveWindows);
        }

        private void OnNavigated(NavigatedInfo info)
        {
            s_logger.Debug("Navegaci\u00F3n finalizada correctamente ({0})", info.Request.RequestUrl);

            this.CreateRecentNavigationEntry(info);
        }

        #endregion

        #region · Windows 7 Taskbar ·

        private void CreateRecentNavigationEntry(NavigatedInfo value)
        {
            if (value.Request.NavigationMode == NavigateMode.New)
            {
                this.Dispatcher.BeginInvoke(
                    (Action)delegate
                    {
                        if (App.RunningOnWin7)
                        {
                            JumpList jl = JumpList.GetJumpList(Application.Current);

                            if (jl != null)
                            {
                                if (jl.JumpItems.Count >= 10)
                                {
                                    jl.JumpItems.Clear();
                                }

                                var q = jl.JumpItems.OfType<JumpTask>().Where(t => t.Arguments.Equals(value.Request.RequestUrl));

                                if (q.Count() == 0)
                                {
                                    jl.JumpItems.Add
                                    (
                                        new JumpTask
                                        {
                                            CustomCategory = "Recent",
                                            Title = value.Title,
                                            Arguments = value.Request.RequestUrl,
                                            IconResourcePath = null,
                                            IconResourceIndex = -1,
                                            Description = null,
                                            WorkingDirectory =
                                                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                        }
                                    );

                                    jl.Apply();
                                }
                            }
                        }
                    }, DispatcherPriority.Background);
            }
        }

        #endregion
    }
}