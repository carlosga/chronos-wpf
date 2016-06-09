// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Chronos.Extensions.Windows;
using Chronos.Presentation.Core.Windows;
using nRoute.Components;

namespace Chronos.Presentation.Windows.Controls
{
    /// <summary>
    /// Provides the ability to create, configure, show, and manage the lifetime of windows
    /// </summary>
    [TemplatePart(Name = WindowElement.PART_CloseButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = WindowElement.PART_ContentPresenter, Type = typeof(ContentPresenter))]
    [TemplateVisualState(Name = NormalVisualState, GroupName = WindowStateGroup)]
    [TemplateVisualState(Name = MinimizedVisualState, GroupName = WindowStateGroup)]
    [TemplateVisualState(Name = MaximizedVisualState, GroupName = WindowStateGroup)]
    public class WindowElement
        : DesktopElement, IWindow, IModalVindow
    {
        #region · Constants ·

        #region · Template Parts ·

        private const string PART_ContentPresenter = "PART_ContentPresenter";
        private const string PART_Root = "PART_Root";
        private const string PART_MaximizeButton = "PART_MaximizeButton";
        private const string PART_MinimizeButton = "PART_MinimizeButton";
        private const string PART_CloseButton = "PART_CloseButton";

        #endregion

        #region · Visual States ·

        private const string WindowStateGroup = "WindowState";
        private const string NormalVisualState = "Normal";
        private const string MinimizedVisualState = "Minimized";
        private const string MaximizedVisualState = "Maximized";

        #endregion

        #region · Misc ·

        private const int MaximizeMargin = 20;

        #endregion        

        #endregion

        #region · Dependency Properties ·

        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(String), typeof(WindowElement),
                new FrameworkPropertyMetadata(String.Empty));

        /// <summary>
        /// Identifies the WindowState dependency property.
        /// </summary>
        public static readonly DependencyProperty WindowStateProperty =
            DependencyProperty.Register("WindowState", typeof(WindowState), typeof(WindowElement),
                new FrameworkPropertyMetadata(WindowState.Normal));

        /// <summary>
        /// Identifies the ShowCloseButton dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowCloseButtonProperty =
            DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(WindowElement),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the ShowMaximizeButton dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowMaximizeButtonProperty =
            DependencyProperty.Register("ShowMaximizeButton", typeof(bool), typeof(WindowElement),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the ShowMinimizeButton dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowMinimizeButtonProperty =
            DependencyProperty.Register("ShowMinimizeButton", typeof(bool), typeof(WindowElement),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the DialogResult dependency property.
        /// </summary>
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.Register("DialogResult", typeof(DialogResult), typeof(WindowElement),
                new FrameworkPropertyMetadata(DialogResult.None));

        /// <summary>
        /// Identifies the ViewMode dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModeProperty =
            DependencyProperty.Register("ViewMode", typeof(ViewModeType), typeof(WindowElement),
                new FrameworkPropertyMetadata(ViewModeType.Default,
                    new PropertyChangedCallback(OnViewModeChanged)));

        #endregion

        #region · Dependency Properties Callback Handlers ·

        private static void OnViewModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                WindowElement window = d as WindowElement;

                if (window.IsActive)
                {
                    ViewModeType oldViewModel = (ViewModeType)e.OldValue;

                    if (oldViewModel == ViewModeType.Add
                        || oldViewModel == ViewModeType.Edit)
                    {
                        window.UpdateActiveElementBindings();
                    }
                    else
                    {
                        window.MoveFocus(FocusNavigationDirection.Next);
                    }
                }
            }
        }

        #endregion

        #region · Static members ·

        /// <summary>
        /// Container panel for modal windows
        /// </summary>
        public static Panel ModalContainerPanel;

        #endregion

        #region · Static Constructor ·

