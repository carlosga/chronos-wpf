// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Configuration;

namespace Chronos.Configuration
{
    /// <summary>
    /// http://utahdnug.org/blogs/josh/archive/2007/08/21/generic-configurationelementcollection.aspx
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public abstract class GenericElementCollection<K, V>
        : ConfigurationElementCollection where V : ConfigurationElement, new()
    {
        /// <summary>
        /// Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Configuration.ConfigurationElementCollectionType"/> of this collection.
        /// </returns>
        public abstract override ConfigurationElementCollectionType CollectionType
        {
            get;
        }

        /// <summary>
        /// Gets the name used to identify this collection of elements in the configuration file when overridden in a derived class.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The name of the collection; otherwise, an empty string. The default is an empty string.
        /// </returns>
        protected abstract override string ElementName
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="ConfigurationElement"/> with the specified key.
        /// </summary>
        /// <value></value>
        public V this[K key]
        {
            get { return (V)BaseGet(key); }
        }

        /// <summary>
        /// Gets the <see cref="ConfigurationElement"/> at the specified index.
        /// </summary>
        /// <value></value>
        public V this[int index]
        {
            get { return (V)BaseGet(index); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericElementCollection&lt;K, V&gt;"/> class.
        /// </summary>
        public GenericElementCollection()
        {
        }

        /// <summary>
        /// Adds the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public void Add(V path)
        {
            BaseAdd(path);
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(K key)
        {
            BaseRemove(key);
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new V();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return GetElementKey((V)element);
        }

        /// <summary>
        /// Gets the element key.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        protected abstract K GetElementKey(V element);
    }
}