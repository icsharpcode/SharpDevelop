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
    public class DocumentFloatingWindow : FloatingWindow  
    {
        static DocumentFloatingWindow()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentFloatingWindow), new FrameworkPropertyMetadata(typeof(DocumentFloatingWindow)));

            ContentProperty.OverrideMetadata(typeof(DocumentFloatingWindow),
                new FrameworkPropertyMetadata(
                    new PropertyChangedCallback(OnContentPropertyChanged),
                    new CoerceValueCallback(OnCoerceValueContentProperty)));
        }

        //DocumentPane _previousPane = null;

        //int _arrayIndexPreviousPane = -1;

        internal DocumentFloatingWindow(DockingManager manager)
            :base(manager)
        {
        }
        
        //public DocumentFloatingWindow(DockingManager manager, DocumentContent content)
        //    : this(manager)
        //{
        //    //create a new temporary pane
        //    FloatingDocumentPane pane = new FloatingDocumentPane(this);

        //    //setup window size
        //    Width = content.ContainerPane.ActualWidth;
        //    Height = content.ContainerPane.ActualHeight;

        //    //save current content position in container pane
        //    _previousPane = content.ContainerPane as DocumentPane;
        //    _arrayIndexPreviousPane = _previousPane.Items.IndexOf(content);
        //    pane.SetValue(ResizingPanel.ResizeWidthProperty, _previousPane.GetValue(ResizingPanel.ResizeWidthProperty));
        //    pane.SetValue(ResizingPanel.ResizeHeightProperty, _previousPane.GetValue(ResizingPanel.ResizeHeightProperty));

        //    pane.Style = _previousPane.Style;

        //    //remove content from container pane
        //    _previousPane.RemoveContent(_arrayIndexPreviousPane);

        //    //add content to my temporary pane
        //    pane.Items.Add(content);

        //    //let templates access this pane
        //    SetHostedPane(pane);

        //    //if (IsDocumentFloatingAllowed)
        //    //{
        //    //    AllowsTransparency = false;
        //    //    WindowStyle = WindowStyle.ToolWindow;
        //    //}
        //}

        //public bool IsDocumentFloatingAllowed
        //{
        //    get
        //    { 
        //        if (HostedPane != null && 
        //            HostedPane.Items.Count > 0)
        //            return ((DocumentContent)HostedPane.Items[0]).IsFloatingAllowed;

        //        return false;
        //    }
        //}


        internal override void OnEndDrag()
        {
            //if (HostedPane.Items.Count > 0)
            //{
            //    DocumentContent content = HostedPane.Items[0] as DocumentContent;
            //    if (!content.IsFloatingAllowed)
            //    {
            //        HostedPane.Items.RemoveAt(0);
            //        _previousPane.Items.Insert(_arrayIndexPreviousPane, content);
            //        _previousPane.SelectedItem = content;
            //        Close();
            //    }
            //    else
            //    {
            //        DocumentPane originalDocumentPane = _previousPane as DocumentPane;
            //        originalDocumentPane.CheckContentsEmpty();
            //    }
            //}
            //else
            //{
            //    DocumentPane originalDocumentPane = _previousPane as DocumentPane;
            //    originalDocumentPane.CheckContentsEmpty();
            //    Close();
            //}

            if (((FloatingDocumentPane)HostedPane).PreviousPane != null)
                ((FloatingDocumentPane)HostedPane).PreviousPane.CheckContentsEmpty();

            if (HostedPane.Items.Count == 0)
                Close();

            base.OnEndDrag();
        }

        public override Pane ClonePane()
        {
            DocumentPane paneToAnchor = new DocumentPane();

            ResizingPanel.SetEffectiveSize(paneToAnchor, new Size(Width, Height));

            //transfer contents from hosted pane in the floating window and
            //the new created dockable pane
            while (HostedPane.Items.Count > 0)
            {
                paneToAnchor.Items.Add(
                    HostedPane.RemoveContent(0));
            }
            paneToAnchor.ApplyTemplate();

            return paneToAnchor;
        }

        internal override void OnShowSelectionBox()
        {
            this.Visibility = Visibility.Hidden;
            base.OnShowSelectionBox();
        }

        internal override void OnHideSelectionBox()
        {
            this.Visibility = Visibility.Visible;
            base.OnHideSelectionBox();
        }

        public override void Dock()
        {
            //if (_previousPane != null)
            //{
            //    if (_previousPane.GetManager() == null)
            //    {
            //         Manager.MainDocumentPane.Items.Insert(0, HostedPane.RemoveContent(0));
            //    }
            //    else
            //    {
            //        if (_arrayIndexPreviousPane > _previousPane.Items.Count)
            //            _arrayIndexPreviousPane = _previousPane.Items.Count;

            //        _previousPane.Items.Insert(_arrayIndexPreviousPane, HostedPane.RemoveContent(0));
            //        _previousPane.SelectedIndex = _arrayIndexPreviousPane;
            //    }
            //    this.Close();
            //}

            ((DocumentContent)HostedPane.Items[0]).Show();

            base.Dock();
        }

        protected override void FilterMessage(object sender, FilterMessageEventArgs e)
        {
            e.Handled = false;

            if (Manager == null)
                return;

            switch (e.Msg)
            {
                case WM_NCLBUTTONDOWN: //Left button down on title -> start dragging over docking manager
                    if (e.WParam.ToInt32() == HTCAPTION)
                    {
                        short x = (short)((e.LParam.ToInt32() & 0xFFFF));
                        short y = (short)((e.LParam.ToInt32() >> 16));

                        Point clickPoint = this.TransformToDeviceDPI(new Point(x, y));
                        Manager.Drag(this, clickPoint, new Point(clickPoint.X - Left, clickPoint.Y - Top));

                        e.Handled = true;
                    }
                    break;
                case WM_NCLBUTTONDBLCLK: //Left Button Double Click -> Maximixe/Normal
                    if (e.WParam.ToInt32() == HTCAPTION)
                    {
                        WindowState = WindowState == System.Windows.WindowState.Maximized ?
                            System.Windows.WindowState.Normal : System.Windows.WindowState.Maximized;
                        e.Handled = true;
                    }
                    break;
            }



            base.FilterMessage(sender, e);
        }

        protected override bool  OpenContextMenu(UIElement popupButton, Point ptMouse)
        {
            ContextMenu cxMenu = FindResource(new ComponentResourceKey(typeof(DockingManager),
                           ContextMenuElement.DocumentFloatingWindow)) as ContextMenu;
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

        static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        static object OnCoerceValueContentProperty(DependencyObject d, object baseValue)
        {
            DocumentFloatingWindow fl = ((DocumentFloatingWindow)d);

            if (fl.Content != null)
            {
                throw new InvalidOperationException("Content on floating windows can't be set more than one time.");
            }

            if (!(baseValue is DocumentContent))
            {
                throw new InvalidOperationException("Content must be of type DocumentContent");
            }

            FloatingDocumentPane paneToReturn = null;

            if (baseValue is DocumentContent)
                paneToReturn = new FloatingDocumentPane(fl, baseValue as DocumentContent);

            return paneToReturn;
        }
    }
}
