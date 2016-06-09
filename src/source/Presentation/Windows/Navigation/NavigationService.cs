// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Chronos.Extensions;
using Chronos.Presentation.Core.Navigation;
using Chronos.Presentation.Core.Services;
using Chronos.Presentation.Core.VirtualDesktops;
using Chronos.Presentation.Core.Windows;
using Chronos.Presentation.Windows.Controls;
using nRoute.Components;
using nRoute.Components.Composition;
using nRoute.Components.Messaging;
using nRoute.Components.Routing;
using nRoute.Navigation;
using nRoute.Services;
using nRoute.SiteMaps;
using nRoute.ViewServices;

namespace Chronos.Presentation.Windows.Navigation
{
    /// <summary>
    /// Contains methods to support navigation.
    /// </summary>
    [MapService(typeof(INavigationService),
        InitializationMode = InitializationMode.OnDemand,
        Lifetime = InstanceLifetime.Singleton)]
    public sealed class NavigationService
        : INavigationService, INavigationHandler
    {
        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class
        /// </summary>
        public NavigationService()
        {
            // Set this as the default navigation handler
            nRoute.Navigation.NavigationService.DefaultNavigationHandler = this;
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Performs the navigation to the given target
        /// </summary>
        /// <param name="target"></param>
        public void Navigate(string target)
        {
            this.Navigate(NavigateMode.New, target, null);
        }

        /// <summary>
        /// Performs the navigation to the given target
        /// </summary>
        /// <param name="target"></param>
        public void Navigate(NavigateMode mode, string target)
        {
            this.Navigate(mode, target, null);
        }

        /// <summary>
        /// Performs the navigation to the given target
        /// </summary>
        /// <param name="target"></param>
        public void Navigate(string target, params object[] args)
        {
            this.Navigate(NavigateMode.New, target, args);
        }

        /// <summary>
        /// Performs the navigation to the given target
        /// </summary>
        /// <param name="target"></param>
        public void Navigate(NavigateMode mode, string target, params object[] args)
        {
            if (String.IsNullOrEmpty(target))
            {
                IShowMessageViewService showMessageService = ViewServiceLocator.GetViewService<IShowMessageViewService>();

                showMessageService.ButtonSetup = DialogButton.Ok;
                showMessageService.Caption = "Warning";
                showMessageService.Text = "Option not mapped to a view.";

                showMessageService.ShowMessage();

                return;
            }

            Application.Current.Dispatcher.BeginInvoke
            (
                DispatcherPriority.Background,
                new ThreadStart
                (
                    delegate
                    {
                        ParametersCollection navParams = null;
                        string area = null;

                        if (args != null && args.Length > 0)
                        {
                            navParams = new ParametersCollection();
                            navParams.Add(NavigationParams.NavigationParamsKey, args);
                        }

                        var smnode = SiteMapService.SiteMap.RootNode.ChildNodes
                            .OfType<NavigationNode>()
                            .Traverse<NavigationNode>(node => ((node.ChildNodes) != null ? node.ChildNodes.OfType<NavigationNode>() : null))
                            .Where(n => n.Url == target).SingleOrDefault();

                        if (smnode != null)
                        {
                            area = smnode.SiteArea;
                        }

                        NavigationRequest request = new NavigationRequest(target, navParams, area, mode);
                        IDisposable navigationToken = nRoute.Navigation.NavigationService.Navigate(request);

                        if (navigationToken != null)
                        {
                            navigationToken.Dispose();
                            navigationToken = null;
                        }
                    }
                )
            );
        }

        #endregion

        #region · INavigationHandler Members ·

        void INavigationHandler.ProcessRequest(NavigationRequest request, Action<bool> requestCallback)
        {
            Debug.Assert(request != null, "request");
            Debug.Assert(requestCallback != null, "requestCallback");

            NavigatingCancelInfo info = new NavigatingCancelInfo(request);

            if (!info.Cancel)
            {
                // change state
                Channel<NavigatingCancelInfo>.Public.OnNext(info);

                // Check if the navigation needs to be canceled
                if (!info.Cancel)
                {
                    requestCallback(true);
                }
                else
                {
                    requestCallback(false);
                }
            }
            else
            {
                requestCallback(false);
            }
        }

        /// <summary>
        /// Process the given <see cref="NavigationResponse"/> instance
        /// </summary>
        /// <param name="response"></param>
        void INavigationHandler.ProcessResponse(NavigationResponse response)
        {
            // basic check
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            // we check if the navigation was successfull or not
            if (response.Status != ResponseStatus.Success)
            {
                this.PublishNavigationFailedInfo(response.Request);

                IShowMessageViewService showMessageService = ViewServiceLocator.GetViewService<IShowMessageViewService>();

                showMessageService.ButtonSetup = DialogButton.Ok;
                showMessageService.Caption = "Chronos - Error en la navegaci\u00F3n";
                showMessageService.Text =
                    ((response.Error != null) ? response.Error.Message : String.Format("No ha sido posible resolver la navegaci\u00F3n solicitada ({0})", response.Request.RequestUrl));

                showMessageService.ShowMessage();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke
                (
                    DispatcherPriority.Background,
                    new ThreadStart
                    (
                        () =>
                        {
                            WindowElement window = response.Content as WindowElement;

                            if (response.ResponseParameters != null &&
                                response.ResponseParameters.Count > 0)
                            {
                                ISupportNavigationLifecycle supporter = nRoute.Navigation.NavigationService.GetSupporter<ISupportNavigationLifecycle>(response.Content);

                                if (supporter != null)
                                {
                                    supporter.Initialize(response.ResponseParameters);
                                }
                            }

                            this.PublishNavigatedInfo(window.Title, response);

                            if (response.Request.NavigationMode == NavigateMode.Modal)
                            {
                                ServiceLocator.GetService<IVirtualDesktopManager>().ShowDialog(window);
                            }
                            else
                            {
                                ServiceLocator.GetService<IVirtualDesktopManager>().Show(window);
                            }
                        }
                    )
                );
            }
        }

        #endregion

        #region · Request Validation ·

        /// <summary>
        /// Validates the given <see cref="NavigationRequest"/> instance
        /// </summary>
        /// <param name="request"></param>
        /// /// <returns>Returns as to if the request was validated or not, if not validated it woun't be processed.</returns>
        private bool ValidateRequest(NavigationRequest request)
        {
            // basic check
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            // by default
            return true;
        }

        #endregion

        #region · Private Methods ·

        private void OnNavigationCancelled()
        {
            Channel<NavigationFailedInfo>.Public.OnNext(new NavigationFailedInfo(null));
        }

        private void PublishNavigationFailedInfo(NavigationRequest request)
        {
            Channel<NavigationFailedInfo>.Public.OnNext(new NavigationFailedInfo(request));
        }

        private void PublishNavigatedInfo(string title, NavigationResponse response)
        {
            Channel<NavigatedInfo>.Public.OnNext
            (
                new NavigatedInfo
                (
                    response.Request,
                    title,
                    (response.Content as WindowElement).Id
                ),
                true
            );
        }

        #endregion
    }
}