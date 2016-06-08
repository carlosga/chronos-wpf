// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Configuration;

namespace Chronos.Configuration
{
    /// <summary>
    /// Widget configuration section handler
    /// </summary>
    public sealed class WidgetsSectionHandler
        : ConfigurationSection
    {
        #region · Properties ·

        /// <summary>
        /// Gets the list of configured widgets.
        /// </summary>
        /// <value>The desktops.</value>
        [ConfigurationProperty("widgets", IsDefaultCollection = true)]
        public WidgetConfigurationElementCollection Widgets
        {
            get
            {
                return (WidgetConfigurationElementCollection)base["widgets"];
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetsSectionHandler"/> class.
        /// </summary>
        public WidgetsSectionHandler()
        {
        }

        #endregion
    }
}
