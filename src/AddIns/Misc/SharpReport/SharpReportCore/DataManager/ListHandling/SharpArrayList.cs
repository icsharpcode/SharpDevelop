using System;
using System.Data;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

namespace SharpReportCore
{
	/// <summary>
	/// This Class is the BaseClass for all Lists witch handles sorting,grouping etc.
	
	/// </summary>
	public class SharpArrayList : ArrayList, IBindingList ,ITypedList,IExtendedList
	{
		Type elementType;
		string name;
		int currentPosition;
		
		bool allowNew = true;
		bool allowEdit = true;
		bool allowRemove = true;
		bool supportsSearching = false;
		bool supportsSorting = false;
		bool isSorted = false;
		
		private ListChangedEventArgs resetEvent = new ListChangedEventArgs(ListChangedType.Reset, -1);

		public event ListChangedEventHandler ListChanged;
		
		
		public SharpArrayList(Type elementType,string name){
			this.Clear();
			this.elementType = elementType;
			this.name = name;
			Reset();
		}
		
		#region ITypedList Member

		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors){			
			if (listAccessors != null && listAccessors.Length > 0){
				Type t = this.elementType;
				for(int i = 0; i < listAccessors.Length; i++){
					PropertyDescriptor pd = listAccessors[i];
					// System.Diagnostics.Debug.WriteLine("*** " + t.FullName + ": " + pd.Name);
					t = (Type) PropertyTypeHash.Instance[t, pd.Name];
				}
				// System.Diagnostics.Debug.WriteLine("*** New Collection for " + t.FullName);
				// if t is null an empty list will be generated
				return SharpTypeDescriptor.GetProperties(t);
			}
			return SharpTypeDescriptor.GetProperties(elementType);
		}

		public static Type GetElementType(IList list,
		                                  Type parentType,
		                                  string propertyName){			
			SharpArrayList al = list as SharpArrayList;
			if (al == null)
				return null;
			return al.elementType;
		}
#if longVersion
		public static Type GetElementType(IList list, 
                                  Type parentType, 
                                  string propertyName){
			SharpArrayList al = null;
			object element = null;
			al = CheckForArrayList(list);
			if (al == null){
				if (list.Count > 0){
					element = list[0];
				}
			}
			if (al == null && element == null){
				PropertyInfo pi = parentType.GetProperty(propertyName);
				if (pi != null){
					object parentObject = null;
					try{
						parentObject = Activator.CreateInstance(parentType);
					}
					catch(Exception ex) {}
					
					if (parentObject != null){
						list = pi.GetValue(parentObject, null) as IList;
						al = CheckForArrayList(list);
					}
				}
			}
			if (al != null){
				return al.elementType;
			}
			else if (element != null){
				return element.GetType();
			}
			return null;
		}


		private static SharpArrayList CheckForArrayList(object l){
			IList list = l as IList;
			if (list == null)
				return null;
			if (list.GetType().FullName == "System.Collections.ArrayList+ReadOnlyArrayList"){
				FieldInfo fi = list.GetType().GetField("_list", BindingFlags.NonPublic | BindingFlags.Instance);
				if (fi != null){
					list = (IList) fi.GetValue(list);
				}
			}
			return list as SharpArrayList;
		}
#endif



		public string GetListName(PropertyDescriptor[] listAccessors){
			return elementType.Name;
		}

		#endregion
		
		protected void Reset(){
			this.currentPosition = 0;
			this.OnListChange (resetEvent);
		}
		
		private void OnListChange (ListChangedEventArgs handler) {
			if (this.ListChanged != null) {
				this.ListChanged (this,handler);
			}
		}
		
		
		#region System.ComponentModel.IBindingList interface implementation
		public bool AllowNew {
			get {
				return this.allowNew;
			}
		}
		
		public bool AllowEdit {
			get {
				return this.allowEdit;
			}
		}
		
		public bool AllowRemove {
			get {
				return this.allowRemove;
			}
		}
		
		public bool SupportsChangeNotification {
			get {
				return true;
			}
		}
		
		public bool SupportsSearching {
			get {
				return this.supportsSearching;
			}
		}
		
		public bool SupportsSorting {
			get {
				return this.supportsSorting;
			}
		}
		
		public bool IsSorted {
			get {
				return this.isSorted;
			}
		}
		
		public System.ComponentModel.PropertyDescriptor SortProperty {
			get {
				return null;
			}
		}
		
		public System.ComponentModel.ListSortDirection SortDirection {
			get {
				return ListSortDirection.Ascending;
			}
		}
		
		public void RemoveSort() {
			throw new NotImplementedException("RemoveSort");
		}
		//TODO Test fehlt
		public void RemoveIndex(System.ComponentModel.PropertyDescriptor property) {
			throw new NotImplementedException("RemoveIndex");
		}
		
		//TODO Test fehlt
		public int Find(System.ComponentModel.PropertyDescriptor property, object key) {
//			return 0;
			throw new NotImplementedException("Find");
		}
		//TODO Test fehlt
		public void ApplySort(System.ComponentModel.PropertyDescriptor property, System.ComponentModel.ListSortDirection direction) {
			throw new NotImplementedException("ApplySort");
		}
		//TODO Test fehlt
		public void AddIndex(System.ComponentModel.PropertyDescriptor property) {
			throw new NotImplementedException("AddIndex");
		}
		
		public object AddNew() {
			throw new NotImplementedException("AddNew");
		}
		
		
		#endregion
		
		#region overrides
		public override int Add(object val) {
			if (this.elementType.GetType().IsAssignableFrom (val.GetType())) {
				System.Console.WriteLine("type ok");
			}
			if ((val.GetType().IsSubclassOf(this.elementType))||( val.GetType() == this.elementType)){
				if (this.allowNew) {
					int i = base.Add(val);
					this.OnListChange (new ListChangedEventArgs(ListChangedType.ItemAdded,i));
					return i;
				} else {
					throw new NotSupportedException("SharpArrayList:Add(object)");
				}
			} else {
				string str = String.Format("Add:Wrong Type {0} {1}",this.elementType,val.GetType());
				throw new ArgumentException(str);
			}
		}
		
		public override void AddRange(System.Collections.ICollection c) {
			foreach (object o in c) {
				this.Add (o);
			}
		}
		
		
		public override void RemoveAt(int index) {
			if (this.allowRemove) {
				if (index > -1) {
					base.RemoveAt(index);
					this.OnListChange (new ListChangedEventArgs(ListChangedType.ItemDeleted,index));
				}
			} else {
				throw new NotSupportedException("SharpArrayList:RemoveAt (index)");
			}
		}
		
		
		#endregion
		
		
		#region SharpReport.Data.IExtendedList interface implementation
		public string Name {
			get {
				return this.name;
			}
			
		}
		
		public int CurrentPosition {
			get {
				return currentPosition;
			}
			set {
				currentPosition = value;
			}
		}
		
		
		public IList IndexList {
			get {
				return(IList)this;
			}
		}
		
		public void BuildHashList(IList list) {
			throw new NotImplementedException("SharpArrayList:BuildHashList");
/*
			this.Clear();
			for (int i = 0;i < list.Count ;i++ ) {
//					this.Add (new PlainIndexItem(i,"satz " + i.ToString()));
			}
			this.OnListChange (new ListChangedEventArgs(ListChangedType.Reset,-1,-1));
	*/	
		}
		#endregion
		
		
	}
}
