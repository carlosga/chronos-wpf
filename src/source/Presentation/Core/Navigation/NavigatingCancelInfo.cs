// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using nRoute.Navigation;

namespace Chronos.Presentation.Core.Navigation
{
    public sealed class NavigatingCancelInfo
    {
        private NavigationRequest _request;

        public NavigationRequest Request
        {
            get { return _request; }
        }

        public bool Cancel
        {
            get;
            set;
        }

        public NavigatingCancelInfo(NavigationRequest request)
        {
            _request = request;
        }
    }
}
