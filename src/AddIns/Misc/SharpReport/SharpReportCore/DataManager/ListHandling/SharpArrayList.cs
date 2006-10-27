using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace SharpReportCore
{
	/// <summary>
	/// This class act's as a IndexList to
	/// <see cref="SharpBaseList"></see>
	/// </summary>
	public class SharpIndexCollection :List<BaseComparer> ,IEnumerable<BaseComparer>{
		string name;
		int currentPosition;
		
		public SharpIndexCollection():this ("IndexList"){
		}
		
		public SharpIndexCollection(string name){
			this.name = name;
		}
		#region IEnumerable implementation
		
		IEnumerator<BaseComparer> IEnumerable<BaseComparer>.GetEnumerator()
		{
			System.Console.WriteLine("IndexList IEnumerable<BaseComparer>.GetEnumerator()" );
			//return new MyEnumerator(this);
			
			for (int i =0;i < this.Count;i++){
				yield return this[i];
			}
			
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			System.Console.WriteLine("IndexList IEnumerable.GetEnumerator()" );
			return ((IEnumerable<BaseComparer>)this).GetEnumerator();
		}
		
		#endregion
		
		
		#region properties
		
		public int CurrentPosition {
			get {
				return currentPosition;
			}
			set {
				currentPosition = value;
			}
		}
		
		public string Name {
			get {
				return name;
			}
		}
		#endregion
	/*
		class MyEnumerator : IEnumerator<BaseComparer>{
			SharpIndexCollection index;
			int current;
			
			public MyEnumerator(SharpIndexCollection index)
			{
				System.Console.WriteLine("Constr:MyEnumerator");
				this.index = index;
				this.current = -1;
			}
			
			public BaseComparer Current {
				get {
					System.Console.WriteLine("\tBaseComparer Current");
					if (this.current == -1) {
						throw new InvalidOperationException("SharpArrayList:Current");
					}
					return this.index[this.current];
				}
			}
			
			object IEnumerator.Current {
				get {
					System.Console.WriteLine("IEnumerator.Current");
					IEnumerator<BaseComparer> enumerator = this;
					return enumerator.Current;
				}
			}
			
			public void Dispose()
			{
				throw new NotImplementedException();
			}
			
			public bool MoveNext()
			{
				System.Console.WriteLine("IndexList:MoveNext");
				this.current++;
				return(this.current < this.index.Count);
			}
			
			public void Reset()
			{
				this.current = -1;
			}
			
		}
		*/
	}
	
	/// <summary>
	/// This class act's as a store of the original Data
	/// </summary>
	
	public class SharpDataCollection<T> : IList<T>,ITypedList{
		Collection<T> list = new Collection<T>();
		Type elementType;
		
		public SharpDataCollection(Type elementType)
		{
			this.elementType = elementType;
		}

		public T this[int index] {
			get {
				return list[index];
			}
			set {
				T oldValue = list[index];
				if (!object.Equals(oldValue, value)) {
					list[index] = value;
				}
			}
		}
		
		public int Count {
			[DebuggerStepThrough]
			get {
				return list.Count;
			}
		}
		
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public int IndexOf(T item)
		{
			return list.IndexOf(item);
		}
		
		public void Insert(int index, T item)
		{
			list.Insert(index, item);
		}
		
		public void RemoveAt(int index)
		{
//			T item = list[index];
			list.RemoveAt(index);
		}
		
		public void Add(T item)
		{
			list.Add(item);
		}
		
		
		public void AddRange(IList range)
		{
			foreach(T t in range) {
				Add(t);
			}
		}
		
		
		public void Clear(){
			list = new Collection<T>();
		}
		
		public bool Contains(T item)
		{
			return list.Contains(item);
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}
		
		public bool Remove(T item)
		{
			if (list.Remove(item)) {
				return true;
			}
			return false;
		}
		#region ITypedList Member

		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors){
			if (listAccessors != null && listAccessors.Length > 0){
				Type t = this.elementType;
				
				for(int i = 0; i < listAccessors.Length; i++){
					PropertyDescriptor pd = listAccessors[i];
					t = (Type) PropertyTypeHash.Instance[t, pd.Name];
				}
				// if t is null an empty list will be generated
				return SharpTypeDescriptor.GetProperties(t);
			}
			return SharpTypeDescriptor.GetProperties(elementType);
		}
		public string GetListName(PropertyDescriptor[] listAccessors){
			return elementType.Name;
		}
		
		public static Type GetElementType(IList list, Type parentType, string propertyName)
		{
			SharpDataCollection<T> al = null;
			object element = null;
			al = CheckForArrayList(list);
			if (al == null)
			{
				if (list.Count > 0)
				{
					element = list[0];
				}
			}
			if (al == null && element == null)
			{
				PropertyInfo pi = parentType.GetProperty(propertyName);
				if (pi != null)
				{
					object parentObject = null;
					try
					{
						parentObject = Activator.CreateInstance(parentType);
					}
					catch(Exception) {}
					if (parentObject != null)
					{
						list = pi.GetValue(parentObject, null) as IList;
						al = CheckForArrayList(list);
					}
				}
			}
			if (al != null)
			{
				return al.elementType;
			}
			else if (element != null)
			{
				return element.GetType();
			}
			return null;
		}
		
		private static SharpDataCollection<T> CheckForArrayList(object l)
		{
			IList list = l as IList;
			if (list == null)
				return null;
			if (list.GetType().FullName == "System.Collections.ArrayList+ReadOnlyArrayList")
			{
				FieldInfo fi = list.GetType().GetField("_list", BindingFlags.NonPublic | BindingFlags.Instance);
				if (fi != null)
				{
					list = (IList) fi.GetValue(list);
				}
			}
			return list as SharpDataCollection<T>;
		}
		#endregion
		
		[DebuggerStepThrough]
		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}
		
		[DebuggerStepThrough]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
	}
}
