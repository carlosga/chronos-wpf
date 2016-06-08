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
        #region · Constants ·

        private const string DefaultIconStyle = "FolderIconStyle";

        #endregion

        #region · NotifyPropertyChanged Cached Instances ·

        private static readonly PropertyChangedEventArgs IconStyleChangedArgs = CreateArgs<ShortcutGroupViewModel>(x => x.IconStyle);

        #endregion

        #region · Logger ·

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region · Fields ·

        private string                      iconStyle;
        private List<IShortcutViewModel>    shortcuts;

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets or sets the shortcut icon style.
        /// </summary>
        /// <value>The icon style.</value>
        public string IconStyle
        {
            get
            {
                if (String.IsNullOrWhiteSpace(this.iconStyle))
                {
                    return DefaultIconStyle;
                }

                return this.iconStyle;
            }
            set { this.iconStyle = value; }
        }

        public List<IShortcutViewModel> Shortcuts
        {
            get
            {
                if (this.shortcuts == null)
                {
                    this.shortcuts = new List<IShortcutViewModel>();
                }

                return this.shortcuts;
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutGroupViewModel"/> class.
        /// </summary>
        public ShortcutGroupViewModel()
            : base()
        {
        }

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
            showMessageService.Caption      = "Eliminar grupo de accesos directos";
            showMessageService.Text         =
                String.Format(
                    "¿Está seguro de que desea eliminar permanentemente este grupo de accesos directos? {0}{1}",
                        Environment.NewLine,
                            this.Title);

            if (showMessageService.ShowMessage() == DialogResult.Yes)
            {
                Logger.Debug("Eliminando grupo de accesos directos '{0}'", this.Title);

                base.Close();

                this.iconStyle = null;

                if (this.shortcuts != null)
                {
                    this.shortcuts.Clear();
                    this.shortcuts  = null;
                }
            }
        }

        #endregion
    }
}
