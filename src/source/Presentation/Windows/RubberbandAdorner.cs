// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Chronos.Presentation.Windows.Controls;

namespace Chronos.Presentation.Windows
{
    /// <summary>
    /// Addorner for the rubber band selection
    /// </summary>
    public class RubberbandAdorner : Adorner
    {
        #region · Fields ·

        private Point? _startPoint;
        private Point? _endPoint;
        private Pen _rubberbandPen;
        private Desktop _desktop;
        private Brush _backgroundBrush;

        #endregion        

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="RubberbandAdorner"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="dragStartPoint">The drag start point.</param>
        public RubberbandAdorner(Desktop canvas, Point? dragStartPoint)
            : base(canvas)
        {
            ColorConverter cconverter = new ColorConverter();

            _desktop = canvas;
            _startPoint = dragStartPoint;
            _rubberbandPen = new Pen(new SolidColorBrush((Color)cconverter.ConvertFrom("#FF7AA3D4")), 1);
            _rubberbandPen.DashStyle = new DashStyle();
            _backgroundBrush = new SolidColorBrush((Color)cconverter.ConvertFrom("#FFC5D5E9"));
            _backgroundBrush.Opacity = 0.40;
        }

        #endregion

        #region · Protected Methods ·

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured)
                {
                    this.CaptureMouse();
                }

                _endPoint = e.GetPosition(this);

                this.UpdateSelection();
                this.InvalidateVisual();
            }
            else
            {
                if (this.IsMouseCaptured)
                {
                    this.ReleaseMouseCapture();
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/> routed event 
        /// reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. 
        /// The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            // release mouse capture
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }

            // remove this adorner from adorner layer
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_desktop);
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this);
            }

            e.Handled = true;
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // without a background the OnMouseMove event would not be fired!
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

            if (_startPoint.HasValue && _endPoint.HasValue)
            {
                drawingContext.DrawRectangle(_backgroundBrush, _rubberbandPen, new Rect(_startPoint.Value, _endPoint.Value));
            }
        }

        #endregion

        #region · Private Methods ·

        private void UpdateSelection()
        {
            _desktop.SelectionService.ClearSelection();

            Rect rubberBand = new Rect(_startPoint.Value, _endPoint.Value);

            foreach (UIElement item in _desktop.Children)
            {
                Rect itemRect = VisualTreeHelper.GetDescendantBounds(item);
                Rect itemBounds = item.TransformToAncestor(_desktop).TransformBounds(itemRect);

                if (rubberBand.Contains(itemBounds))
                {
                    if (item is ISelectable)
                    {
                        ISelectable di = item as ISelectable;

                        if (di.ParentId == Guid.Empty)
                        {
                            _desktop.SelectionService.AddToSelection(di);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
