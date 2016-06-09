// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using nRoute.Navigation;

namespace Chronos.Presentation.Core.Navigation
{
    public sealed class NavigatedInfo
    {
        private NavigationRequest _request;
        private string _title;
        private Guid _windowId;

        public NavigationRequest Request
        {
            get { return _request; }
        }

        public string Title
        {
            get { return _title; }
        }

        public Guid Id
        {
            get { return _windowId; }
        }

        public NavigatedInfo(NavigationRequest request, string title, Guid windowId)
        {
            _request = request;
            _title = title;
            _windowId = windowId;
        }
    }
}
