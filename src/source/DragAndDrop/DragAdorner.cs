// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Chronos.Presentation.DragAndDrop
{
    public sealed class DragAdorner : Adorner
    {
        #region � Fields �

        private UIElement   child;
        private UIElement   owner;
        private double      XCenter;
        private double      YCenter;
        private double      topOffset;
        private double      leftOffset;

        #endregion

        #region � Properties �

        public double LeftOffset
        {
            get { return this.leftOffset; }
            set
            {
                this.leftOffset = (value - this.XCenter);
                this.UpdatePosition();
            }
        }
        
        public double TopOffset
        {
            get { return this.topOffset; }
            set
            {
                this.topOffset = (value - this.YCenter);
                this.UpdatePosition();
            }
        }

        #endregion

        #region � Protected Properties �

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        #endregion

        #region � Constructors �

        public DragAdorner(UIElement owner)
            : base(owner)
        {
        }

        public DragAdorner(UIElement owner, UIElement adornElement, bool useVisualBrush, double opacity)
            : base(owner)
        {
            this.owner  = owner;

            if (useVisualBrush)
            {
                VisualBrush brush   = new VisualBrush(adornElement);
                Rectangle   rect    = new Rectangle();

                brush.Opacity       = opacity;
                rect.RadiusX        = 3;
                rect.RadiusY        = 3;
                rect.Width          = adornElement.DesiredSize.Width;
                rect.Height         = adornElement.DesiredSize.Height;

                this.XCenter        = adornElement.DesiredSize.Width / 2;
                this.YCenter        = adornElement.DesiredSize.Height / 2;

                rect.Fill = brush;

                this.child = rect;
            }
            else
            {
                this.child = adornElement;
            }
        }

        #endregion

        #region � Methods �

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();

            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.leftOffset, this.topOffset));

            return result;
        }

        #endregion

        #region � Protected Methods �

        protected override Visual GetVisualChild(int index)
        {
            return this.child;
        }

        protected override Size MeasureOverride(Size finalSize)
        {
            this.child.Measure(finalSize);
            
            return this.child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.child.Arrange(new Rect(this.child.DesiredSize));

            return finalSize;
        }

        #endregion

        #region � Private Methods �

        private void UpdatePosition()
        {
            AdornerLayer adorner = (AdornerLayer)this.Parent;

            if (adorner != null)
            {
                adorner.Update(this.AdornedElement);
            }
        }

        #endregion
    }
}
