// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;

namespace ICSharpCode.Reports.Core{
	
	public class PropertyTypeHash
	{
		static PropertyTypeHash instance = new PropertyTypeHash();

		static public PropertyTypeHash Instance
		{
			get { return instance; }
		}

		Hashtable types = new Hashtable();

		private static string MakeIndex(Type t, string name)
		{
			return t.FullName + '.' + name;
		}

		public object this[Type type, string fieldName]
		{
			get
			{
				return types[MakeIndex(type, fieldName)];
			}
			set
			{
				if (value == null)
					return;
				string key = MakeIndex(type, fieldName);
				if (!types.Contains(key))
					types.Add(key, value);				
			}
		}

		private PropertyTypeHash()
		{
		}
	
	}
}
