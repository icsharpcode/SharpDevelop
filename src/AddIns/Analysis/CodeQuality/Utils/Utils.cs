// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Mono.Cecil;

namespace ICSharpCode.CodeQuality
{
	/// <summary>
	/// Description of Utils.
	/// </summary>
	public class Utils
	{
		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);
	}
	
	public class AssemblyNameReferenceComparer : IEqualityComparer<AssemblyNameReference>
	{
		public bool Equals(AssemblyNameReference x, AssemblyNameReference y)
		{
			if (x == y) return true;
			if (x != null && y != null)
				return x.FullName == y.FullName;
			return false;
		}
		
		public int GetHashCode(AssemblyNameReference obj)
		{
			if (obj == null) return 0;
			return obj.FullName.GetHashCode();
		}
	}
}
