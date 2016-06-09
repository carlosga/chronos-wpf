// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Chronos.Presentation.Core.Services;
using Chronos.Presentation.Core.ViewModel;
using Chronos.Presentation.Core.Windows;
using NLog;
using nRoute.Components;

namespace Chronos.Presentation.ViewModel
{
    public sealed class ShortcutGroupViewModel
        : ClosableViewModel
    {
        private const string DefaultIconStyle = "FolderIconStyle";

        private static readonly PropertyChangedEventArgs s_iconStyleChangedArgs = CreateArgs<ShortcutGroupViewModel>(x => x.IconStyle);

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private string _iconStyle;
        private List<IShortcutViewModel> _shortcuts;

        /// <summary>
        /// Gets or sets the shortcut icon style.
        /// </summary>
        /// <value>The icon style.</value>
        public string IconStyle
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_iconStyle))
                {
                    return DefaultIconStyle;
                }

                return _iconStyle;
            }
            set { _iconStyle = value; }
        }

        public List<IShortcutViewModel> Shortcuts
        {
            get
            {
                if (_shortcuts == null)
                {
                    _shortcuts = new List<IShortcutViewModel>();
                }

                return _shortcuts;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutGroupViewModel"/> class.
        /// </summary>
        public ShortcutGroupViewModel()
            : base()
        {
        }

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
            showMessageService.Caption = "Eliminar grupo de accesos directos";
            showMessageService.Text =
                String.Format(
                    "\u00BFEst\u00E1 seguro de que desea eliminar permanentemente este grupo de accesos directos? {0}{1}",
                        Environment.NewLine,
                            this.Title);

            if (showMessageService.ShowMessage() == DialogResult.Yes)
            {
                s_logger.Debug("Eliminando grupo de accesos directos '{0}'", this.Title);

                base.Close();

                _iconStyle = null;

                if (_shortcuts != null)
                {
                    _shortcuts.Clear();
                    _shortcuts = null;
                }
            }
        }
    }
}
