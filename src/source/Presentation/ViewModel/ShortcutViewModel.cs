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
        private static readonly PropertyChangedEventArgs s_iconStyleChangedArgs = CreateArgs<ShortcutViewModel>(x => x.IconStyle);
        private static readonly PropertyChangedEventArgs s_targetChangedArgs = CreateArgs<ShortcutViewModel>(x => x.Target);
        private static readonly PropertyChangedEventArgs s_parametersChangedArgs = CreateArgs<ShortcutViewModel>(x => x.Parameters);

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private ActionCommand _openCommand;
        private string _target;
        private string _parameters;
        private string _iconStyle;

        /// <summary>
        /// Gets the open command.
        /// </summary>
        /// <value>The open command.</value>
        public ActionCommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                {
                    _openCommand = new ActionCommand(() => OnOpen());
                }

                return _openCommand;
            }
        }

        /// <summary>
        /// Gets or sets the icon style.
        /// </summary>
        /// <value>The icon style.</value>
        public virtual string IconStyle
        {
            get { return _iconStyle; }
            set
            {
                if (_iconStyle != value)
                {
                    _iconStyle = value;
                    this.NotifyPropertyChanged(s_iconStyleChangedArgs);
                }
            }
        }

        /// <summary>
        /// Gets or sets the shortcut target.
        /// </summary>
        /// <value>The shortcut target.</value>
        public virtual string Target
        {
            get { return _target; }
            set
            {
                if (_target != value)
                {
                    _target = value;
                    this.NotifyPropertyChanged(s_targetChangedArgs);
                }
            }
        }

        /// <summary>
        /// Gets or sets the shortcut navigation parameters
        /// </summary>
        public virtual string Parameters
        {
            get { return _parameters; }
            set
            {
                if (_parameters != value)
                {
                    _parameters = value;
                    this.NotifyPropertyChanged(s_parametersChangedArgs);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutViewModel"/> class.
        /// </summary>
        protected ShortcutViewModel()
            : base()
        {
        }

        /// <summary>
        /// Called when the <see cref="OpenCommand"/>  is executed.
        /// </summary>
        protected abstract void OnOpen();

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

            showMessageService.ButtonSetup = DialogButton.YesNo;
            showMessageService.Caption = "Eliminar acceso directo";
            showMessageService.Text =
                String.Format(
                    "\u00BFEst\u00E1 seguro de que desea eliminar permanentemente este acceso directo? {0}{1}",
                        Environment.NewLine,
                            this.Title);

            if (showMessageService.ShowMessage() == DialogResult.Yes)
            {
                s_logger.Debug("Eliminando acceso directo '{0}'", this.Target);

                base.Close();

                _target = null;
                _parameters = null;
                _iconStyle = null;
                _openCommand = null;
            }
        }
    }
}
