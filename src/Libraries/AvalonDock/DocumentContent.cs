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
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentContent), new FrameworkPropertyMetadata(typeof(DocumentContent)));

            //Control.WidthProperty.OverrideMetadata(typeof(DocumentContent),
            //    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSizePropertyChanged), new CoerceValueCallback(CourceSizeToNaN)));
            //Control.HeightProperty.OverrideMetadata(typeof(DocumentContent),
            //    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSizePropertyChanged), new CoerceValueCallback(CourceSizeToNaN)));
        }

        public DocumentContent()
        {
            base.PropertyChanged += new PropertyChangedEventHandler(DocumentContent_PropertyChanged);
        }



        DateTime _lastActivation = DateTime.MinValue;

        internal DateTime LastActivation
        {
            get { return _lastActivation; }
        }

        void DocumentContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsActiveContent")
            {
                if (IsActiveContent)
                    _lastActivation = DateTime.Now;
            }
        }

        static void OnSizePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        { }

        static object CourceSizeToNaN(DependencyObject sender, object value)
        {
            //return double.NaN;
            return value;
        }

        string _infoTip;
        
        /// <summary>
        /// Gets or sets the information text attached to the document content.
        /// </summary>
        /// <remarks>This text is usually displayed when users switch between documents and helps them to choose the right one.</remarks>
        public string InfoTip
        {
            get { return _infoTip; }
            set 
            { 
                _infoTip = value;
                NotifyPropertyChanged("InfoTip");
            }
        }

        string _contentTypeDescription;

        /// <summary>
        /// Gets or sets a text which describes the type of content contained in this document.
        /// </summary>
        public string ContentTypeDescription
        {
            get { return _contentTypeDescription; }
            set
            {
                _contentTypeDescription = value;
                NotifyPropertyChanged("ContentTypeDescription");
            }
        }

        bool _isFloatingAllowed;

        /// <summary>
        /// Gets or sets a value indicating if this document can float over main window (VS2010 Feature).
        /// </summary>
        public bool IsFloatingAllowed
        {
            get { return _isFloatingAllowed; }
            set
            {
                _isFloatingAllowed = value;
                NotifyPropertyChanged("IsFloatingAllowed");
            }
        }

        protected override void OnDragStart(Point ptMouse, Point ptRelativeMouse)
        {
            //Manager.Drag(this, this.PointToScreenDPI(ptMouse), ptRelativeMouse);
            Manager.Drag(this, HelperFunc.PointToScreenWithoutFlowDirection(this, ptMouse), ptRelativeMouse);                


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
                DragEnabledArea.InputBindings.Add(new InputBinding(ApplicationCommands.Close, new MouseGesture(MouseAction.MiddleClick)));
            }
        }

        /// <summary>
        /// Event fired when the document is about to be closed
        /// </summary>
        public event EventHandler<CancelEventArgs> Closing;

        /// <summary>
        /// Event fired when the document has been closed
        /// </summary>
        /// <remarks>Note that when a document is closed property like <see cref="ManagedContent.ContainerPane"/> or <see cref="ManagedContent.Manager"/> returns null.</remarks>
        public event EventHandler Closed;

        /// <summary>
        /// Ovveride this method to handle <see cref="DocumentContent.OnClosing"/> event.
        /// </summary>
        protected virtual void OnClosing(CancelEventArgs e)
        {
            if (Closing != null && !e.Cancel)
            {
                Closing(this, e);
            }
        }

        /// <summary>
        /// Ovveride this method to handle <see cref="DocumentContent.OnClose"/> event.
        /// </summary>
        protected virtual void OnClosed()
        {
            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Close this content without notifications
        /// </summary>
        internal void InternalClose()
        {


            DocumentPane parentPane = ContainerPane as DocumentPane;
            DockingManager manager = Manager;

            if (manager != null)
            {
                if (manager.ActiveContent == this)
                    manager.ActiveContent = null;

                if (manager.ActiveDocument == this)
                    manager.ActiveDocument = null;
            }           
            
            if (parentPane != null)
            {
                parentPane.Items.Remove(this);

                parentPane.CheckContentsEmpty();
            }
            else
            {
                FloatingDockablePane floatingParentPane = ContainerPane as FloatingDockablePane;
                if (floatingParentPane != null)
                {
                    floatingParentPane.RemoveContent(0);
                    if (floatingParentPane.FloatingWindow != null &&
                        !floatingParentPane.FloatingWindow.IsClosing)
                        floatingParentPane.FloatingWindow.Close();
                }
            }




        }

        /// <summary>
        /// Close this document removing it from its parent container
        /// </summary>
        /// <remarks>Use this function to close a document and remove it from its parent container. Please note
        /// that if you simply remove it from its parent <see cref="DocumentPane"/> without call this method, events like
        /// <see cref="OnClosing"/>/<see cref="OnClosed"/> are not called.
        /// <para>
        /// Note:<see cref="DockableContent"/> cannot be closed: AvalonDock simply hide a <see cref="DockableContent"/> removing all the reference to it.
        /// </para>
        /// </remarks>
        public bool Close()
        {
            if (!IsCloseable)
                return false;

            //if documents are attached to an external source via DockingManager.DocumentsSource
            //let application host handle the document closing by itself
            if (Manager.DocumentsSource != null)
            {
                return Manager.FireRequestDocumentCloseEvent(this);
            }


            CancelEventArgs e = new CancelEventArgs(false);
            OnClosing(e);
            
            if (e.Cancel)
                return false;

            DockingManager oldManager = Manager;

            if (Manager != null)
                Manager.FireDocumentClosingEvent(e);

            if (e.Cancel)
                return false;

            InternalClose();

            OnClosed();

            if (oldManager != null)
                oldManager.FireDocumentClosedEvent();


            //if (Parent != null)
            //    throw new InvalidOperationException();
            Debug.Assert(Parent == null, "Parent MUST bu null after Doc is closed");
            return true;
        }
     
   

    }
}
