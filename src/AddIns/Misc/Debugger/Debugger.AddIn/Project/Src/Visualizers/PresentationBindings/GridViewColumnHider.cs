// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows.Controls;

namespace Debugger.AddIn.Visualizers
{
	public class GridViewColumnWithVisibility
    {
        public bool IsVisible { get; set; }
        public GridViewColumn Column { get; set; }
    }
	
	/// <summary>
	/// Utility for hiding and showing columns in GridView (GridViewColumn does not have Visility).
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

        private void initializeColumns()
        {
            foreach (var column in gridView.Columns)
            {
                allColumns.Add(new GridViewColumnWithVisibility { Column = column, IsVisible = true });
            }
        }

        private GridViewColumnWithVisibility findColumn(string header)
        {
            return allColumns.Where(columnVis => columnVis.Column.Header.ToString() == header).First();
        }

        public void HideColumn(string header)
        {
            var columnVis = findColumn(header);
            if (!columnVis.IsVisible)
            {
                throw new InvalidOperationException("Column is already hidden.");
            }
            else
            {
                var gridColumn = columnVis.Column;
                gridColumn.Width = gridColumn.ActualWidth;
                gridView.Columns.Remove(gridColumn);
                columnVis.IsVisible = false;
            }
        }

        public void ShowColumn(string header)
        {
            int columnsVisibleBefore = 0;
            foreach (var columnVis in this.allColumns)
            {
                if (columnVis.IsVisible)
                {
                    columnsVisibleBefore++;
                }
                if (columnVis.Column.Header.ToString() == header)
                {
                    if (columnVis.IsVisible)
                    {
                        throw new InvalidOperationException("Column is already visible");
                    }
                    else
                    {
                        columnVis.IsVisible = true;
                        gridView.Columns.Insert(columnsVisibleBefore, columnVis.Column);
                    }
                    return;
                }
            }
            throw new ArgumentException("Column not found");
        }
    }
}
