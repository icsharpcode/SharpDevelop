/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.05.2013
 * Time: 20:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting
{
	/// <summary>
	/// Description of Collections.
	/// </summary>
	
	public class ColumnCollection: Collection<AbstractColumn>{
		
		public ColumnCollection()
		{
		}
		
		public AbstractColumn Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			
			return this.FirstOrDefault(x => 0 == String.Compare(x.ColumnName,columnName,true,CultureInfo.InvariantCulture));
		}
	
		
		public void AddRange (IEnumerable<AbstractColumn> items)
		{
			foreach (AbstractColumn item in items){
				this.Add(item);
			}
		}
		
		
		/// <summary>
		/// The Culture is used for direct String Comparison
		/// </summary>
		
		public static CultureInfo Culture
		{
			get { return CultureInfo.CurrentCulture;}
		}
	}
	
	
	
	public class SortColumnCollection: ColumnCollection
	{
		public SortColumnCollection()
		{
		}
		
		public new AbstractColumn Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			
			return this.FirstOrDefault(x => 0 == String.Compare(x.ColumnName,columnName,true,CultureInfo.InvariantCulture));
		}
	
		
		public void AddRange (IEnumerable<SortColumn> items)
		{
			foreach (SortColumn item in items){
				this.Add(item);
			}
		}
	}
	
	
	public class GroupColumnCollection: SortColumnCollection
	{
		public GroupColumnCollection()
		{
		}
		
		public new AbstractColumn Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			
			return this.FirstOrDefault(x => 0 == String.Compare(x.ColumnName,columnName,true,CultureInfo.InvariantCulture));
		}
	}
	
	
	public class ReportItemCollection : Collection<PrintableItem>
	{
		
		// Trick to get the inner list as List<T> (InnerList always has that type because we only use
		// the parameterless constructor on Collection<T>)
		
		private List<PrintableItem> InnerList {
			get { return (List<PrintableItem>)base.Items; }
		}
		
		private void Sort(IComparer<PrintableItem> comparer)
		{
			InnerList.Sort(comparer);
		}
	}
}
