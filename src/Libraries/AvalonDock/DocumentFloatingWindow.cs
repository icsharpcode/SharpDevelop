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

namespace AvalonDock
{
    public class DocumentFloatingWindow : FloatingWindow  
    {
        static DocumentFloatingWindow()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentFloatingWindow), new FrameworkPropertyMetadata(typeof(DocumentFloatingWindow)));

            Window.AllowsTransparencyProperty.OverrideMetadata(typeof(DocumentFloatingWindow), new FrameworkPropertyMetadata(true));
            Window.WindowStyleProperty.OverrideMetadata(typeof(DocumentFloatingWindow), new FrameworkPropertyMetadata(WindowStyle.None));
            Window.ShowInTaskbarProperty.OverrideMetadata(typeof(DocumentFloatingWindow), new FrameworkPropertyMetadata(false));
        }


        public DocumentFloatingWindow(DockingManager manager)
            :base(manager)
        {
        }        

        Pane _previousPane = null;

        int _arrayIndexPreviousPane = -1;

        public DocumentFloatingWindow(DockingManager manager, DocumentContent content)
            : this(manager)
        {
            //create a new temporary pane
            FloatingDockablePane pane = new FloatingDockablePane(this);

            //setup window size
            Width = content.ContainerPane.ActualWidth;
            Height = content.ContainerPane.ActualHeight;

            //save current content position in container pane
            _previousPane = content.ContainerPane;
            _arrayIndexPreviousPane = _previousPane.Items.IndexOf(content);
            pane.SetValue(ResizingPanel.ResizeWidthProperty, _previousPane.GetValue(ResizingPanel.ResizeWidthProperty));
            pane.SetValue(ResizingPanel.ResizeHeightProperty, _previousPane.GetValue(ResizingPanel.ResizeHeightProperty));

            //remove content from container pane
            content.ContainerPane.RemoveContent(_arrayIndexPreviousPane);

            //add content to my temporary pane
            pane.Items.Add(content);

            //let templates access this pane
            HostedPane = pane;

            if (IsDocumentFloatingAllowed)
            {
                AllowsTransparency = false;
                WindowStyle = WindowStyle.ToolWindow;
                NotifyPropertyChanged("IsDocumentFloatingAllowed");
            }
        }

        public bool IsDocumentFloatingAllowed
        {
            get
            { 
                if (HostedPane != null && 
                    HostedPane.Items.Count > 0)
                    return ((DocumentContent)HostedPane.Items[0]).IsFloatingAllowed;

                return false;
            }
        }


        internal override void OnEndDrag()
        {
            if (HostedPane.Items.Count > 0)
            {
                DocumentContent content = HostedPane.Items[0] as DocumentContent;
                if (!content.IsFloatingAllowed)
                {
                    HostedPane.Items.RemoveAt(0);
                    _previousPane.Items.Insert(_arrayIndexPreviousPane, content);
                    _previousPane.SelectedItem = content;
                    Close();
                }
                else
                {
                    DocumentPane originalDocumentPane = _previousPane as DocumentPane;
                    originalDocumentPane.CheckContentsEmpty();
                }
            }
            else
            {
                DocumentPane originalDocumentPane = _previousPane as DocumentPane;
                originalDocumentPane.CheckContentsEmpty();
                Close();
            }


            base.OnEndDrag();
        }


        public override Pane ClonePane()
        {
            DocumentPane paneToAnchor = new DocumentPane();

            ////transfer the resizing panel sizes
            //paneToAnchor.SetValue(ResizingPanel.ResizeWidthProperty,
            //    HostedPane.GetValue(ResizingPanel.ResizeWidthProperty));
            //paneToAnchor.SetValue(ResizingPanel.ResizeHeightProperty,
            //    HostedPane.GetValue(ResizingPanel.ResizeHeightProperty));

            ResizingPanel.SetEffectiveSize(paneToAnchor, new Size(Width, Height));

            //transfer contents from hosted pane in the floating window and
            //the new created dockable pane
            while (HostedPane.Items.Count > 0)
            {
                paneToAnchor.Items.Add(
                    HostedPane.RemoveContent(0));
            }

            return paneToAnchor;
        }


        protected override void OnInitialized(EventArgs e)
        {

            if (IsDocumentFloatingAllowed)
            {
                if (HostedPane != null)
                {
                    Content = HostedPane;
                }
            }

            base.OnInitialized(e);
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
        #region Commands

        protected override void OnExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == TabbedDocumentCommand)
            {
                DocumentContent currentContent =  HostedPane.SelectedItem as DocumentContent;

                _previousPane.Items.Insert(0, HostedPane.RemoveContent(HostedPane.SelectedIndex));
                _previousPane.SelectedIndex = 0;

                if (HostedPane.Items.Count == 0)
                    this.Close();
                e.Handled = true;
            }
            else if (e.Command == CloseCommand)
            {
                DocumentContent docContent = this.HostedPane.Items[0] as DocumentContent;
                e.Handled = docContent.Close();
            }

            base.OnExecuteCommand(sender, e);
        }

        
        #endregion
        //#region Drag
        //protected override IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{
        //    handled = false;

        //    if (!IsDocumentFloatingAllowed)
        //        return IntPtr.Zero;

        //    switch (msg)
        //    {
        //        case WM_SIZE:
        //        case WM_MOVE:
        //            //HostedPane.ReferencedPane.SaveFloatingWindowSizeAndPosition(this);
        //            break;
        //        case WM_NCLBUTTONDOWN:
        //            if (IsDockableWindow && wParam.ToInt32() == HTCAPTION)
        //            {
        //                short x = (short)((lParam.ToInt32() & 0xFFFF));
        //                short y = (short)((lParam.ToInt32() >> 16));

        //                Point clickPoint = this.TransformToDeviceDPI(new Point(x, y));
        //                Manager.Drag(this, clickPoint, new Point(clickPoint.X - Left, clickPoint.Y - Top));

        //                handled = true;
        //            }
        //            break;
        //        case WM_NCLBUTTONDBLCLK:
        //            if (IsDockableWindow && wParam.ToInt32() == HTCAPTION)
        //            {
        //                if (IsDockableWindow)
        //                {
        //                    if (_previousPane != null)
        //                    {
        //                        if (_previousPane.GetManager() == null)
        //                        {
        //                            DockablePane newContainerPane = new DockablePane();
        //                            newContainerPane.Items.Add(HostedPane.RemoveContent(0));
        //                            newContainerPane.SetValue(ResizingPanel.ResizeWidthProperty, _previousPane.GetValue(ResizingPanel.ResizeWidthProperty));
        //                            newContainerPane.SetValue(ResizingPanel.ResizeHeightProperty, _previousPane.GetValue(ResizingPanel.ResizeHeightProperty));
        //                            Manager.Anchor(newContainerPane, ((DockablePane)_previousPane).Anchor);
        //                        }
        //                        else
        //                        {
        //                            if (_arrayIndexPreviousPane > _previousPane.Items.Count)
        //                                _arrayIndexPreviousPane = _previousPane.Items.Count;

        //                            DockableContent currentContent = HostedPane.Items[0] as DockableContent;
        //                            _previousPane.Items.Insert(_arrayIndexPreviousPane, HostedPane.RemoveContent(0));
        //                            _previousPane.SelectedIndex = _arrayIndexPreviousPane;
        //                            currentContent.SetStateToDock();

        //                        }
        //                        this.Close();
        //                    }

        //                    handled = true;
        //                }
        //            }
        //            break;
        //        case WM_NCRBUTTONDOWN:
        //            if (wParam.ToInt32() == HTCAPTION)
        //            {
        //                short x = (short)((lParam.ToInt32() & 0xFFFF));
        //                short y = (short)((lParam.ToInt32() >> 16));

        //                ContextMenu cxMenu = FindResource(new ComponentResourceKey(typeof(DockingManager), ContextMenuElement.FloatingWindow)) as ContextMenu;
        //                if (cxMenu != null)
        //                {
        //                    foreach (MenuItem menuItem in cxMenu.Items)
        //                        menuItem.CommandTarget = this;

        //                    cxMenu.Placement = PlacementMode.AbsolutePoint;
        //                    cxMenu.PlacementRectangle = new Rect(new Point(x, y), new Size(0, 0));
        //                    cxMenu.PlacementTarget = this;
        //                    cxMenu.IsOpen = true;
        //                }

        //                handled = true;
        //            }
        //            break;
        //        case WM_NCRBUTTONUP:
        //            if (wParam.ToInt32() == HTCAPTION)
        //            {

        //                handled = true;
        //            }
        //            break;

        //    }


        //    return IntPtr.Zero;
        //}
        //#endregion

        protected override void Redock()
        {
            if (_previousPane != null)
            {
                if (_previousPane.GetManager() == null)
                {
                     Manager.MainDocumentPane.Items.Insert(0, HostedPane.RemoveContent(0));
                }
                else
                {
                    if (_arrayIndexPreviousPane > _previousPane.Items.Count)
                        _arrayIndexPreviousPane = _previousPane.Items.Count;

                    _previousPane.Items.Insert(_arrayIndexPreviousPane, HostedPane.RemoveContent(0));
                    _previousPane.SelectedIndex = _arrayIndexPreviousPane;
                }
                this.Close();
            }

            base.Redock();
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (e.Cancel)
                return;
            
            if (this.HostedPane.Items.Count > 0)
            {
                DocumentContent docContent = this.HostedPane.Items[0] as DocumentContent;
                if (!docContent.Close())
                    e.Cancel = true;
                else
                    this.HostedPane.Items.Remove(docContent);
            }
            
        }
    }
}
