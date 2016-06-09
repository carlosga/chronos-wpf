// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Chronos.Presentation.Controls
{
    /// <summary>
    /// Implementation of a Split Button
    /// </summary>
    [ContentProperty("Items")]
    [DefaultProperty("Items")]
    public partial class SplitButton
        : Button
    {
        // AddOwner Dependency properties
        public static readonly DependencyProperty IsContextMenuOpenProperty =
                DependencyProperty.Register(
                    "IsContextMenuOpen",
                    typeof(bool),
                    typeof(SplitButton),
                    new FrameworkPropertyMetadata(false,
                            new PropertyChangedCallback(OnIsContextMenuOpenChanged)));

        public static readonly DependencyProperty PlacementProperty =
                ContextMenuService.PlacementProperty.AddOwner(
                    typeof(SplitButton),
                    new FrameworkPropertyMetadata(PlacementMode.Bottom,
                        new PropertyChangedCallback(OnPlacementChanged)));

        public static readonly DependencyProperty PlacementRectangleProperty =
                ContextMenuService.PlacementRectangleProperty.AddOwner(
                    typeof(SplitButton),
                    new FrameworkPropertyMetadata(Rect.Empty,
                        new PropertyChangedCallback(OnPlacementRectangleChanged)));

        public static readonly DependencyProperty HorizontalOffsetProperty =
                ContextMenuService.HorizontalOffsetProperty.AddOwner(
                    typeof(SplitButton),
                    new FrameworkPropertyMetadata(0.0,
                        new PropertyChangedCallback(OnHorizontalOffsetChanged)));

        public static readonly DependencyProperty VerticalOffsetProperty =
                ContextMenuService.VerticalOffsetProperty.AddOwner(
                    typeof(SplitButton),
                    new FrameworkPropertyMetadata(0.0,
                        new PropertyChangedCallback(OnVerticalOffsetChanged)));

        private static void OnIsContextMenuOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButton s = (SplitButton)d;
            s.EnsureContextMenuIsValid();

            if (!s.ContextMenu.HasItems)
            {
                return;
            }

            bool value = (bool)e.NewValue;

            if (value && !s.ContextMenu.IsOpen)
            {
                s.ContextMenu.IsOpen = true;
            }
            else if (!value && s.ContextMenu.IsOpen)
            {
                s.ContextMenu.IsOpen = false;
            }
        }

        /// <summary>
        /// Placement Property changed callback, pass the value through to the buttons context menu
        /// </summary>
        private static void OnPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButton s = d as SplitButton;

            if (s == null)
            {
                return;
            }

            s.EnsureContextMenuIsValid();
            s.ContextMenu.Placement = (PlacementMode)e.NewValue;
        }

        /// <summary>
        /// PlacementRectangle Property changed callback, pass the value through to the buttons context menu
        /// </summary>
        private static void OnPlacementRectangleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButton s = d as SplitButton;

            if (s == null)
            {
                return;
            }

            s.EnsureContextMenuIsValid();
            s.ContextMenu.PlacementRectangle = (Rect)e.NewValue;
        }

        /// <summary>
        /// HorizontalOffset Property changed callback, pass the value through to the buttons context menu
        /// </summary>
        private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButton s = d as SplitButton;

            if (s == null)
            {
                return;
            }

            s.EnsureContextMenuIsValid();
            s.ContextMenu.HorizontalOffset = (double)e.NewValue;
        }

        /// <summary>
        /// VerticalOffset Property changed callback, pass the value through to the buttons context menu
        /// </summary>
        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButton s = d as SplitButton;
            if (s == null)
            {
                return;
            }

            s.EnsureContextMenuIsValid();
            s.ContextMenu.VerticalOffset = (double)e.NewValue;
        }

        /// <summary>
        /// Static Constructor
        /// </summary>
        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SplitButton),
                    new FrameworkPropertyMetadata(typeof(SplitButton)));
        }

        /// <summary>
        /// The Split Button's Items property maps to the base classes ContextMenu.Items property
        /// </summary>
        public ItemCollection Items
        {
            get
            {
                this.EnsureContextMenuIsValid();

                return this.ContextMenu.Items;
            }
        }

        /// <summary>
        /// Gets or sets the IsContextMenuOpen property. 
        /// </summary>
        public bool IsContextMenuOpen
        {
            get { return (bool)GetValue(IsContextMenuOpenProperty); }
            set { SetValue(IsContextMenuOpenProperty, value); }
        }

        /// <summary>
        /// Placement of the Context menu
        /// </summary>
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        /// <summary>
        /// PlacementRectangle of the Context menu
        /// </summary>
        public Rect PlacementRectangle
        {
            get { return (Rect)GetValue(PlacementRectangleProperty); }
            set { SetValue(PlacementRectangleProperty, value); }
        }

        /// <summary>
        /// HorizontalOffset of the Context menu
        /// </summary>
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        /// <summary>
        /// VerticalOffset of the Context menu
        /// </summary>
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public SplitButton()
            : base()
        {
        }

        /// <summary>
        ///     Handles the Base Buttons OnClick event
        /// </summary>
        protected override void OnClick()
        {
            this.OnDropdown();
        }

        /// <summary>
        /// Make sure the Context menu is not null
        /// </summary>
        private void EnsureContextMenuIsValid()
        {
            if (this.ContextMenu == null)
            {
                this.ContextMenu = new ContextMenu();
                this.ContextMenu.PlacementTarget = this;
                this.ContextMenu.Placement = this.Placement;

                this.ContextMenu.Opened += ((sender, routedEventArgs) => IsContextMenuOpen = true);
                this.ContextMenu.Closed += ((sender, routedEventArgs) => IsContextMenuOpen = false);
            }
        }

        private void OnDropdown()
        {
            this.EnsureContextMenuIsValid();

            if (!this.ContextMenu.HasItems)
            {
                return;
            }

            this.ContextMenu.IsOpen = !IsContextMenuOpen; // open it if closed, close it if open
        }
    }
}
