// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Configuration;

namespace Chronos.Configuration
{
    /// <summary>
    /// Widget configuration element
    /// </summary>
    public sealed class WidgetConfigurationElement
        : ConfigurationElement
    {
        #region · Properties ·

        /// <summary>
        /// Gets or sets the widget id.
        /// </summary>
        /// <value>The widget id.</value>
        [ConfigurationProperty("id", IsRequired = true, IsKey=true)]
        public string Id
        {
            get { return (string)this["id"]; }
            set { this["id"] = value; }
        }

        /// <summary>
        /// Gets or sets the widget handler type.
        /// </summary>
        /// <value>The widget handler type.</value>
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetConfigurationElement"/> class.
        /// </summary>
        public WidgetConfigurationElement()
        {
        }

        #endregion	
    }
}
