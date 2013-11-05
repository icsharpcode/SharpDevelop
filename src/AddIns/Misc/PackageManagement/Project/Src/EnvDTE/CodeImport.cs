// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeImport : CodeElement, global::EnvDTE.CodeImport
	{
		string namespaceName;
		
		public CodeImport()
		{
		}
		
		public CodeImport(string namespaceName)
		{
			this.namespaceName = namespaceName;
		}
		
		public string Namespace {
			get {
				return namespaceName;
			}
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementImportStmt; }
		}
	}
}
