// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeNamespace : CodeElement
	{
		NamespaceName namespaceName;
		IProjectContent projectContent;
		
		public CodeNamespace(IProjectContent projectContent, string qualifiedName)
			: this(projectContent, new NamespaceName(qualifiedName))
		{
		}
		
		public CodeNamespace(IProjectContent projectContent, NamespaceName namespaceName)
		{
			this.projectContent = projectContent;
			this.namespaceName = namespaceName;
			this.InfoLocation = vsCMInfoLocation.vsCMInfoLocationExternal;
			this.Language = GetLanguage(projectContent);
		}
		
		public override vsCMElement Kind {
			get { return vsCMElement.vsCMElementNamespace; }
		}
		
		internal NamespaceName NamespaceName {
			get { return namespaceName; }
		}
		
		public string FullName {
			get { return namespaceName.QualifiedName; }
		}
		
		public override string Name {
			get { return namespaceName.LastPart; }
		}
		
		public CodeElements Members {
			get { return new CodeElementsInNamespace(projectContent, namespaceName); }
		}
	}
}
