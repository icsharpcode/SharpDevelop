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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace AvalonDock
{
    public class DocumentNavigatorWindow : AvalonDockWindow, INotifyPropertyChanged
    {
        static DocumentNavigatorWindow()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentNavigatorWindow), new FrameworkPropertyMetadata(typeof(DocumentNavigatorWindow)));


            AllowsTransparencyProperty.OverrideMetadata(typeof(DocumentNavigatorWindow), new FrameworkPropertyMetadata(true));
            WindowStyleProperty.OverrideMetadata(typeof(DocumentNavigatorWindow), new FrameworkPropertyMetadata(WindowStyle.None));
            ShowInTaskbarProperty.OverrideMetadata(typeof(DocumentNavigatorWindow), new FrameworkPropertyMetadata(false));
            BackgroundProperty.OverrideMetadata(typeof(DocumentNavigatorWindow), new FrameworkPropertyMetadata(Brushes.Transparent));
        }

        public static object Theme;

        internal DocumentNavigatorWindow()
        {
        }

        void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Tab)
                CloseThisWindow();//Hide();
            else
            {
                e.Handled = true;
                MoveNextSelectedContent();
            }
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Tab)
                CloseThisWindow();//Hide();
            else
            {
                e.Handled = true;
            }
        }

        void CloseThisWindow()
        {
            Window wndParent = this.Owner;
            Close();
            wndParent.Activate();
        }

        DockingManager _manager;
        public DocumentNavigatorWindow(DockingManager manager)
            :this()
        {
            _manager = manager;
            Keyboard.AddKeyUpHandler(this, new KeyEventHandler(this.OnKeyUp));
            Keyboard.AddKeyDownHandler(this, new KeyEventHandler(this.OnKeyDown));
        }


        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            //List<DocumentContent> listOfDocuments = _manager.FindContents<DocumentContent>();
            List<NavigatorWindowDocumentItem> docs = new List<NavigatorWindowDocumentItem>();
            _manager.Documents.ForEach((DocumentContent doc) =>
            {
                docs.Add(new NavigatorWindowDocumentItem(doc));
            });

            //docs.Sort((NavigatorWindowDocumentItem item1, NavigatorWindowDocumentItem item2) =>
            //{
            //    if (item1 == item2 ||
            //        item1.LastActivation == item2.LastActivation)
            //        return 0;
            //    return (item1.LastActivation < item2.LastActivation) ? 1 : -1;
            //});

            Documents = docs;

            _internalSelect = true;

            SelectedContent = Documents.Find((NavigatorWindowDocumentItem docItem) =>
            {
                return docItem.ItemContent == _manager.ActiveDocument;
            });

            _internalSelect = false;
        }

        protected override void OnDeactivated(EventArgs e)
        {
            if (_manager != null)
            {
                Window mainWindow = Window.GetWindow(_manager);
                if (mainWindow != null)
                {
                    mainWindow.Activate();
                    if (SelectedContent != null)
                    {
                        _manager.Show(SelectedContent.ItemContent as DocumentContent);
                        SelectedContent.ItemContent.Activate();
                    }
                }
            }

            if (!_isClosing)
                CloseThisWindow();

            base.OnDeactivated(e);
        }

        

        ListBox _itemsControl;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _itemsControl = GetTemplateChild("PART_ScrollingPanel") as ListBox;
        }

        List<NavigatorWindowDocumentItem> _documents = new List<NavigatorWindowDocumentItem>();

        public List<NavigatorWindowDocumentItem> Documents
        {
            get { return _documents; }
            private
            set
            {
                _documents = value;
                NotifyPropertyChanged("Documents");
            }
        }

        NavigatorWindowDocumentItem _selectedContent;

        bool _internalSelect = false;

        public NavigatorWindowDocumentItem SelectedContent
        {
            get
            {
                return _selectedContent;
            }
            set
            {
                if (_selectedContent != value)
                {
                    _selectedContent = value;
                    NotifyPropertyChanged("SelectedContent");

                    if (!_internalSelect && _selectedContent != null)
                        CloseThisWindow();//Hide();
                    
                    if (_internalSelect && _itemsControl != null)
                        _itemsControl.ScrollIntoView(_selectedContent);
                }
            }
        }


        public void MoveNextSelectedContent()
        {
            if (_selectedContent == null)
                return;

            if (Documents.Contains(SelectedContent))
            {
                int indexOfSelecteContent = Documents.IndexOf(_selectedContent);

                if (indexOfSelecteContent == Documents.Count - 1)
                {
                    indexOfSelecteContent = 0;
                }
                else
                    indexOfSelecteContent++;
                
                _internalSelect = true;
                SelectedContent = Documents[indexOfSelecteContent];
                _internalSelect = false;
            }
        }

        bool _isClosing = false;
        protected override void OnClosing(CancelEventArgs e)
        {
            _isClosing = true;
            
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            //reset documents list to avoid WPF Bug:
            //http://social.msdn.microsoft.com/forums/en/wpf/thread/f3fc5b7e-e035-4821-908c-b6c07e5c7042/
            //http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=321955
            Documents = new List<NavigatorWindowDocumentItem>();
            
            base.OnClosed(e);
        }
        

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}
