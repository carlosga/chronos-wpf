using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Chronos.Authentication;
using Chronos.Extensions.Windows;
using Chronos.Presentation.Core.Navigation;
using NLog;
using nRoute.Components.Messaging;
using nRoute.Services;
using System.Windows.Media;

namespace Chronos
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
        : nRoute.ApplicationServices.Application
    {
        #region · Logger ·

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region · Static Members ·

        /// <summary>
        /// Determines if the application is running on Windows 7
        /// </summary>
        public static bool RunningOnWin7
        {
            get
            {
                return (Environment.OSVersion.Version.Major > 6) ||
                    (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1);
            }
        }

        #endregion

        #region · Fields ·

        private ChannelObserver<AuthenticationInfo> authenticationObserver;
        private List<string>                        pendingNavigations;
        private string[]                            args;
        private bool                                isLoggedIn;

        #endregion

        #region · Methods ·

        public void Run(string[] arguments)
        {
            this.args = arguments;

            this.Run();
        }

        #endregion

        #region · Overriden Methods ·

        protected override void OnStartup(StartupEventArgs e)
        {
            //RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;

            this.DispatcherUnhandledException       += new DispatcherUnhandledExceptionEventHandler(OnDispatcherUnhandledException);
            SingleInstance.SingleInstanceActivated  += new EventHandler<SingleInstanceEventArgs>(OnSingleInstanceActivated);

            // Pending Navigations
            this.pendingNavigations = new List<string>();

            // Authentication Observer
            this.authenticationObserver = new ChannelObserver<AuthenticationInfo>((l) => OnAuthenticationAction(l));

            // Subscribe on background thread
            this.authenticationObserver.Subscribe(ThreadOption.BackgroundThread);
            
            base.OnStartup(e);

            if (this.args != null && this.args.Length > 0)
            {
                if (this.isLoggedIn)
                {
                    ServiceLocator.GetService<INavigationService>().Navigate(e.Args[0]);
                }
                else
                {
                    this.pendingNavigations.Add(e.Args[0]);
                }
            }

            RenderTier renderTier = this.GetRenderTier();
        }

        #endregion

        #region · Observer Actions ·

        private void OnAuthenticationAction(AuthenticationInfo info)
        {
            switch (info.Action)
            {
                case AuthenticationAction.LogOn:
                    this.isLoggedIn = false;
                    break;

                case AuthenticationAction.LoggedIn:
                    this.isLoggedIn = true;

                    if (this.pendingNavigations != null &&
                        this.pendingNavigations.Count > 0)
                    {
                        this.ProcessPendingNavigations();
                    }
                    break;

                case AuthenticationAction.LogOut:
                    this.isLoggedIn = false;
                    break;
            }
        }

        #endregion

        #region · Private Methods ·

        private void ProcessPendingNavigations()
        {
            foreach (string url in this.pendingNavigations)
            {
                ServiceLocator.GetService<INavigationService>().Navigate(url);
            }
        }

        #endregion

        #region · Event Handlers ·

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.ErrorException("Excepción no manejada", e.Exception);

            e.Handled = true;
        }

        private void OnSingleInstanceActivated(object sender, SingleInstanceEventArgs e)
        {
            if (e != null && e.Args != null && e.Args.Count > 1)
            {
                ServiceLocator.GetService<INavigationService>().Navigate(e.Args[1]);
            }
        }

        #endregion
    }
}
