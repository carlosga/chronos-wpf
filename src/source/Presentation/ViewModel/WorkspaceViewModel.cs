// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Chronos.Presentation.Core.ViewModel;
using Chronos.Presentation.Core.VirtualDesktops;
using Chronos.Presentation.Core.Windows;
using NLog;
using nRoute.Components;

namespace Chronos.Presentation.ViewModel
{
    /// <summary>
    /// Base application class for workspace windows
    /// </summary>
    /// <typeparam name="TEntity">The type of the data model.</typeparam>
    public abstract class WorkspaceViewModel<TEntity> :
        WindowViewModel<TEntity>, IWorkspaceViewModel<TEntity> where TEntity : class, INotifyPropertyChanged, IDataErrorInfo, new()
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private static readonly PropertyChangedEventArgs s_hasBookmarksChangedArgs = CreateArgs<WorkspaceViewModel<TEntity>>(x => x.HasBookMarks);
        private static readonly PropertyChangedEventArgs s_hasRelationsChangedArgs = CreateArgs<WorkspaceViewModel<TEntity>>(x => x.HasRelations);
        private static readonly PropertyChangedEventArgs s_showZoomWindowChangedArgs = CreateArgs<WorkspaceViewModel<TEntity>>(x => x.ShowZoomWindow);
        private static readonly PropertyChangedEventArgs s_zoomLevelChangedArgs = CreateArgs<WorkspaceViewModel<TEntity>>(x => x.ZoomLevel);

        private bool _hasBookMarks;
        private bool _showZoomWindow;
        private double _zoomLevel;

        private ActionCommand _addNewCommand;
        private ActionCommand _editCommand;
        private ActionCommand _deleteCommand;
        private ActionCommand _saveCommand;
        private ActionCommand _discardCommand;
        private ActionCommand _printCommand;
        private ActionCommand _printPreviewCommand;
        private ActionCommand _createShortcutCommand;
        private ActionCommand _bookmarkCurrentCommand;
        private ActionCommand _clearBookmarksCommand;
        private ActionCommand _organizeBookmarksCommand;
        private ActionCommand _showFormHelpCommand;
        private ActionCommand _showZoomWindowCommand;

        /// <summary>
        /// Gets the add new command
        public ActionCommand AddNewCommand
        {
            get
            {
                if (_addNewCommand == null)
                {
                    _addNewCommand = new ActionCommand
                    (
                        () => OnAddNew(),
                        () => CanAddNew()
                    );
                }

                return _addNewCommand;
            }
        }

        /// <summary>
        /// Gets the edit command
        /// </summary>
        public ActionCommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                {
                    _editCommand = new ActionCommand
                    (
                        () => OnEdit(),
                        () => CanEdit()
                    );
                }

