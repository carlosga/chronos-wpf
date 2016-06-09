// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Chronos.Authentication
{
    /// <summary>
    /// Provides information about an information action
    /// </summary>
    public sealed class AuthenticationInfo
    {
        /// <summary>
        /// Gets or sets the user id
        /// </summary>
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="AuthenticationAction"/>
        /// </summary>
        public AuthenticationAction Action
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationInfo"/> class
        /// </summary>
        public AuthenticationInfo()
        {
        }
    }
}
