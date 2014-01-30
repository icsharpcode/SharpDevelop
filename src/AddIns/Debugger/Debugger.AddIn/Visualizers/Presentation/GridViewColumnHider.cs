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
using System.Linq;
using System.Windows.Controls;

using Debugger.AddIn.Visualizers.PresentationBindings;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Widgets;

namespace Debugger.AddIn.Visualizers
{
	public class GridViewColumnWithVisibility : ViewModelBase
	{
		public GridViewColumn Column { get; set; }
		
		public string Header
		{
			get { return this.Column.Header.ToString(); }
		}
		
		private bool isVisible = true;
		public bool IsVisible
		{
			get { return isVisible; }
			set {
				if (isVisible == value) return;
				isVisible = value;
				OnPropertyChanged();
				if (IsVisibleChanged != null) {
					IsVisibleChanged(this, EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler IsVisibleChanged;
	}
	
	/// <summary>
	/// Utility for hiding and showing columns in GridView (GridViewColumn does not have Visility property).
	/// </summary>
	public class GridViewColumnHider
	{
		private GridView gridView;
		private List<GridViewColumnWithVisibility> allColumns = new List<GridViewColumnWithVisibility>();

		public GridViewColumnHider(GridView gridView)
		{
			this.gridView = gridView;
			initializeColumns();
		}

		void initializeColumns()
		{
			foreach (var column in gridView.Columns) {
				// show / hide right in the beginning if supported
				bool isDefaultVisible = (column is GridViewHideableColumn) ? ((GridViewHideableColumn)column).IsVisibleDefault : true;
				// wrap into our ViewModel
				var columnWithVisibility = new GridViewColumnWithVisibility { Column = column, IsVisible = isDefaultVisible };
				allColumns.Add(columnWithVisibility);
				columnWithVisibility.IsVisibleChanged += columnWithVisibility_IsVisibleChanged;
			}
		}

		void columnWithVisibility_IsVisibleChanged(object sender, EventArgs e)
		{
			var column = ((GridViewColumnWithVisibility)sender);
			if (column.IsVisible) {
				this.showColumn(column.Header);
			} else	{
				this.hideColumn(column.Header);
			}
		}
		
		bool canBeHidden(GridViewColumn column)
		{
			var columnHideable = column as GridViewHideableColumn;
			// disable hiding if supported
			if (columnHideable != null && (!columnHideable.CanBeHidden)) {
				return false;
			}
			return true;
		}
		
		public IEnumerable<GridViewColumnWithVisibility> HideableColumns
		{
			get { return allColumns.Where(columnWithVisibility => canBeHidden(columnWithVisibility.Column)); }
		}

		private GridViewColumnWithVisibility findColumn(string header)
		{
			return allColumns.First(columnVis => columnVis.Header == header);
		}

		void hideColumn(string header)
		{
			var columnVis = findColumn(header);
			var gridColumn = columnVis.Column;
			gridColumn.Width = gridColumn.ActualWidth;
			gridView.Columns.Remove(gridColumn);
			columnVis.IsVisible = false;
		}

		void showColumn(string header)
		{
			int columnsVisibleBefore = 0;
			foreach (var columnVis in this.allColumns) {
				if (columnVis.Header == header) {
					columnVis.IsVisible = true;
					gridView.Columns.Insert(columnsVisibleBefore, columnVis.Column);
					return;
				}
				if (columnVis.IsVisible) {
					columnsVisibleBefore++;
				}
			}
			throw new ArgumentException("Column '" + header + "' not found.");
		}
	}
}
