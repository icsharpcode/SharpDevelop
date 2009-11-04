//Copyright (c) 2007-2009, Adolfo Marinucci
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
    }

    public class OverlayWindow : Window, INotifyPropertyChanged
    {
        static OverlayWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(typeof(OverlayWindow)));

            Window.AllowsTransparencyProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(true));
            Window.WindowStyleProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(WindowStyle.None));
            Window.ShowInTaskbarProperty.OverrideMetadata(typeof(OverlayWindow), new FrameworkPropertyMetadata(false));
        }

        public OverlayWindow()
        { }

        DockingManager _manager = null;

        public OverlayWindow(DockingManager manager)
        {
            _manager = manager;
        }

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

            _manager.DragPaneServices.Register(owdBottom);
            _manager.DragPaneServices.Register(owdTop);
            _manager.DragPaneServices.Register(owdLeft);
            _manager.DragPaneServices.Register(owdRight);
            _manager.DragPaneServices.Register(owdPaneBottom);
            _manager.DragPaneServices.Register(owdPaneTop);
            _manager.DragPaneServices.Register(owdPaneLeft);
            _manager.DragPaneServices.Register(owdPaneRight);
            _manager.DragPaneServices.Register(owdPaneInto);
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


        internal bool OnDrop(OverlayWindowDockingButton owdDock, Point point)
        {
            //user has dropped the floating window over a anchor button 
            //create a new dockable pane to insert in the main layout
            Pane paneToAnchor = _manager.DragPaneServices.FloatingWindow.ClonePane();

            //floating window is going to be closed..

            if (owdDock == owdBottom)
                _manager.Anchor(paneToAnchor, AnchorStyle.Bottom);
            else if (owdDock == owdLeft)
                _manager.Anchor(paneToAnchor, AnchorStyle.Left);
            else if (owdDock == owdRight)
                _manager.Anchor(paneToAnchor, AnchorStyle.Right);
            else if (owdDock == owdTop)
                _manager.Anchor(paneToAnchor, AnchorStyle.Top);
            else if (owdDock == owdPaneTop)
                _manager.Anchor(paneToAnchor, CurrentDropPane, AnchorStyle.Top);
            else if (owdDock == owdPaneBottom)
                _manager.Anchor(paneToAnchor, CurrentDropPane, AnchorStyle.Bottom);
            else if (owdDock == owdPaneLeft)
                _manager.Anchor(paneToAnchor, CurrentDropPane, AnchorStyle.Left);
            else if (owdDock == owdPaneRight)
                _manager.Anchor(paneToAnchor, CurrentDropPane, AnchorStyle.Right);
            else if (owdDock == owdPaneInto)
                _manager.DropInto(paneToAnchor, CurrentDropPane);

            selectionBox.Visibility = Visibility.Hidden;

            return true;
        }

        Pane CurrentDropPane = null;

        public void ShowOverlayPaneDockingOptions(Pane pane)
        {

            HideOverlayPaneDockingOptions(pane);

            //check if dockable on a document pane
            DockableStyle currentPaneDockableStyle = 
                _manager.DragPaneServices.FloatingWindow.HostedPane.GetCumulativeDockableStyle();

            //if current drop pane is a DocumentPane ...
            if (pane is DocumentPane &&
                (currentPaneDockableStyle & DockableStyle.Document) == 0)
                return;
            if (pane is DockablePane &&
                (currentPaneDockableStyle & DockableStyle.Dockable) == 0)
                return;


            Rect rectPane = pane.SurfaceRectangle;

            Point myScreenTopLeft = this.PointToScreenDPI(new Point(0, 0));
            rectPane.Offset(-myScreenTopLeft.X, -myScreenTopLeft.Y);//relative to me
            gridPaneRelativeDockingOptions.SetValue(Canvas.LeftProperty, rectPane.Left + rectPane.Width / 2 - gridPaneRelativeDockingOptions.Width / 2);
            gridPaneRelativeDockingOptions.SetValue(Canvas.TopProperty, rectPane.Top + rectPane.Height / 2 - gridPaneRelativeDockingOptions.Height / 2);

            if (pane is DocumentPane)
                gridPaneRelativeDockingOptions.Visibility = Visibility.Visible;
            else
            {
                gridPaneRelativeDockingOptions.Visibility = !(_manager.DragPaneServices.FloatingWindow is DocumentFloatingWindow) ? Visibility.Visible : Visibility.Hidden;
            }


            owdBottom.Enabled = ((currentPaneDockableStyle & DockableStyle.BottomBorder) > 0);
            owdTop.Enabled = ((currentPaneDockableStyle & DockableStyle.TopBorder) > 0);
            owdLeft.Enabled = ((currentPaneDockableStyle & DockableStyle.LeftBorder) > 0);
            owdRight.Enabled = ((currentPaneDockableStyle & DockableStyle.RightBorder) > 0);


            if (pane is DocumentPane)
                owdPaneInto.Enabled = true;
            else
                owdPaneInto.Enabled = !(_manager.DragPaneServices.FloatingWindow is DocumentFloatingWindow);

            int destPaneChildCount = pane.Items.Count;

            owdPaneBottom.Enabled = owdPaneInto.Enabled && destPaneChildCount > 0;
            owdPaneTop.Enabled = owdPaneInto.Enabled && destPaneChildCount > 0;
            owdPaneLeft.Enabled = owdPaneInto.Enabled && destPaneChildCount > 0;
            owdPaneRight.Enabled = owdPaneInto.Enabled && destPaneChildCount > 0;

            CurrentDropPane = pane;
        }

        public void HideOverlayPaneDockingOptions(Pane surfaceElement)
        {

            owdPaneBottom.Enabled = false;
            owdPaneTop.Enabled = false;
            owdPaneLeft.Enabled = false;
            owdPaneRight.Enabled = false;
            owdPaneInto.Enabled = false;

            gridPaneRelativeDockingOptions.Visibility = Visibility.Collapsed;
            CurrentDropPane = null;
            OverlayButtonHover = OverlayButtonHover.None;
        }

        protected override void OnDeactivated(EventArgs e)
        {
            selectionBox.Visibility = Visibility.Hidden;

            base.OnDeactivated(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);


            DockableStyle currentPaneDockableStyle =
                _manager.DragPaneServices.FloatingWindow.HostedPane.GetCumulativeDockableStyle();

            selectionBox.Visibility = Visibility.Hidden;

            owdBottom.Enabled = (currentPaneDockableStyle & DockableStyle.BottomBorder) > 0;
            owdTop.Enabled = (currentPaneDockableStyle & DockableStyle.TopBorder) > 0;
            owdLeft.Enabled = (currentPaneDockableStyle & DockableStyle.LeftBorder) > 0;
            owdRight.Enabled = (currentPaneDockableStyle & DockableStyle.RightBorder) > 0;
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
            OverlayButtonHover = OverlayButtonHover.None;
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
                rectPane = _manager.SurfaceRectangle;
            else
                rectPane = CurrentDropPane.SurfaceRectangle;
            
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
                OverlayButtonHover = OverlayButtonHover.DropBorderLeft;
            else if (owdDock == owdRight)
                OverlayButtonHover = OverlayButtonHover.DropBorderRight;
            else if (owdDock == owdTop)
                OverlayButtonHover = OverlayButtonHover.DropBorderTop;
            else if (owdDock == owdBottom)
                OverlayButtonHover = OverlayButtonHover.DropBorderBottom;
            else if (owdDock == owdPaneInto)
                OverlayButtonHover = OverlayButtonHover.DropPaneInto;
            else if (owdDock == owdPaneRight)
                OverlayButtonHover = OverlayButtonHover.DropPaneRight;
            else if (owdDock == owdPaneTop)
                OverlayButtonHover = OverlayButtonHover.DropPaneTop;
            else if (owdDock == owdPaneLeft)
                OverlayButtonHover = OverlayButtonHover.DropPaneLeft;
            else if (owdDock == owdPaneBottom)
                OverlayButtonHover = OverlayButtonHover.DropPaneBottom;
            else
                OverlayButtonHover = OverlayButtonHover.None;


            selectionBox.Visibility = Visibility.Visible;

            _manager.DragPaneServices.FloatingWindow.OnShowSelectionBox();

        }

        OverlayButtonHover _overlayButtonHover = OverlayButtonHover.None;

        public OverlayButtonHover OverlayButtonHover
        {
            get
            { return _overlayButtonHover; }
            set
            {
                _overlayButtonHover = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("OverlayButtonHover"));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
