// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// This Class represents a Column to sort
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 18.05.2005 11:34:45
	/// </remarks>
	public class SortColumn : AbstractColumn {
		
		private ListSortDirection sortDirection = ListSortDirection.Ascending;
		private bool caseSensitive;
		
		
		public SortColumn():this(String.Empty,ListSortDirection.Ascending,typeof(System.String),false)
		{
		}
		
		public SortColumn(string columnName,Type type ):this(columnName,ListSortDirection.Ascending,type,false)
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
		
		public ListSortDirection SortDirection {
			get {
				return sortDirection;
			}
			set{
				this.sortDirection = value;
			}
		}
		public bool CaseSensitive {
			get {
				return caseSensitive;
			}
		}
		
		#endregion
	}
}
