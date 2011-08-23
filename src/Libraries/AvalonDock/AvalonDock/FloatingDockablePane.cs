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
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace AvalonDock
{
    public class FloatingDockablePane : DockablePane
    {
        static FloatingDockablePane()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatingDockablePane), new FrameworkPropertyMetadata(typeof(FloatingDockablePane)));
            //by design avoid style change

            Pane.ShowHeaderProperty.OverrideMetadata(typeof(FloatingDockablePane), new FrameworkPropertyMetadata(false));
        }



        internal FloatingDockablePane(DockableFloatingWindow floatingWindow, DockablePane paneToTransfer)
        {
            _floatingWindow = floatingWindow;
            _paneToTransfer = paneToTransfer;
        }

        internal FloatingDockablePane(DockableFloatingWindow floatingWindow, DockableContent contentToTransfer)
        {
            _floatingWindow = floatingWindow;
            _contentToTransfer = contentToTransfer;
        }

        protected override void OnInitialized(EventArgs e)
        {
            if (_paneToTransfer != null)
            {
                //setup window size
                ManagedContent selectedContent = _paneToTransfer.SelectedItem as ManagedContent;
                if (selectedContent is DockableContent)
                {
                    _floatingWindow.SizeToContent = (selectedContent as DockableContent).FloatingWindowSizeToContent;
                }

                if (selectedContent != null && selectedContent.FloatingWindowSize.IsEmpty)
                    selectedContent.FloatingWindowSize = new Size(_paneToTransfer.ActualWidth, _paneToTransfer.ActualHeight);

                if (selectedContent != null)
                {
                    _floatingWindow.Width = selectedContent.FloatingWindowSize.Width;
                    _floatingWindow.Height = selectedContent.FloatingWindowSize.Height;
                }
                else
                {
                    _floatingWindow.Width = _paneToTransfer.ActualWidth;
                    _floatingWindow.Height = _paneToTransfer.ActualHeight;
                }

                int selectedIndex = _paneToTransfer.SelectedIndex;

                //remove contents from container pane and insert in hosted pane
                while (_paneToTransfer.Items.Count > 0)
                {
                    DockableContent contentToTranser = _paneToTransfer.Items[0] as DockableContent;

                    contentToTranser.SaveCurrentStateAndPosition();

                    _paneToTransfer.RemoveContent(0);

                    //add content to my temporary pane
                    Items.Add(contentToTranser);

                    contentToTranser.SetStateToDockableWindow();
                }

                SelectedIndex = selectedIndex;

                //transfer the style from the original dockablepane
                //Style = _paneToTransfer.Style;
                AttachStyleFromPane(_paneToTransfer);
                
                ApplyTemplate();

                LayoutTransform = (MatrixTransform)_paneToTransfer.TansformToAncestor();
            }
            else if (_contentToTransfer != null)
            {
                //setup window size
                if (_contentToTransfer.FloatingWindowSize.IsEmpty)
                    _contentToTransfer.FloatingWindowSize = new Size(_contentToTransfer.ContainerPane.ActualWidth, _contentToTransfer.ContainerPane.ActualHeight);

                _floatingWindow.Width = _contentToTransfer.FloatingWindowSize.Width;
                _floatingWindow.Height = _contentToTransfer.FloatingWindowSize.Height;

                //save current content position in container pane
                _previousPane = _contentToTransfer.ContainerPane;

                _arrayIndexPreviousPane = _previousPane.Items.IndexOf(_contentToTransfer);

                _contentToTransfer.SaveCurrentStateAndPosition();

                //remove content from container pane
                _contentToTransfer.ContainerPane.RemoveContent(_arrayIndexPreviousPane);

                //add content to this pane
                Items.Add(_contentToTransfer);

                SelectedIndex = 0;

                
                AttachStyleFromPane(_previousPane as DockablePane);

                DocumentPane originalDocumentPane = _previousPane as DocumentPane;
                if (originalDocumentPane != null)
                    originalDocumentPane.CheckContentsEmpty();


                _contentToTransfer.SetStateToDockableWindow();
                LayoutTransform = (MatrixTransform)_contentToTransfer.TansformToAncestor();
            }

            base.OnInitialized(e);
        }

        void AttachStyleFromPane(DockablePane copyFromPane)
        {
            if (copyFromPane == null)
                return;

            //Binding bnd = new Binding("Style");
            //bnd.Source = copyFromPane;
            //bnd.Mode = BindingMode.OneWay;

            //SetBinding(StyleProperty, bnd);
        }

        DockablePane _paneToTransfer = null;
        DockableContent _contentToTransfer = null;
        
        Pane _previousPane = null;
        int _arrayIndexPreviousPane = -1;

        DockableFloatingWindow _floatingWindow = null;

        public DockableFloatingWindow FloatingWindow
        {
            get { return _floatingWindow; }
        }

        public override DockingManager GetManager()
        {
            return _floatingWindow.Manager;
        }

        protected override void CheckItems(System.Collections.IList newItems)
        {
            if (Items.Count == 0 && FloatingWindow != null)
                FloatingWindow.Close(true);
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                e.NewItems.Cast<DockableContent>().ForEach(c =>
                {
                    if (c.State == DockableContentState.None)
                    {
                        if (FloatingWindow.IsDockableWindow)
                            c.SetStateToDockableWindow();
                        else
                            c.SetStateToFloatingWindow();
                    }
                });
            }

            base.OnItemsChanged(e);
        }

        public override void Dock()
        {
            DockableContent[] contentsToRedock = Items.Cast<DockableContent>().ToArray();

            foreach (var cntToRedock in contentsToRedock)
                cntToRedock.Show();

            base.Dock();
        }
    }
}
