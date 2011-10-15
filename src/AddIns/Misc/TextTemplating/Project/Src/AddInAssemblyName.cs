// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.TextTemplating
{
	public class AddInAssemblyName
	{
		string assemblyShortName;
		
		public AddInAssemblyName(string assemblyFullName)
		{
			GetAssemblyShortName(assemblyFullName);
		}
		
		void GetAssemblyShortName(string assemblyFullName)
		{
			if (assemblyFullName != null) {
				var domAssemblyName = new DomAssemblyName(assemblyFullName);
				assemblyShortName = "ICSharpCode." + domAssemblyName.ShortName;
			}
		}
		
		public bool Matches(IAddIn addIn)
		{
			return CompareIgnoringCase(assemblyShortName, addIn.PrimaryIdentity);
		}
		
		bool CompareIgnoringCase(string a, string b)
		{
			return String.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
	}
}
