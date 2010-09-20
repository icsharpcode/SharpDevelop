// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Common
{
	/// <summary>
	/// Provides sort ordering on ObjectProperties.
	/// </summary>
	class ObjectPropertyComparer : IComparer<ObjectProperty>
	{
		private static readonly ObjectPropertyComparer instance = new ObjectPropertyComparer();
		public static ObjectPropertyComparer Instance
		{
			get { return instance; }
		}
		
		public int Compare(ObjectProperty prop1, ObjectProperty prop2)
		{
			return prop1.Name.CompareTo(prop2.Name);
			
			// order by IsAtomic, Name - 
			// we now don't know whether a property is atomic until rendering it, 
			// so IsAtomic is always true when sorting in ObjectGraphBuilder
			/*int comparedAtomic = prop2.IsAtomic.CompareTo(prop1.IsAtomic);
			if (comparedAtomic != 0)
			{
				return comparedAtomic;
			}
			else
			{
				return prop1.Name.CompareTo(prop2.Name);
			}*/
		}
	}
}
