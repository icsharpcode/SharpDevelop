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
using System.Diagnostics;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AvalonDock
{
    public abstract class Pane : 
        System.Windows.Controls.Primitives.Selector, 
        IDropSurface,
        IDockableControl,
        INotifyPropertyChanged
    {

        public Pane()
        {
            this.Loaded += new RoutedEventHandler(Pane_Loaded);
            this.Unloaded += new RoutedEventHandler(Pane_Unloaded);
        }

        void Pane_Loaded(object sender, RoutedEventArgs e)
        {
            //if (GetManager() == null && Parent != null)
            //    throw new InvalidOperationException("Pane must be put under a DockingManager!");

            AddDragPaneReferences();
        }

        void Pane_Unloaded(object sender, RoutedEventArgs e)
        {
            RemoveDragPaneReferences();
        }

        

        #region Contents management
        public bool HasSingleItem
        {
            get
            {
                return (bool)GetValue(HasSingleItemProperty);
            }
            protected set { SetValue(HasSingleItemPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for HasSingleItem.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey HasSingleItemPropertyKey =
            DependencyProperty.RegisterReadOnly("HasSingleItem", typeof(bool), typeof(Pane), new PropertyMetadata(false));

        public static readonly DependencyProperty HasSingleItemProperty = HasSingleItemPropertyKey.DependencyProperty;


        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            HasSingleItem = (Items.Count == 1);

            if (Items.Count > 0)
            {
                int currentIndex = SelectedIndex;
                SelectedIndex = -1;

                if (currentIndex < 0 ||
                    currentIndex >= Items.Count)
                    currentIndex = Items.Count - 1;

                SelectedIndex = currentIndex;
            }
            //else
            //    RemoveDragPaneReferences();

            base.OnItemsChanged(e);
        }

        //void RefreshContentsSelectedProperty()
        //{
        //    //foreach (ManagedContent mc in Items)
        //    //{
        //    //    //mc.IsSelected = (mc == SelectedItem);
        //    //    //Selector.SetIsSelected(mc 
                
        //    //    if (Selector.GetIsSelected(mc))
        //    //        mc.FocusContent();
        //    //}
        //}

        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (e.Property == SelectedItemProperty)
        //        RefreshContentsSelectedProperty();
        //    //    SetValue(ActiveContentProperty, SelectedItem);

        //    //if (e.Property == ActiveContentProperty)
        //    //{
        //    //    //SetValue(SelectedItemProperty, ActiveContent);

        //    //}

        //    base.OnPropertyChanged(e);
        //} 
        #endregion

        #region IDockableControl Members

        public virtual bool IsDocked
        {
            get { return true; }
        }

        #endregion

        public virtual DockingManager GetManager()
        {
            DependencyObject parent = LogicalTreeHelper.GetParent(this);

            while (parent != null &&
                (!(parent is DockingManager)))
                parent = LogicalTreeHelper.GetParent(parent);

            return parent as DockingManager;
        }

        


        #region IDockableControl Members

        #endregion

        #region Membri di IDropSurface
        #region Drag pane services

        DockingManager _oldManager = null;
        //protected override void OnVisualParentChanged(DependencyObject oldParent)
        //{
        //    base.OnVisualParentChanged(oldParent);

        //    RemoveDragPaneReferences();

        //    AddDragPaneReferences();
        //}

        protected void RemoveDragPaneReferences()
        {
            if (_oldManager != null)
            {
                _oldManager.DragPaneServices.Unregister(this);
                _oldManager = null;
            }

        }

        protected void AddDragPaneReferences()
        { 
            {
                _oldManager = GetManager();
                if (_oldManager != null)
                    _oldManager.DragPaneServices.Register(this);
            }
        }
        #endregion


        public abstract bool IsSurfaceVisible {get;}
        
        public virtual Rect SurfaceRectangle
        {
            get
            {
                if (!IsSurfaceVisible)
                    return new Rect();

                if (PresentationSource.FromVisual(this) == null)
                    return new Rect();

                return new Rect(HelperFunc.PointToScreenWithoutFlowDirection(this, new Point()), new Size(ActualWidth, ActualHeight));
            }
        }

        public virtual void OnDragEnter(Point point)
        {
            GetManager().OverlayWindow.ShowOverlayPaneDockingOptions(this);
        }

        public virtual void OnDragOver(Point point)
        {

        }

        public virtual void OnDragLeave(Point point)
        {
            GetManager().OverlayWindow.HideOverlayPaneDockingOptions(this);
        }

        public virtual bool OnDrop(Point point)
        {
            return false;
        }

        #endregion


        public virtual ManagedContent RemoveContent(int index)
        {
            ManagedContent contentToRemove = Items[index] as ManagedContent;

            Items.RemoveAt(index);

            return contentToRemove;
        }

        protected FrameworkElement _partHeader = null;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //gets a reference to the header for the pane
            _partHeader = GetTemplateChild("PART_Header") as FrameworkElement;
        }


        internal virtual ResizingPanel GetContainerPanel()
        {
            return LogicalTreeHelper.GetParent(this) as ResizingPanel;
        }

        /// <summary>
        /// Closes or hides provided content depending on HideOnClose property
        /// </summary>
        internal virtual void CloseOrHide(DockableContent cntToCloseOrHide)
        {
            CloseOrHide(cntToCloseOrHide, false);
        }

        /// <summary>
        /// Closes or hides provided content depending on HideOnClose property
        /// </summary>
        internal virtual void CloseOrHide(DockableContent cntToCloseOrHide, bool force)
        {
            Debug.Assert(cntToCloseOrHide != null);

            if (!force && !cntToCloseOrHide.IsCloseable)
                return;

            DockingManager manager = GetManager();
            if (cntToCloseOrHide.HideOnClose && manager != null)
                manager.Hide(cntToCloseOrHide);
            else
                RemoveContent(Items.IndexOf(cntToCloseOrHide));
        }


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
