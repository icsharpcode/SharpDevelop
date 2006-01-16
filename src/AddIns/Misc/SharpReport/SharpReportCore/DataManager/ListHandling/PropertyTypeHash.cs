using System;
using System.Collections;

namespace SharpReportCore{
	public class PropertyTypeHash{
		static PropertyTypeHash instance = new PropertyTypeHash();

		static public PropertyTypeHash Instance
		{
			get { return instance; }
		}

		Hashtable types = new Hashtable();

		private string MakeIndex(Type t, string name)
		{
			return t.FullName + '.' + name;
		}

		public object this[Type t, string fieldName]
		{
			get
			{
				return types[MakeIndex(t, fieldName)];
			}
			set
			{
				if (value == null)
					return;
				string key = MakeIndex(t, fieldName);
				if (!types.Contains(key))
					types.Add(key, value);				
			}
		}

		private PropertyTypeHash()
		{
		}
	
	}
}
