// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Xml;

namespace Chronos.Presentation.DragAndDrop
{
    public sealed class ListBoxDragDropDataProvider
        : IDataDropObjectProvider
    {
        private ListBox _list;

        public DragDropProviderActions SupportedActions
        {
            get
            {
                return DragDropProviderActions.Data |
                       DragDropProviderActions.Visual |
                       DragDropProviderActions.Unparent |
                       DragDropProviderActions.MultiFormatData;
            }
        }

        public ListBoxDragDropDataProvider(ListBox list)
        {
            _list = list;
        }

        public void AppendData(ref IDataObject data, MouseEventArgs e)
        {
            if (!(_list.InputHitTest(e.GetPosition(e.OriginalSource as UIElement)) is ListBox)
                && !(_list.InputHitTest(e.GetPosition(e.OriginalSource as UIElement)) is ScrollViewer)
                && !(e.OriginalSource is Thumb))
            {
                object o = _list.SelectedItem;

                // This is cheating .. just for an example's sake.. 
                Debug.Assert(!data.GetDataPresent(DataFormats.Text));

                if (o.GetType() == typeof(XmlElement))
                {
                    data.SetData(DataFormats.Text, ((XmlElement)o).OuterXml);
                }
                else
                {
                    data.SetData(DataFormats.Text, o.ToString());
                }

                Debug.Assert(!data.GetDataPresent(o.GetType().ToString()));

                data.SetData(o.GetType().ToString(), o);
            }
            else
            {
                data = null;
            }
        }

        public object GetData()
        {
            return _list.SelectedItem;
        }

        public UIElement GetVisual(MouseEventArgs e)
        {
            return _list.ItemContainerGenerator.ContainerFromItem(_list.SelectedItem) as UIElement;
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
