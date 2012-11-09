// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeNamespace : CodeElement, global::EnvDTE.CodeNamespace
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
			this.InfoLocation = global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal;
			this.Language = projectContent.GetCodeModelLanguage();
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementNamespace; }
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
		
		public virtual global::EnvDTE.CodeElements Members {
			get { return new CodeElementsInNamespace(projectContent, namespaceName); }
		}
	}
}
