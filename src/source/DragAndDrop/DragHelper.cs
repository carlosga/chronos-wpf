// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using Chronos.Interop;

namespace Chronos.Presentation.DragAndDrop
{
    public sealed class DragHelper
    {
        private static UIElement GetDragElementOnHitTest(UIElement src, MouseEventArgs args)
        {
            HitTestResult hr = VisualTreeHelper.HitTest(src, args.GetPosition((IInputElement)src));
            return hr.VisualHit as UIElement;
        }

        private UIElement _dragSource;
        private UIElement _dragScope;
        private IDataDropObjectProvider _callback;
        private Point _startPoint;
        private DragAdorner _adorner;
        private AdornerLayer _layer;
        private Window _dragdropWindow;
        private DragDropEffects _allowedEffects;
        private bool _mouseLeftScope;
        private bool _isDragging;
        private double _opacity;

        public double Opacity
        {
            get { return _opacity; }
        }

        public DragDropEffects AllowedEffects
        {
            get { return _allowedEffects; }
            set { _allowedEffects = value; }
        }

        private UIElement DragSource
        {
            get { return _dragSource; }
            set
            {
                _dragSource = value;
                this.WireEvents(value);
            }
        }

        private UIElement DragScope
        {
            get { return _dragScope; }
            set { _dragScope = value; }
        }

        private IDataDropObjectProvider Callback
        {
            get { return _callback; }
            set { _callback = value; }
        }

        private bool IsDragging
        {
            get { return _isDragging; }
            set { _isDragging = value; }
        }

        private bool AllowsLink
        {
            get { return ((this.AllowedEffects & DragDropEffects.Link) != 0); }
        }

        private bool AllowsMove
        {
            get { return ((this.AllowedEffects & DragDropEffects.Move) != 0); }
        }

        private bool AllowsCopy
        {
            get { return ((this.AllowedEffects & DragDropEffects.Copy) != 0); }
        }

        public DragHelper(UIElement dragSource, IDataDropObjectProvider callback, UIElement dragScope)
        {
            _allowedEffects = DragDropEffects.Copy | DragDropEffects.Move;
            _opacity = 0.7;
            this.DragSource = dragSource;
            this.Callback = callback;
            this.DragScope = dragScope;
        }

        private void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(DragScope);
        }

#if DEBUG
        void DragSource_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.Assert(IsDragging == false);
        }
#endif

        private void DragSource_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !this.IsDragging)
            {
                Point position = e.GetPosition((IInputElement)DragScope);

                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    this.StartDrag(e);
                }
            }
        }

        private void DragScope_DragLeave(object sender, DragEventArgs args)
        {
            if (args.OriginalSource == this.DragScope)
            {
                _mouseLeftScope = true;
            }
        }

        private void DragScope_DragOver(object sender, DragEventArgs args)
        {
            if (_adorner != null)
            {
                _adorner.LeftOffset = args.GetPosition(this.DragScope).X; /* - _startPoint.X */
                _adorner.TopOffset = args.GetPosition(this.DragScope).Y; /* - _startPoint.Y */
            }
        }

        private void WireEvents(UIElement uie)
        {
            Debug.Assert(uie != null);

            if (uie != null)
            {
                uie.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
                uie.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);

#if DEBUG
                uie.PreviewMouseLeftButtonUp    += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonUp);
