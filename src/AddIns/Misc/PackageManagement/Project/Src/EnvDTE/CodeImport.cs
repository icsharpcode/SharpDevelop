// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeImport : CodeElement, global::EnvDTE.CodeImport
	{
		IUsing import;
		
		public CodeImport()
		{
		}
		
		public CodeImport(IUsing import)
		{
			this.import = import;
			this.Namespace = GetNamespace();
		}
		
		string GetNamespace()
		{
			if (import.Usings.Any()) {
				return import.Usings.First();
			} else if (import.HasAliases) {
				return import.Aliases.Values.First().FullyQualifiedName;
			}
			return String.Empty;
		}
		
		public string Namespace { get; private set; }
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementImportStmt; }
		}
	}
}
