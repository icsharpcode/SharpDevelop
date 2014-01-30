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
using ICSharpCode.Reporting.BaseClasses;

namespace ICSharpCode.Reporting.DataSource.Comparer
{
	/// <summary>
	/// Description of BaseComparer.
	/// </summary>
	public class BaseComparer : IComparable {
		
		private int listIndex;
		private object[] objectArray;

		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>

		public BaseComparer(Collection<AbstractColumn> columnCollection , int listIndex, object[] values) {
			this.ColumnCollection = columnCollection;
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
		

		public Collection<AbstractColumn> ColumnCollection {get;private set;}
			
	}
}
