/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 24.05.2013
 * Time: 20:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;

namespace ICSharpCode.Reporting.BaseClasses
{
	/// <summary>
	/// Description of GroupColumn.
	/// </summary>
	public class GroupColumn : SortColumn
	{

		public GroupColumn():this("",0,ListSortDirection.Ascending)
		{
		}
		
		public GroupColumn(string columnName,int groupLevel, ListSortDirection sortDirection):base(columnName,sortDirection)
		{
			if (GroupLevel < 0) {
				throw new ArgumentException("groupLevel");
			}
			this.GroupLevel = groupLevel;
			
		}
		

		public int GroupLevel {get;private set;}
	
		
	}
}
