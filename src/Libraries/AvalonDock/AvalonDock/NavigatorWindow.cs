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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AvalonDock
{
    /// <summary>
    /// Represent an navigator item within lists of contents that user can choose from the <see cref="NavigatorWindow"/>
    /// </summary>
    public class NavigatorWindowItem
    {
        private string _title;

        public string Title
        {
            get
            {
                return _title;
            }
        }

        private object _icon;

        public object Icon
        {
            get
            {
                return _icon;
            }
        }

        protected ManagedContent _content;

        public ManagedContent ItemContent
        {
            get { return _content; }
        }

		internal NavigatorWindowItem(ManagedContent content)
		{
			_title = content.Title;
			_icon = content.Icon;
			_content = content;
		}
    }

    /// <summary>
    /// Specialized class of <see cref="NavigatorWindowItem"/> for <see cref="DocumentContent"/> objects
    /// </summary>
    public class NavigatorWindowDocumentItem : NavigatorWindowItem
    {
        private string _infoTip;

        public string InfoTip
        {
            get
            {
                return _infoTip;
            }
        }

        private string _contentTypeDescription;

        public string ContentTypeDescription
        {
            get
            {
                return _contentTypeDescription;
            }
        }

        private DateTime _lastActivation;

        public DateTime LastActivation
        {
            get { return _lastActivation; }
        }


        internal NavigatorWindowDocumentItem(DocumentContent document)
            : base(document)
        {
            _infoTip = document.InfoTip;
            if (_infoTip == null && document.ToolTip != null && document.ToolTip is string)
                _infoTip = document.ToolTip.ToString();

            _contentTypeDescription = document.ContentTypeDescription;
            _lastActivation = document.LastActivation;
        }


    }

    /// <summary>
    /// Window that is automatically shown when user press Ctrl+Tab combination
    /// </summary>
    /// <remarks>This window allow user to rapidly select a <see cref="DockableContent"/> object or a <see cref="DocumentContent"/> object.
    /// When selected a content is also activate with the function <see cref="ManagedContent.Activate"/></remarks>
    public class NavigatorWindow : AvalonDockWindow
    {
        #region Constructors
        static NavigatorWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigatorWindow), new FrameworkPropertyMetadata(typeof(NavigatorWindow)));

            AllowsTransparencyProperty.OverrideMetadata(typeof(NavigatorWindow), new FrameworkPropertyMetadata(true));
            WindowStyleProperty.OverrideMetadata(typeof(NavigatorWindow), new FrameworkPropertyMetadata(WindowStyle.None));
            ShowInTaskbarProperty.OverrideMetadata(typeof(NavigatorWindow), new FrameworkPropertyMetadata(false));
            BackgroundProperty.OverrideMetadata(typeof(NavigatorWindow), new FrameworkPropertyMetadata(Brushes.Transparent));
        }

        DockingManager _manager;
        internal NavigatorWindow(DockingManager manager)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            _manager = manager;

            List<NavigatorWindowDocumentItem> docs = new List<NavigatorWindowDocumentItem>();
            _manager.Documents.ForEach((DocumentContent doc) =>
            {
                docs.Add(new NavigatorWindowDocumentItem(doc));
            });

            docs.Sort((NavigatorWindowDocumentItem item1, NavigatorWindowDocumentItem item2) =>
            {
                if (item1 == item2 ||
                    item1.LastActivation == item2.LastActivation)
                    return 0;
                return (item1.LastActivation < item2.LastActivation) ? 1 : -1;
            });

            SetDocuments(new CollectionView(docs));

            List<NavigatorWindowItem> cnts = new List<NavigatorWindowItem>();
            _manager.DockableContents.Where(c => c.State != DockableContentState.Hidden).ForEach((DockableContent cnt) =>
            {
                cnts.Add(new NavigatorWindowItem(cnt));
            });

            SetDockableContents(new CollectionView(cnts));

            Documents.MoveCurrentTo(Documents.OfType<NavigatorWindowDocumentItem>().FirstOrDefault(cntItem =>
            {
                return cntItem.ItemContent == _manager.ActiveDocument;
            }));

            DockableContents.MoveCurrentTo(null);

            Loaded += new RoutedEventHandler(NavigatorWindow_Loaded);
        }
        
        #endregion

        #region Handlers for Tab+ctrl keys events
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key != Key.Tab)
                Hide();
            else
            {
                e.Handled = true;
                MoveNextSelectedContent();
            }

            base.OnKeyDown(e);
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (e.Key != Key.Tab)
            {
                var docSelected = (Documents.CurrentItem as NavigatorWindowDocumentItem).ItemContent as DocumentContent;
                docSelected.Activate();
                Hide();
            }
            else
            {
                e.Handled = true;
            }

            base.OnPreviewKeyUp(e);
        }
        
        #endregion

        #region Current Document/Content changed
        void NavigatorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Documents.CurrentChanged += new EventHandler(Documents_CurrentChanged);
            DockableContents.CurrentChanged += new EventHandler(DockableContents_CurrentChanged);
        }


        void DockableContents_CurrentChanged(object sender, EventArgs e)
        {
            if (DockableContents.CurrentItem == null)
                return;

            Debug.WriteLine(string.Format("DockContent current changed to {0}", (DockableContents.CurrentItem as NavigatorWindowItem).ItemContent.Title));
            var dockCntSelected = (DockableContents.CurrentItem as NavigatorWindowItem).ItemContent as DockableContent;
            Hide();
            dockCntSelected.Activate();
        }

        void Documents_CurrentChanged(object sender, EventArgs e)
        {
            if (Documents.CurrentItem == null)
                return;
            if (_intMoveFlag)
                return;

            Debug.WriteLine(string.Format("Document current changed to {0}", (Documents.CurrentItem as NavigatorWindowItem).ItemContent.Title));

            var docSelected = (Documents.CurrentItem as NavigatorWindowDocumentItem).ItemContent as DocumentContent;
            docSelected.Activate();
            Hide();
        } 
        #endregion

        #region Documents

        /// <summary>
        /// Documents Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey DocumentsPropertyKey
            = DependencyProperty.RegisterReadOnly("Documents", typeof(CollectionView), typeof(NavigatorWindow),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DocumentsProperty
            = DocumentsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the Documents property.  This dependency property 
        /// indicates documents currently hosted by parent <see cref="DockingManager"/>.
        /// </summary>
        public CollectionView Documents
        {
            get { return (CollectionView)GetValue(DocumentsProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the Documents property.  
        /// This dependency property indicates documents currently hosted by parent <see cref="DockingManager"/>.
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        protected void SetDocuments(CollectionView value)
        {
            SetValue(DocumentsPropertyKey, value);
        }

        #endregion

        #region DockableContents

        /// <summary>
        /// DockableContents Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey DockableContentsPropertyKey
            = DependencyProperty.RegisterReadOnly("DockableContents", typeof(CollectionView), typeof(NavigatorWindow),
                new FrameworkPropertyMetadata((CollectionView)null));

        public static readonly DependencyProperty DockableContentsProperty
            = DockableContentsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the DockableContents property.  This dependency property 
        /// indicates dockable contents hosted in parent <see cref="DockingManager"/> object.
        /// </summary>
        public CollectionView DockableContents
        {
            get { return (CollectionView)GetValue(DockableContentsProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the DockableContents property.  
        /// This dependency property indicates dockable contents hosted in parent <see cref="DockingManager"/> object.
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        protected void SetDockableContents(CollectionView value)
        {
            SetValue(DockableContentsPropertyKey, value);
        }

        #endregion

        #region Move to Next document
        bool _intMoveFlag = false;
        public void MoveNextSelectedContent()
        {
            _intMoveFlag = true;
            if (!Documents.MoveCurrentToNext())
                Documents.MoveCurrentToFirst();
            if (Documents.IsCurrentAfterLast)
                Documents.MoveCurrentToFirst();
            _intMoveFlag = false;
        }
        #endregion
    }
}
