// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Chronos.Presentation.Core.Services;
using Chronos.Presentation.Core.Windows;
using Chronos.Presentation.Windows.Controls;
using nRoute.Components.Composition;
using nRoute.ViewServices;

namespace Chronos.Presentation.Windows.ViewServices
{
    /// <summary>
    /// Modal messages view service
    /// </summary>
    [MapViewService(typeof(IShowMessageViewService), ShowMessageViewService.SERVICE_NAME,
            InitializationMode = InitializationMode.OnDemand, Lifetime = ViewServiceLifetime.PerInstance)]
    public sealed class ShowMessageViewService
        : IShowMessageViewService
    {
        #region · Constants ·

        private const string SERVICE_NAME = "ChronosMessageBoxViewService";

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets or sets the message caption
        /// </summary>
        /// <value></value>
        public string Caption
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message text
        /// </summary>
        /// <value></value>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message buttons
        /// </summary>
        /// <value></value>
        public DialogButton ButtonSetup
        {
            get;
            set;
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowMessageViewService"/> class.
        /// </summary>
        public ShowMessageViewService()
        {
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Shows a new message window
        /// </summary>
        /// <returns></returns>
        public DialogResult ShowMessage()
        {
            return MessageWindowElement.Show(this.Caption ?? String.Empty, this.Text, this.ButtonSetup);
        }

        #endregion
    }
}
