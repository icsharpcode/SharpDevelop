// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class FileCodeModelCodeElements : CodeElementsList
	{
		ICompilationUnit compilationUnit;
		List<FileCodeModelCodeNamespace> fileCodeModelNamespaces = new List<FileCodeModelCodeNamespace>();
		
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
			AddClasses();
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
		
		void AddClasses()
		{
			foreach (IClass c in compilationUnit.Classes) {
				FileCodeModelCodeNamespace codeNamespace = GetOrCreateFileCodeModelNamespace(c);
				codeNamespace.AddClass(compilationUnit.ProjectContent, c);
			}
		}
		
		FileCodeModelCodeNamespace GetOrCreateFileCodeModelNamespace(IClass c)
		{
			var codeNamespace = FindFileCodeModelNamespace(c);
			if (codeNamespace != null) {
				return codeNamespace;
			}
			return CreateFileCodeModelNamespace(c);
		}
		
		FileCodeModelCodeNamespace FindFileCodeModelNamespace(IClass c)
		{
			return fileCodeModelNamespaces.FirstOrDefault(ns => ns.FullName == c.Namespace);
		}
		
		FileCodeModelCodeNamespace CreateFileCodeModelNamespace(IClass c)
		{
			var codeNamespace = new FileCodeModelCodeNamespace(compilationUnit.ProjectContent, c.Namespace);
			AddCodeElement(codeNamespace);
			fileCodeModelNamespaces.Add(codeNamespace);
			return codeNamespace;
		}
	}
}
