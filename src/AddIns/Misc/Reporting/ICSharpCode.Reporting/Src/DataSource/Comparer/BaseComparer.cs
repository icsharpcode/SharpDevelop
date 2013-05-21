/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.05.2013
 * Time: 20:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.Reporting.DataSource.Comparer
{
	/// <summary>
	/// Description of BaseComparer.
	/// </summary>
	public class BaseComparer : IComparable {
		
		private int listIndex;
		private object[] objectArray;

		ColumnCollection columnCollection;
		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>

		public BaseComparer(ColumnCollection columnCollection , int listIndex, object[] values) {
			this.columnCollection = columnCollection;
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
		

		public ColumnCollection ColumnCollection {
			get {
				return columnCollection;
			}
		}
	}
}
