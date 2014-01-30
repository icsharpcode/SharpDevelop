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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.XamlBinding.PowerToys.Dialogs
{
	/// <summary>
	/// Interaction logic for SourceClassFormEditor.xaml
	/// </summary>
	public partial class SourceClassFormEditor : Window
	{
		IClass selectedClass;
		
		class PropertyWrapper : INotifyPropertyChanged {
			bool isSelected;
			
			public IProperty Property { get; set; }
			public string Name { get; set; }
			
			public bool IsSelected {
				get { return isSelected; }
				set {
					this.isSelected = value;
					OnPropertyChanged(new PropertyChangedEventArgs("IsSelected"));
				}
			}
			
			public event PropertyChangedEventHandler PropertyChanged;
			
			protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
			{
				if (PropertyChanged != null) {
					PropertyChanged(this, e);
				}
			}
		}
		
		public SourceClassFormEditor(IClass selectedClass)
		{
			InitializeComponent();
			
			this.selectedClass = selectedClass;
			this.Title += selectedClass.Name;
			
			this.lsClassProperties.ItemsSource = selectedClass.Properties.Select(item => new PropertyWrapper() { Property = item, Name = item.Name, IsSelected = false });
		}
		
		void TxtColumnGroupCountTextChanged(object sender, TextChangedEventArgs e)
		{
			int columnCount;
			
			var item = this.lsClassProperties.ItemsSource.OfType<PropertyWrapper>().First();
			item.IsSelected = true;
			
			if (int.TryParse(txtColumnGroupCount.Text, out columnCount)) {
				if (columnCount < 1)
					return;
				int i = columnCount - 1;
				while (i < displayGrid.ColumnDefinitions.Count)
					displayGrid.ColumnDefinitions.RemoveAt(i);
				
				for (i = displayGrid.ColumnDefinitions.Count; i < columnCount; i++) {
					displayGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
					displayGrid.Children.Add(CreateGridSplitter(i));
				}
			}
		}
		
		static GridSplitter CreateGridSplitter(int column)
		{
			GridSplitter splitter = new GridSplitter() { Width = 5, HorizontalAlignment = HorizontalAlignment.Left };
			splitter.SetValue(Grid.ColumnProperty, column);
			return splitter;
		}
	}
}
