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
using System.Windows.Data;

#endregion

namespace ICSharpCode.Data.Core.UI.UserControls
{
    /// <summary>
    /// Interaction logic for DatabasesTreeView.xaml
    /// </summary>

    public partial class DatabasesTreeView : TreeView, INotifyPropertyChanged
    {
        #region Fields

        private ObservableCollection<IDatabase> _databases = new ObservableCollection<IDatabase>();
        private ObservableCollection<IDatabasesTreeViewAdditionalNode> _additionalNodes = new ObservableCollection<IDatabasesTreeViewAdditionalNode>();
        private bool _showCheckBoxes = false;

        #endregion

        #region Properties

        public ObservableCollection<IDatabase> Databases
        {
            get { return _databases; }
            set 
            {
                _databases = value;
                OnPropertyChanged("Databases");
            }
        }

        public ObservableCollection<IDatabasesTreeViewAdditionalNode> AdditionalNodes
        {
            get { return _additionalNodes; }
        }

        public bool ShowCheckBoxes
        {
            get { return _showCheckBoxes; }
            set { _showCheckBoxes = value; }
        }

        #endregion

        #region Constructor

        public DatabasesTreeView()
        {
            InitializeComponent();

            _additionalNodes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(AdditionalNodes_CollectionChanged);
        }

        #endregion

        #region Event handlers

        private void mnuAddDatabase_Click(object sender, RoutedEventArgs e)
        {
            ConnectionWizardWindow connectionWizardWindow = new ConnectionWizardWindow();
            connectionWizardWindow.Owner = Application.Current.MainWindow;
            connectionWizardWindow.ShowDialog();

            if (connectionWizardWindow.DialogResult.Value)
            {
                connectionWizardWindow.SelectedDatabase.LoadDatabase();
                
                if (Databases.FirstOrDefault(database => database.ConnectionString == connectionWizardWindow.SelectedDatabase.ConnectionString) == null)
                    Databases.Add(connectionWizardWindow.SelectedDatabase);
            }
        }

        private void mnuRefresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AdditionalNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (IDatabasesTreeViewAdditionalNode additionalNode in e.OldItems)
                {
                    TreeViewItem tvi = ItemContainerGenerator.ContainerFromItem(additionalNode) as TreeViewItem;

                    if (tvi != null)
                        Items.Remove(tvi);
                }
            }

            if (e.NewItems != null)
            {
                foreach (IDatabasesTreeViewAdditionalNode additionalNode in e.NewItems)
                {
                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Header = additionalNode.Name;
                    tvi.ItemTemplate = Application.Current.TryFindResource(additionalNode.DataTemplate) as DataTemplate;

                    Binding binding = new Binding("Items");
                    binding.Source = additionalNode;
                    tvi.SetBinding(TreeViewItem.ItemsSourceProperty, binding);

                    Items.Add(tvi);
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
