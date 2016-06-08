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
        #region · Logger ·

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region · PropertyChangedEventArgs Cached Instances ·

        private static readonly PropertyChangedEventArgs HasBookmarksChangedArgs        = CreateArgs<WorkspaceViewModel<TEntity>>(x => x.HasBookMarks);
        private static readonly PropertyChangedEventArgs HasRelationsChangedArgs        = CreateArgs<WorkspaceViewModel<TEntity>>(x => x.HasRelations);
        private static readonly PropertyChangedEventArgs ShowZoomWindowChangedArgs      = CreateArgs<WorkspaceViewModel<TEntity>>(x => x.ShowZoomWindow);
        private static readonly PropertyChangedEventArgs ZoomLevelChangedArgs           = CreateArgs<WorkspaceViewModel<TEntity>>(x => x.ZoomLevel);
        
        #endregion

        #region · Fields ·

        private bool    hasBookMarks;
        private bool    showZoomWindow;
        private double  zoomLevel;

        #region · Commands ·

        private ActionCommand addNewCommand;
        private ActionCommand editCommand;
        private ActionCommand deleteCommand;
        private ActionCommand saveCommand;
        private ActionCommand discardCommand;
        private ActionCommand printCommand;
        private ActionCommand printPreviewCommand;
        private ActionCommand createShortcutCommand;
        private ActionCommand bookmarkCurrentCommand;
        private ActionCommand clearBookmarksCommand;
        private ActionCommand organizeBookmarksCommand;
        private ActionCommand showFormHelpCommand;
        private ActionCommand showZoomWindowCommand;

        #endregion

        #endregion

        #region · IWorkspaceViewModel<TEntity> Commands ·

        /// <summary>
        /// Gets the add new command
        public ActionCommand AddNewCommand
        {
            get
            {
                if (this.addNewCommand == null)
                {
                    this.addNewCommand = new ActionCommand
                    (
                        () => OnAddNew(), 
                        () => CanAddNew()
                    );
                }

                return this.addNewCommand;
            }
        }

        /// <summary>
        /// Gets the edit command
        /// </summary>
        public ActionCommand EditCommand
        {
            get
            {
                if (this.editCommand == null)
                {
                    this.editCommand = new ActionCommand
                    (
                        () => OnEdit(),
                        () => CanEdit()
                    );
                }

                return this.editCommand;
            }
        }

        /// <summary>
        /// Gets the delete command
        /// </summary>
        public ActionCommand DeleteCommand
        {
            get
            {
                if (this.deleteCommand == null)
                {
                    this.deleteCommand = new ActionCommand
                    (
                        () => OnDelete(), 
                        () => CanDelete()
                    );
                }

                return this.deleteCommand;
            }
        }

        /// <summary>
        /// Gets the save command
        /// </summary>
        public ActionCommand SaveCommand
        {
            get
            {
                if (this.saveCommand == null)
                {
                    this.saveCommand = new ActionCommand
                    (
                        () => OnSave(),
                        () => CanSave()
                    );
                }

                return this.saveCommand;
            }
        }

        /// <summary>
        /// Gets the discard command
        /// </summary>
        public ActionCommand DiscardCommand
        {
            get
            {
                if (this.discardCommand == null)
                {
                    this.discardCommand = new ActionCommand
                    (
                        () => OnDiscard(),
                        () => CanDiscard()
                    );
                }

                return this.discardCommand;
            }
        }

        /// <summary>
        /// Gets the print command
        /// </summary>
        public ActionCommand PrintCommand
        {
            get
            {
                if (this.printCommand == null)
                {
                    this.printCommand = new ActionCommand
                    (
                        () => OnPrint(),
                        () => CanPrint()
                    );
                }

                return this.printCommand;
            }
        }

        /// <summary>
        /// Gets the print preview command
        /// </summary>
        public ActionCommand PrintPreviewCommand
        {
            get
            {
                if (this.printPreviewCommand == null)
                {
                    this.printPreviewCommand = new ActionCommand
                    (
                        () => OnPrintPreview(),
                        () => CanPrintPreview()
                    );
                }

                return this.printPreviewCommand;
            }
        }

        /// <summary>
        /// Gets the show form help command
        /// </summary>
        public ActionCommand ShowFormHelpCommand
        {
            get
            {
                if (this.showFormHelpCommand == null)
                {
                    this.showFormHelpCommand = new ActionCommand
                    (
                        () => OnShowFormHelp(),
                        () => CanShowFormHelp()
                    );
                }

                return this.showFormHelpCommand;
            }
        }

        /// <summary>
        /// Gets the show zoom window command
        /// </summary>
        public ActionCommand ShowZoomWindowCommand
        {
            get
            {
                if (this.showZoomWindowCommand == null)
                {
                    this.showZoomWindowCommand = new ActionCommand
                    (
                        () => OnShowZoomWindow(),
                        () => CanShowZoomWindow()
                    );
                }

                return this.showZoomWindowCommand;
            }
        }

        #endregion        

        #region · IBookmarkViewModel Commands ·

        /// <summary>
        /// Gets the bookmark current command
        /// </summary>
        public ActionCommand BookmarkCurrentCommand
        {
            get
            {
                if (this.bookmarkCurrentCommand == null)
                {
                    this.bookmarkCurrentCommand = new ActionCommand
                    (
                        () => OnBookmarkCurrent(),
                        () => CanBookmarkCurrent()
                    );
                }

                return this.bookmarkCurrentCommand;
            }
        }

        /// <summary>
        /// Gets the clear bookmarks command
        /// </summary>
        public ActionCommand ClearBookmarksCommand
        {
            get
            {
                if (this.clearBookmarksCommand == null)
                {
                    this.clearBookmarksCommand = new ActionCommand
                    (
                        () => OnClearBookmarks(),
                        () => CanClearBookmarks()
                    );
                }

                return this.clearBookmarksCommand;
            }
        }

        /// <summary>
        /// Gets the organize bookmarks command
        /// </summary>
        public ActionCommand OrganizeBookmarksCommand
        {
            get
            {
                if (this.organizeBookmarksCommand == null)
                {
                    this.organizeBookmarksCommand = new ActionCommand
                    (
                        () => OnOrganizeBookmarks(),
                        () => CanOrganizeBookmarks()
                    );
                }

                return this.organizeBookmarksCommand;
            }
        }

        /// <summary>
        /// Gets the create shortcut command
        /// </summary>
        public ActionCommand CreateShortcutCommand
        {
            get
            {
                if (this.createShortcutCommand == null)
                {
                    this.createShortcutCommand = new ActionCommand
                    (
                        () => OnCreateShortcut(),
                        () => CanCreateShortcut()
                    );
                }

                return this.createShortcutCommand;
            }
        }

        #endregion

        #region · IBookmarkViewModel Properties ·

        /// <summary>
        /// Gets a value indicating if there are available bookmarks
        /// </summary>
        public bool HasBookMarks
        {
            get { return this.hasBookMarks; }
            set
            {
                if (this.hasBookMarks != value)
                {
                    this.hasBookMarks = value;
                    this.NotifyPropertyChanged(HasBookmarksChangedArgs);
                }
            }
        }

        #endregion

        #region · IEntityViewModel<TEntity> Properties & Indexers ·

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

        #endregion

        #region · IWorkspaceViewModel<TEntity> Properties ·

        /// <summary>
        /// Gets or sets a value indicating whether the zoom window is shown
        /// </summary>
        public bool ShowZoomWindow
        {
            get { return this.showZoomWindow; }
            set
            {
                if (this.showZoomWindow != value)
                {
                    this.showZoomWindow = value;
                    this.NotifyPropertyChanged(ShowZoomWindowChangedArgs);
                }
            }
        }

        /// <summary>
        /// Gets or sets the zoom level
        /// </summary>
        public double ZoomLevel
        {
            get { return this.zoomLevel; }
            set
            {
                if (this.zoomLevel != value)
                {
                    this.zoomLevel = value;
                    this.NotifyPropertyChanged(ZoomLevelChangedArgs);
                }
            }
        }

        #endregion

        #region · Private Properties ·

        private bool IsEditing
        {
            get
            {
                return this.ViewMode == ViewModeType.Add ||
                       this.ViewMode == ViewModeType.Edit;
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceViewModel&lt;TEntity&gt;"/> class.
        /// </summary>
        protected WorkspaceViewModel()
            : base()
        {
            this.ZoomLevel = 100;
        }

        #endregion

        #region · Command Actions ·

        #region · Add New ·

        protected virtual bool CanAddNew()
        {
            return false;
        }

        protected virtual void OnAddNew()
        {
            Logger.Debug("Add new '{0}", typeof(TEntity));

            this.ViewMode = ViewModeType.Add;

            TEntity newEntity = new TEntity();

            this.FillDefaultValues();

            this.ResetDataModel(newEntity);
        }

        #endregion

        #region · Edit ·

        protected virtual bool CanEdit()
        {
            return (this.ViewMode == ViewModeType.ViewOnly &&
                    this.OriginalEntity == this.Entity);
        }

        protected virtual void OnEdit()
        {
            Logger.Debug("Edit '{0}", this.Entity.ToString());

            this.ViewMode = ViewModeType.Edit;
        }

        #endregion

        #region · Delete ·

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
                    Logger.Debug("Delete '{0}", this.Entity.ToString());

                    this.OnDeleteAction();
                }
            );

            task.ContinueWith
            (
                _ =>
                {
                    Logger.Debug("Delete successful '{0}'", this.Entity.ToString());

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
                    Logger.Debug("Error at delete '{0}'", t.Exception.ToString());

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

        #endregion

        #region · Save ·

        protected virtual bool CanSave()
        {
            return ((this.ViewMode == ViewModeType.Add ||
                    this.ViewMode == ViewModeType.Edit) &&
                    this.IsValid && this.HasChanges);
        }

        protected void OnSave()
        {
            ViewModeType previousViewMode = this.ViewMode;

            this.ViewMode       = ViewModeType.Busy;
            this.StatusMessage  = "Saving changes, please wait ...";

            Task task = Task.Factory.StartNew
            (
                () =>
                {
                    Logger.Debug("Save changes '{0}'", this.Entity);

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
                    Logger.Debug("Save changes successful '{0}'", this.Entity);

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
                    Logger.Debug("Error at save changes '{0}'", t.Exception.ToString());

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

        #endregion

        #region · Discard ·

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

        #endregion

        #region · Inquiry ·

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

        #endregion

        #region · Print ·

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
                            Logger.Debug("Error al imprimir '{0}'", this.NavigationRoute);

                            this.Invoke(
                                () =>
                                {
                                    this.OnPrintActionFailed();

                                    Exception exception     = null;

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
                            Logger.Debug("Impresion finalizada correctamente '{0}'", this.NavigationRoute);

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

        #endregion

        #region · Print Preview ·

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
                            Logger.Debug("Error al mostrar la vista previa '{0}'", this.NavigationRoute);

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
                            Logger.Debug("Impresion finalizada correctamente '{0}'", this.NavigationRoute);

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

        #endregion

        #region · Bookmark Current Command ·

        protected virtual bool CanBookmarkCurrent()
        {
            return this.ViewMode == ViewModeType.ViewOnly &&
                   (this.OriginalEntity != null && this.OriginalEntity == this.Entity);
        }

        protected virtual void OnBookmarkCurrent()
        {
        }

        #endregion

        #region · Clear Bookmarks Command ·

        protected virtual bool CanClearBookmarks()
        {
            return this.ViewMode == ViewModeType.ViewOnly;
        }

        protected virtual void OnClearBookmarks()
        {
        }

        #endregion

        #region · Organize Bookmarks Command ·

        protected virtual bool CanOrganizeBookmarks()
        {
            return this.ViewMode == ViewModeType.ViewOnly;
        }

        protected virtual void OnOrganizeBookmarks()
        {
        }

        #endregion

        #region · Create Shortcut Command ·

        protected virtual bool CanCreateShortcut()
        {
            return !String.IsNullOrEmpty(this.NavigationRoute);
        }

        protected virtual void OnCreateShortcut()
        {
            this.GetService<IVirtualDesktopManager>().CreateShortcut<InternalShortcutViewModel>(this.Title, this.NavigationRoute);
        }

        #endregion

        #region · Show Form Help Command ·

        private bool CanShowFormHelp()
        {
            return false;
        }

        private void OnShowFormHelp()
        {
        }

        #endregion

        #region · Show Zoom Window Command ·

        private bool CanShowZoomWindow()
        {
            return true;
        }

        private void OnShowZoomWindow()
        {
            this.ShowZoomWindow = !this.ShowZoomWindow;
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

            this.addNewCommand              = null;
            this.editCommand                = null;
            this.deleteCommand              = null;
            this.saveCommand                = null;
            this.discardCommand             = null;
            this.printCommand               = null;
            this.printPreviewCommand        = null;
            this.createShortcutCommand      = null;
            this.bookmarkCurrentCommand     = null;
            this.clearBookmarksCommand      = null;
            this.organizeBookmarksCommand   = null;
            this.showFormHelpCommand        = null;
            this.showZoomWindowCommand      = null;
            this.hasBookMarks               = false;
            this.showZoomWindow             = false;
            this.zoomLevel                  = 0;

            base.Close();
        }

        #endregion  

        #region · Protected Methods ·

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

        #endregion
    }
}