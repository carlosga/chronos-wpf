// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Chronos.Presentation.Core.VirtualDesktops;
using Chronos.Presentation.Core.Widgets;
using Chronos.Presentation.ViewModel;
using Chronos.Presentation.Windows.Controls;
using nRoute.Services;
using nRoute.SiteMaps;

namespace Chronos.Presentation.DragAndDrop
{
    public sealed class DropHelper
    {
        private const string UrlDataFormat = "text/x-moz-url";

        private UIElement _dropTarget = null;
        private string[] _datatypes = { typeof(UIElement).ToString(), "Text" };
        private DragDropEffects _allowedEffects;

        public string[] AllowedDataTypes
        {
            get { return _datatypes; }
            set
            {
                _datatypes = value;

                for (int x = 0; x < _datatypes.Length; x++)
                {
                    _datatypes[x] = _datatypes[x].ToLower();
                }
            }
        }

        public DragDropEffects AllowedEffects
        {
            get { return _allowedEffects; }
            set { _allowedEffects = value; }
        }

        public DropHelper(UIElement wrapper)
        {
            _dropTarget = wrapper;

            _dropTarget.AllowDrop = true;
            _dropTarget.DragOver += new DragEventHandler(DropTarget_DragOver);
            _dropTarget.Drop += new DragEventHandler(DropTarget_Drop);
        }

        private void DropTarget_Drop(object sender, DragEventArgs e)
        {
            IDataObject data = e.Data;
            DragDataWrapper dw = null;
            bool isDataOperation = false;

            Debug.Assert(data != null);

            if (data.GetDataPresent(typeof(DragDataWrapper).ToString()))
            {
                dw = data.GetData(typeof(DragDataWrapper).ToString()) as DragDataWrapper;

                Debug.Assert(dw.Shim != null);

                if ((dw.Shim.SupportedActions & DragDropProviderActions.Data) != 0)
                {
                    isDataOperation = true;
                }
            }

            // Try a BRUTE FORCE APPROACH on UIElement just to show how it could be done
            // BUT NOT ENDORSING IT!!! 
            if (!isDataOperation)
            {
                if (_dropTarget is Canvas)
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        this.DropFileLink(e);
                    }
                    else if (e.Data.GetDataPresent(UrlDataFormat))
                    {
                        this.DropUrl(e);
                    }
                }
            }
            else
            {
                Debug.Assert(dw != null);
                Debug.Assert(dw.Shim != null);

                object rawdata = dw.Data;

                if (_dropTarget is ItemsControl)
                {
                    this.DropItemsControlHandler(e, rawdata);
                }
                else if (_dropTarget is Canvas)
                {
                    if (rawdata is IWidget)
                    {
                        this.DropCanvasWidget(e, rawdata as IWidget);
                    }
                    else if (rawdata is SiteMapNode)
                    {
                        this.DropCanvasInternalLink(e, rawdata as NavigationNode);
                    }
                }
            }

            e.Handled = true;
        }

        private void DropTarget_DragOver(object sender, DragEventArgs e)
        {
            string[] types = e.Data.GetFormats();
            bool match = false;

            if (_datatypes == null || types == null)
            {
                //TODO: ??? Should we set for DragDropEffects.None? 
                return;
            }

            foreach (string s in types)
            {
                foreach (string type in _datatypes)
                {
                    match = (s.ToLower() == type);

                    if (match)
                    {
                        break;
                    }
                }

                if (match)
                {
                    break;
                }
            }

            if (match)
            {
                e.Effects = AllowedEffects;
                e.Handled = true;
            }
        }

        private void DropCanvasWidget(DragEventArgs e, IWidget dropObject)
        {
            ServiceLocator.GetService<IVirtualDesktopManager>()
                          .Show
            (
                dropObject.CreateView() as WidgetElement,
                e.GetPosition(_dropTarget)
            );
        }

        private void DropItemsControlHandler(DragEventArgs e, object rawdata)
        {
            ItemsControl ic = _dropTarget as ItemsControl;
            IList list = ic.ItemsSource as IList;

            if (list == null)
            {
                list = ic.Items as System.Collections.IList;
            }

            if (list != null)
            {
                if (!list.Contains(rawdata))
                {
                    // Here we do not check for Move | Copy ... because this is a DATA operation .. No parent relationshop at all ... 
                    list.Add(rawdata);
                }
                else
                {
                    // nothing was done ... 
                    e.Effects = DragDropEffects.None;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void DropFileLink(DragEventArgs e)
        {
            // External link drop
            Point position = e.GetPosition(_dropTarget);
            string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];

            e.Effects = DragDropEffects.Link;

            foreach (string filename in filenames)
            {
                ServiceLocator.GetService<IVirtualDesktopManager>()
                              .CreateShortcut<ExternalShortcutViewModel>
                (
                    Path.GetFileNameWithoutExtension(filename), filename, position
                );
            }
        }

        private void DropUrl(DragEventArgs e)
        {
            // External link drop
            Point position = e.GetPosition(_dropTarget);
            string data = null;

            using (MemoryStream stream = e.Data.GetData(UrlDataFormat, true) as MemoryStream)
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
                {
                    data = reader.ReadToEnd().Replace("\0", "");
                }
            }

            if (!String.IsNullOrEmpty(data))
            {
                string[] elements = data.Split('\n');

                e.Effects = DragDropEffects.Link;

                if (!String.IsNullOrWhiteSpace(elements[0]) &&
                    !String.IsNullOrWhiteSpace(elements[1]))
                {
                    ServiceLocator.GetService<IVirtualDesktopManager>()
                                  .CreateShortcut<ExternalShortcutViewModel>
                    (
                        elements[1],
                        elements[0],
                        position
                    );
                }
            }
        }

        private void DropCanvasInternalLink(DragEventArgs e, NavigationNode siteMapNode)
        {
            Point position = e.GetPosition(_dropTarget);

            ServiceLocator.GetService<IVirtualDesktopManager>()
                          .CreateShortcut<InternalShortcutViewModel>
            (
                siteMapNode.Title,
                siteMapNode.Url,
                position
            );
        }

        private bool Unparent(DragDataWrapper dw, UIElement uie)
        {
            bool success = false;

            if (dw != null)
            {
                if (dw.AllowChildrenRemove)
                {
                    dw.Shim.UnParent();
                }
            }

            if (!success) // BRUTE FORCE 
            {
                if (uie is FrameworkElement)
                {
                    FrameworkElement fe = uie as FrameworkElement;

                    if (fe.Parent != null)
                    {
                        if (fe.Parent is Panel)
                        {
                            try
                            {
                                ((Panel)(fe.Parent)).Children.Remove(uie);
                                success = true;
                            }
                            catch (Exception)
                            {
#if DEBUG
                                System.Diagnostics.Debug.Assert(false);
#endif
                            }
                        }
                    }
                    else if (fe.Parent is ContentControl)
                    {
                        ContentControl cc = fe.Parent as ContentControl;

                        cc.Content = null;
                        success = true;
                    }
                }
            }

            return success;
        }
    }
}
