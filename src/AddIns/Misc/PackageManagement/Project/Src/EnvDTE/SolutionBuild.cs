// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class SolutionBuild : MarshalByRefObject, global::EnvDTE.SolutionBuild
	{
		public SolutionBuild()
		{
		}
		
		public global::EnvDTE.SolutionConfiguration ActiveConfiguration {
			get { throw new NotImplementedException(); }
		}
		
		/// <summary>
		/// Returns the number of projects that failed to build.
		/// </summary>
		public int LastBuildInfo {
			get { throw new NotImplementedException(); }
		}
		
		public void BuildProject(string solutionConfiguration, string projectUniqueName, bool waitForBuildToFinish)
		{
			throw new NotImplementedException();
		}
	}
}
