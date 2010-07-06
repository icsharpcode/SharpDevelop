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
using System.Xml;
using System.ComponentModel;
using System.Linq;

namespace AvalonDock
{
    /// <summary>
    /// Enumerates all the possible states of <see cref="DockableContent"/>
    /// </summary>
    public enum DockableContentState
    {
        /// <summary>
        /// Content is not associated with any <see cref="DockingManager"/> (Default State)
        /// </summary>
        None,

        /// <summary>
        /// Content is docked to a border of a <see cref="ResizingPanel"/> within as <see cref="DockingManager"/> control
        /// </summary>
        Docked,

        /// <summary>
        /// Content is hosted by a flyout window and is visible only when user move mouse over an anchor thumb located to a <see cref="DockingManager"/> controlo border
        /// </summary>
        AutoHide,

        /// <summary>
        /// Content is hosted by a floating window and user can redock is on its <see cref="DockingManager"/> container control
        /// </summary>
        DockableWindow,

        /// <summary>
        /// Content is hosted by a floating window that can't be docked to a its <see cref="DockingManager"/> container control
        /// </summary>
        FloatingWindow,

        /// <summary>
        /// Content is hosted into a <see cref="DocmumentPane"/>
        /// </summary>
        Document,

        /// <summary>
        /// Content is hidden
        /// </summary>
        Hidden,
    }


    /// <summary>
    /// Defines how a dockable content can be dragged over a docking manager
    /// </summary>
    /// <remarks>This style can be composed with the 'or' operator.</remarks>
    public enum DockableStyle : uint
    { 
        /// <summary>
        /// Content is not dockable at all
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Dockable as document
        /// </summary>
        Document    = 0x0001,

        /// <summary>
        /// Dockable to the left border of <see cref="DockingManager"/>
        /// </summary>
        LeftBorder  = 0x0002,

        /// <summary>
        /// Dockable to the right border of <see cref="DockingManager"/>
        /// </summary>
        RightBorder = 0x0004,

        /// <summary>
        /// Dockable to the top border of <see cref="DockingManager"/>
        /// </summary>
        TopBorder   = 0x0008,

        /// <summary>
        /// Dockable to the bottom border of <see cref="DockingManager"/>
        /// </summary>
        BottomBorder= 0x0010,

        /// <summary>
        /// A <see cref="DockableContent"/> with this style can be hosted in a <see cref="FloatingWindow"/>
        /// </summary>
        Floating = 0x0020,
        
        /// <summary>
        /// A <see cref="DockableContent"/> with this style can be the only one content in a <see cref="DockablePane"/> pane (NOT YET SUPPORTED)
        /// </summary>
        /// <remarks>This style is not compatible with <see cref="DockableStyle.Document"/> style</remarks>
        Single = 0x0040,

        /// <summary>
        /// A <see cref="DockableContet"/> with this style can be autohidden.
        /// </summary>
        AutoHide = 0x0080,

        /// <summary>
        /// Dockable only to a border of a <see cref="DockingManager"/>
        /// </summary>
        DockableToBorders = LeftBorder | RightBorder | TopBorder | BottomBorder | AutoHide,
        
        /// <summary>
        /// Dockable to a border of a <see cref="DockingManager"/> and into a <see cref="DocumentPane"/>
        /// </summary>
        Dockable = DockableToBorders | Document | Floating,

        /// <summary>
        /// Dockable to a border of a <see cref="DockingManager"/> and into a <see cref="DocumentPane"/> but not in autohidden mode (WinForms controls)
        /// </summary>
        DockableButNotAutoHidden = Dockable & ~AutoHide
    }


    /// <summary>
    /// Represent a state of a dockable content that can be used to restore it after it's hidden
    /// </summary>
    internal class DockableContentStateAndPosition
    {
        public readonly Pane ContainerPane = null;
        public readonly Guid ContainerPaneID;
        public readonly int ChildIndex = -1;
        public readonly double Width;
        public readonly double Height;
        public readonly DockableContentState State;
        public readonly AnchorStyle Anchor = AnchorStyle.None;

