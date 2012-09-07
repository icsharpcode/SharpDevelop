// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Solution : MarshalByRefObject
	{
		IPackageManagementProjectService projectService;
		SD.Solution solution;
		
		public Solution(IPackageManagementProjectService projectService)
		{
			this.projectService = projectService;
			this.solution = projectService.OpenSolution;
			this.Projects = new Projects(projectService);
			this.Globals = new SolutionGlobals(this);
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
		
		public Projects Projects { get; private set; }
		public Globals Globals { get; private set; }
		
		internal IList<SD.ProjectSection> Sections {
			get { return solution.Sections; }
		}
		
		internal void Save()
		{
			projectService.Save(solution);
		}
		
		internal bool IsSameSolution(SD.Solution solution)
		{
			throw new NotImplementedException();
		}
	}
}
