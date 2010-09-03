// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
