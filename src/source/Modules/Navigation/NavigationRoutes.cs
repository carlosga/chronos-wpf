// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Chronos.Modules.Navigation
{
    /// <summary>
    /// Navigation route definitions
    /// </summary>
    public static class NavigationRoutes
    {
        #region · Application · 

        /// <summary>
        /// Route for the login view
        /// </summary>
        public const string Login = "Authentication/Login";
        
        /// <summary>
        /// Route for the about box view
        /// </summary>
        public const string About = "Application/About";

        #endregion

        #region · Configuration ·

        /// <summary>
        /// Route for the companies view
        /// </summary>
        public const string Companies = "Configuracion/Empresas";

        #endregion
    }
}
