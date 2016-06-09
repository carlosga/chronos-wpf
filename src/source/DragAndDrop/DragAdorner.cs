// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Chronos.Presentation.DragAndDrop
{
    public sealed class DragAdorner
        : Adorner
    {
        private UIElement _child;
        private UIElement _owner;
        private double _XCenter;
        private double _YCenter;
        private double _topOffset;
        private double _leftOffset;

        public double LeftOffset
        {
            get { return _leftOffset; }
            set
            {
                _leftOffset = (value - _XCenter);
                this.UpdatePosition();
            }
        }

        public double TopOffset
        {
            get { return _topOffset; }
            set
            {
                _topOffset = (value - _YCenter);
                this.UpdatePosition();
            }
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public DragAdorner(UIElement owner)
            : base(owner)
        {
        }

        public DragAdorner(UIElement owner, UIElement adornElement, bool useVisualBrush, double opacity)
            : base(owner)
        {
            _owner = owner;

            if (useVisualBrush)
            {
                VisualBrush brush = new VisualBrush(adornElement);
                Rectangle rect = new Rectangle();

                brush.Opacity = opacity;
                rect.RadiusX = 3;
                rect.RadiusY = 3;
                rect.Width = adornElement.DesiredSize.Width;
                rect.Height = adornElement.DesiredSize.Height;

                _XCenter = adornElement.DesiredSize.Width / 2;
                _YCenter = adornElement.DesiredSize.Height / 2;

                rect.Fill = brush;

                _child = rect;
            }
            else
            {
                _child = adornElement;
            }
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();

            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));

            return result;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        protected override Size MeasureOverride(Size finalSize)
        {
            _child.Measure(finalSize);

            return _child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(_child.DesiredSize));

            return finalSize;
        }

        private void UpdatePosition()
        {
            AdornerLayer adorner = (AdornerLayer)this.Parent;

            if (adorner != null)
            {
                adorner.Update(this.AdornedElement);
            }
        }
    }
}
