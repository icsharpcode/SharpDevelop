/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 27.07.2010
 * Time: 16:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of ChildNavigator.
	/// </summary>
	public class ChildNavigator:IDataNavigator
	{
		IndexList indexList;
		IDataViewStrategy dataStore;
			private System.Collections.Generic.List<BaseComparer>.Enumerator ce;
			
		public ChildNavigator(IDataViewStrategy dataStore,IndexList indexList)
		{
		if (dataStore == null) {
				
				throw new ArgumentNullException("dataStore");
		}	
			this.dataStore = dataStore;
			this.indexList = indexList;
			ce = this.indexList.GetEnumerator();
			ce.MoveNext();
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
//				return false;
			}
		}
		
		public int ChildListCount {
			get {
				throw new NotImplementedException();
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
		
		public int CurrentRow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int Count {
			get {
				return this.indexList.Count;
			}
		}
		
		public object Current {
			get {
				TableStrategy t = this.dataStore as TableStrategy;
				return t.myCurrent(ce.Current.ListIndex);
//					return ci;
			}
		}
		
		public AvailableFieldsCollection AvailableFields {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void Fill(ReportItemCollection collection)
		{
			throw new NotImplementedException();
		}
		
		public bool MoveNext()
		{
			return this.ce.MoveNext();
		}
		
		public void Reset()
		{
			throw new NotImplementedException();
		}
		
		public CurrentItemsCollection GetDataRow()
		{
			throw new NotImplementedException();
		}
		
		public IDataNavigator GetChildNavigator()
		{
			var i = BuildChildList();
			if ((i == null) || (i.Count == 0)) {
				return null;
			} 
			return new ChildNavigator(this.dataStore,i);
		}
		
		public void SwitchGroup()
		{
			throw new NotImplementedException();
		}
		
		public bool ChildMoveNext()
		{
			throw new NotImplementedException();
		}
		
		public void FillChild(ReportItemCollection collection)
		{
			throw new NotImplementedException();
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
