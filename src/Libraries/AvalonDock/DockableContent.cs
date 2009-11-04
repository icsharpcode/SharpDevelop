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
using System.Xml;

namespace AvalonDock
{
    /// <summary>
    /// Enumerates all the possible states of <see cref="DockableContent"/>
    /// </summary>
    public enum DockableContentState
    {
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
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(DockableContent), new FrameworkPropertyMetadata(typeof(DockableContent)));
        }

        public DockableContent()
        {
        }

        #region Drag content
        protected override void OnDragMouseMove(object sender, MouseEventArgs e)
        {
            base.OnDragMouseMove(sender, e);
        }

        protected override void OnDragStart(Point ptMouse, Point ptRelativeMouse)
        {
            if (DockableStyle != DockableStyle.None && 
                State != DockableContentState.AutoHide)
            {
                Manager.Drag(this, HelperFunc.PointToScreenWithoutFlowDirection(this, ptMouse), ptRelativeMouse);                
            }

            base.OnDragStart(ptMouse, ptRelativeMouse);
        }        
        
        #endregion

        #region State Properties & Events

        public delegate void DockableContentStateHandler(object sender, DockableContentState state);
        public event DockableContentStateHandler StateChanged;
		
        public DockableContentState State
        {
            get { return (DockableContentState)GetValue(StatePropertyKey.DependencyProperty); }
            //protected set { SetValue(StatePropertyKey, value); }
            protected set
            {
                SetValue(StatePropertyKey, value);
                if (StateChanged != null)
                    StateChanged(this, value);
            }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey StatePropertyKey =
            DependencyProperty.RegisterReadOnly("State", typeof(DockableContentState), typeof(DockableContent), new UIPropertyMetadata(DockableContentState.Docked));

        DockableStyle _dockableStyle = DockableStyle.Dockable;

        /// <summary>
        /// Get or sets a value that indicates how a dockable content can be dragged over and docked to a <see cref="DockingManager"/>
        /// </summary>
        public DockableStyle DockableStyle
        {
            get { return _dockableStyle; }
            set { _dockableStyle = value; }
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
                State == DockableContentState.Document ||
                State == DockableContentState.Docked ||
                State == DockableContentState.FloatingWindow);

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






        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.CommandBindings.Add(
                new CommandBinding(FloatingCommand, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(DockableCommand, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(ShowAsDocumentCommand, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(DockablePane.ToggleAutoHideCommand, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(HideCommand, this.OnExecuteCommand, this.OnCanExecuteCommand));
        }

        #region Commands

        static object syncRoot = new object();


        private static RoutedUICommand documentCommand = null;
        public static RoutedUICommand ShowAsDocumentCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == documentCommand)
                    {
                        documentCommand = new RoutedUICommand("Tabbed Document", "Document", typeof(DockableContent));
                    }
                }
                return documentCommand;
            }
        }

        private static RoutedUICommand dockableCommand = null;
        public static RoutedUICommand DockableCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == dockableCommand)
                    {
                        dockableCommand = new RoutedUICommand("Dockable", "Dockable", typeof(DockablePane));
                    }
                }
                return dockableCommand;
            }
        }

        private static RoutedUICommand floatingCommand = null;
        public static RoutedUICommand FloatingCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == floatingCommand)
                    {
                        floatingCommand = new RoutedUICommand("F_loating", "Floating", typeof(DockableContent));
                    }
                }
                return floatingCommand;
            }
        }

        private static RoutedUICommand hideCommand = null;
        public static RoutedUICommand HideCommand
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == hideCommand)
                    {
                        hideCommand = new RoutedUICommand("H_ide", "Hide", typeof(DockableContent));
                    }
                }
                return hideCommand;
            }
        }

        internal virtual void OnExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!e.Handled && 
                e.Command == DockablePane.ToggleAutoHideCommand)
            {
                if ((DockableStyle & DockableStyle.AutoHide) > 0)
                {
                    ((ContainerPane as DockablePane)).ToggleAutoHide();
                    e.Handled = true;
                }
            }
            else if (!e.Handled && e.Command == DockableContent.HideCommand)
            {
                Manager.Hide(this);
                
                e.Handled = true;
            }
            else if (!e.Handled && e.Command == DockableContent.FloatingCommand)
            {
                Manager.Show(this, DockableContentState.FloatingWindow);
                e.Handled = true;
            }
            else if (!e.Handled && e.Command == DockableContent.DockableCommand)
            {
                Manager.Show(this, DockableContentState.DockableWindow);
                e.Handled = true;
            }
            else if (!e.Handled && e.Command == DockableContent.ShowAsDocumentCommand)
            {
                Manager.Show(this, DockableContentState.Document);
                e.Handled = true;
            }

        }

        protected virtual void OnCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (State == DockableContentState.AutoHide)
            {
                if (e.Command == DockablePane.ToggleAutoHideCommand ||
                    e.Command == DockablePane.CloseCommand ||
                    e.Command == DockableContent.HideCommand)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            else
                e.CanExecute = true;
        }
        
        #endregion

        #region Operations on content


        /// <summary>
        /// Remove this content from its parent container pane
        /// </summary>
        /// <returns></returns>
        internal DockableContent DetachFromContainerPane()
        {
            if (ContainerPane != null)
            {
                int indexOfContent = ContainerPane.Items.IndexOf(this);
                return ContainerPane.RemoveContent(indexOfContent) as DockableContent;
            }

            return null;
        }

        DockableContentStateAndPosition _savedStateAndPosition = null;

        internal DockableContentStateAndPosition SavedStateAndPosition
        {
            get { return _savedStateAndPosition; }
        }

        internal void SaveCurrentStateAndPosition()
        {
            //if (State == DockableContentState.Docked)
            //{
                _savedStateAndPosition = new DockableContentStateAndPosition(
                    this);
            //}
            //else
            //{
            //    _savedStateAndPosition = null;
            //}
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
            _savedStateAndPosition = null;
        }

        #endregion

        #region Save/Restore Content Layout
        /// <summary>
        /// Save content specific layout settings
        /// </summary>
        /// <param name="storeWriter">Backend store writer</param>
        /// <remarks>Custom derived class can overloads this method to handle custom layout persistence.</remarks>
        public virtual void SaveLayout(XmlWriter storeWriter)
        {
            if (!FloatingWindowSize.IsEmpty)
            {
                storeWriter.WriteAttributeString(
                    "FloatingWindowSize", new SizeConverter().ConvertToInvariantString(FloatingWindowSize));
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
            }

        }

        /// <summary>
        /// Restore content specific layout settings
        /// </summary>
        /// <param name="storeReader">Saved xml element containg content layout settings</param>
        /// <remarks>Custom derived class must overload this method to restore custom layout settings previously saved trought <see cref="SaveLayout"/>.</remarks>
        public virtual void RestoreLayout(XmlElement contentElement)
        {
            if (contentElement.HasAttribute("FloatingWindowSize"))
                FloatingWindowSize = (Size)(new SizeConverter()).ConvertFromInvariantString(contentElement.GetAttribute("FloatingWindowSize"));


            Size effectiveSize = new Size(0d, 0d);
            if (contentElement.HasAttribute("EffectiveSize"))
            {
                // Store
                effectiveSize = (Size)(new SizeConverter()).ConvertFromInvariantString(contentElement.GetAttribute("EffectiveSize"));
            }

            ResizingPanel.SetEffectiveSize(this, effectiveSize);

            if (contentElement.HasAttribute("ChildIndex"))
            {
                _savedStateAndPosition = new DockableContentStateAndPosition(
                    ContainerPane,
                    int.Parse(contentElement.GetAttribute("ChildIndex")),
                    double.Parse(contentElement.GetAttribute("Width")),
                    double.Parse(contentElement.GetAttribute("Height")),
                    (AnchorStyle) Enum.Parse(typeof(AnchorStyle), contentElement.GetAttribute("Anchor")),
					(DockableContentState) Enum.Parse(typeof(DockableContentState), contentElement.GetAttribute("State"))
                    );
                    //contentElement.HasAttribute("State") ? (DockableContentState)Enum.Parse(typeof(DockableContentState), contentElement.GetAttribute("State") );
            }
        } 
        #endregion
    }
}
