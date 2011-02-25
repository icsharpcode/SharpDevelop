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
            var ss = this.indexList[this.indexList.CurrentPosition].ListIndex;
            var current = dataStore.CurrentFromPosition(ss);
            foreach (IDataItem item in collection)
            {
                FillInternal(current, item);
            }
        }
		

		private void FillInternal(object fillFrom,IDataItem item)
		{
			if (item is BaseDataItem)
			{
				var retVal = CollectionStrategy.FollowPropertyPath(fillFrom,item.ColumnName);
				if (retVal != null) {
					item.DBValue = retVal.ToString();
				} else {
					item.DBValue = String.Empty;
				}
			}
			
			else
			{
				/*
				//image processing from IList
				BaseImageItem baseImageItem = item as BaseImageItem;
				
				if (baseImageItem != null) {
					PropertyDescriptor p = this.listProperties.Find(baseImageItem.ColumnName, true);
					if (p != null) {
						baseImageItem.Image = p.GetValue(this.Current) as System.Drawing.Image;
					}
					return;
				}
				*/
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
		
		
		public CurrentItemsCollection GetDataRow
		{
			get {
               var ss = this.indexList[this.indexList.CurrentPosition].ListIndex;
               return dataStore.FillDataRow(ss);
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
