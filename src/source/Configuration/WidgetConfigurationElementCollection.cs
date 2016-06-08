// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Configuration;

namespace Chronos.Configuration
{
    /// <summary>
    /// Represents a <see cref="WidgetConfigurationElement"/> collection
    /// </summary>
    public sealed class WidgetConfigurationElementCollection
        : GenericElementCollection<string, WidgetConfigurationElement>
    {
        #region · Properties ·

        /// <summary>
        /// Gets the type of the collection.
        /// </summary>
        /// <value>The type of the collection.</value>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <value>The name of the element.</value>
        protected override string ElementName
        {
            get { return "widget"; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationElementCollection"/> class.
        /// </summary>
        public WidgetConfigurationElementCollection()
            : base()
        {
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Gets the element key.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        protected override string GetElementKey(WidgetConfigurationElement element)
        {
            return element.Id;
        }

        #endregion
    }
}
