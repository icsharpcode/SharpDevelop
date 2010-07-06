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
using System.Windows.Input;

namespace AvalonDock
{
    internal class FlyoutDockablePane : DockablePane
    {
        static FlyoutDockablePane()
        {
            DockablePane.ShowTabsProperty.AddOwner(typeof(FlyoutDockablePane), new FrameworkPropertyMetadata(false));
        }

        int _arrayIndexPreviousPane = -1;


        public FlyoutDockablePane()
        { }

        public FlyoutDockablePane(DockableContent content)
        {
            _referencedPane = content.ContainerPane as DockablePane;
            _manager = _referencedPane.GetManager();

            //save current content position in container pane
            _arrayIndexPreviousPane = _referencedPane.Items.IndexOf(content);
            Anchor = _referencedPane.Anchor;

            //SetValue(ResizingPanel.ResizeWidthProperty, new GridLength(ResizingPanel.GetEffectiveSize(_referencedPane).Width));
            //SetValue(ResizingPanel.ResizeHeightProperty, new GridLength(ResizingPanel.GetEffectiveSize(_referencedPane).Height));

            this.Style = _referencedPane.Style;

            //remove content from container pane
            //and add content to my temporary pane
            _referencedPane.Items.RemoveAt(_arrayIndexPreviousPane);
            this.Items.Add(content);


            //select the single content in this pane
            SelectedItem = this.Items[0];
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        internal void RestoreOriginalPane()
        {
            if (this.Items.Count == 1)
            {
                _referencedPane.Items.Insert(_arrayIndexPreviousPane, RemoveContent(0));
                //ResizingPanel.SetResizeWidth(_referencedPane, ResizingPanel.GetResizeWidth(this));
                //ResizingPanel.SetResizeHeight(_referencedPane, ResizingPanel.GetResizeHeight(this));
            }            
        }


        DockablePane _referencedPane = null;

        internal DockablePane ReferencedPane
        {
            get { return _referencedPane; }
        }

        DockingManager _manager = null;

        public override DockingManager GetManager()
        {
            return _manager;
        }

        public override void ToggleAutoHide()
        {
            GetManager().ToggleAutoHide(_referencedPane);
        }
   }
}
