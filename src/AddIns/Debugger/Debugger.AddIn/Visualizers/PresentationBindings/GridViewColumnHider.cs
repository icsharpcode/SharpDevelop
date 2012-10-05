// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
				if (isVisible != value)	{
					isVisible = value;
					RaisePropertyChanged(() => IsVisible);
					ExtensionMethods.RaiseEvent(IsVisibleChanged, this, EventArgs.Empty);
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
			foreach (var column in gridView.Columns)
			{
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
			foreach (var columnVis in this.allColumns)
			{
				if (columnVis.Header == header)
				{
					columnVis.IsVisible = true;
					gridView.Columns.Insert(columnsVisibleBefore, columnVis.Column);
					return;
				}
				if (columnVis.IsVisible)
				{
					columnsVisibleBefore++;
				}
			}
			throw new ArgumentException("Column not found");
		}
	}
}
