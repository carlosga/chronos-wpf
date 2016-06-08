// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using nRoute.Navigation;

namespace Chronos.Presentation.Core.Navigation
{
    public sealed class NavigatingCancelInfo
    {
        #region · Fields ·

        private NavigationRequest request;

        #endregion

        #region · Properties ·

        public NavigationRequest Request
        {
            get { return this.request; }
        }

        public bool Cancel
        {
            get;
            set;
        }

        #endregion

        #region · Constructors ·

        public NavigatingCancelInfo(NavigationRequest request)
        {
            this.request = request;
        }

        #endregion
    }
}
