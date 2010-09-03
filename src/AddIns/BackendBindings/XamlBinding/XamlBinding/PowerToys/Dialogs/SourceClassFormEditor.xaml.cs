// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
