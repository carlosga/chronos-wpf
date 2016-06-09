// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chronos.Extensions.Windows
{
    /// <summary>
    /// Extension methods for the System.Windows.UIElement class
    /// </summary>
    public static class UIElementExtensions
    {
        /// <summary>
        /// Gets the parent of an <see cref="UIElement"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static T GetParent<T>(this UIElement element) where T : UIElement
        {
            // Walk the VisualTree to obtain the DesktopItem of the DragThumb
            DependencyObject parent = VisualTreeHelper.GetParent(element);

            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as T;
        }

        /// <summary>
        /// Renders the ui element into a bitmap frame.
        /// </summary>
        /// <param name="element">The UI element.</param>
        /// <returns>The created bitmap frame</returns>
        public static BitmapSource RenderToBitmap(this UIElement element)
        {
            return element.RenderToBitmap(1);
        }

        /// <summary>
        /// Renders the ui element into a bitmap frame using the specified scale.
        /// </summary>
        /// <param name="element">The UI element.</param>
        /// <param name="scale">The scale (default: 1).</param>
        /// <returns>The created bitmap frame</returns>
        public static BitmapSource RenderToBitmap(this UIElement element, double scale)
        {
            var renderWidth = (int)(element.RenderSize.Width * scale);
            var renderHeight = (int)(element.RenderSize.Height * scale);

            var renderTarget = new RenderTargetBitmap(renderWidth, renderHeight, 96, 96, PixelFormats.Pbgra32);
            var sourceBrush = new VisualBrush(element);

            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(element.RenderSize.Width, element.RenderSize.Height)));
            }

            renderTarget.Render(drawingVisual);

            return renderTarget;
        }
    }
}
