// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace Chronos.Extensions.Windows
{
    /// <summary>
    /// Extensions methods for the WPF <see cref="Panel"/> control.
    /// </summary>
    public static class PanelExtensions
    {
        #region · Extension Methods ·

        /// <summary>
        /// Brings the element to the front of the z-order.
        /// </summary>
        /// <param name="panel">The canvas.</param>
        /// <param name="element">The element.</param>
        public static void BringToFront(this Panel panel, UIElement element)
        {
            int index = Panel.GetZIndex(element);
            Panel.SetZIndex(element, panel.Children.Count + 1);

            #region · Define ZIndex Order ·

            UIElement[] sortAux = new UIElement[panel.Children.Count];

            panel.Children.CopyTo(sortAux, 0);

            Array.Sort<UIElement>(sortAux, new Comparison<UIElement>(delegate(UIElement a, UIElement b)
            {
                int aIndex = Canvas.GetZIndex(a);
                int bIndex = Canvas.GetZIndex(b);

                if (aIndex > bIndex)
                {
                    return 1;
                }
                else if (aIndex == bIndex)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }));

            #endregion

            for (int i = 0; i < sortAux.Length; i++)
            {
                Panel.SetZIndex(sortAux[i], i);
            }
        }

        /// <summary>
        /// Brings the element to the bottom of the z-order.
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <param name="element">The element.</param>
        public static void BringToBottom(this Panel panel, UIElement element)
        {
            int index = Panel.GetZIndex(element);
            Panel.SetZIndex(element, -1);

            #region · Define ZIndex Order ·

            UIElement[] sortAux = new UIElement[panel.Children.Count];

            panel.Children.CopyTo(sortAux, 0);

            Array.Sort<UIElement>(sortAux, new Comparison<UIElement>(delegate(UIElement a, UIElement b)
            {
                int aIndex = Canvas.GetZIndex(a);
                int bIndex = Canvas.GetZIndex(b);

                if (aIndex > bIndex)
                {
                    return 1;
                }
                else if (aIndex == bIndex)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }));

            #endregion

            for (int i = 0; i < sortAux.Length; i++)
            {
                Panel.SetZIndex(sortAux[i], i);
            }
        }

        /// <summary>
        /// Moves the specified element to the given position.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="position">The position.</param>
        public static void Move(this UIElement element, Point position)
        {
            element.Move(position.X, position.Y);
        }

        /// <summary>
        /// Moves the specified element to the given position.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        public static void Move(this UIElement element, double left, double top)
        {
            element.MoveLeft(left);
            element.MoveTop(top);
        }

        /// <summary>
        /// Moves the specified element to the given left position.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="left">The left.</param>
        public static void MoveLeft(this UIElement element, double left)
        {
            Canvas.SetLeft(element, left);
        }

        /// <summary>
        /// Moves the specified element to the given top position.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="top">The top.</param>
        public static void MoveTop(this UIElement element, double top)
        {
            Canvas.SetTop(element, top);
        }

        /// <summary>
        /// Sets the value of the ZIndex attached property for a given object. 
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="zindex">The zindex.</param>
        public static void SetZIndex(this UIElement element, int zindex)
        {
            Canvas.SetZIndex(element, zindex);
        }

        /// <summary>
        /// Gets the canvas left position.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static Point GetPosition(this UIElement element)
        {
            return new Point(Canvas.GetLeft(element), Canvas.GetTop(element));
        }

        #endregion
    }
}
