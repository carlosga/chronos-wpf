// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Chronos.Presentation.Core.Services;
using Chronos.Presentation.Core.VirtualDesktops;
using Chronos.Presentation.Core.Windows;
using NLog;

namespace Chronos.Presentation.ViewModel
{
    /// <summary>
    /// External shortcut viewmodel class
    /// </summary>
    public sealed class ExternalShortcutViewModel
        : ShortcutViewModel
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private static readonly string s_defaultIconStyle = "ArrowRight01IconStyle";

        /// <summary>
        /// Gets or sets the shortcut icon style.
        /// </summary>
        /// <value>The icon style.</value>
        public override string IconStyle
        {
            get
            {
                if (String.IsNullOrWhiteSpace(base.IconStyle))
                {
                    return s_defaultIconStyle;
                }

                return base.IconStyle;
            }
            set { base.IconStyle = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalShortcutViewModel"/> class.
        /// </summary>
        public ExternalShortcutViewModel()
            : base()
        {
        }

        /// <summary>
        /// Called when the <see cref="OpenCommand"/>  is executed.
        /// </summary>
        protected override void OnOpen()
        {
            Task task = Task.Factory.StartNew(
                () =>
                {
                    s_logger.Debug("Ejecuci\u00F3n de un acceso directo ({0})", this.Target);

                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = this.Target.ToString();
                        process.StartInfo.UseShellExecute = true;
                        process.StartInfo.LoadUserProfile = true;
                        process.Start();
                    }
                })
            .ContinueWith((t) =>
                {
                    s_logger.ErrorException("Error durante la ejecuci\u00F3n de un acceso directo ({0})", t.Exception.InnerException);

                    IShowMessageViewService showMessageService = this.GetViewService<IShowMessageViewService>();

                    showMessageService.ButtonSetup = DialogButton.Ok;
                    showMessageService.Caption = "Problema con el acceso directo";
                    showMessageService.Text =
                        String.Format(
                            "Ha ocurrido un error durante la ejecuci\u00F3n del acceso directo '{0}'",
                                this.Title);

                    if (showMessageService.ShowMessage() == DialogResult.Yes)
                    {
                        this.GetService<IVirtualDesktopManager>().Close(this.Id);
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
