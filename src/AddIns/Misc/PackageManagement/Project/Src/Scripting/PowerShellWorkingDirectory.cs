// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PowerShellWorkingDirectory
	{
		IPackageManagementProjectService projectService;
		
		public PowerShellWorkingDirectory(IPackageManagementProjectService projectService)
		{
			this.projectService = projectService;
		}
		
		public string GetWorkingDirectory()
		{
			Solution solution = projectService.OpenSolution;
			if (solution != null) {
				return QuotedDirectory(solution.Directory);
			}
			return "$env:USERPROFILE";
		}
		
		string QuotedDirectory(string directory)
		{
			return String.Format("'{0}'", directory);
		}
	}
}
