// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Chronos.Extensions.Windows;
using Chronos.Presentation.Core.ViewModel;
using Chronos.Presentation.ViewModel;

namespace Chronos.Presentation.Windows.Controls
{
    /// <summary>
    /// Virtual desktop representation using a <see cref="Canvas"/> control
    /// </summary>
    public class Desktop
        : Canvas
    {
        #region · Dependency Properties ·

        /// <summary>
        /// Identifies the Id dependency property.
        /// </summary>
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(Guid), typeof(Desktop),
                new FrameworkPropertyMetadata(Guid.NewGuid()));

        #endregion

        #region · Routed Commands ·

        public static RoutedCommand GroupCommand = new RoutedCommand();

        #endregion

        #region · Static Constructor ·

        /// <summary>
        /// Initializes the <see cref="Desktop"/> class.
        /// </summary>
        static Desktop()
        {
            Desktop.DefaultStyleKeyProperty.OverrideMetadata(typeof(Desktop),
                new FrameworkPropertyMetadata(typeof(Desktop)));

            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(
                typeof(Desktop), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        #endregion

        #region · Fields ·

        private Point? _rubberbandSelectionStartPoint;
        private SelectionService _selectionService;

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets or sets the element id.
        /// </summary>
        /// <value>The id.</value>
        public Guid Id
        {
            get { return (Guid)base.GetValue(IdProperty); }
            set { base.SetValue(IdProperty, value); }
        }

        #endregion

        #region · Internal Properties ·

        internal SelectionService SelectionService
        {
            get
            {
                if (_selectionService == null)
                {
                    _selectionService = new SelectionService(this);
                }

                return _selectionService;
            }
        }

        #endregion

        #region · Constructor ·

        /// <summary>
        /// Initializes a new instance of the <see cref="Desktop"/> class.
        /// </summary>
        public Desktop()
            : base()
        {
            this.CommandBindings.Add(new CommandBinding(Desktop.GroupCommand, GroupExecuted, CanGroup));
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Adds the new desktop element.
        /// </summary>
        /// <param name="instance">The new desktop element.</param>
        /// <param name="position">The position.</param>
        public void AddElement(DesktopElement element)
        {
            element.Parent = this;

            this.Children.Add(element);

            this.DeactivateAll();
            element.Activate();
        }

        /// <summary>
        /// Adds the new desktop element.
        /// </summary>
        /// <param name="instance">The new desktop element.</param>
        /// <param name="position">The position.</param>
        public void AddElement(DesktopElement element, Point position)
        {
            element.Parent = this;

            this.Children.Add(element);

            element.Move(Math.Max(0, position.X), Math.Max(0, position.Y));
            element.SetZIndex(0);

            this.DeactivateAll();
            element.Activate();
        }

        /// <summary>
        /// Removes the given element from the desktop.
        /// </summary>
        /// <param name="element">The element.</param>
        public void RemoveElement(DesktopElement instance)
        {
            Debug.Assert(instance is UIElement, "instance");

            this.Children.Remove(instance as UIElement);
        }

        public void Activate()
        {
            // Deactivate all Desktop elements
            this.DeactivateAll();

            this.SetFocus();
        }

        #endregion

        #region  · Mouse Handling Methods ·

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event 
        /// reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> 
        /// that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.
        /// </param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (Object.ReferenceEquals(e.Source, this))
            {
                this.Activate();

                // in case that this click is the start of a 
                // drag operation we cache the start point
                _rubberbandSelectionStartPoint = new Point?(e.GetPosition(this));

                // if you click directly on the canvas all 
                // selected items are 'de-selected'
                SelectionService.ClearSelection();

                e.Handled = true;
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> attached event 
        /// reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                _rubberbandSelectionStartPoint = null;
            }
            else if (_rubberbandSelectionStartPoint.HasValue)   // ... but if mouse button is pressed and start
                                                                // point value is set we do have one
            {
                // create rubberband adorner
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

                if (adornerLayer != null)
                {
                    RubberbandAdorner adorner = new RubberbandAdorner(this, _rubberbandSelectionStartPoint);

                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }

                e.Handled = true;
            }
        }

        #endregion

        #region · Internal Methods ·

        /// <summary>
        /// Called when a <see cref="DesktopElement"/> gets activated.
        /// </summary>
        /// <param name="id">The id.</param>
        internal void OnActivatedElement(Guid id)
        {
            this.InvokeAsynchronouslyInBackground
            (
                () =>
                {
                    this.Children.OfType<DesktopElement>()
                        .Where(e => e.Id != id)
                        .ToList()
                        .ForEach(element => element.Deactivate());
                }
            );
        }

        #endregion

        #region · Private Methods ·

        private void GroupExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var items = from item in this.SelectionService.CurrentSelection.OfType<DesktopElement>()
                        select item;

            ShortcutGroupElement group = new ShortcutGroupElement();
            ShortcutGroupViewModel vm = new ShortcutGroupViewModel();
            Point position = items.First().GetPosition();

            foreach (DesktopElement item in items)
            {
                vm.Shortcuts.Add(item.DataContext as IShortcutViewModel);

                this.RemoveElement(item);
            }

            group.DataContext = vm;

            this.SelectionService.ClearSelection();

            this.InvokeAsynchronouslyInBackground
            (
                () =>
                {
                    this.AddElement(group, position);
                }
            );
        }

        private void CanGroup(object sender, CanExecuteRoutedEventArgs e)
        {
            int count = (from item in this.SelectionService.CurrentSelection.OfType<ISelectable>()
                         select item).Count();

            e.CanExecute = (count > 1);
        }

        private void DeactivateAll()
        {
            this.InvokeAsynchronouslyInBackground
            (
                () =>
                {
                    this.SelectionService.ClearSelection();

                    this.Children
                        .OfType<DesktopElement>()
                        .ToList()
                        .ForEach(element => element.Deactivate());
                }
            );
        }

        #endregion
    }
}
