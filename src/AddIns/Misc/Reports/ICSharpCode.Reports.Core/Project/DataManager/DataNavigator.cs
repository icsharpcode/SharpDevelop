// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

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
		
		public CurrentItemsCollection GetDataRow()
		{
			return this.store.FillDataRow();
		}
		
		#endregion
		
		
		private static Collection<BaseDataItem> ExtraxtDataItems (ReportItemCollection items)
		{
			Collection<BaseDataItem> inheritedReportItems = new Collection<BaseDataItem>(items.OfType<BaseDataItem>().ToList());
			return inheritedReportItems;
		}
		
		
		#region IDataNavigator implementation
		
		public void Fill (ReportItemCollection collection) {
			foreach (IDataItem item in collection) {
				this.store.Fill(item);
			}
		}
		
		
		public bool HasMoreData {
			get {
				if (this.CurrentRow < this.Count -1 ){
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
		
		
		public bool IsSorted {get {return this.store.IsSorted;}}
		
		public bool IsGrouped {get {return this.store.IsGrouped;}}
	
		
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
		
		
		IndexList childList;
		private System.Collections.Generic.List<BaseComparer>.Enumerator ce;
		
		public void SwitchGroup()
		{
			this.childList = BuildChildList();
			ce = childList.GetEnumerator();
			ce.MoveNext();
		}
		
	
		
		public int ChildListCount
		{
			get {
				return BuildChildList().Count;
			}
		}
		
		
		public bool ChildMoveNext()
		{
			return ce.MoveNext();
		}
		
		
		public void FillChild (ReportItemCollection collection)
		{
			TableStrategy tableStrategy = store as TableStrategy;
			var filteredCollection = ExtraxtDataItems(collection);
			
			foreach (BaseDataItem item in filteredCollection) {
				CurrentItemsCollection currentItemsCollection = tableStrategy.FillDataRow(ce.Current.ListIndex);
				CurrentItem s = currentItemsCollection.FirstOrDefault(x => x.ColumnName == ((BaseDataItem)item).ColumnName);
				item.DBValue = s.Value.ToString();
			}
		}
		
		
		private IndexList BuildChildList()
		{
			var t = store as TableStrategy;
			IndexList i = t.IndexList;
			GroupComparer gc = i[t.CurrentPosition] as GroupComparer;
			if (gc == null) {
				return null;
			}
			return gc.IndexList;
		}
		
		#endregion
	
		#region Try make recursive with ChildNavigavtor
		
		public IDataNavigator GetChildNavigator()
		{
			var i = BuildChildList();
			if ((i == null) || (i.Count == 0)) {
				return null;
			} 
			return new ChildNavigator(this.store,i);
		}
		
		#endregion
	}
}
