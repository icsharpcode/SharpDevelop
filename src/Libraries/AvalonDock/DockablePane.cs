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
using System.ComponentModel;
using System.Collections;


namespace AvalonDock
{
    /// <summary>
    /// Anchor types
    /// </summary>
    public enum AnchorStyle
    {
        /// <summary>
        /// No anchor style, while content is hosted in a <see cref="DocumentPane"/> or a <see cref="FloatingWindow"/>
        /// </summary>
        None,
        /// <summary>
        /// Top border anchor
        /// </summary>
        Top,
        /// <summary>
        /// Left border anchor
        /// </summary>
        Left,
        /// <summary>
        /// Bottom border anchor
        /// </summary>
        Bottom,
        /// <summary>
        /// Right border anchor
        /// </summary>
        Right
    }

   
    public class DockablePane : Pane
    {
        static DockablePane()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockablePane), new FrameworkPropertyMetadata(typeof(DockablePane)));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.CommandBindings.Add(
                new CommandBinding(ShowOptionsCommand, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(ToggleAutoHideCommand, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(CloseCommand, this.OnExecuteCommand, this.OnCanExecuteCommand));
        }

        public DockablePane()
        {
            this.Loaded += new RoutedEventHandler(DockablePane_Loaded);
            this.Unloaded += new RoutedEventHandler(DockablePane_Unloaded);
        }

        void DockablePane_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        void DockablePane_Unloaded(object sender, RoutedEventArgs e)
        {
            UnloadOptionsContextMenu();
        }


        #region Dependency properties

        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowHeaderProperty =
            DependencyProperty.Register("ShowHeader", typeof(bool), typeof(DockablePane), new UIPropertyMetadata(true));


        public bool ShowTabs
        {
            get { return (bool)GetValue(ShowTabsProperty); }
            set { SetValue(ShowTabsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowTabs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowTabsProperty =
            DependencyProperty.Register("ShowTabs", typeof(bool), typeof(DockablePane), new UIPropertyMetadata(true));


        public AnchorStyle Anchor
        {
            get { return (AnchorStyle)GetValue(AnchorPropertyKey.DependencyProperty); }
            internal set { SetValue(AnchorPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for Anchor.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey AnchorPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("Anchor", typeof(AnchorStyle), typeof(DockablePane), new UIPropertyMetadata(AnchorStyle.None));


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_partHeader != null)
            {
                _partHeader.MouseDown += new MouseButtonEventHandler(OnHeaderMouseDown);
                _partHeader.MouseMove += new MouseEventHandler(OnHeaderMouseMove);
                _partHeader.MouseUp += new MouseButtonEventHandler(OnHeaderMouseUp);
                _partHeader.MouseEnter += new MouseEventHandler(OnHeaderMouseEnter);
                _partHeader.MouseLeave += new MouseEventHandler(OnHeaderMouseLeave);
            }

            _optionsContextMenuPlacementTarget = GetTemplateChild("PART_ShowContextMenuButton") as UIElement;
        }



        
        #endregion
        
        
        public override bool IsSurfaceVisible
        {
            get
            {
                foreach (ManagedContent managedContent in Items)
                {
                    if (managedContent is DocumentContent)
                        continue;

                    if (((DockableContent)managedContent).State == DockableContentState.Docked)
                        return true;
                }

                return false;
            }
        }


        #region OptionsContextMenu
        ContextMenu cxOptions = null;

        UIElement _optionsContextMenuPlacementTarget = null;

        void LoadOptionsContextMenu()
        {
            Debug.Assert(cxOptions == null);
            cxOptions = FindResource(new ComponentResourceKey(typeof(DockingManager), ContextMenuElement.DockablePane)) as ContextMenu;
            cxOptions.Opened += new RoutedEventHandler(cxOptions_RefreshOpenState);
            cxOptions.Closed += new RoutedEventHandler(cxOptions_RefreshOpenState);
        }

        void UnloadOptionsContextMenu()
        {
            if (cxOptions != null)
            {
                cxOptions.Opened -= new RoutedEventHandler(cxOptions_RefreshOpenState);
                cxOptions.Closed -= new RoutedEventHandler(cxOptions_RefreshOpenState);
                cxOptions = null;
            }
        }


        protected virtual void OpenOptionsContextMenu()
        {
            if (cxOptions == null)
            {
                LoadOptionsContextMenu();
            }

            if (cxOptions != null)
            {
                cxOptions.DataContext = this.SelectedItem as DockableContent;

                foreach (MenuItem menuItem in cxOptions.Items)
                    menuItem.CommandTarget = this.SelectedItem as DockableContent;

                if (_optionsContextMenuPlacementTarget != null)
                {
                    cxOptions.Placement = PlacementMode.Bottom;
                    cxOptions.PlacementTarget = _optionsContextMenuPlacementTarget;
                }
                else
                {
                    cxOptions.Placement = PlacementMode.MousePoint;
                    cxOptions.PlacementTarget = this;
                }

                cxOptions.IsOpen = true;
            }                
        }

        void  cxOptions_RefreshOpenState(object sender, RoutedEventArgs e)
        {
            NotifyPropertyChanged("IsOptionsMenuOpened");
        } 

        public bool IsOptionsMenuOpened
        {
            get 
            {
                return cxOptions != null && cxOptions.IsOpen && (
                    _optionsContextMenuPlacementTarget != null ?
                    cxOptions.PlacementTarget == _optionsContextMenuPlacementTarget :
                    cxOptions.PlacementTarget == this); 
            }
        }

        #endregion

        #region Mouse management

        void FocusContent()
        {
            ManagedContent selectedContent = SelectedItem as ManagedContent;
            if (selectedContent != null && selectedContent.Content is UIElement)
            {
                //UIElement internalContent = selectedContent.Content as UIElement;
                //bool res = internalContent.Focus();
                selectedContent.SetAsActive();
            }
        }

        Point ptStartDrag;
        bool isMouseDown = false;
        protected virtual void OnHeaderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                FocusContent();
                
                if (((DockableContent)SelectedItem).State != DockableContentState.AutoHide)
                {
                    ptStartDrag = e.MouseDevice.GetPosition(this);
                    isMouseDown = true;
                }
            }
        }

        protected virtual void OnHeaderMouseMove(object sender, MouseEventArgs e)
        {
            Point ptMouseMove = e.GetPosition(this);

            if (!e.Handled && isMouseDown)
            {
                if (_partHeader != null &&
                    _partHeader.IsMouseOver)
                {
                    if (!IsMouseCaptured)
                    {
                        if (Math.Abs(ptMouseMove.X - ptStartDrag.X) > SystemParameters.MinimumHorizontalDragDistance ||
                            Math.Abs(ptMouseMove.Y - ptStartDrag.Y) > SystemParameters.MinimumVerticalDragDistance)
                        {
                            isMouseDown = false;
                            ReleaseMouseCapture();
                            DockingManager manager = GetManager();
                            manager.Drag(this, this.PointToScreenDPI(e.GetPosition(this)), e.GetPosition(this));
                            e.Handled = true;
                        }
                    }
                }
            }

        }

        protected virtual void OnHeaderMouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;
            ReleaseMouseCapture();
        }

        protected virtual void OnHeaderMouseEnter(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
           
        }
        protected virtual void OnHeaderMouseLeave(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
           
        }
        #endregion
           
        #region Commands
        private static object syncRoot = new object();


        private static RoutedUICommand optionsCommand = null;
        public static RoutedUICommand ShowOptionsCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == optionsCommand)
                    {
                        optionsCommand = new RoutedUICommand("S_how options", "Options", typeof(DockablePane));
                    }
                }
                return optionsCommand;
            }
        }

        private static RoutedUICommand autoHideCommand = null;
        public static RoutedUICommand ToggleAutoHideCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == autoHideCommand)
                    {
                        autoHideCommand = new RoutedUICommand("A_utohide", "AutoHide", typeof(DockablePane));
                    }
                }
                return autoHideCommand;
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
                        closeCommand = new RoutedUICommand("C_lose", "Close", typeof(DockablePane));
                    }
                }
                return closeCommand;
            }
        }


        internal virtual void OnExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ToggleAutoHideCommand)
            {
                ToggleAutoHide();
                e.Handled = true;
            }
            else if (e.Command == CloseCommand)
            {
                CloseOrHide();
                e.Handled = true;
            }
            else if (e.Command == ShowOptionsCommand)
            {
                OpenOptionsContextMenu();
                e.Handled = true;
            }

        }

        protected virtual void OnCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }        
 
	    #endregion    


        public override bool IsDocked
        {
            get { return IsSurfaceVisible; }
        }


        public DockableStyle GetCumulativeDockableStyle()
        {
            DockableStyle style = DockableStyle.Dockable;

            if (Items.Count == 1 &&
                Items[0] is DocumentContent)
                style = DockableStyle.Document;
            else
            {
                foreach (DockableContent content in this.Items)
                {
                    style &= content.DockableStyle;
                }
            }

            return style;
        }

        public override ManagedContent RemoveContent(int index)
        {
            ManagedContent content = base.RemoveContent(index);

            if (Items.Count == 0)
            {
                ResizingPanel containerPanel = Parent as ResizingPanel;
                if (containerPanel != null)
                    containerPanel.RemoveChild(this);
            }

            return content;
        }

        protected virtual void CheckItems(IList newItems)
        { 
            foreach (object newItem in newItems)
            {
                if (!(newItem is DockableContent))
                    throw new InvalidOperationException("DockablePane can contain only DockableContents!");
            }
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                CheckItems(e.NewItems);

            base.OnItemsChanged(e);
        }



        #region Dockable Pane operations
		/// <summary>
        /// Show/Hide a flyout window containing this pane
        /// </summary>
        internal virtual void ToggleAutoHide()
        {
            bool flag = true;
            foreach (DockableContent cnt in this.Items)
            {
                if ((cnt.DockableStyle & DockableStyle.AutoHide) == 0)
                {
                    flag = false;
                    break;
                }
            }
            if (flag && GetManager() != null)
                GetManager().ToggleAutoHide(this);
        }


        /// <summary>
        /// Closes or hides current content depending on HideOnClose property
        /// </summary>
        internal void CloseOrHide()
        {
            CloseOrHide(SelectedItem as DockableContent, false);
        }



        
	    #endregion    
    
    }
}
