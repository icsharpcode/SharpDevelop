//
// -*- C# -*-
//
// Author:         Roman Taranchenko
// Copyright:      (c) 2004 Roman Taranchenko
// Copying Policy: GNU General Public License
//

using System;
using System.Collections;
using System.IO;

namespace CPPBinding
{

	public interface IDependence 
	{
  
		string Name 
		{
			get;
		}
  
		DependenceTree DependsOn 
		{
			get;
		}

	 	DependenceTree Children 
		{
			get;
		}
  
		void Add(IDependence dependence);
  
		DateTime GetLastWriteTime();
	
		bool Exists
		{
			get;
		}
	
	 	bool Contains(IDependence dep);
	
 		bool Contains(string dep);
	
	}

	public class SourceFile: IDependence 
	{
  
		private string _name;
		private DependenceTree _dependences = new DependenceTree();
  
		public SourceFile(string name) 
		{
			_name = name;
		}
  
		public string Name 
		{
			get 
			{
				return _name;
			}
		}
  
		public DependenceTree Children 
		{
			get 
			{
				return _dependences;
			}
		}
  
		public DependenceTree DependsOn 
		{
			get 
			{
				DependenceTree listOfNames = new DependenceTree();
				FillListOfNames(this, listOfNames);
				return listOfNames;
			}
		}
  
		public void Add(IDependence dependence) 
		{
			_dependences.Add(dependence);
		}
  
		private static void FillListOfNames(IDependence dependence, DependenceTree list) 
		{
			foreach (IDependence dep in dependence.Children) 
			{
				list.Add(dep);
				FillListOfNames(dep, list);
			}
		}
  
		public DateTime GetLastWriteTime()
		{
			DateTime result = DateTime.MinValue;
			if (Exists)
			{
				result = File.GetLastWriteTime(_name);
				foreach (IDependence dep in _dependences)
				{
					DateTime dt = dep.GetLastWriteTime();
					if (dt.CompareTo(result) > 0)
					{
						result = dt;
					}
				}
			}
			return result;
		}
	
		public bool Exists
		{
			get
			{
				return File.Exists(_name);
			}
		}
	
		public override string ToString()
		{
			return _name;
		}

		public bool Contains(IDependence dep)
		{
			return Contains(dep.Name);
		}
	
		public bool Contains(string dep)
		{
			if (_name.Equals(dep)) {
				return true;
			}
			return _dependences.Contains(dep);
		}
	}

	public class DependenceTree: IEnumerable 
	{

		private SortedList _list;

		public DependenceTree() 
		{
			_list = new SortedList();
		}
  
		public void Add(IDependence value) 
		{
			if (!Contains(value)) 
			{
				_list.Add(value.Name, value);
			}
		}
  
		public IDependence Get(string name) 
		{
			return (IDependence) _list.GetByIndex(_list.IndexOfKey(name));
		}
  
		public IDependence Get(IDependence dep) 
		{
			return Get(dep.Name);
		}
  
		public bool Contains(IDependence dep) 
		{
			return Contains(dep.Name);
		}
  
		public bool Contains(string name) 
		{
			return _list.ContainsKey(name);
		}
  
		public IEnumerator GetEnumerator() 
		{
			return new DependenceTreeEnumerator(_list.GetEnumerator());
		}
  
	}

	class DependenceTreeEnumerator: IEnumerator 
	{
		private IDictionaryEnumerator _enum;
  
		public DependenceTreeEnumerator(IDictionaryEnumerator e) 
		{
			_enum = e;
		}
  
		public object Current 
		{
			get 
			{
				return ((DictionaryEntry)_enum.Current).Value;
			}
		}
  
		public bool MoveNext() 
		{
			return _enum.MoveNext();
		}
  
		public void Reset() 
		{
			_enum.Reset();
		}
	}

}
