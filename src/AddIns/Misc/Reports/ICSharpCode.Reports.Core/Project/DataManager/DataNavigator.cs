// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of DataNavigator.
	/// </summary>
	public class DataNavigator :IDataNavigator
//	public class DataNavigator :IDataNavigator,IEnumerable
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
		
		#region IDataNavigator implementation
		
		public void Fill (ReportItemCollection collection) {
			foreach (IReportItem item in collection) {
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
		
		
		#region IEnumarable
		/*
		public IEnumerator RangeEnumerator(int start, int end)
        {
			if (start > end) {
				throw new ArgumentException("start-end");
			}
			for (int i = start; i <= end; i++)
            {
            	IDataViewStrategy d = this.store as IDataViewStrategy;
            	d.CurrentPosition = i;
            	yield return this.Current;
            }
        }

		
		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i =0;i < this.Count;i++){
				this.store.MoveNext();
				yield return this.Current;
			}
		}
		*/
		#endregion
		
		
		#region aaaaaa
		
		public bool HasChildren
		{
			get{
				TableStrategy t = store as TableStrategy;
				IndexList i = t.IndexList;
				GroupComparer gc = i[t.CurrentPosition] as GroupComparer;
				return gc.IndexList.Count > 0;
			}
		}
		
		IndexList childList;
		private System.Collections.Generic.List<BaseComparer>.Enumerator ce;
		
		public void MoveToChilds()
		{
			childList = new IndexList();
			TableStrategy t = store as TableStrategy;
			IndexList i = t.IndexList;
			GroupComparer gc = i[t.CurrentPosition] as GroupComparer;
			childList = gc.IndexList;
			ce = childList.GetEnumerator();
			ce.MoveNext();
		}
		
		public object ReadChild()
		{
			
			var i = ce.Current.ListIndex;
			TableStrategy t = store as TableStrategy;
			return t.Readrandowm (i);
		}
		
		public bool ChildMoveNext()
		{
			return ce.MoveNext();
		}
		#endregion
	
		
	}
	
	
	public class ChildNavigator
	{
		public ChildNavigator (IndexList list)
		{
			IndexList = list;
		}
		
		public IndexList IndexList {get; private set;}
	}
		
}
