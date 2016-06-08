// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Chronos.Authentication;
using Chronos.Modules.Navigation;
using Chronos.Presentation.Core.Navigation;
using Chronos.Presentation.Core.VirtualDesktops;
using NLog;
using nRoute.Components.Composition;
using nRoute.Components.Messaging;
using nRoute.Navigation;
using nRoute.Services;

namespace Chronos
{
    /// <summary>
    /// Dedicated observer for authentication 
    /// </summary>
    [MapChannelObserver(typeof(AuthenticationInfo),
        InitializationMode = InitializationMode.WhenAvailable,
        Lifetime = InstanceLifetime.Singleton)]
    public sealed class AuthenticationObserver : IObserver<AuthenticationInfo>
    {
        #region · Logger ·

        private static Logger Logger = LogManager.GetCurrentClassLogger(); 

        #endregion

        #region · IObserver<AuthenticationInfo> Members ·

        /// <summary>
        /// Notifies the observer that the provider has finished sending push-based notifications.
        /// </summary>
        public void OnCompleted()
        {
        }

        /// <summary>
        /// Notifies the observer that the provider has experienced an error condition.
        /// </summary>
        /// <param name="error">An object that provides additional information about the error.</param>
        public void OnError(Exception error)
        {
        }

        /// <summary>
        ///  Provides the observer with new data.
        /// </summary>
        /// <param name="value">The current notification information.</param>
        public void OnNext(AuthenticationInfo value)
        {
            switch (value.Action)
            {
                case AuthenticationAction.LogOn:
                    Logger.Debug("Autenticación del usuario");
                    ServiceLocator.GetService<INavigationService>()
                                  .Navigate(NavigateMode.Modal, NavigationRoutes.Login);
                    break;

                case AuthenticationAction.LoggedIn:
                    Logger.Debug("Usuario autenticado correctamente");
                    ServiceLocator.GetService<IVirtualDesktopManager>()
                                  .ActivateDefaultDesktop();
                    break;

                case AuthenticationAction.LogOut:
                    Logger.Debug("Cerrando sesión");
                    Task t = Task.Factory.StartNew
                    (
                        () => 
                        {
                            ServiceLocator.GetService<IVirtualDesktopManager>()
                                          .CloseAll();

                            Channel<AuthenticationInfo>.Publish
                            (
                                new AuthenticationInfo 
                                { 
                                    Action = AuthenticationAction.LogOn 
                                }, 
                                true
                            );
                        }
                    );
                    break;
            }
        }

        #endregion
    }
}
