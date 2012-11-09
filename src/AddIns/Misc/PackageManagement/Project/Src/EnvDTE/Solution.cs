// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Solution : MarshalByRefObject, global::EnvDTE.Solution
	{
		IPackageManagementProjectService projectService;
		SD.Solution solution;
		
		public Solution(IPackageManagementProjectService projectService)
		{
			this.projectService = projectService;
			this.solution = projectService.OpenSolution;
			this.Projects = new Projects(projectService);
			this.Globals = new SolutionGlobals(this);
			this.SolutionBuild = new SolutionBuild(this, projectService.ProjectBuilder);
			CreateProperties();
		}
		
		void CreateProperties()
		{
			var propertyFactory = new SolutionPropertyFactory(this);
			Properties = new Properties(propertyFactory);
		}
		
		public string FullName {
			get { return FileName; }
		}
		
		public string FileName {
			get { return solution.FileName; }
		}
		
		public bool IsOpen {
			get { return projectService.OpenSolution == solution; }
		}
		
		public global::EnvDTE.Projects Projects { get; private set; }
		public global::EnvDTE.Globals Globals { get; private set; }
		
		internal IList<SD.ProjectSection> Sections {
			get { return solution.Sections; }
		}
		
		internal void Save()
		{
			projectService.Save(solution);
		}
		
		public global::EnvDTE.ProjectItem FindProjectItem(string fileName)
		{
			foreach (Project project in Projects) {
				ProjectItem item = project.FindProjectItem(fileName);
				if (item != null) {
					return item;
				}
			}
			return null;
		}
		
		public global::EnvDTE.SolutionBuild SolutionBuild { get; private set; }
		public global::EnvDTE.Properties Properties { get; private set; }
		
		internal Project GetStartupProject()
		{
			MSBuildBasedProject project = solution.Preferences.StartupProject as MSBuildBasedProject;
			if (project != null) {
				return new Project(project);
			}
			return null;
		}
		
		internal IEnumerable<string> GetAllPropertyNames()
		{
			return new string[] {
				"Path",
				"StartupProject"
			};
		}
		
		internal SolutionConfiguration GetActiveConfiguration()
		{
			return new SolutionConfiguration(solution.Preferences);
		}
	}
}
