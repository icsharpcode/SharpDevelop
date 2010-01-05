// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>
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
