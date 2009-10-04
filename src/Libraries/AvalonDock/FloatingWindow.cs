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
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace AvalonDock
{
    public abstract class FloatingWindow : Window, INotifyPropertyChanged
    {
        static FloatingWindow()
        {
            Window.ShowInTaskbarProperty.OverrideMetadata(typeof(FloatingWindow), new FrameworkPropertyMetadata(false));
            Window.WindowStyleProperty.OverrideMetadata(typeof(FloatingWindow), new FrameworkPropertyMetadata(WindowStyle.ToolWindow));
        }


        public FloatingWindow()
        {
            this.Loaded += new RoutedEventHandler(OnLoaded);
            this.Unloaded += new RoutedEventHandler(OnUnloaded);

            this.SizeChanged += new SizeChangedEventHandler(FloatingWindow_SizeChanged);

            this.CommandBindings.Add(new CommandBinding(SetAsDockableWindowCommand, OnExecuteCommand, OnCanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(SetAsFloatingWindowCommand, OnExecuteCommand, OnCanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(TabbedDocumentCommand, OnExecuteCommand, OnCanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(CloseCommand, OnExecuteCommand, OnCanExecuteCommand));
        }

        void FloatingWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (HostedPane != null)
            {
                foreach (ManagedContent c in HostedPane.Items)
                    c.FloatingWindowSize = new Size(Width, Height);

                ResizingPanel.SetEffectiveSize(HostedPane, new Size(Width, Height));
            }
        }

        public FloatingWindow(DockingManager manager)
            : this()
        {
            //save manager ref
            _manager = manager;
        }

        DockingManager _manager = null;

        internal DockingManager Manager
        {
            get { return _manager; }
        }


        public FloatingDockablePane HostedPane
        {
            get { return (FloatingDockablePane)GetValue(ReferencedPaneProperty); }
            set { SetValue(ReferencedPaneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HostedPane.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReferencedPaneProperty =
            DependencyProperty.Register("HostedPane", typeof(FloatingDockablePane), typeof(FlyoutPaneWindow));


        internal virtual void OnEndDrag()
        {
        }

        internal virtual void OnShowSelectionBox()
        {

        }

        internal virtual void OnHideSelectionBox()
        {

        }

        #region Active Content Management
        ManagedContent lastActiveContent = null;

        protected override void OnActivated(EventArgs e)
        {
            if (Manager != null)
            {
                lastActiveContent = Manager.ActiveContent;
                Manager.ActiveContent = HostedPane.SelectedItem as ManagedContent;
            }

            base.OnActivated(e);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            if (Manager != null && lastActiveContent != null)
            {
                Manager.ActiveContent = lastActiveContent;
            }
            base.OnDeactivated(e);
        } 
        #endregion

        #region IsClosing Flag Management
        public void Close(bool force)
        {
            ForcedClosing = force;
            base.Close();
        }

        protected bool ForcedClosing { get; private set; }


        internal bool IsClosing { get; private set; }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (HostedPane.Items.Count > 0 && !ForcedClosing)
            {
                ManagedContent cntToClose = HostedPane.Items[0] as ManagedContent;
                if (!cntToClose.IsCloseable)
                {
                    e.Cancel = true;
                    base.OnClosing(e);
                    return;
                }
            }

            IsClosing = true;
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            IsClosing = false;
            base.OnClosed(e);
        }
        #endregion

        public abstract Pane ClonePane();


        #region Enable/Disable window Close Button
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetSystemMenu(
            IntPtr hWnd,
            Int32 bRevert
        );

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetMenuItemCount(
            IntPtr hMenu
        );

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int DrawMenuBar(
            IntPtr hWnd
        );

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnableMenuItem(
            IntPtr hMenu,
            Int32 uIDEnableItem,
            Int32 uEnable
        );

        private const Int32 MF_BYPOSITION = 0x400;
        private const Int32 MF_ENABLED = 0x0000;
        private const Int32 MF_GRAYED = 0x0001;
        private const Int32 MF_DISABLED = 0x0002;

        void EnableXButton()
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            IntPtr hMenu = GetSystemMenu(helper.Handle, 0);

            int menuItemCount = GetMenuItemCount(hMenu);

            EnableMenuItem(hMenu, menuItemCount - 1, MF_BYPOSITION | MF_ENABLED);
            DrawMenuBar(helper.Handle);
        }

        void DisableXButton()
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            IntPtr hMenu = GetSystemMenu(helper.Handle, 0);

            int menuItemCount = GetMenuItemCount(hMenu);

            EnableMenuItem(hMenu, menuItemCount - 1, MF_BYPOSITION | MF_DISABLED | MF_GRAYED);
            DrawMenuBar(helper.Handle);
        }

        #endregion

        #region Non-Client area management

        protected const int WM_MOVE = 0x0003;
        protected const int WM_SIZE = 0x0005;
        protected const int WM_NCMOUSEMOVE = 0xa0;
        protected const int WM_NCLBUTTONDOWN = 0xA1;
        protected const int WM_NCLBUTTONUP = 0xA2;
        protected const int WM_NCLBUTTONDBLCLK = 0xA3;
        protected const int WM_NCRBUTTONDOWN = 0xA4;
        protected const int WM_NCRBUTTONUP = 0xA5;
        protected const int HTCAPTION = 2;
        protected const int SC_MOVE = 0xF010;
        protected const int WM_SYSCOMMAND = 0x0112;



        #region Load/Unload window events
        HwndSource _hwndSource;
        HwndSourceHook _wndProcHandler;

        protected void OnLoaded(object sender, EventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            _hwndSource = HwndSource.FromHwnd(helper.Handle);
            _wndProcHandler = new HwndSourceHook(FilterMessage);
            _hwndSource.AddHook(_wndProcHandler);

            if (HostedPane.Items.Count > 0)
            {
                ManagedContent cntHosted = HostedPane.Items[0] as ManagedContent;
                if (!cntHosted.IsCloseable)
                {
                    DisableXButton();
                }
            }
        }
        protected void OnUnloaded(object sender, EventArgs e)
        {
            if (_hwndSource != null)
                _hwndSource.RemoveHook(_wndProcHandler);
        }
        #endregion


        protected virtual IntPtr FilterMessage(
            IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled
        )
        {
            handled = false;

            if (Manager == null)
                return IntPtr.Zero;

            switch (msg)
            {
                case WM_SIZE:
                case WM_MOVE:
                    //HostedPane.ReferencedPane.SaveFloatingWindowSizeAndPosition(this);
                    break;
                case WM_NCLBUTTONDOWN:
                    if (IsDockableWindow && wParam.ToInt32() == HTCAPTION)
                    {
                        short x = (short)((lParam.ToInt32() & 0xFFFF));
                        short y = (short)((lParam.ToInt32() >> 16));

                        Point clickPoint = this.TransformToDeviceDPI(new Point(x, y));
                        Manager.Drag(this, clickPoint, new Point(clickPoint.X - Left, clickPoint.Y - Top));

                        handled = true;
                    }
                    break;
                case WM_NCLBUTTONDBLCLK:
                    if (IsDockableWindow && wParam.ToInt32() == HTCAPTION)
                    {
                        if (IsDockableWindow)
                        {
                            Redock();
                            handled = true;
                        }
                    }
                    break;
                case WM_NCRBUTTONDOWN:
                    if (wParam.ToInt32() == HTCAPTION)
                    {
                        short x = (short)((lParam.ToInt32() & 0xFFFF));
                        short y = (short)((lParam.ToInt32() >> 16));

                        ContextMenu cxMenu = FindResource(new ComponentResourceKey(typeof(DockingManager), ContextMenuElement.FloatingWindow)) as ContextMenu;
                        if (cxMenu != null)
                        {
                            foreach (MenuItem menuItem in cxMenu.Items)
                                menuItem.CommandTarget = this;

                            cxMenu.Placement = PlacementMode.AbsolutePoint;
                            cxMenu.PlacementRectangle = new Rect(new Point(x, y), new Size(0, 0));
                            cxMenu.PlacementTarget = this;
                            cxMenu.IsOpen = true;
                        }

                        handled = true;
                    }
                    break;
                case WM_NCRBUTTONUP:
                    if (wParam.ToInt32() == HTCAPTION)
                    {

                        handled = true;
                    }
                    break;

            }


            return IntPtr.Zero;
        }
        #endregion

        #region Floating/dockable window state
        bool _dockableWindow = true;

        public bool IsDockableWindow
        {
            get { return _dockableWindow; }
            set
            {
                _dockableWindow = value;

                if (_dockableWindow)
                {
                    foreach (ManagedContent content in HostedPane.Items)
                        if (content is DockableContent)
                            ((DockableContent)content).SetStateToDockableWindow();
                }
                else
                {
                    foreach (ManagedContent content in HostedPane.Items)
                        if (content is DockableContent)
                            ((DockableContent)content).SetStateToFloatingWindow();
                }
            }
        }

        public bool IsFloatingWindow
        {
            get { return !IsDockableWindow; }
            set { IsDockableWindow = !value; }
        }

        protected virtual void Redock()
        { 
            
        }
        #endregion

        #region Commands
        private static object syncRoot = new object();

        private static RoutedUICommand tabbedDocumentCommand = null;
        public static RoutedUICommand TabbedDocumentCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == tabbedDocumentCommand)
                    {
                        tabbedDocumentCommand = new RoutedUICommand("T_abbed Document", "TabbedDocument", typeof(FloatingWindow));
                    }

                }
                return tabbedDocumentCommand;
            }
        }

        private static RoutedUICommand dockableCommand = null;
        public static RoutedUICommand SetAsDockableWindowCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == dockableCommand)
                    {
                        dockableCommand = new RoutedUICommand("D_ockable", "Dockable", typeof(FloatingWindow));
                    }

                }
                return dockableCommand;
            }
        }

        private static RoutedUICommand floatingCommand = null;
        public static RoutedUICommand SetAsFloatingWindowCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == floatingCommand)
                    {
                        floatingCommand = new RoutedUICommand("F_loating", "Floating", typeof(FloatingWindow));
                    }

                }
                return floatingCommand;
            }
        }

        private static RoutedUICommand closeCommand = null;
        public static RoutedUICommand CloseCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == closeCommand)
                    {
                        closeCommand = new RoutedUICommand("Close", "Close", typeof(FloatingWindow));
                    }

                }
                return closeCommand;
            }
        }

        protected virtual void OnExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == SetAsDockableWindowCommand)
            {
                IsDockableWindow = true;
                e.Handled = true;
            }
            else if (e.Command == SetAsFloatingWindowCommand)
            {
                IsFloatingWindow = true;
                e.Handled = true;
            }
        }

        protected virtual void OnCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == SetAsDockableWindowCommand)
                e.CanExecute = IsFloatingWindow;
            else if (e.Command == SetAsFloatingWindowCommand)
                e.CanExecute = IsDockableWindow;
            else
                e.CanExecute = true;
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
