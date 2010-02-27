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
    public class DockableFloatingWindow : FloatingWindow
    {
        static DockableFloatingWindow()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockableFloatingWindow), new FrameworkPropertyMetadata(typeof(DockableFloatingWindow)));
        }


        public DockableFloatingWindow(DockingManager manager)
            : base(manager)
        {
        }


        Pane _previousPane = null;

        int _arrayIndexPreviousPane = -1;


        public DockableFloatingWindow(DockingManager manager, DockableContent content)
            : this(manager)
        {

            //create a new temporary pane
            FloatingDockablePane pane = new FloatingDockablePane(this);

            //setup window size
            //Width = content.ContainerPane.ActualWidth;
            //Height = content.ContainerPane.ActualHeight;

            if (content.FloatingWindowSize.IsEmpty)
                content.FloatingWindowSize = new Size(content.ContainerPane.ActualWidth, content.ContainerPane.ActualHeight);
            
            Width = content.FloatingWindowSize.Width;
            Height = content.FloatingWindowSize.Height;

            //save current content position in container pane
            _previousPane = content.ContainerPane;
            _arrayIndexPreviousPane = _previousPane.Items.IndexOf(content);
            
            pane.Style = content.ContainerPane.Style;

            //remove content from container pane
            content.ContainerPane.RemoveContent(_arrayIndexPreviousPane);
            
            //add content to my temporary pane
            pane.Items.Add(content);

            //let templates access this pane
            HostedPane = pane;

            //Change state on contents
            IsDockableWindow = true;

            DocumentPane originalDocumentPane = _previousPane as DocumentPane;
            if (originalDocumentPane != null)
                originalDocumentPane.CheckContentsEmpty();
        }

        public DockableFloatingWindow(DockingManager manager, DockablePane dockablePane)
            : this(manager)
        {
            //create a new temporary pane
            FloatingDockablePane pane = new FloatingDockablePane(this);

            //setup window size
            ManagedContent selectedContent = dockablePane.SelectedItem as ManagedContent;

            if (selectedContent != null && selectedContent.FloatingWindowSize.IsEmpty)
                selectedContent.FloatingWindowSize = new Size(dockablePane.ActualWidth, dockablePane.ActualHeight);

            if (selectedContent != null)
            {
                Width = selectedContent.FloatingWindowSize.Width;
                Height = selectedContent.FloatingWindowSize.Height;
                this.ResizeMode = selectedContent.FloatingResizeMode;
            }
            else
            {
                Width = dockablePane.ActualWidth;
                Height = dockablePane.ActualHeight;
            }

            //transfer the style from the original dockablepane
            pane.Style = dockablePane.Style;

            //Width = dockablePane.ActualWidth;
            //Height = dockablePane.ActualHeight;

            ////save current content position in container pane
            //pane.SetValue(ResizingPanel.ResizeWidthProperty, dockablePane.GetValue(ResizingPanel.ResizeWidthProperty));
            //pane.SetValue(ResizingPanel.ResizeHeightProperty, dockablePane.GetValue(ResizingPanel.ResizeHeightProperty));

            int selectedIndex = dockablePane.SelectedIndex;

            //remove contents from container pane and insert in hosted pane
            while (dockablePane.Items.Count > 0)
            {
                ManagedContent content = dockablePane.RemoveContent(0);

                //add content to my temporary pane
                pane.Items.Add(content);
            }

            //let templates access this pane
            HostedPane = pane;
            HostedPane.SelectedIndex = selectedIndex;

            //Change state on contents
            IsDockableWindow = true;
        }


        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (HostedPane == null)
                HostedPane = Content as FloatingDockablePane;

            if (HostedPane != null)
            {
                Content = HostedPane;
            }
        }

        

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            DockableContent[] cntsToClose = new DockableContent[HostedPane.Items.Count];
            HostedPane.Items.CopyTo(cntsToClose, 0);

            foreach (DockableContent cntToClose in cntsToClose)
            {
                HostedPane.CloseOrHide(HostedPane.Items[0] as DockableContent, ForcedClosing);
            }

            Manager.UnregisterFloatingWindow(this);
        }

        public override Pane ClonePane()
        {
            DockablePane paneToAnchor = new DockablePane();

            ResizingPanel.SetEffectiveSize(paneToAnchor, new Size(Width, Height));

            if (HostedPane.Style != null)
                paneToAnchor.Style = HostedPane.Style;

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

        protected override void OnExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == TabbedDocumentCommand)
            {
                DockableContent currentContent = HostedPane.SelectedItem as DockableContent;

                Manager.MainDocumentPane.Items.Insert(0, HostedPane.RemoveContent(HostedPane.SelectedIndex));
                Manager.MainDocumentPane.SelectedIndex = 0;

                currentContent.SetStateToDocument();

                if (HostedPane.Items.Count == 0)
                    this.Close();
                e.Handled = true;
            }
            else if (e.Command == CloseCommand)
            {
                //DockableContent currentContent = HostedPane.SelectedItem as DockableContent;
                //Manager.Hide(currentContent);
                HostedPane.CloseOrHide();
                if (HostedPane.Items.Count == 0)
                    this.Close();
                e.Handled = true;
            }

            base.OnExecuteCommand(sender, e);
        }

        protected override void Redock()
        {
            if (_previousPane != null)
            {
                if (_previousPane.GetManager() == null)
                {
                    DockablePane newContainerPane = new DockablePane();
                    newContainerPane.Items.Add(HostedPane.RemoveContent(0));
                    newContainerPane.SetValue(ResizingPanel.ResizeWidthProperty, _previousPane.GetValue(ResizingPanel.ResizeWidthProperty));
                    newContainerPane.SetValue(ResizingPanel.ResizeHeightProperty, _previousPane.GetValue(ResizingPanel.ResizeHeightProperty));

                    if (_previousPane.Style != null)
                        newContainerPane.Style = _previousPane.Style;
                    
                    Manager.Anchor(newContainerPane, ((DockablePane)_previousPane).Anchor);
                }
                else
                {
                    if (_arrayIndexPreviousPane > _previousPane.Items.Count)
                        _arrayIndexPreviousPane = _previousPane.Items.Count;

                    DockableContent currentContent = HostedPane.Items[0] as DockableContent;
                    _previousPane.Items.Insert(_arrayIndexPreviousPane, HostedPane.RemoveContent(0));
                    _previousPane.SelectedIndex = _arrayIndexPreviousPane;
                    currentContent.SetStateToDock();

                }
                this.Close();
            } 
            
            base.Redock();
        }

        
    }
}
