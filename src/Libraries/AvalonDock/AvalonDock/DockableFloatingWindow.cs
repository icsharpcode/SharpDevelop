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
using System.Windows.Interop;
using System.Linq;

namespace AvalonDock
{
    public class DockableFloatingWindow : FloatingWindow
    {
        static DockableFloatingWindow()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockableFloatingWindow), new FrameworkPropertyMetadata(typeof(DockableFloatingWindow)));

            ContentControl.ContentProperty.OverrideMetadata(typeof(DockableFloatingWindow), 
                new FrameworkPropertyMetadata(
                    new PropertyChangedCallback(OnContentPropertyChanged), 
                    new CoerceValueCallback(OnCoerceValueContentProperty)));
        }
        

        internal DockableFloatingWindow(DockingManager manager)
            : base(manager)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");
        }


        //protected override void OnClosed(EventArgs e)
        //{
        //    base.OnClosed(e);

        //    DockableContent[] cntsToClose = new DockableContent[HostedPane.Items.Count];
        //    HostedPane.Items.CopyTo(cntsToClose, 0);

        //    foreach (DockableContent cntToClose in cntsToClose)
        //    {
        //        //HostedPane.CloseOrHide(HostedPane.Items[0] as DockableContent, ForcedClosing);
        //        cntToClose.CloseOrHide(ForcedClosing);
        //    }

        //    Manager.UnregisterFloatingWindow(this);
        //}

        public override Pane ClonePane()
        {
            DockablePane paneToAnchor = new DockablePane();

            ResizingPanel.SetEffectiveSize(paneToAnchor, new Size(Width, Height));

            //if (HostedPane.Style != null)
            //    paneToAnchor.Style = HostedPane.Style;

            int selectedIndex = HostedPane.SelectedIndex;

            //transfer contents from hosted pane in the floating window and
            //the new created dockable pane
            while (HostedPane.Items.Count > 0)
            {
                paneToAnchor.Items.Add(
                    HostedPane.RemoveContent(0));
            }

            paneToAnchor.SelectedIndex = selectedIndex;

            return paneToAnchor;
        }

        #region IsDockableWindow

        /// <summary>
        /// IsDockableWindow Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsDockableWindowProperty =
            DependencyProperty.Register("IsDockableWindow", typeof(bool), typeof(DockableFloatingWindow),
                new FrameworkPropertyMetadata(true,
                    new PropertyChangedCallback(OnIsDockableWindowChanged)));

        /// <summary>
        /// Gets or sets the IsDockableWindow property.  This dependency property 
        /// indicates that <see cref="FloatingWindow"/> can be docked to <see cref="DockingManager"/>.
        /// </summary>
        public bool IsDockableWindow
        {
            get { return (bool)GetValue(IsDockableWindowProperty); }
            set { SetValue(IsDockableWindowProperty, value); }
        }

        /// <summary>
        /// Handles changes to the IsDockableWindow property.
        /// </summary>
        private static void OnIsDockableWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockableFloatingWindow)d).OnIsDockableWindowChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsDockableWindow property.
        /// </summary>
        protected virtual void OnIsDockableWindowChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                foreach (DockableContent content in HostedPane.Items)
                    content.SetStateToDockableWindow();
            }
            else
            {
                foreach (DockableContent content in HostedPane.Items)
                    content.SetStateToFloatingWindow();
            }
        }

        #endregion

        #region Commands

        protected virtual void OnExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == DockableFloatingWindowCommands.SetAsDockableWindow)
            {
                IsDockableWindow = true;
                e.Handled = true;
            }
            else if (e.Command == DockableFloatingWindowCommands.SetAsFloatingWindow)
            {
                IsDockableWindow = false;
                e.Handled = true;
            }
        }

        protected virtual void OnCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == DockableFloatingWindowCommands.SetAsDockableWindow)
                e.CanExecute = !IsDockableWindow;
            else if (e.Command == DockableFloatingWindowCommands.SetAsFloatingWindow)
                e.CanExecute = IsDockableWindow;
        }
        #endregion

        protected override void OnInitialized(EventArgs e)
        {
            this.CommandBindings.Add(new CommandBinding(DockableFloatingWindowCommands.SetAsDockableWindow, OnExecuteCommand, OnCanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(DockableFloatingWindowCommands.SetAsFloatingWindow, OnExecuteCommand, OnCanExecuteCommand));

            base.OnInitialized(e);
        }

        protected override bool OpenContextMenu(UIElement popupButton, Point ptMouse)
        {
            ContextMenu cxMenu = FindResource(new ComponentResourceKey(typeof(DockingManager),
                           ContextMenuElement.DockableFloatingWindow)) as ContextMenu;
            if (cxMenu != null)
            {
                foreach (var menuItem in cxMenu.Items.OfType<MenuItem>())
                    menuItem.CommandTarget = HostedPane.SelectedItem as IInputElement;

                if (popupButton != null)
                {
                    cxMenu.Placement = PlacementMode.Bottom;
                    cxMenu.PlacementTarget = popupButton;
                }
                else
                { 
                    cxMenu.Placement = PlacementMode.Bottom;
                    cxMenu.PlacementRectangle = new Rect(ptMouse, new Size(0, 0));           
                }
                cxMenu.IsOpen = true;

                return true;
            }

            return base.OpenContextMenu(popupButton, ptMouse);
        }

        protected override void FilterMessage(object sender, FilterMessageEventArgs e)
        {
            e.Handled = false;

            if (Manager == null)
                return;

            switch (e.Msg)
            {
                case WM_NCLBUTTONDOWN: //Left button down on title -> start dragging over docking manager
                    if (IsDockableWindow && e.WParam.ToInt32() == HTCAPTION)
                    {
                        short x = (short)((e.LParam.ToInt32() & 0xFFFF));
                        short y = (short)((e.LParam.ToInt32() >> 16));

                        Point clickPoint = this.TransformToDeviceDPI(new Point(x, y));
                        Manager.Drag(this, clickPoint, new Point(clickPoint.X - Left, clickPoint.Y - Top));

                        e.Handled = true;
                    }
                    break;
                case WM_NCLBUTTONDBLCLK: //Left Button Double Click -> dock to docking manager
                    if (IsDockableWindow && e.WParam.ToInt32() == HTCAPTION)
                    {
                        if (IsDockableWindow)
                        {
                            Dock();
                            e.Handled = true;
                        }
                    }
                    break;
             }

            
            
            base.FilterMessage(sender, e);
        }

        static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        
        }

        static object OnCoerceValueContentProperty(DependencyObject d, object baseValue)
        {
            DockableFloatingWindow fl = ((DockableFloatingWindow)d);

            if (fl.Content != null)
            {
                throw new InvalidOperationException("Content on floating windows can't be set more than one time.");
            }

            if (!(baseValue is DockableContent) &&
                !(baseValue is DockablePane))
            {
                throw new InvalidOperationException("Content must be of type DockableContent or DockablePane");
            }

            FloatingDockablePane paneToReturn = null;

            if (baseValue is DockableContent)
                paneToReturn = new FloatingDockablePane(fl, baseValue as DockableContent);
            else if (baseValue is DockablePane)
                paneToReturn = new FloatingDockablePane(fl, baseValue as DockablePane);

            return paneToReturn;
        }


        public override void Dock()
        {
            var dockablePane = HostedPane as FloatingDockablePane;
            
            Debug.Assert(dockablePane != null);

            if (dockablePane != null)
                dockablePane.Dock();
            
            base.Dock();
        }
    }
}
