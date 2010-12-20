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
using System.Windows.Forms.Integration;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Threading;

namespace AvalonDock
{
    /// <summary>
    /// Implements a flyout window
    /// </summary>
    /// <remarks>
    /// The flyout window is showed when user activate a ManagedContent that is in the AutoHide state.
    /// The flyout window appears from a border of the parent docking manager according to the property Anchor.
    /// It contains the selected content under the property ReferencedPane.
    /// When user move focus to an other object outside this window, it automaticcaly is closed.
    /// </remarks>
    [ContentPropertyAttribute("ReferencedPane")]
    public class FlyoutPaneWindow : AvalonDockWindow
    {
        /// <summary>
        /// Points to the internal windows forms control (if exist)
        /// </summary>
        WindowsFormsHost _winFormsHost = null;

        double _targetWidth;
        double _targetHeight;

        /// <summary>
        /// Gets or sets the final width of the window
        /// </summary>
        internal double TargetWidth
        {
            get { return _targetWidth; }
            set { _targetWidth = value; }
        }

        /// <summary>
        /// Gets or sets the final height of the window
        /// </summary>
        internal double TargetHeight
        {
            get { return _targetHeight; }
            set { _targetHeight = value; }
        }

        DockingManager _dockingManager = null;

        internal DockablePaneAnchorTab AnchorTabActivating { get; set; }

