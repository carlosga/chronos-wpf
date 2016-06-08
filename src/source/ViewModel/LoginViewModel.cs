// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using Chronos.Authentication;
using Chronos.Model;
using Chronos.Presentation.Core.VirtualDesktops;
using Chronos.Presentation.Core.Windows;
using Chronos.Presentation.ViewModel;
using nRoute.Components.Messaging;
using nRoute.Services;

namespace Chronos.ViewModel
{
    /// <summary>
    /// Login view view model class
    /// </summary>
    public sealed class LoginViewModel
        : WindowViewModel<UserLogin>
    {
        #region · Data Properties ·

        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        public string UserId
        {
            get { return this.Entity.UserId; }
            set
            {
                if (this.Entity.UserId != value)
                {
                    this.Entity.UserId = value;
                    this.NotifyPropertyChanged(() => UserId);

                    this.InquiryCommand.RequeryCanExecute();
                }
            }
        }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password
        {
            get { return this.Entity.Password; }
            set
            {
                if (this.Entity.Password != value)
                {
                    this.Entity.Password = value;
                    this.NotifyPropertyChanged(() => Password);

                    this.InquiryCommand.RequeryCanExecute();
                }
            }
        }        

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class
        /// </summary>
        public LoginViewModel() 
            : base()
        {
        }

        #endregion

        #region · Overriden Methods ·

        public override bool CanClose()
        {
            return (this.ViewMode != ViewModeType.Busy);
        }

        public override void Close()
        {
            this.GetService<IVirtualDesktopManager>().CloseDialog();
            Application.Current.Shutdown();
        }

        protected override void InitializePropertyStates()
        {
            this.PropertyStates.Add(e => e.UserId);
            this.PropertyStates.Add(e => e.Password);
        }

        protected override void OnViewModeChanged()
        {
            base.OnViewModeChanged();

            if (this.PropertyStates.Count > 0)
            {
                this.PropertyStates[x => x.UserId].IsEditable   = (this.ViewMode != ViewModeType.Busy);
                this.PropertyStates[x => x.Password].IsEditable = (this.ViewMode != ViewModeType.Busy);
            }
        }

        #endregion

        #region · Command Actions ·

        protected override bool CanInquiryData()
        {
            return (!String.IsNullOrEmpty(this.UserId) &&
                    !String.IsNullOrEmpty(this.Password)    && 
                    this.ViewMode != ViewModeType.Busy);
        }

        protected override void OnInquiryAction(InquiryActionResult<UserLogin> result)
        {
            result.Data     = this.Entity;
            result.Result   = InquiryActionResultType.DataFetched;
        }

        protected override void OnInquiryActionComplete(InquiryActionResult<UserLogin> result)
        {
            if (result.Result == InquiryActionResultType.DataFetched)
            {
                Channel<AuthenticationInfo>.Public.OnNext(
                    new AuthenticationInfo
                    {
                        Action = AuthenticationAction.LoggedIn,
                        UserId = this.UserId
                    }, true);

                ServiceLocator.GetService<IVirtualDesktopManager>().CloseDialog();
            }
            else if (result.Result == InquiryActionResultType.DataNotFound)
            {
                this.NotificationMessage = "Username and password do not match.";

                this.ViewMode = ViewModeType.Default;
            }
        }

        #endregion
    }
}
