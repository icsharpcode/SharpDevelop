// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public class OpenMSBuildProjects
	{
		IPackageManagementProjectService projectService;
		
		public OpenMSBuildProjects(IPackageManagementProjectService projectService)
		{
			this.projectService = projectService;
		}
		
		public MSBuildBasedProject FindProject(string name)
		{
			foreach (IProject project in projectService.GetOpenProjects()) {
				if (IsProjectNameMatch(project, name)) {
					return project as MSBuildBasedProject;
				}
			}
			return null;
		}
		
		bool IsProjectNameMatch(IProject project, string name)
		{
			return IsMatchIgnoringCase(project.Name, name) ||
				(project.FileName == name);
		}
		
		bool IsMatchIgnoringCase(string a, string b)
		{
			return String.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
	}
}
