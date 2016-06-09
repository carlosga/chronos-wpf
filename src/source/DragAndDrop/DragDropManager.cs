// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Chronos.Presentation.DragAndDrop
{
    public sealed class DragDropManager
    {
        public static readonly DragDropManager Instance = new DragDropManager();

        /// <summary>
        /// Identifies the IsDesktopCanvas dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDropTargetProperty =
            DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(DragDropManager),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(OnIsDropTarget)));

        /// <summary>
        /// Identifies the IsDragSource dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDragSourceProperty =
            DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDropManager),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(OnIsDragSource)));

        /// <summary>
        /// Gets the value of the IsDropTarget attached property
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool GetIsDropTarget(DependencyObject d)
        {
            return (bool)d.GetValue(IsDropTargetProperty);
        }

        /// <summary>
        /// Sets the value of the IsDropTarget attached property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        public static void SetIsDropTarget(DependencyObject d, bool value)
        {
            d.SetValue(IsDropTargetProperty, value);
        }

        /// <summary>
        /// Gets the value of the IsDragSource attached property
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool GetIsDragSource(DependencyObject d)
        {
            return (bool)d.GetValue(IsDragSourceProperty);
        }

        /// <summary>
        /// Sets the value of the IsDragSource attached property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        public static void SetIsDragSource(DependencyObject d, bool value)
        {
            d.SetValue(IsDragSourceProperty, value);
        }

        private static void OnIsDropTarget(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;

            if (element != null)
            {
                DragDropManager.Instance.AttachDropTarget(element);
            }
        }

        private static void OnIsDragSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;

            if (element != null)
            {
                DragDropManager.Instance.AttachDragSource(element);
            }
        }

        private Dictionary<FrameworkElement, DropHelper> _dropTargets;
        private Dictionary<FrameworkElement, DragHelper> _dragSources;

        private DragDropManager()
        {
            _dropTargets = new Dictionary<FrameworkElement, DropHelper>();
            _dragSources = new Dictionary<FrameworkElement, DragHelper>();
        }

        private void AttachDropTarget(FrameworkElement element)
        {
            if (!_dropTargets.ContainsKey(element))
            {
                element.Unloaded += new RoutedEventHandler(DragDropManager_Unloaded);
                _dropTargets.Add(element, new DropHelper(element));
            }
        }

        private void AttachDragSource(FrameworkElement element)
        {
            if (!_dragSources.ContainsKey(element))
            {
                element.Unloaded += new RoutedEventHandler(DragDropManager_Unloaded);

                if (element is ListBox)
                {
                    _dragSources.Add(element, new DragHelper(element, new ListBoxDragDropDataProvider(element as ListBox), null));
                }
                else if (element is TreeView)
                {
                    _dragSources.Add(element, new DragHelper(element, new TreeViewDragDropDataProvider(element as TreeView), null));
                }
            }
        }

        private void DragDropManager_Unloaded(object sender, RoutedEventArgs e)
        {
            (sender as FrameworkElement).Unloaded -= new RoutedEventHandler(DragDropManager_Unloaded);

            if (_dragSources.ContainsKey(sender as FrameworkElement))
            {
                _dragSources.Remove(sender as FrameworkElement);
            }
            else if (_dropTargets.ContainsKey(sender as FrameworkElement))
            {
                _dropTargets.Remove(sender as FrameworkElement);
            }
        }
    }
}
