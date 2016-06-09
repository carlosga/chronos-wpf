// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;

namespace Chronos.Presentation.DragAndDrop
{
    public sealed class DragDataWrapper
    {
        private DependencyObject _source;
        private object _data;
        private bool _allowChildrenRemove;
        private IDataDropObjectProvider _shim;

        public DependencyObject Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public bool AllowChildrenRemove
        {
            get { return _allowChildrenRemove; }
            set { _allowChildrenRemove = value; }
        }

        public IDataDropObjectProvider Shim
        {
            get { return _shim; }
            set { _shim = value; }
        }

        public DragDataWrapper()
        {
        }
    }
}
