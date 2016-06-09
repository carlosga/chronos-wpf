// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chronos.Presentation.Windows.Controls
{
    /// <summary>
    /// Provides the ability to display shortcuts in a <see cref="Desktop"/> control.
    /// </summary>
    public sealed class ShortcutElement
        : DesktopElement, ISelectable
    {
        #region · Dependency Properties ·

        /// <summary>
        /// Identifies the IsSelected dependency property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected",
                                       typeof(bool),
                                       typeof(ShortcutElement),
                                       new FrameworkPropertyMetadata(false));

        #endregion

        #region · Static Constructors ·

        /// <summary>
        /// Initializes the <see cref="ShortcutElement"/> class.
        /// </summary>
        static ShortcutElement()
        {
            // set the key to reference the style for this control
            ShortcutElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ShortcutElement), new FrameworkPropertyMetadata(typeof(ShortcutElement)));

            Control.IsTabStopProperty.OverrideMetadata(typeof(ShortcutElement),
                new FrameworkPropertyMetadata(false));

            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(
                typeof(ShortcutElement), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));

            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(
                typeof(ShortcutElement), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));

            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(
                typeof(ShortcutElement), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
        }

        #endregion

        #region · ISelectable Members ·

        public Guid ParentId
        {
            get;
            set;
        }

        public bool IsSelected
        {
            get { return (bool)base.GetValue(IsSelectedProperty); }
            set { base.SetValue(IsSelectedProperty, value); }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutElement"/> class.
        /// </summary>
        public ShortcutElement()
            : base()
        {
        }

        #endregion
    }
}
