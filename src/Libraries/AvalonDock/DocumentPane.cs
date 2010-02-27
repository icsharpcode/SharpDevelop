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

namespace AvalonDock
{
    public class DocumentPane : Pane
    {
        static DocumentPane()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentPane), new FrameworkPropertyMetadata(typeof(DocumentPane)));
        }

        public DocumentPane()
        {
            this.Loaded += new RoutedEventHandler(DocumentPane_Loaded);
        }

        void DocumentPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parent == null)
                return;

            if (GetManager() == null)
                throw new InvalidOperationException("DocumentPane must be put under a DockingManager!");

            //try to set this as main document pane
            if (GetManager().MainDocumentPane == null)
            {
                GetManager().MainDocumentPane = this;
                NotifyPropertyChanged("IsMainDocumentPane");
            }
            else
            {
                //or ensure that this document pane is under or at the same level of the MainDocumentPane
                GetManager().EnsurePanePositionIsValid(this);
            }
        }

        public bool? IsMainDocumentPane
        {
            get 
            {
                if (GetManager() == null)
                    return null;

                return GetManager().MainDocumentPane == this;
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            this.CommandBindings.Add(
                new CommandBinding(ShowDocumentsListMenuCommand, ExecutedShowDocumentsListMenuCommand, CanExecuteShowDocumentsListMenuCommand));
            this.CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Close, ExecutedCloseCommand, CanExecuteCloseCommand));
            this.CommandBindings.Add(
                new CommandBinding(CloseCommand, ExecutedCloseCommand, CanExecuteCloseCommand));

            this.CommandBindings.Add(
                new CommandBinding(CloseAllButThisCommand, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(NewHorizontalTabGroupCommand, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(NewVerticalTabGroupCommand, this.OnExecuteCommand, this.OnCanExecuteCommand));


            base.OnInitialized(e);
        }

        #region DocumentPane Commands


        #region Show Document Window List Command

        public static RoutedCommand ShowDocumentsListMenuCommand = new RoutedCommand();

        public void ExecutedShowDocumentsListMenuCommand(object sender,
            ExecutedRoutedEventArgs e)
        {
            //MessageBox.Show("ShowOptionsMenu");
            ShowDocumentsListMenu(this, null);
        }


        public void CanExecuteShowDocumentsListMenuCommand(object sender,
            CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion


        #region Close Command

        //ApplicationCommands.Close command....

        public void ExecutedCloseCommand(object sender,
            ExecutedRoutedEventArgs e)
        {
            if (GetManager() == null)
                return;

            ManagedContent contentToClose = SelectedItem as ManagedContent;

            if (e.Parameter is ManagedContent)
                contentToClose = e.Parameter as ManagedContent;

            DockableContent dockableContent = contentToClose as DockableContent;

            if (dockableContent != null)
                CloseOrHide(dockableContent);
            else
            {
                DocumentContent documentContent = contentToClose as DocumentContent;
                documentContent.Close();

                //if (documentContent != null)
                //    Items.Remove(documentContent);

                //CheckContentsEmpty();
            }

        }


        public void CanExecuteCloseCommand(object sender,
            CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region Activate Document Command
        public static RoutedCommand ActivateDocumentCommand = new RoutedCommand();

        public void ExecutedActivateDocumentCommand(object sender,
            ExecutedRoutedEventArgs e)
        {
            ManagedContent doc = e.Parameter as ManagedContent;
            if (doc != null)
            {
                if (!DocumentTabPanel.GetIsHeaderVisible(doc))
                {
                    DocumentPane parentPane = doc.ContainerPane as DocumentPane;
                    parentPane.Items.Remove(doc);
                    parentPane.Items.Insert(0, doc);
                }
                ////doc.IsSelected = true;
                ////Selector.SetIsSelected(doc, true);
                //if (this.GetManager() != null)
                //    this.GetManager().ActiveContent = doc;
                doc.SetAsActive();
            }            
        }

        public void CanExecuteActivateDocumentCommand(object sender,
            CanExecuteRoutedEventArgs e)
        {
            //ManagedContent doc = e.Parameter as ManagedContent;
            //if (doc != null && !doc.IsSelected)
                e.CanExecute = true;
        }

        #endregion


        #region Commands
        private static object syncRoot = new object();


        private static RoutedUICommand closeAllButThisCommand = null;
        public static RoutedUICommand CloseAllButThisCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == closeAllButThisCommand)
                    {
                        closeAllButThisCommand = new RoutedUICommand("Close All But This", "CloseAllButThis", typeof(DocumentPane));
                    }
                }
                return closeAllButThisCommand;
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
                        closeCommand = new RoutedUICommand("C_lose", "Close", typeof(DocumentPane));
                    }
                }
                return closeCommand;
            }
        }

        private static RoutedUICommand newHTabGroupCommand = null;
        public static RoutedUICommand NewHorizontalTabGroupCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == newHTabGroupCommand)
                    {
                        newHTabGroupCommand = new RoutedUICommand("New Horizontal Tab Group", "NewHorizontalTabGroup", typeof(DocumentPane));
                    }
                }
                return newHTabGroupCommand;
            }
        }

        private static RoutedUICommand newVTabGroupCommand = null;
        public static RoutedUICommand NewVerticalTabGroupCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == newVTabGroupCommand)
                    {
                        newVTabGroupCommand = new RoutedUICommand("New Vertical Tab Group", "NewVerticalTabGroup", typeof(DocumentPane));
                    }
                }
                return newVTabGroupCommand;
            }
        }



        internal virtual void OnExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == CloseAllButThisCommand)
            {
                CloseAllButThis();
                e.Handled = true;
            }
            else if (e.Command == NewHorizontalTabGroupCommand)
            {
                NewHorizontalTabGroup();
                e.Handled = true;
            }
            else if (e.Command == NewVerticalTabGroupCommand)
            {
                NewVerticalTabGroup();
                e.Handled = true;
            }

        }

        protected virtual void OnCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.GetManager() != null;

            if (e.CanExecute)
            {
                if (e.Command == NewHorizontalTabGroupCommand ||
                    e.Command == NewVerticalTabGroupCommand)
                {
                    if (this.Items.Count <= 1)
                        e.CanExecute = false;
                }
            }
        }

        void CloseAllButThis()
        {
            DocumentContent activeContent = GetManager().ActiveDocument as DocumentContent;
            foreach (DocumentContent cnt in this.GetManager().Documents)
            {
                if (cnt != activeContent)
                    cnt.Close();
            }
        }
        void NewHorizontalTabGroup()
        {
            ManagedContent activeContent = SelectedItem as ManagedContent;
            DocumentPane newContainerPane = new DocumentPane();
            
            int indexOfDocumentInItsContainer = activeContent.ContainerPane.Items.IndexOf(activeContent);
            activeContent.ContainerPane.RemoveContent(indexOfDocumentInItsContainer);
            newContainerPane.Items.Add(activeContent);

            GetManager().Anchor(newContainerPane, this, AnchorStyle.Bottom);

        }
        void NewVerticalTabGroup()
        {
            ManagedContent activeContent = SelectedItem as ManagedContent;
            DocumentPane newContainerPane = new DocumentPane();

            int indexOfDocumentInItsContainer = activeContent.ContainerPane.Items.IndexOf(activeContent);
            activeContent.ContainerPane.RemoveContent(indexOfDocumentInItsContainer);
            newContainerPane.Items.Add(activeContent);

            GetManager().Anchor(newContainerPane, this, AnchorStyle.Right);
        }

        #endregion    

        #endregion

        UIElement _optionsContextMenuPlacementTarget;

        public override void OnApplyTemplate()
        {
            _optionsContextMenuPlacementTarget = GetTemplateChild("PART_ShowContextMenuButton") as UIElement;

            

            base.OnApplyTemplate();
        }

        void ShowDocumentsListMenu(object sender, RoutedEventArgs e)
        {
            if (Items.Count == 0)
                return; //nothings to show

            ContextMenu cxMenuDocuments = (ContextMenu)FindResource("DocumentsListMenu");
            if (cxMenuDocuments != null)
            {
                cxMenuDocuments.ItemsSource = Items;
                cxMenuDocuments.CommandBindings.Add(new CommandBinding(ActivateDocumentCommand, new ExecutedRoutedEventHandler(this.ExecutedActivateDocumentCommand), new CanExecuteRoutedEventHandler(CanExecuteActivateDocumentCommand)));

                if (_optionsContextMenuPlacementTarget != null)
                {
                    cxMenuDocuments.Placement = PlacementMode.Bottom;
                    cxMenuDocuments.PlacementTarget = _optionsContextMenuPlacementTarget;
                }
                else
                {
                    cxMenuDocuments.Placement = PlacementMode.MousePoint;
                    cxMenuDocuments.PlacementTarget = this;
                }
                
                cxMenuDocuments.IsOpen = true;
            }
        }
        
        public override bool IsSurfaceVisible
        {
            get 
            {
                if (IsMainDocumentPane.HasValue &&
                    !IsMainDocumentPane.Value &&
                    Items.Count == 0)
                    return false;

                return true; 
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (!e.Handled)
            {
                if (_partHeader != null &&
                    !_partHeader.IsMouseOver)
                {
                    //prevent document content to start dragging when it is clicked outside of the header area
                    e.Handled = true;
                }
            }
        }

        internal void CheckContentsEmpty()
        { 
            if (Items.Count == 0)
            {
                bool isMainDocPaneToBeClose = IsMainDocumentPane.HasValue &&
                    IsMainDocumentPane.Value;

                if (isMainDocPaneToBeClose)
                {
                    DockingManager manager = GetManager();
                    DocumentPane candidateNewMainDocPane = manager.FindAnotherLogicalChildContained<DocumentPane>(this);
                    if (candidateNewMainDocPane != null)
                    {
                        ResizingPanel containerPanel = Parent as ResizingPanel;
                        if (containerPanel != null)
                            containerPanel.RemoveChild(this);

                        manager.MainDocumentPane = candidateNewMainDocPane;
                        candidateNewMainDocPane.NotifyPropertyChanged("IsMainDocumentPane");
                    }
                }
                else
                {
                    ResizingPanel containerPanel = Parent as ResizingPanel;
                    if (containerPanel != null)
                        containerPanel.RemoveChild(this);
                }
            }

        }


        internal override ResizingPanel GetContainerPanel()
        {
            return GetParentDocumentPaneResizingPanel();
        }

        internal DocumentPaneResizingPanel GetParentDocumentPaneResizingPanel()
        {
            ResizingPanel parentPanel = LogicalTreeHelper.GetParent(this) as ResizingPanel;
            
            if (parentPanel == null)
                return null;

            while (!(parentPanel is DocumentPaneResizingPanel))
            {
                parentPanel = LogicalTreeHelper.GetParent(parentPanel) as ResizingPanel;

                if (parentPanel == null)
                    return null;
            }

            return parentPanel as DocumentPaneResizingPanel;
        }

        public override Rect SurfaceRectangle
        {
            get
            {
                //it is dragging a document let drop in this document pane
                if (GetManager().DragPaneServices.FloatingWindow is DocumentFloatingWindow)
                    return base.SurfaceRectangle;

                //otherwise we should provide a drop surface for all the DocumentPaneResizingPanel
                DocumentPaneResizingPanel parentPanel = GetParentDocumentPaneResizingPanel();

                if (parentPanel == null)
                    return base.SurfaceRectangle;

                return new Rect(HelperFunc.PointToScreenWithoutFlowDirection(parentPanel, new Point(0, 0)), new Size(parentPanel.ActualWidth, parentPanel.ActualHeight));
                //return new Rect(parentPanel.PointToScreen(new Point(0, 0)), new Size(parentPanel.ActualWidth, parentPanel.ActualHeight));
            }
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object newItem in e.NewItems)
                {
                    if (!(newItem is DockableContent) &&
                        !(newItem is DocumentContent))
                        throw new InvalidOperationException("DocumentPane can contain only DockableContents or DocumentContents!");

                    if (newItem is DockableContent &&
                        (((DockableContent)newItem).DockableStyle & DockableStyle.Document) == 0)
                    {
                        ((DockableContent)newItem).DockableStyle |= DockableStyle.Document;
                    }

                }
            }

            base.OnItemsChanged(e);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            ManagedContent selectedContent = this.SelectedItem as ManagedContent;
            if (selectedContent != null && GetManager() != null)
                GetManager().ActiveDocument = selectedContent;

            base.OnSelectionChanged(e);
        }

    }
}
