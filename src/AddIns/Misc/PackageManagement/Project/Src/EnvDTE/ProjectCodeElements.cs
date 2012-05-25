// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectCodeElements : CodeElements
	{
		List<CodeElement> codeElements = new List<CodeElement>();
		IProjectContent projectContent;
		
		public ProjectCodeElements(IProjectContent projectContent)
		{
			this.projectContent = projectContent;
			AddCodeElements();
		}
		
		void AddCodeElements()
		{
			AddNamespaceCodeElements();
			AddClassesWithNoNamespace();
		}
		
		void AddNamespaceCodeElements()
		{
			codeElements.AddRange(CreateChildNodeNamespaces());
		}
		
		ChildCodeNamespaces CreateChildNodeNamespaces()
		{
			return new ChildCodeNamespaces(projectContent, new CodeNamespace(projectContent, String.Empty));
		}
		
		void AddClassesWithNoNamespace()
		{
			codeElements.AddRange(new CodeTypesInNamespace(projectContent, String.Empty));
		}
		
		public int Count {
			get { return codeElements.Count; }
		}
		
		public IEnumerator GetEnumerator()
		{
			return codeElements.GetEnumerator();
		}
	}
}
