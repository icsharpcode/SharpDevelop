// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;

/// <summary>
///
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 28.11.2005 13:43:22
/// </remarks>

namespace ICSharpCode.Reports.Core
{
	public  class GroupComparer :SortComparer
	{
		
		public GroupComparer (ColumnCollection owner, int listIndex, object[] values):base(owner,listIndex,values)
		{
			IndexList = new IndexList();
		}
		
		public IndexList IndexList {get;set;}
	}
}
