// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.CodeQualityAnalysis.Utility
{
	/// <summary>
	/// Description of VisibleMatrix.
	/// </summary>
	public abstract class VisibleMatrix<TItem, TValue> : Matrix<TItem, TValue>
	{
		public new List<Cell<TItem>> HeaderRows
		{
			get
			{
				return base.HeaderRows.Where(c => c.Visible).ToList();
			}
			
			set
			{
				base.HeaderRows = value;
			}
		}
		
		public new List<Cell<TItem>> HeaderColumns
		{
			get
			{
				return base.HeaderColumns.Where(c => c.Visible).ToList();
			}
			
			set
			{
				base.HeaderColumns = value;
			}
		}
	}
}
