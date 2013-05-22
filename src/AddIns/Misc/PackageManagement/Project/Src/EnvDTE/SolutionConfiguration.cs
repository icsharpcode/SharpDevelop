// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class SolutionConfiguration : MarshalByRefObject, global::EnvDTE.SolutionConfiguration
	{
		ISolution solution;
		
		public SolutionConfiguration(ISolution solution)
		{
			this.solution = solution;
		}
		
		public string Name {
			get { return solution.ActiveConfiguration.Configuration; }
		}
	}
}
