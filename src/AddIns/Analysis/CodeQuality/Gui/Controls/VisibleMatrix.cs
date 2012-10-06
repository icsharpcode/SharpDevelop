// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.CodeQuality.Gui
{
	/// <summary>
	/// Description of VisibleMatrix.
	/// </summary>
	public abstract class VisibleMatrix<TItem, TValue> : Matrix<TItem, TValue>
	{
		protected List<Cell<TItem>> visibleHeaderRows;
		protected List<Cell<TItem>> visibleHeaderColumns;
		
		public override List<Cell<TItem>> HeaderRows {
			get {
				return visibleHeaderRows;
			}
		}
		
		public override List<Cell<TItem>> HeaderColumns {
			get {
				return visibleHeaderColumns;
			}
		} 
		
		public override void SetVisibleItems(HeaderType type, ICollection<TItem> visibleItems)
		{
			var items = type == HeaderType.Columns ? headerColumns : headerRows;
			
			foreach (var item in items) {
				var foundItem = visibleItems.SingleOrDefault(n => n.Equals(item.Value));
				item.Visible = foundItem != null;
			}
			
			visibleHeaderRows = headerRows.Where(c => c.Visible).ToList();
			visibleHeaderColumns = headerColumns.Where(c => c.Visible).ToList();
		}
	}
}
