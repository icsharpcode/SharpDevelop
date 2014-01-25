// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// This class act's as a IndexList to
	/// <see cref="SharpBaseList"></see>
	/// </summary>
	public class IndexList :List<BaseComparer> 
	{
		string name;
		int currentPosition;
		
		public IndexList():this ("IndexList")
		{
		}
		
		public IndexList(string name)
		{
			this.name = name;
		}
		
	
		
		#region properties
		
		public int CurrentPosition 
		{
			get {
				return currentPosition;
			}
			set {
				currentPosition = value;
			}
		}
		
		public string Name 
		{
			get {
				return name;
			}
		}
		#endregion
	
	}
	
	
	public class DataCollection<T> : IList<T>,ITypedList
	{
		Collection<T> list = new Collection<T>();
		Type elementType;
		
		public DataCollection(Type elementType)
		{
			this.elementType = elementType;
		}

		public T this[int index] 
		{
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
		
		public int Count 
		{
			[DebuggerStepThrough]
			get {
				return list.Count;
			}
		}
		
		public bool IsReadOnly 
		{
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
				return ExtendedTypeDescriptor.GetProperties(t);
			}
			return ExtendedTypeDescriptor.GetProperties(elementType);
		}
		
		
		public string GetListName(PropertyDescriptor[] listAccessors){
			return elementType.Name;
		}
		
		public static Type GetElementType(IList list, Type parentType, string propertyName)
		{
			DataCollection<T> al = null;
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
		
		private static DataCollection<T> CheckForArrayList(object l)
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
			return list as DataCollection<T>;
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
