// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class FileCodeModelCodeNamespaceMembers : CodeElementsList
	{
		public FileCodeModelCodeNamespaceMembers()
		{
		}
		
		public void AddClass(IProjectContent projectContent, IClass c)
		{
			var codeClass = new CodeClass2(projectContent, c);
			AddCodeElement(codeClass);
		}
	}
}
