// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;
using System.Windows.Input;
using nRoute.Components;
using nRoute.SiteMaps;

namespace Chronos.Presentation.Windows.SiteMaps
{
    /// <summary>
    /// Command based sitemap node
    /// </summary>
    public sealed class CommandSiteMapNode 
        : NavigationNode
    {
        #region · Fields ·

        private ICommand executeCommand;

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the execute command.
        /// </summary>
        /// <value>The execute command.</value>
        [IgnoreDataMember]
        public ICommand ExecuteCommand
        {
            get
            {
                if (!this.HasChildNodes && !String.IsNullOrWhiteSpace(this.Url))
                {
                    this.executeCommand = new ActionCommand(() => Execute());
                }

                return this.executeCommand;
            }
        }

        #endregion

        #region · Constructors ·

        public CommandSiteMapNode()
            : base()
        {
        }

        #endregion

        #region · Overriden Methods ·

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Title;
        }

        #endregion
    }
}
