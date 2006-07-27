/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 23.03.2006
 * Time: 13:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
//using System.Collections;

namespace SharpReportCore
{
	/// <summary>
	/// Description of DataNavigator.
	/// </summary>
	
	public class DataNavigator :IDataNavigator{
		IDataViewStrategy store;
		GroupSeperator groupSeperator;
		
		public event EventHandler <ListChangedEventArgs> ListChanged;
//		public event EventHandler <EventArgs> GroupChanging;
		public event EventHandler <GroupChangedEventArgs> GroupChanged;
		
		public DataNavigator(IDataViewStrategy store){
			this.store = store;
			this.store.ListChanged += new EventHandler<ListChangedEventArgs> (OnListChanged);
			this.store.GroupChanged += new EventHandler<GroupChangedEventArgs> (OnGroupChange);
		}

		private void OnListChanged (object sender,System.ComponentModel.ListChangedEventArgs e) {
			if (this.ListChanged != null) {
				this.ListChanged (this,e);
			}
		}
		
//		private void NotifyGroupChanging () {
//			if (this.GroupChanging!= null) {
//				this.GroupChanging (this,EventArgs.Empty);
//			}		
//		}
		
		private void NotifyGroupChanged() {
			if (this.store.IsGrouped) {
				if (this.GroupChanged != null) {
					this.GroupChanged (this,new GroupChangedEventArgs(this.groupSeperator));
				}
			}
		}
		
		private void OnGroupChange (object sender,GroupChangedEventArgs e) {
			this.groupSeperator = e.GroupSeperator;
			this.NotifyGroupChanged();
		}
		
		#region IDataNavigator implementation
		
		public void Fill (ReportItemCollection collection) {
//			this.NotifyGroupChanging();
			this.NotifyGroupChanged();
			foreach (IItemRenderer item in collection) {
				this.store.Fill(item);
			}
//			this.NotifyGroupChanged();
		}
		
		
		public bool HasMoreData {
			get {
				if (this.CurrentRow < this.Count ){
					return true;
				} else {
					return false;
				}
			}
		}
		
		
		/// <summary>
		/// Indicate's if the current <see cref="GroupSeperator"></see> has ChildRows
		/// </summary>
		public bool HasChilds {
			get {
				return this.store.HasChilds;
			}
		}
		
		public int CurrentRow  {
			get {return this.store.CurrentRow;}
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
	}
}
