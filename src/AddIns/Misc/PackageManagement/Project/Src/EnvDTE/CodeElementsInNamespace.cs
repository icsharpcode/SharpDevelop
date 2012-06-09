// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeElementsInNamespace : CodeElementsList
	{
		IProjectContent projectContent;
		NamespaceName namespaceName;
		
		public CodeElementsInNamespace(IProjectContent projectContent, string qualifiedNamespaceName)
			: this(projectContent, new NamespaceName(qualifiedNamespaceName))
		{
		}
		
		public CodeElementsInNamespace(IProjectContent projectContent, NamespaceName namespaceName)
		{
			this.projectContent = projectContent;
			this.namespaceName = namespaceName;
			GetCodeElements();
		}
		
		void GetCodeElements()
		{
			foreach (ICompletionEntry entry in projectContent.GetNamespaceContents(namespaceName.QualifiedName)) {
				AddCodeElement(entry);
			}
		}
		
		void AddCodeElement(ICompletionEntry entry)
		{
			var namespaceEntry = entry as NamespaceEntry;
			var classEntry = entry as IClass;
			if (namespaceEntry != null) {
				AddCodeNamespace(namespaceEntry);
			} else if (classEntry != null) {
				AddCodeClass(classEntry);
			}
		}
		
		void AddCodeNamespace(NamespaceEntry namespaceEntry)
		{
			if (!String.IsNullOrEmpty(namespaceEntry.Name)) {
				NamespaceName childNamespaceName = namespaceName.CreateChildNamespaceName(namespaceEntry.Name);
				AddCodeElement(new CodeNamespace(projectContent, childNamespaceName));
			}
		}
		
		void AddCodeClass(IClass c)
		{
			if (c.ClassType == ClassType.Interface) {
				AddCodeElement(new CodeInterface(projectContent, c));
			} else {
				AddCodeElement(new CodeClass2(projectContent, c));
			}
		}
	}
}
