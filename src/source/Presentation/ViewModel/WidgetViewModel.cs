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
        private static readonly PropertyChangedEventArgs s_descriptionChangedArgs = CreateArgs<WidgetViewModel>(x => x.Description);

        private string _description;

        /// <summary>
        /// Returns the widget description.
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    this.NotifyPropertyChanged(s_descriptionChangedArgs);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetViewModel"/> class.
        /// </summary>
        protected WidgetViewModel()
            : base()
        {
        }

        /// <summary>
        /// Called when the related view is being closed.
        /// </summary>
        public override void Close()
        {
            base.Close();

            _description = null;
        }
    }
}
