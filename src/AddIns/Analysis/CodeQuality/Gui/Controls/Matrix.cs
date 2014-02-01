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
using System.Text;

namespace ICSharpCode.CodeQuality.Gui
{
	public abstract class Matrix<TItem, TValue>
	{
		protected List<Cell<TItem>> headerRows;
		protected List<Cell<TItem>> headerColumns;
		
		public virtual List<Cell<TItem>> HeaderRows
		{
			get {
				return headerRows;
			}
		}
		
		public virtual List<Cell<TItem>> HeaderColumns
		{
			get {
				return headerColumns;
			}
		}
		
		protected Matrix()
		{
			headerRows = new List<Cell<TItem>>();
			headerColumns = new List<Cell<TItem>>();
		}
		
		public void AddRow(TItem value)
		{
			headerRows.Add(new Cell<TItem>(value));
		}
		
		public void AddColumn(TItem value)
		{
			headerColumns.Add(new Cell<TItem>(value));
		}
		
		public TValue this[int rowIndex, int columnIndex]
		{
			get
			{
				if (rowIndex > HeaderRows.Count || rowIndex < 0 ||
				    columnIndex > HeaderColumns.Count || columnIndex < 0)
					return default(TValue);
				
				var from = HeaderRows[rowIndex].Value;
				var to = HeaderColumns[columnIndex].Value;
				
				var result = GetCellValue(rowIndex, columnIndex);
				return result;
			}
		}
		
		public abstract void SetVisibleItems(HeaderType type, ICollection<TItem> visibleItems);
		protected abstract TValue GetCellValue(int rowIndex, int columnIndex);
	}

	public class Cell<TItem>
	{
		public TItem Value { get; set; }
		public bool Visible { get; set; }

		public Cell(TItem value)
		{
			Value = value;
		}
	}
	
	public enum HeaderType
	{
		Columns,
		Rows
	}
}
