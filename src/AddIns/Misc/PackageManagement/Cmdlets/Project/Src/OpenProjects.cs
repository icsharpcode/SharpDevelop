// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Management.Automation;

using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	public class OpenProjects
	{
		IPackageManagementSolution solution;
		
		public OpenProjects(IPackageManagementSolution solution)
		{
			this.solution = solution;
		}
		
		public IEnumerable<Project> GetAllProjects()
		{
			foreach (IProject project in solution.GetMSBuildProjects()) {
				yield return CreateProject(project);
			}
		}
		
		Project CreateProject(IProject project)
		{
			return new Project(project as MSBuildBasedProject);
		}
		
		public IEnumerable<Project> GetFilteredProjects(string[] projectNames)
		{
			foreach (string projectName in projectNames) {
				WildcardPattern wildcard = CreateWildcard(projectName);
				foreach (Project project in GetAllProjects()) {
					if (wildcard.IsMatch(project.Name)) {
						yield return project;
					}
				}
			}
		}
		
		WildcardPattern CreateWildcard(string pattern)
		{
			return new WildcardPattern(pattern, WildcardOptions.IgnoreCase);
		}
	}
}
