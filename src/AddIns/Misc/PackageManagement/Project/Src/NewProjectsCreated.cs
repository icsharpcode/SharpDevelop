// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public class NewProjectsCreated
	{
		ProjectCreateInformation createInfo;
		OpenMSBuildProjects openProjects;
		
		public NewProjectsCreated(
			ProjectCreateInformation createInfo,
			IPackageManagementProjectService projectService)
			: this(createInfo, new OpenMSBuildProjects(projectService))
		{
		}
		
		public NewProjectsCreated(
			ProjectCreateInformation createInfo,
			OpenMSBuildProjects openProjects)
		{
			this.createInfo = createInfo;
			this.openProjects = openProjects;
		}
		
		public IEnumerable<MSBuildBasedProject> GetProjects()
		{
			foreach (IProject project in createInfo.CreatedProjects) {
				MSBuildBasedProject openProject = FindProject(project.Name);
				if (openProject != null) {
					yield return openProject;
				}
			}
		}
		
		MSBuildBasedProject FindProject(string name)
		{
			return openProjects.FindProject(name);
		}
	}
}
