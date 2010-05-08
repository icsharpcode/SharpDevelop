// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;

/// <summary>
/// This Class is the BaseClass for all Comparers
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 10.11.2005 23:41:04
/// </remarks>
namespace ICSharpCode.Reports.Core {	
	public class BaseComparer : System.IComparable {
		
		private int listIndex;
		private object[] objectArray;
//		private SortColumnCollection columnCollection;
		Collection<AbstractColumn> columnCollection;
		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
//		public BaseComparer(ColumnCollection owner, int listIndex, object[] values) {
//		public BaseComparer(SortColumnCollection owner, int listIndex, object[] values) {
		public BaseComparer(Collection<AbstractColumn> owner, int listIndex, object[] values) {
			this.columnCollection = owner;
			this.listIndex = listIndex;
			this.objectArray = values;
		}
		
		/// <summary>
		/// Interface method from IComparable
		/// </summary>
		/// <remarks>
		/// Interface method from IComparable
		/// 
		/// </remarks>
		/// <param name='obj'>a <see cref="BaseComparer"></see></param>
		public virtual int CompareTo(object obj) {
			return 0;
		}
		
		/// <summary>
		/// Ausgeben der Werte als Pseudo-CSV
		/// </summary>
		public override string ToString()
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.AppendFormat("{0};", this.listIndex);
			foreach (object value in objectArray)
			{
				if (value == null || value == DBNull.Value)
				{
					builder.AppendFormat("<NULL>");
				}
				else if (value.GetType() == typeof(string))
				{
					builder.AppendFormat("\"{0}\"", (string)value);
				}
				else if (value is IFormattable)
				{
					builder.AppendFormat("{0}", ((IFormattable)value).ToString("g", System.Globalization.CultureInfo.InvariantCulture));
				}
				else
				{
					builder.AppendFormat("[{0}]", value.ToString());
				}
				builder.Append(';');
			}
			return builder.ToString();
		}

		
		public int ListIndex {
			get {
				return listIndex;
			}
		}
		
		public object[] ObjectArray {
			get {
				return objectArray;
			}
		}
		
		public Collection<AbstractColumn> ColumnCollection {
//		public SortColumnCollection ColumnCollection {
			get {
				return columnCollection;
			}
		}
		
		
		
		
	}
}