        static FlyoutPaneWindow()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlyoutPaneWindow), new FrameworkPropertyMetadata(typeof(FlyoutPaneWindow)));

            //AllowsTransparency slow down perfomance under XP/VISTA because rendering is enterely perfomed using CPU
            //Window.AllowsTransparencyProperty.OverrideMetadata(typeof(FlyoutPaneWindow), new FrameworkPropertyMetadata(true));

            WindowStyleProperty.OverrideMetadata(typeof(FlyoutPaneWindow), new FrameworkPropertyMetadata(WindowStyle.None));
            ShowInTaskbarProperty.OverrideMetadata(typeof(FlyoutPaneWindow), new FrameworkPropertyMetadata(false));
            ResizeModeProperty.OverrideMetadata(typeof(FlyoutPaneWindow), new FrameworkPropertyMetadata(ResizeMode.NoResize));
            BackgroundProperty.OverrideMetadata(typeof(FlyoutPaneWindow), new FrameworkPropertyMetadata(Brushes.Transparent));
        }

        public FlyoutPaneWindow()
        {
            Title = "AvalonDock_FlyoutPaneWindow";
        }

        public FlyoutPaneWindow(DockingManager manager, DockableContent content)
            : this()
        {
            //create a new temporary pane
            _refPane = new FlyoutDockablePane(content);
            _dockingManager = manager;

            _winFormsHost = ReferencedPane.GetLogicalChildContained<WindowsFormsHost>();

            if (_winFormsHost != null)
            {
                AllowsTransparency = false;
            }

            this.Loaded += new RoutedEventHandler(FlyoutPaneWindow_Loaded);
        }


        void FlyoutPaneWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //prevents user from activating this window
            //WindowInteropWrapper wiw = new WindowInteropWrapper(this);
            //wiw.WindowActivating += (s, args) => args.Cancel = true;

            //Open the pane with or without animation
            OpenPane();
        }


        public override void OnApplyTemplate()
        {
            var resizer = GetTemplateChild("PART_Resizer") as Resizer;

            if (resizer != null) resizer.DragStarted += (s, e) =>
            {
                IsResizing = true;

                ShowResizerOverlayWindow(s as Resizer);
                Debug.WriteLine(string.Format("resizer.DragStarted() Rect->{0}", new Rect(Left, Top, Width, Height)));
            };

            if (resizer != null) resizer.DragDelta += (s, e) =>
            {
                switch (CorrectedAnchor)
                {
                    case AnchorStyle.Left:
                    case AnchorStyle.Right:
                        {
                            double newLeft = _initialStartPoint.X + e.HorizontalChange;
                            newLeft = Math.Max(newLeft, 0.0);
                            newLeft = Math.Min(newLeft, MaxWidth);

                            Canvas.SetLeft(_resizerGhost, newLeft);
                        }
                        break;
                    case AnchorStyle.Top:
                    case AnchorStyle.Bottom:
                        {
                            double newTop = _initialStartPoint.Y + e.VerticalChange;
                            newTop = Math.Max(newTop, 0.0);
                            newTop = Math.Min(newTop, MaxHeight);

                            Canvas.SetTop(_resizerGhost, newTop);
                        }
                        break;

                }

                Debug.WriteLine(string.Format("resizer.DragDelta() Rect->{0}", new Rect(Left, Top, Width, Height)));

            };

            if (resizer != null) resizer.DragCompleted += (s, e) =>
            {
                switch (CorrectedAnchor)
                {
                    case AnchorStyle.Left:
                        {
                            Width = Canvas.GetLeft(_resizerGhost) + _resizerGhost.Width;
                            ApplyRegion(new Rect(0, 0, this.Width, this.Height));
                        }
                        break;
                    case AnchorStyle.Right:
                        {
                            double newWidth = MaxWidth - Canvas.GetLeft(_resizerGhost);
                            Left -= newWidth - Width;
                            Width = newWidth;


                            ApplyRegion(new Rect(0, 0, this.Width, this.Height));
                        }
                        break;
                    case AnchorStyle.Top:
                        {
                            Height = Canvas.GetTop(_resizerGhost) + _resizerGhost.Height;
                            ApplyRegion(new Rect(0, 0, this.Width, this.Height));
                        }
                        break;
                    case AnchorStyle.Bottom:
                        {
                            double newHeight = MaxHeight - Canvas.GetTop(_resizerGhost);
                            Top -= newHeight - Height;
                            Height = newHeight;


                            ApplyRegion(new Rect(0, 0, this.Width, this.Height));
                        }
                        break;
                }

                IsResizing = false;
                SaveFlyoutSizeToContent();
                HideResizerOverlayWindow();
                Debug.WriteLine(string.Format("resizer.DragCompleted() Rect->{0}", new Rect(Left, Top, Width, Height)));

            };
            
            base.OnApplyTemplate();
        }

        void SaveFlyoutSizeToContent()
        {
            if (ReferencedPane.ActualWidth > 0.0 &&
                ReferencedPane.ActualHeight > 0.0)
            {
                var flyoutContent = ReferencedPane.SelectedItem as DockableContent;

                if (Anchor == AnchorStyle.Left ||
                    Anchor == AnchorStyle.Right)
                    flyoutContent.FlyoutWindowSize =
                        new Size(ReferencedPane.ActualWidth, flyoutContent.FlyoutWindowSize.Height <= 0 ? ReferencedPane.ActualHeight : flyoutContent.FlyoutWindowSize.Height);
                else
                    flyoutContent.FlyoutWindowSize =
                        new Size(flyoutContent.FlyoutWindowSize.Width <= 0 ? ReferencedPane.ActualWidth : flyoutContent.FlyoutWindowSize.Width, ReferencedPane.ActualHeight);

                Debug.WriteLine(string.Format("Save flyout size for content '{0}' -> {1}", flyoutContent.Name, flyoutContent.FlyoutWindowSize));
            }        
        }

        /// <summary>
        /// Handles the closed event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            StopClosingTimer();

            ReferencedPane.RestoreOriginalPane();

            base.OnClosed(e);

            IsClosed = true;
        }


        /// <summary>
        /// Gets a value indicating i fthis window is closed
        /// </summary>
        internal bool IsClosed { get; private set; }

        /// <summary>
        /// Anchor of the flyout window
        /// </summary>
        public AnchorStyle Anchor
        {
            get { return ReferencedPane.Anchor; }
        }

        /// <summary>
        /// Get th anchor of the window corrected with the FlowDirection property
        /// </summary>
        AnchorStyle CorrectedAnchor
        {
            get
            {
                if (Anchor == AnchorStyle.Left && FlowDirection == FlowDirection.RightToLeft)
                    return AnchorStyle.Right;
                else if (Anchor == AnchorStyle.Right && FlowDirection == FlowDirection.RightToLeft)
                    return AnchorStyle.Left;

                return Anchor;
            }
        }

        /// <summary>
        /// Refrenced pane
        /// </summary>
        FlyoutDockablePane _refPane;

        /// <summary>
        /// Gets the pane that is hosted in flyout window
        /// </summary>
        internal FlyoutDockablePane ReferencedPane
        {
            get { return _refPane; }
        }
     
        /// <summary>
        /// Initialize the window
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            //Attach the referenced pane to show
            if (ReferencedPane == null)
                _refPane = this.Content as FlyoutDockablePane;

            if (ReferencedPane != null)
            {
                //move the pane under me as content
                //ReferencePane now changes visual tree!
                Content = ReferencedPane;
                InitClosingTimer();
            }
        }

        /// <summary>
        /// Popup window that hosts the resizer thumb
        /// </summary>
        /// <remarks>Resizer is host in a popup window becaus it needs to overlay winforms controls.</remarks>
        Window _resizerPopup = null;

        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();

        //    _resizerPopup = GetTemplateChild("INT_ResizerPopup") as Popup;
        //} 


        #region Resize management

        Border _resizerGhost = null;
        Window _resizerWindowHost = null;
        Vector _initialStartPoint;

        void ShowResizerOverlayWindow(Resizer splitter)
        {
            _resizerGhost = new Border()
            {
                Background = Brushes.Black,
                Opacity = 0.7
            };

            if (CorrectedAnchor == AnchorStyle.Left || CorrectedAnchor == AnchorStyle.Right)
            {
                _resizerGhost.Width = splitter.Width;
                _resizerGhost.Height = MaxHeight;
            }
            else
            {
                _resizerGhost.Height = splitter.Height;
                _resizerGhost.Width = MaxWidth;
            }

            Canvas panelHostResizer = new Canvas()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch
            };

            panelHostResizer.Children.Add(_resizerGhost);

            _resizerWindowHost = new Window()
            {
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = System.Windows.WindowStyle.None,
                ShowInTaskbar = false,
                AllowsTransparency = true,
                Background = null,
                Width = MaxWidth,
                Height = MaxHeight,
                Left = Left,
                Top = Top,
                ShowActivated = false,
                Owner = this,
                Content = panelHostResizer
            };

            if (CorrectedAnchor == AnchorStyle.Right)
                _resizerWindowHost.Left = Left - MaxWidth + Width;
            else if (CorrectedAnchor == AnchorStyle.Bottom)
                _resizerWindowHost.Top = Top - MaxHeight + Height;

            if (CorrectedAnchor == AnchorStyle.Left)
            {
                Canvas.SetLeft(_resizerGhost, Width - splitter.Width);
            }
            else if (CorrectedAnchor == AnchorStyle.Right)
            {
                Canvas.SetLeft(_resizerGhost, MaxWidth - Width);
            }
            else if (CorrectedAnchor == AnchorStyle.Top)
            {
                Canvas.SetTop(_resizerGhost, Height - splitter.Height);
            }
            else if (CorrectedAnchor == AnchorStyle.Bottom)
            {
                Canvas.SetTop(_resizerGhost, MaxHeight - Height);
            }

            _initialStartPoint = new Vector(Canvas.GetLeft(_resizerGhost), Canvas.GetTop(_resizerGhost));

            _resizerWindowHost.Show();
        }

        void HideResizerOverlayWindow()
        {
            if (_resizerWindowHost != null)
            {
                _resizerWindowHost.Close();
                _resizerWindowHost = null;
            }
        }

        /// <summary>
        /// Gets a value indicating if user is resizer the window
        /// </summary>
        public bool IsResizing { get; private set; }
         
        #endregion

        #region Closing window strategies


        DispatcherTimer _closingTimer = null;

        /// <summary>
        /// Creates the closing timer
        /// </summary>
        void InitClosingTimer()
        {
            if (_closingTimer == null)
            {
                _closingTimer = new DispatcherTimer(
                            new TimeSpan(0, 0, 1),
                            DispatcherPriority.Background,
                            new EventHandler(ForceCloseWindow),
                            Dispatcher.CurrentDispatcher);
                _closingTimer.Start();
            }
        }

        /// <summary>
        /// Stop the closing timer
        /// </summary>
        void StopClosingTimer()
        {
            if (_closingTimer != null)
            {
                _closingTimer.Stop();
                _closingTimer = null;
            }
        }

        /// <summary>
        /// This handler is called when the closing time delay is elapsed (user is focusing to something else of the UI)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ForceCloseWindow(object sender, EventArgs e)
        {
            //Debug.WriteLine(string.Format("IsMouseOver={0}", IsMouseOverPane));
            //Debug.WriteLine(string.Format("IsFocused={0}", _winFormsHost != null && _winFormsHost.IsFocused));

            //options menu is open don't close the flyout window
            if (ReferencedPane.IsOptionsMenuOpen ||
                IsMouseOverPane ||
                (_winFormsHost != null && _winFormsHost.IsFocused && _refPane.Items.Count > 0 && ((ManagedContent)_refPane.Items[0]).IsActiveContent) ||
                IsActive ||
                _refPane.IsKeyboardFocusWithin    ||
                _refPane.ContainsActiveContent ||
                (AnchorTabActivating != null && AnchorTabActivating.IsMouseOver) ||
                IsResizing ||
                (_resizerPopup != null && _resizerPopup.IsMouseOver))
            {
                return;
            }

            StopClosingTimer();

            if (IsClosed)
                return;

            ClosePane();
        }

        bool IsMouseOverPane
        {
            get
            {
                InteropHelper.Win32Point pt = new InteropHelper.Win32Point();
                if (!InteropHelper.GetCursorPos(ref pt))
                    return false;

                Point ptMouse = PointToScreen(new Point());
                //Debug.WriteLine(string.Format("{0}-{1}", pt.X, pt.Y));

                Rect rectWindow = new Rect(ptMouse.X, ptMouse.Y, Width, Height);
                return rectWindow.Contains(new Point(pt.X,pt.Y));
            }
        }


        #endregion

        #region Open/Close Flyout window

        /// <summary>
        /// Gets a value indicating if the flyout window is closing
        /// </summary>
        public bool IsClosing { get; private set; }

        /// <summary>
        /// Initiate a closing animation
        /// </summary>
        void CloseWidthAnimation()
        {
            AnchorStyle CorrectedAnchor = Anchor;

            if (CorrectedAnchor == AnchorStyle.Left && FlowDirection == FlowDirection.RightToLeft)
                CorrectedAnchor = AnchorStyle.Right;
            else if (CorrectedAnchor == AnchorStyle.Right && FlowDirection == FlowDirection.RightToLeft)
                CorrectedAnchor = AnchorStyle.Left;

            double wnd_Width = this.ActualWidth;
            double wnd_Height = this.ActualHeight;
            double wnd_Left = this.Left;
            double wnd_Top = this.Top;

            int wnd_TrimWidth = (int)wnd_Width;
            int wnd_TrimHeight = (int)wnd_Height;

            int stepWidth = (int)(wnd_Width / 4);
            int stepHeight = (int)(wnd_Height / 4);    

            DispatcherTimer animTimer = new DispatcherTimer();
            animTimer.Interval = TimeSpan.FromMilliseconds(1);

            animTimer.Tick += (sender, eventArgs) =>
            {
                bool stopTimer = false;
                double newLeft = 0.0;
                double newTop = 0.0;
                switch (CorrectedAnchor)
                {
                    case AnchorStyle.Right:
                        newLeft = this.Left;
                        if (this.Left + stepWidth >= wnd_Left + wnd_Width)
                        {
                            newLeft = wnd_Left + wnd_Width;
                            wnd_TrimWidth = 0;
                            stopTimer = true;
                        }
                        else
                        {
                            newLeft += stepWidth;
                            wnd_TrimWidth -= stepWidth;
                            wnd_TrimWidth = Math.Max(wnd_TrimWidth, 0);
                        }

                        //SetWindowRgn(new WindowInteropHelper(this).Handle, CreateRectRgn(0, 0, wnd_TrimWidth, wnd_TrimHeight), true);
                        ApplyRegion(new Rect(0, 0, wnd_TrimWidth, wnd_TrimHeight));
                        this.Left = newLeft;
                        break;
                    case AnchorStyle.Left:
                        newLeft = this.Left;
                        if (this.Left - stepWidth <= wnd_Left - wnd_Width)
                        {
                            newLeft = wnd_Left - wnd_Width;
                            wnd_TrimWidth = 0;
                            stopTimer = true;
                        }
                        else
                        {
                            newLeft -= stepWidth;
                            wnd_TrimWidth -= stepWidth;
                            wnd_TrimWidth = Math.Max(wnd_TrimWidth, 0);
                        }

                        this.Left = newLeft;
                        //SetWindowRgn(new WindowInteropHelper(this).Handle, CreateRectRgn((int)(wnd_Left - this.Left), 0, (int)(wnd_Width), wnd_TrimHeight), true);
                        ApplyRegion(
                            new Rect((int)(wnd_Left - this.Left), 0, (int)(wnd_Width), wnd_TrimHeight));
                        break;
                    case AnchorStyle.Bottom:
                        newTop = this.Top;
                        if (this.Top + stepHeight >= wnd_Top + wnd_Height)
                        {
                            newTop = wnd_Top + wnd_Height;
                            wnd_TrimHeight = 0;
                            stopTimer = true;
                        }
                        else
                        {
                            newTop += stepHeight;
                            wnd_TrimHeight -= stepHeight;
                            wnd_TrimHeight = Math.Max(wnd_TrimHeight, 0);
                        }

                        //SetWindowRgn(new WindowInteropHelper(this).Handle, CreateRectRgn(0, 0, wnd_TrimWidth, wnd_TrimHeight), true);
                        ApplyRegion(
                            new Rect(0, 0, wnd_TrimWidth, wnd_TrimHeight));
                        this.Top = newTop;
                        break;
                    case AnchorStyle.Top:
                        newTop = this.Top;
                        if (this.Top - stepHeight <= wnd_Top - wnd_Height)
                        {
                            newTop = wnd_Top - wnd_Height;
                            wnd_TrimHeight = 0;
                            stopTimer = true;
                        }
                        else
                        {
                            newTop -= stepHeight;
                            wnd_TrimHeight -= stepHeight;
                            wnd_TrimHeight = Math.Max(wnd_TrimWidth, 0);
                        }

                        this.Top = newTop;
                        ApplyRegion(
                            new Rect(0, (int)(wnd_Top - this.Top), wnd_TrimWidth, (int)(wnd_Height)));
                        //SetWindowRgn(new WindowInteropHelper(this).Handle, CreateRectRgn(0, (int)(wnd_Top - this.Top), wnd_TrimWidth, (int)(wnd_Height)), true);
                        break;
                }

                if (stopTimer)
                {
                    //window is being closed
                    Width = 0.0;
                    Height = 0.0;
                    animTimer.Stop();
                    if (!IsClosed)
                        Close();
                    IsClosing = false;
                }
            };

            IsClosing = true;
            animTimer.Start();
        }

        /// <summary>
        /// Gets a flag indicating if flyout window is opening
        /// </summary>
        public bool IsOpening { get; private set; }

        /// <summary>
        /// Initiate an opening animation
        /// </summary>
        void OpenWidthAnimation()
        {
            double wnd_Width = this._targetWidth > 0.0 ? this._targetWidth : this.ActualWidth;
            double wnd_Height = this._targetHeight > 0.0 ? this._targetHeight : this.ActualHeight;
            double wnd_Left = this.Left;
            double wnd_Top = this.Top;

            int wnd_TrimWidth = 0;
            int wnd_TrimHeight = 0;

            int stepWidth = (int)(wnd_Width / 4);
            int stepHeight = (int)(wnd_Height / 4);

            if (CorrectedAnchor == AnchorStyle.Left)
            {
                InteropHelper.SetWindowRgn(new WindowInteropHelper(this).Handle, InteropHelper.CreateRectRgn(0, 0, 0, (int)wnd_Height - wnd_TrimHeight), true);
                this.Left = wnd_Left - wnd_Width;
            }
            else if (CorrectedAnchor == AnchorStyle.Top)
            {
                InteropHelper.SetWindowRgn(new WindowInteropHelper(this).Handle, InteropHelper.CreateRectRgn(0, 0, (int)wnd_Width - wnd_TrimWidth, 0), true);
                this.Top = wnd_Top - wnd_Height;
            }

            DispatcherTimer animTimer = new DispatcherTimer();
            animTimer.Interval = TimeSpan.FromMilliseconds(1);

            animTimer.Tick += (sender, eventArgs) =>
            {
                bool stopTimer = false;
                switch (CorrectedAnchor)
                {
                    case AnchorStyle.Right:
                        {
                            double newLeft = this.Left;
                            if (this.Left - stepWidth <= wnd_Left - wnd_Width)
                            {
                                newLeft = wnd_Left - wnd_Width;
                                wnd_TrimWidth = (int)wnd_Width;
                                stopTimer = true;
                            }
                            else
                            {
                                newLeft -= stepWidth;
                                wnd_TrimWidth += stepWidth;
                            }

                            Width = _targetWidth;
                            this.Left = newLeft;
                            ApplyRegion(new Rect(0, 0, wnd_TrimWidth, (int)wnd_Height - wnd_TrimHeight));
                        }
                        break;
                    case AnchorStyle.Left:
                        {
                            double newLeft = this.Left;
                            if (this.Left + stepWidth >= wnd_Left)
                            {
                                newLeft = wnd_Left;
                                wnd_TrimWidth = (int)wnd_Width;
                                stopTimer = true;
                            }
                            else
                            {
                                newLeft += stepWidth;
                                wnd_TrimWidth += stepWidth;
                            }

                            ApplyRegion(
                                new Rect((int)(wnd_Left - this.Left), 0, (int)(wnd_Width), (int)wnd_Height - wnd_TrimHeight));
                            
                            Width = _targetWidth;
                            this.Left = newLeft;
                        }
                        break;
                    case AnchorStyle.Bottom:
                        {
                            double newTop = this.Top;
                            if (this.Top - stepHeight <= wnd_Top - wnd_Height)
                            {
                                newTop = wnd_Top - wnd_Height;
                                wnd_TrimHeight = (int)wnd_Height;
                                stopTimer = true;
                            }
                            else
                            {
                                newTop -= stepHeight;
                                wnd_TrimHeight += stepHeight;
                            }

                            ApplyRegion(
                                new Rect(0, 0, (int)wnd_Width - wnd_TrimWidth, wnd_TrimHeight));

                            Height = _targetHeight;
                            this.Top = newTop;
                            
                        }
                        break;
                    case AnchorStyle.Top:
                        {
                            double newTop = this.Top;
                            if (this.Top + stepHeight >= wnd_Top)
                            {
                                newTop = wnd_Top;
                                wnd_TrimHeight = (int)wnd_Height;
                                stopTimer = true;
                            }
                            else
                            {
                                newTop += stepHeight;
                                wnd_TrimHeight += stepHeight;
                            }

                            ApplyRegion(
                                new Rect(0, (int)(wnd_Top - this.Top), (int)wnd_Width - wnd_TrimWidth, (int)(wnd_Height)));

                            Height = _targetHeight;
                            this.Top = newTop;
                        }
                        break;
                }

                if (stopTimer)
                {
                    UpdatePositionAndSize();
                    animTimer.Stop();
                    IsOpening = false;
                }
            };

            IsOpening = true;
            animTimer.Start();
        }

        /// <summary>
        /// Open the flyout window with or without animation depending on the ShowAnimated flag
        /// </summary>
        void OpenPane()
        {
            if (_dockingManager.IsAnimationEnabled)
            {
                OpenWidthAnimation();
            }
            else
            {
                switch (CorrectedAnchor)
                {
                    case AnchorStyle.Left:
                        Width = this._targetWidth;
                        break;
                    case AnchorStyle.Right:
                        Width = this._targetWidth;
                        Left -= this.Width;
                        break;
                    case AnchorStyle.Top:
                        Height = this._targetHeight;
                        break;
                    case AnchorStyle.Bottom:
                        Height = this._targetHeight;
                        Top -= this.Height;
                        break;
                }

                UpdatePositionAndSize();
                //ShowResizerPopup();
                //StartClosingTimer();
            }
        }

        /// <summary>
        /// Close the flyout window with or without animation depending on the ShowAnimated flag
        /// </summary>
        void ClosePane()
        {
            if (_dockingManager.IsAnimationEnabled)
            {
                CloseWidthAnimation();
            }
            else
            {
                if (!IsClosed)
                    Close();
            }
        }

        #endregion


        #region Clipping Region

        protected override void OnActivated(EventArgs e)
        {
            if (!IsOpening && !IsClosing)
                UpdatePositionAndSize();

            base.OnActivated(e);
        }

        //protected override void OnDeactivated(EventArgs e)
        //{
        //    //StartClosingTimer();
        //    base.OnDeactivated(e);
        //}

        internal void UpdatePositionAndSize()
        {
            ApplyRegion(new Rect(0, 0, Width, Height));
        }

        Rect _lastApplyRect = Rect.Empty;

        void ApplyRegion(Rect wndRect)
        {
            if (!this.CanTransform())
                return;

            wndRect = new Rect(
                this.TransformFromDeviceDPI(wndRect.TopLeft),
                this.TransformFromDeviceDPI(wndRect.Size));

            _lastApplyRect = wndRect;

            if (PresentationSource.FromVisual(this) == null)
                return;


            if (_dockingManager != null)
            {
                List<Rect> otherRects = new List<Rect>();

                foreach (Window fl in Window.GetWindow(_dockingManager).OwnedWindows)
                {
                    //not with myself!
                    if (fl == this)
                        continue;

                    if (!fl.IsVisible)
                        continue;

                    //Issue 11545, thx to SrdjanPolic
                    Rect flRect = new Rect(
                    PointFromScreen(new Point(fl.Left, fl.Top)),
                    PointFromScreen(new Point(fl.Left + fl.RestoreBounds.Width, fl.Top + fl.RestoreBounds.Height)));

                    if (flRect.IntersectsWith(wndRect) && fl.AllowsTransparency == false)
                        otherRects.Add(Rect.Intersect(flRect, wndRect));

                    //Rect flRect = new Rect(
                    //    PointFromScreen(new Point(fl.Left, fl.Top)), 
                    //    PointFromScreen(new Point(fl.Left + fl.Width, fl.Top + fl.Height)));

                    //if (flRect.IntersectsWith(wndRect))
                    //    otherRects.Add(Rect.Intersect(flRect, wndRect));
                }

                IntPtr hDestRegn = InteropHelper.CreateRectRgn(
                        (int)wndRect.Left,
                        (int)wndRect.Top,
                        (int)wndRect.Right,
                        (int)wndRect.Bottom);

                foreach (Rect otherRect in otherRects)
                {
                    IntPtr otherWin32Rect = InteropHelper.CreateRectRgn(
                        (int)otherRect.Left,
                        (int)otherRect.Top,
                        (int)otherRect.Right,
                        (int)otherRect.Bottom);

                    InteropHelper.CombineRgn(hDestRegn, hDestRegn, otherWin32Rect, (int)InteropHelper.CombineRgnStyles.RGN_DIFF);
                }


                InteropHelper.SetWindowRgn(new WindowInteropHelper(this).Handle, hDestRegn, true);
            }        
        }
        #endregion
    }
}
