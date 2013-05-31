/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.05.2013
 * Time: 20:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;

namespace ICSharpCode.Reporting.BaseClasses
{
	/// <summary>
	/// Description of SortColumn.
	/// </summary>
	public class SortColumn : AbstractColumn {
		
		private ListSortDirection sortDirection = ListSortDirection.Ascending;
		private bool caseSensitive;
		
		
		public SortColumn():this(String.Empty,ListSortDirection.Ascending,typeof(System.String),false)
		{
		}
		

		public SortColumn(string columnName,ListSortDirection sortDirection)
			:this(columnName,sortDirection,typeof(System.String),false){
		}
		
		
		public SortColumn(string columnName, ListSortDirection sortDirection, Type type,bool caseSensitive ):base (columnName,type)
		{
			this.caseSensitive = caseSensitive;
			this.sortDirection = sortDirection;
		}
		
		#region properties
		
		public ListSortDirection SortDirection {get;set;}
		
			
		public bool CaseSensitive {get;private set;}
			
		
		#endregion
	}
}
