// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ICSharpCode.NRefactory.TypeSystem;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Compares members - properties come before fields.
	/// </summary>
	public sealed class PropertiesFirstComparer : IComparer<IMember>
	{
		private static PropertiesFirstComparer instance = new PropertiesFirstComparer();
		
		public static PropertiesFirstComparer Instance {
			get { return instance; }
		}
		
		private PropertiesFirstComparer() {}
		
		public int Compare(IMember x, IMember y)
		{
			if ((x is IProperty) && (y is IField)) return -1;
			if ((x is IField) && (y is IProperty)) return 1;
			return x.Name.CompareTo(y.Name);
		}
	}
}
