// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using nRoute.Navigation;

namespace Chronos.Presentation.Core.Navigation
{
    public sealed class NavigatedInfo
    {
        #region · Fields ·

        private NavigationRequest   request;
        private string              title;
        private Guid                windowId;

        #endregion

        #region · Properties ·

        public NavigationRequest Request
        {
            get { return this.request; }
        }

        public string Title
        {
            get { return this.title; }
        }

        public Guid Id
        {
            get { return this.windowId; }
        }

        #endregion

        #region · Constructors ·

        public NavigatedInfo(NavigationRequest request, string title, Guid windowId)
        {
            this.request    = request;
            this.title      = title;
            this.windowId   = windowId;
        }        

        #endregion
    }
}