        /// <summary>
        /// Initializes the <see cref="WindowElement"/> class.
        /// </summary>
        static WindowElement()
        {
            WindowElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowElement),
                new FrameworkPropertyMetadata(typeof(WindowElement)));

            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(WindowElement),
                new FrameworkPropertyMetadata(false));

            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(
                typeof(WindowElement), new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));

            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(
                typeof(WindowElement), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));

            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(
                typeof(WindowElement), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));

            if (!DesignMode.IsInDesignMode
                && Application.Current.GetRenderTier() != RenderTier.Tier2)
            {
                WindowElement.CacheModeProperty.OverrideMetadata(typeof(WindowElement),
                    new FrameworkPropertyMetadata(new BitmapCache { EnableClearType = true, RenderAtScale = 1, SnapsToDevicePixels = true }));
            }
        }

        #endregion

        #region · Events ·

        /// <summary>
        /// Occurs when the window is about to close.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Occurs directly after System.Windows.Window.Close() is called, and can be
        ///     handled to cancel window closure.
        /// </summary>
        public event CancelEventHandler Closing;

        /// <summary>
        /// Occurs when the window's System.Windows.Window.WindowState property changes.
        /// </summary>
        public event EventHandler WindowStateChanged;

        #endregion

        #region · Fields ·

        private ContentPresenter _contentPresenter;
        private DispatcherFrame _dispatcherFrame;
        private WindowState _oldWindowState;
        private bool _isShowed;
        private bool _isModal;

        #region · Commands ·

        private ICommand _minimizeCommand;
        private ICommand _maximizeCommand;
        private ICommand _closeCommand;

        #endregion

        #endregion

        #region · IWindow Commands ·

        /// <summary>
        /// Gets the maximize window command
        /// </summary>
        /// <value></value>
        public ICommand MaximizeCommand
        {
            get
            {
                if (_maximizeCommand == null)
                {
                    _maximizeCommand = new ActionCommand(() => OnMaximizeWindow());
                }

                return _maximizeCommand;
            }
        }

        /// <summary>
        /// Gets the minimize window command
        /// </summary>
        /// <value></value>
        public ICommand MinimizeCommand
        {
            get
            {
                if (_minimizeCommand == null)
                {
                    _minimizeCommand = new ActionCommand(() => OnMinimizeWindow());
                }

                return _minimizeCommand;
            }
        }

        /// <summary>
        /// Gets the close window command
        /// </summary>
        /// <value></value>
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new ActionCommand(() => OnCloseWindow());
                }

                return _closeCommand;
            }
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets or sets a window's title. This is a dependency property. 
        /// </summary>
        public String Title
        {
            get { return (String)base.GetValue(WindowElement.TitleProperty); }
            set { base.SetValue(WindowElement.TitleProperty, value); }
        }

        /// <summary>
        /// Gets the dialog result
        /// </summary>
        public DialogResult DialogResult
        {
            get { return (DialogResult)base.GetValue(DialogResultProperty); }
            set { base.SetValue(DialogResultProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether a window is restored, minimized, or maximized. 
        /// This is a dependency property.
        /// </summary>
        /// <value>A <see cref="WindowState"/> that determines whether a window is restored, minimized, or maximized. The default is Normal (restored).</value>
        public WindowState WindowState
        {
            get { return (WindowState)base.GetValue(WindowStateProperty); }
            set
            {
                if ((WindowState)this.GetValue(WindowStateProperty) != value)
                {
                    if (_oldWindowState == System.Windows.WindowState.Maximized
                        && this.WindowState == System.Windows.WindowState.Minimized
                        && value == System.Windows.WindowState.Normal)
                    {
                        this.UpdateWindowState(this.WindowState, _oldWindowState);

                        base.SetValue(WindowStateProperty, _oldWindowState);
                    }
                    else
                    {
                        this.UpdateWindowState(this.WindowState, value);

                        base.SetValue(WindowStateProperty, value);
                    }

                    if (this.WindowStateChanged != null)
                    {
                        this.WindowStateChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the close button is visible. 
        /// This is a dependency property. 
        /// </summary>
        public bool ShowCloseButton
        {
            get { return (bool)base.GetValue(ShowCloseButtonProperty); }
            set { base.SetValue(ShowCloseButtonProperty, value); }
        }

        /// <summary>
        /// Gets a value that indicates whether the maximize button is visible. 
        /// This is a dependency property. 
        /// </summary>
        public bool ShowMaximizeButton
        {
            get { return (bool)base.GetValue(ShowMaximizeButtonProperty); }
            set { base.SetValue(ShowMaximizeButtonProperty, value); }
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
        /// Gets a value indicating whether the element can be dragged.
        /// </summary>
        /// <value><c>true</c> if this instance can drag; otherwise, <c>false</c>.</value>
        public override bool CanDrag
        {
            get { return (bool)base.GetValue(CanDragProperty) && this.WindowState == System.Windows.WindowState.Normal; }
            set { base.SetValue(CanDragProperty, value); }
        }

        /// <summary>
        /// Gets the view mode
        /// </summary>
        public ViewModeType ViewMode
        {
            get { return (ViewModeType)base.GetValue(ViewModeProperty); }
            set
            {
                base.SetValue(ViewModeProperty, value);

                if (value == ViewModeType.Busy)
                {
                    this.GiveFocus();
                    Application.Current.DoEvents();
                }
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowElement"/> class.
        /// </summary>
        public WindowElement()
            : base()
        {
        }

        #endregion

        #region · Methods ·

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _contentPresenter = this.GetTemplateChild(WindowElement.PART_ContentPresenter) as ContentPresenter;
        }

        /// <summary>
        /// Shows the window
        /// </summary>
        public void Show()
        {
            if (!_isShowed)
            {
                _isShowed = true;
            }

            this.OnActivated();
        }

        /// <summary>
        /// Shows the window as a modal dialog
        /// </summary>
        public DialogResult ShowDialog()
        {
            this.Parent = WindowElement.ModalContainerPanel;
            _isModal = true;

            this.LockKeyboardNavigation();
            this.LockMouseOutside();

            this.Show();

            // Set DialogResult default value
            this.DialogResult = DialogResult.None;

            try
            {
                // Push the current thread to a modal state
                ComponentDispatcher.PushModal();

                // Create a DispatcherFrame instance and use it to start a message loop
                _dispatcherFrame = new DispatcherFrame();
                Dispatcher.PushFrame(_dispatcherFrame);
            }
            finally
            {
                // Pop the current thread from modal state
                ComponentDispatcher.PopModal();
            }

            return this.DialogResult;
        }

        /// <summary>
        /// Manually closes a <see cref="WindowElement"/>.
        /// </summary>
        public override void Close()
        {
            CancelEventArgs e = new CancelEventArgs();

            if (this.Closing != null)
            {
                this.Closing(this, e);
            }

            if (!e.Cancel)
            {
                this.LockMouseOutside(false);

                // Clean up
                _maximizeCommand = null;
                _minimizeCommand = null;
                _closeCommand = null;
                _dispatcherFrame = null;
                _isModal = false;
                _isShowed = false;

                this.CommandBindings.Clear();

                base.Close();

                if (this.Closed != null)
                {
                    this.Closed(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Hides the Window
        /// </summary>
        public void Hide()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;

            if (_isModal && _dispatcherFrame != null)
            {
                _dispatcherFrame.Continue = false;
                _dispatcherFrame = null;
            }
        }

        #endregion

        #region · Protected Methods ·

        /// <summary>
        /// Focuses the window
        /// </summary>
        protected override void GiveFocus()
        {
            this.MoveFocus(FocusNavigationDirection.Next);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.GotKeyboardFocus"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            this.OnActivated();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if ((e.Key == Key.Enter || e.Key == Key.Return) &&
                Keyboard.Modifiers == ModifierKeys.None)
            {
                if (e.OriginalSource != this)
                {
                    if (e.OriginalSource is DataGridCell ||
                        e.OriginalSource is ComboBoxItem)
                    {
                    }
                    else
                    {
                        FocusNavigationDirection direction = FocusNavigationDirection.Next;
                        UIElement element = e.OriginalSource as UIElement;

                        if (element != null)
                        {
                            element.MoveFocus(new TraversalRequest(direction));
                            e.Handled = true;
                        }
                    }
                }
            }

            base.OnPreviewKeyDown(e);
        }

        #endregion

        #region · Command Execution Methods ·

        protected virtual void OnCloseWindow()
        {
            if (_isModal)
            {
                this.Hide();
            }
            else
            {
                this.Close();
            }
        }

        protected virtual void OnMaximizeWindow()
        {
            if (this.WindowState != WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        protected virtual void OnMinimizeWindow()
        {
            if (this.WindowState != WindowState.Minimized)
            {
                this.WindowState = WindowState.Minimized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        #endregion

        #region · Private Methods ·

        /// <summary>
        /// http://blogs.msdn.com/b/ryantrem/archive/2010/04/09/globally-updating-binding-sources-in-wpf.aspx
        /// </summary>
        private void UpdateActiveElementBindings()
        {
            if (Keyboard.FocusedElement != null &&
                Keyboard.FocusedElement is DependencyObject)
            {
                DependencyObject element = (DependencyObject)Keyboard.FocusedElement;
                LocalValueEnumerator localValueEnumerator = element.GetLocalValueEnumerator();

                while (localValueEnumerator.MoveNext())
                {
                    BindingExpressionBase bindingExpression = BindingOperations.GetBindingExpressionBase(element, localValueEnumerator.Current.Property);

                    if (bindingExpression != null)
                    {
                        bindingExpression.UpdateSource();
                        bindingExpression.UpdateTarget();
                    }
                }
            }
        }

        private void LockKeyboardNavigation()
        {
            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.None);
            KeyboardNavigation.SetControlTabNavigation(this, KeyboardNavigationMode.None);
        }

        private void LockMouseOutside()
        {
            this.LockMouseOutside(true);
        }

        private void LockMouseOutside(bool doLock)
        {
            if (doLock)
            {
                Debug.Assert(WindowElement.ModalContainerPanel != null);

                Binding wBinding = new Binding("ActualWidth");
                Binding hBinding = new Binding("ActualHeight");
                Canvas fence = new Canvas();

                wBinding.Source = WindowElement.ModalContainerPanel;
                hBinding.Source = WindowElement.ModalContainerPanel;

                fence.SetBinding(Canvas.WidthProperty, wBinding);
                fence.SetBinding(Canvas.HeightProperty, hBinding);

                fence.Background = new SolidColorBrush(Color.FromArgb(141, 162, 174, 255));
                fence.Opacity = 0.5;

                WindowElement.ModalContainerPanel.Children.Add(this);
                WindowElement.ModalContainerPanel.Children.Add(fence);
                WindowElement.ModalContainerPanel.BringToBottom(fence);
                WindowElement.ModalContainerPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Debug.Assert(WindowElement.ModalContainerPanel != null);

                for (int i = 0; i < WindowElement.ModalContainerPanel.Children.Count; i++)
                {
                    UIElement target = WindowElement.ModalContainerPanel.Children[i];

                    if (target is Panel)
                    {
                        WindowElement.ModalContainerPanel.Children.Remove(target);
                        break;
                    }
                }

                WindowElement.ModalContainerPanel.Children.Remove(this);
                WindowElement.ModalContainerPanel.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void UpdateWindowState(WindowState oldWindowState, WindowState newWindowState)
        {
            if (newWindowState == WindowState.Maximized)
            {
                if (oldWindowState == System.Windows.WindowState.Minimized)
                {
                    this.Visibility = System.Windows.Visibility.Visible;
                }

                VisualStateManager.GoToState(this, MaximizedVisualState, true);

                ScaleTransform st = _contentPresenter.LayoutTransform as ScaleTransform;

                if (st != null)
                {
                    this.OldPosition = this.GetPosition();

                    if (this.ConstraintToParent)
                    {
                        if (this.ActualWidth > this.ActualHeight)
                        {
                            st.ScaleX = Math.Round((this.Parent.ActualWidth - MaximizeMargin) / this.OriginalSize.Width, 2);
                        }
                        else
                        {
                            st.ScaleY = Math.Round((this.Parent.ActualHeight - MaximizeMargin) / this.OriginalSize.Height, 2);
                        }

                        Application.Current.DoEvents();

                        this.MoveElement
                        (
                            (this.Parent.ActualWidth - this.ActualWidth) / 2,
                            (this.Parent.ActualHeight - this.ActualHeight) / 2
                        );
                    }
                    else
                    {
                        double scaleX = Math.Round(this.GetParent<Window>().ActualWidth / this.OriginalSize.Width, 2);
                        double scaleY = Math.Round(this.GetParent<Window>().ActualHeight / this.OriginalSize.Height, 2);

                        if (scaleX < scaleY)
                        {
                            st.ScaleX = (scaleX > 2.5) ? 2.5 : scaleX;
                        }
                        else
                        {
                            st.ScaleY = (scaleY > 2.5) ? 2.5 : scaleY;
                        }

                        Application.Current.DoEvents();

                        this.MoveElement
                        (
                            (this.GetParent<Window>().ActualWidth - this.ActualWidth) / 2,
                            (this.GetParent<Window>().ActualHeight - this.ActualHeight) / 2 - (this.GetParent<Window>().ActualHeight - this.Parent.ActualHeight)
                        );
                    }
                }

                this.Parent.BringToFront(this);
            }
            else if (newWindowState == WindowState.Normal)
            {
                this.Visibility = System.Windows.Visibility.Visible;

                VisualStateManager.GoToState(this, NormalVisualState, true);

                if (oldWindowState == WindowState.Minimized)
                {
                    this.Activate();
                }
                else if (oldWindowState == WindowState.Maximized)
                {
                    ScaleTransform st = _contentPresenter.LayoutTransform as ScaleTransform;

                    if (st != null)
                    {
                        st.ScaleX = 1.0;
                    }
                }

                this.MoveElement(this.OldPosition.X, this.OldPosition.Y);
            }
            else if (newWindowState == WindowState.Minimized)
            {
                VisualStateManager.GoToState(this, MinimizedVisualState, true);

                this.Visibility = Visibility.Collapsed;
                _oldWindowState = oldWindowState;
                this.OldPosition = this.GetPosition();

                this.OnDeactivated();
            }
        }

        #endregion
    }
}