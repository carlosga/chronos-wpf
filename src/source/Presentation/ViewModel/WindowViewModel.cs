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
        : NavigationViewModel, IWindowViewModel where TEntity : class, new()
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

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

            public InquiryActionResult()
            {
            }
        }

        private static readonly PropertyChangedEventArgs s_viewModeChangedArgs = CreateArgs<WindowViewModel<TEntity>>(x => x.ViewMode);
        private static readonly PropertyChangedEventArgs s_statusMessageChangedArgs = CreateArgs<WindowViewModel<TEntity>>(x => x.StatusMessage);
        private static readonly PropertyChangedEventArgs s_notificationMessageChangedArgs = CreateArgs<WindowViewModel<TEntity>>(x => x.NotificationMessage);

        /// <summary>
        /// Occurs when view mode is changed.
        /// </summary>
        public event EventHandler ViewModeChanged;

        private ActionCommand _inquiryCommand;
        private TEntity _originalEntity;
        private string _statusMessage;
        private string _notificationMessage;
        private TEntity _entity;
        private ViewModeType _viewMode;
        private PropertyStateCollection<TEntity> _propertyStates;

        /// <summary>
        /// Gets the property state collection
        /// </summary>
        public PropertyStateCollection<TEntity> PropertyStates
        {
            get
            {
                if (_propertyStates == null)
                {
                    _propertyStates = new PropertyStateCollection<TEntity>();
                }

                return _propertyStates;
            }
        }

        /// <summary>
        /// Gets or sets the state of the smart part.
        /// </summary>
        /// <value>The state of the smart part.</value>
        public ViewModeType ViewMode
        {
            get { return _viewMode; }
            set
            {
                if (_viewMode != value)
                {
                    _viewMode = value;
                    try
                    {
                        OnViewModeChanged();
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
                if (_inquiryCommand == null)
                {
                    _inquiryCommand = new ActionCommand
                    (
                        () => OnInquiryData(),
                        () => CanInquiryData()
                    );
                }

                return _inquiryCommand;
            }
        }

        /// <summary>
        /// Gets or sets the status message text
        /// </summary>
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    NotifyPropertyChanged(s_statusMessageChangedArgs);
                }
            }
        }

        /// <summary>
        /// Gets or sets the notification message text
        /// </summary>
        public string NotificationMessage
        {
            get { return _notificationMessage; }
            set
            {
                if (_notificationMessage != value)
                {
                    _notificationMessage = value;
                    this.NotifyPropertyChanged(s_notificationMessageChangedArgs);
                }
            }
        }

        /// <summary>
        /// Gets or sets the data model.
        /// </summary>
        /// <value>The data model.</value>
        protected TEntity Entity
        {
            get { return _entity; }
            private set
            {
                if (!Object.ReferenceEquals(_entity, value))
                {
                    _entity = value;
                }
            }
        }

        protected TEntity OriginalEntity
        {
            get { return _originalEntity; }
            set { _originalEntity = value; }
        }

        protected WindowViewModel()
            : base()
        {
            this.InitializePropertyStates();

            this.Entity = new TEntity();
            this.ViewMode = ViewModeType.ViewOnly;
        }

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

            this.ViewMode = ViewModeType.Busy;
            this.StatusMessage = "Obteniendo datos, espere por favor ...";

            Task task = Task.Factory.StartNew
            (
                (o) =>
                {
                    s_logger.Debug("Obteniendo datos '{0}'", this.Entity.ToString());

                    this.OnInquiryAction(result);
                }, result
            );

            task.ContinueWith
            (
                (t) =>
                {
                    s_logger.Debug("Error al obtener datos '{0}'", t.Exception.InnerException.ToString());

                    this.OnInquiryActionFailed();

                    Exception exception = null;
                    this.OriginalEntity = null;

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
                    s_logger.Debug("Datos obtenidos correctamente '{0}'", this.Entity.ToString());

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

                    this.OriginalEntity = _entity;
                    this.ViewMode = ViewModeType.ViewOnly;
                    break;
            }
        }

        protected virtual void OnInquiryActionFailed()
        {
            this.ViewMode = ViewModeType.ViewOnly;
        }

        /// <summary>
        /// Called when the related view is being closed.
        /// </summary>
        public override void Close()
        {
            s_logger.Debug("Cerrar ventana '{0}'", this.GetType());

            if (_originalEntity != null)
            {
                _originalEntity = null;
            }
            if (_entity != null)
            {
                _entity = null;
            }
            if (_propertyStates != null)
            {
                _propertyStates.Clear();
                _propertyStates = null;
            }

            _inquiryCommand = null;
            _statusMessage = null;
            _notificationMessage = null;

            base.Close();
        }

        protected void ResetDataModel()
        {
            s_logger.Debug("Resetar model '{0}'", this.GetType());
            this.ResetDataModel(new TEntity());
            s_logger.Debug("Model reseteado '{0}'", this.GetType());
        }

        protected virtual void ResetDataModel(TEntity model)
        {
            this.OnResetingDataModel(this.Entity, model);

            this.OriginalEntity = null;
            this.Entity = model;

            this.OnResetedDataModel(model);
        }

        protected virtual void OnResetingDataModel(TEntity oldModel, TEntity newModel)
        {
        }

        protected virtual void OnResetedDataModel(TEntity newModel)
        {
            this.NotifyAllPropertiesChanged();
        }

        protected virtual void InitializePropertyStates()
        {
        }

        protected virtual void OnViewModeChanged()
        {
            if (this.ViewModeChanged != null)
            {
                this.ViewModeChanged(this, new EventArgs());
            }

            this.NotifyPropertyChanged(s_viewModeChangedArgs);
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
            if (args != s_statusMessageChangedArgs)
            {
                this.StatusMessage = null;
            }
            if (args != s_notificationMessageChangedArgs)
            {
                this.NotificationMessage = null;
            }

            this.UpdateAllowedUserActions();
            base.NotifyPropertyChanged(args);
        }
    }
}
