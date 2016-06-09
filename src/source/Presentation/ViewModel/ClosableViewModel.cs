// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Chronos.Presentation.Core.ViewModel;
using Chronos.Presentation.Core.VirtualDesktops;
using NLog;
using nRoute.Components;
using nRoute.Components.Messaging;

namespace Chronos.Presentation.ViewModel
{
    /// <summary>
    /// Base class for closeable view models
    /// </summary>
    public abstract class ClosableViewModel
        : ViewModelBase, IClosableViewModel
    {
        private static readonly PropertyChangedEventArgs s_idChangedArgs = CreateArgs<ClosableViewModel>(x => x.Id);
        private static readonly PropertyChangedEventArgs s_titleChangedArgs = CreateArgs<ClosableViewModel>(x => x.Title);

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private Guid _id;
        private ActionCommand _closeCommand;
        private string _title;
        private List<IChannelObserver> _observers;

        /// <summary>
        /// Gets the Close Command
        /// </summary>
        public ActionCommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new ActionCommand
                    (
                        () => Close(),
                        () => CanClose()
                    );
                }

                return _closeCommand;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>The id.</value>
        public Guid Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    this.NotifyPropertyChanged(s_idChangedArgs);
                }
            }
        }

        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public virtual string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    this.NotifyPropertyChanged(s_titleChangedArgs);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosableViewModel"/> class.
        /// </summary>
        protected ClosableViewModel()
            : base()
        {
            this.Id = Guid.NewGuid();
            _observers = new List<IChannelObserver>();
        }

        protected void Subscribe<T>(Action<T> payloadHandler)
            where T : class
        {
            this.Subscribe<T>(payloadHandler, ThreadOption.BackgroundThread);
        }

        protected void Subscribe<T>(Action<T> payloadHandler, ThreadOption threadOption)
            where T : class
        {
            ChannelObserver<T> observer = new ChannelObserver<T>((l) => payloadHandler(l));

            observer.Subscribe(threadOption);

            _observers.Add(observer);
        }

        protected void Publish<T>(T message)
            where T : class
        {
            Channel<T>.Publish(message, true);
        }

        protected void Publish<T>(T message, bool async)
            where T : class
        {
            Channel<T>.Publish(message, async);
        }

        /// <summary>
        /// Determines whether the view related to this view model can be closed.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the related view can be closed; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanClose()
        {
            return true;
        }

        /// <summary>
        /// Called when the related view is being closed.
        /// </summary>
        public virtual void Close()
        {
            if (_observers != null)
            {
                _observers.ForEach(o => o.Unsubscribe());
                _observers.Clear();
                _observers = null;
            }

            this.GetService<IVirtualDesktopManager>().Close(this.Id);

            _id = Guid.Empty;
            _title = null;
            _closeCommand = null;
        }
    }
}
