// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Chronos.Extensions.Windows;

namespace Chronos.Presentation.Windows.Controls
{
    /// <summary>
    /// Provides the ability to create, configure, show, and manage the lifetime of widgets
    /// </summary>
    [TemplatePart(Name = WidgetElement.PART_MinimizeButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = WidgetElement.PART_CloseButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = WidgetElement.PART_ContentPresenter, Type = typeof(ContentPresenter))]
    public class WidgetElement 
        : DesktopElement
    {
        #region · Constants ·

        private const string PART_ContentPresenter  = "PART_ContentPresenter";
        private const string PART_MinimizeButton    = "PART_MinimizeButton";
        private const string PART_CloseButton       = "PART_CloseButton";

        #endregion
            
        #region · Dependency Properties ·

        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(String), typeof(WidgetElement),
                new FrameworkPropertyMetadata(String.Empty));

        /// <summary>
        /// Identifies the WindowState dependency property.
        /// </summary>
        public static readonly DependencyProperty WidgetStateProperty =
            DependencyProperty.Register("WidgetState", typeof(WindowState), typeof(WidgetElement),
                new FrameworkPropertyMetadata(WindowState.Normal));

        /// <summary>
        /// Identifies the ShowMinimizeButton dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowMinimizeButtonProperty =
            DependencyProperty.Register("ShowMinimizeButton", typeof(bool), typeof(WidgetElement),
                new FrameworkPropertyMetadata(true));

        #endregion

        #region · Routed Commands ·

        /// <summary>
        /// Minimize window command
        /// </summary>
        public static RoutedCommand MinimizeCommand;

        #endregion

        #region · Static Constructor ·

        /// <summary>
        /// Initializes the <see cref="WidgetElement"/> class.
        /// </summary>
        static WidgetElement()
        {
            WidgetElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(WidgetElement),
                new FrameworkPropertyMetadata(typeof(WidgetElement)));

            WidgetElement.MinimizeCommand = new RoutedCommand("Minimize", typeof(WidgetElement));

            Control.IsTabStopProperty.OverrideMetadata(typeof(WidgetElement),
                new FrameworkPropertyMetadata(false));

            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(
                typeof(WidgetElement), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));

            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(
                typeof(WidgetElement), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));

            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(
                typeof(WidgetElement), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));

            if (!DesignMode.IsInDesignMode
                && Application.Current.GetRenderTier() != RenderTier.Tier2)
            {
                WidgetElement.CacheModeProperty.OverrideMetadata(typeof(WidgetElement),
                    new FrameworkPropertyMetadata(new BitmapCache { EnableClearType = true, RenderAtScale = 1, SnapsToDevicePixels = true }));
            }
        }

        #endregion

        #region · Fields ·

        private Size previousSize;

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets or sets a value that indicates whether a window is restored or minimized. 
        /// This is a dependency property.
        /// </summary>
        /// <value>A <see cref="WindowState"/> that determines whether a window is restored, minimized, or maximized. The default is Normal (restored).</value>
        public WindowState WidgetState
        {
            get { return (WindowState)base.GetValue(WidgetStateProperty); }
            set
            {
                if ((WindowState)this.GetValue(WidgetStateProperty) != value)
                {
                    base.SetValue(WidgetStateProperty, value);
                }

                this.AdjustLayout();
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the minimize button is visible. 
        /// This is a dependency property. 
        /// </summary>
        public bool ShowMinimizeButton
        {
            get { return (bool)base.GetValue(ShowMinimizeButtonProperty); }
            set { base.SetValue(ShowMinimizeButtonProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Widget title 
        /// </summary>
        public string Title
        {
            get { return (string)base.GetValue(TitleProperty); }
            set { base.SetValue(TitleProperty, value); }
        }

        #endregion

        #region · Internal Properties ·

        /// <summary>
        /// Gets the Widget real Height based on the <see cref="WidgetState"/>.
        /// </summary>
        /// <remarks>Used for Widget serialization on <see cref="DesktopSerializer"/> class</remarks>
        internal double RealHeight
        {
            get
            {
                if (this.WidgetState == WindowState.Minimized)
                {
                    return this.previousSize.Height;
                }

                return this.Height;
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetElement"/> class.
        /// </summary>
        public WidgetElement()
            : base()
        {
            CommandBinding bindingMinimize = new CommandBinding(WidgetElement.MinimizeCommand, new ExecutedRoutedEventHandler(this.OnMinimizeWindow));
            this.CommandBindings.Add(bindingMinimize);
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Closes the desktop element
        /// </summary>
        public override void Close()
        {
            this.CommandBindings.Clear();

            base.Close();
        }

        #endregion

        #region · Protected Methods
        
        /// <summary>
        /// Focuses the window
        /// </summary>
        protected override void GiveFocus()
        {
            this.MoveFocus(FocusNavigationDirection.Next);
        }

        #endregion

        #region · Command Actions ·

        /// <summary>
        /// Occurs when the widget is going to be minimized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.WidgetState != WindowState.Minimized)
            {
                this.WidgetState = WindowState.Minimized;
            }
            else
            {
                this.WidgetState = WindowState.Normal;
            }
        }

        #endregion

        #region · Private Methods ·
        
        private void AdjustLayout()
        {
            if (this.WidgetState == WindowState.Minimized)
            {
                this.previousSize   = new Size(this.Width, this.Height);
                this.Height         = 35;
                this.CanResize      = false;
            }
            else
            {
                this.Height         = this.previousSize.Height;
                this.previousSize   = new Size(this.Width, this.Height);
                this.CanResize      = true;
            }
        }

        #endregion
    }
}
