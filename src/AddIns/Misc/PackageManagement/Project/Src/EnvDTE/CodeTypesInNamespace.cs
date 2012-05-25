// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeTypesInNamespace : List<CodeType>
	{
		IProjectContent projectContent;
		CodeNamespace codeNamespace;
		
		public CodeTypesInNamespace(IProjectContent projectContent, string namespaceName)
			: this(projectContent, new CodeNamespace(projectContent, namespaceName))
		{
		}
		
		public CodeTypesInNamespace(IProjectContent projectContent, CodeNamespace codeNamespace)
		{
			this.projectContent = projectContent;
			this.codeNamespace = codeNamespace;
			AddTypes();
		}
		
		void AddTypes()
		{
			foreach (ICompletionEntry completionEntry in GetTypesInNamespace()) {
				AddMember(completionEntry);
			}
		}
		
		List<ICompletionEntry> GetTypesInNamespace()
		{
			return projectContent.GetNamespaceContents(codeNamespace.QualifiedName);
		}
		
		void AddMember(ICompletionEntry completionEntry)
		{
			IClass classMember = completionEntry as IClass;
			if (classMember != null) {
				AddClassMember(classMember);
			}
		}
		
		void AddClassMember(IClass classMember)
		{
			Add(new CodeClass2(classMember));
		}
	}
}
