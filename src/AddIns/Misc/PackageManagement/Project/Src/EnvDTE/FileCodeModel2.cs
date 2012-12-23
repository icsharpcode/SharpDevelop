// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class FileCodeModel2 : MarshalByRefObject, global::EnvDTE.FileCodeModel2
	{
		Project project;
		FileProjectItem projectItem;
		IDocumentNamespaceCreator namespaceCreator;
		
		public FileCodeModel2(Project project, FileProjectItem projectItem)
			: this(project, projectItem, new DocumentNamespaceCreator())
		{
		}
		
		public FileCodeModel2(
			Project project,
			FileProjectItem projectItem,
			IDocumentNamespaceCreator namespaceCreator)
		{
			this.project = project;
			this.projectItem = projectItem;
			this.namespaceCreator = namespaceCreator;
		}
		
		public global::EnvDTE.CodeElements CodeElements {
			get { return new FileCodeModelCodeElements(GetCompilationUnit()); }
		}
		
		public void AddImport(string name, object position = null, string alias = null)
		{
			namespaceCreator.AddNamespace(GetCompilationUnit(), name);
		}
		
		ICompilationUnit GetCompilationUnit()
		{
			return project.GetCompilationUnit(projectItem.FileName);
		}
	}
}
