// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;

namespace Chronos.Presentation.DragAndDrop
{
    public sealed class DragDataWrapper
    {
        #region · Fields ·

        private DependencyObject        source;
        private object                  data;
        private bool                    allowChildrenRemove;
        private IDataDropObjectProvider shim;

        #endregion

        #region · Properties ·

        public DependencyObject Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        public object Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        public bool AllowChildrenRemove
        {
            get { return this.allowChildrenRemove; }
            set { this.allowChildrenRemove = value; }
        }

        public IDataDropObjectProvider Shim
        {
            get { return this.shim; }
            set { this.shim = value; }
        }

        #endregion

        #region · Constructors ·

        public DragDataWrapper()
        {
        }

        #endregion
    }
}
