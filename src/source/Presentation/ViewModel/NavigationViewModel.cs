﻿// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Chronos.Presentation.Core.Navigation;
using Chronos.Presentation.Core.ViewModel;
using Chronos.Presentation.Core.VirtualDesktops;
using NLog;
using nRoute.Components;
using nRoute.Navigation;

namespace Chronos.Presentation.ViewModel
{
    /// <summary>
    /// base class for navigation based viewmodel iumplementations
    /// </summary>
    public abstract class NavigationViewModel 
        : ClosableViewModel, INavigationViewModel
    {
        #region · Logger ·

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region · Fields ·

        #region · Commands ·

        private ActionCommand newWindowCommand;
        private ActionCommand restoreCommand;

        #endregion

        #endregion

        #region · INavigationViewModel Commands ·

        /// <summary>
        /// Gets the open new window command
        /// </summary>
        public ActionCommand NewWindowCommand
        {
            get
            {
                if (this.newWindowCommand == null)
                {
                    this.newWindowCommand = new ActionCommand
                    (
                        () => OnOpenNewWindow(),
                        () => CanOpenNewWindow()
                    );
                }

                return this.newWindowCommand;
            }
        }

        /// <summary>
        /// Gets the restore command
        /// </summary>
        public ActionCommand RestoreCommand
        {
            get
            {
                if (this.restoreCommand == null)
                {
                    this.restoreCommand = new ActionCommand
                    (
                        () => OnRestore(),
                        () => CanRestore()
                    );
                }

                return this.restoreCommand;
            }
        }

        #endregion

        #region · INavigationViewModel Properties ·

        /// <summary>
        /// Gets the navigation route
        /// </summary>
        public virtual string NavigationRoute
        {
            get { return String.Empty; }
        }

        /// <summary>
        /// Gets a value indicating if the viewmodel has available relations
        /// </summary>
        public virtual bool HasRelations
        {
            get { return false; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationViewModel"/> class.
        /// </summary>
        protected NavigationViewModel()
            : base()
        {
        }

        #endregion

        #region · Methods ·

        public override void Close()
        {
            this.newWindowCommand   = null;
            this.restoreCommand     = null;

            base.Close();
        }

        #endregion

        #region · Navigation Methods ·

        /// <summary>
        /// Performs the navigation to the given target
        /// </summary>
        /// <param name="target"></param>
        protected void Navigate(string target)
        {
            this.Navigate(NavigateMode.New, target, null);
        }

        /// <summary>
        /// Performs the navigation to the given target
        /// </summary>
        /// <param name="target"></param>
        protected void Navigate(NavigateMode mode, string target)
        {
            this.Navigate(mode, target, null);
        }

        /// <summary>
        /// Performs the navigation to the given target
        /// </summary>
        /// <param name="target"></param>
        protected void Navigate(string target, params object[] args)
        {
            this.Navigate(NavigateMode.New, target, args);
        }

        /// <summary>
        /// Performs the navigation to the given target
        /// </summary>
        /// <param name="target"></param>
        protected void Navigate(NavigateMode mode, string target, params object[] args)
        {
            this.GetService<INavigationService>().Navigate(mode, target, args);
        }

        #endregion

        #region · ISupportNavigationLifecycle Members ·

        /// <summary>
        /// Closings the specified confirm callback.
        /// </summary>
        /// <param name="confirmCallback">The confirm callback.</param>
        void ISupportNavigationLifecycle.Closing(System.Action<bool> confirmCallback)
        {
            this.OnClosing(confirmCallback);
        }

        /// <summary>
        /// Initializes the specified request parameters.
        /// </summary>
        /// <param name="requestParameters">The request parameters.</param>
        void ISupportNavigationLifecycle.Initialize(ParametersCollection requestParameters)
        {
            this.OnInitialize(requestParameters);
        }

        #endregion

        #region · ISupportNavigationState Members ·

        /// <summary>
        /// Restores the state.
        /// </summary>
        /// <param name="state">The state.</param>
        void ISupportNavigationState.RestoreState(ParametersCollection state)
        {
            this.OnRestoreState(state);
        }

        /// <summary>
        /// Saves the state.
        /// </summary>
        /// <returns></returns>
        ParametersCollection ISupportNavigationState.SaveState()
        {
            return this.OnSaveState();
        }

        #endregion

        #region · Command Actions ·

        protected virtual bool CanOpenNewWindow()
        {
            return !String.IsNullOrEmpty(this.NavigationRoute);
        }

        protected virtual void OnOpenNewWindow()
        {
            Logger.Debug("Abrir una nueva ventana '{0}'", this.GetType());

            this.GetService<INavigationService>().Navigate(this.NavigationRoute);
        }

        protected virtual bool CanRestore()
        {
            return true;
        }

        protected virtual void OnRestore()
        {
            Logger.Debug("Restaurar ventana '{0}'", this.GetType());

            this.GetService<IVirtualDesktopManager>().Restore(this.Id);
        }

        #endregion

        #region · Protected Methods ·
        
        /// <summary>
        /// Called when closing.
        /// </summary>
        /// <param name="confirmCallback">The confirm callback.</param>
        protected virtual void OnClosing(Action<bool> confirmCallback)
        {
            confirmCallback(false);
        }

        /// <summary>
        /// Called when the window is going to be initialized after navigation.
        /// </summary>
        /// <param name="requestParameters">The request parameters.</param>
        protected virtual void OnInitialize(ParametersCollection requestParameters)
        {
        }

        /// <summary>
        /// Called when restore state.
        /// </summary>
        /// <param name="state">The state.</param>
        protected virtual void OnRestoreState(ParametersCollection state)
        {
        }

        /// <summary>
        /// Called when save state.
        /// </summary>
        /// <returns></returns>
        protected virtual ParametersCollection OnSaveState()
        {
            return null;
        }

        #endregion
    }
}
