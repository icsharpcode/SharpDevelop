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
	
	public class SortColumnCollection: Collection<AbstractColumn>
	{
		public SortColumnCollection()
		{
		}
		
		public AbstractColumn Find (string columnName)
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
}
