// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers
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
		}
	}
}
