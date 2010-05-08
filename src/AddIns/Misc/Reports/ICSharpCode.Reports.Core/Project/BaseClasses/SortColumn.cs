/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 29.08.2009
 * Zeit: 16:27
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.ComponentModel;



	/// <summary>
	/// This Class represents a Column to sort
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 18.05.2005 11:34:45
	/// </remarks>
	/// 
namespace ICSharpCode.Reports.Core
{	
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

