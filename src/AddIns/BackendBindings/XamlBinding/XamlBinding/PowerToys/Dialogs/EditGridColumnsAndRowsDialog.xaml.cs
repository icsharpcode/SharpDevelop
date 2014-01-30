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
using System.Diagnostics;
using System.Globalization;
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

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.XamlBinding.PowerToys.Dialogs
{
	/// <summary>
	/// Interaction logic for EditGridColumnsAndRowsDialog.xaml
	/// </summary>
	public partial class EditGridColumnsAndRowsDialog : Window
	{
		readonly XName rowDefsName, colDefsName;
		readonly XName rowDefName, colDefName;
		
		readonly XName gridRowName = XName.Get("Grid.Row");
		readonly XName gridColName = XName.Get("Grid.Column");
		
		readonly string currentWpfNamespace;
		
		XElement gridTree;
		XElement rowDefitions;
		XElement colDefitions;
		IList<XElement> additionalProperties;
		
		bool gridLengthInvalid;
		
		Stack<UndoStep> undoStack;
		Stack<UndoStep> redoStack;
		
		public EditGridColumnsAndRowsDialog(XElement gridTree)
		{
			InitializeComponent();
			
			currentWpfNamespace = gridTree.GetCurrentNamespaces()
				.First(i => CompletionDataHelper.WpfXamlNamespaces.Contains(i));
			
			rowDefsName = XName.Get("Grid.RowDefinitions", currentWpfNamespace);
			colDefsName = XName.Get("Grid.ColumnDefinitions", currentWpfNamespace);
			
			rowDefName = XName.Get("RowDefinition", currentWpfNamespace);
			colDefName = XName.Get("ColumnDefinition", currentWpfNamespace);
			
			this.gridTree = gridTree;
			this.rowDefitions = gridTree.Element(rowDefsName) ?? new XElement(rowDefsName);
			this.colDefitions = gridTree.Element(colDefsName) ?? new XElement(colDefsName);
			
			if (this.rowDefitions.Parent != null)
				this.rowDefitions.Remove();
			if (this.colDefitions.Parent != null)
				this.colDefitions.Remove();
			
			foreach (var height in this.rowDefitions.Elements().Select(row => row.Attribute("Height"))) {
				if (height.Value.Trim() == "1*")
					height.Value = "*";
				else
					height.Value = height.Value.Trim();
			}
			
			foreach (var width in this.colDefitions.Elements().Select(col => col.Attribute("Width"))) {
				if (width.Value.Trim() == "1*")
					width.Value = "*";
				else
					width.Value = width.Value.Trim();
			}
			
			this.additionalProperties = gridTree.Elements().Where(e => e.Name.LocalName.Contains(".")).ToList();
			this.additionalProperties.ForEach(item => { if (item.Parent != null) item.Remove(); });
			
			this.redoStack = new Stack<UndoStep>();
			this.undoStack = new Stack<UndoStep>();
			
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, delegate { UndoItemClick(null, null); }));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, delegate { RedoItemClick(null, null); }));
			
			int maxCols = Math.Max(this.colDefitions.Elements().Count(), 1);
			int maxRows = Math.Max(this.rowDefitions.Elements().Count(), 1);
			
			this.gridTree.Elements().ForEach(el => NormalizeElementGridIndices(el, maxCols, maxRows));
			
			RebuildGrid();
		}
		
		void NormalizeElementGridIndices(XElement element, int maxCols, int maxRows)
		{
			XAttribute a = element.Attribute(gridColName);
			XAttribute b = element.Attribute(gridRowName);
			int value;
			if (a != null && int.TryParse(a.Value, out value))
				element.SetAttributeValue(gridColName, Utils.MinMax(value, 0, maxCols - 1));
			else
				element.SetAttributeValue(gridColName, 0);
			if (b != null && int.TryParse(b.Value, out value))
				element.SetAttributeValue(gridRowName, Utils.MinMax(value, 0, maxRows - 1));
			else
				element.SetAttributeValue(gridRowName, 0);
		}
		
		static MenuItem CreateItem(string header, Action<StackPanel> clickAction, StackPanel senderItem)
		{
			MenuItem item = new MenuItem();
			
			item.Header = header;
			item.Click += delegate { clickAction(senderItem); };
			
			return item;
		}
		
		static MenuItem CreateItem(string header, Action<int, int> clickAction, int cellIndex)
		{
			MenuItem item = new MenuItem();
			
			item.Header = header;
			item.Click += delegate { clickAction(cellIndex, 1); };
			
			return item;
		}
		
		void InsertAbove(StackPanel block)
		{
			UpdateUndoRedoState();
			
			int row = (int)block.GetValue(Grid.RowProperty);
			
			var newRow = new XElement(rowDefName);
			newRow.SetAttributeValue(XName.Get("Height"), "*");
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
						var rowAttrib = element.Attribute(gridRowName) ?? new XAttribute(gridRowName, 0);
						int rowAttribValue = 0;
						if (int.TryParse(rowAttrib.Value, out rowAttribValue))
							return rowAttribValue >= row;
						
						return false;
					}
				);
			
			controls.ForEach(item => MoveRowItem(item, 1));
			
			RebuildGrid();
		}
		
		void InsertBelow(StackPanel block)
		{
			UpdateUndoRedoState();
			
			int row = (int)block.GetValue(Grid.RowProperty);
			
			var newRow = new XElement(rowDefName);
			newRow.SetAttributeValue(XName.Get("Height"), "*");
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
						var rowAttrib = element.Attribute(gridRowName) ?? new XAttribute(gridRowName, 0);
						int rowAttribValue = 0;
						if (int.TryParse(rowAttrib.Value, out rowAttribValue))
							return rowAttribValue > row;
						
						return false;
					}
				);
			
			controls.ForEach(item => MoveRowItem(item, 1));
			
			RebuildGrid();
		}
		
		void MoveUp(int row, int steps)
		{
			if (steps < 1 || row - steps < 0)
				return;
			
			UpdateUndoRedoState();
			
			var selItem = rowDefitions.Elements().Skip(row).FirstOrDefault();
			if (selItem == null)
				return;
			selItem.Remove();
			
			var before = rowDefitions.Elements().Skip(row - steps).FirstOrDefault();
			if (before == null)
				return;
			before.AddBeforeSelf(selItem);
			
			var controls = gridTree.Elements().Where(element => IsSameRow(element, row)).ToList();
			
			var controlsDown = gridTree.Elements()
				.Where(
					element2 => {
						var rowAttrib = element2.Attribute(gridRowName) ?? new XAttribute(gridRowName, 0);
						int rowAttribValue = 0;
						if (int.TryParse(rowAttrib.Value, out rowAttribValue))
							return rowAttribValue < row && rowAttribValue >= (row - steps);
						
						return false;
					}
				).ToList();
			
			controls.ForEach(item => MoveRowItem(item, -steps));
			controlsDown.ForEach(item2 => MoveRowItem(item2, 1));
			
			RebuildGrid();
		}
		
		bool IsSameRow(XElement element, int row)
		{
			var rowAttrib = element.Attribute(gridRowName) ?? new XAttribute(gridRowName, 0);
			int rowAttribValue = 0;
			if (int.TryParse(rowAttrib.Value, out rowAttribValue))
				return rowAttribValue == row;
			
			return false;
		}
		
		void MoveDown(int row, int steps)
		{
			if (steps < 1 || row + steps > rowDefitions.Elements().Count())
				return;
			
			UpdateUndoRedoState();

			var selItem = rowDefitions.Elements().Skip(row).FirstOrDefault();
			if (selItem == null)
				return;
			selItem.Remove();
			var before = rowDefitions.Elements().Skip(row + steps).FirstOrDefault();
			if (before == null)
				rowDefitions.Add(selItem);
			else
				before.AddBeforeSelf(selItem);
			
			var controls = gridTree.Elements().Where(element => IsSameRow(element, row)).ToList();
			
			var controlsUp = gridTree.Elements()
				.Where(
					element2 => {
						var rowAttrib = element2.Attribute(gridRowName) ?? new XAttribute(gridRowName, 0);
						int rowAttribValue = 0;
						if (int.TryParse(rowAttrib.Value, out rowAttribValue))
							return rowAttribValue > row && rowAttribValue <= (row + steps);
						
						return false;
					}
				).ToList();
			
			controls.ForEach(item => MoveRowItem(item, steps));
			controlsUp.ForEach(item2 => MoveRowItem(item2, -1));
			
			RebuildGrid();
		}
		
		void DeleteRow(StackPanel block)
		{
			int row = (int)block.GetValue(Grid.RowProperty);
			UpdateUndoRedoState();

			var items = rowDefitions.Elements().Skip(row);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.Remove();
			
			var controls = gridTree.Elements()
				.Where(
					element => {
						var rowAttrib = element.Attribute(gridRowName) ?? new XAttribute(gridRowName, 0);
						int rowAttribValue = 0;
						if (int.TryParse(rowAttrib.Value, out rowAttribValue))
							return rowAttribValue >= row;
						
						return false;
					}
				);
			
			controls.ForEach(item => MoveRowItem(item, -1));
			
			RebuildGrid();
		}
		
		void InsertBefore(StackPanel block)
		{
			int column = (int)block.GetValue(Grid.ColumnProperty);
			UpdateUndoRedoState();

			var newColumn = new XElement(colDefName);
			newColumn.SetAttributeValue(XName.Get("Width"), "*");
			var items = colDefitions.Elements().Skip(column);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.AddBeforeSelf(newColumn);
			else
				colDefitions.Add(newColumn);
			
			var controls = gridTree.Elements()
				.Where(
					element => {
						var colAttrib = element.Attribute(gridColName) ?? new XAttribute(gridColName, 0);
						int colAttribValue = 0;
						if (int.TryParse(colAttrib.Value, out colAttribValue))
							return colAttribValue >= column;
						
						return false;
					}
				);
			
			controls.ForEach(item => MoveColumnItem(item, 1));
			
			RebuildGrid();
		}
		
		void InsertAfter(StackPanel block)
		{
			int column = (int)block.GetValue(Grid.ColumnProperty);
			UpdateUndoRedoState();

			var newColumn = new XElement(colDefName);
			newColumn.SetAttributeValue(XName.Get("Width"), "*");
			var items = colDefitions.Elements().Skip(column);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.AddAfterSelf(newColumn);
			else
				colDefitions.Add(newColumn);
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var colAttrib = element.Attribute(gridColName) ?? new XAttribute(gridColName, 0);
						int colAttribValue = 0;
						if (int.TryParse(colAttrib.Value, out colAttribValue))
							return colAttribValue > column;
						
						return false;
					}
				);
			
			controls.ForEach(item => MoveColumnItem(item, 1));
			
			RebuildGrid();
		}
		
		void MoveLeft(int column, int steps)
		{
			if (steps < 1 || column - steps < 0)
				return;
			
			UpdateUndoRedoState();
			
			var selItem = colDefitions.Elements().Skip(column).FirstOrDefault();
			if (selItem == null)
				return;
			selItem.Remove();
			var before = colDefitions.Elements().Skip(column - steps).FirstOrDefault();
			if (before == null)
				return;
			before.AddBeforeSelf(selItem);
			
			var controls = gridTree.Elements().Where(element => IsSameColumn(element, column)).ToList();
			
			var controlsLeft = gridTree
				.Elements()
				.Where(
					element2 => {
						var colAttrib = element2.Attribute(gridColName) ?? new XAttribute(gridColName, 0);
						int colAttribValue = 0;
						if (int.TryParse(colAttrib.Value, out colAttribValue))
							return colAttribValue < column && colAttribValue >= (column - steps);
						
						return false;
					}
				).ToList();
			
			controls.ForEach(item => MoveColumnItem(item, -steps));
			controlsLeft.ForEach(item => MoveColumnItem(item, 1));

			
			RebuildGrid();
		}
		
		bool IsSameColumn(XElement element, int column)
		{
			var colAttrib = element.Attribute(gridColName) ?? new XAttribute(gridColName, 0);
			int colAttribValue = 0;
			if (int.TryParse(colAttrib.Value, out colAttribValue))
				return colAttribValue == column;
			
			return false;
		}
		
		void MoveColumnItem(XElement item, int steps)
		{
			var colAttrib = item.Attribute(gridColName) ?? new XAttribute(gridColName, 0);
			item.SetAttributeValue(gridColName, int.Parse(colAttrib.Value, CultureInfo.InvariantCulture) + steps);
		}
		
		void MoveRowItem(XElement item, int steps)
		{
			var rowAttrib = item.Attribute(gridRowName) ?? new XAttribute(gridRowName, 0);
			item.SetAttributeValue(gridRowName, int.Parse(rowAttrib.Value, CultureInfo.InvariantCulture) + steps);
		}
		
		void MoveRight(int column, int steps)
		{
			if (steps < 1 || column + steps > colDefitions.Elements().Count())
				return;
			
			UpdateUndoRedoState();
			
			var selItem = colDefitions.Elements().Skip(column).FirstOrDefault();
			if (selItem == null)
				return;
			selItem.Remove();
			var before = colDefitions.Elements().Skip(column + steps).FirstOrDefault();
			if (before == null)
				colDefitions.Add(selItem);
			else
				before.AddBeforeSelf(selItem);
			
			var controls = gridTree.Elements().Where(element => IsSameColumn(element, column)).ToList();
			
			var controlsRight = gridTree
				.Elements()
				.Where(
					element2 => {
						var colAttrib = element2.Attribute(gridColName) ?? new XAttribute(gridColName, 0);
						int colAttribValue = 0;
						if (int.TryParse(colAttrib.Value, out colAttribValue))
							return colAttribValue > column && colAttribValue <= (column + steps);
						
						return false;
					}
				).ToList();
			
			controls.ForEach(item => MoveColumnItem(item, steps));
			controlsRight.ForEach(item2 => MoveColumnItem(item2, -1));
			
			RebuildGrid();
		}
		
		void DeleteColumn(StackPanel block)
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
						var colAttrib = element.Attribute(gridColName) ?? new XAttribute(gridColName, 0);
						int colAttribValue = 0;
						if (int.TryParse(colAttrib.Value, out colAttribValue))
							return colAttribValue >= column;
						
						return false;
					}
				);
			
			controls.ForEach(item => MoveColumnItem(item, -1));

			
			RebuildGrid();
		}
		
		void BtnCancelClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
		
		void BtnOKClick(object sender, RoutedEventArgs e)
		{
			if (gridLengthInvalid) {
				MessageService.ShowError("Grid is invalid, please check the row heights and column widths!");
				return;
			}
			
			this.DialogResult = true;
		}
		
		void RebuildGrid()
		{
			if (this.marker != null) {
				AdornerLayer.GetAdornerLayer(this.buttonPanel).Remove(this.marker);
				AdornerLayer.GetAdornerLayer(this.dropPanel).Remove(this.marker);
				this.marker = null;
			}
			
			this.gridDisplay.Children.Clear();
			this.gridDisplay.RowDefinitions.Clear();
			this.gridDisplay.ColumnDefinitions.Clear();
			
			
			this.columnWidthGrid.ColumnDefinitions.Clear();
			this.columnWidthGrid.Children.Clear();
			
			this.rowHeightGrid.RowDefinitions.Clear();
			this.rowHeightGrid.Children.Clear();
			
			int rows = rowDefitions.Elements().Count();
			int cols = colDefitions.Elements().Count();
			
			if (rows == 0) {
				rowDefitions.Add(new XElement(rowDefName).AddAttribute("Height", "*"));
				rows = 1;
			}
			if (cols == 0) {
				colDefitions.Add(new XElement(colDefName).AddAttribute("Width", "*"));
				cols = 1;
			}
			
			for (int i = 0; i < cols; i++) {
				this.gridDisplay.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
				this.columnWidthGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
				GridLengthEditor editor = new GridLengthEditor(Orientation.Horizontal, i, (colDefitions.Elements().ElementAt(i).Attribute("Width") ?? new XAttribute("Width", "")).Value);
				
				editor.SelectedValueChanged += new EventHandler<GridLengthSelectionChangedEventArgs>(EditorSelectedValueChanged);
				editor.Deleted += new EventHandler<GridLengthSelectionChangedEventArgs>(EditorDeleted);
				editor.MouseLeftButtonDown += new MouseButtonEventHandler(EditorMouseLeftButtonDown);
				editor.Drop += new DragEventHandler(EditorDrop);
				editor.DragOver += new DragEventHandler(EditorDragOver);
				
				editor.AllowDrop = true;
				
				this.columnWidthGrid.Children.Add(editor);
				
				Button leftAddButton = new Button() {
					Content = "+",
					HorizontalAlignment = HorizontalAlignment.Left,
					Margin = new Thickness(-10, 10, 5,10),
					Padding = new Thickness(3),
					Tag = i
				};
				
				leftAddButton.Click += BtnAddColumnClick;
				
				leftAddButton.SetValue(Grid.ColumnProperty, i);
				this.columnWidthGrid.Children.Add(leftAddButton);
				
				if (cols == i + 1) {
					Button rightAddButton = new Button() {
						Content = "+",
						HorizontalAlignment = HorizontalAlignment.Right,
						Margin = new Thickness(5, 10, 0, 10),
						Padding = new Thickness(3)
					};
					
					rightAddButton.Click += BtnAddColumnClick;
					
					rightAddButton.SetValue(Grid.ColumnProperty, i);
					this.columnWidthGrid.Children.Add(rightAddButton);
				}
			}
			
			for (int i = 0; i < rows; i++) {
				this.gridDisplay.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
				
				this.rowHeightGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
				GridLengthEditor editor = new GridLengthEditor(Orientation.Vertical, i, (rowDefitions.Elements().ElementAt(i).Attribute("Height") ?? new XAttribute("Height", "")).Value);
				
				editor.SelectedValueChanged += new EventHandler<GridLengthSelectionChangedEventArgs>(EditorSelectedValueChanged);
				editor.Deleted += new EventHandler<GridLengthSelectionChangedEventArgs>(EditorDeleted);
				editor.MouseLeftButtonDown += new MouseButtonEventHandler(EditorMouseLeftButtonDown);
				editor.Drop += new DragEventHandler(EditorDrop);
				editor.DragOver += new DragEventHandler(EditorDragOver);
				
				editor.AllowDrop = true;
				
				this.rowHeightGrid.Children.Add(editor);
				
				Button topAddButton = new Button() {
					Content = "+",
					VerticalAlignment = VerticalAlignment.Top,
					Margin = new Thickness(10, -10, 10, 5),
					Padding = new Thickness(3),
					Tag = i
				};
				
				topAddButton.Click += BtnAddRowClick;
				
				topAddButton.SetValue(Grid.RowProperty, i);
				this.rowHeightGrid.Children.Add(topAddButton);
				
				if (rows == i + 1) {
					Button bottomAddButton = new Button() {
						Content = "+",
						VerticalAlignment = VerticalAlignment.Bottom,
						Margin = new Thickness(10, 5, 10, 0),
						Padding = new Thickness(3)
					};
					
					bottomAddButton.Click += BtnAddRowClick;
					
					bottomAddButton.SetValue(Grid.RowProperty, i);
					this.rowHeightGrid.Children.Add(bottomAddButton);
				}
				
				for (int j = 0; j < cols; j++) {
					StackPanel displayRect = new StackPanel() {
						Margin = new Thickness(5),
						Background = Brushes.LightGray,
						Orientation = Orientation.Vertical
					};
					
					displayRect.AllowDrop = true;
					
					displayRect.Drop += new DragEventHandler(DisplayRectDrop);
					displayRect.DragOver += new DragEventHandler(DisplayRectDragOver);
					
					displayRect.Children.AddRange(BuildItemsForCell(i, j));
					
					displayRect.SetValue(Grid.RowProperty, i);
					displayRect.SetValue(Grid.ColumnProperty, j);
					
					displayRect.ContextMenuOpening += new ContextMenuEventHandler(DisplayRectContextMenuOpening);
					
					this.gridDisplay.Children.Add(displayRect);
				}
			}
			
			this.InvalidateVisual();
		}

		void EditorDragOver(object sender, DragEventArgs e)
		{
			try {
				GridLengthEditor target = sender as GridLengthEditor;
				GridLengthEditor source = e.Data.GetData(typeof(GridLengthEditor)) as GridLengthEditor;
				e.Handled =  true;
				
				if (marker != null) {
					AdornerLayer.GetAdornerLayer(marker.AdornedElement).Remove(marker);
					marker = null;
				}
				
				if (target != null && source != null && source.Orientation == target.Orientation
				    && (target != source && (target.Cell < source.Cell || target.Cell > source.Cell + 1))) {
					marker = DragDropMarkerAdorner.CreateAdornerCellMove(target);
					e.Effects = DragDropEffects.Move;
					return;
				}
				
				e.Effects = DragDropEffects.None;
			} catch (Exception ex) {
				Core.LoggingService.Error(ex);
			}
		}

		void EditorDrop(object sender, DragEventArgs e)
		{
			try {
				GridLengthEditor source = e.Data.GetData(typeof(GridLengthEditor)) as GridLengthEditor;
				GridLengthEditor target = sender as GridLengthEditor;
				
				if (source != null && target != null) {
					if (source.Orientation == Orientation.Horizontal) {
						if (source.Cell > target.Cell)
							MoveLeft(source.Cell, Math.Abs(source.Cell - target.Cell));
						else
							MoveRight(source.Cell, Math.Abs(source.Cell - target.Cell) - 1);
					}
					
					if (source.Orientation == Orientation.Vertical) {
						if (source.Cell > target.Cell)
							MoveUp(source.Cell, Math.Abs(source.Cell - target.Cell));
						else
							MoveDown(source.Cell, Math.Abs(source.Cell - target.Cell) - 1);
					}
				}
			} catch (Exception ex) {
				Core.LoggingService.Error(ex);
			}
		}

		void EditorMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragDropEffects allowedEffects = DragDropEffects.Move;
			GridLengthEditor editor = sender as GridLengthEditor;
			DragDrop.DoDragDrop(editor, editor, allowedEffects);
		}

		void EditorDeleted(object sender, GridLengthSelectionChangedEventArgs e)
		{
			if (e.Type == Orientation.Horizontal)
				DeleteColumn(gridDisplay.Children.OfType<StackPanel>().First(item => (int)item.GetValue(Grid.ColumnProperty) == e.Cell));
			else
				DeleteRow(gridDisplay.Children.OfType<StackPanel>().First(item => (int)item.GetValue(Grid.RowProperty) == e.Cell));
		}

		void EditorSelectedValueChanged(object sender, GridLengthSelectionChangedEventArgs e)
		{
			UpdateUndoRedoState();
			
			string value = "Invalid";
			
			if (e.Value != null)
				value = e.Value;

			if (e.Type == Orientation.Horizontal)
				colDefitions.Elements().ElementAt(e.Cell).SetAttributeValue("Width", value);
			else
				rowDefitions.Elements().ElementAt(e.Cell).SetAttributeValue("Height", value);
			
			gridLengthInvalid = colDefitions.Elements().Any(col => (col.Attribute("Width") ?? new XAttribute("Width", "*")).Value == "Invalid")
				|| rowDefitions.Elements().Any(row => (row.Attribute("Height") ?? new XAttribute("Height", "*")).Value == "Invalid");
		}
		
		DragDropMarkerAdorner marker = null;

		void DisplayRectDragOver(object sender, DragEventArgs e)
		{
			try {
				StackPanel target = sender as StackPanel;
				e.Handled =  true;
				
				if (marker != null) {
					AdornerLayer.GetAdornerLayer(marker.AdornedElement).Remove(marker);
					marker = null;
				}
				
				if (target != null) {
					FrameworkElement element = target.InputHitTest(e.GetPosition(target)) as FrameworkElement;
					if (e.Data.GetData(typeof(XElement)) != null && (element is StackPanel || element.TemplatedParent is Label)) {
						marker = DragDropMarkerAdorner.CreateAdornerContentMove(target, element);
						e.Effects = DragDropEffects.Move;
						return;
					}
				}
				
				e.Effects = DragDropEffects.None;
			} catch (Exception ex) {
				Core.LoggingService.Error(ex);
			}
		}

		void DisplayRectDrop(object sender, DragEventArgs e)
		{
			try {
				if (e.Data.GetData(typeof(XElement)) != null) {
					XElement data = e.Data.GetData(typeof(XElement)) as XElement;
					
					UpdateUndoRedoState();
					
					StackPanel target = sender as StackPanel;
					int x = (int)target.GetValue(Grid.ColumnProperty);
					int y = (int)target.GetValue(Grid.RowProperty);
					
					data.SetAttributeValue(gridColName, x);
					data.SetAttributeValue(gridRowName, y);
					
					Point p = e.GetPosition(target);
					TextBlock block = target.InputHitTest(p) as TextBlock;
					
					if (block != null) {
						XElement element = block.Tag as XElement;
						data.MoveBefore(element);
					} else {
						XElement parent = gridTree;
						XElement element = parent.Elements().LastOrDefault();
						if (data.Parent != null)
							data.Remove();
						if (element == null)
							parent.Add(data);
						else {
							if (element.Parent == null)
								parent.Add(data);
							else
								element.AddAfterSelf(data);
						}
					}
					
					RebuildGrid();
				}
			} catch (Exception ex) {
				Core.LoggingService.Error(ex);
			}
		}
		
		IEnumerable<UIElement> BuildItemsForCell(int row, int column)
		{
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var rowAttrib = element.Attribute(gridRowName) ?? new XAttribute(gridRowName, 0);
						var colAttrib = element.Attribute(gridColName) ?? new XAttribute(gridColName, 0);
						return 	row.ToString() == rowAttrib.Value && column.ToString() == colAttrib.Value;
					}
				);
			
			foreach (var control in controls) {
				var nameAttrib = control.Attribute(XName.Get("Name", CompletionDataHelper.XamlNamespace)) ?? control.Attribute(XName.Get("Name"));
				StringBuilder builder = new StringBuilder(control.Name.LocalName);
				if (nameAttrib != null)
					builder.Append(" (" + nameAttrib.Value + ")");
				
				Label label = new Label() {
					Content = builder.ToString(),
					Template = this.Resources["itemTemplate"] as ControlTemplate,
					AllowDrop = true,
					Tag = control
				};
				
				label.MouseLeftButtonDown += new MouseButtonEventHandler(LabelMouseLeftButtonDown);

				Debug.Assert(label.Tag != null);
				
				yield return label;
			}
		}

		void LabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragDropEffects allowedEffects = DragDropEffects.Move;
			Label label = sender as Label;
			DragDrop.DoDragDrop(label, label.Tag, allowedEffects);
		}
		
		void UpdateUndoRedoState()
		{
			this.undoStack.Push(UndoStep.CreateStep(gridTree, rowDefitions, colDefitions, additionalProperties));
			this.redoStack.Clear();
		}
		
		public XElement ConstructedTree
		{
			get {
				gridTree.AddFirst(additionalProperties);
				gridTree.AddFirst(colDefitions);
				gridTree.AddFirst(rowDefitions);
				
				return gridTree;
			}
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
			
			StackPanel block = sender as StackPanel;
			
			ContextMenu menu = new ContextMenu() {
				Items = {
					undoItem,
					redoItem,
					new Separator(),
					new MenuItem() {
						Header = "Row",
						Items = {
							CreateItem("Insert above", InsertAbove, block),
							CreateItem("Insert below", InsertBelow, block),
							new Separator(),
							CreateItem("Move up", MoveUp, (int)block.GetValue(Grid.RowProperty)),
							CreateItem("Move down", MoveDown, (int)block.GetValue(Grid.RowProperty)),
							new Separator(),
							CreateItem("Delete", DeleteRow, block)
						}
					},
					new MenuItem() {
						Header = "Column",
						Items = {
							CreateItem("Insert before", InsertBefore, block),
							CreateItem("Insert after", InsertAfter, block),
							new Separator(),
							CreateItem("Move left", MoveLeft,  (int)block.GetValue(Grid.ColumnProperty)),
							CreateItem("Move right", MoveRight, (int)block.GetValue(Grid.ColumnProperty)),
							new Separator(),
							CreateItem("Delete", DeleteColumn, block)
						}
					}
				}
			};
			
			menu.IsOpen = true;
		}

		void RedoItemClick(object sender, RoutedEventArgs e)
		{
			if (redoStack.Count > 0)
				HandleSteps(redoStack, undoStack);
		}

		void UndoItemClick(object sender, RoutedEventArgs e)
		{
			if (undoStack.Count > 0)
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
		
		void BtnDeleteItemClick(object sender, RoutedEventArgs e)
		{
			Button source = e.OriginalSource as Button;
			XElement item = source.Tag as XElement;
			if (item != null) {
				UpdateUndoRedoState();
				item.Remove();
			}
			
			RebuildGrid();
		}
		
		void BtnAddRowClick(object sender, RoutedEventArgs e)
		{
			Button b = sender as Button;
			if (b.Tag == null) {
				InsertBelow(gridDisplay.Children.OfType<StackPanel>()
				            .First(item => (int)item.GetValue(Grid.RowProperty) == rowDefitions.Elements().Count() - 1));
			} else {
				InsertAbove(gridDisplay.Children.OfType<StackPanel>()
				            .First(item => (int)item.GetValue(Grid.RowProperty) == (int)b.Tag));
			}
		}
		
		void BtnAddColumnClick(object sender, RoutedEventArgs e)
		{
			Button b = sender as Button;
			if (b.Tag == null) {
				InsertAfter(gridDisplay.Children.OfType<StackPanel>()
				            .First(item => (int)item.GetValue(Grid.ColumnProperty) == colDefitions.Elements().Count() - 1));
			} else {
				InsertBefore(gridDisplay.Children.OfType<StackPanel>()
				             .First(item => (int)item.GetValue(Grid.ColumnProperty) == (int)b.Tag));
			}
		}
		
		void ButtonPanelDrop(object sender, DragEventArgs e)
		{
			try {
				GridLengthEditor source = e.Data.GetData(typeof(GridLengthEditor)) as GridLengthEditor;
				
				if (source != null)
					MoveDown(source.Cell, Math.Abs(source.Cell - rowDefitions.Elements().Count()) - 1);
			} catch (Exception ex) {
				Core.LoggingService.Error(ex);
			}
		}
		
		void DropPanelDrop(object sender, DragEventArgs e)
		{
			try {
				GridLengthEditor source = e.Data.GetData(typeof(GridLengthEditor)) as GridLengthEditor;
				
				if (source != null)
					MoveRight(source.Cell, Math.Abs(source.Cell - colDefitions.Elements().Count()) - 1);
			} catch (Exception ex) {
				Core.LoggingService.Error(ex);
			}
		}
		
		void DropPanelDragOver(object sender, DragEventArgs e)
		{
			try {
				StackPanel target = sender as StackPanel;
				GridLengthEditor source = e.Data.GetData(typeof(GridLengthEditor)) as GridLengthEditor;
				e.Handled = true;
				
				if (marker != null) {
					AdornerLayer.GetAdornerLayer(marker.AdornedElement).Remove(marker);
					marker = null;
				}
				
				if (target != null && source != null && source.Orientation == Orientation.Horizontal && source.Cell + 1 < colDefitions.Elements().Count()) {
					marker = DragDropMarkerAdorner.CreateAdornerCellMove(target);
					e.Effects = DragDropEffects.Move;
					return;
				}
				
				e.Effects = DragDropEffects.None;
			} catch (Exception ex) {
				Core.LoggingService.Error(ex);
			}
		}
		
		void ButtonPanelDragOver(object sender, DragEventArgs e)
		{
			try {
				StackPanel target = sender as StackPanel;
				GridLengthEditor source = e.Data.GetData(typeof(GridLengthEditor)) as GridLengthEditor;
				e.Handled = true;
				
				if (marker != null) {
					AdornerLayer.GetAdornerLayer(marker.AdornedElement).Remove(marker);
					marker = null;
				}
				
				if (target != null && source != null && source.Orientation == Orientation.Vertical && source.Cell + 1 < rowDefitions.Elements().Count()) {
					marker = DragDropMarkerAdorner.CreateAdornerCellMove(target);
					e.Effects = DragDropEffects.Move;
					return;
				}
				
				e.Effects = DragDropEffects.None;
			} catch (Exception ex) {
				Core.LoggingService.Error(ex);
			}
		}
	}
}
