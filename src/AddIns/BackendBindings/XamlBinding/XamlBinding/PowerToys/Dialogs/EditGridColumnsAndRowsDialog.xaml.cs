// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
using System.Windows.Shapes;
using System.Xml.Linq;

namespace ICSharpCode.XamlBinding.PowerToys.Dialogs
{
	/// <summary>
	/// Interaction logic for EditGridColumnsAndRowsDialog.xaml
	/// </summary>
	public partial class EditGridColumnsAndRowsDialog : Window
	{
		XElement gridTree;
		
		public EditGridColumnsAndRowsDialog(XElement gridTree)
		{
			InitializeComponent();
			
			this.gridTree = gridTree;
			RebuildGrid();
		}
		
		void BtnCancelClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
		
		void BtnOKClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
		
		void RebuildGrid()
		{
			this.gridDisplay.Children.Clear();
			this.gridDisplay.RowDefinitions.Clear();
			this.gridDisplay.ColumnDefinitions.Clear();
			
			XName rowDefName = XName.Get("Grid.RowDefinitions", CompletionDataHelper.WpfXamlNamespace);
			XName colDefName = XName.Get("Grid.ColumnDefinitions", CompletionDataHelper.WpfXamlNamespace);
			
			var rows = (gridTree.Element(rowDefName) ?? new XElement(rowDefName)).Elements().ToList();
			var cols = (gridTree.Element(colDefName) ?? new XElement(colDefName)).Elements().ToList();
			
			var rowDefintion = new XElement(XName.Get("RowDefinition", CompletionDataHelper.WpfXamlNamespace));
			var colDefintion = new XElement(XName.Get("ColumnDefinition", CompletionDataHelper.WpfXamlNamespace));
			
			rowDefintion.SetAttributeValue("Height", "Auto");
			colDefintion.SetAttributeValue("Width", "Auto");
			
			if (rows.Count == 0)
				rows.Add(rowDefintion);
			if (cols.Count == 0)
				rows.Add(colDefintion);
			
			for (int i = 0; i < rows.Count; i++) {
				this.gridDisplay.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
				
				for (int j = 0; j < cols.Count; j++) {
					this.gridDisplay.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
					
					Rectangle displayRect = new Rectangle() {
						Stroke = Brushes.Black, 
						StrokeThickness = 2, 
						Margin = new Thickness(5), 
						Fill = Brushes.CornflowerBlue, 
						VerticalAlignment = VerticalAlignment.Stretch, 
						HorizontalAlignment = HorizontalAlignment.Stretch
					};
					
					displayRect.SetValue(Grid.ColumnProperty, j);
					displayRect.SetValue(Grid.RowProperty, i);
					
					this.gridDisplay.Children.Add(displayRect);
				}
			}
			
			this.InvalidateVisual();
		}
	}
}