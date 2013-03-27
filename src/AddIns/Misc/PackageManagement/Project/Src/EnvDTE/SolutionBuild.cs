// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class SolutionBuild : MarshalByRefObject, global::EnvDTE.SolutionBuild
	{
		Solution solution;
		IProjectBuilder projectBuilder;
		int lastBuildInfo;
		
		public SolutionBuild()
		{
		}
		
		public SolutionBuild(Solution solution, IProjectBuilder projectBuilder)
		{
			this.solution = solution;
			this.projectBuilder = projectBuilder;
		}
		
		public global::EnvDTE.SolutionConfiguration ActiveConfiguration {
			get { return solution.GetActiveConfiguration(); }
		}
		
		/// <summary>
		/// Returns the number of projects that failed to build.
		/// </summary>
		public int LastBuildInfo {
			get { return lastBuildInfo; }
		}
		
		public void BuildProject(string solutionConfiguration, string projectUniqueName, bool waitForBuildToFinish)
		{
			Project project = solution.Projects.Item(projectUniqueName) as Project;
			projectBuilder.Build(project.MSBuildProject);
			UpdateLastBuildInfo(projectBuilder.BuildResults);
		}
		
		void UpdateLastBuildInfo(BuildResults buildResults)
		{
			if (buildResults.ErrorCount > 0) {
				lastBuildInfo = 1;
			} else {
				lastBuildInfo = 0;
			}
		}
		
		public object StartupProjects {
			get { return GetStartupProjects(); }
		}
		
		object[] GetStartupProjects()
		{
			var project = solution.GetStartupProject();
			if (project != null) {
				return new string[] { project.UniqueName };
			}
			return new string[0];
		}
	}
}
