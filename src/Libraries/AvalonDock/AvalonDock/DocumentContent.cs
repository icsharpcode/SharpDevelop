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
using System.ComponentModel;
using System.Diagnostics;


namespace AvalonDock
{
    /// <summary>
    /// Represent a document which can be host by a <see cref="DocumentPane"/>.
    /// </summary>
    /// <remarks>A document is always hosted by a <see cref="DocumentPane"/> usually in the central area of <see cref="DockingManager"/>.
    /// It has limited dragging features becaus it can be only moved to an other <see cref="DocumentPane"/> and can't float as a separate window.
    /// You can access all documents within <see cref="DockingManager"/> with property <see cref="DockingManager.Documents"/>.</remarks>
    public class DocumentContent : ManagedContent
    {

        static DocumentContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentContent), new FrameworkPropertyMetadata(typeof(DocumentContent)));

            //Control.WidthProperty.OverrideMetadata(typeof(DocumentContent),
            //    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSizePropertyChanged), new CoerceValueCallback(CourceSizeToNaN)));
            //Control.HeightProperty.OverrideMetadata(typeof(DocumentContent),
            //    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSizePropertyChanged), new CoerceValueCallback(CourceSizeToNaN)));
        }

        public DocumentContent()
        {
            //base.PropertyChanged += new PropertyChangedEventHandler(DocumentContent_PropertyChanged);
        }

        protected override void OnContentLoaded()
        {
            //now the logical tree is up
            DockingManager manager = GetParentManager(null);

            //if can't find the manager there is a problem
            //if (manager == null)
            //    throw new InvalidOperationException(string.Format(
            //        "Unable to find DockingManager object in the logical tree of document '{0}'", Title));

            //manager.Documents.Add(this);

            base.OnContentLoaded();
        }

        protected override void OnContentUnloaded()
        {
            //if (Manager != null)
            //    Manager.Documents.Remove(this);

            base.OnContentUnloaded();
        }

        #region InfoTip

        /// <summary>
        /// InfoTip Dependency Property
        /// </summary>
        public static readonly DependencyProperty InfoTipProperty =
            DependencyProperty.Register("InfoTip", typeof(string), typeof(DocumentContent),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the InfoTip property.  This dependency property 
        /// indicates information text attached to the document content.
        /// </summary>
        /// <remarks>This text is usually displayed when users switch between documents and helps them to choose the right one.</remarks>
        public string InfoTip
        {
            get { return (string)GetValue(InfoTipProperty); }
            set { SetValue(InfoTipProperty, value); }
        }

        #endregion

        #region ContentTypeDescription

        /// <summary>
        /// ContentTypeDescription Dependency Property
        /// </summary>
        public static readonly DependencyProperty ContentTypeDescriptionProperty =
            DependencyProperty.Register("ContentTypeDescription", typeof(string), typeof(DocumentContent),
                new FrameworkPropertyMetadata((string)string.Empty));

        /// <summary>
        /// Gets or sets the ContentTypeDescription property.  This dependency property 
        /// indicates a text which describes the type of content contained in this document.
        /// </summary>
        public string ContentTypeDescription
        {
            get { return (string)GetValue(ContentTypeDescriptionProperty); }
            set { SetValue(ContentTypeDescriptionProperty, value); }
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating if this document can float over main window (VS2010 Feature).
        /// </summary>
        public bool IsFloatingAllowed
        {
            get { return (bool)GetValue(IsFloatingAllowedProperty); }
            set { SetValue(IsFloatingAllowedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFloatingAllowed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFloatingAllowedProperty =
            DependencyProperty.Register("IsFloatingAllowed", typeof(bool), typeof(DocumentContent), new PropertyMetadata(true));


        #region IsFloating

        /// <summary>
        /// IsFloating Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey IsFloatingPropertyKey
            = DependencyProperty.RegisterReadOnly("IsFloating", typeof(bool), typeof(DocumentContent),
                new FrameworkPropertyMetadata((bool)false));

        public static readonly DependencyProperty IsFloatingProperty
            = IsFloatingPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the IsFloating property.  This dependency property 
        /// indicates if the DocumentContent is floating inside an external window.
        /// </summary>
        public bool IsFloating
        {
            get { return (bool)GetValue(IsFloatingProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the IsFloating property.  
        /// This dependency property indicates if the DocumentContent is floating inside an external window.
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        internal void SetIsFloating(bool value)
        {
            SetValue(IsFloatingPropertyKey, value);
        }

        #endregion


        protected override void OnDragStart(Point ptMouse, Point ptRelativeMouse)
        {
            if (IsFloatingAllowed)
            {
                Manager.Drag(this, HelperFunc.PointToScreenWithoutFlowDirection(this, ptMouse), ptRelativeMouse);
            }

            base.OnDragStart(ptMouse, ptRelativeMouse);
        }

        protected override void OnDragMouseMove(object sender, MouseEventArgs e)
        {
            base.OnDragMouseMove(sender, e);
        }

        protected override void OnDragMouseLeave(object sender, MouseEventArgs e)
        {
            
            base.OnDragMouseLeave(sender, e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (DragEnabledArea != null)
            {
                if (string.IsNullOrEmpty((string)DragEnabledArea.ToolTip))
                    DragEnabledArea.ToolTip = InfoTip;
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.CommandBindings.Add(
                new CommandBinding(DocumentContentCommands.FloatingDocument, this.OnExecuteCommand, this.OnCanExecuteCommand));
            this.CommandBindings.Add(
                new CommandBinding(DocumentContentCommands.TabbedDocument, this.OnExecuteCommand, this.OnCanExecuteCommand));
        }

        void OnExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!e.Handled && e.Command == DocumentContentCommands.FloatingDocument)
            {
                this.Show(true);
                Activate();
                e.Handled = true;
            }
            else if (!e.Handled && e.Command == DocumentContentCommands.TabbedDocument)
            {
                this.Show(false);
                Activate();
                e.Handled = true;
            }
        }

        void OnCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanExecuteCommand(e.Command);
        }

        /// <summary>
        /// Show <see cref="DocumentContent"/> as tabbed document
        /// </summary>
        public override void Show()
        {
            Show(false);
        }

        /// <summary>
        /// Show <see cref="DocumentContent"/> as tabbed document inside the provided <see cref="DockingManager"/>
        /// </summary>
        /// <param name="manager">Docking manager target</param>
        public override void Show(DockingManager manager)
        {
            Show(manager, false);
        }

        /// <summary>
        /// Show <see cref="DocumentContent"/> as tabbed document or inside a floating window
        /// </summary>
        public void Show(bool showAsFloatingWindow)
        {
            if (!CanExecuteCommand(ManagedContentCommands.Show))
                throw new InvalidOperationException("This operation can be executed in this state");

            Manager.Show(this, showAsFloatingWindow);
        }

        /// <summary>
        /// Show <see cref="DocumentContent"/> as tabbed document inside the provided <see cref="DockingManager"/>
        /// </summary>
        /// <param name="manager">Docking manager target</param>
        /// <param name="showAsFloatingWindow">True if document should be shown inside a floating window (<see cref="DocumentFloatingWindow"/>)</param>
        public void Show(DockingManager manager, bool showAsFloatingWindow)
        {
            if (Manager != null && Manager != manager)
                throw new InvalidOperationException("Please remove the content from previous DockingManager (using the Close method)");

            if (!CanExecuteCommand(ManagedContentCommands.Show))
                throw new InvalidOperationException("This operation can be executed in this state");

            manager.Show(this, showAsFloatingWindow);

            manager.Documents.Add(this);
        }

        /// <summary>
        /// Activate the document showing its header if it's not visible
        /// </summary>
        public override void Activate()
        {
            base.Activate();

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

            //Active this content as the active document
            if (Manager != null)
                Manager.ActiveDocument = this;

            //ensure this content is rendered first
            Panel.SetZIndex(this, 2);
        }

        /// <summary>
        /// Retrive a value indicating if the command can be executed based to the dockable content state
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected override bool CanExecuteCommand(ICommand command)
        {
            if (command == DocumentContentCommands.FloatingDocument)
            {
                return !IsFloating && IsFloatingAllowed;
            }
            else if (command == DocumentContentCommands.TabbedDocument)
            {
                return IsFloating;
            }

            return true;
        }

        /// <summary>
        /// Close this content without notifications
        /// </summary>
        internal void InternalClose()
        {
            DockingManager manager = Manager;

            if (manager != null)
            {
                if (manager.ActiveContent == this)
                    manager.ActiveContent = null;

                if (manager.ActiveDocument == this)
                    manager.ActiveDocument = null;
            }           
            
            DocumentPane parentPane = ContainerPane as DocumentPane;
            FloatingDocumentPane floatingParentPane = ContainerPane as FloatingDocumentPane;

            if (floatingParentPane != null)
            {
                floatingParentPane.RemoveContent(0);
                if (floatingParentPane.FloatingWindow != null &&
                    !floatingParentPane.FloatingWindow.IsClosing)
                    floatingParentPane.FloatingWindow.Close();
            }
            else if (parentPane != null)
            {
                parentPane.Items.Remove(this);

                parentPane.CheckContentsEmpty();
            }
        }

        /// <summary>
        /// Close this document removing it from its parent container
        /// </summary>
        /// <remarks>Use this function to close a document and remove it from its parent container. Please note
        /// that if you simply remove it from its parent <see cref="DocumentPane"/> without call this method, events like
        /// <see cref="OnClosing"/>/<see cref="OnClosed"/> are not called.
        /// </remarks>
        public override bool Close()
        {
            if (!IsCloseable)
                return false;

            ////if documents are attached to an external source via DockingManager.DocumentsSource
            ////let application host handle the document closing by itself
            //if (Manager.DocumentsSource != null)
            //{
            //    //return Manager.FireRequestDocumentCloseEvent(this);
            //    Manager.HandleDocumentClose(this);
            //}


            CancelEventArgs e = new CancelEventArgs(false);
            OnClosing(e);
            
            if (e.Cancel)
                return false;

            DockingManager oldManager = Manager;

            if (oldManager != null)
                oldManager.FireDocumentClosingEvent(e);

            if (e.Cancel)
                return false;

            InternalClose();

            OnClosed();

            //if documents are attached to an external source via DockingManager.DocumentsSource
            //let application host handle the document closing by itself
            if (oldManager != null &&
                oldManager.DocumentsSource != null)
            {
                oldManager.HandleDocumentClose(this);
            }

            if (oldManager != null)
                oldManager.FireDocumentClosedEvent();


            Debug.Assert(Parent == null, "Parent MUST bu null after Doc is closed");
            return true;
        }

        /// <summary>
        /// Hide the <see cref="DocumentContent"/> (Close the document)
        /// </summary>
        /// <returns></returns>
        public override bool Hide()
        {
            return Close();
        }



        #region Save/RestoreLayout
        public override void SaveLayout(System.Xml.XmlWriter storeWriter)
        {
            base.SaveLayout(storeWriter);
        }

        public override void RestoreLayout(System.Xml.XmlElement contentElement)
        {
            base.RestoreLayout(contentElement);
        }
        #endregion
    }
}
