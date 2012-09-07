// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeTypeBaseTypes : CodeElementsList
	{
		IProjectContent projectContent;
		IClass c;
		
		public CodeTypeBaseTypes(IProjectContent projectContent, IClass c)
		{
			this.projectContent = projectContent;
			this.c = c;
			AddBaseTypes();
		}
		
		void AddBaseTypes()
		{
			if (c.BaseType != null) {
				AddCodeElement(CodeClass2.CreateFromBaseType(projectContent, c.BaseType));
			}
		}
	}
}
