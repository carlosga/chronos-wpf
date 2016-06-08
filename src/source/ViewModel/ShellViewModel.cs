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

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region · Fields ·

        private WindowState windowState;
        private string      userName;

        #region · Commands ·

        private ICommand maximizeCommand;
        private ICommand minimizeCommand;
        private ICommand showWidgetLibraryCommand;
        private ICommand shutdownCommand;
        private ICommand closeSessionCommand;
        private ICommand switchDesktopCommand;
        private ICommand showDesktopCommand;
        private ICommand saveCurrentDesktopCommand;
        private ICommand saveAllDesktopsCommand;
        private ICommand showAboutBoxCommand;

        #endregion

        #region · Observers ·

        private ChannelObserver<AuthenticationInfo>         authenticationObserver;
        private ChannelObserver<ActiveDesktopChangedInfo>   activeDesktopObserver;
        private ChannelObserver<NavigatedInfo>              navigatedObserver;

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
                if (this.maximizeCommand == null)
                {
                    this.maximizeCommand = new ActionCommand(() => OnMaximizeWindow());
                }

                return this.maximizeCommand;
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
                if (this.minimizeCommand == null)
                {
                    this.minimizeCommand = new ActionCommand(() => OnMinimizeWindow());
                }

                return this.minimizeCommand;
            }
        }

        /// <summary>
        /// Gets the switch desktop command
        /// </summary>
        public ICommand SwitchDesktopCommand
        {
            get
            {
                if (this.switchDesktopCommand == null)
                {
                    this.switchDesktopCommand = new ActionCommand(() => OnSwitchDesktop());
                }

                return this.switchDesktopCommand;
            }
        }

        /// <summary>
        /// Gets the show desktop command
        /// </summary>
        public ICommand ShowDesktopCommand
        {
            get
            {
                if (this.showDesktopCommand == null)
                {
                    this.showDesktopCommand = new ActionCommand(() => OnShowDesktop());
                }

                return this.showDesktopCommand;
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
                if (this.saveCurrentDesktopCommand == null)
                {
                    this.saveCurrentDesktopCommand = new ActionCommand(() => OnSaveCurrentDesktop());
                }

                return this.saveCurrentDesktopCommand;
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
                if (this.saveAllDesktopsCommand == null)
                {
                    this.saveAllDesktopsCommand = new ActionCommand(() => OnSaveAllDesktops());
                }

                return this.saveAllDesktopsCommand;
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
                if (this.showWidgetLibraryCommand == null)
                {
                    this.showWidgetLibraryCommand = new ActionCommand(() => OnShowWidgetLibrary());
                }

                return this.showWidgetLibraryCommand;
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
                if (this.showAboutBoxCommand == null)
                {
                    this.showAboutBoxCommand = new ActionCommand(() => OnShowAboutBoxCommand());
                }

                return this.showAboutBoxCommand;
            }
        }

        /// <summary>
        /// Gets the shutdown command
        /// </summary>
        public ICommand ShutdownCommand
        {
            get
            {
                if (this.shutdownCommand == null)
                {
                    this.shutdownCommand = new ActionCommand(() => OnShutdown());
                }

                return this.shutdownCommand;
            }
        }

        /// <summary>
        /// Gets the log off command
        /// </summary>
        public ICommand CloseSessionCommand
        {
            get
            {
                if (this.closeSessionCommand == null)
                {
                    this.closeSessionCommand = new ActionCommand(() => OnCloseSession());
                }

                return this.closeSessionCommand;
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
            get { return this.windowState; }
            set
            {
                if (this.windowState != value)
                {
                    this.windowState = value;
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
            get { return (!String.IsNullOrEmpty(this.userName) ? this.userName : "Sesión no iniciada"); }
            private set
            {
                this.userName = value;
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
            Logger.Debug("Finalizando la sesión");
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

            Logger.Debug("Cambiado el estado de la ventana principal de la aplicación ({0})", this.WindowState);
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
            Logger.Debug("Inicializando observers de nroute");

            // Authentication Observer
            this.authenticationObserver = new ChannelObserver<AuthenticationInfo>(
                (l) => OnAuthenticationAction(l));

            // Subscribe on the UI Thread
            this.authenticationObserver.Subscribe(ThreadOption.BackgroundThread);

            // Active desktop changed observer
            this.activeDesktopObserver = new ChannelObserver<ActiveDesktopChangedInfo>(
                (l) => OnActiveDesktopChanged(l));

            // Subscribe on the UI Thread
            this.activeDesktopObserver.Subscribe(ThreadOption.UIThread);

            // Navigation observers

            // Navigated observer
            this.navigatedObserver = new ChannelObserver<NavigatedInfo>(
                (l) => OnNavigated(l));

            this.navigatedObserver.Subscribe(ThreadOption.BackgroundThread);
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
            Logger.Debug("Cambiando el escritorio activo");

            this.NotifyPropertyChanged(() => ActiveWindows);
        }

        private void OnNavigated(NavigatedInfo info)
        {
            Logger.Debug("Navegación finalizada correctamente ({0})", info.Request.RequestUrl);

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
                                            CustomCategory      = "Recent",
                                            Title               = value.Title,
                                            Arguments           = value.Request.RequestUrl,
                                            IconResourcePath    = null,
                                            IconResourceIndex   = -1,
                                            Description         = null,
                                            WorkingDirectory    =
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