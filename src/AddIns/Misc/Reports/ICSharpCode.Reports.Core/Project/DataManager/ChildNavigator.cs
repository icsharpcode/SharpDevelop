// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.ComponentModel;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of ChildNavigator.
	/// </summary>
	public class ChildNavigator:IDataNavigator
	{
		private IndexList indexList;
		private IDataViewStrategy dataStore;
		private System.Collections.Generic.List<BaseComparer>.Enumerator enumerator;
			
		public ChildNavigator(IDataViewStrategy dataStore,IndexList indexList)
		{
			if (dataStore == null) {
				throw new ArgumentNullException("dataStore");
			}
			this.dataStore = dataStore;
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
                return dataStore.CurrentFromPosition(this.indexList[CurrentRow].ListIndex);
			}
		}
		
		
		public AvailableFieldsCollection AvailableFields {
			get {
				return dataStore.AvailableFields;
			}
		}
	

        public void Fill(ReportItemCollection collection)
        {
            var position = this.indexList[this.indexList.CurrentPosition].ListIndex;
            dataStore.Fill(position,collection);
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
		
		
		public CurrentItemsCollection GetDataRow
		{
			get {
               var position = this.indexList[this.indexList.CurrentPosition].ListIndex;
               return dataStore.FillDataRow(position);
			}
		}


		public IDataNavigator GetChildNavigator
		{
			get {
				var i = BuildChildList();
				if ((i == null) || (i.Count == 0)) {
					return null;
				}
				return new ChildNavigator(this.dataStore,i);
			}
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
