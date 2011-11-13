// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
	
namespace ICSharpCode.Reports.Core
{
	
	/// <summary>
	/// This Class build an Grouping Item
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 05.05.2005 22:26:29
	/// </remarks>
	public class GroupColumn : SortColumn
	{
		int groupLevel;
		
		public GroupColumn():this("",0,ListSortDirection.Ascending)
		{
		}
		
		public GroupColumn(string columnName,int groupLevel, ListSortDirection sortDirection):base(columnName,sortDirection)
		{
			this.groupLevel = groupLevel;
			if (groupLevel < 0) {
				throw new GroupLevelException();
			}
		}
		

		public int GroupLevel {
			get {
				return groupLevel;
			}
		}

	}
}
