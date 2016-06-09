// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using Chronos.Presentation.Core.ViewModel;

namespace Chronos.Presentation.ViewModel
{
    /// <summary>
    /// Base class for entity viewmodel implementations
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityViewModel<TEntity>
        : ViewModelBase, IEntityViewModel<TEntity> where TEntity : class, IDataErrorInfo, new()
    {
        private TEntity _entity;

        /// <summary>
        /// Gets the entity model instance
        /// </summary>
        public TEntity Entity
        {
            get { return _entity; }
        }

        /// <summary>
        /// Gets value indicating whether this instance is valid
        /// </summary>
        public virtual bool IsValid
        {
            get { return String.IsNullOrEmpty(this.Error); }
        }

        /// <summary>
        /// Gets value indicating whether this instance has changes
        /// </summary>
        public virtual bool HasChanges
        {
            get { return false; }
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <value>
        /// An error message indicating what is wrong with this object. The default is
        /// an empty string ("").
        /// </value>
        public virtual string Error
        {
            get
            {
                if (this.Entity != null)
                {
                    return this.Entity.Error;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="columnName">The name of the property whose error message to get.</param>
        /// <value>The error message for the property. The default is an empty string ("").</value>
        public virtual string this[string columnName]
        {
            get
            {
                if (this.Entity != null)
                {
                    return this.Entity[columnName];
                }

                return null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityViewModel"/> class
        /// </summary>
        protected EntityViewModel()
            : base()
        {
            _entity = new TEntity();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityViewModel"/> class
        /// with the given entity model
        /// </summary>
        /// <param name="entity"></param>
        protected EntityViewModel(TEntity entity)
            : this()
        {
            _entity = entity;
        }
    }
}

