// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Chronos.Presentation.Core.Navigation;
using NLog;

namespace Chronos.Presentation.ViewModel
{
    /// <summary>
    /// Internal shortcut viewmodel class
    /// </summary>
    public sealed class InternalShortcutViewModel
        : ShortcutViewModel
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private static readonly string s_defaultIconStyle = "WindowIconStyle";

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
        /// Initializes a new instance of the <see cref="InternalShortcutViewModel"/> class.
        /// </summary>
        public InternalShortcutViewModel()
            : base()
        {
        }

        /// <summary>
        /// Called when the <see cref="OpenCommand"/>  is executed.
        /// </summary>
        protected override void OnOpen()
        {
            s_logger.Debug("Ejecuci\u00F3n de un acceso directo ({0})", this.Target);

            this.GetService<INavigationService>().Navigate(this.Target, 10, 20, "BBB");
        }
    }
}
