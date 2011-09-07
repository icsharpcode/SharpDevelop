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
        
        private DoubleKeyDictionary<TItem, TItem, TValue> cache;

        protected Matrix()
        {
            headerRows = new List<Cell<TItem>>();
            headerColumns = new List<Cell<TItem>>();
            cache = new DoubleKeyDictionary<TItem, TItem, TValue>();
        }
        
        public void AddRow(TItem value)
        {
        	headerRows.Add(new Cell<TItem>(value));
        }
        
        public void AddColumn(TItem value)
        {
        	headerColumns.Add(new Cell<TItem>(value));
        }
        
        private TValue GetFromCache(TItem rowIndex, TItem columnIndex)
    	{
        	return cache[rowIndex, columnIndex];
    	}
    	
    	private void SaveToCache(TItem rowIndex, TItem columnIndex, TValue result)
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
        		
        		var from = HeaderRows[rowIndex].Value;
				var to = HeaderColumns[columnIndex].Value;
        		
        		var cacheResult = GetFromCache(from, to);
        		if (cacheResult != null)
        			return cacheResult;
        		
    			var result = GetCellValue(rowIndex, columnIndex);
    			SaveToCache(from, to, result);
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
