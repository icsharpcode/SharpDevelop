/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 22.05.2013
 * Time: 20:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

using ICSharpCode.Reporting.BaseClasses;

namespace ICSharpCode.Reporting.DataSource.Comparer
{
	/// <summary>
	/// Description of SortComparer.
	/// </summary>
	public class SortComparer : BaseComparer {
		
		public SortComparer(Collection<AbstractColumn> owner, int listIndex, object[] values):base(owner,listIndex,values)
		{
		}
		
		
		internal int CompareTo(SortComparer value)
		{
			// we shouldn't get to this point
			if (value == null)
				throw new ArgumentNullException("value");
			
			if (value.ObjectArray.Length != base.ObjectArray.Length)
				throw new InvalidOperationException();
			
			int compare = 0;
			
			for (int index = 0; index < base.ObjectArray.Length; index++)
			{
				object leftValue = base.ObjectArray[index];

				object rightValue = value.ObjectArray[index];

				// Indizes sind hier deckungsgleich

				SortColumn sortColumn = (SortColumn)base.ColumnCollection[index];

				bool descending = (sortColumn.SortDirection == ListSortDirection.Descending);
				
				// null means equl
				if (leftValue == null || leftValue == System.DBNull.Value)
				{
					if (rightValue != null && rightValue != System.DBNull.Value)
					{
						return (descending) ? 1 : -1;
					}
					
					// Beide Null
					continue;
				}
				
				if (rightValue == null || rightValue == System.DBNull.Value)
				{
					return (descending) ? -1 : 1;
				}
				
				
				if (leftValue.GetType() != rightValue.GetType()){
					string s = String.Format(CultureInfo.CurrentCulture,
					                         "{0} {1} {2}",this.GetType().ToString(),
					                         leftValue.GetType().ToString(),
					                         rightValue.GetType().ToString());
					
					throw new ArgumentException(s);
				}
				if (leftValue.GetType() == typeof(string))
				{
					compare = String.Compare((string)leftValue, (string)rightValue,
					                         !sortColumn.CaseSensitive, CultureInfo.CurrentCulture);
				}
				else
				{
					compare = ((IComparable)leftValue).CompareTo(rightValue);
				}
				
				// Sind ungleich, tauschen je nach Richtung
				if (compare != 0)
				{
					return (descending) ? -compare : compare;
				}
			}
			
			// Gleich Werte, dann Index bercksichtigen
			return this.ListIndex.CompareTo(value.ListIndex);
		}
		public override int CompareTo(object obj) {
			base.CompareTo(obj);
			return this.CompareTo((SortComparer)obj);
		}
	
		
	}
}
