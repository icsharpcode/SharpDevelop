/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 20.05.2013
 * Time: 18:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;

namespace ICSharpCode.Reporting.DataSource
{
	/// <summary>
	/// Description of PropertyTypeHash.
	/// </summary>
	internal class PropertyTypeHash
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
