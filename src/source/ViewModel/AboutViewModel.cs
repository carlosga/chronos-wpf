// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Deployment.Application;
using System.Reflection;
using Chronos.Presentation.Core.VirtualDesktops;
using Chronos.Presentation.ViewModel;

namespace Chronos.ViewModel
{
    /// <summary>
    /// About window viewmodel
    /// </summary>
    public sealed class AboutViewModel
        : NavigationViewModel
    {
        #region · Properties ·

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public string Version
        {
            get
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }
                else
                {
                    return Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
            }
        }

        /// <summary>
        /// Gets the application product id.
        /// </summary>
        /// <value>The application product id.</value>
        public string ProductId
        {
            get { return Guid.NewGuid().ToString(); }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutViewModel"/> class.
        /// </summary>
        public AboutViewModel()
            : base()
        {
        }

        #endregion

        #region · Overriden Methods ·

        public override bool CanClose()
        {
            return true;
        }

        public override void Close()
        {
            this.GetService<IVirtualDesktopManager>().CloseDialog();
        }

        #endregion
    }
}
