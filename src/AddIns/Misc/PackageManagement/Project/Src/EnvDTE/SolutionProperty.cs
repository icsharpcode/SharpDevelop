// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class SolutionProperty : Property
	{
		Solution solution;
		
		public SolutionProperty(Solution solution, string name)
			: base(name)
		{
			this.solution = solution;
		}
		
		protected override object GetValue()
		{
			if (Name == "StartupProject") {
				return GetStartupProjectName();
			} else if (Name == "Path") {
				return solution.FileName;
			}
			return String.Empty;
		}
		
		string GetStartupProjectName()
		{
			Project project = solution.GetStartupProject();
			if (project != null) {
				return solution.GetStartupProject().Name;
			}
			return String.Empty;
		}
	}
}
