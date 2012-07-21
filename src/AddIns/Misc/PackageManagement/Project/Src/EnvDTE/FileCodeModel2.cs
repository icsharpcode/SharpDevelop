// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class FileCodeModel2
	{
		Project project;
		FileProjectItem projectItem;
		
		public FileCodeModel2(Project project, FileProjectItem projectItem)
		{
			this.project = project;
			this.projectItem = projectItem;
		}
		
		public CodeElements CodeElements {
			get { return new FileCodeModelCodeElements(project, projectItem); }
		}
		
		public void AddImport(string name, object position = null, string alias = null)
		{
			
		}
	}
}
