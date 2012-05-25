// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement.EnvDTE;

namespace PackageManagement.Tests.Helpers
{
	public static class CodeElementsHelpers
	{
		public static CodeElement FirstOrDefault(this CodeElements codeElements)
		{
			return ToList(codeElements).FirstOrDefault();
		}
		
		public static List<CodeElement> ToList(this CodeElements codeElements)
		{
			var list = new List<CodeElement>();
			foreach (CodeElement codeElement in codeElements) {
				list.Add(codeElement);
			}
			return list;
		}
	}
}
