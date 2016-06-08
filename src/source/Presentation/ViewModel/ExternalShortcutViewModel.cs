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
        #region · Logger ·

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region · Consts ·

        private static readonly string DefaultIconStyle = "ArrowRight01IconStyle";

        #endregion

        #region · Properties ·

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
                    return DefaultIconStyle;
                }

                return base.IconStyle;
            }
            set { base.IconStyle = value; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalShortcutViewModel"/> class.
        /// </summary>
        public ExternalShortcutViewModel()
            : base()
        {
        }

        #endregion

        #region · Command Execution ·

        /// <summary>
        /// Called when the <see cref="OpenCommand"/>  is executed.
        /// </summary>
        protected override void OnOpen()
        {
            Task task = Task.Factory.StartNew(
                () =>
                {
                    Logger.Debug("Ejecución de un acceso directo ({0})", this.Target);

                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName          = this.Target.ToString();
                        process.StartInfo.UseShellExecute   = true;
                        process.StartInfo.LoadUserProfile   = true;
                        process.Start();
                    }
                })
            .ContinueWith((t) =>
                {
                    Logger.ErrorException("Error durante la ejecución de un acceso directo ({0})", t.Exception.InnerException);

                    IShowMessageViewService showMessageService = this.GetViewService<IShowMessageViewService>();

                    showMessageService.ButtonSetup  = DialogButton.Ok;
                    showMessageService.Caption      = "Problema con el acceso directo";
                    showMessageService.Text         =
                        String.Format(
                            "Ha ocurrido un error durante la ejecución del acceso directo '{0}'",
                                this.Title);

                    if (showMessageService.ShowMessage() == DialogResult.Yes)
                    {
                        this.GetService<IVirtualDesktopManager>().Close(this.Id);
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
        }

        #endregion
    }
}
