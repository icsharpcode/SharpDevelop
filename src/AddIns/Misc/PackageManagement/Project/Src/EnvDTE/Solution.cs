// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Solution : MarshalByRefObject, global::EnvDTE.Solution
	{
		IPackageManagementProjectService projectService;
		SD.ISolution solution;
		
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
		
		internal ICollection<SD.SolutionSection> Sections {
			get { return solution.GlobalSections; }
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
			var project = solution.StartupProject as MSBuildBasedProject;
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
			return new SolutionConfiguration(solution);
		}
	}
}
