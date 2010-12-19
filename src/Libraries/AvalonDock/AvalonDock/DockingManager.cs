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
using System.Diagnostics;
using System.Windows.Interop;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;


namespace AvalonDock
{
 
    /// <summary>
    /// Represents a control which manages a dockable layout for its children
    /// </summary>
    public class DockingManager : ContentControl, IDropSurface, INotifyPropertyChanged, IDisposable
    {
        static DockingManager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockingManager), new FrameworkPropertyMetadata(typeof(DockingManager)));
        }


        public DockingManager()
        {
            Debug.WriteLine("DockingManager ctr");

            Documents = new ManagedContentCollection<DocumentContent>(this);
            DockableContents = new ManagedContentCollection<DockableContent>(this);
            //HiddenContents = new ManagedContentCollection<DockableContent>(this);

            this.Loaded += new RoutedEventHandler(DockingManager_Loaded);
            this.Unloaded += new RoutedEventHandler(DockingManager_Unloaded);
        }


        #region Control lifetime management
         ~DockingManager()
        {
            Dispose(false);
        }

        bool _isControlLoaded = false;
        
        void DockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("DockingManager Loaded");

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                foreach (FloatingWindow floatingWindow in _floatingWindows)
                {
                    floatingWindow.Owner = Window.GetWindow(this);
                    if (floatingWindow.IsVisible)
                        floatingWindow.Hide();

                    floatingWindow.Show();
                }

                DragPaneServices.Register(this);
            }


            _isControlLoaded = true;
        }

        void DockingManager_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("DockingManager Unloaded");

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                //cleanup pending resources
                HideFlyoutWindow();

                if (_overlayWindow != null)
                {
                    _overlayWindow.Close();
                    _overlayWindow = null;
                }

                foreach (FloatingWindow floatingWindow in _floatingWindows)
                    floatingWindow.Hide();

                //navigator windows are now automatically disposed when 
                //no longer used. In this way we avoid WPF bug:
                //http://social.msdn.microsoft.com/forums/en/wpf/thread/f3fc5b7e-e035-4821-908c-b6c07e5c7042/
                //if (navigatorWindow != null)
                //{
                //    navigatorWindow.Close();
                //    navigatorWindow = null;
                //}

                //if (documentNavigatorWindow != null)
                //{
                //    documentNavigatorWindow.Close();
                //    documentNavigatorWindow = null;
                //}

                DragPaneServices.Unregister(this);
            }

            _isControlLoaded = false;
        }

        /// <summary>
        /// Call this function if you want to deallocate external floating windows, that otherwise are closed when main window is closed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                while (_floatingWindows.Count > 0)
                {
                    _floatingWindows[0].Owner = null;
                    _floatingWindows[0].Close();
                }                
            }

            _disposed = true;
        }

        #endregion

        Panel _leftAnchorTabPanel;
        Panel _rightAnchorTabPanel;
        Panel _topAnchorTabPanel;
        Panel _bottomAnchorTabPanel;

        List<Panel> _anchorTabPanels = new List<Panel>();

        bool _OnApplyTemplateFlag = false;


        Panel ReplaceAnchorTabPanel(Panel oldPanel, Panel newPanel)
        {
            if (oldPanel == null)
            {
                _anchorTabPanels.Add(newPanel);
                return newPanel;
            }
            else
            {
                _anchorTabPanels.Remove(oldPanel);
                while (oldPanel.Children.Count > 0)
                {
                    UIElement tabToTransfer = oldPanel.Children[0];
                    oldPanel.Children.RemoveAt(0);

                    newPanel.Children.Add(tabToTransfer);
                }
                _anchorTabPanels.Add(newPanel);

                return newPanel;
            }
        }
        
        /// <summary>
        /// Overriden to get a reference to underlying template elements
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Panel leftPanel = GetTemplateChild("PART_LeftAnchorTabPanel") as Panel;
            if (leftPanel == null)
                throw new ArgumentException("PART_LeftAnchorTabPanel template child element not fount!");

            Panel rightPanel = GetTemplateChild("PART_RightAnchorTabPanel") as Panel;
            if (rightPanel == null)
                throw new ArgumentException("PART_RightAnchorTabPanel template child element not fount!");

            Panel topPanel = GetTemplateChild("PART_TopAnchorTabPanel") as Panel;
            if (topPanel == null)
                throw new ArgumentException("PART_TopAnchorTabPanel template child element not fount!");

            Panel bottomPanel = GetTemplateChild("PART_BottomAnchorTabPanel") as Panel;
            if (bottomPanel == null)
                throw new ArgumentException("PART_BottomAnchorTabPanel template child element not fount!");


            _leftAnchorTabPanel = ReplaceAnchorTabPanel(_leftAnchorTabPanel, leftPanel);
            _rightAnchorTabPanel = ReplaceAnchorTabPanel(_rightAnchorTabPanel, rightPanel);
            _topAnchorTabPanel = ReplaceAnchorTabPanel(_topAnchorTabPanel, topPanel);
            _bottomAnchorTabPanel = ReplaceAnchorTabPanel(_bottomAnchorTabPanel, bottomPanel);
            
            _OnApplyTemplateFlag = true;
        }


        #region Access to contents and pane

        #region ActiveDocument


        /// <summary>
        /// ActiveDocument Dependency Property
        /// </summary>
        public static readonly DependencyProperty ActiveDocumentProperty =
            DependencyProperty.Register("ActiveDocument", typeof(ManagedContent), typeof(DockingManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnActiveDocumentChanged),
                    new CoerceValueCallback(CoerceActiveDocumentValue)));

        /// <summary>
        /// Gets or sets the ActiveDocument property.  This dependency property 
        /// indicates currently active document.
        /// </summary>
        /// <remarks>The active document not neessary receive keyboard focus. To set keyboard focus on a content see <see cref="ActiveContent"/></remarks>
        public ManagedContent ActiveDocument
        {
            get { return (ManagedContent)GetValue(ActiveDocumentProperty); }
            set { SetValue(ActiveDocumentProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ActiveDocument property.
        /// </summary>
        private static void OnActiveDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnActiveDocumentChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ActiveDocument property.
        /// </summary>
        protected virtual void OnActiveDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            var cntActivated = e.NewValue as ManagedContent;
            var cntDeactivated = e.OldValue as ManagedContent;

            if (cntDeactivated != null)
                cntDeactivated.SetIsActiveDocument(false);

            if (cntActivated != null)
                cntActivated.SetIsActiveDocument(true);

            if (ActiveContent == null)
                ActiveContent = cntActivated;

            NotifyPropertyChanged("ActiveDocument");

            if (ActiveDocumentChanged != null)
                ActiveDocumentChanged(this, EventArgs.Empty);

            if (ActiveDocument == null)
            {
                var docToActivate = Documents.OrderBy(d => d.LastActivation).FirstOrDefault();
                if (docToActivate != null)
                    docToActivate.Activate();
            }
        }

        /// <summary>
        /// Coerces the ActiveDocument value.
        /// </summary>
        private static object CoerceActiveDocumentValue(DependencyObject d, object value)
        {
            var contentToCoerce = value as ManagedContent;
            if (contentToCoerce != null &&
                (contentToCoerce.ContainerPane == null ||
                 contentToCoerce.ContainerPane.GetManager() != d))
            {
                //value is not contained in a document pane/ documentfloatingwindow so cant be the active document!
                throw new InvalidOperationException("Unable to set active document");
            }

            return value;
        }


        /// <summary>
        /// Raised whenever the <see cref="ActiveDocument"/> property changes
        /// </summary>
        public event EventHandler ActiveDocumentChanged;

        #endregion


        //ManagedContent _activeContent = null;

        ///// <summary>
        ///// Get or set the active content
        ///// </summary>
        ///// <remarks>An activated content is automatically selected in its container pane and receive logical as well keyboard focus.</remarks>
        //public ManagedContent ActiveContent
        //{
        //    get
        //    {
        //        return _activeContent;
        //    }
        //    internal set
        //    {
 
        //    }
        //}

        #region ActiveContent

        /// <summary>
        /// ActiveContent Dependency Property
        /// </summary>
        public static readonly DependencyProperty ActiveContentProperty =
            DependencyProperty.Register("ActiveContent", typeof(ManagedContent), typeof(DockingManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnActiveContentChanged),
                    new CoerceValueCallback(CoerceActiveContentValue)));

        /// <summary>
        /// Gets or sets the ActiveContent property.  This dependency property 
        /// indicates the active content.
        /// </summary>
        public ManagedContent ActiveContent
        {
            get { return (ManagedContent)GetValue(ActiveContentProperty); }
            set { SetValue(ActiveContentProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ActiveContent property.
        /// </summary>
        private static void OnActiveContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnActiveContentChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ActiveContent property.
        /// </summary>
        protected virtual void OnActiveContentChanged(DependencyPropertyChangedEventArgs e)
        {
            var cntActivated = e.NewValue as ManagedContent;
            var cntDeactivated = e.OldValue as ManagedContent;

            if (cntDeactivated != null)
                cntDeactivated.SetIsActiveContent(false);

            if (cntActivated != null)
                cntActivated.SetIsActiveContent(true);

            if (cntActivated != null &&
                cntActivated.ContainerPane is DocumentPane)
                ActiveDocument = cntActivated;
            
            //for backward compatibility
            NotifyPropertyChanged("ActiveContent");

            if (ActiveContentChanged != null)
                ActiveContentChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Coerces the ActiveContent value.
        /// </summary>
        private static object CoerceActiveContentValue(DependencyObject d, object value)
        {
            return value;
        }

        /// <summary>
        /// Raised whenever the <see cref="ActiveContent"/> changes
        /// </summary>
        public event EventHandler ActiveContentChanged;

        #endregion

        /// <summary>
        /// Gets the active dockable content
        /// </summary>
        /// <remarks>If no dockbale content us active at the moment returns null.</remarks>
        public DockableContent ActiveDockableContent
        {
            get
            {
                IInputElement focusedElement = FocusManager.GetFocusedElement(this);

                return focusedElement as DockableContent;
            }
        }

        #region DockableContents

        /// <summary>
        /// DockableContents Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey DockableContentsPropertyKey
            = DependencyProperty.RegisterReadOnly("DockableContents", typeof(ManagedContentCollection<DockableContent>), typeof(DockingManager),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DockableContentsProperty
            = DockableContentsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the DockableContents property.  This dependency property 
        /// retrives the collection of <see cref="DockableContent"/> that are bound to <see cref="DockingManager"/>
        /// </summary>
        public ManagedContentCollection<DockableContent> DockableContents
        {
            get { return (ManagedContentCollection<DockableContent>)GetValue(DockableContentsProperty); }
            protected set { SetValue(DockableContentsPropertyKey, value); }
        }
        #endregion


        #region Documents

        /// <summary>
        /// DockableContents Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey DocumentsPropertyKey
            = DependencyProperty.RegisterReadOnly("Documents", typeof(ManagedContentCollection<DocumentContent>), typeof(DockingManager),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DocumentsProperty
            = DocumentsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the DockableContents property.  This dependency property 
        /// retrives the collection of <see cref="DocumentContent"/> that are bound to <see cref="DockingManager"/>
        /// </summary>
        public ManagedContentCollection<DocumentContent> Documents
        {
            get { return (ManagedContentCollection<DocumentContent>)GetValue(DocumentsProperty); }
            protected set { SetValue(DocumentsPropertyKey, value); }
        }
        #endregion


        #region Documents Source

        /// <summary>
        /// Get or set the source collection for documents
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Bindable(true)]
        public IEnumerable DocumentsSource
        {
            get { return (IEnumerable)GetValue(DocumentsSourceProperty); }
            set { SetValue(DocumentsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DocumentsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DocumentsSourceProperty =
            DependencyProperty.Register("DocumentsSource", typeof(IEnumerable), typeof(DockingManager), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) => ((DockingManager)s).OnDocumentsSourceChanged(e.OldValue as IEnumerable, e.NewValue as IEnumerable))));

        void OnDocumentsSourceChanged(IEnumerable oldSource, IEnumerable newSource)
        {
            if (oldSource != null)
            {
                INotifyCollectionChanged oldSourceNotityIntf = oldSource as INotifyCollectionChanged;
                if (oldSourceNotityIntf != null)
                    oldSourceNotityIntf.CollectionChanged -= new NotifyCollectionChangedEventHandler(DocumentsSourceCollectionChanged);
            }

            if (newSource != null)
            {
                INotifyCollectionChanged newSourceNotityIntf = newSource as INotifyCollectionChanged;
                if (newSourceNotityIntf != null)
                    newSourceNotityIntf.CollectionChanged += new NotifyCollectionChangedEventHandler(DocumentsSourceCollectionChanged);
            }
        }

        void DocumentsSourceCollectionChanged(
            object sender, 
            NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                //close first documents that do not belong to the MainDocumentPane
                DocumentContent[] docs = this.Documents.ToArray();

                docs.Where(d => ((DocumentPane)d.Parent).IsMainDocumentPane.GetValueOrDefault()).ForEach(d => d.InternalClose());
                docs.Where(d => d.Parent != null && !((DocumentPane)d.Parent).IsMainDocumentPane.GetValueOrDefault()).ForEach(d => d.InternalClose());
            }

            if (e.OldItems != null &&
                (e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Replace))
            {
                foreach (object newDoc in e.OldItems)
                {
                    if (newDoc is DocumentContent)
                    {
                        DocumentContent documentToAdd = newDoc as DocumentContent;
                        documentToAdd.InternalClose();
                    }
                    else if (newDoc is FrameworkElement)
                    {
                        DocumentContent docContainer = ((FrameworkElement)newDoc).Parent as DocumentContent;
                        if (docContainer != null)
                            docContainer.InternalClose();
                    }
                }
            }

            if (e.NewItems != null &&
                (e.Action == NotifyCollectionChangedAction.Add ||
                e.Action == NotifyCollectionChangedAction.Replace))
            {
                if (MainDocumentPane == null)
                    throw new InvalidOperationException("DockingManager must have at least a DocumentPane to host documents");

                foreach (object newDoc in e.NewItems)
                {
                    if (newDoc is DocumentContent)
                    {
                        DocumentContent documentToAdd = newDoc as DocumentContent;
                        if (documentToAdd.Parent is DocumentPane)
                        {
                            ((DocumentPane)documentToAdd.Parent).Items.Clear();
                        }

                        MainDocumentPane.Items.Add(documentToAdd);
                    }
                    else if (newDoc is UIElement) //limit objects to be at least framework elements
                    {
                        DocumentContent documentToAdd = new DocumentContent()
                        {
                            Content = newDoc
                        };

                        MainDocumentPane.Items.Add(documentToAdd);
                    }
                    else
                        throw new InvalidOperationException(string.Format("Unable to add type {0} as DocumentContent", newDoc));
                }
            }

            RefreshContents();
        }

        internal void HandleDocumentClose(DocumentContent contentClosed)
        {
            IList listToUpdate = DocumentsSource as IList;
            if (listToUpdate != null)
                listToUpdate.Remove(contentClosed);
        }

        internal void HandleDocumentOpen(DocumentContent contentClosed)
        {
            IList listToUpdate = DocumentsSource as IList;
            if (listToUpdate != null)
                listToUpdate.Add(contentClosed);
        }
        #endregion      
        
        /// <summary>
        /// Returns the main document pane
        /// </summary>
        /// <param name="parentPanel"></param>
        /// <returns></returns>
        internal static DocumentPane GetMainDocumentPane(ResizingPanel parentPanel)
        {
            foreach (UIElement child in parentPanel.Children)
            {
                if (child is DocumentPane)
                    return child as DocumentPane;
               
                if (child is ResizingPanel)
                {
                    DocumentPane foundDocPane = GetMainDocumentPane(child as ResizingPanel);
                    if (foundDocPane != null)
                        return foundDocPane;
                }
            }

            return null;
        }

        internal static bool IsPanelContainingDocumentPane(ResizingPanel parentPanel)
        {
            foreach (UIElement child in parentPanel.Children)
            {
                if (child is DocumentPane)
                    return true;
                if (child is ResizingPanel)
                {
                    bool foundDocPane = IsPanelContainingDocumentPane(child as ResizingPanel);
                    if (foundDocPane)
                        return foundDocPane;
                }
            }

            return false;
        }

        internal void EnsurePanePositionIsValid(DocumentPane pane)
        {
            if (pane == MainDocumentPane)
                return;

            //A document pane must be at maindocument pane level or deeper
            if (MainDocumentPane.Parent == this)
            {
                throw new InvalidOperationException("A document pane can't be positioned at this level!");
            }
        }

        bool? FindPaneInPanel(ResizingPanel panel, Pane paneToFind)
        {
            foreach (UIElement child in panel.Children)
            {
                if (child == paneToFind)
                    return true;
                else if (child is DockablePane)
                    return null;
                else if (child is ResizingPanel)
                {
                    bool? found = FindPaneInPanel(child as ResizingPanel, paneToFind);
                    if (found.HasValue && found.Value)
                        return true;
                }
            }

            return false;
        }

        DocumentPane _mainDocumentPane;

        /// <summary>
        /// Gets the main <see cref="DocumentPane"/> that can be used to add new document
        /// </summary>
        public DocumentPane MainDocumentPane
        {
            get { return _mainDocumentPane; }
            set
            {
                if (_mainDocumentPane == null)
                {
                    _mainDocumentPane = value;

                    if (DocumentsSource != null)
                    {
                        foreach (object newDoc in DocumentsSource)
                        {
                            if (newDoc is DocumentContent)
                            {
                                MainDocumentPane.Items.Add(newDoc);
                            }
                            else if (newDoc is FrameworkElement) //limit objects to be at least framework elements
                            {
                                DocumentContent docContainer = new DocumentContent();
                                docContainer.Content = newDoc;

                                MainDocumentPane.Items.Add(docContainer);
                            }
                        }
                    }
                }
                else
                {
                    _mainDocumentPane = value;
                }

            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            
            UpdateLayout();
            RefreshContents();
        }

        bool _allowRefreshContents = true;

        internal void RefreshContents()
        {
            if (!_allowRefreshContents)
                return;

            var contentsFoundUnderMe = new LogicalTreeAdapter(this).Descendants<DependencyObject>().Where(d => d.Item is ManagedContent).Select(d => d.Item).Cast<ManagedContent>();
            var contentsFoundInFloatingMode = _floatingWindows.SelectMany(d => d.HostedPane.Items.Cast<ManagedContent>());
            DockableContent contentFoundInFlyoutMode = null;
            
            if (_flyoutWindow != null &&
                _flyoutWindow.ReferencedPane != null &&
                _flyoutWindow.ReferencedPane.Items.Count > 0)
            {
                contentFoundInFlyoutMode =  _flyoutWindow.ReferencedPane.Items[0] as DockableContent;
            }

            var contentsFound = new List<ManagedContent>();
            contentsFound.AddRange(contentsFoundUnderMe);
            contentsFound.AddRange(contentsFoundInFloatingMode);
            if (contentFoundInFlyoutMode !=  null)
                contentsFound.Add(contentFoundInFlyoutMode);

            var dockableContensToRemove = DockableContents.Except(contentsFound.OfType<DockableContent>());
            var dockableContensToAdd = contentsFound.OfType<DockableContent>().Except(DockableContents);

            dockableContensToAdd.ToArray().ForEach(d => 
            {
                if (d.State != DockableContentState.Hidden)
                    DockableContents.Add(d);
            });
            dockableContensToRemove.ToArray().ForEach(d =>
            {
                if (d.State != DockableContentState.Hidden)
                    DockableContents.Remove(d);
            });

            var documentsToRemove = Documents.Except(contentsFound.OfType<DocumentContent>());
            var documentsToAdd = contentsFound.OfType<DocumentContent>().Except(Documents);

            documentsToAdd.ToArray().ForEach(d => Documents.Add(d));
            documentsToRemove.ToArray().ForEach(d => Documents.Remove(d));

            //refresh MainDocumentPane
            if (MainDocumentPane == null ||
                MainDocumentPane.GetManager() != this)
            {
                ILinqToTree<DependencyObject> itemFound = new LogicalTreeAdapter(this).Descendants<DependencyObject>().FirstOrDefault(d => d.Item is DocumentPane);
                
                MainDocumentPane = itemFound != null ? itemFound.Item as DocumentPane : null;
            }

            //_floatingWindows.ForEach(fl => fl.CheckContents());
            CheckValidPanesFromTabGroups();
        }

        internal void ClearEmptyPanes()
        {
            if (RestoringLayout)
                return;

            while (true)
            {
                bool foundEmptyPaneToRemove = false;
                var emptyDockablePanes = new LogicalTreeAdapter(this).Descendants<DependencyObject>().Where(i => (i.Item is DockablePane) && (i.Item as DockablePane).Items.Count == 0).Select(i => i.Item).Cast<DockablePane>().ToArray();

                emptyDockablePanes.ForEach(dp =>
                {
                    if (!DockableContents.Any(dc => 
                        {
                            if (dc.SavedStateAndPosition != null &&
                                (dc.SavedStateAndPosition.ContainerPane == dp || dc.SavedStateAndPosition.ContainerPaneID == dp.ID))
                                return true;

                            if (dc.State == DockableContentState.AutoHide)
                            {
                                var flyoutDocPane = dc.ContainerPane as FlyoutDockablePane;
                                if (flyoutDocPane != null && flyoutDocPane.ReferencedPane == dp)
                                    return true;
                            }

                            return false;
                        }))
                    {
                        var containerPanel = dp.Parent as ResizingPanel;
                        if (containerPanel != null)
                        {
                            containerPanel.RemoveChild(dp);
                            foundEmptyPaneToRemove = true;
                        }
                    }
                });

                if (!foundEmptyPaneToRemove)
                    break;
            }

        }


        internal void ClearEmptyPanels(ResizingPanel panelToClear)
        {
            if (panelToClear == null)
                return;

            foreach (var childPanel in panelToClear.Children.OfType<ResizingPanel>().ToArray())
            {
                if (childPanel.Children.Count == 0)
                {
                    panelToClear.RemoveChild(childPanel);
                }
                else
                {
                    ClearEmptyPanels(childPanel);
                }
            }
        }

        /// <summary>
        /// This method ensure that content of this <see cref="DockingManager"/> is not empty
        /// </summary>
        void EnsureContentNotEmpty()
        {
            if (RestoringLayout)
                return;

            if (Content == null)
            {
                Content = new DocumentPane();
                RefreshContents();
            }
        }

        //internal List<T> FindContents<T>() where T : ManagedContent
        //{
        //    List<T> resList = new List<T>();

        //    if (Content is Pane)
        //    {
        //        foreach (ManagedContent c in ((Pane)Content).Items)
        //        {
        //            if (c is T)
        //            {
        //                resList.Add((T)c);
        //            }
        //        }
        //    }
        //    else if (Content is ResizingPanel)
        //    {
        //        FindContents<T>(resList, Content as ResizingPanel);
        //    }

        //    foreach (FloatingWindow flWindow in _floatingWindows)
        //    {
        //        foreach (ManagedContent c in flWindow.HostedPane.Items)
        //        {
        //            if (c is T)
        //                resList.Add(c as T);
        //        }
        //    }

        //    if (_flyoutWindow != null && _flyoutWindow.ReferencedPane != null)
        //    {
        //        foreach (ManagedContent c in _flyoutWindow.ReferencedPane.Items)
        //        {
        //            if (c is T)
        //                resList.Add(c as T);
        //        }
        //    }


        //    return resList;
        //}

        //void FindContents<T>(List<T> listOfFoundContents, ResizingPanel parentPanel) where T : ManagedContent
        //{
        //    foreach (UIElement child in parentPanel.Children)
        //    {
        //        if (child is Pane)
        //        {
        //            foreach (ManagedContent c in ((Pane)child).Items)
        //            {
        //                if (c is T)
        //                {
        //                    listOfFoundContents.Add((T)c);
        //                }
        //            }
        //        }
        //        else if (child is ResizingPanel)
        //        {
        //            FindContents<T>(listOfFoundContents, child as ResizingPanel);
        //        }
        //    }
        //}

        #endregion

        #region Floating windows management
        List<FloatingWindow> _floatingWindows = new List<FloatingWindow>();

        /// <summary>
        /// Get all floating windows created by the <see cref="DockingManager"/> while user dragged contents
        /// </summary>
        public FloatingWindow[] FloatingWindows
        {
            get 
            {
                if (_floatingWindows == null ||
                    _floatingWindows.Count == 0)
                    return new DockableFloatingWindow[] { };

                return _floatingWindows.ToArray();
            }
        }

        internal void RegisterFloatingWindow(FloatingWindow floatingWindow)
        {
            if (_floatingWindows != null)
            {
                floatingWindow.FlowDirection = this.FlowDirection;
                if (!_floatingWindows.Contains(floatingWindow))
                    _floatingWindows.Add(floatingWindow);
            }
        }

        internal void UnregisterFloatingWindow(FloatingWindow floatingWindow)
        {
            if (_floatingWindows != null)
                _floatingWindows.Remove(floatingWindow);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.CommandBindings.Clear();

            //Keyboard.AddKeyDownHandler(this, (s, ke) =>
            //    {
            //        if (ke.Key == Key.Tab && Keyboard.IsKeyDown(Key.LeftCtrl))
            //        {
            //            ShowNavigatorWindow();
            //            ke.Handled = true;
            //        }
            //    });

            //Keyboard.AddKeyDownHandler(this, (s, ke) =>
            //{
            //    if (ke.Key == Key.Tab)
            //    {
            //        if (navigatorWindow != null && navigatorWindow.IsVisible)
            //        {
            //            HideNavigatorWindow();
            //            ke.Handled = true;
            //        }
            //    }
            //});

            //this.CommandBindings.Add(new CommandBinding(ShowNavigatorWindowCommand, OnExecuteCommand, OnCanExecuteCommand));
            //this.CommandBindings.Add(new CommandBinding(ShowDocumentNavigatorWindowCommand, OnExecuteCommand, OnCanExecuteCommand));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == FlowDirectionProperty)
            {
                _floatingWindows.ForEach(fl =>
                    {
                        fl.FlowDirection = FlowDirection;
                    });

                if (_flyoutWindow != null)
                {
                    _flyoutWindow.FlowDirection = FlowDirection;
                }
            }

            base.OnPropertyChanged(e);
        }
        #endregion
     
        NavigatorWindow navigatorWindow = null;

        void ShowNavigatorWindow()
        {
            HideNavigatorWindow();

            if (navigatorWindow == null)
            {
                navigatorWindow = new NavigatorWindow(this);
                navigatorWindow.Owner = Window.GetWindow(this);
            }

            Point locDockingManager = this.PointToScreenDPI(new Point());
            navigatorWindow.Left = locDockingManager.X;
            navigatorWindow.Top = locDockingManager.Y;
            navigatorWindow.Width = this.ActualWidth;
            navigatorWindow.Height = this.ActualHeight;
            navigatorWindow.ShowActivated = false;
            navigatorWindow.Show();
            //navigatorWindow.Focus();
        }

        void HideNavigatorWindow()
        {
            if (navigatorWindow != null)
            {
                navigatorWindow.Close();
                navigatorWindow = null;
            }
        }

        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // accept Control or Control+Shift
            bool isCtrlDown = (Keyboard.Modifiers & ~ModifierKeys.Shift) == ModifierKeys.Control;
            bool _navigatorWindowIsVisible = navigatorWindow != null ? navigatorWindow.IsVisible : false;
            Debug.WriteLine(string.Format("OnKeyDn {0} CtrlDn={1}", e.Key, isCtrlDown));

            if (e.Key == Key.Tab && isCtrlDown)
            {
                if (!_navigatorWindowIsVisible)
                {
                    ShowNavigatorWindow();
                }
                
                e.Handled = true;
            }
            else if (NavigatorWindow.IsKeyHandled(e.Key))
            {
                HideNavigatorWindow();
            }


            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            // accept Control or Control+Shift
            bool isCtrlDown = (Keyboard.Modifiers & ~ModifierKeys.Shift) == ModifierKeys.Control;
            bool _navigatorWindowIsVisible = navigatorWindow != null ? navigatorWindow.IsVisible : false;
            Debug.WriteLine(string.Format("OnKeyUp {0} CtrlDn={1}", e.Key, isCtrlDown));

            if (NavigatorWindow.IsKeyHandled(e.Key) && isCtrlDown)
            {
                if (!_navigatorWindowIsVisible && e.Key == Key.Tab)
                {
                    ShowNavigatorWindow();
                    _navigatorWindowIsVisible = true;
                }

                if (_navigatorWindowIsVisible)
                    e.Handled = navigatorWindow.HandleKey(e.Key); 
            }
            else
            {
                if (_navigatorWindowIsVisible)
                {
                    if (navigatorWindow.Documents.CurrentItem != null)
                    {
                        var docSelected = (navigatorWindow.Documents.CurrentItem as NavigatorWindowDocumentItem).ItemContent as DocumentContent;
                        docSelected.Activate();
                    }
                    else if (navigatorWindow.DockableContents.CurrentItem != null)
                    {
                        var cntSelected = (navigatorWindow.DockableContents.CurrentItem as NavigatorWindowItem).ItemContent as DockableContent;
                        cntSelected.Activate();
                    }

                    HideNavigatorWindow();
                }

            }

            base.OnKeyUp(e);
        }

        #region DockablePane operations
        /// <summary>
        /// Anchor a dockable pane to a border
        /// </summary>
        /// <param name="paneToAnchor"></param>
        /// <param name="anchor"></param>
        public void Anchor(DockablePane paneToAnchor, AnchorStyle anchor)
        {
            //ensure that content property is not empty
            EnsureContentNotEmpty();

            if (Content == null)
                return;

            //remove the pane from its original children collection
            FrameworkElement parentElement = paneToAnchor.Parent as FrameworkElement;

            if (anchor == AnchorStyle.None)
                anchor = AnchorStyle.Right;

            //Change anchor border according to FlowDirection
            if (FlowDirection == FlowDirection.RightToLeft)
            {
                if (anchor == AnchorStyle.Right)
                    anchor = AnchorStyle.Left;
                else if (anchor == AnchorStyle.Left)
                    anchor = AnchorStyle.Right;
            }

            //parentElement should be a DockingManager or a ResizingPanel
            if (parentElement is ContentControl)
            {
                ((ContentControl)parentElement).Content = null;
            }

            //and insert in the top level panel if exist
            ResizingPanel toplevelPanel = Content as ResizingPanel;

            if (toplevelPanel != null && toplevelPanel.Children.Count == 0)
            {
                Content = null;
                toplevelPanel = null;
            }

            Orientation requestedOrientation =
                (anchor == AnchorStyle.Bottom || anchor == AnchorStyle.Top) ? Orientation.Vertical : Orientation.Horizontal;

            //if toplevel panel contains only one child then just override the orientation
            //as requested
            if (toplevelPanel != null && toplevelPanel.Children.Count == 1)
                toplevelPanel.Orientation = requestedOrientation;

            if (toplevelPanel == null ||
                toplevelPanel.Orientation != requestedOrientation)
            {
                //if toplevel panel doesn't exist or it has not the correct orientation
                //we have to create a new one and set it as content of docking manager
                toplevelPanel = new ResizingPanel();
                toplevelPanel.Orientation = requestedOrientation;

                FrameworkElement contentElement = Content as FrameworkElement;
                
                _allowRefreshContents = false;
                Content = null;

                if (anchor == AnchorStyle.Left ||
                    anchor == AnchorStyle.Top)
                {
                    toplevelPanel.Children.Add(paneToAnchor);
                    toplevelPanel.InsertChildRelativeTo(contentElement, paneToAnchor, true);
                }
                else
                {
                    toplevelPanel.Children.Add(paneToAnchor);
                    toplevelPanel.InsertChildRelativeTo(contentElement, paneToAnchor, false);
                }

                _allowRefreshContents = true;
                Content = toplevelPanel;
            }
            else
            {
                
                //here we have a docking manager content with the right orientation
                //so we have only to insert new child at correct position
                if (anchor == AnchorStyle.Left ||
                    anchor == AnchorStyle.Top)
                {
                    //add new child before first one (prepend)
                    toplevelPanel.InsertChildRelativeTo(paneToAnchor, toplevelPanel.Children[0] as FrameworkElement, false);
                }
                else
                {
                    //add new child after last one (append)
                    toplevelPanel.InsertChildRelativeTo(paneToAnchor, toplevelPanel.Children[toplevelPanel.Children.Count - 1] as FrameworkElement, true);
                }
            }

            //Refresh anchor style
            DockablePane paneToAnchorAsDockablePane = paneToAnchor as DockablePane;

            if (paneToAnchorAsDockablePane != null)
            {
                paneToAnchorAsDockablePane.Anchor = anchor;
            }



            if (anchor == AnchorStyle.Left ||
                anchor == AnchorStyle.Right)
            {
                double w = Math.Min(
                    ActualWidth / 2.0,
                    ResizingPanel.GetEffectiveSize(paneToAnchor).Width);
                ResizingPanel.SetResizeWidth(paneToAnchor, new GridLength(w, GridUnitType.Pixel));
                ResizingPanel.SetResizeHeight(paneToAnchor, new GridLength(1.0, GridUnitType.Star));
            }
            else
            {
                double h = Math.Min(
                    ActualHeight / 2.0,
                    ResizingPanel.GetEffectiveSize(paneToAnchor).Height);
                ResizingPanel.SetResizeWidth(paneToAnchor, new GridLength(1.0, GridUnitType.Star));
                ResizingPanel.SetResizeHeight(paneToAnchor, new GridLength(h, GridUnitType.Pixel));
            }
            
            //refresh contents state
            paneToAnchor.Items.OfType<DockableContent>().ForEach(dc =>
                {
                    dc.SetStateToDock();
                });          
            
            
            //paneToAnchor.Focus();
            toplevelPanel.InvalidateMeasure();
        }

        /// <summary>
        /// Anchor a pane (<see cref="DockablePane"/> and <see cref="DocumentPane"/>) to a border of a another pane
        /// </summary>
        /// <param name="paneToAnchor">Pane to anchor</param>
        /// <param name="relativePane">Pane relative</param>
        /// <param name="anchor">Position relative to the target pane</param>
        public void Anchor(Pane paneToAnchor, Pane relativePane, AnchorStyle anchor)
        {
            //ensure that content property is not empty
            EnsureContentNotEmpty(); 
            
            if (anchor == AnchorStyle.None)
                anchor = AnchorStyle.Right;

            //Change anchor border according to FlowDirection
            if (FlowDirection == FlowDirection.RightToLeft)
            {
                if (anchor == AnchorStyle.Right)
                    anchor = AnchorStyle.Left;
                else if (anchor == AnchorStyle.Left)
                    anchor = AnchorStyle.Right;
            }

            if (paneToAnchor is DockablePane &&
                relativePane is DockablePane)
                Anchor(paneToAnchor as DockablePane, relativePane as DockablePane, anchor);
            else if (paneToAnchor is DockablePane &&
                relativePane is DocumentPane)
                Anchor(paneToAnchor as DockablePane, relativePane as DocumentPane, anchor);
            else if (paneToAnchor is DocumentPane &&
                relativePane is DocumentPane)
                Anchor(paneToAnchor as DocumentPane, relativePane as DocumentPane, anchor);
            else
                throw new InvalidOperationException();

            CheckForSingleChildPanels();
        }


        /// <summary>
        /// Anchor a dockable pane (<see cref="DockablePane"/>) to a border of a document pane
        /// </summary>
        /// <param name="paneToAnchor">Pane to anchor</param>
        /// <param name="relativePane">Pane relative</param>
        /// <param name="anchor"></param>
        public void Anchor(DockablePane paneToAnchor, DocumentPane relativePane, AnchorStyle anchor)
        {
            //ensure that content property is not empty
            EnsureContentNotEmpty(); 
            
            if (anchor == AnchorStyle.None)
                anchor = AnchorStyle.Right;

            //get a reference to the resizingpanel container of relativePane
            ResizingPanel relativePaneContainer = LogicalTreeHelper.GetParent(relativePane) as ResizingPanel;
            DocumentPaneResizingPanel relativeDocumentPaneContainer = relativePane.GetParentDocumentPaneResizingPanel();
            Orientation requestedOrientation =
                (anchor == AnchorStyle.Bottom || anchor == AnchorStyle.Top) ? Orientation.Vertical : Orientation.Horizontal;

            if (relativePaneContainer == null)
            {
                Debug.Assert(relativePane.Parent == this);

                this.Content = null;

                relativeDocumentPaneContainer = new DocumentPaneResizingPanel();
                relativeDocumentPaneContainer.Children.Add(relativePane);

                relativePaneContainer = new ResizingPanel();
                relativePaneContainer.Orientation = requestedOrientation;

                this.Content = relativePaneContainer;

                relativePaneContainer.Children.Add(relativeDocumentPaneContainer);
            }

            if (relativeDocumentPaneContainer == null)
            {
                relativeDocumentPaneContainer = new DocumentPaneResizingPanel();
                relativeDocumentPaneContainer.Orientation = requestedOrientation;


                int indexOfPaneToReplace = relativePaneContainer.Children.IndexOf(relativePane);
                relativePaneContainer.Children.RemoveAt(indexOfPaneToReplace);

                relativeDocumentPaneContainer.Children.Add(relativePane);

                relativePaneContainer.Children.Insert(indexOfPaneToReplace, relativeDocumentPaneContainer);
            }

            relativePaneContainer = LogicalTreeHelper.GetParent(relativeDocumentPaneContainer) as ResizingPanel;

            //Debug.Assert(relativePaneContainer is DocumentPaneResizingPanel, "By now we can't have a pane without a resizing panel containing it");
            if (relativePaneContainer == null)
            {
                Debug.Assert(relativeDocumentPaneContainer.Parent == this);

                this.Content = null;

                relativePaneContainer = new ResizingPanel();
                relativePaneContainer.Orientation = requestedOrientation;

                this.Content = relativePaneContainer;

                relativePaneContainer.Children.Add(relativeDocumentPaneContainer);
            }

            #region Create and setup container panel
            if (relativePaneContainer != null)
            {
                //check if orientation is right
                if (relativePaneContainer.Orientation != requestedOrientation)
                {
                    //if the existing panel is not oriented as we want
                    //create a new one
                    ResizingPanel newPanel = new ResizingPanel();
                    newPanel.Orientation = requestedOrientation;


                    if (newPanel.Orientation == Orientation.Horizontal)
                        ResizingPanel.SetResizeHeight(newPanel, ResizingPanel.GetResizeHeight(relativePane));
                    else
                        ResizingPanel.SetResizeWidth(newPanel, ResizingPanel.GetResizeWidth(relativePane));


                    //replace relative pane in its' container panel
                    //with this new panel
                    int indexofRelativePane = relativePaneContainer.Children.IndexOf(relativeDocumentPaneContainer);
                    relativePaneContainer.Children.Remove(relativeDocumentPaneContainer);
                    relativePaneContainer.Children.Insert(indexofRelativePane, newPanel);

                    //now we have a panel correctly placed in the tree
                    newPanel.Children.Add(relativeDocumentPaneContainer);

                    //use InsertChildRelativeTo function to add a resizingsplitter between
                    //the two children
                    newPanel.InsertChildRelativeTo(
                        paneToAnchor, relativeDocumentPaneContainer, anchor == AnchorStyle.Right || anchor == AnchorStyle.Bottom);
                }
                else
                {

                    if (anchor == AnchorStyle.Left ||
                        anchor == AnchorStyle.Top)
                    {
                        //add new child before first (prepend)
                        relativePaneContainer.InsertChildRelativeTo(paneToAnchor,
                            relativeDocumentPaneContainer, false);
                    }
                    else
                    {
                        //add new child after last (append)
                        relativePaneContainer.InsertChildRelativeTo(paneToAnchor,
                            relativeDocumentPaneContainer, true);
                    }
                }

            }

            relativePaneContainer.InvalidateMeasure();
            #endregion



            if (anchor == AnchorStyle.Left ||
                anchor == AnchorStyle.Right)
            {
                double w = Math.Min(
                    ResizingPanel.GetEffectiveSize(relativePane).Width / 2.0,
                    ResizingPanel.GetEffectiveSize(paneToAnchor).Width);
                ResizingPanel.SetResizeWidth(paneToAnchor, new GridLength(w, GridUnitType.Pixel));
                ResizingPanel.SetResizeHeight(paneToAnchor, new GridLength(1.0, GridUnitType.Star));
            }
            else
            {
                double h = Math.Min(
                    ResizingPanel.GetEffectiveSize(relativePane).Height / 2.0,
                    ResizingPanel.GetEffectiveSize(paneToAnchor).Height);
                ResizingPanel.SetResizeWidth(paneToAnchor, new GridLength(1.0, GridUnitType.Star));
                ResizingPanel.SetResizeHeight(paneToAnchor, new GridLength(h, GridUnitType.Pixel));
            }

            //refresh contents state
            foreach (DockableContent draggedContent in paneToAnchor.Items)
            {
                draggedContent.SetStateToDock();
            }


            //than set the new anchor style for the pane
            paneToAnchor.Anchor = anchor;
            //paneToAnchor.Focus();
        }

        /// <summary>
        /// Anchor a document pane (<see cref="DocumentPane"/>) to a border of an other document pane
        /// </summary>
        /// <param name="paneToAnchor">Pane to anchor</param>
        /// <param name="relativePane">Pane relative</param>
        /// <param name="anchor"></param>
        public void Anchor(DocumentPane paneToAnchor, DocumentPane relativePane, AnchorStyle anchor)
        {
            //ensure that content property is not empty
            EnsureContentNotEmpty();
            
            if (anchor == AnchorStyle.None)
                anchor = AnchorStyle.Right;

            //get a reference to the resizingpanel container of relativePane
            ResizingPanel relativePaneContainer = LogicalTreeHelper.GetParent(relativePane) as ResizingPanel;
            DocumentPaneResizingPanel relativeDocumentPaneContainer = relativePane.GetParentDocumentPaneResizingPanel();
            Orientation requestedOrientation =
                (anchor == AnchorStyle.Bottom || anchor == AnchorStyle.Top) ? Orientation.Vertical : Orientation.Horizontal;

            if (relativePaneContainer == null)
            {
                Debug.Assert(relativePane.Parent == this);

                
                relativeDocumentPaneContainer = new DocumentPaneResizingPanel();
                relativePaneContainer = relativeDocumentPaneContainer;

                relativeDocumentPaneContainer.Orientation = requestedOrientation;
                this.Content = relativePaneContainer;

                relativePaneContainer.Children.Add(relativePane);

            }

            if (relativeDocumentPaneContainer == null)
            {
                relativeDocumentPaneContainer = new DocumentPaneResizingPanel();
                relativeDocumentPaneContainer.Orientation = requestedOrientation;
                
                
                int indexOfPaneToReplace = relativePaneContainer.Children.IndexOf(relativePane);
                relativePaneContainer.Children.RemoveAt(indexOfPaneToReplace);

                relativeDocumentPaneContainer.Children.Add(relativePane);

                relativePaneContainer.Children.Insert(indexOfPaneToReplace, relativeDocumentPaneContainer);

                relativePaneContainer = relativeDocumentPaneContainer;
            }  

            Debug.Assert(relativePaneContainer != null, "By now we can't have a pane without a resizing panel containing it");

            #region Create and setup container panel
            if (relativePaneContainer != null)
            {
                //check if orientation is right
                if (relativePaneContainer.Orientation != requestedOrientation)
                {
                    //if the existing panel is not oriented as we want
                    //create a new one
                    DocumentPaneResizingPanel newPanel = new DocumentPaneResizingPanel();
                    newPanel.Orientation = requestedOrientation;

                    //replace relative pane in its' container panel
                    //with this new panel
                    int indexofRelativePane = relativePaneContainer.Children.IndexOf(relativePane);
                    relativePaneContainer.Children.Remove(relativePane);
                    relativePaneContainer.Children.Insert(indexofRelativePane, newPanel);

                    //now we have a panel correctly placed in the tree
                    newPanel.Children.Add(relativePane);

                    //use InsertChildRelativeTo function to add a resizingsplitter between
                    //the two children
                    newPanel.InsertChildRelativeTo(
                        paneToAnchor, relativePane, anchor == AnchorStyle.Right || anchor == AnchorStyle.Bottom);

                    relativePaneContainer = newPanel;
                }
                else
                {
                    if (anchor == AnchorStyle.Left ||
                        anchor == AnchorStyle.Top)
                    {
                        //add new child before first (prepend)
                        relativePaneContainer.InsertChildRelativeTo(paneToAnchor,
                            relativePane, false);
                    }
                    else
                    {
                        //add new child after last (append)
                        relativePaneContainer.InsertChildRelativeTo(paneToAnchor,
                            relativePane, true);
                    }

                    //if (relativePaneContainer.Orientation == Orientation.Horizontal)
                    //{
                    //    Size desideredSize = ResizingPanel.GetEffectiveSize(paneToAnchor);
                    //    double approxStarForNewPane = desideredSize.Width / relativePaneContainer.ActualWidth;
                    //    approxStarForNewPane = Math.Min(approxStarForNewPane, 1.0);
                    //    paneToAnchor.SetValue(ResizingPanel.ResizeWidthProperty, new GridLength(approxStarForNewPane, GridUnitType.Star));
                    //}
                }

                relativePaneContainer.InvalidateMeasure();
            }
            #endregion

            //paneToAnchor.Focus();

            //(paneToAnchor.SelectedItem as ManagedContent).Activate();
            //if (paneToAnchor.SelectedItem is DocumentContent)
            //    ActiveDocument = paneToAnchor.SelectedItem as DocumentContent;

            paneToAnchor.SelectedIndex = 0;
        }

        /// <summary>
        /// Anchor a dockable pane (<see cref="DockablePane"/>) to a border of an other dockable pane
        /// </summary>
        /// <param name="paneToAnchor">Pane to anchor</param>
        /// <param name="relativePane">Pane relative</param>
        /// <param name="anchor"></param>
        public void Anchor(DockablePane paneToAnchor, DockablePane relativePane, AnchorStyle anchor)
        {
            //ensure that content property is not empty
            EnsureContentNotEmpty();
            
            if (anchor == AnchorStyle.None)
                anchor = AnchorStyle.Right;

            //get a refernce to the resizingpanel container of relativePane
            ResizingPanel relativePaneContainer = LogicalTreeHelper.GetParent(relativePane) as ResizingPanel;
            Orientation requestedOrientation =
                (anchor == AnchorStyle.Bottom || anchor == AnchorStyle.Top) ? Orientation.Vertical : Orientation.Horizontal;

            if (relativePaneContainer == null)
            {
                Debug.Assert(relativePane.Parent == this);

                relativePaneContainer = new ResizingPanel();
                relativePaneContainer.Orientation = requestedOrientation;
                this.Content = relativePaneContainer;
                relativePaneContainer.Children.Add(relativePane);
            }

            Debug.Assert(relativePaneContainer != null, "By now we can't have a pane without a resizing panel containing it");

            #region Create and setup container panel
            if (relativePaneContainer != null)
            {
                //check if orientation is right
                if (relativePaneContainer.Orientation != requestedOrientation)
                {
                    //if the existing panel is not oriented as we want
                    //create a new one
                    ResizingPanel newPanel = new ResizingPanel();
                    newPanel.Orientation = requestedOrientation;

                    if (newPanel.Orientation == Orientation.Horizontal)
                        ResizingPanel.SetResizeHeight(newPanel, ResizingPanel.GetResizeHeight(relativePane));
                    else
                        ResizingPanel.SetResizeWidth(newPanel, ResizingPanel.GetResizeWidth(relativePane));

                    //replace relative pane in its' container panel
                    //with this new panel
                    int indexofRelativePane = relativePaneContainer.Children.IndexOf(relativePane);
                    relativePaneContainer.Children.Remove(relativePane);
                    relativePaneContainer.Children.Insert(indexofRelativePane, newPanel);

                    //now we have a panel correctly placed in the tree
                    newPanel.Children.Add(relativePane);
                    newPanel.InsertChildRelativeTo(
                        paneToAnchor, relativePane, anchor == AnchorStyle.Right || anchor == AnchorStyle.Bottom);

                    relativePaneContainer = newPanel;
                }
                else
                {
                    if (anchor == AnchorStyle.Left ||
                        anchor == AnchorStyle.Top)
                    {
                        //add new child before first (prepend)
                        relativePaneContainer.InsertChildRelativeTo(paneToAnchor,
                            relativePane, false);
                    }
                    else
                    {
                        //add new child after last (append)
                        relativePaneContainer.InsertChildRelativeTo(paneToAnchor,
                            relativePane, true);
                    }
                }


                relativePaneContainer.InvalidateMeasure();
            } 
            #endregion

            //than set the new anchor style for the pane
            paneToAnchor.Anchor = relativePane.Anchor;

            if (anchor == AnchorStyle.Left ||
                anchor == AnchorStyle.Right)
            {
                double w = Math.Min(
                    ResizingPanel.GetEffectiveSize(relativePane).Width / 2.0,
                    ResizingPanel.GetEffectiveSize(paneToAnchor).Width);
                ResizingPanel.SetResizeWidth(paneToAnchor, new GridLength(w, GridUnitType.Pixel));
                ResizingPanel.SetResizeHeight(paneToAnchor, new GridLength(1.0, GridUnitType.Star));
            }
            else
            {
                double h = Math.Min(
                    ResizingPanel.GetEffectiveSize(relativePane).Height / 2.0,
                    ResizingPanel.GetEffectiveSize(paneToAnchor).Height);
                ResizingPanel.SetResizeWidth(paneToAnchor, new GridLength(1.0, GridUnitType.Star));
                ResizingPanel.SetResizeHeight(paneToAnchor, new GridLength(h, GridUnitType.Pixel));
            }

            //refresh contents state
            foreach (DockableContent draggedContent in paneToAnchor.Items)
            {
                draggedContent.SetStateToDock();
            }

            if (relativePaneContainer != null)
                relativePaneContainer.AdjustPanelSizes();

            //paneToAnchor.Focus();
        }

        #region DropInto methods
        internal void DropInto(Pane paneDragged, Pane paneToDropInto)
        {
            if (paneDragged is DockablePane &&
                paneToDropInto is DockablePane)
                DropInto(paneDragged as DockablePane, paneToDropInto as DockablePane);
            else if (paneDragged is DockablePane &&
                paneToDropInto is DocumentPane)
                DropInto(paneDragged as DockablePane, paneToDropInto as DocumentPane);
            else if (paneDragged is DocumentPane &&
                paneToDropInto is DocumentPane)
                DropInto(paneDragged as DocumentPane, paneToDropInto as DocumentPane);
            else
                throw new InvalidOperationException();
        }
        internal void DropInto(DocumentPane paneDragged, DocumentPane paneToDropInto)
        {
            //transfer tha contents of dragged pane (conatined in a FloatingWindow)
            //to the pane which user select
            //ManagedContent contentToFocus = null;
            while (paneDragged.Items.Count > 0)
            {
                var contentToTransfer = paneDragged.RemoveContent(0);
                paneToDropInto.Items.Insert(0, contentToTransfer);
                //contentToFocus = contentToTransfer;
                contentToTransfer.Activate();
            }

     
            //paneToDropInto.SelectedIndex = 0;
            //paneToDropInto.Focus();
            //if (contentToFocus != null)
            //    contentToFocus.Activate();
        }
        internal void DropInto(DockablePane paneDragged, DocumentPane paneToDropInto)
        {
            //if (paneToDropInto != MainDocumentPane)
            //    paneToDropInto = MainDocumentPane;

            //transfer contents of dragged pane (contained in a FloatingWindow)
            //to the pane which user select, taking care of setting contents state
            //to Dock (using Dock() method of class DockablePane).
            while (paneDragged.Items.Count > 0)
            {
                var contentToTransfer = paneDragged.RemoveContent(0);
                paneToDropInto.Items.Add(contentToTransfer);


                var dockContentToTransfer = contentToTransfer as DockableContent;

                if (dockContentToTransfer != null)
                    dockContentToTransfer.SetStateToDocument();
                
                contentToTransfer.Activate();
            }

            //paneToDropInto.SelectedIndex = paneToDropInto.Items.Count - 1;
            //paneToDropInto.Focus();
        }
        internal void DropInto(DockablePane paneDragged, DockablePane paneToDropInto)
        {
            //transfer tha contents of dragged pane (conatined in a FloatingWindow)
            //to the pane which user select, taking care of setting contents state
            //to Dock (using Dock() method of class DockablePane).
            while (paneDragged.Items.Count > 0)
            {
                ManagedContent contentToTransfer = paneDragged.RemoveContent(0);
                paneToDropInto.Items.Add(contentToTransfer);


                DockableContent dockContentToTransfer = contentToTransfer as DockableContent;

                if (dockContentToTransfer != null)
                    dockContentToTransfer.SetStateToDock();
            }


            paneToDropInto.SelectedIndex = paneToDropInto.Items.Count - 1;
            //paneToDropInto.Focus();
        }
        #endregion

        bool RemoveContentFromTabGroup(DockableContent contentToRemove)
        {
            foreach (Panel anchorTabPanel in _anchorTabPanels)
            {
                foreach (DockablePaneAnchorTabGroup group in anchorTabPanel.Children)
                {
                    foreach (DockablePaneAnchorTab tab in group.Children)
                    {
                        if (tab.ReferencedContent == contentToRemove)
                        {
                            group.Children.Remove(tab);
                            if (group.Children.Count == 0)
                                anchorTabPanel.Children.Remove(group);

                            return true;
                        }
                    }
                }
            }

            return false;            
        }

        /// <summary>
        /// Remove a pane from border tab groups
        /// </summary>
        /// <param name="pane">Pane to remove</param>
        /// <returns>True if pane was removed, false otherwise</returns>
        bool RemovePaneFromTabGroups(DockablePane paneToRemove)
        {
            foreach (Panel anchorTabPanel in _anchorTabPanels)
            {
                foreach (DockablePaneAnchorTabGroup group in anchorTabPanel.Children)
                {
                    if (group.ReferencedPane == paneToRemove)
                    {
                        anchorTabPanel.Children.Remove(group);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Check if only vali panes are referenced by anchor tabs
        /// </summary>
        void CheckValidPanesFromTabGroups()
        {
            if (RestoringLayout)
                return;

            foreach (var anchorTabPanel in _anchorTabPanels)
            {
                foreach (var group in anchorTabPanel.Children.Cast<DockablePaneAnchorTabGroup>().ToArray())
                {
                    if (group.ReferencedPane.GetManager() != this)
                    {
                        anchorTabPanel.Children.Remove(group);
                    }
                }
            }

            if (_flyoutWindow != null && 
                _flyoutWindow.ReferencedPane != null &&
                _flyoutWindow.ReferencedPane.Items.Count == 1)
            {
                var cntFlyingOut = _flyoutWindow.ReferencedPane.Items[0] as ManagedContent;
                if (((FlyoutDockablePane)cntFlyingOut.ContainerPane).ReferencedPane.GetManager() != this)
                    HideFlyoutWindow();
            }
        }

        [Conditional("DEBUG")]
        void CheckForSingleChildPanels()
        {
            //Debug.Assert(!
            //new LogicalTreeAdapter(this).Descendants<DependencyObject>().Any(
            //    di => di.Item is ResizingPanel && ((ResizingPanel)di.Item).Children.Count == 1)
            //);
        }

        /// <summary>
        /// Autohides/redock a dockable pane
        /// </summary>
        /// <param name="pane">Pane to auto hide/redock</param>
        internal void ToggleAutoHide(DockablePane pane)
        {
            if (!_OnApplyTemplateFlag)
            {
                Debug.WriteLine("Layout has been restored before creating DockingManager object: force WPF to apply the template..."); 
                ApplyTemplate();
            }


            //if pane is in auto hide state then is found 
            //referenced by a DockablePaneAnchorTabGroup
            //if so redock it in its original position
            if (RemovePaneFromTabGroups(pane))
            {
                #region Pane is present in tab anchor panels
                DockableContent selectedContent = 
                    _flyoutWindow != null &&
                    _flyoutWindow.ReferencedPane != null &&
                    _flyoutWindow.ReferencedPane.Items.Count > 0 ? _flyoutWindow.ReferencedPane.Items[0] as DockableContent : 
                    pane.Items[0] as DockableContent;

                HideFlyoutWindow();

                ResizingPanel parentPanel = pane.Parent as ResizingPanel;
                if (parentPanel != null && parentPanel.Children.Count >= 3)
                {
                    parentPanel.AdjustPanelSizes();
                }

                //reset content state to docked
                foreach (DockableContent content in pane.Items)
                {
                    content.SetStateToDock();
                }

                pane.Focus();
                pane.SelectedItem = selectedContent;
                ActiveContent = selectedContent; 
                #endregion
            }
            else
            {
                #region Pane is not auto hidden
                //Create e new group
                DockablePaneAnchorTabGroup group = new DockablePaneAnchorTabGroup();

                //associate it to the pane
                group.ReferencedPane = pane;

                DockableContent selectedContent = pane.SelectedItem as DockableContent;

                //add contents to it
                foreach (DockableContent content in pane.Items)
                {
                    DockablePaneAnchorTab tab = new DockablePaneAnchorTab();
                    tab.ReferencedContent = content;
                    //tab.Anchor = pane.Anchor;
                    //tab.Icon = content.Icon;


                    group.Children.Add(tab);
                    content.SetStateToAutoHide();
                }

                //place group under correct anchor tabpanel
                switch (pane.Anchor)
                {
                    case AnchorStyle.Left:
                        if (_leftAnchorTabPanel != null)
                            _leftAnchorTabPanel.Children.Add(group);
                        break;
                    case AnchorStyle.Top:
                        if (_topAnchorTabPanel != null)
                            _topAnchorTabPanel.Children.Add(group);
                        break;
                    case AnchorStyle.Right:
                        if (_rightAnchorTabPanel != null)
                            _rightAnchorTabPanel.Children.Add(group);
                        break;
                    case AnchorStyle.Bottom:
                        if (_bottomAnchorTabPanel != null)
                            _bottomAnchorTabPanel.Children.Add(group);
                        break;
                }
                
                #endregion
            }

            //refresh arrangements traversing bottom-up visual tree
            FrameworkElement parentElement = pane.Parent as FrameworkElement;

            // Daniel Grunwald 2010/12/19: stop at 'this' to fix SD-1786
            while (parentElement != null && parentElement != this)
            {
                parentElement.InvalidateMeasure();
                parentElement = parentElement.Parent as FrameworkElement;
            }


        }
        
        #endregion
        
        #region Hide/Show contents
        /// <summary>
        /// Hide a dockable content removing it from its container <see cref="Pane"/>
        /// </summary>
        /// <param name="content">Content to hide</param>
        /// <remarks>Note that if you simply remove a content from its container without calling this method, the
        /// layout serializer component can't managed correctly the removed content.</remarks>
        internal void Hide(DockableContent content)
        {
            if (content.State == DockableContentState.Hidden)
            {
                DockableContents.Add(content);
                return;
            }

            if (!content.IsCloseable)
                return;

            if (content.State != DockableContentState.FloatingWindow &&
                content.State != DockableContentState.DockableWindow)
            {
                //save position only if hiding from a docked or autohidden pane
                content.SaveCurrentStateAndPosition();
            }

            if (content.State == DockableContentState.AutoHide)
            {
                HideFlyoutWindow();
                RemoveContentFromTabGroup(content);
            }
            
            if (content.State == DockableContentState.FloatingWindow ||
                content.State == DockableContentState.DockableWindow)
            {
                DockableFloatingWindow floatingWindow = Window.GetWindow(content) as DockableFloatingWindow;

                if (floatingWindow != null &&
                    (floatingWindow.Content as Pane).HasSingleItem && 
                    !floatingWindow.IsClosing)
                {
                    floatingWindow.Close();
                }
            }

            if (content.State != DockableContentState.Hidden)
            {
                DockableContents.Add(content);

                content.SetStateToHidden();
                content.DetachFromContainerPane();
            }

            if (ActiveDocument == content)
                ActiveDocument = null;

            if (ActiveContent == content)
                ActiveContent = null;
        }

        /// <summary>
        /// Show or add a document in AvalonDock
        /// </summary>
        /// <param name="document">Document to show/add.</param>
        /// <remarks>If document provided is not present in the <see cref="Documents"/> list, this method inserts it in first position of <see cref="MainDocumentPane.Items"/> collection.
        /// In both cases select it in the container <see cref="DocumentPane"/>.</remarks>
        internal void Show(DocumentContent document)
        {
            bool found = Documents.FirstOrDefault(d => d == document) != null;

            if (!found && MainDocumentPane != null)
            {
                if (document.Parent is DocumentPane)
                {
                    ((DocumentPane)document.Parent).Items.Clear();
                }

                MainDocumentPane.Items.Insert(0, document);
            }

        }

        /// <summary>
        /// Show or add a document in AvalonDock
        /// </summary>
        /// <param name="document">Document to show/add.</param>
        /// <param name="floating">Indicates if the document should be placed in a floating window</param>
        /// <remarks>If document provided is not present in the <see cref="Documents"/> list, this method inserts it in first position of <see cref="MainDocumentPane.Items"/> collection.
        /// In both cases select it in the container <see cref="DocumentPane"/>.</remarks>
        internal void Show(DocumentContent document, bool floating)
        {
            bool found = Documents.FirstOrDefault(d => d == document) != null;

            if (!found && MainDocumentPane != null)
            {
                if (document.Parent is DocumentPane)
                {
                    ((DocumentPane)document.Parent).Items.Clear();
                }

                if (floating)
                {
                    DocumentFloatingWindow floatingWindow = new DocumentFloatingWindow(this);
                    floatingWindow.Owner = Window.GetWindow(this);
                    floatingWindow.Content = document;
                    floatingWindow.Show();
                }
                else
                    MainDocumentPane.Items.Insert(0, document);
            }
            else if (found && document.ContainerPane is FloatingDocumentPane)
            {
                var containerPane = document.ContainerPane as FloatingDocumentPane;
                DocumentPane previousPane = containerPane.PreviousPane;
                int arrayIndexPreviuosPane = containerPane.ArrayIndexPreviousPane;
                //if previous pane exist that redock to it
                if (previousPane == null ||
                    previousPane.GetManager() != this)
                {
                    previousPane = MainDocumentPane;
                    arrayIndexPreviuosPane = 0;
                }

                if (previousPane != null)
                {
                    if (arrayIndexPreviuosPane > previousPane.Items.Count)
                        arrayIndexPreviuosPane = previousPane.Items.Count;

                    previousPane.Items.Insert(arrayIndexPreviuosPane,
                        containerPane.RemoveContent(0));
                }

                containerPane.FloatingWindow.Close();
            }

        }

        /// <summary>
        /// Show a dockable content in its container <see cref="Pane"/>
        /// </summary>
        /// <param name="content">Content to show</param>
        internal void Show(DockableContent content)
        {
            //if desideredState is not defined, use the saved state if exists
            if (content.SavedStateAndPosition != null)
                Show(content, content.SavedStateAndPosition.State);
            else
                Show(content, DockableContentState.Docked);
        }

        /// <summary>
        /// Show a dockable content in its container with a desidered state
        /// </summary>
        /// <param name="content">Content to show</param>
        /// <param name="desideredState">State desidered</param>
        internal void Show(DockableContent content, DockableContentState desideredState)
        {
            Show(content, desideredState, AnchorStyle.None);
        }

        /// <summary>
        /// Show a dockable content in its container with a desidered state
        /// </summary>
        /// <param name="content">Content to show</param>
        /// <param name="desideredState">State desidered</param>
        /// <param name="desideredAnchor">Border to which anchor the newly created container pane</param>
        /// <remarks></remarks>
        internal void Show(DockableContent content, DockableContentState desideredState, AnchorStyle desideredAnchor)
        {
            Debug.WriteLine(string.Format("Show Content={0}, desideredState={1}, desideredAnchor={2}", content.Name, desideredState, desideredAnchor));
            
            #region Dockable content
	
            if (desideredState == DockableContentState.Hidden)//??!!show hidden?
                Hide(content);

            if (content.State == DockableContentState.AutoHide)
            {
                //first redock the content
                (content.ContainerPane as DockablePane).ToggleAutoHide();
                //then show it as desidered
                Show(content, desideredState, desideredAnchor);
            }
            else if (content.State == DockableContentState.Docked ||
                content.State == DockableContentState.Document ||
                content.State == DockableContentState.None)
            {
                if (content.ContainerPane == null || 
                    content.State == DockableContentState.None)
                {
                    //Problem!? try to rescue
                    if (content.State == DockableContentState.Docked || 
                        content.State == DockableContentState.None)
                    {
                        //find the the pane which the desidered anchor style
                        //DockablePane foundPane = this.FindChildDockablePane(desideredAnchor != AnchorStyle.None ? desideredAnchor : AnchorStyle.Right);
                        //first search for a pane with other contents (avoiding empty panes which are containers for hidden contents)
                        ILinqToTree<DependencyObject> itemFound = new LogicalTreeAdapter(this).Descendants().FirstOrDefault(el => el.Item is DockablePane && (el.Item as DockablePane).Anchor == desideredAnchor && (el.Item as DockablePane).IsDocked);

                        if (itemFound == null)//search for all panes (even empty)
                            itemFound = new LogicalTreeAdapter(this).Descendants().FirstOrDefault(el => el.Item is DockablePane && (el.Item as DockablePane).Anchor == desideredAnchor && (el.Item as DockablePane).Items.Count == 0);

                        DockablePane foundPane = itemFound != null ? itemFound.Item as DockablePane : null;

                        if (foundPane != null)
                        {
                            content.SetStateToDock(); 
                            foundPane.Items.Add(content);
                            var containerPanel = foundPane.Parent as ResizingPanel;
                            if (containerPanel != null)
                                containerPanel.InvalidateMeasure();
                        }
                        else
                        {
                            //if no suitable pane was found create e new one on the fly
                            if (content.ContainerPane != null)
                            {
                                content.ContainerPane.RemoveContent(content);
                            }

                            DockablePane pane = new DockablePane();
                            pane.Items.Add(content);
                            Anchor(pane, desideredAnchor);
                        }
                    }
                    else
                    {
                        //add to main document pane
                        MainDocumentPane.Items.Add(content);
                    }

                }
                
                if (content.ContainerPane.GetManager() == null)
                {
                    //disconnect the parent pane from previous panel
                    //((Panel)content.ContainerPane.Parent).Children.Remove(content.ContainerPane);
                    if (content.ContainerPane.Parent != null)
                    {
                        ((Panel)content.ContainerPane.Parent).Children.Remove(content.ContainerPane);
                    }

                    Anchor(content.ContainerPane as DockablePane, desideredAnchor);
                }

                if (desideredState == DockableContentState.DockableWindow ||
                     desideredState == DockableContentState.FloatingWindow)
                {
                    var floatingWindow = new DockableFloatingWindow(this);
                    floatingWindow.Content = content;

                    var mainWindow = Window.GetWindow(this);
                    if (mainWindow.IsVisible)
                        floatingWindow.Owner = mainWindow;

                    //floatingWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    //if (content.Content != null)
                    //{
                    //    floatingWindow.Width = Math.Min(((FrameworkElement)content.Content).ActualWidth, ResizingPanel.GetResizeWidth(content.ContainerPane));
                    //    floatingWindow.Height = Math.Min(((FrameworkElement)content.Content).ActualHeight, ResizingPanel.GetResizeHeight(content.ContainerPane));
                    //}
                    //else
                    ////{
                    //    floatingWindow.Width = 400;
                    //    floatingWindow.Height = 400;
                    //}
                    
                    floatingWindow.Show();
                    

                }
                else if (desideredState == DockableContentState.AutoHide)
                {
                    var paneContainer = content.ContainerPane as DockablePane;
                    Debug.Assert(paneContainer != null);

                    if (paneContainer != null)
                        paneContainer.ToggleAutoHide();

                    content.Activate();
                }
                else if (desideredState == DockableContentState.Document)
                {
                    DocumentPane docPane = MainDocumentPane;
                    if (docPane != null)
                    {
                        docPane.Items.Add(content.DetachFromContainerPane());
                        docPane.SelectedItem = content;
                        content.SetStateToDocument();
                    }
                }
                else
                {
                    content.ContainerPane.SelectedItem = content;
                    content.Activate();

                    DockablePane dockParent = content.ContainerPane as DockablePane;
                    if (content.ActualWidth == 0.0 && (
                        dockParent.Anchor == AnchorStyle.Left || dockParent.Anchor == AnchorStyle.Right))
                    {
                        ResizingPanel.SetResizeWidth(dockParent, new GridLength(200));
                        ResizingPanel.SetEffectiveSize(dockParent, new Size(200, 0.0));
                    }
                    else if (content.ActualWidth == 0.0 && (
                        dockParent.Anchor == AnchorStyle.Top || dockParent.Anchor == AnchorStyle.Bottom))
                    {
                        ResizingPanel.SetResizeHeight(dockParent, new GridLength(200));
                        ResizingPanel.SetEffectiveSize(dockParent, new Size(200, 0.0));
                    }
                    
                }
            }
            else if (content.State == DockableContentState.Document)
            {
                if (content.ContainerPane != null)
                    content.ContainerPane.SelectedItem = this;
                content.Activate();
            }
            else if (content.State == DockableContentState.Hidden ||
                content.State == DockableContentState.DockableWindow ||
                content.State == DockableContentState.FloatingWindow)
            {
                if (content.State == DockableContentState.Hidden)
                {
                    //Debug.Assert(HiddenContents.Contains(content));
                    //HiddenContents.Remove(content);
                }
                else
                {
                    FloatingWindow floatingWindow = null;
                    floatingWindow = (content.ContainerPane as FloatingDockablePane).FloatingWindow;
                    content.DetachFromContainerPane();

                    if (floatingWindow.HostedPane.Items.Count == 0)
                        floatingWindow.Close();
                }

                if (desideredState == DockableContentState.Docked ||
                    desideredState == DockableContentState.AutoHide)
                {

                    if (content.SavedStateAndPosition != null &&
                        content.SavedStateAndPosition.ContainerPane != null &&
                        content.SavedStateAndPosition.ChildIndex >= 0 &&
                        content.SavedStateAndPosition.ContainerPane.GetManager() == this &&
                        desideredState == DockableContentState.Docked)
                    {
                        //ok previous container pane is here..
                        Pane prevPane = content.SavedStateAndPosition.ContainerPane;

                        if (content.SavedStateAndPosition.ChildIndex < prevPane.Items.Count)
                        {
                            prevPane.Items.Insert(content.SavedStateAndPosition.ChildIndex, content);
                        }
                        else
                        {
                            prevPane.Items.Add(content);
                        }

                        if (prevPane.Items.Count == 1)
                        {
                            if (!double.IsNaN(content.SavedStateAndPosition.Width) ||
                                !double.IsInfinity(content.SavedStateAndPosition.Width))
                            {
                                ResizingPanel.SetResizeWidth(content, 
                                    new GridLength(content.SavedStateAndPosition.Width));
                            }
                        }

                        DockablePane prevDockablePane = prevPane as DockablePane;
                        if (prevDockablePane != null && prevDockablePane.IsAutoHidden)
                        {
                            prevDockablePane.ToggleAutoHide();
                        }

                        content.SetStateToDock();
                        content.Activate();
                        
                        (prevPane.Parent as UIElement).InvalidateMeasure();
                    }
                    else
                    {
                        if (desideredAnchor == AnchorStyle.None &&
                            content.SavedStateAndPosition != null &&
                            content.SavedStateAndPosition.Anchor != AnchorStyle.None)
                            desideredAnchor = content.SavedStateAndPosition.Anchor;

                        if (desideredAnchor == AnchorStyle.None)
                            desideredAnchor = AnchorStyle.Right;

                        DockablePane foundPane = null;
                        
                        if (desideredState == DockableContentState.Docked)
                        {
                            //first not empty panes
                            ILinqToTree<DependencyObject> itemFound = new LogicalTreeAdapter(this).Descendants().FirstOrDefault(el => el.Item is DockablePane && (el.Item as DockablePane).Anchor == desideredAnchor && (el.Item as DockablePane).IsDocked);

                            if (itemFound == null)//look for all panes even empty
                                itemFound = new LogicalTreeAdapter(this).Descendants().FirstOrDefault(el => el.Item is DockablePane && (el.Item as DockablePane).Anchor == desideredAnchor && (el.Item as DockablePane).Items.Count == 0);

                            foundPane = itemFound != null ? itemFound.Item as DockablePane : null;
                        }

                        if (foundPane != null)
                        {
                            content.SetStateToDock();
                            foundPane.Items.Add(content);

                            if ((foundPane.IsAutoHidden && desideredState == DockableContentState.Docked) ||
                                 (!foundPane.IsAutoHidden && desideredState == DockableContentState.AutoHide))
                                foundPane.ToggleAutoHide();
                        }
                        else
                        {
                            DockablePane newHostpane = new DockablePane();
                            newHostpane.Items.Add(content);

                            if (desideredAnchor == AnchorStyle.Left ||
                                desideredAnchor == AnchorStyle.Right)
                            {
                                double w = 200;
                                if (content.SavedStateAndPosition != null &&
                                    !double.IsInfinity(content.SavedStateAndPosition.Width) &&
                                    !double.IsNaN(content.SavedStateAndPosition.Width))
                                    w = content.SavedStateAndPosition.Width;

                                ResizingPanel.SetResizeWidth(newHostpane, new GridLength(w));
                                ResizingPanel.SetEffectiveSize(newHostpane, new Size(w, 0.0));
                            }
                            else
                            {
                                double h = 200;
                                if (content.SavedStateAndPosition != null &&
                                    !double.IsInfinity(content.SavedStateAndPosition.Height) &&
                                    !double.IsNaN(content.SavedStateAndPosition.Height))
                                    h = content.SavedStateAndPosition.Height;

                                ResizingPanel.SetResizeHeight(newHostpane, new GridLength(h));
                                ResizingPanel.SetEffectiveSize(newHostpane, new Size(0.0, h));
                            }


                            Anchor(newHostpane, desideredAnchor);

                            if (desideredState == DockableContentState.AutoHide)
                            {
                                ToggleAutoHide(newHostpane);
                            }
                        }
                    }
                        
                    ActiveContent = content;
                }
                else if (desideredState == DockableContentState.DockableWindow ||
                    desideredState == DockableContentState.FloatingWindow)
                {
					DockablePane newHostpane = null;
					FloatingDockablePane prevHostpane = null;
					if (content.SavedStateAndPosition != null && content.SavedStateAndPosition.ContainerPane != null && content.SavedStateAndPosition.ContainerPane is FloatingDockablePane)
					{
						prevHostpane = content.SavedStateAndPosition.ContainerPane as FloatingDockablePane;
						if (!prevHostpane.Items.Contains(content))
							prevHostpane.Items.Add(content);
					}
					else
					{
                        newHostpane = new DockablePane();
						newHostpane.Items.Add(content);

					}

                    if (desideredState == DockableContentState.DockableWindow)
                        content.SetStateToDockableWindow();
                    else if (desideredState == DockableContentState.FloatingWindow)
                        content.SetStateToFloatingWindow();

					if (prevHostpane != null)
					{
                        //check to see if floating window that host prevHostPane is already loaded (hosting other contents)
                        var floatingWindow = prevHostpane.Parent as DockableFloatingWindow;
                        if (floatingWindow != null && floatingWindow.IsLoaded)
                        {
                            floatingWindow.Activate();
                        }
                        else
                        {
                            floatingWindow = new DockableFloatingWindow(this);
                            floatingWindow.Content = content;
                            floatingWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                            floatingWindow.Top = prevHostpane.FloatingWindow.Top;
                            floatingWindow.Left = prevHostpane.FloatingWindow.Left;
                            floatingWindow.Width = prevHostpane.FloatingWindow.Width;
                            floatingWindow.Height = prevHostpane.FloatingWindow.Height;
                            //floatingWindow.Owner = Window.GetWindow(this);
                            var mainWindow = Window.GetWindow(this);
                            if (mainWindow.IsVisible)
                                floatingWindow.Owner = mainWindow;

                            

                            //now I've created a new pane to host the hidden content
                            //if a an hidden content is shown that has prevHostpane as saved pane
                            //I want that it is relocated in this new pane that I've created right now
                            var hiddenContents = DockableContents.Where(c => c.State == DockableContentState.Hidden).ToArray();
                            foreach (var hiddenContent in hiddenContents)
                            {
                                if (hiddenContent.SavedStateAndPosition.ContainerPane == prevHostpane)
                                {
                                    hiddenContent.SavedStateAndPosition = new DockableContentStateAndPosition(
                                        (floatingWindow.Content as Pane),
                                        hiddenContent.SavedStateAndPosition.ChildIndex,
                                        hiddenContent.SavedStateAndPosition.Width,
                                        hiddenContent.SavedStateAndPosition.Height,
                                        hiddenContent.SavedStateAndPosition.Anchor,
                                        hiddenContent.SavedStateAndPosition.State);
                                }
                            }
						    
                            floatingWindow.Show();          
                        }
					}
					else if (newHostpane != null)
					{
						var floatingWindow = new DockableFloatingWindow(this);
                        floatingWindow.Content = newHostpane;
						floatingWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
						floatingWindow.Width = 200;
						floatingWindow.Height = 500;
						//floatingWindow.Owner = Window.GetWindow(this);
                        var mainWindow = Window.GetWindow(this);
                        if (mainWindow.IsVisible)
                            floatingWindow.Owner = mainWindow;

						
						floatingWindow.Show();
					}

                }
                else if (desideredState == DockableContentState.Document)
                {
                    DocumentPane docPane = MainDocumentPane;
                    if (docPane != null)
                    {
                        docPane.Items.Add(content);
                        docPane.SelectedItem = content;
                        content.SetStateToDocument();
                    }
                }
            }

           #endregion          
        }
        #endregion


        #region Anchor Style Update routines
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            //at the moment this is the easiest way to get anchor properties always updated
            if (this.Content as ResizingPanel != null)
                UpdateAnchorStyle();

            //hide the flyout window because transform could be changed
            HideFlyoutWindow();

            return base.ArrangeOverride(arrangeBounds);
        }

        internal void UpdateAnchorStyle()
        {
            ResizingPanel mainPanel = this.Content as ResizingPanel;
            Debug.Assert(mainPanel != null);


            UpdateAnchorStyle(mainPanel);
        }

        /// <summary>
        /// Update the <see cref="DockablePane.Anchor"/> property relative to the <see cref="DocumentContent"/> object
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        /// <remarks>Traverse the logical tree starting from root <see cref="ResizingPanel"/> and set property <see cref="DockablePane.Anchor"/> of dockable pane found.</remarks>
        void UpdateAnchorStyle(ResizingPanel panel)
        {
            AnchorStyle currentAnchor = panel.Orientation == Orientation.Horizontal ? AnchorStyle.Left : AnchorStyle.Top;
            bool foundDocumentContent = false;

            foreach (FrameworkElement child in panel.Children)
            {
                if (child is ResizingPanel)
                {
                    if (!foundDocumentContent &&
                        GetMainDocumentPane(child as ResizingPanel) != null)
                    {
                        foundDocumentContent = true;
                        currentAnchor = panel.Orientation == Orientation.Horizontal ? AnchorStyle.Right : AnchorStyle.Bottom;
                        UpdateAnchorStyle(child as ResizingPanel);
                    }
                    else
                        ForceAnchorStyle(child as ResizingPanel, currentAnchor);
                }
                else if (child is DocumentPane)
                {
                    foundDocumentContent = true;
                    currentAnchor = panel.Orientation == Orientation.Horizontal ? AnchorStyle.Right : AnchorStyle.Bottom;
                }
                else if (child is DockablePane)
                {
                    (child as DockablePane).Anchor = currentAnchor;
                }
            }
        }

        /// <summary>
        /// Called by <see cref="UpdateAnchorStyle"/> whene a <see cref="DocumentContent"/> object has been found
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="forcedAnchor"></param>
        void ForceAnchorStyle(ResizingPanel panel, AnchorStyle forcedAnchor)
        {
            foreach (FrameworkElement child in panel.Children)
            {
                if (child is ResizingPanel)
                {
                    ForceAnchorStyle((child as ResizingPanel), forcedAnchor);
                }
                else if ((child is DockablePane))
                {
                    ((DockablePane)child).Anchor = forcedAnchor;
                }
            }
        }
        
        #endregion


        #region Flyout window
        /// <summary>
        /// Stores the only one flyout window that can be open at time
        /// </summary>
        FlyoutPaneWindow _flyoutWindow = null;

        /// <summary>
        /// This object is used to handle interop events (i.e. WindowsPosChanging) of the main window the contains this
        /// DockingManager object
        /// </summary>
        /// <remarks>WindowsPosChanging are useful to automatically resize the FlyoutWindow when user move
        /// the main window.</remarks>
        WindowInteropWrapper _wndInteropWrapper = null;


        /// <summary>
        /// Gets or sets a value indicating if flyout windows should animate when are open or closed
        /// </summary>
        public bool IsAnimationEnabled
        {
            get { return (bool)GetValue(IsAnimationEnabledProperty); }
            set { SetValue(IsAnimationEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAnimationEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAnimationEnabledProperty =
            DependencyProperty.Register("IsAnimationEnabled", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(false));
                


        /// <summary>
        /// Closes the flyout window
        /// </summary>
        void HideFlyoutWindow()
        {
            if (_flyoutWindow != null && !_flyoutWindow.IsClosing)
            {
                var flWindow = _flyoutWindow;
                _flyoutWindow = null;
                flWindow.Closing -= new System.ComponentModel.CancelEventHandler(OnFlyoutWindowClosing);
                flWindow.Close();
            }
        }

        /// <summary>
        /// Shows a flyout window for a content
        /// </summary>
        /// <param name="content">Content to show</param>
        internal void ShowFlyoutWindow(DockableContent content, DockablePaneAnchorTab tabActivating)
        {
            //check if parent window is Active
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow == null || !parentWindow.IsVisible)
                return;
            
            if (!parentWindow.IsActive && (_flyoutWindow == null || !_flyoutWindow.IsActive))
                return;

            //check if content is already visible in a flyout window
            if (_flyoutWindow != null &&
                _flyoutWindow.ReferencedPane.Items.Contains(content))
            {
                //continue to show the winow
                //_flyoutWindow.KeepWindowOpen();
                return;
            }

            //hide previous window
            HideFlyoutWindow();

            //select this content in the referenced pane
            content.ContainerPane.SelectedItem = content;

            if (_wndInteropWrapper == null)
            {
                _wndInteropWrapper = new WindowInteropWrapper(parentWindow);
                _wndInteropWrapper.WindowPosChanging += (s, e) =>
                    {
                        //update the flyout window
                        UpdateFlyoutWindowPosition();
                    };
            }

            //create e new window
            _flyoutWindow = new FlyoutPaneWindow(this, content);
            _flyoutWindow.Owner = parentWindow;
            _flyoutWindow.FlowDirection = this.FlowDirection;
            _flyoutWindow.ShowActivated = false;
            _flyoutWindow.AnchorTabActivating = tabActivating;
            
            UpdateFlyoutWindowPosition(true);

            _flyoutWindow.Closing += new System.ComponentModel.CancelEventHandler(OnFlyoutWindowClosing);
            _flyoutWindow.Show();

            //this.Focus();
        }

        /// <summary>
        /// Handles the resize changed event to update location and size of the flyout window
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdateFlyoutWindowPosition();
            base.OnRenderSizeChanged(sizeInfo);
        }
        
        /// <summary>
        /// Handle the closing event of the flyout window to reset internal variables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnFlyoutWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_flyoutWindow != null)
            {
                _flyoutWindow.Closing -= new System.ComponentModel.CancelEventHandler(OnFlyoutWindowClosing);
                _flyoutWindow.Owner = null;
            }
        }

        /// <summary>
        /// Update location and size of the flyout window
        /// </summary>
        void UpdateFlyoutWindowPosition()
        {
            UpdateFlyoutWindowPosition(false);
        }

        /// <summary>
        /// Update location and size of the flyout window
        /// </summary>
        /// <param name="initialSetup">Indicates if thw current flyout window should be setup for the first time.</param>
        void UpdateFlyoutWindowPosition(bool initialSetup)
        {
            if (_flyoutWindow == null)
                return;
            if (_flyoutWindow.ReferencedPane == null)
                return;
            if (_flyoutWindow.ReferencedPane.SelectedItem == null)
                return;

            var actualSize = this.TransformedActualSize();

            double leftTabsWidth = FlowDirection == FlowDirection.LeftToRight ? _leftAnchorTabPanel.TransformedActualSize().Width : _rightAnchorTabPanel.TransformedActualSize().Width;
            double rightTabsWidth = FlowDirection == FlowDirection.LeftToRight ? _rightAnchorTabPanel.TransformedActualSize().Width : _leftAnchorTabPanel.TransformedActualSize().Width;
            double topTabsHeight = _topAnchorTabPanel.TransformedActualSize().Height;
            double bottomTabsHeight = _bottomAnchorTabPanel.TransformedActualSize().Height;
            bool hOrientation = _flyoutWindow.ReferencedPane.Anchor == AnchorStyle.Right || _flyoutWindow.ReferencedPane.Anchor == AnchorStyle.Left;

            Point locDockingManager = HelperFunc.PointToScreenWithoutFlowDirection(this, new Point());
            Point locContent = HelperFunc.PointToScreenWithoutFlowDirection(Content as FrameworkElement, new Point());

            Size initialSetupFlyoutWindowSize = Size.Empty;
            initialSetupFlyoutWindowSize = (_flyoutWindow.ReferencedPane.SelectedItem as DockableContent).FlyoutWindowSize;

            if (hOrientation && initialSetupFlyoutWindowSize.Width <= 0.0)
                initialSetupFlyoutWindowSize = ResizingPanel.GetEffectiveSize(_flyoutWindow.ReferencedPane.ReferencedPane);

            if (!hOrientation && initialSetupFlyoutWindowSize.Height <= 0.0)
                initialSetupFlyoutWindowSize = ResizingPanel.GetEffectiveSize(_flyoutWindow.ReferencedPane.ReferencedPane);

            initialSetupFlyoutWindowSize = this.TransformSize(initialSetupFlyoutWindowSize);

            double resWidth = initialSetup ? initialSetupFlyoutWindowSize.Width : _flyoutWindow.Width;
            double resHeight = initialSetup ? initialSetupFlyoutWindowSize.Height : _flyoutWindow.Height;

            if (_flyoutWindow.ReferencedPane.Anchor == AnchorStyle.Right)
            {
                _flyoutWindow.MaxWidth = actualSize.Width - rightTabsWidth;
                _flyoutWindow.MaxHeight = actualSize.Height - topTabsHeight - bottomTabsHeight;

                _flyoutWindow.Top = locDockingManager.Y + topTabsHeight;
                _flyoutWindow.Height = _flyoutWindow.MaxHeight;


                if (initialSetup)
                {
                    _flyoutWindow.Left = (FlowDirection == FlowDirection.LeftToRight ? locDockingManager.X + actualSize.Width - rightTabsWidth : locDockingManager.X + leftTabsWidth);
                    _flyoutWindow.Width = 0.0;
                    _flyoutWindow.TargetWidth = resWidth;
                }
                else
                {
                    if (!_flyoutWindow.IsOpening && !_flyoutWindow.IsClosing)
                        _flyoutWindow.Left = (FlowDirection == FlowDirection.LeftToRight ? locDockingManager.X + actualSize.Width - rightTabsWidth - _flyoutWindow.Width : locDockingManager.X + leftTabsWidth);
                }
            }
            if (_flyoutWindow.ReferencedPane.Anchor == AnchorStyle.Left)
            {
                _flyoutWindow.MaxWidth = actualSize.Width - leftTabsWidth;
                _flyoutWindow.MaxHeight = actualSize.Height - topTabsHeight - bottomTabsHeight;

                _flyoutWindow.Top = locDockingManager.Y + topTabsHeight;
                _flyoutWindow.Height = _flyoutWindow.MaxHeight;


                if (initialSetup)
                {
                    _flyoutWindow.Left = FlowDirection == FlowDirection.RightToLeft ? locDockingManager.X + actualSize.Width - rightTabsWidth : locDockingManager.X + leftTabsWidth;
                    _flyoutWindow.Width = 0.0;
                    _flyoutWindow.TargetWidth = resWidth;
                }
                else
                {
                    if (!_flyoutWindow.IsOpening && !_flyoutWindow.IsClosing)
                        _flyoutWindow.Left = FlowDirection == FlowDirection.RightToLeft ? locDockingManager.X + actualSize.Width - rightTabsWidth - _flyoutWindow.Width : locDockingManager.X + leftTabsWidth;
                }
            }
            if (_flyoutWindow.ReferencedPane.Anchor == AnchorStyle.Top)
            {
                _flyoutWindow.MaxWidth = actualSize.Width - rightTabsWidth - leftTabsWidth;
                _flyoutWindow.MaxHeight = actualSize.Height - topTabsHeight;

                _flyoutWindow.Left = locDockingManager.X + leftTabsWidth;
                _flyoutWindow.Width = _flyoutWindow.MaxWidth;
                
                if (initialSetup)
                {
                    _flyoutWindow.Height = 0.0;
                    _flyoutWindow.TargetHeight = resHeight;
                }
                else
                {
                    if (!_flyoutWindow.IsOpening && !_flyoutWindow.IsClosing)
                        _flyoutWindow.Top = locDockingManager.Y + topTabsHeight;
                }
            }
            if (_flyoutWindow.ReferencedPane.Anchor == AnchorStyle.Bottom)
            {
                _flyoutWindow.MaxWidth = actualSize.Width - rightTabsWidth - leftTabsWidth;
                _flyoutWindow.MaxHeight = actualSize.Height - bottomTabsHeight;

                _flyoutWindow.Left = locDockingManager.X + leftTabsWidth;
                _flyoutWindow.Width = _flyoutWindow.MaxWidth;

                if (initialSetup)
                {
                    _flyoutWindow.Top = locDockingManager.Y + actualSize.Height - bottomTabsHeight;
                    _flyoutWindow.Height = 0.0;
                    _flyoutWindow.TargetHeight = resHeight;
                }
                else
                {
                    if (!_flyoutWindow.IsOpening && !_flyoutWindow.IsClosing)
                        _flyoutWindow.Top = locDockingManager.Y + actualSize.Height - bottomTabsHeight - _flyoutWindow.Height;
                    //if (_flyoutWindow.IsClosing)
                    //    _flyoutWindow.Top = locDockingManager.Y + actualSize.Height - bottomTabsHeight - _flyoutWindow.Height;
                }
            }

            if (_flyoutWindow != null && !_flyoutWindow.IsClosing)
                _flyoutWindow.UpdatePositionAndSize();

            if (initialSetup)
                _flyoutWindow.ReferencedPane.LayoutTransform = (MatrixTransform)this.TansformToAncestor();

            Debug.WriteLine(string.Format("UpdateFlyoutWindowPosition() Rect->{0} InitialSetup={1}", new Rect(_flyoutWindow.Left, _flyoutWindow.Top, _flyoutWindow.Width, _flyoutWindow.Height), initialSetup));
        }
        
        #endregion


        #region DragDrop Operations
        /// <summary>
        /// Begins dragging operations
        /// </summary>
        /// <param name="floatingWindow">Floating window containing pane which is dragged by user</param>
        /// <param name="point">Current mouse position</param>
        /// <param name="offset">Offset to be use to set floating window screen position</param>
        /// <returns>Retruns True is drag is completed, false otherwise</returns>
        internal bool Drag(FloatingWindow floatingWindow, Point point, Point offset)
        {
            bool mouseCaptured = IsMouseCaptured;

            if (!mouseCaptured)
                mouseCaptured = CaptureMouse();

            if (mouseCaptured)
            {
                floatingWindow.Owner = Window.GetWindow(this);

                DragPaneServices.StartDrag(floatingWindow, point, offset);
                return true;
            }

            return false;
        }

        internal void Drag(DocumentContent documentContent, Point point, Point offset)
        {
            if (CaptureMouse())
            {
                DocumentFloatingWindow floatingWindow = new DocumentFloatingWindow(this);
                floatingWindow.Content = documentContent;
                Drag(floatingWindow, point, offset);
            }

        }


        internal void Drag(DockableContent dockableContent, Point point, Point offset)
        {
            if (CaptureMouse())
            {
                var floatingWindow = new DockableFloatingWindow(this);
                floatingWindow.Content = dockableContent;
                floatingWindow.Owner = Window.GetWindow(this);
                Drag(floatingWindow, point, offset);
            }
        }


        internal void Drag(DockablePane dockablePane, Point point, Point offset)
        {
            if (CaptureMouse())
            {
                var floatingWindow = new DockableFloatingWindow(this);
                floatingWindow.Content = dockablePane;
                floatingWindow.Owner = Window.GetWindow(this);
                Drag(floatingWindow, point, offset);
            }
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                DragPaneServices.MoveDrag(this.PointToScreenDPI(e.GetPosition(this)));

                if (_flyoutWindow != null)
                    _flyoutWindow.UpdatePositionAndSize();
            }

            base.OnMouseMove(e);
        }


        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            CompleteDragging(e.GetPosition(this));
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            CompleteDragging(e.GetPosition(this));
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Ends all previously initiated dragging operations
        /// </summary>
        /// <param name="ptEndDrag"></param>
        void CompleteDragging(Point ptEndDrag)
        {
            if (IsMouseCaptured)
            {
                DragPaneServices.EndDrag(this.PointToScreenDPI(ptEndDrag));
                ReleaseMouseCapture();
            }
        }

        DragPaneServices _dragPaneServices;

        internal DragPaneServices DragPaneServices
        {
            get
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                    return null;

                if (_dragPaneServices == null)
                    _dragPaneServices = new DragPaneServices(this);

                return _dragPaneServices;
            }
        }
        #endregion

        #region IDropSurface
        bool IDropSurface.IsSurfaceVisible
        {
            get 
            { 
                //a DockingManager is always visible for drop a pane
                return true; 
            }
        }

        /// <summary>
        /// Returns a rectangle where this surface is active
        /// </summary>
        Rect IDropSurface.SurfaceRectangle
        {
            get
            {
                if (PresentationSource.FromVisual(this) != null)
                {
                    var actualSize = this.TransformedActualSize();
                    return new Rect(HelperFunc.PointToScreenWithoutFlowDirection(this, new Point(0, 0)), new Size(actualSize.Width, actualSize.Height));
                }

                return Rect.Empty;
            }
        }

        /// <summary>
        /// Overlay window which shows docking placeholders
        /// </summary>
        OverlayWindow _overlayWindow;

        /// <summary>
        /// Returns current overlay window
        /// </summary>
        internal OverlayWindow OverlayWindow
        {
            get
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                    throw new NotSupportedException("OverlayWindow not valid in design mode");

                if (_overlayWindow == null)
                    _overlayWindow = new OverlayWindow(this);

                return _overlayWindow;
            }
        }

        /// <summary>
        /// Handles this sourface mouse entering (show current overlay window)
        /// </summary>
        /// <param name="point">Current mouse position</param>
        void IDropSurface.OnDragEnter(Point point)
        {
            if (OverlayWindow.IsVisible)
                return;

            var actualSize = this.TransformedActualSize();
            OverlayWindow.Owner = DragPaneServices.FloatingWindow;
            Point origPoint = HelperFunc.PointToScreenWithoutFlowDirection(this, new Point());
            OverlayWindow.Left = origPoint.X;
            OverlayWindow.Top = origPoint.Y;
            OverlayWindow.Width = actualSize.Width;
            OverlayWindow.Height = actualSize.Height;

            //don't pass transform matrix to Overlay window otherwise anchor thumbs will be resized
            OverlayWindow.Show();
        }

        /// <summary>
        /// Handles mouse overing this surface
        /// </summary>
        /// <param name="point"></param>
        void IDropSurface.OnDragOver(Point point)
        {

        }

        /// <summary>
        /// Handles mouse leave event during drag (hide overlay window)
        /// </summary>
        /// <param name="point"></param>
        void IDropSurface.OnDragLeave(Point point)
        {
            OverlayWindow.Owner = null;
            OverlayWindow.Hide();
        }

        /// <summary>
        /// Handler drop events
        /// </summary>
        /// <param name="point">Current mouse position</param>
        /// <returns>Returns alwasy false because this surface doesn't support direct drop</returns>
        bool IDropSurface.OnDrop(Point point)
        {
            return false;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Persistence

        void SaveLayout(XmlWriter xmlWriter, DockablePane pane)
        {
            bool needToSavePane = pane.Items.Count > 0;

            //if a pane is reference by a content than save it
            if (DockableContents.FirstOrDefault(d => d.SavedStateAndPosition != null && d.SavedStateAndPosition.ContainerPane == pane) != null)
                needToSavePane = true;

            if (needToSavePane)
            {
                xmlWriter.WriteStartElement("DockablePane");

                xmlWriter.WriteAttributeString("ResizeWidth", ResizingPanel.GetResizeWidth(pane).ToString());
                xmlWriter.WriteAttributeString("ResizeHeight", ResizingPanel.GetResizeHeight(pane).ToString());
                xmlWriter.WriteAttributeString("EffectiveSize", new SizeConverter().ConvertToInvariantString(ResizingPanel.GetEffectiveSize(pane)));
                xmlWriter.WriteAttributeString("ID", pane.ID.ToString());
                xmlWriter.WriteAttributeString("Anchor", pane.Anchor.ToString());
                
                if (pane.Items.Count > 1)
                    xmlWriter.WriteAttributeString("SelectedIndex", XmlConvert.ToString(pane.SelectedIndex));

                xmlWriter.WriteAttributeString("IsAutoHidden", XmlConvert.ToString(pane.IsAutoHidden));

                foreach (DockableContent content in pane.Items)
                {
                    SaveLayout(xmlWriter, content);
                }


                xmlWriter.WriteEndElement();
            }
        }

        void SaveLayout(XmlWriter xmlWriter, DockableFloatingWindow flWindow)
        {
            xmlWriter.WriteStartElement("FloatingWindow");
            xmlWriter.WriteAttributeString("IsDockableWindow", XmlConvert.ToString(flWindow.IsDockableWindow));

            xmlWriter.WriteAttributeString("Top", XmlConvert.ToString(flWindow.Top));
            xmlWriter.WriteAttributeString("Left", XmlConvert.ToString(flWindow.Left));
            xmlWriter.WriteAttributeString("Width", XmlConvert.ToString(flWindow.Width));
            xmlWriter.WriteAttributeString("Height", XmlConvert.ToString(flWindow.Height));
            
            
            SaveLayout(xmlWriter, flWindow.HostedPane as DockablePane);

            xmlWriter.WriteEndElement();
        }

        void SaveLayout(XmlWriter xmlWriter, DockableContent content)
        {
            Debug.Assert(!string.IsNullOrEmpty(content.Name),
            "DockableContent must have a Name to save its content.\n" +
            "Click Ignore to skip this element and continue with save."
            );

            if (!string.IsNullOrEmpty(content.Name))
            {
                xmlWriter.WriteStartElement("DockableContent");

                xmlWriter.WriteAttributeString("Name", content.Name);

                content.SaveLayout(xmlWriter);

                xmlWriter.WriteEndElement();
            }
        }

        void SaveLayout(XmlWriter xmlWriter, DocumentContent content)
        {
            if (!string.IsNullOrEmpty(content.Name))
            {
                xmlWriter.WriteStartElement("DocumentContent");

                xmlWriter.WriteAttributeString("Name", content.Name);

                content.SaveLayout(xmlWriter);

                xmlWriter.WriteEndElement();
            }
        }

        void SaveLayout(XmlWriter xmlWriter, DocumentPane pane)
        {
            if (pane.Items.Count == 0 && !pane.IsMainDocumentPane.GetValueOrDefault())
                return;

            xmlWriter.WriteStartElement("DocumentPane");

            if (pane.IsMainDocumentPane.GetValueOrDefault())
                xmlWriter.WriteAttributeString("IsMain", "true");

            if (pane.Items.Count > 1)
                xmlWriter.WriteAttributeString("SelectedIndex", XmlConvert.ToString(pane.SelectedIndex));

            xmlWriter.WriteAttributeString("ResizeWidth", ResizingPanel.GetResizeWidth(pane).ToString());
            xmlWriter.WriteAttributeString("ResizeHeight", ResizingPanel.GetResizeHeight(pane).ToString());
            xmlWriter.WriteAttributeString("EffectiveSize", new SizeConverter().ConvertToInvariantString(ResizingPanel.GetEffectiveSize(pane)));

            foreach (ManagedContent content in pane.Items)
            {
                if (content is DockableContent)
                {
                    var dockableContent = content as DockableContent;
                    SaveLayout(xmlWriter, dockableContent);
                }
                else if (content is DocumentContent)
                {
                    var documentContent = content as DocumentContent;
                    SaveLayout(xmlWriter, documentContent);
                }
            }

            xmlWriter.WriteEndElement();
        }

        //void SaveLayout(XmlWriter xmlWriter, DocumentPaneResizingPanel panelToSerialize)
        //{
        //    xmlWriter.WriteStartElement("DocumentPanePlaceHolder");

        //    //List<DockableContent> listOfFoundContents = new List<DockableContent>();
        //    //FindContents<DockableContent>(listOfFoundContents, panelToSerialize);
        //    var listOfFoundContents = new LogicalTreeAdapter(panelToSerialize).Descendants().Where(i => i.Item is DockableContent).Select(i => i.Item);

        //    foreach (DockableContent content in listOfFoundContents)
        //    {
        //        SaveLayout(xmlWriter, content);
        //    }


        //    xmlWriter.WriteEndElement();
        //}
        
        void SaveLayout(XmlWriter xmlWriter, ResizingPanel panelToSerialize)
        {
            if (panelToSerialize is DocumentPaneResizingPanel)
                xmlWriter.WriteStartElement("DocumentPaneResizingPanel");
            else
                xmlWriter.WriteStartElement("ResizingPanel");

            //if (!double.IsInfinity(ResizingPanel.GetResizeWidth(panelToSerialize)))
            //    xmlWriter.WriteAttributeString("ResizeWidth", XmlConvert.ToString(ResizingPanel.GetResizeWidth(panelToSerialize)));
            //if (!double.IsInfinity(ResizingPanel.GetResizeHeight(panelToSerialize)))
            //    xmlWriter.WriteAttributeString("ResizeHeight", XmlConvert.ToString(ResizingPanel.GetResizeHeight(panelToSerialize)));

            xmlWriter.WriteAttributeString("ResizeWidth", ResizingPanel.GetResizeWidth(panelToSerialize).ToString());
            xmlWriter.WriteAttributeString("ResizeHeight", ResizingPanel.GetResizeHeight(panelToSerialize).ToString());
            xmlWriter.WriteAttributeString("EffectiveSize", new SizeConverter().ConvertToInvariantString(ResizingPanel.GetEffectiveSize(panelToSerialize)));

            xmlWriter.WriteAttributeString("Orientation", Convert.ToString(panelToSerialize.Orientation));
            

            foreach (UIElement child in panelToSerialize.Children)
            {
                if (child is DockablePane)
                    SaveLayout(xmlWriter, child as DockablePane);
                else if (child is DocumentPane)
                    SaveLayout(xmlWriter, child as DocumentPane);
                //else if (child is DocumentPaneResizingPanel)
                //    SaveLayout(xmlWriter, child as DocumentPaneResizingPanel);
                else if (child is ResizingPanel)
                    SaveLayout(xmlWriter, child as ResizingPanel);
            }

            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Save layout as xml to a <see cref="TextWriter"/>
        /// </summary>
        /// <param name="textWriter">Text writter object which receive the xml text</param>
        /// <remarks>The writer is not closed.</remarks>
        public void SaveLayout(TextWriter textWriter)
        {
            XmlTextWriter sw = new XmlTextWriter(textWriter);

            sw.Formatting = Formatting.Indented;
            sw.Indentation = 4;

            SaveLayout(sw);
            
        }

        /// <summary>
        /// Save layout as xml to generic stream
        /// </summary>
        /// <param name="backendStream">Stream receiving the xml string</param>
        /// <remarks>The stream is not closed</remarks>
        public void SaveLayout(Stream backendStream)
        {
            XmlTextWriter sw = new XmlTextWriter(backendStream, Encoding.Default);

            sw.Formatting = Formatting.Indented;
            sw.Indentation = 4;

            SaveLayout(sw);
        }

        /// <summary>
        /// Save layout as xml text into a file
        /// </summary>
        /// <param name="filename">Path to the file</param>
        /// <remarks>The file is created as new or overwritten is already exist a file with same name.</remarks>
        public void SaveLayout(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
                SaveLayout(stream);
        }

        /// <summary>
        /// Layout format version
        /// </summary>
        const string layoutFileVersion = "1.3.0";
        
        /// <summary>
        /// Send layout configuration to a <see cref="XmlTextWriter"/> object
        /// </summary>
        /// <param name="sw">Object which stores the xml</param>
        /// <remarks>The writer is not closed.</remarks>
        public void SaveLayout(XmlWriter sw)
        {
            if (!_isControlLoaded)
                throw new InvalidOperationException("Unable to serialize docking layout while DockingManager control is unloaded");

            EnsureContentNotEmpty();
            ClearEmptyPanes();
            HideFlyoutWindow();

            sw.WriteStartElement("DockingManager");
            sw.WriteAttributeString("version", layoutFileVersion);

            if (Content is ResizingPanel)
                SaveLayout(sw, Content as ResizingPanel);
            else if (Content is DocumentPane)
                SaveLayout(sw, Content as DocumentPane);

            sw.WriteStartElement("Hidden");

            var hiddenContents = DockableContents.Where(c => c.State == DockableContentState.Hidden).ToArray();
            foreach (DockableContent content in hiddenContents)
            {
                SaveLayout(sw, content);
            }
            sw.WriteEndElement();

            sw.WriteStartElement("Windows");
            foreach (var flWindow in _floatingWindows.OfType<DockableFloatingWindow>())
            {
                SaveLayout(sw, flWindow);
            }
            sw.WriteEndElement();
            
            sw.WriteEndElement();//dockingmanager

            sw.Flush();
        }


        void DetachContentFromDockingManager(DockableContent content)
        {
            if (content.State == DockableContentState.AutoHide)
            {
                DockablePane parentContainer = content.Parent as DockablePane;
                if (parentContainer != null &&
                    parentContainer.Items.Count == 1)
                    parentContainer.ToggleAutoHide();
            }
            if (content.State == DockableContentState.DockableWindow ||
                content.State == DockableContentState.FloatingWindow)
            {
                DockablePane parentContainer = content.Parent as DockablePane;

                if (parentContainer != null &&
                    parentContainer.Items.Count == 1)
                {
                    FloatingWindow floatingWindow = Window.GetWindow(content) as FloatingWindow;
                    floatingWindow.Close(true);
                }
            }
            //this content can be hidden also if was contained in closed floating window 
            if (content.State == DockableContentState.Hidden)
                Show(content, DockableContentState.Docked);

            content.DetachFromContainerPane();
        }


        public delegate void DeserializationCallbackHandler(object sender, DeserializationCallbackEventArgs e);
        public DeserializationCallbackHandler DeserializationCallback { get; set; }


        //void ShowAllHiddenContents()
        //{
        //    var hiddenContents = DockableContents.Where(c => c.State == DockableContentState.Hidden).ToArray();
        //    foreach (var hiddenContent in hiddenContents)
        //    {
        //        //Debug.Assert(HiddenContents[0].State == DockableContentState.Hidden);
        //        hiddenContent.Show();
        //    }
        //}
        
        #region Restore Layout Overloads

        /// <summary>
        /// Restore a <see cref="DockingManager"/> layout from xml
        /// </summary>
        /// <param name="backendStream"></param>
        public void RestoreLayout(Stream backendStream)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(backendStream);

            RestoreLayout(doc);
        }

        /// <summary>
        /// Restore a <see cref="DockingManager"/> layout from xml
        /// </summary>
        /// <param name="reader"></param>
        public void RestoreLayout(XmlReader reader)
        {
            XmlDocument doc = new XmlDocument();
            
            doc.Load(reader);

            RestoreLayout(doc);
        }

        /// <summary>
        /// Loads a xml content from a file and restore the <see cref="DockingManager"/> layout contained in it
        /// </summary>
        /// <param name="filename"></param>
        public void RestoreLayout(string filename)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(filename);

            RestoreLayout(doc);
        }

        /// <summary>
        /// Restore a <see cref="DockingManager"/> layout from a xml string
        /// </summary>
        /// <param name="reader"></param>
        public void RestoreLayout(TextReader reader)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(reader);

            RestoreLayout(doc);
        }


        #region Restore Layout Core
        ///// <summary>
        ///// Restore from xml a document pane
        ///// </summary>
        ///// <param name="childElement"></param>
        ///// <param name="mainExistingDocumentPane"></param>
        ///// <param name="existingDocumentPanel"></param>
        ///// <param name="dockableContents"></param>
        //void RestoreDocumentPaneLayout(XmlElement childElement, out DocumentPane mainExistingDocumentPane, out DocumentPaneResizingPanel existingDocumentPanel, DockableContent[] dockableContents)
        //{
        //    mainExistingDocumentPane = (Content is DocumentPane) ? Content as DocumentPane : GetMainDocumentPane(Content as ResizingPanel);
        //    if (mainExistingDocumentPane != null)
        //    {
        //        existingDocumentPanel = mainExistingDocumentPane.GetParentDocumentPaneResizingPanel();
        //    }
        //    else
        //    {
        //        existingDocumentPanel = null;
        //    }

        //    if (existingDocumentPanel != null)
        //    {
        //        if (existingDocumentPanel.Parent is ResizingPanel)
        //        {
        //            ((ResizingPanel)existingDocumentPanel.Parent).RemoveChild(existingDocumentPanel);
        //        }
        //        else if (existingDocumentPanel.Parent is DockingManager)
        //        {
        //            ((DockingManager)existingDocumentPanel.Parent).Content = null;
        //        }
        //    }
        //    else if (mainExistingDocumentPane != null)
        //    {
        //        if (mainExistingDocumentPane.Parent is ResizingPanel)
        //        {
        //            ((ResizingPanel)mainExistingDocumentPane.Parent).RemoveChild(mainExistingDocumentPane);
        //        }
        //        else if (mainExistingDocumentPane.Parent is DockingManager)
        //        {
        //            ((DockingManager)mainExistingDocumentPane.Parent).Content = null;
        //        }
        //    }

        //    foreach (XmlElement contentElement in childElement.ChildNodes)
        //    {
        //        if (contentElement.HasAttribute("Name"))
        //        {
        //            DockableContent foundContent = null;
        //            string contentName = contentElement.GetAttribute("Name");
        //            foreach (DockableContent content in dockableContents)
        //            {
        //                if (content.Name == contentName)
        //                {
        //                    foundContent = content;
        //                    break;
        //                }
        //            }

        //            if (foundContent == null &&
        //                DeserializationCallback != null)
        //            {
        //                DeserializationCallbackEventArgs e = new DeserializationCallbackEventArgs(contentName);
        //                DeserializationCallback(this, e);

        //                foundContent = e.Content as DockableContent;
        //            }


        //            if (foundContent != null)
        //            {
        //                DetachContentFromDockingManager(foundContent);
        //                mainExistingDocumentPane.Items.Add(foundContent);
        //                foundContent.SetStateToDocument();

        //                //call custom layout persistence method
        //                foundContent.RestoreLayout(contentElement);
        //            }
        //        }
        //    }
        //}

        DocumentPane RestoreDocumentPaneLayout(XmlElement mainElement, DockableContent[] actualContents, DocumentContent[] actualDocuments)
        {
            var documentPane = new DocumentPane();

            if (mainElement.HasAttribute("ResizeWidth"))
                ResizingPanel.SetResizeWidth(documentPane, (GridLength)GLConverter.ConvertFromInvariantString(mainElement.GetAttribute("ResizeWidth")));
            if (mainElement.HasAttribute("ResizeHeight"))
                ResizingPanel.SetResizeHeight(documentPane, (GridLength)GLConverter.ConvertFromInvariantString(mainElement.GetAttribute("ResizeHeight")));
            if (mainElement.HasAttribute("EffectiveSize"))
                ResizingPanel.SetEffectiveSize(documentPane, (Size)(new SizeConverter()).ConvertFromInvariantString(mainElement.GetAttribute("EffectiveSize")));

            foreach (XmlElement contentElement in mainElement.ChildNodes)
            {
                if (contentElement.Name == "DockableContent" &&
                                            contentElement.HasAttribute("Name"))
                {
                    DockableContent foundContent = null;
                    string contentName = contentElement.GetAttribute("Name");

                    foundContent = actualContents.FirstOrDefault(c => c.Name == contentName);

                    if (foundContent == null &&
                        DeserializationCallback != null)
                    {
                        DeserializationCallbackEventArgs e = new DeserializationCallbackEventArgs(contentName);
                        DeserializationCallback(this, e);

                        foundContent = e.Content as DockableContent;
                    }


                    if (foundContent != null)
                    {
                        DetachContentFromDockingManager(foundContent);
                        documentPane.Items.Add(foundContent);
                        foundContent.SetStateToDocument();

                        //call custom layout persistence method
                        foundContent.RestoreLayout(contentElement);
                    }
                }
                else if (contentElement.Name == "DocumentContent" &&
                    contentElement.HasAttribute("Name"))
                {
                    DocumentContent foundDocument = null;
                    string contentName = contentElement.GetAttribute("Name");

                    foundDocument = actualDocuments.FirstOrDefault(c => c.Name == contentName);

                    if (foundDocument == null &&
                        DeserializationCallback != null)
                    {
                        DeserializationCallbackEventArgs e = new DeserializationCallbackEventArgs(contentName);
                        DeserializationCallback(this, e);

                        foundDocument = e.Content as DocumentContent;
                    }


                    if (foundDocument != null)
                    {
                        foundDocument.DetachFromContainerPane();
                        documentPane.Items.Add(foundDocument);
                    }
                }
            }

            if (mainElement.HasAttribute("SelectedIndex"))
                documentPane.SelectedIndex = XmlConvert.ToInt32(mainElement.GetAttribute("SelectedIndex"));

            return documentPane;

        }

        DockablePane RestoreDockablePaneLayout(XmlElement mainElement, DockableContent[] actualContents, DocumentContent[] actualDocuments)
        {

            DockablePane pane = new DockablePane();

            if (mainElement.HasAttribute("Anchor"))
                pane.Anchor = (AnchorStyle)Enum.Parse(typeof(AnchorStyle), mainElement.GetAttribute("Anchor"));
            if (mainElement.HasAttribute("ResizeWidth"))
                ResizingPanel.SetResizeWidth(pane, (GridLength)GLConverter.ConvertFromInvariantString(mainElement.GetAttribute("ResizeWidth")));
            if (mainElement.HasAttribute("ResizeHeight"))
                ResizingPanel.SetResizeHeight(pane, (GridLength)GLConverter.ConvertFromInvariantString(mainElement.GetAttribute("ResizeHeight")));
            if (mainElement.HasAttribute("EffectiveSize"))
                ResizingPanel.SetEffectiveSize(pane, (Size)(new SizeConverter()).ConvertFromInvariantString(mainElement.GetAttribute("EffectiveSize")));
            if (mainElement.HasAttribute("ID"))
                pane.ID = new Guid(mainElement.GetAttribute("ID"));

            bool toggleAutoHide = false;
            if (mainElement.HasAttribute("IsAutoHidden"))
                toggleAutoHide = XmlConvert.ToBoolean(mainElement.GetAttribute("IsAutoHidden"));

            foreach (XmlElement contentElement in mainElement.ChildNodes)
            {
                if (contentElement.HasAttribute("Name"))
                {
                    DockableContent foundContent = null;
                    string contentName = contentElement.GetAttribute("Name");

                    foundContent = actualContents.FirstOrDefault(c => c.Name == contentName);

                    if (foundContent == null &&
                        DeserializationCallback != null)
                    {
                        DeserializationCallbackEventArgs e = new DeserializationCallbackEventArgs(contentName);
                        DeserializationCallback(this, e);

                        foundContent = e.Content as DockableContent;
                    }


                    if (foundContent != null)
                    {
                        DetachContentFromDockingManager(foundContent);
                        pane.Items.Add(foundContent);
                        foundContent.SetStateToDock();

                        //call custom layout persistence method
                        foundContent.RestoreLayout(contentElement);
                    }
                }
            }

            if (toggleAutoHide && pane.Items.Count > 0)
                ToggleAutoHide(pane);

            if (mainElement.HasAttribute("SelectedIndex"))
                pane.SelectedIndex = XmlConvert.ToInt32(mainElement.GetAttribute("SelectedIndex"));

            return pane;
        }

        ResizingPanel RestoreResizingPanel(XmlElement mainElement, DockableContent[] actualContents, DocumentContent[] actualDocuments, ref DocumentPane mainDocumentPane)
        {
            ResizingPanel panel = null;

            if (mainElement.Name == "DocumentPaneResizingPanel")
                panel = new DocumentPaneResizingPanel();
            else
                panel = new ResizingPanel();

            if (mainElement.HasAttribute("Orientation"))
                panel.Orientation = (Orientation)Enum.Parse(typeof(Orientation), mainElement.GetAttribute("Orientation"));
            if (mainElement.HasAttribute("ResizeWidth"))
                ResizingPanel.SetResizeWidth(panel, (GridLength)GLConverter.ConvertFromInvariantString(mainElement.GetAttribute("ResizeWidth")));
            if (mainElement.HasAttribute("ResizeHeight"))
                ResizingPanel.SetResizeHeight(panel, (GridLength)GLConverter.ConvertFromInvariantString(mainElement.GetAttribute("ResizeHeight")));
            if (mainElement.HasAttribute("EffectiveSize"))
                ResizingPanel.SetEffectiveSize(panel, (Size)(new SizeConverter()).ConvertFromInvariantString(mainElement.GetAttribute("EffectiveSize")));


            foreach (XmlElement childElement in mainElement.ChildNodes)
            {
                if (childElement.Name == "ResizingPanel" ||
                    childElement.Name == "DocumentPaneResizingPanel")
                {
                    var childPanel = RestoreResizingPanel(childElement, actualContents, actualDocuments, ref mainDocumentPane);

                    if (childPanel.Children.Count > 0)
                    {
                        panel.Children.Add(childPanel);
                    }
                    else
                    {
                        Debug.WriteLine("Found empty ResizingPanel in stored layout, it will be discarded.");
                    }
                }
                #region Restore DockablePane
                else if (childElement.Name == "DockablePane")
                {
                    var pane = RestoreDockablePaneLayout(childElement, actualContents, actualDocuments);

                    //restore dockable panes even if no contents are inside (an hidden content could refer this pane in SaveStateAndPosition)
                    panel.Children.Add(pane);

                }
                #endregion
                #region Restore Contents inside a DocumentPane
                else if (childElement.Name == "DocumentPane")
                {
                    var documentPane = RestoreDocumentPaneLayout(childElement, actualContents, actualDocuments);
                    
                    bool isMainDocumentPane = false;
                    if (childElement.HasAttribute("IsMain"))
                        isMainDocumentPane = XmlConvert.ToBoolean(childElement.GetAttribute("IsMain"));

                    if (documentPane.Items.Count > 0 ||
                        isMainDocumentPane)
                        panel.Children.Add(documentPane);

                    if (isMainDocumentPane)
                    {
                        if (mainDocumentPane != null)
                            throw new InvalidOperationException("Main document pane is set more than one time");

                        mainDocumentPane = documentPane;
                    }
                }

                #endregion
            }

            return panel;
        
        }

        /// <summary>
        /// Restore from xml a resizing panel or a documentpane
        /// </summary>
        /// <param name="mainElement"></param>
        /// <param name="actualContents"></param>
        /// <returns></returns>
        object RestoreLayout(XmlElement mainElement, DockableContent[] actualContents, DocumentContent[] actualDocuments, ref DocumentPane mainDocumentPane)
        {
            if (mainElement == null)
                throw new ArgumentNullException("mainElement");

            if (mainElement.Name == "ResizingPanel" ||
                mainElement.Name == "DocumentPaneResizingPanel")
            {
                return RestoreResizingPanel(mainElement, actualContents, actualDocuments, ref mainDocumentPane);
            }
            else if (mainElement.Name == "DocumentPane")
            {
                mainDocumentPane = RestoreDocumentPaneLayout(mainElement, actualContents, actualDocuments);
                return mainDocumentPane;
            }

            throw new InvalidOperationException(string.Format("Unable to deserialize '{0}' element", mainElement.Name));
        }

        /// <summary>
        /// True while is restoring a layout
        /// </summary>
        protected bool RestoringLayout { get; private set; }

        /// <summary>
        /// Internal main restore layout method
        /// </summary>
        /// <param name="doc">Document Xml from which restore layout</param>
        void RestoreLayout(XmlDocument doc)
        {
            if (!_isControlLoaded)
                throw new InvalidOperationException("Unable to deserialize a docking layout while DockingManager control is unloaded");

            if (doc.DocumentElement == null ||
                doc.DocumentElement.Name != "DockingManager")
            {
                Debug.Assert(false, "Layout file hasn't a valid structure!");
                throw new InvalidOperationException("Layout file had not a valid structure!");
            }

            if (doc.DocumentElement.GetAttribute("version") != layoutFileVersion)
                throw new FileFormatException("Unsupported layout file version");

            if (doc.DocumentElement.ChildNodes.Count != 3 ||
                (doc.DocumentElement.ChildNodes[0].Name != "ResizingPanel" && doc.DocumentElement.ChildNodes[0].Name != "DocumentPane") ||
                doc.DocumentElement.ChildNodes[1].Name != "Hidden" ||
                doc.DocumentElement.ChildNodes[2].Name != "Windows")
            {
                Debug.Assert(false, "Layout file hasn't a valid structure!");
                throw new InvalidOperationException("Layout file hasn't a valid structure!");
            }

            //Hide temp windows
            HideFlyoutWindow();
            HideNavigatorWindow();
            //HideDocumentNavigatorWindow();

            RestoringLayout = true;

            //show all auto hidden panes
            var panesAutoHidden = DockableContents.Where(c => c.State == DockableContentState.AutoHide).Select(c => c.ContainerPane).Distinct();
            foreach (DockablePane pane in panesAutoHidden)
                pane.ToggleAutoHide();

            DockableContent[] actualContents = DockableContents.ToArray();
            DocumentContent[] actualDocuments = Documents.ToArray();


            //first detach all my actual contents
            this.Content = null;
            this.ActiveContent = null;
            this.ActiveDocument = null;

            //restore main panel
            XmlElement rootElement = doc.DocumentElement.ChildNodes[0] as XmlElement;
            DocumentPane mainDocumentPane = null;
            this.Content = RestoreLayout(rootElement, actualContents, actualDocuments, ref mainDocumentPane);
            MainDocumentPane = mainDocumentPane;
            
            //restore hidden contents
            foreach (XmlElement hiddenContentElement in doc.DocumentElement.ChildNodes[1].ChildNodes)
            {
                var hiddenContentName = hiddenContentElement.GetAttribute("Name");

                var hiddenContent = actualContents.FirstOrDefault(c => c.Name == hiddenContentName &&
                    c.State != DockableContentState.Hidden);

                if (hiddenContent != null)
                {
                    Hide(hiddenContent);
                    hiddenContent.RestoreLayout(hiddenContentElement);
                }
            }

            //restore floating windows
            foreach (XmlElement flWindowElement in doc.DocumentElement.ChildNodes[2].ChildNodes)
            {
                if (flWindowElement.ChildNodes.Count != 1)
                    continue;//handles invalid layouts structures

                bool isDockableWindow = XmlConvert.ToBoolean(flWindowElement.GetAttribute("IsDockableWindow"));
                Point location = new Point(XmlConvert.ToDouble(flWindowElement.GetAttribute("Left")), XmlConvert.ToDouble(flWindowElement.GetAttribute("Top")));
                Size size = new Size(XmlConvert.ToDouble(flWindowElement.GetAttribute("Width")), XmlConvert.ToDouble(flWindowElement.GetAttribute("Height")));

                XmlElement paneElement = flWindowElement.ChildNodes[0] as XmlElement;

                DockablePane paneForFloatingWindow = new DockablePane();
                if (paneElement.HasAttribute("ResizingWidth"))
                    ResizingPanel.SetResizeWidth(paneForFloatingWindow, (GridLength)GLConverter.ConvertFromInvariantString(paneElement.GetAttribute("ResizeWidth")));
                if (paneElement.HasAttribute("ResizingHeight"))
                    ResizingPanel.SetResizeHeight(paneForFloatingWindow, (GridLength)GLConverter.ConvertFromInvariantString(paneElement.GetAttribute("ResizeHeight")));
                paneForFloatingWindow.Anchor = (AnchorStyle)Enum.Parse(typeof(AnchorStyle), paneElement.GetAttribute("Anchor"));


                DockableContent contentToTransfer = null;
                foreach (XmlElement contentElement in paneElement.ChildNodes)
                {
                    #region Find the content to transfer
                    string contentToFindName = contentElement.GetAttribute("Name");
                    contentToTransfer = actualContents.FirstOrDefault(c => c.Name == contentToFindName);

                    if (contentToTransfer == null &&
                        DeserializationCallback != null)
                    {
                        DeserializationCallbackEventArgs e = new DeserializationCallbackEventArgs(contentToFindName);
                        DeserializationCallback(this, e);

                        contentToTransfer = e.Content as DockableContent;
                    }
                    #endregion
                    if (contentToTransfer != null)
                    {
                        DetachContentFromDockingManager(contentToTransfer);
                        paneForFloatingWindow.Items.Add(contentToTransfer);
                        contentToTransfer.RestoreLayout(contentElement);
                    }
                }

                if (paneForFloatingWindow.Items.Count > 0)
                {
                    var flWindow = new DockableFloatingWindow(this);
                    flWindow.Content = paneForFloatingWindow;
                    flWindow.Left = location.X;
                    flWindow.Top = location.Y;
                    flWindow.Width = size.Width;
                    flWindow.Height = size.Height;
                    flWindow.Owner = Window.GetWindow(this);

                    flWindow.IsDockableWindow = isDockableWindow;
                    flWindow.ShowActivated = false;

                    flWindow.ApplyTemplate();
                    flWindow.Show();
                }
            }

            ClearEmptyPanels(Content as ResizingPanel);

            //get documents that are not present in last layout and must be included
            //in the new one
            var documentsNotTransferred = actualDocuments.Where(d => d.ContainerPane == null || d.ContainerPane.GetManager() != this).ToArray();

            Debug.Assert(MainDocumentPane != null && MainDocumentPane.GetManager() == this);

            if (MainDocumentPane != null && documentsNotTransferred.Count() > 0)
            {
                documentsNotTransferred.ForEach(d => MainDocumentPane.Items.Add(d.DetachFromContainerPane()));
            }

            //get contents that are not present in the new layout and hide them
            var contentsNotTransferred = actualContents.Where(c => c.ContainerPane == null || c.ContainerPane.GetManager() != this).ToArray();

            contentsNotTransferred.ForEach(c =>
                {
                    Hide(c);
                });

            RestoringLayout = false;

            ClearEmptyPanes();
            RefreshContents();

            if (ActiveDocument != null &&
               (ActiveDocument.ContainerPane == null ||
               ActiveDocument.ContainerPane.GetManager() != this))
            {
                if (Documents.Count > 0)
                    ActiveDocument = Documents[0];
                else
                    ActiveDocument = null;
            }

            ActiveContent = ActiveDocument;
        } 
        #endregion
        
        #endregion
        /// <summary>
        /// Static converter used to convert GridLength from/to string
        /// </summary>
        static GridLengthConverter GLConverter = new GridLengthConverter();

        #endregion

        #region OnClosing/OnClosedDocument events
        /// <summary>
        /// Event fired when the document is about to be closed
        /// </summary>
        public event EventHandler<CancelEventArgs> DocumentClosing;

        /// <summary>
        /// Event fired when a document has been closed
        /// </summary>
        /// <remarks>Note that when a document is closed, property like <see cref="ManagedContent.ContainerPane"/> or <see cref="ManagedContent.Manager"/> returns null.</remarks>
        public event EventHandler DocumentClosed;

        /// <summary>
        /// Ovveride this method to handle <see cref="DocumentClosing"/> event.
        /// </summary>
        protected virtual void OnDocumentClosing(CancelEventArgs e)
        {
            if (DocumentClosing != null && !e.Cancel)
            {
                DocumentClosing(this, e);
            }
        }

        /// <summary>
        /// Ovveride this method to handle <see cref="DocumentClosed"/> event.
        /// </summary>
        protected virtual void OnDocumentClosed()
        {
            if (DocumentClosed != null)
                DocumentClosed(this, EventArgs.Empty);
        }

        internal void FireDocumentClosingEvent(CancelEventArgs e)
        {
            OnDocumentClosing(e);
        }

        internal void FireDocumentClosedEvent()
        {
            OnDocumentClosed();
        }

        //public event EventHandler<RequestDocumentCloseEventArgs> RequestDocumentClose;

        //internal bool FireRequestDocumentCloseEvent(DocumentContent doc)
        //{
        //    bool res = false;

        //    if (RequestDocumentClose != null)
        //    {
        //        RequestDocumentCloseEventArgs args = new RequestDocumentCloseEventArgs(doc);
        //        RequestDocumentClose(this, args);
        //        res = !args.Cancel;
        //    }

        //    return res;
        //}


        #endregion

    }
}
