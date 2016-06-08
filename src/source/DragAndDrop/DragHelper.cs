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
        #region � Static Methods �

        private static UIElement GetDragElementOnHitTest(UIElement src, MouseEventArgs args)
        {
            HitTestResult hr = VisualTreeHelper.HitTest(src, args.GetPosition((IInputElement)src));
            return hr.VisualHit as UIElement;
        }

        #endregion

        #region � Fields �

        private UIElement               dragSource;
        private UIElement               dragScope;
        private IDataDropObjectProvider callback;
        private Point                   startPoint;
        private DragAdorner             adorner;
        private AdornerLayer            layer;
        private Window                  dragdropWindow;
        private DragDropEffects         allowedEffects;
        private bool                    mouseLeftScope;
        private bool                    isDragging;
        private double                  opacity;        

        #endregion

        #region � Properties �

        public double Opacity
        {
            get { return this.opacity; }
        }

        public DragDropEffects AllowedEffects
        {
            get { return this.allowedEffects; }
            set { this.allowedEffects = value; }
        }

        #endregion

        #region � Private Properties �

        private UIElement DragSource
        {
            get { return this.dragSource; }
            set
            {
                this.dragSource = value;
                this.WireEvents(value);
            }
        }

        private UIElement DragScope
        {
            get { return this.dragScope; }
            set { this.dragScope = value; }
        }

        private IDataDropObjectProvider Callback
        {
            get { return this.callback; }
            set { this.callback = value; }
        }

        private bool IsDragging
        {
            get { return this.isDragging; }
            set { this.isDragging = value; }
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

        #endregion

        #region � Constructors �

        public DragHelper(UIElement dragSource, IDataDropObjectProvider callback, UIElement dragScope)
        {
            this.allowedEffects = DragDropEffects.Copy | DragDropEffects.Move;
            this.opacity        = 0.7;
            this.DragSource     = dragSource;
            this.Callback       = callback;
            this.DragScope      = dragScope;
        }

        #endregion

        #region � DragSource Event Handlers �

        private void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.startPoint = e.GetPosition(DragScope);
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

                if (Math.Abs(position.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    this.StartDrag(e);
                }
            }
        }

        #endregion

        #region � DragScope Event Handlers �

        private void DragScope_DragLeave(object sender, DragEventArgs args)
        {
            if (args.OriginalSource == this.DragScope)
            {
                this.mouseLeftScope = true;
            }
        }

        private void DragScope_DragOver(object sender, DragEventArgs args)
        {
            if (this.adorner != null)
            {
                this.adorner.LeftOffset = args.GetPosition(this.DragScope).X; /* - _startPoint.X */
                this.adorner.TopOffset  = args.GetPosition(this.DragScope).Y; /* - _startPoint.Y */
            }
        }

        #endregion

        #region � Event Wiring �

        private void WireEvents(UIElement uie)
        {
            Debug.Assert(uie != null);

            if (uie != null)
            {
                uie.PreviewMouseLeftButtonDown  += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
                uie.PreviewMouseMove            += new MouseEventHandler(DragSource_PreviewMouseMove);

#if DEBUG
                uie.PreviewMouseLeftButtonUp    += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonUp);
#endif
            }
        }

        #endregion

        #region � Private Methods �

        private void StartDrag(MouseEventArgs args)
        {
            IDataObject data        = null;
            UIElement   dragelement = null;

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

                dw.Source   = this.DragSource;
                dw.Shim     = this.Callback;

                Debug.Assert(this.DragScope == null, "The DragDataWrapper is meant for in-proc...  Sorry for asserting, just wanted to confirm.. comment out assertion if needed"); 
            }
            else
            {
                dragelement = args.OriginalSource as UIElement;
                data        = new DataObject(typeof(UIElement).ToString(), dragelement);                 
            }

            if (dragelement == null || data == null || dragelement == this.DragSource)
            {
                return;
            }         

            DragEventHandler                dragOver        = null;
            DragEventHandler                dragLeave       = null;
            QueryContinueDragEventHandler   queryContinue   = null;
            GiveFeedbackEventHandler        giveFeedback    = null;
            DragDropEffects                 effects         = this.GetDragDropEffects();
            DragDropEffects                 resultEffects;

            // Inprocess Drag  ...
            if (this.DragScope != null)
            {
                bool previousAllowDrop = this.DragScope.AllowDrop;

                this.adorner    = new DragAdorner(this.DragScope, (UIElement)dragelement, true, this.Opacity);
                this.layer      = AdornerLayer.GetAdornerLayer(this.DragScope as Visual);

                this.layer.Add(this.adorner);

                if (this.DragScope != this.DragSource)
                {
                    this.DragScope.AllowDrop = true;

                    DragDrop.AddPreviewDragOverHandler((DependencyObject)this.DragScope, dragOver = new DragEventHandler(DragScope_DragOver));
                    DragDrop.AddPreviewDragLeaveHandler(this.DragScope, dragLeave = new DragEventHandler(DragScope_DragLeave));
                    DragDrop.AddPreviewQueryContinueDragHandler(this.DragSource, queryContinue = new QueryContinueDragEventHandler(OnQueryContinueDrag));
                }

                try
                {
                    this.IsDragging     = true;
                    this.mouseLeftScope = false;                    
                    resultEffects       = DragDrop.DoDragDrop(this.DragSource, data, effects);

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
                    this.dragdropWindow.Show();
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

            bool ctrl   = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool shift  = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

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
            args.UseDefaultCursors  = ((this.Callback.SupportedActions & DragDropProviderActions.Visual) == 0);
            args.Handled            = true;
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

                if (this.adorner != null)
                {
                    this.adorner.LeftOffset    = p.X /* - _startPoint.X */ ;
                    this.adorner.TopOffset     = p.Y /* - _startPoint.Y */ ;
                }

                if (this.mouseLeftScope)
                {
                    e.Action = DragAction.Cancel;
                    e.Handled = true; 
                } 
            }       
        }

        private void DestroyDragDropWindow()
        {
            if (this.dragdropWindow != null)
            {
                this.dragdropWindow.Close();
                this.dragdropWindow = null;
            }
        }

        private void CreateDragDropWindow(Visual dragElement)
        {
            Debug.Assert(this.dragdropWindow == null);

            this.dragdropWindow = new Window();

            this.dragdropWindow.WindowStyle        = WindowStyle.None;
            this.dragdropWindow.AllowsTransparency = true;
            this.dragdropWindow.AllowDrop          = false;
            this.dragdropWindow.Background         = null;
            this.dragdropWindow.IsHitTestVisible   = false;
            this.dragdropWindow.SizeToContent      = SizeToContent.WidthAndHeight;
            this.dragdropWindow.Topmost            = true;
            this.dragdropWindow.ShowInTaskbar      = false;

            this.dragdropWindow.SourceInitialized += new EventHandler(
                delegate(object sender, EventArgs args)
                {
                    //TODO assert that we can do this.. 
                    PresentationSource  windowSource    = PresentationSource.FromVisual(this.dragdropWindow);
                    IntPtr              handle          = ((HwndSource)windowSource).Handle;

                    Int32 styles = Win32Interop.GetWindowLong(handle, Win32Interop.GWL_EXSTYLE);

                    Win32Interop.SetWindowLong(handle, Win32Interop.GWL_EXSTYLE, styles | Win32Interop.WS_EX_LAYERED | Win32Interop.WS_EX_TRANSPARENT);
                });

            Rectangle r = new Rectangle();

            r.Width     = ((FrameworkElement)dragElement).ActualWidth;
            r.Height    = ((FrameworkElement)dragElement).ActualHeight;
            r.Fill  = new VisualBrush(dragElement);

            this.dragdropWindow.Content = r;

            // we want QueryContinueDrag notification so we can update the window position
            //DragDrop.AddPreviewQueryContinueDragHandler(source, QueryContinueDrag);

            // put the window in the right place to start
            this.UpdateWindowLocation();
        }

        private void UpdateWindowLocation()
        {
            if (this.dragdropWindow != null)
            {
                Win32Interop.POINT p;

                if (!Win32Interop.GetCursorPos(out p))
                {
                    return;
                }

                this.dragdropWindow.Left   = (double)p.X;
                this.dragdropWindow.Top    = (double)p.Y;
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

        #endregion
    }
}
