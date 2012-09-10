// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Projects : MarshalByRefObject, IEnumerable<Project>
	{
		IPackageManagementProjectService projectService;
		
		public Projects(IPackageManagementProjectService projectService)
		{
			this.projectService = projectService;
		}
		
		public IEnumerator<Project> GetEnumerator()
		{
			return GetProjectsInSolution().GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		IEnumerable<Project> GetProjectsInSolution()
		{
			foreach (SD.MSBuildBasedProject msbuildProject in GetOpenMSBuildProjects()) {
				yield return new Project(msbuildProject);
			}
		}
		
		IEnumerable<SD.IProject> GetOpenMSBuildProjects()
		{
			return projectService.GetOpenProjects();
		}
		
		/// <summary>
		/// Index of 1 returns the first project.
		/// </summary>
		public Project Item(int index)
		{
			return GetProjectsInSolution()
				.Skip(index - 1)
				.First();
		}
		
		public int Count {
			get { return GetProjectsInSolution().Count(); }
		}
	}
}
