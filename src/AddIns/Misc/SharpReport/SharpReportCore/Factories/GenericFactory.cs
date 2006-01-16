using System;
using System.Collections;
using System.Reflection;

namespace SharpReportCore {
	/// <summary>
	/// Base class for all assembly based factories in the system.
	/// </summary>
	public class GenericFactory{
		
		Hashtable typeHash = new Hashtable();

		public virtual string[] AvailableTypes{
			get { 
				string[] result = new string[typeHash.Count];
				int i = 0;
				foreach(DictionaryEntry de in typeHash)
					result[i++] = (string) de.Key;
				return result;	
			}
		}

		public bool Contains(string name){
			foreach(string s in AvailableTypes){
				if (s == name)
					return true;
			}
			return false;
		}

		protected virtual object Create(string name){
			if (!Contains(name)) {
				return null;
			} else {
				return Activator.CreateInstance((Type)typeHash[name]);
			}
		}

		protected virtual object Create(string name, object[] parameters){
			return Activator.CreateInstance((Type)typeHash[name], parameters);
		}

		public GenericFactory(Assembly ass, Type assignableType, string nameSpace){
			Type at = typeof(object);
			foreach (Type t in ass.GetTypes())
			{
				if (t.IsAbstract)
					continue;
				if (((nameSpace != null && t.FullName.StartsWith(nameSpace)) 
					|| nameSpace == null) 
					&& assignableType.IsAssignableFrom(t))
				{
					object[] attrs = t.GetCustomAttributes(typeof(DisplayNameAttribute), false);
					if (attrs.Length > 0)
					{
						DisplayNameAttribute dna = (DisplayNameAttribute) attrs[0];
						typeHash.Add(dna.Name, t);
					}
					else
						typeHash.Add(t.Name, t);
				}
			}
		}

		public Type this[string name]{
			get { return typeHash[name] as Type; }
		}

		public GenericFactory(Assembly ass, Type assignableType) : this(ass, assignableType, null){			
		}
	}
}
