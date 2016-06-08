// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using Chronos.Presentation.Core.ViewModel;

namespace Chronos.Presentation.ViewModel
{
    /// <summary>
    /// Base class for widgets viewmodel imeplementations
    /// </summary>
    public abstract class WidgetViewModel 
        : ClosableViewModel, IWidgetViewModel
    {
        #region · NotifyPropertyChanged Cached Instances ·

        private static readonly PropertyChangedEventArgs DescriptionChangedArgs = CreateArgs<WidgetViewModel>(x => x.Description);

        #endregion

        #region · Fields ·

        private string description;

        #endregion

        #region · Properties ·

        /// <summary>
        /// Returns the widget description.
        /// </summary>
        public virtual string Description
        {
            get { return this.description; }
            set
            {
                if (this.description != value)
                {
                    this.description = value;
                    this.NotifyPropertyChanged(DescriptionChangedArgs);
                }
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetViewModel"/> class.
        /// </summary>
        protected WidgetViewModel()
            : base()
        {
        }

        #endregion

        #region · Command Actions ·

        /// <summary>
        /// Called when the related view is being closed.
        /// </summary>
        public override void Close()
        {
            base.Close();
            
            this.description = null;
        }

        #endregion
    }
}
