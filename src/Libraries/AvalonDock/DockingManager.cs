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
using System.Windows.Interop;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Linq;

namespace AvalonDock
{
 
    /// <summary>
    /// Represents a control which manages a dockable layout for its children
    /// </summary>
    public class DockingManager : System.Windows.Controls.ContentControl, IDropSurface, INotifyPropertyChanged, IDisposable
    {
        static DockingManager()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockingManager), new FrameworkPropertyMetadata(typeof(DockingManager)));
        }


        public DockingManager()
        {
            DragPaneServices.Register(this);
            this.Unloaded += new RoutedEventHandler(DockingManager_Unloaded);
            this.Loaded += new RoutedEventHandler(DockingManager_Loaded);
        }


        #region Control lifetime management
         ~DockingManager()
        {
            Dispose(false);
        }

        bool _isControlLoaded = false;
        
        void DockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (FloatingWindow floatingWindow in _floatingWindows)
            {
                floatingWindow.Owner = Window.GetWindow(this);

                floatingWindow.Show();
            }

            DragPaneServices.Register(this);

            _isControlLoaded = true;
        }

        void DockingManager_Unloaded(object sender, RoutedEventArgs e)
        {
            //cleanup pending resources
            HideAutoHideWindow();
            

            if (_wndInteropWrapper != null)
            {
                _wndInteropWrapper.OnWindowPosChanging -= new EventHandler(_wndInteropWrapper_OnWindowPosChanging);
                _wndInteropWrapper.Dispose();
                _wndInteropWrapper = null;
            }

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

        
        /// <summary>
        /// Overriden to get a reference to underlying template elements
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _leftAnchorTabPanel = GetTemplateChild("PART_LeftAnchorTabPanel") as Panel;
            _rightAnchorTabPanel = GetTemplateChild("PART_RightAnchorTabPanel") as Panel;
            _topAnchorTabPanel = GetTemplateChild("PART_TopAnchorTabPanel") as Panel;
            _bottomAnchorTabPanel = GetTemplateChild("PART_BottomAnchorTabPanel") as Panel;

            if (_leftAnchorTabPanel != null)
                _anchorTabPanels.Add(_leftAnchorTabPanel);
            if (_rightAnchorTabPanel != null)
                _anchorTabPanels.Add(_rightAnchorTabPanel);
            if (_topAnchorTabPanel != null)
                _anchorTabPanels.Add(_topAnchorTabPanel);
            if (_bottomAnchorTabPanel != null)
                _anchorTabPanels.Add(_bottomAnchorTabPanel);

            System.Diagnostics.Debug.Assert(_leftAnchorTabPanel != null);
            System.Diagnostics.Debug.Assert(_rightAnchorTabPanel != null);
            System.Diagnostics.Debug.Assert(_topAnchorTabPanel != null);
            System.Diagnostics.Debug.Assert(_bottomAnchorTabPanel != null);
            _OnApplyTemplateFlag = true;
        }


        #region Access to contents and pane
        ManagedContent _activeDocument = null;

        /// <summary>
        /// Get or set the active document
        /// </summary>
        /// <remarks>The active document not neessary receive keyboard focus. To set keyboard focus on a content see <see cref="ActiveContent"/></remarks>
        public ManagedContent ActiveDocument
        {
            get 
            {
                return _activeDocument;
            }
            set
            {
                if (_activeDocument != value &&
                    value.ContainerPane is DocumentPane)
                {
                    List<ManagedContent> listOfAllDocuments = FindContents<ManagedContent>();
                    listOfAllDocuments.ForEach((ManagedContent cnt) =>
                        {
                            cnt.IsActiveDocument = cnt == value;
                        });

                    _activeDocument = value;
                    NotifyPropertyChanged("ActiveDocument");
                }
            }
        }

        ManagedContent _activeContent = null;

        /// <summary>
        /// Get or set the active content
        /// </summary>
        /// <remarks>An activated content is automatically selected in its container pane and receive logical as well keyboard focus.</remarks>
        public ManagedContent ActiveContent
        {
            get
            {
                return _activeContent;
            }
            internal set
            {
                ActiveDocument = value;

                if (_activeContent != value)
                {
                    List<ManagedContent> listOfAllContents = FindContents<ManagedContent>();
                    listOfAllContents.ForEach((ManagedContent cnt) =>
                        {
                            cnt.IsActiveContent = (value == cnt);
                        });

                    _floatingWindows.ForEach((DockableFloatingWindow fw) =>
                        {
                            foreach (DockableContent cnt in fw.HostedPane.Items)
                            {
                                cnt.IsActiveContent = (value == cnt);
                            }
                        });
                    if (_flyoutWindow != null)
                    {
                        foreach (DockableContent cnt in _flyoutWindow.ReferencedPane.Items)
                        {
                            cnt.IsActiveContent = (value == cnt);
                        }
                    }


                    _activeContent = value;
                    NotifyPropertyChanged("ActiveContent");
                }

            }
        }

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

        /// <summary>
        /// Gets an array of all dockable contents currenty managed
        /// </summary>
        public DockableContent[] DockableContents
        {
            get
            {
                List<DockableContent> contents = FindContents<DockableContent>();

                foreach (FloatingWindow flWindow in _floatingWindows)
                {
                    foreach (DockableContent content in flWindow.HostedPane.Items)
                        contents.Add(content);
                }

                foreach (DockableContent content in _hiddenContents)
                    contents.Add(content);

                return contents.ToArray();
            }
        }

        /// <summary>
        /// Gets an array of all document contents
        /// </summary>
        public DocumentContent[] Documents
        {
            get
            {
                return FindContents<DocumentContent>().ToArray<DocumentContent>();
            }
        }

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
            internal set
            {
                if (_mainDocumentPane == null)
                    _mainDocumentPane = value;
            }
        }

        internal List<T> FindContents<T>() where T : ManagedContent
        {
            List<T> resList = new List<T>();


            if (Content is Pane)
            {
                foreach (ManagedContent c in ((Pane)Content).Items)
                {
                    if (c is T)
                    {
                        resList.Add((T)c);
                    }
                }
            }
            else if (Content is ResizingPanel)
            {
                FindContents<T>(resList, Content as ResizingPanel);
            }

            foreach (FloatingWindow flWindow in _floatingWindows)
            {
                foreach (ManagedContent c in flWindow.HostedPane.Items)
                {
                    if (c is T)
                        resList.Add(c as T);
                }
            }

            if (_flyoutWindow != null && _flyoutWindow.ReferencedPane != null)
            {
                foreach (ManagedContent c in _flyoutWindow.ReferencedPane.Items)
                {
                    if (c is T)
                        resList.Add(c as T);
                }
            }


            return resList;
        }


        void FindContents<T>(List<T> listOfFoundContents, ResizingPanel parentPanel) where T : ManagedContent
        {
            foreach (UIElement child in parentPanel.Children)
            {
                if (child is Pane)
                {
                    foreach (ManagedContent c in ((Pane)child).Items)
                    {
                        if (c is T)
                        {
                            listOfFoundContents.Add((T)c);
                        }
                    }
                }
                else if (child is ResizingPanel)
                {
                    FindContents<T>(listOfFoundContents, child as ResizingPanel);
                }
            }
        }

        #endregion

        #region Floating windows management
        List<DockableFloatingWindow> _floatingWindows = new List<DockableFloatingWindow>();

        public DockableFloatingWindow[] FloatingWindows
        {
            get 
            {
                if (_floatingWindows == null ||
                    _floatingWindows.Count == 0)
                    return new DockableFloatingWindow[0];

                return _floatingWindows.ToArray();
            }
        }

        internal void RegisterFloatingWindow(DockableFloatingWindow floatingWindow)
        {
            if (_floatingWindows != null)
            {
                floatingWindow.FlowDirection = this.FlowDirection;
                _floatingWindows.Add(floatingWindow);
            }
        }

        internal void UnregisterFloatingWindow(DockableFloatingWindow floatingWindow)
        {
            if (_floatingWindows != null)
                _floatingWindows.Remove(floatingWindow);
        }




        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.CommandBindings.Clear();
            this.CommandBindings.Add(new CommandBinding(ShowNavigatorWindowCommand, OnExecuteCommand, OnCanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ShowDocumentNavigatorWindowCommand, OnExecuteCommand, OnCanExecuteCommand));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == FlowDirectionProperty)
            {
                _floatingWindows.ForEach((DockableFloatingWindow fl) =>
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
     
        #region Commands

        private static object syncRoot = new object();


        private static RoutedUICommand showNavigatorCommand = null;
        
        /// <summary>
        /// Get the command to show navigator window
        /// </summary>
        public static RoutedUICommand ShowNavigatorWindowCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == showNavigatorCommand)
                    {
                        showNavigatorCommand = new RoutedUICommand("S_how navigator window", "Navigator", typeof(DockingManager));
                        showNavigatorCommand.InputGestures.Add(new KeyGesture(Key.Tab, ModifierKeys.Control));
                    }

                }
                return showNavigatorCommand;
            }
        }

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
            navigatorWindow.Show();
            navigatorWindow.Focus();
        }

        void HideNavigatorWindow()
        {
            if (navigatorWindow != null)
            {
                navigatorWindow.Hide();
            }
        }


        void OnExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ShowNavigatorWindowCommand && ((Keyboard.Modifiers & ModifierKeys.Control)>0) )
            {
                ShowNavigatorWindow();
                e.Handled = true;
            }
            else if (e.Command == ShowDocumentNavigatorWindowCommand && ((Keyboard.Modifiers & ModifierKeys.Shift) > 0))
            {
                ShowDocumentNavigatorWindow();
                e.Handled = true;
            }
            
        }

        void OnCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        private static RoutedUICommand showDocumentNavigatorCommand = null;

        /// <summary>
        /// Get the command to show document navigator window
        /// </summary>
        public static RoutedUICommand ShowDocumentNavigatorWindowCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == showDocumentNavigatorCommand)
                    {
                        showDocumentNavigatorCommand = new RoutedUICommand("S_how document navigator window", "DocumentNavigator", typeof(DockingManager));
                        showDocumentNavigatorCommand.InputGestures.Add(new KeyGesture(Key.Tab, ModifierKeys.Shift));
                    }

                }
                return showDocumentNavigatorCommand;
            }
        }

        DocumentNavigatorWindow documentNavigatorWindow = null;

        void ShowDocumentNavigatorWindow()
        {
            HideDocumentNavigatorWindow();

            //if (documentNavigatorWindow == null)
            {
                documentNavigatorWindow = new DocumentNavigatorWindow(this);
                documentNavigatorWindow.Owner = Window.GetWindow(this);
            }

            if (MainDocumentPane == null)
                return;

            Point locMainDocumentPane = MainDocumentPane.PointToScreenDPI(new Point());
            documentNavigatorWindow.Left = locMainDocumentPane.X;
            documentNavigatorWindow.Top = locMainDocumentPane.Y;
            documentNavigatorWindow.Width = MainDocumentPane.ActualWidth;
            documentNavigatorWindow.Height = MainDocumentPane.ActualHeight;
            documentNavigatorWindow.Show();
            documentNavigatorWindow.Focus();
        }

        void HideDocumentNavigatorWindow()
        {
            //if (documentNavigatorWindow != null)
            //{
            //    documentNavigatorWindow.Hide();
                
            //    //don't close this window to be more responsive
            //    documentNavigatorWindow.Close();
            //    documentNavigatorWindow = null;
            //}
        }


        #endregion


        #region DockablePane operations
        /// <summary>
        /// Anchor a dockable pane to a border
        /// </summary>
        /// <param name="paneToAnchor"></param>
        /// <param name="anchor"></param>
        public void Anchor(Pane paneToAnchor, AnchorStyle anchor)
        {
            //remove the pane from its original children collection
            FrameworkElement parentElement = paneToAnchor.Parent as FrameworkElement;

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
                Content = toplevelPanel;

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
            foreach (ManagedContent content in paneToAnchor.Items)
            {
                if (content is DockableContent)
                {
                    ((DockableContent)content).SetStateToDock();
                }
            }           
            
            
            paneToAnchor.Focus();
        }

        /// <summary>
        /// Anchor a dockable pane (<see cref="DockablePane"/>) to a border of a docked pane
        /// </summary>
        /// <param name="paneToAnchor">Pane to anchor</param>
        /// <param name="relativePane">Pane relative</param>
        /// <param name="anchor"></param>
        public void Anchor(Pane paneToAnchor, Pane relativePane, AnchorStyle anchor)
        {
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
                Debug.Assert(false);
        }


        /// <summary>
        /// Anchor a dockable pane (<see cref="DockablePane"/>) to a border of a document pane
        /// </summary>
        /// <param name="paneToAnchor">Pane to anchor</param>
        /// <param name="relativePane">Pane relative</param>
        /// <param name="anchor"></param>
        public void Anchor(DockablePane paneToAnchor, DocumentPane relativePane, AnchorStyle anchor)
        {
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
            paneToAnchor.Focus();
        }



        /// <summary>
        /// Anchor a document pane (<see cref="DockablePane"/>) to a border of an other document pane
        /// </summary>
        /// <param name="paneToAnchor">Pane to anchor</param>
        /// <param name="relativePane">Pane relative</param>
        /// <param name="anchor"></param>
        public void Anchor(DocumentPane paneToAnchor, DocumentPane relativePane, AnchorStyle anchor)
        {
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
                }

            }
            #endregion

            paneToAnchor.Focus();

            (paneToAnchor.SelectedItem as ManagedContent).SetAsActive();
            if (paneToAnchor.SelectedItem is DocumentContent)
                ActiveDocument = paneToAnchor.SelectedItem as DocumentContent;
            
        }

        /// <summary>
        /// Anchor a dockable pane (<see cref="DockablePane"/>) to a border of an other dockable pane
        /// </summary>
        /// <param name="paneToAnchor">Pane to anchor</param>
        /// <param name="relativePane">Pane relative</param>
        /// <param name="anchor"></param>
        public void Anchor(DockablePane paneToAnchor, DockablePane relativePane, AnchorStyle anchor)
        {
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

            paneToAnchor.Focus();
        }


        public void DropInto(Pane paneDragged, Pane paneToDropInto)
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
                Debug.Assert(false);
        }


        public void DropInto(DocumentPane paneDragged, DocumentPane paneToDropInto)
        {
            //transfer tha contents of dragged pane (conatined in a FloatingWindow)
            //to the pane which user select
            ManagedContent contentToFocus = null;
            while (paneDragged.Items.Count > 0)
            {
                ManagedContent contentToTransfer = paneDragged.RemoveContent(0);
                paneToDropInto.Items.Insert(0, contentToTransfer);
                contentToFocus = contentToTransfer;
            }

            
            paneToDropInto.SelectedIndex = 0;
            paneToDropInto.Focus();
            if (contentToFocus != null)
                contentToFocus.SetAsActive();
        }

        public void DropInto(DockablePane paneDragged, DocumentPane paneToDropInto)
        {
            if (paneToDropInto != MainDocumentPane)
                paneToDropInto = MainDocumentPane;

            //transfer contents of dragged pane (contained in a FloatingWindow)
            //to the pane which user select, taking care of setting contents state
            //to Dock (using Dock() method of class DockablePane).
            while (paneDragged.Items.Count > 0)
            {
                ManagedContent contentToTransfer = paneDragged.RemoveContent(0);
                paneToDropInto.Items.Add(contentToTransfer);


                DockableContent dockContentToTransfer = contentToTransfer as DockableContent;

                if (dockContentToTransfer != null)
                    dockContentToTransfer.SetStateToDocument();
            }

            paneToDropInto.SelectedIndex = paneToDropInto.Items.Count - 1;
            paneToDropInto.Focus();
        }

        public void DropInto(DockablePane paneDragged, DockablePane paneToDropInto)
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
            paneToDropInto.Focus();
        }

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
                        //foreach (DockablePaneAnchorTab tab in group.Children)
                        //{
                        //    tab.ReferencedContent.Icon = tab.Icon;
                        //    tab.Icon = null;
                        //}

                        anchorTabPanel.Children.Remove(group);
                        return true;
                    }
                }
            }


            return false;
        }

        /// <summary>
        /// Autohides/redock a dockable pane
        /// </summary>
        /// <param name="pane">Pane to auto hide/redock</param>
        public void ToggleAutoHide(DockablePane pane)
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

                HideAutoHideWindow();

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
                    tab.Anchor = pane.Anchor;
                    tab.Icon = content.Icon;


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

            while (parentElement != null)
            {
                parentElement.InvalidateMeasure();
                parentElement = parentElement.Parent as FrameworkElement;
            }


        }
        
        #endregion
        
        
        #region Hide/Show contents

        List<DockableContent> _hiddenContents = new List<DockableContent>();

