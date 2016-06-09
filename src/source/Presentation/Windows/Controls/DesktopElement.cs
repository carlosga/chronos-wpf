// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Chronos.Extensions.Windows;
using Chronos.Presentation.Core.Windows;

namespace Chronos.Presentation.Windows.Controls
{
    /// <summary>
    /// Base class for <see cref="Desktop"/> elements (shortcuts, widgets,...)
    /// </summary>
    [TemplatePart(Name = DesktopElement.PART_Dragger, Type = typeof(FrameworkElement))]
    public abstract class DesktopElement
        : ContentControl, IDesktopElement, IActiveAware
    {
        #region · Constants ·

        protected const string PART_Dragger = "PART_Dragger";
        protected const int ResizeSideThichness = 5;
        protected const int ResizeCornerSize = 5;

        #endregion

        #region · Dependency Properties ·

        /// <summary>
        /// Identifies the Id dependency property.
        /// </summary>
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(Guid), typeof(DesktopElement),
                new FrameworkPropertyMetadata(Guid.NewGuid()));

        /// <summary>
        /// Identifies the WindowStartupLocation dependency property.
        /// </summary>
        public static readonly DependencyProperty StartupLocationProperty =
            DependencyProperty.Register("StartupLocation", typeof(StartupPosition), typeof(WindowElement),
                new FrameworkPropertyMetadata(StartupPosition.CenterParent, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Identifies the CanResize dependency property.
        /// </summary>
        public static readonly DependencyProperty CanResizeProperty =
            DependencyProperty.Register("CanResize", typeof(bool), typeof(DesktopElement),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the CanDrag dependency property.
        /// </summary>
        public static readonly DependencyProperty CanDragProperty =
            DependencyProperty.Register("CanDrag", typeof(bool), typeof(DesktopElement),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the CanClose dependency property.
        /// </summary>
        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof(bool), typeof(DesktopElement),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the ConstraintToParent dependency property.
        /// </summary>
        public static readonly DependencyProperty ConstraintToParentProperty =
            DependencyProperty.Register("ConstraintToParent", typeof(bool), typeof(DesktopElement),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies the IsActive dependency property
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
          DependencyProperty.Register("IsActive",
                                       typeof(bool),
                                       typeof(DesktopElement),
                                       new FrameworkPropertyMetadata(false));

        #endregion

        #region · Static Constructors ·

        /// <summary>
        /// Initializes the <see cref="DesktopElement"/> class.
        /// </summary>
        static DesktopElement()
        {
            // set the key to reference the style for this control
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DesktopElement), new FrameworkPropertyMetadata(typeof(DesktopElement)));
        }

        #endregion

        #region · Static Routed Events ·

        /// <summary>
        /// Occurs when a desktop element is activated
        /// </summary>
        public static readonly RoutedEvent ActivatedEvent = EventManager.
            RegisterRoutedEvent("Activated", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(DesktopElement));

        /// <summary>
        /// Occurs when a desktop element is deactivated
        /// </summary>
        public static readonly RoutedEvent DeactivatedEvent = EventManager.
            RegisterRoutedEvent("Deactivated", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(DesktopElement));

        #endregion

        #region · Events ·

        /// <summary>
        /// Occurs when the element becomes activated
        /// </summary>
        public event RoutedEventHandler Activated
        {
            add { base.AddHandler(DesktopElement.ActivatedEvent, value); }
            remove { base.RemoveHandler(DesktopElement.ActivatedEvent, value); }
        }

        /// <summary>
        /// Occurs when the element becomes deactivated
        /// </summary>
        public event RoutedEventHandler Deactivated
        {
            add { base.AddHandler(DesktopElement.DeactivatedEvent, value); }
            remove { base.RemoveHandler(DesktopElement.DeactivatedEvent, value); }
        }

        #endregion

        #region · Fields ·

        private Panel _parent;
        private FrameworkElement _partDragger;
        private DragOrResizeStatus _previewDragOrResizeStatus;
        private DragOrResizeStatus _dragOrResizeStatus;
        private Point _startMousePosition;
        private Point _previousMousePosition;
        private Point _oldPosition;
        private Size _originalSize;
        private Size _previousSize;
        private bool _isInitialized;
        private bool _oldCanResize;

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

        /// <summary>
        /// Gets the window parent control
        /// </summary>
        public new Panel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <summary>
        /// Gets or sets the position of the window when first shown.
        /// </summary>
        public StartupPosition StartupLocation
        {
            get { return (StartupPosition)base.GetValue(StartupLocationProperty); }
            set { base.SetValue(StartupLocationProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element can be resized
        /// </summary>
        public bool CanResize
        {
            get { return (bool)base.GetValue(CanResizeProperty); }
            set { base.SetValue(CanResizeProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the element can be dragged.
        /// </summary>
        /// <value><c>true</c> if this instance can drag; otherwise, <c>false</c>.</value>
        public virtual bool CanDrag
        {
            get { return (bool)base.GetValue(CanDragProperty); }
            set { base.SetValue(CanDragProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the element can be closed.
        /// </summary>
        /// <value><c>true</c> if this instance can be closed; otherwise, <c>false</c>.</value>
        public virtual bool CanClose
        {
            get { return (bool)base.GetValue(CanCloseProperty); }
            set { base.SetValue(CanCloseProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the element should be constrained to parent.
        /// </summary>
        /// <value><c>true</c> if this instance can drag; otherwise, <c>false</c>.</value>
        public virtual bool ConstraintToParent
        {
            get { return (bool)base.GetValue(ConstraintToParentProperty); }
            set { base.SetValue(ConstraintToParentProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element is active.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsActive
        {
            get { return (bool)base.GetValue(IsActiveProperty); }
        }

        #endregion

        #region · Protected Properties ·

        /// <summary>
        /// Gets the current drag or resize status
        /// </summary>
        protected DragOrResizeStatus DragOrResizeStatus
        {
            get { return _dragOrResizeStatus; }
        }

        /// <summary>
        /// Gets or sets the old element position.
        /// </summary>
        protected Point OldPosition
        {
            get { return _oldPosition; }
            set { _oldPosition = value; }
        }

        /// <summary>
        /// Gets or sets the original element size
        /// </summary>
        protected Size OriginalSize
        {
            get { return _originalSize; }
            set { _originalSize = value; }
        }

        /// <summary>
        /// Gets the parent <see cref="Desktop"/>.
        /// </summary>
        /// <value>The parent desktop.</value>
        protected Desktop ParentDesktop
        {
            //get { return this.GetParent<Desktop>(); }
            get { return _parent as Desktop; }
        }

        protected FrameworkElement PartDragger
        {
            get { return _partDragger; }
            set { _partDragger = value; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopElement"/> class.
        /// </summary>
        protected DesktopElement()
        {
            _previewDragOrResizeStatus = DragOrResizeStatus.None;
            _dragOrResizeStatus = DragOrResizeStatus.None;
            _startMousePosition = new Point();
            _oldCanResize = this.CanResize;
        }

        #endregion

        #region  · Methods ·

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PartDragger = this.GetTemplateChild(DesktopElement.PART_Dragger) as FrameworkElement;
        }

        /// <summary>
        /// Attempts to bring the element to the foreground and activates it.
        /// </summary>
        public void Activate()
        {
            this.OnActivated();
        }

        /// <summary>
        /// Deactivates the element
        /// </summary>
        public void Deactivate()
        {
            this.OnDeactivated();
        }

        /// <summary>
        /// Closes the desktop element
        /// </summary>
        public virtual void Close()
        {
            if (this.Parent != null)
            {
                this.Parent.Children.Remove(this);
            }

            // Clean up
            this.Id = Guid.Empty;
            _previewDragOrResizeStatus = DragOrResizeStatus.None;
            _dragOrResizeStatus = DragOrResizeStatus.None;
            _startMousePosition = new Point();
            _previousMousePosition = new Point();
            _oldPosition = new Point();
            _previousSize = Size.Empty;
            _originalSize = Size.Empty;
            _isInitialized = false;
            _oldCanResize = false;
            _partDragger = null;
            _parent = null;
            this.Content = null;
            this.DataContext = null;
        }

        #endregion

        #region · Protected Methods ·

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged"/> event, using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (!_isInitialized)
            {
                this.RefreshCalculatedVisualProperties();

                this.OriginalSize = new Size(this.ActualWidth, this.ActualHeight);
                _isInitialized = true;
            }
        }

        protected void RefreshCalculatedVisualProperties()
        {
            if (!DesignMode.IsInDesignMode)
            {
                switch (this.StartupLocation)
                {
                    case StartupPosition.CenterParent:
                        if (!this.ConstraintToParent &&
                            this.GetParent<Window>() != null)
                        {
                            this.MoveElement
                            (
                                (this.GetParent<Window>().ActualWidth - this.ActualWidth) / 2,
                                (this.GetParent<Window>().ActualHeight - this.ActualHeight) / 2 - (this.GetParent<Window>().ActualHeight - this.Parent.ActualHeight)
                            );
                        }
                        else if (this.Parent != null)
                        {
                            this.MoveElement
                            (
                                (this.Parent.ActualWidth - this.ActualWidth) / 2,
                                (this.Parent.ActualHeight - this.ActualHeight) / 2
                            );
                        }
                        break;

                    case StartupPosition.Manual:
                        break;

                    case StartupPosition.WindowsDefaultLocation:
                        this.MoveElement(5, 5);
                        break;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="DesktopElement.Activated"/> event.
        /// </summary>
        protected virtual void OnActivated()
        {
            if (!(bool)this.GetValue(DesktopElement.IsActiveProperty))
            {
                if (this.ParentDesktop != null)
                {
                    this.ParentDesktop.OnActivatedElement(this.Id);
                    this.ParentDesktop.BringToFront(this);
                }

                this.GiveFocus();

                base.SetValue(DesktopElement.IsActiveProperty, true);

                RoutedEventArgs e = new RoutedEventArgs(DesktopElement.ActivatedEvent, this);
                base.RaiseEvent(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DesktopElement.Deactivated"/> event.
        /// </summary>
        protected virtual void OnDeactivated()
        {
            if (this.IsActive)
            {
                base.SetValue(DesktopElement.IsActiveProperty, false);

                RoutedEventArgs e = new RoutedEventArgs(DesktopElement.DeactivatedEvent, this);
                base.RaiseEvent(e);
            }
        }

        /// <summary>
        /// Focuses the element
        /// </summary>
        protected virtual void GiveFocus()
        {
            this.SetFocus();
        }

        #endregion

        #region · Protected Element Move/Resize Methods ·

        protected virtual void MoveElement(double x, double y)
        {
            this.Move(x, y);
        }

        protected virtual void MoveElementLeft(double value)
        {
            this.MoveLeft(value);
        }

        protected virtual void MoveElementTop(double value)
        {
            this.MoveTop(value);
        }

        protected virtual void AdjustBounds(Point mousePosition)
        {
            Point position = this.GetPosition();

            if (this.Parent != null)
            {
                Vector changeFromStart = Point.Subtract(mousePosition, _startMousePosition);

                if (_dragOrResizeStatus == DragOrResizeStatus.Drag)
                {
                    if (this.CanDrag)
                    {
                        double x = position.X + changeFromStart.X;
                        double y = position.Y + changeFromStart.Y;

                        if (this.ConstraintToParent)
                        {
                            if (x < 0)
                            {
                                x = 0;
                            }
                            if (y < 0)
                            {
                                y = 0;
                            }
                            if (y + this.ActualHeight > this.ParentDesktop.ActualHeight)
                            {
                                y = this.ParentDesktop.ActualHeight - this.ActualHeight;
                            }
                            if (x + this.ActualWidth > this.ParentDesktop.ActualWidth)
                            {
                                x = this.ParentDesktop.ActualWidth - this.ActualWidth;
                            }
                        }

                        if (x != position.X || y != position.Y)
                        {
                            this.MoveElement(x, y);
                        }
                    }
                }
                else
                {
                    Size size = this.RenderSize;
                    Vector changeFromPrevious = Point.Subtract(mousePosition, _previousMousePosition);

                    if (_dragOrResizeStatus.IsOnRight)
                    {
                        if (size.Width + changeFromPrevious.X > this.MinWidth)
                        {
                            size.Width += changeFromPrevious.X;
                        }
                    }
                    else if (_dragOrResizeStatus.IsOnLeft)
                    {
                        if (size.Width - changeFromStart.X > this.MinWidth)
                        {
                            this.MoveElementLeft(position.X + changeFromStart.X);
                            size.Width -= changeFromStart.X;
                        }
                    }

                    if (_dragOrResizeStatus.IsOnBottom)
                    {
                        if (size.Height + changeFromPrevious.Y > this.MinHeight)
                        {
                            size.Height += changeFromPrevious.Y;
                        }
                    }
                    else if (_dragOrResizeStatus.IsOnTop)
                    {
                        if (size.Height - changeFromStart.Y > this.MinHeight)
                        {
                            this.MoveElementTop(position.Y + changeFromStart.Y);
                            size.Height -= changeFromStart.Y;
                        }
                    }

                    this.Width = size.Width;
                    this.Height = size.Height;
                }
            }
        }

        #endregion

        #region · Mouse Handling Methods ·

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseDown"/> attached routed event 
        /// reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. 
        /// The event data reports that one or more mouse buttons were pressed.
        /// </param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (!this.IsActive)
            {
                this.OnActivated();
            }

            base.OnPreviewMouseDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonDown"/> routed event 
        /// reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. 
        /// The event data reports that the left mouse button was pressed.
        /// </param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled
                && e.ClickCount == 1
                && e.Source == this)
            {
                if (_dragOrResizeStatus == DragOrResizeStatus.None &&
                    _previewDragOrResizeStatus != DragOrResizeStatus.None)
                {
                    e.Handled = true;

                    _dragOrResizeStatus = _previewDragOrResizeStatus;
                    _startMousePosition = _previousMousePosition = e.GetPosition(this);

                    this.CaptureMouse();
                }
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseMove"/> attached event 
        /// reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (_dragOrResizeStatus == DragOrResizeStatus.None)
            {
                // http://www.switchonthecode.com/tutorials/wpf-snippet-reliably-getting-the-mouse-position
                Point point = e.GetPosition(this);

                _previewDragOrResizeStatus = this.GetDragOrResizeMode(point);

                if (!this.CanResize
                    && _previewDragOrResizeStatus != DragOrResizeStatus.Drag
                    && _previewDragOrResizeStatus != DragOrResizeStatus.None)
                {
                    _previewDragOrResizeStatus = DragOrResizeStatus.None;
                }

                this.SetResizeCursor(_previewDragOrResizeStatus);
            }
            else if (this.IsMouseCaptured)
            {
                if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {
                    // http://www.switchonthecode.com/tutorials/wpf-snippet-reliably-getting-the-mouse-position
                    Point point = e.GetPosition(this);

                    if (Math.Abs(point.X - _previousMousePosition.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(point.Y - _previousMousePosition.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        e.Handled = true;

                        this.AdjustBounds(point);
                        _previousMousePosition = point;
                    }
                }
                else
                {
                    this.CancelDragOrResize();
                }
            }
            else
            {
                this.CancelDragOrResize();
            }

            base.OnPreviewMouseMove(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> routed event reaches an element 
        /// in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports 
        /// that the left mouse button was released.
        /// </param>
        protected override void OnPreviewMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                e.Handled = true;
                this.CancelDragOrResize();
            }

            base.OnPreviewMouseLeftButtonUp(e);
        }

        #endregion

        #region · Drag and Resize Methods ·

        private DragOrResizeStatus GetDragOrResizeMode(Point position)
        {
            DragOrResizeStatus status = DragOrResizeStatus.None;

            if (this.CanDrag &&
                _partDragger != null &&
                _partDragger.IsMouseOver)
            {
                status = DragOrResizeStatus.Drag;
            }
            else if (this.CanResize)
            {
                if (position.X <= ResizeSideThichness) // left
                {
                    status = this.GetLeftDragStatus(position);
                }
                else if (this.ActualWidth - position.X <= ResizeSideThichness) // right
                {
                    status = this.GetRightDragStatus(position);
                }
                else if (position.Y <= ResizeSideThichness) // top
                {
                    status = this.GetTopDragStatus(position);
                }
                else if (this.ActualHeight - position.Y <= ResizeSideThichness) // bottom
                {
                    status = this.GetBottomDragStatus(position);
                }
            }

            return status;
        }

        private DragOrResizeStatus GetBottomDragStatus(Point position)
        {
            DragOrResizeStatus status;

            if (position.X <= ResizeCornerSize)
            {
                status = DragOrResizeStatus.BottomLeft;
            }
            else if (this.ActualWidth - position.X <= ResizeCornerSize)
            {
                status = DragOrResizeStatus.BottomRight;
            }
            else
            {
                status = DragOrResizeStatus.BottomCenter;
            }

            return status;
        }

        private DragOrResizeStatus GetTopDragStatus(Point position)
        {
            DragOrResizeStatus status;

            if (position.X <= ResizeCornerSize)
            {
                status = DragOrResizeStatus.TopLeft;
            }
            else if (this.ActualWidth - position.X <= ResizeCornerSize)
            {
                status = DragOrResizeStatus.TopRight;
            }
            else
            {
                status = DragOrResizeStatus.TopCenter;
            }

            return status;
        }

        private DragOrResizeStatus GetRightDragStatus(Point position)
        {
            DragOrResizeStatus status;

            if (position.Y <= ResizeCornerSize)
            {
                status = DragOrResizeStatus.TopRight;
            }
            else if (this.ActualHeight - position.Y <= ResizeCornerSize)
            {
                status = DragOrResizeStatus.BottomRight;
            }
            else
            {
                status = DragOrResizeStatus.MiddleRight;
            }

            return status;
        }

        private DragOrResizeStatus GetLeftDragStatus(Point position)
        {
            DragOrResizeStatus status;

            if (position.Y <= ResizeCornerSize)
            {
                status = DragOrResizeStatus.TopLeft;
            }
            else if (this.ActualHeight - position.Y <= ResizeCornerSize)
            {
                status = DragOrResizeStatus.BottomLeft;
            }
            else
            {
                status = DragOrResizeStatus.MiddleLeft;
            }

            return status;
        }

        private void CancelDragOrResize()
        {
            this.Cursor = null;
            _dragOrResizeStatus = DragOrResizeStatus.None;
            _previewDragOrResizeStatus = DragOrResizeStatus.None;

            this.ReleaseMouseCapture();
        }

        private void SetResizeCursor(DragOrResizeStatus resizeStatus)
        {
            if (this.CanResize || this.CanDrag)
            {
                if (resizeStatus.IsDragging)
                {
                    this.Cursor = null;
                }
                else if (resizeStatus.IsOnTopLeftOrBottomRight)
                {
                    this.Cursor = Cursors.SizeNWSE;
                }
                else if (resizeStatus.IsOnTopRightOrBottomLeft)
                {
                    this.Cursor = Cursors.SizeNESW;
                }
                else if (resizeStatus.IsOnTopRightOrBottomLeft)
                {
                    this.Cursor = Cursors.SizeNESW;
                }
                else if (resizeStatus.IsOnTopCenterOrBottomCenter)
                {
                    this.Cursor = Cursors.SizeNS;
                }
                else if (resizeStatus.IsOnMiddleLeftOrMiddleRight)
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else if (this.Cursor != null)
                {
                    this.Cursor = null;
                }
            }
            else if (this.Cursor != null)
            {
                this.Cursor = null;
            }
        }

        #endregion
    }
}
