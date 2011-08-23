// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Compares members - .NET properties come before .NET fields.
	/// </summary>
	public sealed class PropertiesFirstComparer : IComparer<MemberInfo>
	{
		private static PropertiesFirstComparer instance = new PropertiesFirstComparer();
		
		public static PropertiesFirstComparer Instance {
			get {
				return instance;
			}
		}
		
		private PropertiesFirstComparer()
		{
		}
		
		public int Compare(MemberInfo x, MemberInfo y)
		{
			if ((x is PropertyInfo) && (y is FieldInfo)) return -1;
			if ((y is PropertyInfo) && (x is FieldInfo)) return 1;
			return x.Name.CompareTo(y.Name);
		}
	}
}
