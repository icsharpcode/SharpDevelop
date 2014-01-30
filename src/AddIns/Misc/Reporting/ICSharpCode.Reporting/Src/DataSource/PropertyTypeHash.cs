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

namespace ICSharpCode.Reporting.DataSource
{
	/// <summary>
	/// Description of PropertyTypeHash.
	/// </summary>
	class PropertyTypeHash
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

		PropertyTypeHash()
		{
		}
	}
}