#endif
            }
        }

        private void StartDrag(MouseEventArgs args)
        {
            IDataObject data = null;
            UIElement dragelement = null;

            // ADD THE DATA 
            if (this.Callback != null)
            {
                DragDataWrapper dw = new DragDataWrapper();

                data = new DataObject(typeof(DragDataWrapper).ToString(), dw);

                if ((this.Callback.SupportedActions & DragDropProviderActions.MultiFormatData) != 0)
                {
                    this.Callback.AppendData(ref data, args);
                }

                if ((this.Callback.SupportedActions & DragDropProviderActions.Data) != 0)
                {
                    dw.Data = this.Callback.GetData();
                }

                if ((this.Callback.SupportedActions & DragDropProviderActions.Visual) != 0)
                {
                    dragelement = this.Callback.GetVisual(args);
                }
                else
                {
                    dragelement = args.OriginalSource as UIElement;
                }

                dw.Source = this.DragSource;
                dw.Shim = this.Callback;

                Debug.Assert(this.DragScope == null, "The DragDataWrapper is meant for in-proc...  Sorry for asserting, just wanted to confirm.. comment out assertion if needed");
            }
            else
            {
                dragelement = args.OriginalSource as UIElement;
                data = new DataObject(typeof(UIElement).ToString(), dragelement);
            }

            if (dragelement == null || data == null || dragelement == this.DragSource)
            {
                return;
            }

            DragEventHandler dragOver = null;
            DragEventHandler dragLeave = null;
            QueryContinueDragEventHandler queryContinue = null;
            GiveFeedbackEventHandler giveFeedback = null;
            DragDropEffects effects = this.GetDragDropEffects();
            DragDropEffects resultEffects;

            // Inprocess Drag  ...
            if (this.DragScope != null)
            {
                bool previousAllowDrop = this.DragScope.AllowDrop;

                _adorner = new DragAdorner(this.DragScope, (UIElement)dragelement, true, this.Opacity);
                _layer = AdornerLayer.GetAdornerLayer(this.DragScope as Visual);

                _layer.Add(_adorner);

                if (this.DragScope != this.DragSource)
                {
                    this.DragScope.AllowDrop = true;

                    DragDrop.AddPreviewDragOverHandler((DependencyObject)this.DragScope, dragOver = new DragEventHandler(DragScope_DragOver));
                    DragDrop.AddPreviewDragLeaveHandler(this.DragScope, dragLeave = new DragEventHandler(DragScope_DragLeave));
                    DragDrop.AddPreviewQueryContinueDragHandler(this.DragSource, queryContinue = new QueryContinueDragEventHandler(OnQueryContinueDrag));
                }

                try
                {
                    this.IsDragging = true;
                    _mouseLeftScope = false;
                    resultEffects = DragDrop.DoDragDrop(this.DragSource, data, effects);

                    this.DragFinished(resultEffects);
                }
                catch
                {
                    Debug.Assert(false);
                }

                if (this.DragScope != this.DragSource)
                {
                    this.DragScope.AllowDrop = previousAllowDrop;

                    DragDrop.RemovePreviewDragOverHandler(this.DragScope, dragOver);
                    DragDrop.RemovePreviewDragLeaveHandler(this.DragScope, dragLeave);
                    DragDrop.RemovePreviewQueryContinueDragHandler(this.DragSource, queryContinue);
                }
            }
            else
            {
                DragDrop.AddPreviewQueryContinueDragHandler(this.DragSource, queryContinue = new QueryContinueDragEventHandler(OnQueryContinueDrag));
                DragDrop.AddGiveFeedbackHandler(this.DragSource, giveFeedback = new GiveFeedbackEventHandler(OnGiveFeedback));

                this.IsDragging = true;

                if ((this.Callback.SupportedActions & DragDropProviderActions.Visual) != 0)
                {
                    this.CreateDragDropWindow(dragelement);
                    _dragdropWindow.Show();
                }

                try
                {
                    resultEffects = DragDrop.DoDragDrop(this.DragSource, data, effects);
                }
                finally
                {
                }

                if ((this.Callback.SupportedActions & DragDropProviderActions.Visual) != 0)
                {
                    this.DestroyDragDropWindow();
                }

                DragDrop.RemovePreviewQueryContinueDragHandler(this.DragSource, OnQueryContinueDrag);
                DragDrop.AddGiveFeedbackHandler(this.DragSource, OnGiveFeedback);

                this.IsDragging = false;
                this.DragFinished(resultEffects);
            }
        }

        private DragDropEffects GetDragDropEffects()
        {
            DragDropEffects effects = DragDropEffects.None;

            bool ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (ctrl && shift && this.AllowsLink)
            {
                effects |= DragDropEffects.Link;
            }
            else if (ctrl && this.AllowsCopy)
            {
                effects |= DragDropEffects.Copy;
            }
            else if (this.AllowsMove)
            {
                effects |= DragDropEffects.Move;
            }

            return effects;
        }

        private void OnGiveFeedback(object sender, GiveFeedbackEventArgs args)
        {
            args.UseDefaultCursors = ((this.Callback.SupportedActions & DragDropProviderActions.Visual) == 0);
            args.Handled = true;
        }

        private void OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
