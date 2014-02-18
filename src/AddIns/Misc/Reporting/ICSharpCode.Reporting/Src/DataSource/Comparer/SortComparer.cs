// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Items;

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
