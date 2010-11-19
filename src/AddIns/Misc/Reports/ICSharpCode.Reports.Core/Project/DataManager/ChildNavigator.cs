// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of ChildNavigator.
	/// </summary>
	public class ChildNavigator:IDataNavigator
	{
		private IndexList indexList;
		private IDataViewStrategy store;
		private System.Collections.Generic.List<BaseComparer>.Enumerator enumerator;
			
		public ChildNavigator(IDataViewStrategy dataStore,IndexList indexList)
		{
			if (dataStore == null) {
				throw new ArgumentNullException("dataStore");
			}
			this.store = dataStore;
			this.indexList = indexList;
			enumerator = this.indexList.GetEnumerator();
			enumerator.MoveNext();
		}
		
		
		public bool HasMoreData {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool HasChildren {
			get {
				IndexList ind = BuildChildList();
				return ((ind != null) && (ind.Count > 0));
			}
		}
		
		
		public bool IsSorted {
			get {
				throw new NotImplementedException();
			}
		}
		
		
		public bool IsGrouped {
			get {
				throw new NotImplementedException();
			}
		}
		
		
		public int CurrentRow 
		{
			get {return this.indexList.CurrentPosition;}
		}
		
		
		public int Count {
			get {
				return this.indexList.Count;
			}
		}
		
		
		public object Current {
			get {
				TableStrategy t = this.store as TableStrategy;
//				return t.myCurrent(enumerator.Current.ListIndex);
				return t.myCurrent(this.indexList[CurrentRow].ListIndex);
			}
		}
		
		
		public AvailableFieldsCollection AvailableFields {
			get {
				return store.AvailableFields;
			}
		}
		
		
		public void Fill(ReportItemCollection collection)
		{
			TableStrategy tableStrategy =  store as TableStrategy;
			foreach (var item in collection) {
				IDataItem dataItem = item as IDataItem;
				if (dataItem != null) {
					CurrentItemsCollection currentItemsCollection = tableStrategy.FillDataRow(this.indexList[CurrentRow].ListIndex);
					CurrentItem s = currentItemsCollection.FirstOrDefault(x => x.ColumnName == dataItem.ColumnName);
					dataItem.DBValue = s.Value.ToString();
				}
				
			}
		}
		
		public bool MoveNext()
		{
			this.indexList.CurrentPosition ++;
			return this.indexList.CurrentPosition<this.indexList.Count;
		}
		
		public void Reset()
		{
			this.indexList.CurrentPosition = -1;
		}
		
		public CurrentItemsCollection GetDataRow()
		{
			var st= store as TableStrategy;
			return st.FillDataRow(this.indexList[CurrentRow].ListIndex);
		}
		
		
		public IDataNavigator GetChildNavigator()
		{
			var i = BuildChildList();
			if ((i == null) || (i.Count == 0)) {
				return null;
			} 
			return new ChildNavigator(this.store,i);
		}
		
		
		private IndexList BuildChildList()
		{
			GroupComparer gc = this.indexList[this.indexList.CurrentPosition] as GroupComparer;
			if (gc == null) {
				return null;
			}
			return gc.IndexList;
		}
	}
}
