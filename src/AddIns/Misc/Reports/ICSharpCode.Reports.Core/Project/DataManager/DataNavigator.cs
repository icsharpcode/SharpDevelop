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
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of DataNavigator.
	/// </summary>
	public class DataNavigator :IDataNavigator
	{
		
		private IDataViewStrategy store;
		
		public DataNavigator(IDataViewStrategy store){
			this.store = store;
		}

		
		#region test
		
		public CurrentItemsCollection GetDataRow
		{
			get {return this.store.FillDataRow();}
		}
		
		#endregion
		
		
		#region IDataNavigator implementation
		
		public void Fill (ReportItemCollection collection) {
			foreach (var item in collection) {
				IDataItem dataItem = item as IDataItem;
				if (dataItem != null) {
					this.store.Fill(dataItem);
				}
			}
		}
		
		
		public bool HasMoreData
		{
			get {
				if (this.CurrentRow < store.IndexList.Count -1 ){
					return true;
				} else {
					return false;
				}
			}
		}
		
		
		public AvailableFieldsCollection AvailableFields{
			get
			{
				return this.store.AvailableFields;
			}
		}
		
		
		public int CurrentRow  {
			get {return this.store.CurrentPosition;}
		}
	
		
		public int Count  {
			get {return this.store.Count;}
		}
		
		
		public bool MoveNext () {
			return this.store.MoveNext();
		}
		
		
		public void Reset() {
			this.store.Reset();
		}
		
		
		public object Current {
			get {
				return this.store.Current;
			}
		}
		
		#endregion
		
		
		#region GroupedList
		
		public bool HasChildren
		{
			get{
				IndexList ind = BuildChildList();
				return ((ind != null) && (ind.Count > 0));
			}
		}
	
		
		private IndexList BuildChildList()
		{

			IndexList i = store.IndexList;
			GroupComparer gc = i[store.CurrentPosition] as GroupComparer;
			if (gc == null) {
				return null;
			}
			return gc.IndexList;
		}
		
		#endregion
	
		
		#region Try make recursive with ChildNavigavtor
		
		public IDataNavigator GetChildNavigator
		{
			get {
				var i = BuildChildList();
				if ((i == null) || (i.Count == 0)) {
					return null;
				}
				return new ChildNavigator(this.store,i);
			}
		}
		
		#endregion
	}
}
