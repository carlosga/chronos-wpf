// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Chronos.Presentation.DragAndDrop
{
    public sealed class DragDropManager
    {
        #region · Singleton Instance ·

        public static readonly DragDropManager Instance = new DragDropManager();

        #endregion

        #region · Attached Properties ·

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

        #endregion

        #region · Dependency Property Get/Set Methods ·

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

        #endregion

        #region · Dependency Property Callbacks ·

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

        #endregion

        #region · Fields ·

        private Dictionary<FrameworkElement, DropHelper> dropTargets;
        private Dictionary<FrameworkElement, DragHelper> dragSources;

        #endregion

        #region · Constructors ·

        private DragDropManager()
        {
            this.dropTargets = new Dictionary<FrameworkElement, DropHelper>();
            this.dragSources = new Dictionary<FrameworkElement, DragHelper>();
        }

        #endregion

        #region · Private Methods ·

        private void AttachDropTarget(FrameworkElement element)
        {
            if (!this.dropTargets.ContainsKey(element))
            {
                element.Unloaded += new RoutedEventHandler(DragDropManager_Unloaded);
                this.dropTargets.Add(element, new DropHelper(element));
            }
        }

        private void AttachDragSource(FrameworkElement element)
        {
            if (!this.dragSources.ContainsKey(element))
            {
                element.Unloaded += new RoutedEventHandler(DragDropManager_Unloaded);

                if (element is ListBox)
                {
                    this.dragSources.Add(element, new DragHelper(element, new ListBoxDragDropDataProvider(element as ListBox), null));
                }
                else if (element is TreeView)
                {
                    this.dragSources.Add(element, new DragHelper(element, new TreeViewDragDropDataProvider(element as TreeView), null));
                }
            }
        }

        #endregion

        #region · Event Handlers ·

        private void DragDropManager_Unloaded(object sender, RoutedEventArgs e)
        {
            (sender as FrameworkElement).Unloaded -= new RoutedEventHandler(DragDropManager_Unloaded);
                            
            if (this.dragSources.ContainsKey(sender as FrameworkElement))
            {
                this.dragSources.Remove(sender as FrameworkElement);
            }
            else if (this.dropTargets.ContainsKey(sender as FrameworkElement))
            {
                this.dropTargets.Remove(sender as FrameworkElement);
            }
        }

        #endregion
    }
}