        public DockableContentStateAndPosition(
            Pane containerPane,
            int childIndex,
            double width,
            double height,
            AnchorStyle anchor,
			DockableContentState state)
        {
            ContainerPane = containerPane;
            ChildIndex = childIndex;
            Width = Math.Max(width, 100.0);
            Height = Math.Max(height, 100.0);
            Anchor = anchor;
			State = state;
        }

        public DockableContentStateAndPosition(
            Guid containerPaneID,
            int childIndex,
            double width,
            double height,
            AnchorStyle anchor,
            DockableContentState state)
        {
            ContainerPaneID = containerPaneID;
            ChildIndex = childIndex;
            Width = Math.Max(width, 100.0);
            Height = Math.Max(height, 100.0);
            Anchor = anchor;
            State = state;
        }

       public DockableContentStateAndPosition(
            DockableContent cntToSave)
        {
            ContainerPane = cntToSave.ContainerPane;
            ChildIndex = ContainerPane.Items.IndexOf(cntToSave);
            Width = Math.Max(ContainerPane.ActualWidth, 100.0);
            Height = Math.Max(ContainerPane.ActualHeight, 100.0);
            State = cntToSave.State;

            DockablePane dockablePane = ContainerPane as DockablePane;
            if (dockablePane != null)
            {
                Anchor = dockablePane.Anchor;
            }
        }
    }

