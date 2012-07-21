// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class FileCodeModelCodeElements : CodeElementsList
	{
		Project project;
		FileProjectItem projectItem;
		
		public FileCodeModelCodeElements(Project project, FileProjectItem projectItem)
		{
			this.project = project;
			this.projectItem = projectItem;
			GetCodeElements();
		}
		
		void GetCodeElements()
		{
			foreach (IUsing namespaceUsing in GetNamespaceImports()) {
				AddNamespaceImport(namespaceUsing);
			}
		}
		
		IList<IUsing> GetNamespaceImports()
		{
			return project
				.GetCompilationUnit(projectItem.FileName)
				.UsingScope
				.Usings;
		}
		
		void AddNamespaceImport(IUsing import)
		{
			AddCodeElement(new CodeImport(import));
		}
	}
}