                return _editCommand;
            }
        }

        /// <summary>
        /// Gets the delete command
        /// </summary>
        public ActionCommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new ActionCommand
                    (
                        () => OnDelete(),
                        () => CanDelete()
                    );
                }

                return _deleteCommand;
            }
        }

        /// <summary>
        /// Gets the save command
        /// </summary>
        public ActionCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new ActionCommand
                    (
                        () => OnSave(),
                        () => CanSave()
                    );
                }

                return _saveCommand;
            }
        }

        /// <summary>
        /// Gets the discard command
        /// </summary>
        public ActionCommand DiscardCommand
        {
            get
            {
                if (_discardCommand == null)
                {
                    _discardCommand = new ActionCommand
                    (
                        () => OnDiscard(),
                        () => CanDiscard()
                    );
                }

                return _discardCommand;
            }
        }

        /// <summary>
        /// Gets the print command
        /// </summary>
        public ActionCommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                {
                    _printCommand = new ActionCommand
                    (
                        () => OnPrint(),
                        () => CanPrint()
                    );
                }

                return _printCommand;
            }
        }

        /// <summary>
        /// Gets the print preview command
        /// </summary>
        public ActionCommand PrintPreviewCommand
        {
            get
            {
                if (_printPreviewCommand == null)
                {
                    _printPreviewCommand = new ActionCommand
                    (
                        () => OnPrintPreview(),
                        () => CanPrintPreview()
                    );
                }

                return _printPreviewCommand;
            }
        }

        /// <summary>
        /// Gets the show form help command
        /// </summary>
        public ActionCommand ShowFormHelpCommand
        {
            get
            {
                if (_showFormHelpCommand == null)
                {
                    _showFormHelpCommand = new ActionCommand
                    (
                        () => OnShowFormHelp(),
                        () => CanShowFormHelp()
                    );
                }

                return _showFormHelpCommand;
            }
        }

        /// <summary>
        /// Gets the show zoom window command
        /// </summary>
        public ActionCommand ShowZoomWindowCommand
        {
            get
            {
                if (_showZoomWindowCommand == null)
                {
                    _showZoomWindowCommand = new ActionCommand
                    (
                        () => OnShowZoomWindow(),
                        () => CanShowZoomWindow()
                    );
                }

                return _showZoomWindowCommand;
            }
        }

        /// <summary>
        /// Gets the bookmark current command
        /// </summary>
        public ActionCommand BookmarkCurrentCommand
        {
            get
            {
                if (_bookmarkCurrentCommand == null)
                {
                    _bookmarkCurrentCommand = new ActionCommand
                    (
                        () => OnBookmarkCurrent(),
                        () => CanBookmarkCurrent()
                    );
                }

                return _bookmarkCurrentCommand;
            }
        }

        /// <summary>
        /// Gets the clear bookmarks command
        /// </summary>
        public ActionCommand ClearBookmarksCommand
        {
            get
            {
                if (_clearBookmarksCommand == null)
                {
                    _clearBookmarksCommand = new ActionCommand
                    (
                        () => OnClearBookmarks(),
                        () => CanClearBookmarks()
                    );
                }

                return _clearBookmarksCommand;
            }
        }

        /// <summary>
        /// Gets the organize bookmarks command
        /// </summary>
        public ActionCommand OrganizeBookmarksCommand
        {
            get
            {
                if (_organizeBookmarksCommand == null)
                {
                    _organizeBookmarksCommand = new ActionCommand
                    (
                        () => OnOrganizeBookmarks(),
                        () => CanOrganizeBookmarks()
                    );
                }

                return _organizeBookmarksCommand;
            }
        }

        /// <summary>
        /// Gets the create shortcut command
        /// </summary>
        public ActionCommand CreateShortcutCommand
        {
            get
            {
                if (_createShortcutCommand == null)
                {
                    _createShortcutCommand = new ActionCommand
                    (
                        () => OnCreateShortcut(),
                        () => CanCreateShortcut()
                    );
                }

                return _createShortcutCommand;
            }
        }

        /// <summary>
        /// Gets a value indicating if there are available bookmarks
        /// </summary>
        public bool HasBookMarks
        {
            get { return _hasBookMarks; }
            set
            {
                if (_hasBookMarks != value)
                {
                    _hasBookMarks = value;
                    this.NotifyPropertyChanged(s_hasBookmarksChangedArgs);
                }
            }
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
                if (this.IsEditing &&
                    this.Entity != null)
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
                if (this.IsEditing &&
                    this.Entity != null)
                {
                    return this.Entity[columnName];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating wheter this instance is valid
        /// </summary>
        public virtual bool IsValid
        {
            get { return String.IsNullOrEmpty(this.Error); }
        }

        /// <summary>
        /// Gets a value indicating wheter this instance has changes
        /// </summary>
        public virtual bool HasChanges
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the zoom window is shown
        /// </summary>
        public bool ShowZoomWindow
        {
            get { return _showZoomWindow; }
            set
            {
                if (_showZoomWindow != value)
                {
                    _showZoomWindow = value;
                    this.NotifyPropertyChanged(s_showZoomWindowChangedArgs);
                }
            }
        }

        /// <summary>
        /// Gets or sets the zoom level
        /// </summary>
        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set
            {
                if (_zoomLevel != value)
                {
                    _zoomLevel = value;
                    this.NotifyPropertyChanged(s_zoomLevelChangedArgs);
                }
            }
        }

        private bool IsEditing
        {
            get
            {
                return this.ViewMode == ViewModeType.Add ||
                       this.ViewMode == ViewModeType.Edit;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceViewModel&lt;TEntity&gt;"/> class.
        /// </summary>
        protected WorkspaceViewModel()
            : base()
        {
            this.ZoomLevel = 100;
        }

        protected virtual bool CanAddNew()
        {
            return false;
        }

        protected virtual void OnAddNew()
        {
            s_logger.Debug("Add new '{0}", typeof(TEntity));

            this.ViewMode = ViewModeType.Add;

            TEntity newEntity = new TEntity();

            this.FillDefaultValues();

            this.ResetDataModel(newEntity);
        }

        protected virtual bool CanEdit()
        {
            return (this.ViewMode == ViewModeType.ViewOnly &&
                    this.OriginalEntity == this.Entity);
        }

        protected virtual void OnEdit()
        {
            s_logger.Debug("Edit '{0}", this.Entity.ToString());

            this.ViewMode = ViewModeType.Edit;
        }

        protected virtual bool CanDelete()
        {
            return (this.ViewMode == ViewModeType.ViewOnly &&
                    this.OriginalEntity == this.Entity);
        }

        protected void OnDelete()
        {
            this.ViewMode = ViewModeType.Busy;

            Task task = Task.Factory.StartNew
            (
                () =>
                {
                    s_logger.Debug("Delete '{0}", this.Entity.ToString());

                    this.OnDeleteAction();
                }
            );

            task.ContinueWith
            (
                _ =>
                {
                    s_logger.Debug("Delete successful '{0}'", this.Entity.ToString());

                    this.OnDeleteActionComplete();
                },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent,
                TaskScheduler.FromCurrentSynchronizationContext()
            );

            task.ContinueWith
            (
                (t) =>
                {
                    s_logger.Debug("Error at delete '{0}'", t.Exception.ToString());

                    this.OnDeleteActionFailed(ViewModeType.ViewOnly);

                    Exception exception = null;

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
        }

        protected virtual void OnDeleteAction()
        {
        }

        protected virtual void OnDeleteActionComplete()
        {
            this.ResetDataModel();

            this.ViewMode = ViewModeType.ViewOnly;
        }

        protected virtual void OnDeleteActionFailed(ViewModeType previousViewMode)
        {
            this.ViewMode = previousViewMode;
        }

        protected virtual bool CanSave()
        {
            return ((this.ViewMode == ViewModeType.Add ||
                    this.ViewMode == ViewModeType.Edit) &&
                    this.IsValid && this.HasChanges);
        }

        protected void OnSave()
        {
            ViewModeType previousViewMode = this.ViewMode;

            this.ViewMode = ViewModeType.Busy;
            this.StatusMessage = "Saving changes, please wait ...";

            Task task = Task.Factory.StartNew
            (
                () =>
                {
                    s_logger.Debug("Save changes '{0}'", this.Entity);

                    if (this.Entity != null && this.HasChanges)
                    {
                        this.OnSaveAction();
                    }
                }
            );

            task.ContinueWith
            (
                _ =>
                {
                    s_logger.Debug("Save changes successful '{0}'", this.Entity);

                    this.OnSaveActionComplete();
                },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent,
                TaskScheduler.FromCurrentSynchronizationContext()
            );

            task.ContinueWith
            (
                (t) =>
                {
                    s_logger.Debug("Error at save changes '{0}'", t.Exception.ToString());

                    this.OnSaveActionFailed(previousViewMode);

                    Exception exception = null;

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
        }

        protected virtual void OnSaveAction()
        {
        }

        protected virtual void OnSaveActionComplete()
        {
            this.ResetDataModel();

            this.ViewMode = ViewModeType.ViewOnly;
        }

        protected virtual void OnSaveActionFailed(ViewModeType previousViewMode)
        {
            this.ViewMode = previousViewMode;
        }

        protected virtual bool CanDiscard()
        {
            return (this.ViewMode == ViewModeType.Add ||
                    this.ViewMode == ViewModeType.Edit);
        }

        protected virtual void OnDiscard()
        {
            this.ViewMode = ViewModeType.Busy;

            this.ResetDataModel();

            this.ViewMode = ViewModeType.ViewOnly;
        }

        protected override void OnInquiryActionComplete(InquiryActionResult<TEntity> result)
        {
            switch (result.Result)
            {
                case InquiryActionResultType.RequestedNew:
                    this.OnAddNew();
                    break;

                default:
                    base.OnInquiryActionComplete(result);
                    break;
            }
        }

        protected virtual bool CanPrint()
        {
            return false;
        }

        protected void OnPrint()
        {
            this.ViewMode = ViewModeType.Busy;

            Task.Factory.StartNew(
                () =>
                {
                    Task task = Task.Factory.StartNew
                    (
                        () =>
                        {
                            this.OnPrintAction();
                        }
                    );

                    task.ContinueWith(
                        (t) =>
                        {
                            s_logger.Debug("Error al imprimir '{0}'", this.NavigationRoute);

                            this.Invoke(
                                () =>
                                {
                                    this.OnPrintActionFailed();

                                    Exception exception = null;

                                    if (t.Exception.InnerException != null)
                                    {
                                        exception = t.Exception.InnerException;
                                    }
                                    else
                                    {
                                        exception = t.Exception;
                                    }

                                    this.NotificationMessage = exception.Message;
                                });
                        }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.AttachedToParent);

                    task.ContinueWith(
                        _ =>
                        {
                            s_logger.Debug("Impresion finalizada correctamente '{0}'", this.NavigationRoute);

                            this.Invoke(
                                () =>
                                {
                                    this.StatusMessage = null;
                                    this.OnPrintActionComplete();
                                });
                        }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent);
                });
        }

        protected virtual void OnPrintAction()
        {
        }

        protected virtual void OnPrintActionComplete()
        {
            this.ViewMode = ViewModeType.ViewOnly;
        }

        protected virtual void OnPrintActionFailed()
        {
            this.ViewMode = ViewModeType.ViewOnly;
        }

        protected virtual bool CanPrintPreview()
        {
            return false;
        }

        protected void OnPrintPreview()
        {
            this.ViewMode = ViewModeType.Busy;

            Task.Factory.StartNew(
                () =>
                {
                    Task task = Task.Factory.StartNew
                    (
                        () =>
                        {
                            this.OnPrintPreviewAction();
                        }
                    );

                    task.ContinueWith(
                        (t) =>
                        {
                            s_logger.Debug("Error al mostrar la vista previa '{0}'", this.NavigationRoute);

                            this.Invoke(
                                () =>
                                {
                                    this.OnPrintPreviewActionFailed();

                                    Exception exception = null;

                                    if (t.Exception.InnerException != null)
                                    {
                                        exception = t.Exception.InnerException;
                                    }
                                    else
                                    {
                                        exception = t.Exception;
                                    }

                                    this.NotificationMessage = exception.Message;
                                });
                        }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.AttachedToParent);

                    task.ContinueWith(
                        _ =>
                        {
                            s_logger.Debug("Impresion finalizada correctamente '{0}'", this.NavigationRoute);

                            this.Invoke(
                                () =>
                                {
                                    this.StatusMessage = null;
                                    this.OnPrintPreviewActionComplete();
                                });
                        }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent);
                });
        }

        protected virtual void OnPrintPreviewAction()
        {
        }

        protected virtual void OnPrintPreviewActionComplete()
        {
            this.ViewMode = ViewModeType.ViewOnly;
        }

        protected virtual void OnPrintPreviewActionFailed()
        {
            this.ViewMode = ViewModeType.ViewOnly;
        }

        protected virtual bool CanBookmarkCurrent()
        {
            return this.ViewMode == ViewModeType.ViewOnly &&
                   (this.OriginalEntity != null && this.OriginalEntity == this.Entity);
        }

        protected virtual void OnBookmarkCurrent()
        {
        }

        protected virtual bool CanClearBookmarks()
        {
            return this.ViewMode == ViewModeType.ViewOnly;
        }

        protected virtual void OnClearBookmarks()
        {
        }

        protected virtual bool CanOrganizeBookmarks()
        {
            return this.ViewMode == ViewModeType.ViewOnly;
        }

        protected virtual void OnOrganizeBookmarks()
        {
        }

        protected virtual bool CanCreateShortcut()
        {
            return !String.IsNullOrEmpty(this.NavigationRoute);
        }

        protected virtual void OnCreateShortcut()
        {
            this.GetService<IVirtualDesktopManager>().CreateShortcut<InternalShortcutViewModel>(this.Title, this.NavigationRoute);
        }

        private bool CanShowFormHelp()
        {
            return false;
        }

        private void OnShowFormHelp()
        {
        }

        private bool CanShowZoomWindow()
        {
            return true;
        }

        private void OnShowZoomWindow()
        {
            this.ShowZoomWindow = !this.ShowZoomWindow;
        }

        /// <summary>
        /// Called when the related view is being closed.
        /// </summary>
        public override void Close()
        {
            s_logger.Debug("Cerrar ventana '{0}'", this.GetType());

            _addNewCommand = null;
            _editCommand = null;
            _deleteCommand = null;
            _saveCommand = null;
            _discardCommand = null;
            _printCommand = null;
            _printPreviewCommand = null;
            _createShortcutCommand = null;
            _bookmarkCurrentCommand = null;
            _clearBookmarksCommand = null;
            _organizeBookmarksCommand = null;
            _showFormHelpCommand = null;
            _showZoomWindowCommand = null;
            _hasBookMarks = false;
            _showZoomWindow = false;
            _zoomLevel = 0;

            base.Close();
        }

        protected virtual void FillDefaultValues()
        {
        }

        protected override void UpdateAllowedUserActions()
        {
            this.Invoke
            (
                () =>
                {
                    this.AddNewCommand.RequeryCanExecute();
                    this.EditCommand.RequeryCanExecute();
                    this.DeleteCommand.RequeryCanExecute();
                    this.SaveCommand.RequeryCanExecute();
                    this.DiscardCommand.RequeryCanExecute();
                    this.InquiryCommand.RequeryCanExecute();
                    this.NewWindowCommand.RequeryCanExecute();
                    this.CreateShortcutCommand.RequeryCanExecute();
                    this.BookmarkCurrentCommand.RequeryCanExecute();
                    this.ClearBookmarksCommand.RequeryCanExecute();
                    this.OrganizeBookmarksCommand.RequeryCanExecute();
                    this.PrintCommand.RequeryCanExecute();
                    this.PrintPreviewCommand.RequeryCanExecute();
                    this.ShowFormHelpCommand.RequeryCanExecute();
                    this.ShowZoomWindowCommand.RequeryCanExecute();
                }
            );
        }
    }
}
