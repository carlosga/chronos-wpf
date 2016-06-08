// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using Chronos.Presentation.Core.Services;
using Chronos.Presentation.Core.ViewModel;
using Chronos.Presentation.Core.Windows;
using NLog;
using nRoute.Components;

namespace Chronos.Presentation.ViewModel
{
    /// <summary>
    /// Base class for shortcut viewmodel implementations
    /// </summary>
    public abstract class ShortcutViewModel 
        : ClosableViewModel, IShortcutViewModel
    {
        #region · NotifyPropertyChanged Cached Instances ·

        private static readonly PropertyChangedEventArgs IconStyleChangedArgs   = CreateArgs<ShortcutViewModel>(x => x.IconStyle);
        private static readonly PropertyChangedEventArgs TargetChangedArgs      = CreateArgs<ShortcutViewModel>(x => x.Target);
        private static readonly PropertyChangedEventArgs ParametersChangedArgs  = CreateArgs<ShortcutViewModel>(x => x.Parameters);

        #endregion

        #region · Logger ·

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region · Fields ·

        private string target;
        private string parameters;
        private string iconStyle;

        #region · Commands ·

        private ActionCommand openCommand;

        #endregion

        #endregion

        #region · Commands ·

        /// <summary>
        /// Gets the open command.
        /// </summary>
        /// <value>The open command.</value>
        public ActionCommand OpenCommand
        {
            get
            {
                if (this.openCommand == null)
                {
                    this.openCommand = new ActionCommand(() => OnOpen());
                }

                return this.openCommand;
            }
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets or sets the icon style.
        /// </summary>
        /// <value>The icon style.</value>
        public virtual string IconStyle
        {
            get { return this.iconStyle; }
            set
            {
                if (this.iconStyle != value)
                {
                    this.iconStyle = value;
                    this.NotifyPropertyChanged(IconStyleChangedArgs);
                }
            }
        }

        /// <summary>
        /// Gets or sets the shortcut target.
        /// </summary>
        /// <value>The shortcut target.</value>
        public virtual string Target
        {
            get { return this.target; }
            set
            {
                if (this.target != value)
                {
                    this.target = value;
                    this.NotifyPropertyChanged(TargetChangedArgs);
                }
            }
        }

        /// <summary>
        /// Gets or sets the shortcut navigation parameters
        /// </summary>
        public virtual string Parameters
        {
            get { return this.parameters; }
            set
            {
                if (this.parameters != value)
                {
                    this.parameters = value;
                    this.NotifyPropertyChanged(ParametersChangedArgs);
                }
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutViewModel"/> class.
        /// </summary>
        protected ShortcutViewModel()
            : base()
        {
        }

        #endregion

        #region · Command Actions ·

        /// <summary>
        /// Called when the <see cref="OpenCommand"/>  is executed.
        /// </summary>
        protected abstract void OnOpen();

        #endregion

        #region · Overriden Methods ·

        /// <summary>
        /// Determines whether the view related to this view model can be closed.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the related view can be closed; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanClose()
        {
            return true;
        }

        /// <summary>
        /// Called when the related view is being closed.
        /// </summary>
        public override void Close()
        {            
            IShowMessageViewService showMessageService = this.GetViewService<IShowMessageViewService>();

            showMessageService.ButtonSetup  = DialogButton.YesNo;
            showMessageService.Caption      = "Eliminar acceso directo";
            showMessageService.Text         =
                String.Format(
                    "¿Está seguro de que desea eliminar permanentemente este acceso directo? {0}{1}",
                        Environment.NewLine,
                            this.Title);

            if (showMessageService.ShowMessage() == DialogResult.Yes)
            {
                Logger.Debug("Eliminando acceso directo '{0}'", this.Target);

                base.Close();

                this.target         = null;
                this.parameters     = null;
                this.iconStyle      = null;
                this.openCommand    = null;
            }
        }

        #endregion
    }
}
