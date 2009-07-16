// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
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
		static readonly XName rowDefsName = XName.Get("Grid.RowDefinitions", CompletionDataHelper.WpfXamlNamespace);
		static readonly XName colDefsName = XName.Get("Grid.ColumnDefinitions", CompletionDataHelper.WpfXamlNamespace);
		
		static readonly XName rowDefName = XName.Get("RowDefinition", CompletionDataHelper.WpfXamlNamespace);
		static readonly XName colDefName = XName.Get("ColumnDefinition", CompletionDataHelper.WpfXamlNamespace);
		
		XElement gridTree;
		XElement rowDefitions;
		XElement colDefitions;
		IList<XElement> additionalProperties;
		int selectedCellX = -1, selectedCellY = -1;
		bool? swap = null;
		
		class UndoStep
		{
			public XElement Tree { get; set; }
			public XElement RowDefinitions { get; set; }
			public XElement ColumnDefinitions { get; set; }
			public IList<XElement> AdditionalProperties { get; set; }
			
			public static UndoStep CreateStep(XElement tree, XElement rows, XElement cols, IEnumerable<XElement> properties)
			{
				XElement rowCopy = new XElement(rows);
				XElement colCopy = new XElement(cols);
				XElement treeCopy = new XElement(tree);
				
				IList<XElement> propertiesCopy = properties.Select(item => new XElement(item)).ToList();
				
				return new UndoStep() {
					Tree = treeCopy,
					RowDefinitions = rowCopy,
					ColumnDefinitions = colCopy,
					AdditionalProperties = propertiesCopy
				};
			}
			
			public static UndoStep Copy(UndoStep original)
			{
				return CreateStep(original.Tree, original.RowDefinitions, original.ColumnDefinitions, original.AdditionalProperties);
			}
		}
		
		Stack<UndoStep> undoStack;
		Stack<UndoStep> redoStack;
		
		public EditGridColumnsAndRowsDialog(XElement gridTree)
		{
			InitializeComponent();
			
			this.gridTree = gridTree;
			this.rowDefitions = gridTree.Element(rowDefsName) ?? new XElement(rowDefsName);
			this.colDefitions = gridTree.Element(colDefsName) ?? new XElement(colDefsName);
			
			if (this.rowDefitions.Parent != null)
				this.rowDefitions.Remove();
			if (this.colDefitions.Parent != null)
				this.colDefitions.Remove();
			
			this.additionalProperties = gridTree.Elements().Where(e => e.Name.LocalName.Contains(".")).ToList();
			this.additionalProperties.ForEach(item => { if (item.Parent != null) item.Remove(); });
			
			this.redoStack = new Stack<UndoStep>();
			this.undoStack = new Stack<UndoStep>();
			
			RebuildGrid();
		}
		
		MenuItem CreateItem(string header, Action<TextBlock> clickAction, TextBlock senderItem)
		{
			MenuItem item = new MenuItem();
			
			item.Header = header;
			item.Click += delegate { clickAction(senderItem); };
			
			return item;
		}
		
		void InsertAbove(TextBlock block)
		{
			UpdateUndoRedoState();
			
			int row = (int)block.GetValue(Grid.RowProperty);
			
			var newRow = new XElement(rowDefName);
			newRow.SetAttributeValue(XName.Get("Height"), "Auto");
			var items = rowDefitions.Elements().Skip(row);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.AddBeforeSelf(newRow);
			else
				rowDefitions.Add(newRow);
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						int rowAttribValue = 0;
						if (int.TryParse(rowAttrib.Value, out rowAttribValue))
							return rowAttribValue >= row;
						
						return false;
					}
				);
			
			controls.ForEach(
				item => {
					var rowAttrib = item.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
					item.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value) + 1);
				}
			);
			
			RebuildGrid();
		}
		
		void InsertBelow(TextBlock block)
		{
			UpdateUndoRedoState();
			
			int row = (int)block.GetValue(Grid.RowProperty);
			
			var newRow = new XElement(rowDefName);
			newRow.SetAttributeValue(XName.Get("Height"), "Auto");
			var items = rowDefitions.Elements().Skip(row);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.AddAfterSelf(newRow);
			else
				rowDefitions.Add(newRow);
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						int rowAttribValue = 0;
						if (int.TryParse(rowAttrib.Value, out rowAttribValue))
							return rowAttribValue > row;
						
						return false;
					}
				);
			
			controls.ForEach(
				item => {
					var rowAttrib = item.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
					item.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value) + 1);
				}
			);
			
			RebuildGrid();
		}
		
		void MoveUp(TextBlock block)
		{
			int row = (int)block.GetValue(Grid.RowProperty);
			if (row > 0) {
				UpdateUndoRedoState();
				
				var items = rowDefitions.Elements().Skip(row);
				var selItem = items.FirstOrDefault();
				if (selItem == null)
					return;
				selItem.Remove();
				items = rowDefitions.Elements().Skip(row - 1);
				var before = items.FirstOrDefault();
				if (before == null)
					return;
				before.AddBeforeSelf(selItem);
				
				var controls = gridTree
					.Elements()
					.Where(
						element => {
							var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
							int rowAttribValue = 0;
							if (int.TryParse(rowAttrib.Value, out rowAttribValue))
								return rowAttribValue == row;
							
							return false;
						}
					).ToList();
				
				var controlsDown = gridTree
					.Elements()
					.Where(
						element2 => {
							var rowAttrib = element2.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
							int rowAttribValue = 0;
							if (int.TryParse(rowAttrib.Value, out rowAttribValue))
								return rowAttribValue == (row - 1);
							
							return false;
						}
					).ToList();
				
				controls.ForEach(
					item => {
						var rowAttrib = item.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						item.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value) - 1);
					}
				);
				
				controlsDown.ForEach(
					item2 => {
						var rowAttrib = item2.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						item2.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value) + 1);
					}
				);
				
				RebuildGrid();
			}
		}
		
		void MoveDown(TextBlock block)
		{
			int row = (int)block.GetValue(Grid.RowProperty);
			if (row < rowDefitions.Elements().Count() - 1) {
				UpdateUndoRedoState();

				var items = rowDefitions.Elements().Skip(row);
				var selItem = items.FirstOrDefault();
				if (selItem == null)
					return;
				selItem.Remove();
				items = rowDefitions.Elements().Skip(row - 1);
				var before = items.FirstOrDefault();
				if (before == null)
					return;
				before.AddBeforeSelf(selItem);
				
				var controls = gridTree
					.Elements()
					.Where(
						element => {
							var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
							int rowAttribValue = 0;
							if (int.TryParse(rowAttrib.Value, out rowAttribValue))
								return rowAttribValue == row;
							
							return false;
						}
					).ToList();
				
				var controlsUp = gridTree
					.Elements()
					.Where(
						element2 => {
							var rowAttrib = element2.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
							int rowAttribValue = 0;
							if (int.TryParse(rowAttrib.Value, out rowAttribValue))
								return rowAttribValue == (row + 1);
							
							return false;
						}
					).ToList();
				
				controls.ForEach(
					item => {
						var rowAttrib = item.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						item.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value) + 1);
					}
				);
				
				controlsUp.ForEach(
					item2 => {
						var rowAttrib = item2.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						item2.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value) - 1);
					}
				);
				
				RebuildGrid();
			}
		}
		
		void DeleteRow(TextBlock block)
		{
			int row = (int)block.GetValue(Grid.RowProperty);
			UpdateUndoRedoState();

			var items = rowDefitions.Elements().Skip(row);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.Remove();
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						int rowAttribValue = 0;
						if (int.TryParse(rowAttrib.Value, out rowAttribValue))
							return rowAttribValue >= row;
						
						return false;
					}
				);
			
			controls.ForEach(
				item => {
					var rowAttrib = item.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
					item.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value) - 1);
				}
			);
			
			RebuildGrid();
		}
		
		void InsertBefore(TextBlock block)
		{
			int column = (int)block.GetValue(Grid.ColumnProperty);
			UpdateUndoRedoState();

			var newColumn = new XElement(colDefName);
			newColumn.SetAttributeValue(XName.Get("Width"), "Auto");
			var items = colDefitions.Elements().Skip(column);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.AddBeforeSelf(newColumn);
			else
				colDefitions.Add(newColumn);
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						int colAttribValue = 0;
						if (int.TryParse(colAttrib.Value, out colAttribValue))
							return colAttribValue >= column;
						
						return false;
					}
				);
			
			controls.ForEach(
				item => {
					var colAttrib = item.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
					item.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value) + 1);
				}
			);
			
			RebuildGrid();
		}
		
		void InsertAfter(TextBlock block)
		{
			int column = (int)block.GetValue(Grid.ColumnProperty);
			UpdateUndoRedoState();

			var newColumn = new XElement(colDefName);
			newColumn.SetAttributeValue(XName.Get("Width"), "Auto");
			var items = colDefitions.Elements().Skip(column);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.AddBeforeSelf(newColumn);
			else
				colDefitions.Add(newColumn);
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						int colAttribValue = 0;
						if (int.TryParse(colAttrib.Value, out colAttribValue))
							return colAttribValue > column;
						
						return false;
					}
				);
			
			controls.ForEach(
				item => {
					var colAttrib = item.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
					item.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value) + 1);
				}
			);
			
			RebuildGrid();
		}
		
		void MoveLeft(TextBlock block)
		{
			int column = (int)block.GetValue(Grid.ColumnProperty);
			
			if (column > 0) {
				UpdateUndoRedoState();
				
				var items = colDefitions.Elements().Skip(column);
				var selItem = items.FirstOrDefault();
				if (selItem == null)
					return;
				selItem.Remove();
				items = colDefitions.Elements().Skip(column - 1);
				var before = items.FirstOrDefault();
				if (before == null)
					return;
				before.AddBeforeSelf(selItem);
				
				var controls = gridTree
					.Elements()
					.Where(
						element => {
							var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
							int colAttribValue = 0;
							if (int.TryParse(colAttrib.Value, out colAttribValue))
								return colAttribValue == column;
							
							return false;
						}
					).ToList();
				
				var controlsLeft = gridTree
					.Elements()
					.Where(
						element2 => {
							var colAttrib = element2.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
							int colAttribValue = 0;
							if (int.TryParse(colAttrib.Value, out colAttribValue))
								return colAttribValue == (column - 1);
							
							return false;
						}
					).ToList();
				
				controls.ForEach(
					item => {
						var colAttrib = item.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						item.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value) - 1);
					}
				);
				
				controlsLeft.ForEach(
					item2 => {
						var colAttrib = item2.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						item2.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value) + 1);
					}
				);
				
				RebuildGrid();
			}
		}
		
		void MoveRight(TextBlock block)
		{
			int column = (int)block.GetValue(Grid.ColumnProperty);
			
			if (column < colDefitions.Elements().Count() - 1) {
				UpdateUndoRedoState();
				
				var items = colDefitions.Elements().Skip(column);
				var selItem = items.FirstOrDefault();
				if (selItem == null)
					return;
				selItem.Remove();
				items = colDefitions.Elements().Skip(column - 1);
				var before = items.FirstOrDefault();
				if (before == null)
					return;
				before.AddBeforeSelf(selItem);
				
				var controls = gridTree
					.Elements()
					.Where(
						element => {
							var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
							int colAttribValue = 0;
							if (int.TryParse(colAttrib.Value, out colAttribValue))
								return colAttribValue == column;
							
							return false;
						}
					).ToList();
				
				var controlsRight = gridTree
					.Elements()
					.Where(
						element2 => {
							var colAttrib = element2.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
							int colAttribValue = 0;
							if (int.TryParse(colAttrib.Value, out colAttribValue))
								return colAttribValue == (column + 1);
							
							return false;
						}
					).ToList();
				
				controls.ForEach(
					item => {
						var colAttrib = item.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						item.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value) + 1);
					}
				);
				
				controlsRight.ForEach(
					item2 => {
						var colAttrib = item2.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						item2.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value) - 1);
					}
				);
				
				RebuildGrid();
			}
		}
		
		void DeleteColumn(TextBlock block)
		{
			int column = (int)block.GetValue(Grid.ColumnProperty);
			UpdateUndoRedoState();

			var items = colDefitions.Elements().Skip(column);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.Remove();
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						int colAttribValue = 0;
						if (int.TryParse(colAttrib.Value, out colAttribValue))
							return colAttribValue >= column;
						
						return false;
					}
				);
			
			controls.ForEach(
				item => {
					var colAttrib = item.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
					item.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value) - 1);
				}
			);
			
			RebuildGrid();
		}
		
		void SwapContent(TextBlock block)
		{
			lblInstruction.Text = "Click on the cell you want to swap the selected cell with.";
			lblInstruction.Visibility = Visibility.Visible;
			
			this.selectedCellX = (int)block.GetValue(Grid.ColumnProperty);
			this.selectedCellY = (int)block.GetValue(Grid.RowProperty);
			
			swap = true;
		}
		
		void MoveContent(TextBlock block)
		{
			lblInstruction.Text = "Click on the cell you want to move the selected content to.";
			lblInstruction.Visibility = Visibility.Visible;
			
			this.selectedCellX = (int)block.GetValue(Grid.ColumnProperty);
			this.selectedCellY = (int)block.GetValue(Grid.RowProperty);
			
			swap = false;
		}
		
		void DeleteContent(TextBlock block)
		{
			int column = (int)block.GetValue(Grid.ColumnProperty);
			int row = (int)block.GetValue(Grid.RowProperty);
			UpdateUndoRedoState();

			gridTree.Elements()
				.Where(
					element => {
						var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						int colAttribValue = 0, rowAttribValue = 0;
						if (int.TryParse(colAttrib.Value, out colAttribValue) && int.TryParse(rowAttrib.Value, out rowAttribValue))
							return colAttribValue == column && rowAttribValue == row;
						
						return false;
					}
				).ForEach(item => item.Remove());
			
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
		
		string BuildDescriptionForCell(int row, int col)
		{
			StringBuilder builder = new StringBuilder();
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						return 	row.ToString() == rowAttrib.Value && col.ToString() == colAttrib.Value;
					}
				);
			
			foreach (var control in controls) {
				var nameAttrib = control.Attribute(XName.Get("Name", CompletionDataHelper.XamlNamespace)) ?? control.Attribute(XName.Get("Name"));
				if (builder.Length > 0)
					builder.Append(", ");
				builder.Append(control.Name.LocalName);
				if (nameAttrib != null)
					builder.Append(" (" + nameAttrib.Value + ")");
			}
			
			return builder.ToString();
		}
		
		void RebuildGrid()
		{
			this.gridDisplay.Children.Clear();
			this.gridDisplay.RowDefinitions.Clear();
			this.gridDisplay.ColumnDefinitions.Clear();
			
			int rows = rowDefitions.Elements().Count();
			int cols = colDefitions.Elements().Count();
			
			if (rows == 0) {
				rowDefitions.Add(new XElement(rowDefName).AddAttribute("Height", "Auto"));
				rows = 1;
			}
			if (cols == 0) {
				colDefitions.Add(new XElement(colDefName).AddAttribute("Width", "Auto"));
				cols = 1;
			}
			
			for (int i = 0; i < cols; i++)
				this.gridDisplay.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			
			for (int i = 0; i < rows; i++) {
				this.gridDisplay.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
				
				for (int j = 0; j < cols; j++) {
					TextBlock displayRect = new TextBlock() {
						Margin = new Thickness(5),
						Background = Brushes.CornflowerBlue,
						Text = BuildDescriptionForCell(i, j),
						TextAlignment = TextAlignment.Center,
						TextWrapping = TextWrapping.Wrap
					};
					
					displayRect.SetValue(Grid.RowProperty, i);
					displayRect.SetValue(Grid.ColumnProperty, j);
					
					displayRect.ContextMenuOpening += new ContextMenuEventHandler(DisplayRectContextMenuOpening);
					displayRect.MouseLeftButtonDown += new MouseButtonEventHandler(DisplayRectMouseLeftButtonDown);
					
					this.gridDisplay.Children.Add(displayRect);
				}
			}
			
			this.InvalidateVisual();
		}
		
		void UpdateUndoRedoState()
		{
			this.undoStack.Push(UndoStep.CreateStep(gridTree, rowDefitions, colDefitions, additionalProperties));
			this.redoStack.Clear();
		}

		void DisplayRectMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (selectedCellX > -1 && selectedCellY > -1 && swap.HasValue) {
				UpdateUndoRedoState();
				
				TextBlock block = sender as TextBlock;
				int targetX = (int)block.GetValue(Grid.ColumnProperty);
				int targetY = (int)block.GetValue(Grid.RowProperty);
				
				var elements = gridTree.Elements()
					.Where(
						element => {
							var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
							var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
							int colAttribValue = 0, rowAttribValue = 0;
							if (int.TryParse(colAttrib.Value, out colAttribValue) && int.TryParse(rowAttrib.Value, out rowAttribValue))
								return colAttribValue == targetX && rowAttribValue == targetY;
							
							return false;
						}
					).ToList();
				
				var elements2 = gridTree.Elements()
					.Where(
						element => {
							var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
							var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
							int colAttribValue = 0, rowAttribValue = 0;
							if (int.TryParse(colAttrib.Value, out colAttribValue) && int.TryParse(rowAttrib.Value, out rowAttribValue))
								return colAttribValue == selectedCellX && rowAttribValue == selectedCellY;
							
							return false;
						}
					).ToList();

				if (swap == true) {
					elements.ForEach(
						element => {
							element.SetAttributeValue(XName.Get("Grid.Column"), selectedCellX);
							element.SetAttributeValue(XName.Get("Grid.Row"), selectedCellY);
						}
					);
				}
				
				elements2.ForEach(
					element => {
						element.SetAttributeValue(XName.Get("Grid.Column"), targetX);
						element.SetAttributeValue(XName.Get("Grid.Row"), targetY);
					}
				);
			}
			
			lblInstruction.Visibility = Visibility.Collapsed;
			selectedCellX = selectedCellY = -1;
			swap = null;
			
			RebuildGrid();
		}
		
		public XElement GetConstructedTree()
		{
			gridTree.AddFirst(additionalProperties);
			gridTree.AddFirst(colDefitions);
			gridTree.AddFirst(rowDefitions);
			
			return gridTree;
		}

		void DisplayRectContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			MenuItem undoItem = new MenuItem();
			undoItem.Header = "Undo";
			undoItem.IsEnabled = undoStack.Count > 0;
			undoItem.Click += new RoutedEventHandler(UndoItemClick);
			
			MenuItem redoItem = new MenuItem();
			redoItem.Header = "Redo";
			redoItem.IsEnabled = redoStack.Count > 0;
			redoItem.Click += new RoutedEventHandler(RedoItemClick);
			
			ContextMenu menu = new ContextMenu() {
				Items = {
					undoItem,
					redoItem,
					new Separator(),
					new MenuItem() {
						Header = "Row",
						Items = {
							CreateItem("Insert above", InsertAbove, sender as TextBlock),
							CreateItem("Insert below", InsertBelow, sender as TextBlock),
							new Separator(),
							CreateItem("Move up", MoveUp, sender as TextBlock),
							CreateItem("Move down", MoveDown, sender as TextBlock),
							new Separator(),
							CreateItem("Delete", DeleteRow, sender as TextBlock)
						}
					},
					new MenuItem() {
						Header = "Column",
						Items = {
							CreateItem("Insert before", InsertBefore, sender as TextBlock),
							CreateItem("Insert after", InsertAfter, sender as TextBlock),
							new Separator(),
							CreateItem("Move left", MoveLeft, sender as TextBlock),
							CreateItem("Move right", MoveRight, sender as TextBlock),
							new Separator(),
							CreateItem("Delete", DeleteColumn, sender as TextBlock)
						}
					},
					new MenuItem() {
						Header = "Cell",
						Items = {
							CreateItem("Swap content", SwapContent, sender as TextBlock),
							CreateItem("Move content", MoveContent, sender as TextBlock),
							CreateItem("Delete content", DeleteContent, sender as TextBlock)
						}
					}
				}
			};
			
			menu.IsOpen = true;
		}

		void RedoItemClick(object sender, RoutedEventArgs e)
		{
			HandleSteps(redoStack, undoStack);
		}

		void UndoItemClick(object sender, RoutedEventArgs e)
		{
			HandleSteps(undoStack, redoStack);
		}
		
		void HandleSteps(Stack<UndoStep> stack1, Stack<UndoStep> stack2)
		{
			UndoStep step = stack1.Pop();
			
			stack2.Push(UndoStep.CreateStep(gridTree, rowDefitions, colDefitions, additionalProperties));
			
			this.additionalProperties = step.AdditionalProperties;
			this.rowDefitions = step.RowDefinitions;
			this.colDefitions = step.ColumnDefinitions;
			this.gridTree = step.Tree;
			
			RebuildGrid();
		}
	}
}