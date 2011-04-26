// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class References
	{
		MSBuildBasedProject project;
		IPackageManagementProjectService projectService;
		
		public References(MSBuildBasedProject project)
			: this(project, new PackageManagementProjectService())
		{
		}
		
		public References(
			MSBuildBasedProject project,
			IPackageManagementProjectService projectService)
		{
			this.project = project;
			this.projectService = projectService;
		}
		
		public void Add(string path)
		{
			var referenceItem = new ReferenceProjectItem(project, path);
			projectService.AddProjectItem(project, referenceItem);
			SaveProject();
		}
		
		void SaveProject()
		{
			projectService.Save(project);
		}
	}
}
