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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.Windows.Threading;

namespace AvalonDock
{
    public abstract class Pane : 
        System.Windows.Controls.Primitives.Selector, 
        IDropSurface,
        IDockableControl,
        INotifyPropertyChanged
    {
        static Pane()
        {
            //FocusableProperty.OverrideMetadata(typeof(Pane), new FrameworkPropertyMetadata(false));
        }

        internal Pane()
        {
            this.Loaded += new RoutedEventHandler(OnLoaded);
            this.Unloaded += new RoutedEventHandler(OnUnloaded);
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            //if (GetManager() == null && Parent != null)
            //    throw new InvalidOperationException("Pane must be put under a DockingManager!");

            AddDragPaneReferences();
        }

        protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
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

        
        ManagedContent _lastSelectedContent = null;
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.RemovedItems != null &&
                e.RemovedItems.Count > 0 &&
                e.AddedItems != null &&
                e.AddedItems.Count > 0)
                _lastSelectedContent = e.RemovedItems[0] as ManagedContent;

            base.OnSelectionChanged(e);
        }

        
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DockingManager dockManager = GetManager();
            if (dockManager != null)
                dockManager.RefreshContents();

            if (e.NewItems != null)
                CheckItems(e.NewItems);

            HasSingleItem = (Items.Count == 1);

            if (_lastSelectedContent != null &&
                !Items.Contains(_lastSelectedContent))
                _lastSelectedContent = null;

            if ((e.NewItems == null || e.NewItems.Count == 0) &&
                (e.OldItems != null && e.OldItems.Count > 0))
            {
                if (_lastSelectedContent != null &&
                    Items.Contains(_lastSelectedContent))
                    SelectedItem = _lastSelectedContent;
            }

            //let base class handle SelectedIndex/Item value
            base.OnItemsChanged(e);
            
            if (Items.Count > 0)
            {
                int currentIndex = SelectedIndex;

                if (currentIndex < 0 ||
                    currentIndex >= Items.Count)
                    currentIndex = Items.Count - 1;

                SelectedItem = Items.GetItemAt(currentIndex);
            }

            RefreshContainsActiveContentProperty();

            if (Items.Count > 0)
            {
                var parentPanel = Parent as ResizingPanel;
                while (parentPanel != null && parentPanel.IsLoaded)
                {
                    parentPanel.UpdateLayout();
                    parentPanel.InvalidateMeasure();
                    parentPanel = parentPanel.Parent as ResizingPanel;
                }
            }


        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            DockingManager dockManager = GetManager();
            if (dockManager != null)
                dockManager.RefreshContents();
            
            base.OnVisualParentChanged(oldParent);
        }

        protected virtual void CheckItems(IList newItems)
        {
            foreach (object newItem in newItems)
            {
                if (!(newItem is ManagedContent))
                    throw new InvalidOperationException("Pane can contain only ManagedContents!");
            }
        }

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

        #region ContainsActiveContent

        /// <summary>
        /// ContainsActiveContent Read-Only Dependency Property
        /// </summary>
        /// <remarks>This property is specially intended for use in restyling.</remarks>
        private static readonly DependencyPropertyKey ContainsActiveContentPropertyKey
            = DependencyProperty.RegisterReadOnly("ContainsActiveContent", typeof(bool), typeof(Pane),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(OnContainsActiveContentChanged)));

        public static readonly DependencyProperty ContainsActiveContentProperty
            = ContainsActiveContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the ContainsActiveContent property.  This dependency property 
        /// indicates if this <see cref="Pane"/> contains a <see cref="ManagedContent"/> set as active content into the parent <see cref="DockingManager"/> object. 
        /// </summary>
        public bool ContainsActiveContent
        {
            get { return (bool)GetValue(ContainsActiveContentProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the ContainsActiveContent property.  
        /// This dependency property indicates if this <see cref="Pane"/> contains a <see cref="ManagedContent"/> set as active content into the parent <see cref="DockingManager"/> object.
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        protected void SetContainsActiveContent(bool value)
        {
            SetValue(ContainsActiveContentPropertyKey, value);
        }

        /// <summary>
        /// Handles changes to the ContainsActiveContent property.
        /// </summary>
        private static void OnContainsActiveContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Pane)d).OnContainsActiveContentChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ContainsActiveContent property.
        /// </summary>
        protected virtual void OnContainsActiveContentChanged(DependencyPropertyChangedEventArgs e)
        {
        }


        internal void RefreshContainsActiveContentProperty()
        {
            SetContainsActiveContent(
                Items.Cast<ManagedContent>().FirstOrDefault(d => d.IsActiveContent) != null);

            if (Items.Count> 0)
                Debug.WriteLine(string.Format("{0} ContainsActiveContent ={1}", (Items[0] as ManagedContent).Title, ContainsActiveContent));
        }
        #endregion

        #region IDockableControl Members

        #endregion

        #region Membri di IDropSurface
        #region Drag pane services

        DockingManager _oldManager = null;

        protected void RemoveDragPaneReferences()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (_oldManager != null)
                {
                    _oldManager.DragPaneServices.Unregister(this);
                    _oldManager = null;
                }
            }
        }

        protected void AddDragPaneReferences()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _oldManager = GetManager();
                if (_oldManager != null)
                    _oldManager.DragPaneServices.Register(this);
            }
        }
        #endregion

        protected abstract bool IsSurfaceVisible
        {
            get;
        }

        bool IDropSurface.IsSurfaceVisible { get { return IsSurfaceVisible; } }


        protected virtual Rect SurfaceRectangle
        { 
            get
            {
                if (!IsSurfaceVisible)
                    return new Rect();

                if (PresentationSource.FromVisual(this) == null)
                    return new Rect();

                var actualSize = this.TransformedActualSize();
                return new Rect(HelperFunc.PointToScreenWithoutFlowDirection(this, new Point()), new Size(actualSize.Width, actualSize.Height));
            }            
        }
        Rect IDropSurface.SurfaceRectangle
        {
            get { return SurfaceRectangle; }
        }

        protected virtual void OnDragEnter(Point point)
        {
            GetManager().OverlayWindow.ShowOverlayPaneDockingOptions(this);
        }
        void IDropSurface.OnDragEnter(Point point)
        {
            OnDragEnter(point);
        }

        protected virtual void OnDragOver(Point point)
        { 
            
        }

        void IDropSurface.OnDragOver(Point point)
        {
            OnDragOver(point);
        }

        protected virtual void OnDragLeave(Point point)
        {
            GetManager().OverlayWindow.HideOverlayPaneDockingOptions(this);
        }


        void IDropSurface.OnDragLeave(Point point)
        {
            OnDragLeave(point);
        }


        protected virtual bool OnDrop(Point point)
        {
            return false;
        }

        bool IDropSurface.OnDrop(Point point)
        {
            return OnDrop(point);
        }

        #endregion

        #region Commands
        protected override void OnInitialized(EventArgs e)
        {
            this.CommandBindings.Add(
                new CommandBinding(PaneCommands.Dock, this.OnExecuteCommand, this.OnCanExecuteCommand));

            base.OnInitialized(e);
        }

        protected virtual void OnExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == PaneCommands.Dock)
            {
                Dock();
                e.Handled = true;
            }
        }

        protected virtual void OnCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.GetManager() != null;

            if (e.CanExecute)
            {
                if (e.Command == PaneCommands.Dock)
                {
                    e.CanExecute = true;
                }
            }

            Debug.WriteLine(string.Format("Pane.OnCanExecuteCommand({0}) = {1} (ContinueRouting={2})", e.Command, e.CanExecute, e.ContinueRouting));
        }

        /// <summary>
        /// Dock contained contents to the container <see cref="DockingManager"/>
        /// </summary>
        public virtual void Dock()
        { 
        
        }
        #endregion

        internal virtual ManagedContent RemoveContent(ManagedContent contentToRemove)
        {
            Items.Remove(contentToRemove);

            return contentToRemove;
        }


        internal virtual ManagedContent RemoveContent(int index)
        {
            ManagedContent contentToRemove = Items[index] as ManagedContent;

            return RemoveContent(contentToRemove);
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
        /// Gest or set a value indicating if pane should show the tab header
        /// </summary>
        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowHeaderProperty =
            DependencyProperty.Register("ShowHeader", typeof(bool), typeof(Pane), new UIPropertyMetadata(true));

        /// <summary>
        /// Move focus to pane content and activate it
        /// </summary>
        protected void FocusContent()
        {
            ManagedContent selectedContent = SelectedItem as ManagedContent;
            if (selectedContent != null)// && selectedContent.Content is UIElement)
            {
                //UIElement internalContent = selectedContent.Content as UIElement;
                //bool res = Focus();
                //Keyboard.Focus(internalContent);
                selectedContent.Activate();
            }
        }

        #region OptionsContextMenu
        protected ContextMenu cxOptions = null;
        ContextMenu _attachedCxOptions = null;
        /// <summary>
        /// Open the option context menu
        /// </summary>
        /// <param name="menuTarget">Target element under which context menu will be shown. Pass null if context menu
        /// should be shown at mouse position.</param>
        /// <returns>True if context menu resource was found and open, false otherwise.</returns>
        public virtual bool OpenOptionsMenu(UIElement menuTarget)
        {
            if (_attachedCxOptions != cxOptions)
            {
                if (_attachedCxOptions != null)
                {
                    cxOptions.Opened -= (s, e) => UpdateIsOptionsMenuOpen();
                    cxOptions.Closed -= (s, e) => UpdateIsOptionsMenuOpen();
                }
            
                _attachedCxOptions = cxOptions;

                if (_attachedCxOptions != null)
                {
                    cxOptions.Opened += (s, e) => UpdateIsOptionsMenuOpen();
                    cxOptions.Closed += (s, e) => UpdateIsOptionsMenuOpen();
                }
            }

            if (cxOptions != null)
            {
                //FocusContent();
            }

            if (cxOptions != null)
            {
                cxOptions.DataContext = this.SelectedItem;

                foreach (MenuItem menuItem in cxOptions.Items.OfType<MenuItem>())
                    menuItem.CommandTarget = this.SelectedItem as IInputElement;

                if (menuTarget != null)
                {
                    cxOptions.Placement = PlacementMode.Bottom;
                    cxOptions.PlacementTarget = menuTarget;
                }
                else
                {
                    cxOptions.Placement = PlacementMode.MousePoint;
                    cxOptions.PlacementTarget = this;
                }

                cxOptions.IsOpen = true;
            }

            return (cxOptions != null && cxOptions.IsOpen);
        }

        /// <summary>
        /// Close the options context menu
        /// </summary>
        public virtual void CloseOptionsMenu()
        {
            if (cxOptions != null)
            {
                cxOptions.IsOpen = false;
                cxOptions = null;
            }
        }

        /// <summary>
        /// Gets a value indicating if the options context menu is open
        /// </summary>
        public bool IsOptionsMenuOpen
        {
            get { return (bool)GetValue(IsOptionsMenuOpenProperty); }
            protected set { SetValue(IsOptionsMenuOpenPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for IsOptionsMenuOpen.  This enables animation, styling, binding, etc...
        static readonly DependencyPropertyKey IsOptionsMenuOpenPropertyKey =
            DependencyProperty.RegisterReadOnly("IsOptionsMenuOpen", typeof(bool), typeof(DockablePane), new UIPropertyMetadata(false));

        public static readonly DependencyProperty IsOptionsMenuOpenProperty = IsOptionsMenuOpenPropertyKey.DependencyProperty;

        void UpdateIsOptionsMenuOpen()
        {
            if (cxOptions != null)
            {
                var selectedContent = cxOptions.DataContext as DockableContent;

                if (selectedContent != null && selectedContent.ContainerPane != null)
                {
                    (selectedContent.ContainerPane as Pane).IsOptionsMenuOpen =
                        cxOptions.IsOpen;
                }
            }
        }

        #endregion


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
