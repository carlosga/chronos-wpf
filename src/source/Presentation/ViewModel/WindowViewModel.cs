// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Chronos.Presentation.Core.ViewModel;
using Chronos.Presentation.Core.Windows;
using NLog;
using nRoute.Components;

namespace Chronos.Presentation.ViewModel
{
    public abstract class WindowViewModel<TEntity>
        : NavigationViewModel, IWindowViewModel where TEntity: class, new()
    {
        #region · Logger ·

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region · Inner Types ·

        [Serializable]
        protected enum InquiryActionResultType
        {
            DataFetched,
            RequestedNew,
            DataNotFound
        }

        protected sealed class InquiryActionResult<TResult>
            where TResult : class
        {
            #region · Properties ·

            public TResult Data
            {
                get;
                set;
            }

            public InquiryActionResultType Result
            {
                get;
                set;
            }

            #endregion

            #region · Constructors ·

            public InquiryActionResult()
            {
            }

            #endregion
        }

        #endregion

        #region · PropertyChangedEventArgs Cached Instances ·

        private static readonly PropertyChangedEventArgs ViewModeChangedArgs            = CreateArgs<WindowViewModel<TEntity>>(x => x.ViewMode);
        private static readonly PropertyChangedEventArgs StatusMessageChangedArgs       = CreateArgs<WindowViewModel<TEntity>>(x => x.StatusMessage);
        private static readonly PropertyChangedEventArgs NotificationMessageChangedArgs = CreateArgs<WindowViewModel<TEntity>>(x => x.NotificationMessage);

        #endregion

        #region · Events ·

        /// <summary>
        /// Occurs when view mode is changed.
        /// </summary>
        public event EventHandler ViewModeChanged;

        #endregion

        #region · Fields ·

        private TEntity                             originalEntity;
        private string                              statusMessage;
        private string                              notificationMessage;
        private TEntity                             entity;
        private ViewModeType                        viewMode;
        private PropertyStateCollection<TEntity>    propertyStates;

        #region · Commands ·

        private ActionCommand inquiryCommand;

        #endregion
        
        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the property state collection
        /// </summary>
        public PropertyStateCollection<TEntity> PropertyStates
        {
            get
            {
                if (this.propertyStates == null)
                {
                    this.propertyStates = new PropertyStateCollection<TEntity>();
                }

                return this.propertyStates;
            }
        }

        /// <summary>
        /// Gets or sets the state of the smart part.
        /// </summary>
        /// <value>The state of the smart part.</value>
        public ViewModeType ViewMode
        {
            get { return this.viewMode; }
            set
            {
                if (this.viewMode != value)
                {
                    this.viewMode = value;
                    try
                    {
                        this.OnViewModeChanged();
                    }
                    catch
                    {
#warning TODO: This is done to prevent DataGrid erroros when discarding changes on new records
                    }
                }
            }
        }

        /// <summary>
        /// Gets the inquiry data command
        /// </summary>
        public ActionCommand InquiryCommand
        {
            get
            {
                if (this.inquiryCommand == null)
                {
                    this.inquiryCommand = new ActionCommand
                    (
                        () => OnInquiryData(),
                        () => CanInquiryData()
                    );
                }

                return this.inquiryCommand;
            }
        }

        /// <summary>
        /// Gets or sets the status message text
        /// </summary>
        public string StatusMessage
        {
            get { return this.statusMessage; }
            set
            {
                if (this.statusMessage != value)
                {
                    this.statusMessage = value;
                    this.NotifyPropertyChanged(StatusMessageChangedArgs);
                }
            }
        }

        /// <summary>
        /// Gets or sets the notification message text
        /// </summary>
        public string NotificationMessage
        {
            get { return this.notificationMessage; }
            set
            {
                if (this.notificationMessage != value)
                {
                    this.notificationMessage = value;
                    this.NotifyPropertyChanged(NotificationMessageChangedArgs);
                }
            }
        }

        #endregion

        #region · Protected Properties ·

        /// <summary>
        /// Gets or sets the data model.
        /// </summary>
        /// <value>The data model.</value>
        protected TEntity Entity
        {
            get { return this.entity; }
            private set
            {
                if (!Object.ReferenceEquals(this.entity, value))
                {
                    this.entity = value;
                }
            }
        }

        protected TEntity OriginalEntity
        {
            get { return this.originalEntity; }
            set { this.originalEntity = value; }
        }

        #endregion

        #region · Constructors ·

        protected WindowViewModel()
            : base()
        {
            this.InitializePropertyStates();

            this.Entity     = new TEntity();
            this.ViewMode   = ViewModeType.ViewOnly;
        }

        #endregion

        #region · Command Actions ·

        #region · Inquiry ·

        protected virtual bool CanInquiryData()
        {
            return (this.ViewMode == ViewModeType.ViewOnly);
        }

        protected void OnInquiryData()
        {
            InquiryActionResult<TEntity> result = new InquiryActionResult<TEntity>
            {
                Result = InquiryActionResultType.DataNotFound
            };

            this.ViewMode       = ViewModeType.Busy;
            this.StatusMessage  = "Obteniendo datos, espere por favor ...";

            Task task = Task.Factory.StartNew
            (
                (o) =>
                {
                    Logger.Debug("Obteniendo datos '{0}'", this.Entity.ToString());

                    this.OnInquiryAction(result);
                }, result
            );

            task.ContinueWith
            (
                (t) =>
                {
                    Logger.Debug("Error al obtener datos '{0}'", t.Exception.InnerException.ToString());

                    this.OnInquiryActionFailed();

                    Exception exception     = null;
                    this.OriginalEntity     = null;

                    if (t.Exception.InnerException != null)
                    {
                        exception = t.Exception.InnerException;
                    }
                    else
                    {
                        exception = t.Exception;
                    }

                    this.NotificationMessage = exception.Message;
                }, 
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.AttachedToParent,
                TaskScheduler.FromCurrentSynchronizationContext()
            );

            task.ContinueWith
            (
                _ =>
                {
                    Logger.Debug("Datos obtenidos correctamente '{0}'", this.Entity.ToString());

                    this.StatusMessage = null;
                    this.OnInquiryActionComplete(result);
                }, 
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent,
                TaskScheduler.FromCurrentSynchronizationContext()
            );
        }

        protected abstract void OnInquiryAction(InquiryActionResult<TEntity> result);

        protected virtual void OnInquiryActionComplete(InquiryActionResult<TEntity> result)
        {
            switch (result.Result)
            {
                case InquiryActionResultType.DataNotFound:
                    this.OriginalEntity = null;

                    this.ResetDataModel();
                    
                    this.ViewMode = ViewModeType.ViewOnly;
                    break;

                case InquiryActionResultType.RequestedNew:
                    throw new InvalidOperationException("Requested New is not valid on this type of ViewModel");

                case InquiryActionResultType.DataFetched:
                    this.ResetDataModel(result.Data);

                    this.OriginalEntity = this.entity;
                    this.ViewMode       = ViewModeType.ViewOnly;
                    break;
            }
        }

        protected virtual void OnInquiryActionFailed()
        {
            this.ViewMode = ViewModeType.ViewOnly;
        }

        #endregion

        #endregion

        #region · Overriden Methods ·

        /// <summary>
        /// Called when the related view is being closed.
        /// </summary>
        public override void Close()
        {
            Logger.Debug("Cerrar ventana '{0}'", this.GetType());

            if (this.originalEntity != null)
            {
                this.originalEntity = null;
            }
            if (this.entity != null)
            {
                this.entity = null;
            }
            if (this.propertyStates != null)
            {
                this.propertyStates.Clear();
                this.propertyStates = null;
            }

            this.inquiryCommand         = null;
            this.statusMessage          = null;
            this.notificationMessage    = null;

            base.Close();
        }

        #endregion

        #region · DataModel Reset Methods ·

        protected void ResetDataModel()
        {
            Logger.Debug("Resetar model '{0}'", this.GetType());
            this.ResetDataModel(new TEntity());
            Logger.Debug("Model reseteado '{0}'", this.GetType());
        }

        protected virtual void ResetDataModel(TEntity model)
        {
            this.OnResetingDataModel(this.Entity, model);

            this.OriginalEntity = null;
            this.Entity         = model;

            this.OnResetedDataModel(model);
        }

        protected virtual void OnResetingDataModel(TEntity oldModel, TEntity newModel)
        {
        }

        protected virtual void OnResetedDataModel(TEntity newModel)
        {
            this.NotifyAllPropertiesChanged();
        }

        #endregion        

        #region · Protected Methods ·

        protected virtual void InitializePropertyStates()
        {
        }

        protected virtual void OnViewModeChanged()
        {
            if (this.ViewModeChanged != null)
            {
                this.ViewModeChanged(this, new EventArgs());            
            }

            this.NotifyPropertyChanged(ViewModeChangedArgs);
        }

        protected virtual void UpdateAllowedUserActions()
        {
            this.Invoke
            (
                () =>
                {
                    this.InquiryCommand.RequeryCanExecute();
                }
            );
        }

        protected override void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            if (args != StatusMessageChangedArgs)
            {
                this.StatusMessage = null;
            }
            if (args != NotificationMessageChangedArgs)
            {
                this.NotificationMessage = null;
            }

            this.UpdateAllowedUserActions();
            base.NotifyPropertyChanged(args);
        }

        #endregion
    }
}