    /// <summary>
    /// Identifies a content that can be drag over a <see cref="DockingManager"/> control or hosted by a window floating over it (<see cref="FloatingWindow"/>).
    /// </summary>
    public class DockableContent : ManagedContent
    {
        static DockableContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockableContent), new FrameworkPropertyMetadata(typeof(DockableContent)));
        }

        public DockableContent()
        {
        }

        protected override void OnContentLoaded()
        {
            //now the logical tree is up
            DockingManager manager = GetParentManager(null);

            //if can't find the manager there is a problem
            //if (manager == null)
            //    throw new InvalidOperationException("Unable to find DockingManager object in the logical tree");
            //if (manager != null)
            //    manager.DockableContents.Add(this);

            base.OnContentLoaded();
        }

        protected override void OnContentUnloaded()
        {
            //if (Manager != null && !Manager.HiddenContents.Contains(this))
            //    Manager.DockableContents.Remove(this);

            base.OnContentUnloaded();
        }

		SizeToContent _floatingWindowSizeToContent = SizeToContent.Manual;

		/// <summary>
		/// Gets or sets a value indicating if this dockable content should change the size of a FloatingWindow when undocked
		/// </summary>
		public SizeToContent FloatingWindowSizeToContent
		{
			get { return _floatingWindowSizeToContent; }
			set
			{
				_floatingWindowSizeToContent = value;
                RaisePropertyChanged("FloatingWindowSizeToContent");
			}
		}

        #region Drag content
        protected override void OnDragMouseMove(object sender, MouseEventArgs e)
        {
            base.OnDragMouseMove(sender, e);
        }

        protected override void OnDragStart(Point ptMouse, Point ptRelativeMouse)
        {
            if (DockableStyle != DockableStyle.None && 
                (State == DockableContentState.Docked || State == DockableContentState.Document || State == DockableContentState.DockableWindow) &&
                !Manager.DragPaneServices.IsDragging)
            {
                Manager.Drag(this, HelperFunc.PointToScreenWithoutFlowDirection(this, ptMouse), ptRelativeMouse);                
            }

            base.OnDragStart(ptMouse, ptRelativeMouse);
        }        
        
        #endregion

        #region State Properties & Events

        #region StateChanged

        /// <summary>
        /// StateChanged Routed Event
        /// </summary>
        public static readonly RoutedEvent StateChangedEvent = EventManager.RegisterRoutedEvent("StateChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DockableContent));

        /// <summary>
        /// Occurs when State property changes
        /// </summary>
        public event RoutedEventHandler StateChanged
        {
            add { AddHandler(StateChangedEvent, value); }
            remove { RemoveHandler(StateChangedEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the StateChanged event.
        /// </summary>
        protected RoutedEventArgs RaiseStateChangedEvent()
        {
            return RaiseStateChangedEvent(this);
        }

        /// <summary>
        /// A static helper method to raise the StateChanged event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        static RoutedEventArgs RaiseStateChangedEvent(DependencyObject target)
        {
            if (target == null) return null;

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = StateChangedEvent;
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion
       
        /// <summary>
        /// Gets the state of <see cref="DockableContent"/>
        /// </summary>
        public DockableContentState State
        {
            get { return (DockableContentState)GetValue(StatePropertyKey.DependencyProperty); }
            protected set { SetValue(StatePropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey StatePropertyKey =
            DependencyProperty.RegisterReadOnly("State",
            typeof(DockableContentState),
            typeof(DockableContent),
            new FrameworkPropertyMetadata(
                DockableContentState.None,
                new PropertyChangedCallback(
                    (s, e) =>
                    {
                        ((DockableContent)s).OnStateChanged((DockableContentState)e.OldValue, (DockableContentState)e.NewValue);
                    }
                    )));

        protected virtual void OnStateChanged(DockableContentState oldState, DockableContentState newState)
        {
            RaiseStateChangedEvent(this);

            if (ContainerPane is DockablePane)
                ((DockablePane)ContainerPane).UpdateCanAutohideProperty();
        }

        public static readonly DependencyProperty StateProperty =
            StatePropertyKey.DependencyProperty;

        #region DockableStyle

        /// <summary>
        /// DockableStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty DockableStyleProperty =
            DependencyProperty.Register("DockableStyle", typeof(DockableStyle), typeof(DockableContent),
                new FrameworkPropertyMetadata(DockableStyle.Dockable));

        /// <summary>
        /// Get or sets a value that indicates how a dockable content can be dragged over and docked to a <see cref="DockingManager"/>
        /// </summary>
        public DockableStyle DockableStyle
        {
            get { return (DockableStyle)GetValue(DockableStyleProperty); }
            set { SetValue(DockableStyleProperty, value); }
        }

        #endregion

        


        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {

            base.OnVisualParentChanged(oldParent);           
            
            //if (oldParent == null && State == DockableContentState.None)
            //{
            //    if (Parent is FloatingDockablePane)
            //    {
            //        throw new InvalidOperationException();
            //    }

            //    SetStateToDock();
            //}

        }
        

	    #endregion  

        #region StateMachine

        internal void SetStateToAutoHide()
        {
            State = DockableContentState.AutoHide;
        }

        internal void SetStateToDock()
        {
            State = DockableContentState.Docked;
        }

        internal void SetStateToDockableWindow()
        {
            if (State == DockableContentState.DockableWindow)
                return;

            Debug.Assert(
                State == DockableContentState.None ||
                State == DockableContentState.Document ||
                State == DockableContentState.Docked ||
                State == DockableContentState.FloatingWindow ||
                State == DockableContentState.Hidden);

            State = DockableContentState.DockableWindow;
        }

        internal void SetStateToFloatingWindow()
        {
            if (State == DockableContentState.FloatingWindow)
                return;

            Debug.Assert(
                State == DockableContentState.Document ||
                State == DockableContentState.Docked ||
                State == DockableContentState.DockableWindow);

            State = DockableContentState.FloatingWindow;
        }

        internal void SetStateToHidden()
        {
            State = DockableContentState.Hidden;
        }
        
        internal void SetStateToDocument()
        {
            State = DockableContentState.Document;
        }
        #endregion 

        #region HideOnClose
        public static DependencyProperty HideOnCloseKey = DependencyProperty.Register("HideOnClose", typeof(bool), typeof(DockableContent), new PropertyMetadata(true));

        public bool HideOnClose
        {
            get { return (bool)GetValue(HideOnCloseKey); }
            set { SetValue(HideOnCloseKey, value); }
        }
        #endregion

        //#region OnIsActiveContentChanged (flyout windows)
        //protected override void OnIsActiveContentChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (State == DockableContentState.AutoHide)
        //        Manager.ShowFlyoutWindow(this, null);

        //    base.OnIsActiveContentChanged(e);
        //}
        //#endregion
        
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.CommandBindings.Add(
                new CommandBinding(DockableContentCommands.FloatingWindow, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(DockableContentCommands.DockableFloatingWindow, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(ManagedContentCommands.Show, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(DockableContentCommands.ShowAsDocument, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(DockableContentCommands.ToggleAutoHide, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(ManagedContentCommands.Hide, this.OnExecuteCommand, this.OnCanExecuteCommand));
        }

        #region Commands

        void OnExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!e.Handled &&
                e.Command == DockableContentCommands.ToggleAutoHide)
            {
                ToggleAutoHide();

                e.Handled = true;
            }
            else if (!e.Handled && e.Command == DockableContentCommands.FloatingWindow)
            {
                ShowAsFloatingWindow(false);

                e.Handled = true;
            }
            else if (!e.Handled && e.Command == DockableContentCommands.DockableFloatingWindow)
            {
                ShowAsFloatingWindow(true);

                e.Handled = true;
            }
            else if (!e.Handled && e.Command == DockableContentCommands.ShowAsDocument)
            {
                ShowAsDocument();
                e.Handled = true;
            }
        }

        void OnCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanExecuteCommand(e.Command);
        }

        /// <summary>
        /// Show <see cref="DockableContent"/> as docked pane
        /// </summary>
        public override void Show()
        {
            Show(Manager);
        }

        /// <summary>
        /// Show <see cref="DockableContent"/> as docked pane within provided <see cref="DockingManager"/>
        /// </summary>
        public override void Show(DockingManager manager)
        {
            if (SavedStateAndPosition != null && State == DockableContentState.Hidden)
                Show(manager, SavedStateAndPosition.Anchor);
            else
                Show(manager, AnchorStyle.None);
        }

        /// <summary>
        /// Show <see cref="DockableContent"/> as docked pane
        /// </summary>
        /// <param name="desideredAnchor">Desidered anchor position</param>
        public void Show(AnchorStyle desideredAnchor)
        {
            Show(Manager, desideredAnchor);
        }

        /// <summary>
        /// Show <see cref="DockableContent"/> as docked pane
        /// </summary>
        public void Show(DockingManager manager, AnchorStyle desideredAnchor)
        {
            if (Manager != null && Manager != manager)
                throw new InvalidOperationException("Please remove the content from previous DockingManager (using the Close method)");

            if (manager == null && !CanExecuteCommand(ManagedContentCommands.Show))
                throw new InvalidOperationException("This operation can be executed in this state");

            if (State == DockableContentState.Docked)
            {
                //if already shown as docked content just activate it
                Activate();
            }
            else
            {
                if (SavedStateAndPosition != null && State == DockableContentState.Hidden)
                    manager.Show(this, SavedStateAndPosition.State, desideredAnchor);
                else
                    manager.Show(this, DockableContentState.Docked, desideredAnchor);
            }
        }

        /// <summary>
        /// Show as <see cref="DockableContent"/> as a tabbed document
        /// </summary>
        public void ShowAsDocument()
        {
            ShowAsDocument(Manager);
        }

        /// <summary>
        /// Show as <see cref="DockableContent"/> as a tabbed document under the provided <see cref="DockingManager"/>
        /// </summary>
        public void ShowAsDocument(DockingManager manager)
        {
            if (Manager != null && Manager != manager)
                throw new InvalidOperationException("Please remove the content from previous DockingManager (using the Close method)");

            //Manager = manager; 

            if (manager == null && !CanExecuteCommand(DockableContentCommands.ShowAsDocument))
                throw new InvalidOperationException("This operation can be executed in this state");

            manager.Show(this, DockableContentState.Document);
        }

        /// <summary>
        /// Show the content as floating window inside the provided <see cref="DockingManager"/>
        /// </summary>
        /// <param name="dockableWindow">True if the resulting floating window can the be re-docked to the docking manager.</param>
        public void ShowAsFloatingWindow(bool dockableWindow)
        {
            ShowAsFloatingWindow(Manager, dockableWindow);
        }

        /// <summary>
        /// Show the content ad floating window
        /// </summary>
        /// <param name="dockableWindow">True if the resulting floating window can the be re-docked to the docking manager.</param>
        public void ShowAsFloatingWindow(DockingManager manager, bool dockableWindow)
        {
            if (Manager != null && Manager != manager)
                throw new InvalidOperationException("Please remove the content from previous DockingManager (using the Close method)");

            //Manager = manager;

            if (manager == null)
            {
                if (dockableWindow &&
                    !CanExecuteCommand(DockableContentCommands.DockableFloatingWindow))
                    throw new InvalidOperationException("This operation can be executed in this state");
                if (!dockableWindow &&
                    !CanExecuteCommand(DockableContentCommands.FloatingWindow))
                    throw new InvalidOperationException("This operation can be executed in this state");
            }

            manager.Show(this, dockableWindow ? DockableContentState.DockableWindow : DockableContentState.FloatingWindow);
        }


        /// <summary>
        /// Hides this content
        /// </summary>
        public override bool Hide()
        {
            if (!CanExecuteCommand(ManagedContentCommands.Hide))
                return false;

            Manager.Hide(this);
            return true;
        }

        /// <summary>
        /// Close content
        /// </summary>
        public override bool Close()
        {
            return CloseOrHide();
        }

        /// <summary>
        /// Close content
        /// </summary>
        /// <param name="forceClose">If true forces the content closing regardless of the <see cref="HideOnClose"/> property.</param>
        public bool Close(bool forceClose)
        {
            return CloseOrHide(true);
        }

        /// <summary>
        /// Closes or hides provided content depending on HideOnClose property
        /// </summary>
        internal virtual bool CloseOrHide()
        {
            return CloseOrHide(false);
        }

        /// <summary>
        /// Closes or hides content depending on HideOnClose property
        /// </summary>
        internal virtual bool CloseOrHide(bool force)
        {
            if (!force &&
                !IsCloseable)
                return false; 
            
            if (HideOnClose)
            {
                Hide();
                return true;
            }
            else
            {
                if (!CanExecuteCommand(ManagedContentCommands.Close))
                    throw new InvalidOperationException("This operation can be executed in this state");

                CancelEventArgs e = new CancelEventArgs(false);
                OnClosing(e);

                if (e.Cancel)
                    return false;
                
                if (ContainerPane != null)
                {
                    ContainerPane.RemoveContent(
                        ContainerPane.Items.IndexOf(this));
                }

                OnClosed();
                return true;
            }            
        }

        /// <summary>
        /// Slides out this content to a border of the containing docking manager
        /// </summary>
        public void ToggleAutoHide()
        {
            if (!CanExecuteCommand(DockableContentCommands.ToggleAutoHide))
                throw new InvalidOperationException("This operation can be executed in this state");

            (ContainerPane as DockablePane).ToggleAutoHide();
        }

        public override void Activate()
        {
            if (State == DockableContentState.AutoHide)
            {
                if (Manager != null && this.IsLoaded)
                {
                    Manager.ShowFlyoutWindow(this, null);
                }
            }
            else if (State == DockableContentState.Document)
            {
                if (!DocumentTabPanel.GetIsHeaderVisible(this))
                {
                    DocumentPane parentPane = this.ContainerPane as DocumentPane;
                    if (parentPane != null &&
                        parentPane.GetManager() != null &&
                        parentPane.Items.IndexOf(this) != 0)
                    {
                        parentPane.Items.Remove(this);
                        parentPane.Items.Insert(0, this);
                        parentPane.SelectedIndex = 0;
                    }
                }            
            }
            base.Activate();
        }

        /// <summary>
        /// Retrive a value indicating if the command can be executed based to the dockable content state
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected override bool CanExecuteCommand(ICommand command)
        {
            //if (State == DockableContentState.None)
            //    return false;
            
            if (command == ManagedContentCommands.Hide)
            {
                if (State == DockableContentState.Hidden)
                {
                    return false;
                }
            }
            else if (command == DockableContentCommands.ShowAsDocument)
            {
                if (State == DockableContentState.Document)
                {
                    return false;
                }
            }
            else if (command == DockableContentCommands.FloatingWindow)
            {
                if (State == DockableContentState.FloatingWindow ||
                    State == DockableContentState.DockableWindow)
                {
                    return false;
                }
            }
            else if (command == DockableContentCommands.ToggleAutoHide)
            {
                if (State == DockableContentState.AutoHide ||
                    State == DockableContentState.Document ||
                    State == DockableContentState.FloatingWindow ||
                    State == DockableContentState.DockableWindow)
                {
                    return false;
                }
            }
            else if (command == DockableContentCommands.ShowAsDocument)
            {
                if (State == DockableContentState.Document)
                {
                    return false;
                }
            }
            else if (command == DockableContentCommands.DockableFloatingWindow)
            {
                if (State == DockableContentState.FloatingWindow ||
                    State == DockableContentState.DockableWindow)
                {
                    return false;
                }
            }
            else if (command == ManagedContentCommands.Show)
            {
                if (State == DockableContentState.Docked)
                {
                    return false;
                }
            }

            return base.CanExecuteCommand(command);
        }
        #endregion

        #region Operations on content




        DockableContentStateAndPosition _savedStateAndPosition = null;

        internal DockableContentStateAndPosition SavedStateAndPosition
        {
            get { return _savedStateAndPosition; }
            set { _savedStateAndPosition = value; }
        }

        internal void SaveCurrentStateAndPosition()
        {
            SavedStateAndPosition = new DockableContentStateAndPosition(
                this);
        }

        /// <summary>
        /// Reset internal state and position of the content
        /// </summary>
        /// <remarks>After a <see cref="DockableContent"/> is hidden AvalonDock save its state and position in order to
        /// restore it correctly when user wants to reshow it calling <see cref="DockingManager.Show"/> function. Call this method
        /// if you want to reset these data and provide your state and anchor style calling one of the overloads of the function
        /// <see cref="DockingManager.Show"/>.</remarks>
        public void ResetSavedStateAndPosition()
        {
            SavedStateAndPosition = null;
        }

        #endregion

        #region Save/Restore Content Layout
        /// <summary>
        /// Save content specific layout settings
        /// </summary>
        /// <param name="storeWriter">Backend store writer</param>
        /// <remarks>Custom derived class can overloads this method to handle custom layout persistence.</remarks>
        public override void SaveLayout(XmlWriter storeWriter)
        {
            if (!FloatingWindowSize.IsEmpty)
            {
                storeWriter.WriteAttributeString(
                    "FloatingWindowSize", new SizeConverter().ConvertToInvariantString(FloatingWindowSize));
            }

            if (!FlyoutWindowSize.IsEmpty)
            {
                storeWriter.WriteAttributeString(
                    "FlyoutWindowSize", new SizeConverter().ConvertToInvariantString(FlyoutWindowSize));
            }

            if (SavedStateAndPosition != null)
            {
                storeWriter.WriteAttributeString(
                    "ChildIndex", SavedStateAndPosition.ChildIndex.ToString());
                storeWriter.WriteAttributeString(
                    "Width", SavedStateAndPosition.Width.ToString());
                storeWriter.WriteAttributeString(
                    "Height", SavedStateAndPosition.Height.ToString());
                storeWriter.WriteAttributeString(
                    "Anchor", SavedStateAndPosition.Anchor.ToString());
                storeWriter.WriteAttributeString(
                    "State", SavedStateAndPosition.State.ToString());

                if (SavedStateAndPosition.ContainerPane is DockablePane)
                {
                    Guid idToSave = (SavedStateAndPosition.ContainerPane as DockablePane).ID;
                    storeWriter.WriteAttributeString(
                        "ContainerPaneID", idToSave.ToString());
                }
            }

        }

        /// <summary>
        /// Restore content specific layout settings
        /// </summary>
        /// <param name="storeReader">Saved xml element containg content layout settings</param>
        /// <remarks>Custom derived class must overload this method to restore custom layout settings previously saved trought <see cref="SaveLayout"/>.</remarks>
        public override void RestoreLayout(XmlElement contentElement)
        {
            if (contentElement.HasAttribute("FloatingWindowSize"))
                FloatingWindowSize = (Size)(new SizeConverter()).ConvertFromInvariantString(contentElement.GetAttribute("FloatingWindowSize"));
            if (contentElement.HasAttribute("FlyoutWindowSize"))
                FlyoutWindowSize = (Size)(new SizeConverter()).ConvertFromInvariantString(contentElement.GetAttribute("FlyoutWindowSize"));

            Size effectiveSize = new Size(0d, 0d);
            if (contentElement.HasAttribute("EffectiveSize"))
            {
                // Store
                effectiveSize = (Size)(new SizeConverter()).ConvertFromInvariantString(contentElement.GetAttribute("EffectiveSize"));
            }

            ResizingPanel.SetEffectiveSize(this, effectiveSize);

            if (contentElement.HasAttribute("ChildIndex"))
            {
                Pane paneRef = null;
                Guid containerPaneGuid = Guid.Empty;
                if (contentElement.HasAttribute("ContainerPaneID"))
                {
                    containerPaneGuid = new Guid(contentElement.GetAttribute("ContainerPaneID"));

                    if (Manager != null)
                    { 
                        ILinqToTree<DependencyObject> itemFound = new LogicalTreeAdapter(Manager).Descendants().FirstOrDefault(el => el.Item is DockablePane && ((el.Item as DockablePane).ID == containerPaneGuid));
                        paneRef = itemFound != null ? itemFound.Item as DockablePane : null;
                    }
                }

                if (paneRef != null)
                {
                    _savedStateAndPosition = new DockableContentStateAndPosition(
                        paneRef,
                        int.Parse(contentElement.GetAttribute("ChildIndex")),
                        double.Parse(contentElement.GetAttribute("Width")),
                        double.Parse(contentElement.GetAttribute("Height")),
                        (AnchorStyle)Enum.Parse(typeof(AnchorStyle), contentElement.GetAttribute("Anchor")),
                        (DockableContentState)Enum.Parse(typeof(DockableContentState), contentElement.GetAttribute("State")));
                }
                else
                {
                    _savedStateAndPosition = new DockableContentStateAndPosition(
                       containerPaneGuid,
                       int.Parse(contentElement.GetAttribute("ChildIndex")),
                       double.Parse(contentElement.GetAttribute("Width")),
                       double.Parse(contentElement.GetAttribute("Height")),
                       (AnchorStyle)Enum.Parse(typeof(AnchorStyle), contentElement.GetAttribute("Anchor")),
                       (DockableContentState)Enum.Parse(typeof(DockableContentState), contentElement.GetAttribute("State")));
                }
            }
        } 
        #endregion

        #region FlyoutWindowSize

        /// <summary>
        /// FlyoutWindowSize Dependency Property
        /// </summary>
        public static readonly DependencyProperty FlyoutWindowSizeProperty =
            DependencyProperty.Register("FlyoutWindowSize", typeof(Size), typeof(DockableContent),
                new FrameworkPropertyMetadata((Size)Size.Empty,
                    new PropertyChangedCallback(OnFlyoutWindowSizeChanged),
                    new CoerceValueCallback(CoerceFlyoutWindowSizeValue)));

        /// <summary>
        /// Gets or sets the FlyoutWindowSize property.  This dependency property 
        /// indicates size of the window hosting this content when is in auto-hidden state.
        /// This property is persisted when layout of the container <see cref="DockingManager"/> is saved.
        /// </summary>
        public Size FlyoutWindowSize
        {
            get { return (Size)GetValue(FlyoutWindowSizeProperty); }
            set { SetValue(FlyoutWindowSizeProperty, value); }
        }

        /// <summary>
        /// Handles changes to the FlyoutWindowSize property.
        /// </summary>
        private static void OnFlyoutWindowSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }


        /// <summary>
        /// Coerces the FlyoutWindowSize value.
        /// </summary>
        private static object CoerceFlyoutWindowSizeValue(DependencyObject d, object value)
        {
            //coerces size to 100.0-100.0
            Size newSize = (Size)value;

            newSize.Width = Math.Max(100.0, newSize.Width);
            newSize.Height = Math.Max(100.0, newSize.Height);

            return newSize;
        }

        #endregion



    }
}
