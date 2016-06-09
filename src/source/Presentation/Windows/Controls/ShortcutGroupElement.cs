// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chronos.Presentation.Windows.Controls
{
    public sealed class ShortcutGroupElement
        : DesktopElement, ISelectable
    {
        #region · Dependency Properties ·

        /// <summary>
        /// Identifies the IsSelected dependency property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected",
                                       typeof(bool),
                                       typeof(ShortcutGroupElement),
                                       new FrameworkPropertyMetadata(false));

        #endregion

        #region · Static Constructors ·

        /// <summary>
        /// Initializes the <see cref="ShortcutElement"/> class.
        /// </summary>
        static ShortcutGroupElement()
        {
            // set the key to reference the style for this control
            ShortcutGroupElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ShortcutGroupElement), new FrameworkPropertyMetadata(typeof(ShortcutGroupElement)));

            Control.IsTabStopProperty.OverrideMetadata(typeof(ShortcutGroupElement),
                new FrameworkPropertyMetadata(false));

            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(
                typeof(ShortcutGroupElement), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));

            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(
                typeof(ShortcutGroupElement), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));

            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(
                typeof(ShortcutGroupElement), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
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
        /// Initializes a new instance of the <see cref="ShortcutGroupElement"/> class.
        /// </summary>
        public ShortcutGroupElement()
            : base()
        {
        }

        #endregion
    }
}
