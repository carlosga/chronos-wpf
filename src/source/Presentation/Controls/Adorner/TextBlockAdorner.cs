using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chronos.Presentation.Controls.Adorner
{
    public sealed class TextBlockAdorner
        : System.Windows.Documents.Adorner
    {
        private readonly TextBlock _adornerTextBlock;

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public TextBlockAdorner(UIElement adornedElement, string label, Style labelStyle)
            : base(adornedElement)
        {
            _adornerTextBlock = new TextBlock { Style = labelStyle, Text = label };
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _adornerTextBlock.Measure(constraint);

            return constraint;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _adornerTextBlock.Arrange(new Rect(finalSize));

            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _adornerTextBlock;
        }
    }
}
