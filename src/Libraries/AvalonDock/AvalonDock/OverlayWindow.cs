//Copyright (c) 2007-2010, Adolfo Marinucci
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, 
//are permitted provided that the following conditions are met:
//
//* Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer.
//* Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution.
//* Neither the name of Adolfo Marinucci nor the names of its contributors may 
//  be used to endorse or promote products derived from this software without 
//  specific prior written permission.
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
//AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
//IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
//INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
//PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
//HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
//EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Markup;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace AvalonDock
{
    public enum OverlayButtonHover
    { 
        None,
        DropPaneInto,
        DropPaneLeft,
        DropPaneRight,
        DropPaneTop,
        DropPaneBottom,
        DropBorderLeft,
        DropBorderRight,
        DropBorderTop,
        DropBorderBottom,
        DropMainPaneLeft,
        DropMainPaneRight,
        DropMainPaneTop,
        DropMainPaneBottom,
    }

    public class OverlayWindow : AvalonDockWindow//, INotifyPropertyChanged
    {
        static OverlayWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(typeof(OverlayWindow)));

            Window.AllowsTransparencyProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(true));
            Window.WindowStyleProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(WindowStyle.None));
            Window.ShowInTaskbarProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(false));
            Window.ShowActivatedProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(false));
        }

        public static object Theme;

        internal OverlayWindow()
        { 
        
        }

        DockingManager _manager = null;

        internal OverlayWindow(DockingManager manager)
        {
            _manager = manager;
        }

        FrameworkElement gridPaneRelativeDockingOptions;
        FrameworkElement selectionBox;

        OverlayWindowDockingButton owdBottom;
        OverlayWindowDockingButton owdTop;
        OverlayWindowDockingButton owdLeft;
        OverlayWindowDockingButton owdRight;
        OverlayWindowDockingButton owdPaneBottom;
        OverlayWindowDockingButton owdPaneTop;
        OverlayWindowDockingButton owdPaneLeft;
        OverlayWindowDockingButton owdPaneRight;
        OverlayWindowDockingButton owdPaneInto;

        OverlayWindowDockingButton owdMainPaneBottom;
        OverlayWindowDockingButton owdMainPaneTop;
        OverlayWindowDockingButton owdMainPaneLeft;
        OverlayWindowDockingButton owdMainPaneRight;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            gridPaneRelativeDockingOptions  = GetTemplateChild("PART_gridPaneRelativeDockingOptions") as FrameworkElement;
            selectionBox = GetTemplateChild("PART_SelectionBox") as FrameworkElement;

            owdBottom = new OverlayWindowDockingButton(GetTemplateChild("PART_btnDockBottom") as FrameworkElement, this);
            owdTop = new OverlayWindowDockingButton(GetTemplateChild("PART_btnDockTop") as FrameworkElement, this);
            owdLeft = new OverlayWindowDockingButton(GetTemplateChild("PART_btnDockLeft") as FrameworkElement, this);
            owdRight = new OverlayWindowDockingButton(GetTemplateChild("PART_btnDockRight") as FrameworkElement, this);

            owdPaneBottom = new OverlayWindowDockingButton(GetTemplateChild("PART_btnDockPaneBottom") as FrameworkElement, this);
            owdPaneTop = new OverlayWindowDockingButton(GetTemplateChild("PART_btnDockPaneTop") as FrameworkElement, this);
            owdPaneLeft = new OverlayWindowDockingButton(GetTemplateChild("PART_btnDockPaneLeft") as FrameworkElement, this);
            owdPaneRight = new OverlayWindowDockingButton(GetTemplateChild("PART_btnDockPaneRight") as FrameworkElement, this);
            owdPaneInto = new OverlayWindowDockingButton(GetTemplateChild("PART_btnDockPaneInto") as FrameworkElement, this);

            var btn = GetTemplateChild("PART_btnDockMainPaneBottom") as FrameworkElement;
            if (btn != null) owdMainPaneBottom = new OverlayWindowDockingButton(btn, this);
            
            btn = GetTemplateChild("PART_btnDockMainPaneTop") as FrameworkElement;
            if (btn != null) owdMainPaneTop = new OverlayWindowDockingButton(btn, this);

            btn = GetTemplateChild("PART_btnDockMainPaneLeft") as FrameworkElement;
            if (btn != null) owdMainPaneLeft = new OverlayWindowDockingButton(btn, this);

            btn = GetTemplateChild("PART_btnDockMainPaneRight") as FrameworkElement;
            if (btn != null) owdMainPaneRight = new OverlayWindowDockingButton(btn, this);


            _manager.DragPaneServices.Register(owdPaneBottom);
            _manager.DragPaneServices.Register(owdPaneTop);
            _manager.DragPaneServices.Register(owdPaneLeft);
            _manager.DragPaneServices.Register(owdPaneRight);
            _manager.DragPaneServices.Register(owdPaneInto);
            _manager.DragPaneServices.Register(owdBottom);
            _manager.DragPaneServices.Register(owdTop);
            _manager.DragPaneServices.Register(owdLeft);
            _manager.DragPaneServices.Register(owdRight);

            if (owdMainPaneTop != null) _manager.DragPaneServices.Register(owdMainPaneTop);
            if (owdMainPaneLeft != null) _manager.DragPaneServices.Register(owdMainPaneLeft);
            if (owdMainPaneRight != null) _manager.DragPaneServices.Register(owdMainPaneRight);
            if (owdMainPaneBottom != null) _manager.DragPaneServices.Register(owdMainPaneBottom);
        }

        internal bool OnDrop(OverlayWindowDockingButton owdDock, Point point)
        {
            //calculate desidered size
            Rect rectPane;

            switch (OverlayButtonHover)
            {
                case AvalonDock.OverlayButtonHover.DropBorderBottom:
                case AvalonDock.OverlayButtonHover.DropBorderLeft:
                case AvalonDock.OverlayButtonHover.DropBorderTop:
                case AvalonDock.OverlayButtonHover.DropBorderRight:
                    rectPane = (_manager as IDropSurface).SurfaceRectangle;
                    break;
                default:
                    rectPane = (CurrentDropPane as IDropSurface).SurfaceRectangle;
                    break;
            }

            var desideredWidth = Math.Min(
                rectPane.Width / 2.0,
                ResizingPanel.GetEffectiveSize(_manager.DragPaneServices.FloatingWindow.HostedPane).Width);
            var desideredHeight = Math.Min(
                rectPane.Height / 2.0,
                ResizingPanel.GetEffectiveSize(_manager.DragPaneServices.FloatingWindow.HostedPane).Height);

            var desideredSize = new Size(
                desideredWidth,
                desideredHeight);

            //user has dropped the floating window over a anchor button 
            //create a new dockable pane to insert in the main layout
            //FIX: clone pane and return true only if overlayButtonOver is not set to None!!
            
            
            //floating window is going to be closed..
            selectionBox.Visibility = Visibility.Hidden;

            //take the overlaybutton hover property to get the right button highlighted
            switch (OverlayButtonHover)
            {
                case AvalonDock.OverlayButtonHover.DropBorderBottom:
                    _manager.Anchor(
                        _manager.DragPaneServices.FloatingWindow.ClonePane() as DockablePane,
                        AnchorStyle.Bottom);
                    break;
                case AvalonDock.OverlayButtonHover.DropBorderTop:
                    _manager.Anchor(
                        _manager.DragPaneServices.FloatingWindow.ClonePane() as DockablePane, 
                        AnchorStyle.Top);
                    break;
                case AvalonDock.OverlayButtonHover.DropBorderLeft:
                    _manager.Anchor(
                        _manager.DragPaneServices.FloatingWindow.ClonePane() as DockablePane, 
                        AnchorStyle.Left);
                    break;
                case AvalonDock.OverlayButtonHover.DropBorderRight:
                    _manager.Anchor(
                        _manager.DragPaneServices.FloatingWindow.ClonePane() as DockablePane, 
                        AnchorStyle.Right);
                    break;
                case AvalonDock.OverlayButtonHover.DropPaneBottom:
                    _manager.Anchor(
                        _manager.DragPaneServices.FloatingWindow.ClonePane(),
                        CurrentDropPane, AnchorStyle.Bottom);
                    break;
                case AvalonDock.OverlayButtonHover.DropPaneTop:
                    _manager.Anchor(
                        _manager.DragPaneServices.FloatingWindow.ClonePane(),
                        CurrentDropPane, AnchorStyle.Top);
                    break;
                case AvalonDock.OverlayButtonHover.DropPaneLeft:
                    _manager.Anchor(
                        _manager.DragPaneServices.FloatingWindow.ClonePane(),
                        CurrentDropPane, AnchorStyle.Left);
                    break;
                case AvalonDock.OverlayButtonHover.DropPaneRight:
                    _manager.Anchor(
                        _manager.DragPaneServices.FloatingWindow.ClonePane(),
                        CurrentDropPane, AnchorStyle.Right);
                    break;
                case AvalonDock.OverlayButtonHover.DropPaneInto:
                    _manager.DropInto(
                        _manager.DragPaneServices.FloatingWindow.ClonePane(), 
                        CurrentDropPane);
                    break;
                default:
                    return false;
            }
            

            return true;
        }

        Pane CurrentDropPane = null;

        internal void ShowOverlayPaneDockingOptions(Pane paneOvering)
        {
            var draggingPane = _manager.DragPaneServices.FloatingWindow.HostedPane;
            var isDraggingADocumentPane = draggingPane is DocumentPane;
            var isDraggingADockablePane = draggingPane is DockablePane;


            HideOverlayPaneDockingOptions(paneOvering);



            //check if dockable on a document pane
            DockableStyle currentPaneDockableStyle =
                isDraggingADocumentPane ? 
                DockableStyle.Document :
                (draggingPane as DockablePane).GetCumulativeDockableStyle();

            //if current drop pane is a DocumentPane ...
            if (paneOvering is DocumentPane &&
                (currentPaneDockableStyle & DockableStyle.Document) == 0)
                return;
            if (paneOvering is DockablePane &&
                (currentPaneDockableStyle & DockableStyle.Dockable) == 0)
                return;


            Rect rectPane = (paneOvering as IDropSurface).SurfaceRectangle;

            Point myScreenTopLeft = this.PointToScreenDPI(new Point(0, 0));
            rectPane.Offset(-myScreenTopLeft.X, -myScreenTopLeft.Y);//relative to me

            gridPaneRelativeDockingOptions.SetValue(Canvas.LeftProperty, rectPane.Left);
            gridPaneRelativeDockingOptions.SetValue(Canvas.TopProperty, rectPane.Top);
            gridPaneRelativeDockingOptions.Width = rectPane.Width;
            gridPaneRelativeDockingOptions.Height = rectPane.Height;
            //gridPaneRelativeDockingOptions.SetValue(Canvas.LeftProperty, rectPane.Left + rectPane.Width / 2 - gridPaneRelativeDockingOptions.Width / 2);
            //gridPaneRelativeDockingOptions.SetValue(Canvas.TopProperty, rectPane.Top + rectPane.Height / 2 - gridPaneRelativeDockingOptions.Height / 2);

            if (paneOvering is DocumentPane)
                gridPaneRelativeDockingOptions.Visibility = Visibility.Visible;
            else
            {
                gridPaneRelativeDockingOptions.Visibility = !isDraggingADocumentPane ? Visibility.Visible : Visibility.Hidden;
            }

            owdBottom.Enabled = ((currentPaneDockableStyle & DockableStyle.BottomBorder) > 0);
            owdTop.Enabled = ((currentPaneDockableStyle & DockableStyle.TopBorder) > 0);
            owdLeft.Enabled = ((currentPaneDockableStyle & DockableStyle.LeftBorder) > 0);
            owdRight.Enabled = ((currentPaneDockableStyle & DockableStyle.RightBorder) > 0);


            if (paneOvering is DocumentPane)
                owdPaneInto.Enabled = true;
            else
                owdPaneInto.Enabled = !(_manager.DragPaneServices.FloatingWindow is DocumentFloatingWindow);


            if (paneOvering is DockablePane || isDraggingADocumentPane)
            {
                if (owdMainPaneBottom != null) owdMainPaneBottom.Enabled = false;
                if (owdMainPaneTop != null) owdMainPaneTop.Enabled = false;
                if (owdMainPaneLeft != null) owdMainPaneLeft.Enabled = false;
                if (owdMainPaneRight != null) owdMainPaneRight.Enabled = false;
            }
            else if (isDraggingADockablePane)
            {
                if (owdMainPaneBottom != null) owdMainPaneBottom.Enabled = true;
                if (owdMainPaneTop != null) owdMainPaneTop.Enabled = true;
                if (owdMainPaneLeft != null) owdMainPaneLeft.Enabled = true;
                if (owdMainPaneRight != null) owdMainPaneRight.Enabled = true;
            }

            int destPaneChildCount = paneOvering.Items.Count;

            owdPaneBottom.Enabled = owdPaneInto.Enabled && destPaneChildCount > 0;
            owdPaneTop.Enabled = owdPaneInto.Enabled && destPaneChildCount > 0;
            owdPaneLeft.Enabled = owdPaneInto.Enabled && destPaneChildCount > 0;
            owdPaneRight.Enabled = owdPaneInto.Enabled && destPaneChildCount > 0;

            CurrentDropPane = paneOvering;
        }

        internal void HideOverlayPaneDockingOptions(Pane surfaceElement)
        {
            owdPaneBottom.Enabled = false;
            owdPaneTop.Enabled = false;
            owdPaneLeft.Enabled = false;
            owdPaneRight.Enabled = false;
            owdPaneInto.Enabled = false;

            gridPaneRelativeDockingOptions.Visibility = Visibility.Collapsed;
            CurrentDropPane = null;
            SetOverlayButtonHover(OverlayButtonHover.None);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            selectionBox.Visibility = Visibility.Hidden;

            base.OnDeactivated(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            DockableStyle currentPaneDockableStyle =
                _manager.DragPaneServices.FloatingWindow.HostedPane is FloatingDocumentPane ?
                DockableStyle.Document :
                (_manager.DragPaneServices.FloatingWindow.HostedPane as DockablePane).GetCumulativeDockableStyle();

            selectionBox.Visibility = Visibility.Hidden;

            owdBottom.Enabled = (currentPaneDockableStyle & DockableStyle.BottomBorder) > 0;
            owdTop.Enabled = (currentPaneDockableStyle & DockableStyle.TopBorder) > 0;
            owdLeft.Enabled = (currentPaneDockableStyle & DockableStyle.LeftBorder) > 0;
            owdRight.Enabled = (currentPaneDockableStyle & DockableStyle.RightBorder) > 0;

            
            base.OnActivated(e);
        }

        /// <summary>
        /// Shows a highlighting rectangle
        /// </summary>
        /// <param name="overlayWindowDockingButton"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        internal void OnDragEnter(OverlayWindowDockingButton owdDock, Point point)
        {
            OnDragOver(owdDock, point);
        }

        /// <summary>
        /// Hides the highlighting rectangle
        /// </summary>
        /// <param name="overlayWindowDockingButton"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        internal void OnDragLeave(OverlayWindowDockingButton owdDock, Point point)
        {
            selectionBox.Visibility = Visibility.Hidden;
            _manager.DragPaneServices.FloatingWindow.OnHideSelectionBox();
            SetOverlayButtonHover(OverlayButtonHover.None);
        }

        internal void OnDragOver(OverlayWindowDockingButton owdDock, Point point)
        {
            if (selectionBox == null)
                return;

            Rect rectPane;

            if (owdDock == owdBottom ||
                owdDock == owdLeft ||
                owdDock == owdTop ||
                owdDock == owdRight)
                rectPane = (_manager as IDropSurface).SurfaceRectangle;
            else
                rectPane = (CurrentDropPane as IDropSurface).SurfaceRectangle;
            
            double selectionBoxWidth = Math.Min(
                rectPane.Width / 2.0,
                ResizingPanel.GetEffectiveSize(_manager.DragPaneServices.FloatingWindow.HostedPane).Width);
            double selectionBoxHeight = Math.Min(
                rectPane.Height / 2.0,
                ResizingPanel.GetEffectiveSize(_manager.DragPaneServices.FloatingWindow.HostedPane).Height);


            Point myScreenTopLeft = this.PointToScreenDPI(new Point());
            rectPane.Offset(-myScreenTopLeft.X, -myScreenTopLeft.Y);//relative to me

            if (owdDock == owdBottom || owdDock == owdPaneBottom)
            {
                selectionBox.SetValue(Canvas.LeftProperty, rectPane.Left);
                selectionBox.SetValue(Canvas.TopProperty, rectPane.Top + rectPane.Height - selectionBoxHeight);
                selectionBox.Width = rectPane.Width;
                selectionBox.Height = selectionBoxHeight;
            }
            if (owdDock == owdLeft || owdDock == owdPaneLeft)
            {
                selectionBox.SetValue(Canvas.LeftProperty, rectPane.Left);
                selectionBox.SetValue(Canvas.TopProperty, rectPane.Top);
                selectionBox.Width = selectionBoxWidth;
                selectionBox.Height = rectPane.Height;
            }
            if (owdDock == owdRight || owdDock == owdPaneRight)
            {
                selectionBox.SetValue(Canvas.LeftProperty, rectPane.Left + rectPane.Width - selectionBoxWidth);
                selectionBox.SetValue(Canvas.TopProperty, rectPane.Top);
                selectionBox.Width = selectionBoxWidth;
                selectionBox.Height = rectPane.Height;
            }
            if (owdDock == owdTop || owdDock == owdPaneTop)
            {
                selectionBox.SetValue(Canvas.LeftProperty, rectPane.Left);
                selectionBox.SetValue(Canvas.TopProperty, rectPane.Top);
                selectionBox.Width = rectPane.Width;
                selectionBox.Height = selectionBoxHeight;
            }
            if (owdDock == owdPaneInto)
            {
                selectionBox.SetValue(Canvas.LeftProperty, rectPane.Left);
                selectionBox.SetValue(Canvas.TopProperty, rectPane.Top);
                selectionBox.Width = rectPane.Width;
                selectionBox.Height = rectPane.Height;
            }

            if (owdDock == owdLeft)
                SetOverlayButtonHover(OverlayButtonHover.DropBorderLeft);//OverlayButtonHover = OverlayButtonHover.DropBorderLeft;
            else if (owdDock == owdRight)
                SetOverlayButtonHover(OverlayButtonHover.DropBorderRight);//OverlayButtonHover = OverlayButtonHover.DropBorderRight;
            else if (owdDock == owdTop)
                SetOverlayButtonHover(OverlayButtonHover.DropBorderTop);//OverlayButtonHover = OverlayButtonHover.DropBorderTop;
            else if (owdDock == owdBottom)
                SetOverlayButtonHover(OverlayButtonHover.DropBorderBottom);//OverlayButtonHover = OverlayButtonHover.DropBorderBottom;
            else if (owdDock == owdPaneInto)
                SetOverlayButtonHover(OverlayButtonHover.DropPaneInto);//OverlayButtonHover = OverlayButtonHover.DropPaneInto;
            else if (owdDock == owdPaneRight)
                SetOverlayButtonHover(OverlayButtonHover.DropPaneRight);//OverlayButtonHover = OverlayButtonHover.DropPaneRight;
            else if (owdDock == owdPaneTop)
                SetOverlayButtonHover(OverlayButtonHover.DropPaneTop);//OverlayButtonHover = OverlayButtonHover.DropPaneTop;
            else if (owdDock == owdPaneLeft)
                SetOverlayButtonHover(OverlayButtonHover.DropPaneLeft);//OverlayButtonHover = OverlayButtonHover.DropPaneLeft;
            else if (owdDock == owdPaneBottom)
                SetOverlayButtonHover(OverlayButtonHover.DropPaneBottom);//OverlayButtonHover = OverlayButtonHover.DropPaneBottom;
            else
                SetOverlayButtonHover(OverlayButtonHover.None);//OverlayButtonHover = OverlayButtonHover.None;


            selectionBox.Visibility = Visibility.Visible;

            _manager.DragPaneServices.FloatingWindow.OnShowSelectionBox();

        }

        #region OverlayButtonHover

        /// <summary>
        /// OverlayButtonHover Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey OverlayButtonHoverPropertyKey
            = DependencyProperty.RegisterReadOnly("OverlayButtonHover", typeof(OverlayButtonHover), typeof(OverlayWindow),
                new FrameworkPropertyMetadata(OverlayButtonHover.None));

        public static readonly DependencyProperty OverlayButtonHoverProperty
            = OverlayButtonHoverPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the OverlayButtonHover property.  This dependency property 
        /// indicates ....
        /// </summary>
        public OverlayButtonHover OverlayButtonHover
        {
            get { return (OverlayButtonHover)GetValue(OverlayButtonHoverProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the OverlayButtonHover property.  
        /// This dependency property indicates indicates which anchor button is currently highlighted by user.
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        protected void SetOverlayButtonHover(OverlayButtonHover value)
        {
            SetValue(OverlayButtonHoverPropertyKey, value);
        }


        #endregion
    }
}
