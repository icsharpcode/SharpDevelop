// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

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
