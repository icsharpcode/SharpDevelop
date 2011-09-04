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
        public List<Cell<TItem>> HeaderRows { get; set; }
        public List<Cell<TItem>> HeaderColumns { get; set; }
        
        private DoubleKeyDictionary<int, int, TValue> cache;

        protected Matrix()
        {
            HeaderRows = new List<Cell<TItem>>();
            HeaderColumns = new List<Cell<TItem>>();
            cache = new DoubleKeyDictionary<int, int, TValue>();
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
}
