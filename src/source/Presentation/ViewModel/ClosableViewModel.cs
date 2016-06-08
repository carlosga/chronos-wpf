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
        #region · NotifyPropertyChanged Cached Instances ·

        private static readonly PropertyChangedEventArgs IdChangedArgs      = CreateArgs<ClosableViewModel>(x => x.Id);
        private static readonly PropertyChangedEventArgs TitleChangedArgs   = CreateArgs<ClosableViewModel>(x => x.Title);

        #endregion

        #region · Logger ·

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region · Fields ·

        private Guid                    id;
        private ActionCommand           closeCommand;
        private string                  title;
        private List<IChannelObserver>  observers;

        #endregion

        #region · Commands ·

        /// <summary>
        /// Gets the Close Command
        /// </summary>
        public ActionCommand CloseCommand
        {
            get
            {
                if (this.closeCommand == null)
                {
                    this.closeCommand = new ActionCommand
                    (
                        () => Close(),
                        () => CanClose()
                    );
                }

                return this.closeCommand;
            }
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>The id.</value>
        public Guid Id
        {
            get { return this.id; }
            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    this.NotifyPropertyChanged(IdChangedArgs);
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
            get { return this.title; }
            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    this.NotifyPropertyChanged(TitleChangedArgs);
                }
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosableViewModel"/> class.
        /// </summary>
        protected ClosableViewModel()
            : base()
        {
            this.Id         = Guid.NewGuid();
            this.observers  = new List<IChannelObserver>();
        }

        #endregion

        #region · Messaging Methods ·

        protected void Subscribe<T>(Action<T> payloadHandler)
            where T: class
        {
            this.Subscribe<T>(payloadHandler, ThreadOption.BackgroundThread);
        }

        protected void Subscribe<T>(Action<T> payloadHandler, ThreadOption threadOption)
            where T: class
        {
            ChannelObserver<T> observer = new ChannelObserver<T>((l) => payloadHandler(l));

            observer.Subscribe(threadOption);

            this.observers.Add(observer);
        }

        protected void Publish<T>(T message)
            where T: class
        {
            Channel<T>.Publish(message, true);
        }

        protected void Publish<T>(T message, bool async)
            where T: class
        {
            Channel<T>.Publish(message, async);
        }

        #endregion

        #region · Command Actions ·

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
            if (this.observers != null)
            {
                this.observers.ForEach(o => o.Unsubscribe());
                this.observers.Clear();
                this.observers = null;
            }

            this.GetService<IVirtualDesktopManager>().Close(this.Id);

            this.id             = Guid.Empty;
            this.title          = null;
            this.closeCommand   = null;
        }

        #endregion
    }
}
