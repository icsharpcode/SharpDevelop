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
using System.Windows.Forms.Integration;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Threading;

namespace AvalonDock
{
    [ContentPropertyAttribute("ReferencedPane")]
    public class FlyoutPaneWindow : System.Windows.Window
    {
        static FlyoutPaneWindow()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlyoutPaneWindow), new FrameworkPropertyMetadata(typeof(FlyoutPaneWindow)));

            //AllowsTransparency slow down perfomance under XP/VISTA because rendering is enterely perfomed using CPU
            //Window.AllowsTransparencyProperty.OverrideMetadata(typeof(FlyoutPaneWindow), new FrameworkPropertyMetadata(true));

            Window.WindowStyleProperty.OverrideMetadata(typeof(FlyoutPaneWindow), new FrameworkPropertyMetadata(WindowStyle.None));
            Window.ShowInTaskbarProperty.OverrideMetadata(typeof(FlyoutPaneWindow), new FrameworkPropertyMetadata(false));
            Window.ResizeModeProperty.OverrideMetadata(typeof(FlyoutPaneWindow), new FrameworkPropertyMetadata(ResizeMode.NoResize));
            Control.BackgroundProperty.OverrideMetadata(typeof(FlyoutPaneWindow), new FrameworkPropertyMetadata(Brushes.Transparent));
        }

        public FlyoutPaneWindow()
        {
            Title = "AvalonDock_FlyoutPaneWindow";
        }


        WindowsFormsHost _winFormsHost = null;
        double _targetWidth;
        double _targetHeight;

        internal double TargetWidth
        {
            get { return _targetWidth; }
            set { _targetWidth = value; }
        }

        internal double TargetHeight
        {
            get { return _targetHeight; }
            set { _targetHeight = value; }
        }

        double _minLeft;
        double _minTop;

        internal double MinLeft
        {
            get { return _minLeft; }
            set { _minLeft = value; }
        }

        internal double MinTop
        {
            get { return _minTop; }
            set { _minTop = value; }
        }

        DockingManager _dockingManager = null;

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
            StartOpenAnimation();
            //Storyboard storyBoard = new Storyboard();
            //double originalLeft = this.Left;
            //double originalTop = this.Top;

            //AnchorStyle CorrectedAnchor = Anchor;

            //if (CorrectedAnchor == AnchorStyle.Left && FlowDirection == FlowDirection.RightToLeft)
            //    CorrectedAnchor = AnchorStyle.Right;
            //else if (CorrectedAnchor == AnchorStyle.Right && FlowDirection == FlowDirection.RightToLeft)
            //    CorrectedAnchor = AnchorStyle.Left;


            //if (CorrectedAnchor == AnchorStyle.Left || CorrectedAnchor == AnchorStyle.Right)
            //{
            //    DoubleAnimation anim = new DoubleAnimation(0.0, _targetWidth, new Duration(TimeSpan.FromMilliseconds(200)));
            //    Storyboard.SetTargetProperty(anim, new PropertyPath("Width"));
            //    this.Width = _targetWidth;
            //    //storyBoard.Children.Add(anim);
            //}
            //if (CorrectedAnchor == AnchorStyle.Right)
            //{
            //    //DoubleAnimation anim = new DoubleAnimation(this.Left, this.Left + this.ActualWidth, new Duration(TimeSpan.FromMilliseconds(500)));

            //    DoubleAnimation anim = new DoubleAnimation(this.Left, Left - _targetWidth, new Duration(TimeSpan.FromMilliseconds(200)));
            //    Storyboard.SetTargetProperty(anim, new PropertyPath("Left"));
            //    storyBoard.Children.Add(anim);
            //}

            //if (CorrectedAnchor == AnchorStyle.Top || CorrectedAnchor == AnchorStyle.Bottom)
            //{
            //    DoubleAnimation anim = new DoubleAnimation(0.0, _targetHeight, new Duration(TimeSpan.FromMilliseconds(200)));
            //    Storyboard.SetTargetProperty(anim, new PropertyPath("Height"));
            //    storyBoard.Children.Add(anim);
            //}
            //if (CorrectedAnchor == AnchorStyle.Bottom)
            //{
            //    DoubleAnimation anim = new DoubleAnimation(originalTop, originalTop - _targetHeight, new Duration(TimeSpan.FromMilliseconds(200)));
            //    Storyboard.SetTargetProperty(anim, new PropertyPath("Top"));
            //    storyBoard.Children.Add(anim);
            //}

            //{
            //    DoubleAnimation anim = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromMilliseconds(100)));
            //    Storyboard.SetTargetProperty(anim, new PropertyPath("Opacity"));
            //    //AllowsTransparency slow down perfomance under XP/VISTA because rendering is enterely perfomed using CPU
            //    //storyBoard.Children.Add(anim);
            //}

            //storyBoard.Completed += (anim, eventargs) =>
            //    {
            //        if (CorrectedAnchor == AnchorStyle.Left)
            //        {
            //            this.Left = originalLeft;
            //            this.Width = _targetWidth;
            //        }
            //        if (CorrectedAnchor == AnchorStyle.Right)
            //        {
            //            this.Left = originalLeft - _targetWidth;
            //            this.Width = _targetWidth;
            //        }
            //        if (CorrectedAnchor == AnchorStyle.Top)
            //        {
            //            this.Top = originalTop;
            //            this.Height = _targetHeight;
            //        }
            //        if (CorrectedAnchor == AnchorStyle.Bottom)
            //        {
            //            this.Top = originalTop - _targetHeight;
            //            this.Height = _targetHeight;
            //        }
            //    };

            //foreach (AnimationTimeline animTimeLine in storyBoard.Children)
            //{
            //    animTimeLine.FillBehavior = FillBehavior.Stop;
            //}

            //storyBoard.Begin(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            ReferencedPane.RestoreOriginalPane();

            base.OnClosed(e);

            _closed = true;
        }

        bool _closed = false;

        internal bool IsClosed
        {
            get { return _closed; }
        }

        //public AnchorStyle Anchor
        //{
        //    get { return (AnchorStyle)GetValue(AnchorPropertyKey.DependencyProperty); }
        //    protected set { SetValue(AnchorPropertyKey, value); }
        //}

        //// Using a DependencyProperty as the backing store for Anchor.  This enables animation, styling, binding, etc...
        //public static readonly DependencyPropertyKey AnchorPropertyKey =
        //    DependencyProperty.RegisterReadOnly("Anchor", typeof(AnchorStyle), typeof(FlyoutPaneWindow), new UIPropertyMetadata(AnchorStyle.Right));

        //public FlyoutDockablePane ReferencedPane
        //{
        //    get { return (FlyoutDockablePane)GetValue(ReferencedPaneProperty); }
        //    set { SetValue(ReferencedPaneProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for EmbeddedPane.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ReferencedPaneProperty =
        //    DependencyProperty.Register("ReferencedPane", typeof(FlyoutDockablePane), typeof(FlyoutPaneWindow));

        //AnchorStyle _anchor = AnchorStyle.Top;
        //public AnchorStyle Anchor
        //{
        //    get { return _anchor; }
        //    set { _anchor = value; }
        //}

        public AnchorStyle Anchor
        {
            get { return ReferencedPane.Anchor; }
        }

        FlyoutDockablePane _refPane;

        internal FlyoutDockablePane ReferencedPane
        {
            get { return _refPane; }
            //set 
            //{
            //    _refPane = value; 
            //}
        }
     

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (ReferencedPane == null)
                _refPane = this.Content as FlyoutDockablePane;

            if (ReferencedPane != null)
            {

                Content = ReferencedPane;

                _closingTimer = new DispatcherTimer(
                            new TimeSpan(0, 0, 2),
                            DispatcherPriority.Normal,
                            new EventHandler(OnCloseWindow),
                            Dispatcher.CurrentDispatcher);
            }


        }

        UIElement _resizer = null;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _resizer = GetTemplateChild("INT_Resizer") as UIElement;

            if (_resizer != null)
            {
                _resizer.MouseDown += new MouseButtonEventHandler(_resizer_MouseDown);
                _resizer.MouseMove += new MouseEventHandler(_resizer_MouseMove);
                _resizer.MouseUp += new MouseButtonEventHandler(_resizer_MouseUp);
            }
        }


        #region Resize management
        double originalWidth = 0.0;
        double originalHeight = 0.0;
        double originalLeft = 0.0;
        double originalTop = 0.0;
        Point ptStartDrag;

        private void _resizer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UIElement dragElement = sender as UIElement;

            originalLeft = Left;
            originalTop = Top;
            originalWidth = Width;
            originalHeight = Height;

            ptStartDrag = e.GetPosition(dragElement);
            dragElement.CaptureMouse();
        }

        private void _resizer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UIElement dragElement = sender as UIElement;

            if (dragElement.IsMouseCaptured)
            {
                Point ptMoveDrag = e.GetPosition(dragElement);
                AnchorStyle CorrectedAnchor = Anchor;

                if (CorrectedAnchor == AnchorStyle.Left && FlowDirection == FlowDirection.RightToLeft)
                    CorrectedAnchor = AnchorStyle.Right;
                else if (CorrectedAnchor == AnchorStyle.Right && FlowDirection == FlowDirection.RightToLeft)
                    CorrectedAnchor = AnchorStyle.Left;

                double deltaX = FlowDirection == FlowDirection.LeftToRight ? ptMoveDrag.X - ptStartDrag.X : ptStartDrag.X - ptMoveDrag.X;

                double newWidth = Width;
                double newHeight = Height;

                double newLeft = Left;
                double newTop = Top;

                if (CorrectedAnchor == AnchorStyle.Left)
                {
                    if (newWidth + deltaX < 4.0)
                        newWidth = 4.0;
                    else
                        newWidth += deltaX;

                }
                else if (CorrectedAnchor == AnchorStyle.Top)
                {
                    if (newHeight + (ptMoveDrag.Y - ptStartDrag.Y) < 4.0)
                        newHeight = 4.0;
                    else
                        newHeight += ptMoveDrag.Y - ptStartDrag.Y;

                }
                else if (CorrectedAnchor == AnchorStyle.Right)
                {
                    if (newWidth - (deltaX) < 4)
                    {
                        newLeft = originalLeft + originalWidth - 4;
                        newWidth = 4;
                    }
                    else
                    {
                        newLeft += deltaX;
                        newWidth -= deltaX;
                    }

                }
                else if (CorrectedAnchor == AnchorStyle.Bottom)
                {
                    if (newHeight - (ptMoveDrag.Y - ptStartDrag.Y) < 4)
                    {
                        newTop = originalTop + originalHeight - 4;
                        newHeight = 4;
                    }
                    else
                    {
                        newTop += ptMoveDrag.Y - ptStartDrag.Y;
                        newHeight -= ptMoveDrag.Y - ptStartDrag.Y;
                    }
                }

                //ResizingPanel.SetResizeHeight(ReferencedPane, ReferencedPane.ActualHeight);
                //ResizingPanel.SetResizeWidth(ReferencedPane, ReferencedPane.ActualWidth);

                Width = Math.Min(newWidth, MaxWidth);
                Height = Math.Min(newHeight, MaxHeight);

                Left = Math.Max(newLeft, MinLeft);
                Top = Math.Max(newTop, MinTop);

                ApplyRegion(new Rect(0, 0, this.Width, this.Height));
            }
        }

        private void _resizer_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UIElement dragElement = sender as UIElement;
            dragElement.ReleaseMouseCapture();

        }
        
        #endregion

        #region Closing window strategies


        DispatcherTimer _closingTimer = null;

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (!IsFocused && !IsKeyboardFocusWithin && !ReferencedPane.IsOptionsMenuOpened)
                _closingTimer.Start();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            _closingTimer.Stop();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (!IsMouseOver && !ReferencedPane.IsOptionsMenuOpened)
                _closingTimer.Start();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);

            if (!IsMouseOver)
                _closingTimer.Start();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            _closingTimer.Stop();
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            _closingTimer.Stop();
        }

        internal void StartCloseWindow()
        {
            _closingTimer.Start();
        }

        void OnCloseWindow(object sender, EventArgs e)
        {
            //options menu is open don't close the flyout window
            if (ReferencedPane.IsOptionsMenuOpened ||
                IsMouseDirectlyOver ||
                (_winFormsHost != null && _winFormsHost.IsFocused && _refPane.Items.Count > 0 && ((ManagedContent)_refPane.Items[0]).IsActiveContent))
            {
                _closingTimer.Start();
                return;
            }

            _closingTimer.Stop();

            if (IsClosed)
                return;

            StartCloseAnimation();
        }

        [DllImport("user32.dll")]
        static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateRectRgn(int left, int top, int right, int bottom);

        
        [DllImport("gdi32.dll")]
        static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);
       
        enum CombineRgnStyles : int
        {
            RGN_AND = 1,
            RGN_OR = 2,
            RGN_XOR = 3,
            RGN_DIFF = 4,
            RGN_COPY = 5,
            RGN_MIN = RGN_AND,
            RGN_MAX = RGN_COPY
        }
        

        public bool IsClosing { get; private set; }

        internal void StartCloseAnimation()
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

                int stepWidth = (int)(wnd_Width / 20);
                int stepHeight = (int)(wnd_Height / 20);    

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

        public bool IsOpening { get; private set; }

        internal void StartOpenAnimation()
        {
            AnchorStyle CorrectedAnchor = Anchor;

            if (CorrectedAnchor == AnchorStyle.Left && FlowDirection == FlowDirection.RightToLeft)
                CorrectedAnchor = AnchorStyle.Right;
            else if (CorrectedAnchor == AnchorStyle.Right && FlowDirection == FlowDirection.RightToLeft)
                CorrectedAnchor = AnchorStyle.Left;

            double wnd_Width = this._targetWidth > 0.0 ? this._targetWidth : this.ActualWidth;
            double wnd_Height = this._targetHeight > 0.0 ? this._targetHeight : this.ActualHeight;
            double wnd_Left = this.Left;
            double wnd_Top = this.Top;


            int wnd_TrimWidth = 0;
            int wnd_TrimHeight = 0;

            int stepWidth = (int)(wnd_Width / 8);
            int stepHeight = (int)(wnd_Height / 8);

            if (CorrectedAnchor == AnchorStyle.Left)
            {
                SetWindowRgn(new WindowInteropHelper(this).Handle, CreateRectRgn(0, 0, 0, (int)wnd_Height - wnd_TrimHeight), true);
                this.Left = wnd_Left - wnd_Width;
            }
            else if (CorrectedAnchor == AnchorStyle.Top)
            {
                SetWindowRgn(new WindowInteropHelper(this).Handle, CreateRectRgn(0, 0, (int)wnd_Width - wnd_TrimWidth, 0), true);
                this.Top = wnd_Top - wnd_Height;
            }

            DispatcherTimer animTimer = new DispatcherTimer();
            animTimer.Interval = TimeSpan.FromMilliseconds(2);

            animTimer.Tick += (sender, eventArgs) =>
            {
                bool stopTimer = false;
                switch (CorrectedAnchor)
                {
                    case AnchorStyle.Right:
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

                        //SetWindowRgn(new WindowInteropHelper(this).Handle, CreateRectRgn(0, 0, wnd_TrimWidth, (int)wnd_Height - wnd_TrimHeight), true);
                        Width = _targetWidth;
                        this.Left = newLeft;
                        ApplyRegion(new Rect(0, 0, wnd_TrimWidth, (int)wnd_Height - wnd_TrimHeight));
                        break;
                    case AnchorStyle.Left:
                        if (this.Left + stepWidth >= wnd_Left)
                        {
                            this.Left = wnd_Left;
                            wnd_TrimWidth = (int)wnd_Width;
                            stopTimer = true;
                        }
                        else
                        {
                            this.Left += stepWidth;
                            wnd_TrimWidth += stepWidth;
                        }

                        //SetWindowRgn(new WindowInteropHelper(this).Handle, CreateRectRgn((int)(wnd_Left - this.Left), 0, (int)(wnd_Width), (int)wnd_Height - wnd_TrimHeight), true);
                        ApplyRegion(
                            new Rect((int)(wnd_Left - this.Left), 0, (int)(wnd_Width), (int)wnd_Height - wnd_TrimHeight));
                        Width = _targetWidth;
                        break;
                    case AnchorStyle.Bottom:
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

                        //SetWindowRgn(new WindowInteropHelper(this).Handle, CreateRectRgn(0, 0, (int)wnd_Width - wnd_TrimWidth, wnd_TrimHeight), true);
                        ApplyRegion(
                            new Rect(0, 0, (int)wnd_Width - wnd_TrimWidth, wnd_TrimHeight));

                        Height = _targetHeight;
                        this.Top = newTop;
                        break;
                    case AnchorStyle.Top:
                        if (this.Top + stepHeight >= wnd_Top)
                        {
                            this.Top = wnd_Top;
                            wnd_TrimHeight = (int)wnd_Height;
                            stopTimer = true;
                        }
                        else
                        {
                            this.Top += stepHeight;
                            wnd_TrimHeight += stepHeight;
                        }

                        //SetWindowRgn(new WindowInteropHelper(this).Handle, CreateRectRgn(0, (int)(wnd_Top - this.Top), (int)wnd_Width - wnd_TrimWidth, (int)(wnd_Height)), true);
                        ApplyRegion(
                            new Rect(0, (int)(wnd_Top - this.Top), (int)wnd_Width - wnd_TrimWidth, (int)(wnd_Height)));

                        Height = _targetHeight;
                        break;
                }

                if (stopTimer)
                {
                    //SetWindowRgn(new WindowInteropHelper(this).Handle, IntPtr.Zero, false);
                    UpdateClipRegion();
                    animTimer.Stop();
                    IsOpening = false;
                }
            };

            IsOpening = true;
            animTimer.Start();

        
        }

        //internal void StartCloseAnimation()
        //{
        //    AnchorStyle CorrectedAnchor = Anchor;

        //    if (CorrectedAnchor == AnchorStyle.Left && FlowDirection == FlowDirection.RightToLeft)
        //        CorrectedAnchor = AnchorStyle.Right;
        //    else if (CorrectedAnchor == AnchorStyle.Right && FlowDirection == FlowDirection.RightToLeft)
        //        CorrectedAnchor = AnchorStyle.Left;


        //    //Let closing animation to occur
        //    //Here we get a reference to a storyboard resource with a name ClosingStoryboard and 
        //    //wait that it completes before closing the window
        //    FrameworkElement targetElement = GetTemplateChild("INT_pane") as FrameworkElement;
        //    if (targetElement != null)
        //    {
        //        Storyboard storyBoard = new Storyboard();

        //        if (CorrectedAnchor == AnchorStyle.Left || CorrectedAnchor == AnchorStyle.Right)
        //        {
        //            DoubleAnimation anim = new DoubleAnimation(this.ActualWidth, 0.0, new Duration(TimeSpan.FromMilliseconds(500)));
        //            Storyboard.SetTargetProperty(anim, new PropertyPath("Width"));
        //            //storyBoard.Children.Add(anim);
        //        }
        //        if (CorrectedAnchor == AnchorStyle.Right)
        //        {
        //            DoubleAnimation anim = new DoubleAnimation(this.Left, this.Left + this.ActualWidth, new Duration(TimeSpan.FromMilliseconds(500)));
        //            Storyboard.SetTargetProperty(anim, new PropertyPath("Left"));
        //            storyBoard.Children.Add(anim);
        //        }
        //        if (CorrectedAnchor == AnchorStyle.Top || CorrectedAnchor == AnchorStyle.Bottom)
        //        {
        //            DoubleAnimation anim = new DoubleAnimation(this.Height, 0.0, new Duration(TimeSpan.FromMilliseconds(500)));
        //            Storyboard.SetTargetProperty(anim, new PropertyPath("Height"));
        //            storyBoard.Children.Add(anim);
        //        }
        //        if (CorrectedAnchor == AnchorStyle.Bottom)
        //        {
        //            DoubleAnimation anim = new DoubleAnimation(this.Top, this.Top + this.Height, new Duration(TimeSpan.FromMilliseconds(500)));
        //            Storyboard.SetTargetProperty(anim, new PropertyPath("Top"));
        //            storyBoard.Children.Add(anim);
        //        }

        //        {
        //            //DoubleAnimation anim = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromMilliseconds(500)));
        //            //Storyboard.SetTargetProperty(anim, new PropertyPath("Opacity"));
        //            //AllowsTransparency slow down perfomance under XP/VISTA because rendering is enterely perfomed using CPU
        //            //storyBoard.Children.Add(anim);
        //        }

        //        storyBoard.Completed += (animation, eventArgs) =>
        //        {
        //            if (!IsClosed)
        //                Close();
        //        };

        //        foreach (AnimationTimeline animTimeLine in storyBoard.Children)
        //        {
        //            animTimeLine.FillBehavior = FillBehavior.Stop;
        //        }

        //        storyBoard.Begin(this);
        //    }
        
        //}


        
        #endregion


        #region Clipping Region

        protected override void OnActivated(EventArgs e)
        {
            if (!IsOpening && !IsClosing)
                UpdateClipRegion();

            base.OnActivated(e);
        }

        internal void UpdateClipRegion()
        {
            //ApplyRegion(_lastApplyRect.IsEmpty ? new Rect(0, 0, this.Width, this.Height) : _lastApplyRect);
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

                    Rect flRect = new Rect(
                        PointFromScreen(new Point(fl.Left, fl.Top)), 
                        PointFromScreen(new Point(fl.Left + fl.Width, fl.Top + fl.Height)));

                    if (flRect.IntersectsWith(wndRect))
                        otherRects.Add(Rect.Intersect(flRect, wndRect));
                }

                IntPtr hDestRegn = CreateRectRgn(
                        (int)wndRect.Left,
                        (int)wndRect.Top,
                        (int)wndRect.Right,
                        (int)wndRect.Bottom);

                foreach (Rect otherRect in otherRects)
                {
                    IntPtr otherWin32Rect = CreateRectRgn(
                        (int)otherRect.Left,
                        (int)otherRect.Top,
                        (int)otherRect.Right,
                        (int)otherRect.Bottom);

                    CombineRgn(hDestRegn, hDestRegn, otherWin32Rect, (int)CombineRgnStyles.RGN_DIFF);
                }


                SetWindowRgn(new WindowInteropHelper(this).Handle, hDestRegn, true);
            }        
        }

        #endregion
    }
}