#if DEBUG
            if (this.DragScope != null)
            {
                Point pd = Mouse.GetPosition(this.DragScope);
            }
#endif
            if (this.DragScope == null)
            {
                this.UpdateWindowLocation();
            }
            else
            {
                Point p = Mouse.GetPosition(this.DragScope);

                if (_adorner != null)
                {
                    _adorner.LeftOffset = p.X /* - _startPoint.X */ ;
                    _adorner.TopOffset = p.Y /* - _startPoint.Y */ ;
                }

                if (_mouseLeftScope)
                {
                    e.Action = DragAction.Cancel;
                    e.Handled = true;
                }
            }
        }

        private void DestroyDragDropWindow()
        {
            if (_dragdropWindow != null)
            {
                _dragdropWindow.Close();
                _dragdropWindow = null;
            }
        }

        private void CreateDragDropWindow(Visual dragElement)
        {
            Debug.Assert(_dragdropWindow == null);

            _dragdropWindow = new Window();

            _dragdropWindow.WindowStyle = WindowStyle.None;
            _dragdropWindow.AllowsTransparency = true;
            _dragdropWindow.AllowDrop = false;
            _dragdropWindow.Background = null;
            _dragdropWindow.IsHitTestVisible = false;
            _dragdropWindow.SizeToContent = SizeToContent.WidthAndHeight;
            _dragdropWindow.Topmost = true;
            _dragdropWindow.ShowInTaskbar = false;

            _dragdropWindow.SourceInitialized += new EventHandler(
                delegate (object sender, EventArgs args)
                {
                    //TODO assert that we can do this.. 
                    PresentationSource windowSource = PresentationSource.FromVisual(_dragdropWindow);
                    IntPtr handle = ((HwndSource)windowSource).Handle;

                    Int32 styles = Win32Interop.GetWindowLong(handle, Win32Interop.GWL_EXSTYLE);

                    Win32Interop.SetWindowLong(handle, Win32Interop.GWL_EXSTYLE, styles | Win32Interop.WS_EX_LAYERED | Win32Interop.WS_EX_TRANSPARENT);
                });

            Rectangle r = new Rectangle();

            r.Width = ((FrameworkElement)dragElement).ActualWidth;
            r.Height = ((FrameworkElement)dragElement).ActualHeight;
            r.Fill = new VisualBrush(dragElement);

            _dragdropWindow.Content = r;

            // we want QueryContinueDrag notification so we can update the window position
            //DragDrop.AddPreviewQueryContinueDragHandler(source, QueryContinueDrag);

            // put the window in the right place to start
            this.UpdateWindowLocation();
        }

        private void UpdateWindowLocation()
        {
            if (_dragdropWindow != null)
            {
                Win32Interop.POINT p;

                if (!Win32Interop.GetCursorPos(out p))
                {
                    return;
                }

                _dragdropWindow.Left = (double)p.X;
                _dragdropWindow.Top = (double)p.Y;
            }
        }

        private void DragFinished(DragDropEffects ret)
        {
            Mouse.Capture(null);

            if (this.IsDragging)
            {
                if (this.DragScope != null)
                {
                    //AdornerLayer.GetAdornerLayer(this.DragScope).Remove(this.adorner);
                    //this.adorner = null;
                }
                else
                {
                    this.DestroyDragDropWindow();
                }
            }

            this.IsDragging = false;
        }
    }
}
