// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class SolutionBuild : MarshalByRefObject, global::EnvDTE.SolutionBuild
	{
		Solution solution;
		
		public SolutionBuild()
		{
		}
		
		public SolutionBuild(Solution solution)
		{
			this.solution = solution;
		}
		
		public global::EnvDTE.SolutionConfiguration ActiveConfiguration {
			get { return solution.GetActiveConfiguration(); }
		}
		
		/// <summary>
		/// Returns the number of projects that failed to build.
		/// </summary>
		public int LastBuildInfo {
			get { return 0; }
		}
		
		public void BuildProject(string solutionConfiguration, string projectUniqueName, bool waitForBuildToFinish)
		{
			throw new NotImplementedException();
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
