// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class FileCodeModelCodeElements : CodeElementsList
	{
		ICompilationUnit compilationUnit;
		
		public FileCodeModelCodeElements(ICompilationUnit compilationUnit)
		{
			this.compilationUnit = compilationUnit;
			GetCodeElements();
		}
		
		void GetCodeElements()
		{
			foreach (IUsing namespaceUsing in GetNamespaceImports()) {
				AddNamespaceImport(namespaceUsing);
			}
		}
		
		IList<IUsing> GetNamespaceImports()
		{
			return compilationUnit
				.UsingScope
				.Usings;
		}
		
		void AddNamespaceImport(IUsing import)
		{
			AddCodeElement(new CodeImport(import));
		}
	}
}
