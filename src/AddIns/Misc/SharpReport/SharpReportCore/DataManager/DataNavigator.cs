/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 23.03.2006
 * Time: 13:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.ComponentModel;

namespace SharpReportCore
{
	/// <summary>
	/// Description of DataNavigator.
	/// </summary>
	
	
	public class DataNavigator :IDataNavigator,IEnumerable{
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
		/*
		public delegate void TestDelegate(string s);
		public delegate bool TestBoolDelegate(string s,string ss);
		
		public SharpIndexCollection PlainDelegate (TestDelegate t) {
			SharpIndexCollection cl = new SharpIndexCollection("Filtered");
System.Collections.IEnumerable e = (this as System.Collections.IEnumerable);
			System.Collections.IEnumerator en = e.GetEnumerator();
			while(en.MoveNext()){
				System.Console.WriteLine("\twhile");
				t("x");
			}
			
			return cl;
		}
	
		public SharpIndexCollection BoolDelegate (TestBoolDelegate t) {
			SharpIndexCollection cl = new SharpIndexCollection("Filtered");
			System.Collections.IEnumerable e = (this as System.Collections.IEnumerable);
			System.Collections.IEnumerator en = e.GetEnumerator();
			while(en.MoveNext()){
				System.Console.WriteLine("\t{0}",en.Current);
				if (t("x","y")) {
				    	System.Console.WriteLine("added");
				}
			}
			return cl;
		}
		*/
		
		/*
		public void Scroll() {
			System.Console.WriteLine("");
			System.Console.WriteLine("----Scroll did we need this -----");
			System.Collections.IEnumerable e = (store as System.Collections.IEnumerable);
			System.Collections.IEnumerator en = e.GetEnumerator();
			while (en.MoveNext()){
				System.Console.WriteLine("{0} ",en.Current);
			}
			System.Console.WriteLine("----");
		}
		*/
		
		#region IEnumarable
		public System.Collections.IEnumerable RangeTester(int start, int end)
        {
			System.Console.WriteLine("RangeIterator Range form {0} to {1}",start,end);
			for (int i = start; i <= end; i++)
            {
            	IDataViewStrategy d = this.store as IDataViewStrategy;
            	d.CurrentRow = i;
            	yield return this.Current;
            }
        }

		
		IEnumerator IEnumerable.GetEnumerator(){
			System.Console.WriteLine("Navi:IEnumerable.GetEnumerator() {0}",this.Count);
				for (int i =0;i < this.Count;i++){
				this.store.MoveNext();
				yield return this.Current;
			}
		}
		
		#endregion
	}
}
