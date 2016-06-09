// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Xml;
using Chronos.Extensions.Windows;

namespace Chronos.Presentation.DragAndDrop
{
    public sealed class TreeViewDragDropDataProvider
        : IDataDropObjectProvider
    {
        private TreeView _treeview;

        public DragDropProviderActions SupportedActions
        {
            get
            {
                return DragDropProviderActions.Data |
                       DragDropProviderActions.MultiFormatData;
            }
        }

        public TreeViewDragDropDataProvider(TreeView treeview)
        {
            _treeview = treeview;
        }

        public void AppendData(ref IDataObject data, MouseEventArgs e)
        {
            if (!(_treeview.InputHitTest(e.GetPosition(e.OriginalSource as UIElement)) is TreeView)
                && !(_treeview.InputHitTest(e.GetPosition(e.OriginalSource as UIElement)) is ScrollViewer)
                && !(e.OriginalSource is Thumb))
            {
                TreeViewItem selectedUIelement = this.GetVisual(e).GetParent<TreeViewItem>();

                if (selectedUIelement != null && selectedUIelement.Items.Count == 0)
                {
                    object selectedItem = _treeview.SelectedItem;

                    if (selectedItem != null)
                    {
                        if (selectedItem.GetType() == typeof(XmlElement))
                        {
                            data.SetData(DataFormats.Text, ((XmlElement)selectedItem).OuterXml);
                        }
                        else
                        {
                            data.SetData(DataFormats.Text, selectedItem.ToString());
                        }

                        data.SetData(selectedItem.GetType().ToString(), selectedItem);
                    }
                    else
                    {
                        data = null;
                    }
                }
                else
                {
                    data = null;
                }
            }
            else
            {
                data = null;
            }
        }

        public object GetData()
        {
            return _treeview.SelectedItem;
        }

        public UIElement GetVisual(MouseEventArgs e)
        {
            // return this.treeview.ItemContainerGenerator.ContainerFromItem(this.treeview.SelectedItem) as UIElement;
            return e.OriginalSource as UIElement;
        }

        public void GiveFeedback(System.Windows.GiveFeedbackEventArgs args)
        {
            throw new NotImplementedException("Forgot to check the Supported actions??");
        }

        public void ContinueDrag(System.Windows.QueryContinueDragEventArgs args)
        {
            throw new NotImplementedException("Forgot to check the Supported actions??");
        }

        public bool UnParent()
        {
            // We are passing data, nothing to unparent 
            throw new NotImplementedException("We are passing data, nothing to unparent... what up ");
        }
    }
}
