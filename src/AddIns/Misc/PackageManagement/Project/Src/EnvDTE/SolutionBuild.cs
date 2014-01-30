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
