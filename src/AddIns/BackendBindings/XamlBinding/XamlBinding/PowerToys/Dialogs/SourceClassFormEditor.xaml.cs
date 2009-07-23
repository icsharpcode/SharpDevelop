// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.XamlBinding.PowerToys.Dialogs
{
	/// <summary>
	/// Interaction logic for SourceClassFormEditor.xaml
	/// </summary>
	public partial class SourceClassFormEditor : Window
	{
		IClass selectedClass;
		
		public SourceClassFormEditor(IClass selectedClass)
		{
			InitializeComponent();
			
			this.selectedClass = selectedClass;
			this.Title += selectedClass.Name;
			
			this.lsClassProperties.ItemsSource = selectedClass.Properties.Select(item => new { Property = item, Name = item.Name, IsSelected = false });
		}
		
		void TxtColumnGroupCountTextChanged(object sender, TextChangedEventArgs e)
		{
			int columnCount;
			
			if (int.TryParse(txtColumnGroupCount.Text, out columnCount)) {
				for (int i = displayGrid.ColumnDefinitions.Count; i < columnCount; i++) {
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