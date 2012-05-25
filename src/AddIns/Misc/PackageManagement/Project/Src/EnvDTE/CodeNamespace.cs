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
		CodeNamespaceMembers members;
		
		public CodeNamespace(IProjectContent projectContent, string qualifiedName)
			: base(null)
		{
			this.projectContent = projectContent;
			this.namespaceName = new NamespaceName(qualifiedName);
			this.InfoLocation = vsCMInfoLocation.vsCMInfoLocationExternal;
		}
		
		internal string QualifiedName {
			get { return namespaceName.QualifiedName; }
		}
		
		internal NamespaceName NamespaceName {
			get { return namespaceName; }
		}
		
		public string FullName {
			get { return this.Name; }
		}
		
		public override string Name {
			get { return namespaceName.LastPart; }
		}
		
		public CodeElements Members {
			get { 
				if (members == null) {
					members = new CodeNamespaceMembers(projectContent, this);
				}
				return members;
			}
		}
		
		internal string GetChildNamespaceName(string qualifiedName)
		{
			return namespaceName.GetChildNamespaceName(qualifiedName);
		}
	}
}
