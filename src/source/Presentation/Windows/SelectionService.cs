// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Presentation.Windows.Controls;

namespace Chronos.Presentation.Windows
{
    internal sealed class SelectionService
    {
        #region · Fields ·

        private Desktop _canvas;
        private List<ISelectable> _currentSelection;

        #endregion

        #region · Internal Properties ·

        internal List<ISelectable> CurrentSelection
        {
            get
            {
                if (_currentSelection == null)
                {
                    _currentSelection = new List<ISelectable>();
                }

                return _currentSelection;
            }
        }

        #endregion

        #region · Constructors ·

        public SelectionService(Desktop canvas)
        {
            _canvas = canvas;
        }

        #endregion

        #region · Internal Methods ·

        internal void SelectItem(ISelectable item)
        {
            this.ClearSelection();
            this.AddToSelection(item);
        }

        internal void AddToSelection(ISelectable item)
        {
            if (item is IGroupable)
            {
                List<IGroupable> groupItems = this.GetGroupMembers(item as IGroupable);

                foreach (ISelectable groupItem in groupItems)
                {
                    groupItem.IsSelected = true;
                    this.CurrentSelection.Add(groupItem);
                }
            }
            else
            {
                item.IsSelected = true;
                this.CurrentSelection.Add(item);
            }
        }

        internal void RemoveFromSelection(ISelectable item)
        {
            if (item is IGroupable)
            {
                List<IGroupable> groupItems = GetGroupMembers(item as IGroupable);

                foreach (ISelectable groupItem in groupItems)
                {
                    groupItem.IsSelected = false;
                    this.CurrentSelection.Remove(groupItem);
                }
            }
            else
            {
                item.IsSelected = false;
                this.CurrentSelection.Remove(item);
            }
        }

        internal void ClearSelection()
        {
            this.CurrentSelection.ForEach(item => item.IsSelected = false);
            this.CurrentSelection.Clear();
        }

        internal void SelectAll()
        {
            this.ClearSelection();
            this.CurrentSelection.AddRange(_canvas.Children.OfType<ISelectable>());
            this.CurrentSelection.ForEach(item => item.IsSelected = true);
        }

        internal List<IGroupable> GetGroupMembers(IGroupable item)
        {
            IEnumerable<IGroupable> list = _canvas.Children.OfType<IGroupable>();
            IGroupable rootItem = this.GetRoot(list, item);

            return GetGroupMembers(list, rootItem);
        }

        internal IGroupable GetGroupRoot(IGroupable item)
        {
            IEnumerable<IGroupable> list = _canvas.Children.OfType<IGroupable>();

            return this.GetRoot(list, item);
        }

        #endregion

        #region · Private Methods ·

        private IGroupable GetRoot(IEnumerable<IGroupable> list, IGroupable node)
        {
            if (node == null || node.ParentId == Guid.Empty)
            {
                return node;
            }
            else
            {
                foreach (IGroupable item in list)
                {
                    if (item.Id == node.ParentId)
                    {
                        return this.GetRoot(list, item);
                    }
                }

                return null;
            }
        }

        private List<IGroupable> GetGroupMembers(IEnumerable<IGroupable> list, IGroupable parent)
        {
            List<IGroupable> groupMembers = new List<IGroupable>();
            groupMembers.Add(parent);

            var children = list.Where(node => node.ParentId == parent.Id);

            foreach (IGroupable child in children)
            {
                groupMembers.AddRange(this.GetGroupMembers(list, child));
            }

            return groupMembers;
        }

        #endregion
    }
}