#if DEBUG
        internal void CheckHiddenState(DockableContent contentToCheck)
        {
            if (contentToCheck.State == DockableContentState.Hidden)
                Debug.Assert(_hiddenContents.Contains(contentToCheck));
            else
                Debug.Assert(!_hiddenContents.Contains(contentToCheck));
        }
#endif
        /// <summary>
        /// Hide a dockable content removing it from its container <see cref="Pane"/>
        /// </summary>
        /// <param name="content">Content to hide</param>
        /// <remarks>Note that if you simply remove a content from its container without calling this method, the
        /// layout serializer component can't managed correctly the removed content.</remarks>
        public void Hide(DockableContent content)
        {
            if (content.State == DockableContentState.Hidden)
                return;

            content.SaveCurrentStateAndPosition();

            if (content.State == DockableContentState.AutoHide)
            {
                HideAutoHideWindow();
                RemoveContentFromTabGroup(content);
            }
            
            if (content.State == DockableContentState.FloatingWindow ||
                content.State == DockableContentState.DockableWindow)
            {
                DockableFloatingWindow floatingWindow = Window.GetWindow(content) as DockableFloatingWindow;

                if (floatingWindow != null &&
                    floatingWindow.HostedPane.HasSingleItem)
                {
                    floatingWindow.Close();
                }

                content.DetachFromContainerPane();
            }
            else
            {
                content.DetachFromContainerPane();
            }


            if (content.State != DockableContentState.Hidden)
            {
                content.SetStateToHidden();

                Debug.Assert(!_hiddenContents.Contains(content));
                _hiddenContents.Add(content);
            }

            //if (ActiveContent == content)
            //    ActiveContent = null;
        }

        /// <summary>
        /// Show or add a document in AvalonDock
        /// </summary>
        /// <param name="document">Document to show/add.</param>
        /// <remarks>If document provided is not present in the <see cref="Documents"/> list, this method inserts it in first position of <see cref="MainDocumentPane.Items"/> collection.
        /// In both cases select it in the container <see cref="DocumentPane"/>.</remarks>
        public void Show(DocumentContent document)
        {
            bool found = false;
            foreach (DocumentContent doc in Documents)
            {
                if (doc == document)
                {
                    found = true;
                    break;
                }
            }

            if (!found && MainDocumentPane != null)
            {
                MainDocumentPane.Items.Insert(0, document);
            }

            //DocumentPane docPane = document.ContainerPane as DocumentPane;

            //if (docPane != null)
            //{
            //    docPane.SelectedItem = document;
            //    if (document.Content is IInputElement)
            //        Keyboard.Focus(document.Content as IInputElement);
            //}
            document.SetAsActive();
        }

        /// <summary>
        /// Show a dockable content in its container <see cref="Pane"/>
        /// </summary>
        /// <param name="content">Content to show</param>
        public void Show(DockableContent content)
        {
            Show(content, DockableContentState.Docked);
        }

        /// <summary>
        /// Show a dockable content in its container with a desidered state
        /// </summary>
        /// <param name="content">Content to show</param>
        /// <param name="desideredState">State desidered</param>
        public void Show(DockableContent content, DockableContentState desideredState)
        {
            Show(content, desideredState, AnchorStyle.Right);
        }

        /// <summary>
        /// Show a dockable content in its container with a desidered state
        /// </summary>
        /// <param name="content">Content to show</param>
        /// <param name="desideredState">State desidered</param>
        /// <param name="desideredAnchor">Border to which anchor the newly created container pane</param>
        /// <remarks></remarks>
        public void Show(DockableContent content, DockableContentState desideredState, AnchorStyle desideredAnchor)
        {
            #region Dockable content
	
            if (desideredState == DockableContentState.Hidden)//??!!show hidden?
                Hide(content);

            if (content.State == DockableContentState.AutoHide)
            {
                ShowFlyoutWindow(content);
            }
            else if (content.State == DockableContentState.Docked ||
                content.State == DockableContentState.Document)
            {
                if (content.ContainerPane == null)
                {
                    //Problem!? try to rescue
                    if (content.State == DockableContentState.Docked)
                    {
                        //find the the pane which the desidered anchor style
                        DockablePane foundPane = this.FindChildDockablePane(desideredAnchor);
                        if (foundPane != null)
                            foundPane.Items.Add(content);
                        else
                        {
                            //if no suitable pane was found create e new one on the fly
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
                    ((Panel)content.ContainerPane.Parent).Children.Remove(content.ContainerPane);
                    Anchor(content.ContainerPane, desideredAnchor);
                }

                if (desideredState == DockableContentState.DockableWindow ||
                     desideredState == DockableContentState.FloatingWindow)
                {
                    DockableFloatingWindow floatingWindow = new DockableFloatingWindow(this, content);
                    floatingWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    floatingWindow.Owner = Window.GetWindow(this);
                    //if (content.Content != null)
                    //{
                    //    floatingWindow.Width = Math.Min(((FrameworkElement)content.Content).ActualWidth, ResizingPanel.GetResizeWidth(content.ContainerPane));
                    //    floatingWindow.Height = Math.Min(((FrameworkElement)content.Content).ActualHeight, ResizingPanel.GetResizeHeight(content.ContainerPane));
                    //}
                    //else
                    //{
                        floatingWindow.Width = 400;
                        floatingWindow.Height = 400;
                    //}
                    RegisterFloatingWindow(floatingWindow);
                    floatingWindow.Show();

                }
                else if (desideredState == DockableContentState.AutoHide)
                {
                    if (content.ContainerPane != null)
                        content.ContainerPane.SelectedItem = this;
                    //content.FocusContent();
                    //if (content.Content is IInputElement)
                    //    Keyboard.Focus(content.Content as IInputElement);
                    content.SetAsActive();

                    DockablePane.ToggleAutoHideCommand.Execute(null, content.ContainerPane);
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
                    content.SetAsActive();
                    //ActiveContent = content;
                    ////content.FocusContent();
                    //if (content.Content is IInputElement)
                    //    Keyboard.Focus(content.Content as IInputElement);
                }
            }
            else if (
                content.State == DockableContentState.DockableWindow ||
                content.State == DockableContentState.FloatingWindow)
            {
                FloatingDockablePane containerPane = content.ContainerPane as FloatingDockablePane;
                if (containerPane != null)
                    containerPane.FloatingWindow.Activate();

            }
            else if (content.State == DockableContentState.Document)
            {
                if (content.ContainerPane != null)
                    content.ContainerPane.SelectedItem = this;
                //content.FocusContent();
                //if (content.Content is IInputElement)
                //    Keyboard.Focus(content.Content as IInputElement);
                content.SetAsActive();
            }
            else if (content.State == DockableContentState.Hidden)
            {
                Debug.Assert(_hiddenContents.Contains(content));

                _hiddenContents.Remove(content);

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

                        prevPane.SelectedItem = content;
                        content.SetStateToDock();
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

                            //ResizingPanel.SetResizeWidth(newHostpane, w);
                        }
                        else
                        {
                            double h = 200;
                            if (content.SavedStateAndPosition != null &&
                                !double.IsInfinity(content.SavedStateAndPosition.Height) &&
                                !double.IsNaN(content.SavedStateAndPosition.Height))
                                h = content.SavedStateAndPosition.Height;

                            //ResizingPanel.SetResizeHeight(newHostpane, h);
                        }

                        Anchor(newHostpane, desideredAnchor);

                        if (desideredState == DockableContentState.AutoHide)
                        {
                            DockablePane.ToggleAutoHideCommand.Execute(null, newHostpane);
                        }

                    }
                        
                    ActiveContent = content;
                }
                else if (desideredState == DockableContentState.DockableWindow ||
                    desideredState == DockableContentState.FloatingWindow)
                {
                    DockablePane newHostpane = new DockablePane();
                    newHostpane.Items.Add(content);
                    content.SetStateToDock();

                    //ResizingPanel.SetResizeWidth(newHostpane, 200);
                    //ResizingPanel.SetResizeWidth(newHostpane, 500);

                    DockableFloatingWindow floatingWindow = new DockableFloatingWindow(this, newHostpane);
                    floatingWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    floatingWindow.Width = 200;
                    floatingWindow.Height = 500;
                    floatingWindow.Owner = Window.GetWindow(this);
                    RegisterFloatingWindow(floatingWindow);
                    floatingWindow.Show();

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

#if DEBUG
            CheckHiddenState(content);
#endif
        }
        #endregion


        #region Anchor Style Update routines
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            //at the moment this is the easy way to get anchor properties always updated
            if (this.Content as ResizingPanel != null)
                UpdateAnchorStyle();

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
        FlyoutPaneWindow _flyoutWindow = null;
        WindowInteropWrapper _wndInteropWrapper = null;

        void HideAutoHideWindow()
        {
            if (_flyoutWindow != null)
            {
                _flyoutWindow.Height = 0.0;
                _flyoutWindow.Width = 0.0;
                _flyoutWindow.Close();
                _flyoutWindow = null;
            }
        }


        internal void ShowFlyoutWindow(DockableContent content)
        {
            //check if parent window is Active
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow == null)
                return;
            
            if (!parentWindow.IsActive && (_flyoutWindow == null || !_flyoutWindow.IsActive))
                return;


            //check if content is already visible in a flyout window
            if (_flyoutWindow != null &&
                _flyoutWindow.ReferencedPane.Items.Contains(content))
                return;

            //hide previous create window
            HideAutoHideWindow();

            //select this content in the referenced pane
            content.ContainerPane.SelectedItem = content;


            if (_wndInteropWrapper == null)
            {
                _wndInteropWrapper = new WindowInteropWrapper();
                _wndInteropWrapper.OnWindowPosChanging+=new EventHandler(_wndInteropWrapper_OnWindowPosChanging);
            }
            _wndInteropWrapper.AttachedObject = parentWindow;

            //create e new window
            _flyoutWindow = new FlyoutPaneWindow(this, content);
            _flyoutWindow.Owner = parentWindow;
            _flyoutWindow.FlowDirection = this.FlowDirection;
            
            UpdateFlyoutWindowPosition(true);

            _flyoutWindow.Closing += new System.ComponentModel.CancelEventHandler(_flyoutWindow_Closing);
            _flyoutWindow.Show();

            //this.Focus();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdateFlyoutWindowPosition();
            base.OnRenderSizeChanged(sizeInfo);
        }
        

        void _flyoutWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _flyoutWindow.Closing -= new System.ComponentModel.CancelEventHandler(_flyoutWindow_Closing);
            _flyoutWindow.Owner = null;
            _wndInteropWrapper.AttachedObject = null;
        }

        void UpdateFlyoutWindowPosition()
        {
            UpdateFlyoutWindowPosition(false);
        }

        void UpdateFlyoutWindowPosition(bool initialSetup)
        {
            if (_flyoutWindow == null)
                return;

            double leftTabsWidth = FlowDirection == FlowDirection.LeftToRight ? _leftAnchorTabPanel.ActualWidth : _rightAnchorTabPanel.ActualWidth;
            double rightTabsWidth = FlowDirection == FlowDirection.LeftToRight ? _rightAnchorTabPanel.ActualWidth : _leftAnchorTabPanel.ActualWidth;
            double topTabsHeight = _topAnchorTabPanel.ActualHeight;
            double bottomTabsHeight = _bottomAnchorTabPanel.ActualHeight;

            Point locDockingManager = HelperFunc.PointToScreenWithoutFlowDirection(this, new Point());
            double resWidth = initialSetup ? ResizingPanel.GetResizeWidth(_flyoutWindow.ReferencedPane).Value : _flyoutWindow.Width;
            double resHeight = initialSetup ? ResizingPanel.GetResizeHeight(_flyoutWindow.ReferencedPane).Value : _flyoutWindow.Height;
            //double resWidth = initialSetup ? ResizingPanel.GetEffectiveSize(_flyoutWindow.ReferencedPane).Width : _flyoutWindow.Width;
            //double resHeight = initialSetup ? ResizingPanel.GetEffectiveSize(_flyoutWindow.ReferencedPane).Height : _flyoutWindow.Height;

            _flyoutWindow.MinLeft = locDockingManager.X;
            _flyoutWindow.MinTop = locDockingManager.Y;
                
            if (_flyoutWindow.ReferencedPane.Anchor == AnchorStyle.Right)
            {
                _flyoutWindow.Top = locDockingManager.Y + topTabsHeight;
                _flyoutWindow.Height = this.ActualHeight - topTabsHeight - bottomTabsHeight;

                _flyoutWindow.MaxWidth = ActualWidth - rightTabsWidth;
                _flyoutWindow.MaxHeight = ActualHeight;

                if (initialSetup)
                {
                    _flyoutWindow.Left = FlowDirection == FlowDirection.LeftToRight ? locDockingManager.X + this.ActualWidth - rightTabsWidth : locDockingManager.X + leftTabsWidth;
                    _flyoutWindow.Width = 0.0;
                    _flyoutWindow.TargetWidth = resWidth;
                }
                else
                {
                    if (!_flyoutWindow.IsOpening && !_flyoutWindow.IsClosing)
                        _flyoutWindow.Left = FlowDirection == FlowDirection.LeftToRight ? locDockingManager.X + this.ActualWidth - rightTabsWidth - _flyoutWindow.Width : locDockingManager.X + leftTabsWidth;
                }
            }
            if (_flyoutWindow.ReferencedPane.Anchor == AnchorStyle.Left)
            {
                _flyoutWindow.Top = locDockingManager.Y + topTabsHeight;
                _flyoutWindow.Height = this.ActualHeight - topTabsHeight - bottomTabsHeight;

                _flyoutWindow.MaxWidth = ActualWidth - leftTabsWidth;
                _flyoutWindow.MaxHeight = ActualHeight;

                if (initialSetup)
                {
                    _flyoutWindow.Left = FlowDirection == FlowDirection.RightToLeft ? locDockingManager.X + this.ActualWidth - rightTabsWidth : locDockingManager.X + leftTabsWidth;
                    _flyoutWindow.Width = 0.0;
                    _flyoutWindow.TargetWidth = resWidth;
                }
                else
                {
                    if (!_flyoutWindow.IsOpening && !_flyoutWindow.IsClosing)
                        _flyoutWindow.Left = FlowDirection == FlowDirection.RightToLeft ? locDockingManager.X + this.ActualWidth - rightTabsWidth - _flyoutWindow.Width : locDockingManager.X + leftTabsWidth;
                }
            }
            if (_flyoutWindow.ReferencedPane.Anchor == AnchorStyle.Top)
            {
                _flyoutWindow.Left = locDockingManager.X + leftTabsWidth;
                _flyoutWindow.Width = this.ActualWidth - rightTabsWidth -leftTabsWidth;

                _flyoutWindow.MaxWidth = ActualWidth;
                _flyoutWindow.MaxHeight = ActualHeight - topTabsHeight;

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
                _flyoutWindow.Left = locDockingManager.X + leftTabsWidth;
                _flyoutWindow.Width = this.ActualWidth - rightTabsWidth - leftTabsWidth;

                _flyoutWindow.MaxWidth = ActualWidth;
                _flyoutWindow.MaxHeight = ActualHeight - bottomTabsHeight;

                if (initialSetup)
                {
                    _flyoutWindow.Top = locDockingManager.Y + this.ActualHeight - bottomTabsHeight;
                    _flyoutWindow.Height = 0.0;
                    _flyoutWindow.TargetHeight = resHeight;
                }
                else
                {
                    if (!_flyoutWindow.IsOpening && !_flyoutWindow.IsClosing)
                        _flyoutWindow.Top = locDockingManager.Y + this.ActualHeight - bottomTabsHeight - _flyoutWindow.Height;
                    if (_flyoutWindow.IsClosing)
                        _flyoutWindow.Top = locDockingManager.Y + this.ActualHeight - bottomTabsHeight - _flyoutWindow.Height; 
                }
            }

            if (_flyoutWindow != null && !_flyoutWindow.IsClosing)
                _flyoutWindow.UpdateClipRegion();
        }

        void  _wndInteropWrapper_OnWindowPosChanging(object sender, EventArgs e)
        {
            UpdateFlyoutWindowPosition();
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
                DocumentFloatingWindow floatingWindow = new DocumentFloatingWindow(this, documentContent);
                Drag(floatingWindow, point, offset);
            }

        }


        internal void Drag(DockableContent dockableContent, Point point, Point offset)
        {
            if (CaptureMouse())
            {
                DockableFloatingWindow floatingWindow = new DockableFloatingWindow(this, dockableContent);
                floatingWindow.Owner = Window.GetWindow(this);
                RegisterFloatingWindow(floatingWindow);
                Drag(floatingWindow, point, offset);
            }
        }


        internal void Drag(DockablePane dockablePane, Point point, Point offset)
        {
            if (CaptureMouse())
            {
                DockableFloatingWindow floatingWindow = new DockableFloatingWindow(this, dockablePane);
                floatingWindow.Owner = Window.GetWindow(this);
                RegisterFloatingWindow(floatingWindow);
                Drag(floatingWindow, point, offset);
            }
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                DragPaneServices.MoveDrag(this.PointToScreenDPI(e.GetPosition(this)));

                if (_flyoutWindow != null)
                    _flyoutWindow.UpdateClipRegion();
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
                if (_dragPaneServices == null)
                    _dragPaneServices = new DragPaneServices(this);

                return _dragPaneServices;
            }
        }
        #endregion


        #region IDropSurface
        public bool IsSurfaceVisible
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
        public Rect SurfaceRectangle
        {
            get
            { 
                if (PresentationSource.FromVisual(this) != null)
                    return new Rect(HelperFunc.PointToScreenWithoutFlowDirection(this, new Point(0, 0)), new Size(ActualWidth, ActualHeight));
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
                if (_overlayWindow == null)
                    _overlayWindow = new OverlayWindow(this);

                return _overlayWindow;
            }
        }

        /// <summary>
        /// Handles this sourface mouse entering (show current overlay window)
        /// </summary>
        /// <param name="point">Current mouse position</param>
        public void OnDragEnter(Point point)
        {
            if (OverlayWindow.IsVisible)
                return;

            OverlayWindow.Owner = DragPaneServices.FloatingWindow;
            //OverlayWindow.Left = PointToScreen(new Point(0, 0)).X;
            //OverlayWindow.Top = PointToScreen(new Point(0, 0)).Y;
            Point origPoint = HelperFunc.PointToScreenWithoutFlowDirection(this, new Point());
            OverlayWindow.Left = origPoint.X;
            OverlayWindow.Top = origPoint.Y;
            OverlayWindow.Width = ActualWidth;
            OverlayWindow.Height = ActualHeight;

            OverlayWindow.Show();
        }

        /// <summary>
        /// Handles mouse overing this surface
        /// </summary>
        /// <param name="point"></param>
        public void OnDragOver(Point point)
        {

        }

        /// <summary>
        /// Handles mouse leave event during drag (hide overlay window)
        /// </summary>
        /// <param name="point"></param>
        public void OnDragLeave(Point point)
        {
            OverlayWindow.Owner = null;
            OverlayWindow.Hide();
            Window mainWindow = Window.GetWindow(this);
            if (mainWindow != null)
                mainWindow.Activate();
        }

        /// <summary>
        /// Handler drop events
        /// </summary>
        /// <param name="point">Current mouse position</param>
        /// <returns>Returns alwasy false because this surface doesn't support direct drop</returns>
        public bool OnDrop(Point point)
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
            if (pane.Items.Count > 0)
            {
                xmlWriter.WriteStartElement("DockablePane");

                //if (!double.IsInfinity(ResizingPanel.GetResizeWidth(pane)))
                //    xmlWriter.WriteAttributeString("ResizeWidth", XmlConvert.ToString(ResizingPanel.GetResizeWidth(pane)));
                ////if (!double.IsInfinity(ResizingPanel.GetResizeHeight(pane)))
                //    xmlWriter.WriteAttributeString("ResizeHeight", XmlConvert.ToString(ResizingPanel.GetResizeHeight(pane)));
                xmlWriter.WriteAttributeString("ResizeWidth", ResizingPanel.GetResizeWidth(pane).ToString());
                xmlWriter.WriteAttributeString("ResizeHeight", ResizingPanel.GetResizeHeight(pane).ToString());
                xmlWriter.WriteAttributeString("EffectiveSize", new SizeConverter().ConvertToInvariantString(ResizingPanel.GetEffectiveSize(pane)));

                xmlWriter.WriteAttributeString("Anchor", pane.Anchor.ToString());

                Debug.Assert(pane.Items.Count > 0);

                foreach (DockableContent content in pane.Items)
                {
                    SaveLayout(xmlWriter, content);
                }


                xmlWriter.WriteEndElement();
            }
        }

        void SaveLayout(XmlWriter xmlWriter, DockableFloatingWindow flWindow)
        {
            xmlWriter.WriteStartElement("FloatingWinfow");
            xmlWriter.WriteAttributeString("IsDockableWindow", XmlConvert.ToString(flWindow.IsDockableWindow));

            xmlWriter.WriteAttributeString("Top", XmlConvert.ToString(flWindow.Top));
            xmlWriter.WriteAttributeString("Left", XmlConvert.ToString(flWindow.Left));
            xmlWriter.WriteAttributeString("Width", XmlConvert.ToString(flWindow.Width));
            xmlWriter.WriteAttributeString("Height", XmlConvert.ToString(flWindow.Height));
            
            
            SaveLayout(xmlWriter, flWindow.HostedPane);

            xmlWriter.WriteEndElement();
        }

        void SaveLayout(XmlWriter xmlWriter, DockableContent content)
        {
            Debug.Assert(!string.IsNullOrEmpty(content.Name));
            if (!string.IsNullOrEmpty(content.Name))
            {
                xmlWriter.WriteStartElement("DockableContent");

                xmlWriter.WriteAttributeString("Name", content.Name);
                xmlWriter.WriteAttributeString("AutoHide", XmlConvert.ToString(content.State == DockableContentState.AutoHide));

                content.SaveLayout(xmlWriter);

                xmlWriter.WriteEndElement();
            }
        }

        void SaveLayout(XmlWriter xmlWriter, DocumentPane pane)
        {
            xmlWriter.WriteStartElement("DocumentPanePlaceHolder");

            foreach (ManagedContent content in pane.Items)
            {
                if (content is DockableContent)
                {
                    DockableContent dockableContent = content as DockableContent;
                    SaveLayout(xmlWriter, dockableContent);
                }
            }

            xmlWriter.WriteEndElement();
        }

        void SaveLayout(XmlWriter xmlWriter, DocumentPaneResizingPanel panelToSerialize)
        {
            xmlWriter.WriteStartElement("DocumentPanePlaceHolder");

            List<DockableContent> listOfFoundContents = new List<DockableContent>();
            FindContents<DockableContent>(listOfFoundContents, panelToSerialize);

            foreach (DockableContent content in listOfFoundContents)
            {
                SaveLayout(xmlWriter, content);
            }


            xmlWriter.WriteEndElement();
        }
        
        void SaveLayout(XmlWriter xmlWriter, ResizingPanel panelToSerialize)
        {
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
                else if (child is DocumentPaneResizingPanel)
                    SaveLayout(xmlWriter, child as DocumentPaneResizingPanel);
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
        /// Send layout configuration to a <see cref="XmlTextWriter"/> object
        /// </summary>
        /// <param name="sw">Object which stores the xml</param>
        /// <remarks>The writer is not closed.</remarks>
        public void SaveLayout(XmlWriter sw)
        {
            if (!_isControlLoaded)
                throw new InvalidOperationException("Unable to serialize docking layout while DockingManager control is unloaded");

            sw.WriteStartElement("DockingManager");

            if (Content is ResizingPanel)
                SaveLayout(sw, Content as ResizingPanel);
            else if (Content is DocumentPane)
                SaveLayout(sw, Content as DocumentPane);
            else if (Content is DocumentPaneResizingPanel)
                SaveLayout(sw, Content as DocumentPaneResizingPanel);

            sw.WriteStartElement("Hidden");
            foreach (DockableContent content in _hiddenContents)
            {
                SaveLayout(sw, content);
            }
            sw.WriteEndElement();

            sw.WriteStartElement("Windows");
            foreach (DockableFloatingWindow flWindow in _floatingWindows)
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
                if ((content.Parent as DockablePane).Items.Count == 1)
                    ToggleAutoHide(content.Parent as DockablePane);
            }
            if (content.State == DockableContentState.DockableWindow ||
                content.State == DockableContentState.FloatingWindow)
            {
                if ((content.Parent as DockablePane).Items.Count == 1)
                {
                    FloatingWindow floatingWindow = Window.GetWindow(content) as FloatingWindow;
                    floatingWindow.Close();
                }
            }
            //this content can be hidden also if was contained in closed floating window 
            if (content.State == DockableContentState.Hidden)
                Show(content);

            content.DetachFromContainerPane();
        }

        void ShowAllHiddenContents()
        {
            while (_hiddenContents.Count > 0)
            {
                Debug.Assert(_hiddenContents[0].State == DockableContentState.Hidden);
                Show(_hiddenContents[0]);
            }
        }

        void RestoreDocumentPaneLayout(XmlElement childElement, out DocumentPane mainExistingDocumentPane, out DocumentPaneResizingPanel existingDocumentPanel)
        {
            mainExistingDocumentPane = (Content is DocumentPane) ? Content as DocumentPane : GetMainDocumentPane(Content as ResizingPanel);
            existingDocumentPanel = mainExistingDocumentPane.GetParentDocumentPaneResizingPanel();

            if (existingDocumentPanel != null)
            {
                if (existingDocumentPanel.Parent is ResizingPanel)
                {
                    ((ResizingPanel)existingDocumentPanel.Parent).RemoveChild(existingDocumentPanel);
                }
                else if (existingDocumentPanel.Parent is DockingManager)
                {
                    ((DockingManager)existingDocumentPanel.Parent).Content = null;
                }
            }
            else if (mainExistingDocumentPane != null)
            {
                if (mainExistingDocumentPane.Parent is ResizingPanel)
                {
                    ((ResizingPanel)mainExistingDocumentPane.Parent).RemoveChild(mainExistingDocumentPane);
                }
                else if (mainExistingDocumentPane.Parent is DockingManager)
                {
                    ((DockingManager)mainExistingDocumentPane.Parent).Content = null;
                }
            }

            foreach (XmlElement contentElement in childElement.ChildNodes)
            {
                if (contentElement.HasAttribute("Name"))
                {
                    foreach (DockableContent content in DockableContents)
                    {
                        if (content.Name == contentElement.GetAttribute("Name"))
                        {
                            DetachContentFromDockingManager(content);
                            mainExistingDocumentPane.Items.Add(content);
                            content.SetStateToDocument();
                            content.RestoreLayout(contentElement);
                            break;
                        }
                    }
                }
            }
        }

        ResizingPanel RestoreLayout(XmlElement panelElement, DockableContent[] dockableContents)
        {
            ResizingPanel panel = new ResizingPanel();

            if (panelElement.HasAttribute("Orientation"))
                panel.Orientation = (Orientation)Enum.Parse(typeof(Orientation), panelElement.GetAttribute("Orientation"));


            foreach (XmlElement childElement in panelElement.ChildNodes)
            {
                if (childElement.Name == "ResizingPanel")
                {
                    ResizingPanel childPanel = RestoreLayout(childElement, dockableContents);
                    if (childElement.HasAttribute("ResizeWidth"))
                        ResizingPanel.SetResizeWidth(childPanel, (GridLength)GLConverter.ConvertFromInvariantString(childElement.GetAttribute("ResizeWidth")));
                    if (childElement.HasAttribute("ResizeHeight"))
                        ResizingPanel.SetResizeHeight(childPanel, (GridLength)GLConverter.ConvertFromInvariantString(childElement.GetAttribute("ResizeHeight")));
                    if (childElement.HasAttribute("EffectiveSize"))
                        ResizingPanel.SetEffectiveSize(childPanel, (Size)(new SizeConverter()).ConvertFromInvariantString(childElement.GetAttribute("EffectiveSize")));
                    
                    panel.Children.Add(childPanel);
                }
                #region Restore DockablePane
                else if (childElement.Name == "DockablePane")
                {
                    DockablePane pane = new DockablePane();

                    if (childElement.HasAttribute("Anchor"))
                        pane.Anchor = (AnchorStyle)Enum.Parse(typeof(AnchorStyle), childElement.GetAttribute("Anchor"));
                    if (childElement.HasAttribute("ResizeWidth"))
                        ResizingPanel.SetResizeWidth(pane, (GridLength)GLConverter.ConvertFromInvariantString(childElement.GetAttribute("ResizeWidth")));
                    if (childElement.HasAttribute("ResizeHeight"))
                        ResizingPanel.SetResizeHeight(pane, (GridLength)GLConverter.ConvertFromInvariantString(childElement.GetAttribute("ResizeHeight")));
                    if (childElement.HasAttribute("EffectiveSize"))
                        ResizingPanel.SetEffectiveSize(pane, (Size)(new SizeConverter()).ConvertFromInvariantString(childElement.GetAttribute("EffectiveSize")));

            //storeWriter.WriteAttributeString(
            //    "EffectiveSize", new SizeConverter().ConvertToInvariantString(ResizingPanel.GetEffectiveSize(ContainerPane)));

                    bool toggleAutoHide = false;
                    foreach (XmlElement contentElement in childElement.ChildNodes)
                    {
                        if (contentElement.HasAttribute("Name"))
                        {
                            foreach (DockableContent content in dockableContents)
                            {
                                if (content.Name == contentElement.GetAttribute("Name"))
                                {
                                    DetachContentFromDockingManager(content);
                                    pane.Items.Add(content);
                                    content.SetStateToDock();
                                    if (contentElement.HasAttribute("AutoHide") &&
                                        XmlConvert.ToBoolean(contentElement.GetAttribute("AutoHide")) &&
                                        pane.Items.Count == 1)
                                        toggleAutoHide = true;

                                    //call custom layout persistence method
                                    content.RestoreLayout(contentElement);

                                    break;
                                }
                            }
                        }
                    }
                    if (pane.Items.Count > 0)
                    {
                        if (toggleAutoHide)
                            ToggleAutoHide(pane);

                        if (pane.Items.Count > 0)
                            panel.Children.Add(pane);
                    }
                } 
                #endregion
                #region Restore Contents inside a DocumentPane
                else if (childElement.Name == "DocumentPanePlaceHolder")
                {
                    DocumentPaneResizingPanel existingDocumentPanel = null;
                    DocumentPane mainExistingDocumentPane = null;

                    RestoreDocumentPaneLayout(childElement, out mainExistingDocumentPane, out existingDocumentPanel);

                    if (existingDocumentPanel != null)
                    {
                        panel.Children.Add(existingDocumentPanel);
                    }
                    else if (mainExistingDocumentPane != null)
                    {
                        panel.Children.Add(mainExistingDocumentPane);
                    }
                }
                
                #endregion
            }

            return panel;
        }


        public void RestoreLayout(Stream backendStream)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(backendStream);

            RestoreLayout(doc);
        }

        public void RestoreLayout(XmlReader reader)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(reader);

            RestoreLayout(doc);
        }

        public void RestoreLayout(string filename)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(filename);

            RestoreLayout(doc);
        }

        public void RestoreLayout(TextReader reader)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(reader);

            RestoreLayout(doc);
        }
   


        void RestoreLayout(XmlDocument doc)
        {
            if (!_isControlLoaded)
                throw new InvalidOperationException("Unable to deserialize a docking layout while DockingManager control is unloaded");

            if (doc.DocumentElement == null ||
                doc.DocumentElement.Name != "DockingManager")
            {
                Debug.Assert(false, "Layout file had not a valid structure!");
                return;
            }

            if (doc.DocumentElement.ChildNodes.Count != 3 ||
                (doc.DocumentElement.ChildNodes[0].Name != "ResizingPanel" && doc.DocumentElement.ChildNodes[0].Name != "DocumentPanePlaceHolder") ||
                doc.DocumentElement.ChildNodes[1].Name != "Hidden" ||
                doc.DocumentElement.ChildNodes[2].Name != "Windows")
            {
                Debug.Assert(false, "Layout file hasn't a valid structure!");
                return;
            }

            DockableContent[] actualContents = DockableContents;
            
            //show all hidden contents
            ShowAllHiddenContents();

            //restore main panel
            XmlElement rootElement = doc.DocumentElement.ChildNodes[0] as XmlElement;
            if (rootElement.Name == "ResizingPanel")
            {
                this.Content = RestoreLayout(rootElement, actualContents);
            }
            else if (rootElement.Name == "DocumentPanePlaceHolder")
            {
                DocumentPaneResizingPanel existingDocumentPanel = null;
                DocumentPane mainExistingDocumentPane = null;

                RestoreDocumentPaneLayout(rootElement, out mainExistingDocumentPane, out existingDocumentPanel);

                if (existingDocumentPanel != null)
                {
                    this.Content = existingDocumentPanel;
                }
                else if (mainExistingDocumentPane != null)
                {
                    this.Content = mainExistingDocumentPane;
                }
            }

            //restore hidden contents
            foreach (XmlElement hiddenContentElement in doc.DocumentElement.ChildNodes[1].ChildNodes)
            {
                foreach (DockableContent hiddenContent in actualContents)
                {
                    if (hiddenContentElement.GetAttribute("Name") == hiddenContent.Name
                        && hiddenContent.State != DockableContentState.Hidden)
                    {
                        Hide(hiddenContent);
                    }
                }
            }

            //restore floating windows
            foreach (XmlElement flWindowElement in doc.DocumentElement.ChildNodes[2].ChildNodes)
            {
                bool isDockableWindow = XmlConvert.ToBoolean(flWindowElement.GetAttribute("IsDockableWindow"));
                Point location = new Point(XmlConvert.ToDouble(flWindowElement.GetAttribute("Left")), XmlConvert.ToDouble(flWindowElement.GetAttribute("Top")));
                Size size = new Size(XmlConvert.ToDouble(flWindowElement.GetAttribute("Width")), XmlConvert.ToDouble(flWindowElement.GetAttribute("Height")));

                DockableFloatingWindow flWindow = new DockableFloatingWindow(this);
                flWindow.Left = location.X;
                flWindow.Top = location.Y;
                flWindow.Width = size.Width;
                flWindow.Height = size.Height;
                flWindow.Owner = Window.GetWindow(this);

                XmlElement paneElement = flWindowElement.ChildNodes[0] as XmlElement;

                FloatingDockablePane paneForFloatingWindow = new FloatingDockablePane(flWindow);
                if (paneElement.HasAttribute("ResizingWidth"))
                    ResizingPanel.SetResizeWidth(paneForFloatingWindow, (GridLength)GLConverter.ConvertFromInvariantString(paneElement.GetAttribute("ResizeWidth")));
                if (paneElement.HasAttribute("ResizingHeight"))
                    ResizingPanel.SetResizeHeight(paneForFloatingWindow, (GridLength)GLConverter.ConvertFromInvariantString(paneElement.GetAttribute("ResizeHeight")));
                paneForFloatingWindow.Anchor = (AnchorStyle)Enum.Parse(typeof(AnchorStyle), paneElement.GetAttribute("Anchor"));


                DockableContent contentToTransfer = null;
                foreach (XmlElement contentElement in paneElement.ChildNodes)
                {
                    #region Find the content to transfer
                    foreach (DockableContent content in actualContents)
                    {
                        if (contentElement.GetAttribute("Name") == content.Name)
                        {
                            contentToTransfer = content;
                            break;
                        }
                    } 
                    #endregion
                
                
                    if (contentToTransfer != null)
                    {
                        DetachContentFromDockingManager(contentToTransfer);
                        paneForFloatingWindow.Items.Add(contentToTransfer);
                        contentToTransfer.RestoreLayout(contentElement);
                    }
                }

                flWindow.HostedPane = paneForFloatingWindow;
                flWindow.IsDockableWindow = isDockableWindow;

                RegisterFloatingWindow(flWindow);

                flWindow.ApplyTemplate();
                flWindow.Show();

            }
        }

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

        #endregion
    }
}
