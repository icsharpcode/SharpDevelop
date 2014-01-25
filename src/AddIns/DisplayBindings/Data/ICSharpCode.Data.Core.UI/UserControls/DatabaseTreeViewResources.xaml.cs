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
