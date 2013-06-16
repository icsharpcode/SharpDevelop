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
	public class FileCodeModelCodeNamespace : CodeNamespace
	{
		public FileCodeModelCodeNamespace(CodeModelContext context, string namespaceName)
			: base(context, namespaceName)
		{
		}
		
		public override string Name {
			get { return base.FullName; }
		}
		
		public override global::EnvDTE.vsCMInfoLocation InfoLocation {
			get { return global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject; }
		}
		
		CodeElementsList<CodeElement> members = new CodeElementsList<CodeElement>();
		
		public override global::EnvDTE.CodeElements Members {
			get { return members; }
		}
		
		internal void AddMember(CodeElement member)
		{
			members.Add(member);
		}
	}
}
