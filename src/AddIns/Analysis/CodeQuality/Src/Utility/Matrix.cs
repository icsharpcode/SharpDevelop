// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.CodeQualityAnalysis.Utility
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
        
        private DoubleKeyDictionary<int, int, TValue> cache;

        protected Matrix()
        {
            headerRows = new List<Cell<TItem>>();
            headerColumns = new List<Cell<TItem>>();
            cache = new DoubleKeyDictionary<int, int, TValue>();
        }
        
        public void AddRow(TItem value)
        {
        	headerRows.Add(new Cell<TItem>(value));
        }
        
        public void AddColumn(TItem value)
        {
        	headerColumns.Add(new Cell<TItem>(value));
        }
        
        private TValue GetFromCache(int rowIndex, int columnIndex)
    	{
        	return cache[rowIndex, columnIndex];
    	}
    	
    	private void SaveToCache(int rowIndex, int columnIndex, TValue result)
    	{
    		cache.Add(rowIndex, columnIndex, result);
    	}
        
        public TValue this[int rowIndex, int columnIndex]
		{
        	get
        	{
        		if (rowIndex > HeaderRows.Count || rowIndex < 0 ||
        		    columnIndex > HeaderColumns.Count || columnIndex < 0)
        			return default(TValue);
        		
        		var cacheResult = GetFromCache(rowIndex, columnIndex);
        		if (cacheResult != null)
        			return cacheResult;
        		
    			var result = GetCellValue(rowIndex, columnIndex);
    			SaveToCache(rowIndex, columnIndex, result);
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
