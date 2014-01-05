// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	/// <summary>
	/// File code model namespaces take the full name of the namespace that a class
	/// is inside. So for the FileCodeModelNamespace class the CodeNamespace.Name
	/// would be ICSharpCode.PackageManagement.EnvDTE.
	/// This differs from the CodeModel CodeNamespace which breaks up the namespaces into
	/// parts.
	/// </summary>
	public class FileCodeModelCodeNamespace : CodeElement, global::EnvDTE.CodeNamespace
	{
		NamespaceName namespaceName;
		CodeElementsList<CodeElement> members = new CodeElementsList<CodeElement>();
		
		public FileCodeModelCodeNamespace(CodeModelContext context, string namespaceName)
			: this(context, new NamespaceName(namespaceName))
		{
		}
		
		FileCodeModelCodeNamespace(CodeModelContext context, NamespaceName namespaceName)
		{
			this.context = context;
			this.namespaceName = namespaceName;
			this.InfoLocation = global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal;
			this.Language = context.CurrentProject.GetCodeModelLanguage();
		}
		
		public override global::EnvDTE.vsCMInfoLocation InfoLocation {
			get { return global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject; }
		}
		
		public global::EnvDTE.CodeElements Members {
			get { return members; }
		}
		
		internal void AddMember(CodeElement member)
		{
			members.Add(member);
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
			get { return FullName; }
		}
	}
}
