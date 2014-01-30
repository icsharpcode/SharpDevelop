// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#region Usings

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using ICSharpCode.Data.Core.Interfaces;
using ICSharpCode.Data.Core.UI.Windows;
using ICSharpCode.SharpDevelop.Gui;

#endregion

namespace ICSharpCode.Data.Core.UI.UserControls
{
    /// <summary>
    /// Interaction logic for DatabasesTreeView.xaml
    /// </summary>

    public partial class DatabaseTreeView : TreeView, INotifyPropertyChanged
    {
        #region Fields

        public static readonly DependencyProperty DatabaseProperty =
            DependencyProperty.Register("Database", typeof(IDatabase), typeof(DatabaseTreeView), new FrameworkPropertyMetadata(null));

        private bool _showCheckBoxes = false;

        #endregion

        #region Properties

        public IDatabase Database
        {
            get { return (IDatabase)GetValue(DatabaseProperty); }
            set { SetValue(DatabaseProperty, value); }
        }

        public bool ShowCheckBoxes
        {
            get { return _showCheckBoxes; }
            set { _showCheckBoxes = value; }
        }

        #endregion

        #region Constructor

        public DatabaseTreeView()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void chkIsSelected_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                if (checkBox.Tag is IDatabaseObjectsCollection)
                {
                    IDatabaseObjectsCollection collection = checkBox.Tag as IDatabaseObjectsCollection;

                    foreach (IDatabaseObjectBase item in collection.NonGenericItems)
                    {
                        if (checkBox.IsChecked.HasValue && checkBox.IsChecked.Value)
                            item.IsSelected = true;
                        else
                            item.IsSelected = false;
                    }
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
