// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
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
using ICSharpCode.Data.Core.DatabaseObjects;
using ICSharpCode.Data.Core.Interfaces;

#endregion

namespace ICSharpCode.Data.Core.UI.UserControls
{
    /// <summary>
    /// Interaction logic for DatabaseTreeViewResources.xaml
    /// </summary>
    public partial class DatabaseTreeViewResources : ResourceDictionary
    {
        public DatabaseTreeViewResources()
        {
            InitializeComponent();
        }

        private void DatabaseObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
            {
                DragDropEffects allowedEffects = DragDropEffects.Scroll | DragDropEffects.Copy;
                DragDrop.DoDragDrop(frameworkElement, frameworkElement.DataContext, allowedEffects);
            }
        }

        private void chkStandardTemplateIsSelected_Click(object sender, RoutedEventArgs e)
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
    }
}
