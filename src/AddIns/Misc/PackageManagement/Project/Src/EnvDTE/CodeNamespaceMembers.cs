// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeNamespaceMembers : CodeElements
	{
		List<CodeElement> elements = new List<CodeElement>();
		IProjectContent projectContent;
		CodeNamespace codeNamespace;
		
		public CodeNamespaceMembers(IProjectContent projectContent, CodeNamespace codeNamespace)
		{
			this.projectContent = projectContent;
			this.codeNamespace = codeNamespace;
			GetMembers();
		}
		
		void GetMembers()
		{
			AddNamespaceMembers();
			AddTypesInNamespace();
		}
		
		void AddNamespaceMembers()
		{
			elements.AddRange(new ChildCodeNamespaces(projectContent, codeNamespace));
		}
		
		void AddTypesInNamespace()
		{
			elements.AddRange(new CodeTypesInNamespace(projectContent, codeNamespace));
		}
		
		public int Count {
			get { return elements.Count; }
		}
		
		public IEnumerator GetEnumerator()
		{
			return elements.GetEnumerator();
		}
	}
}
