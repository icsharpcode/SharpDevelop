/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 08.11.2005
 * Time: 22:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Reflection;
using SharpReportCore;	

namespace SharpReportCore {
	
	public class SortComparer : BaseComparer {
		
		public SortComparer(ColumnCollection owner, int listIndex, object[] values):
			base(owner,listIndex,values){

		}
		
		internal int CompareTo(SortComparer value)
		{
			// we shouldn't get to this point
			if (value == null)
				throw new ArgumentNullException("value");
			
			if (value.ObjectArray.Length != base.ObjectArray.Length)
				throw new InvalidOperationException("Differnet size of compare data");
			
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
				
				
				if (leftValue.GetType() != rightValue.GetType())
					throw new InvalidOperationException("COmpare of different types is not supported");
				
				if (leftValue.GetType() == typeof(string))
				{
					compare = String.Compare((string)leftValue, (string)rightValue,
					                         !sortColumn.CaseSensitive, base.ColumnCollection.Culture);
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
