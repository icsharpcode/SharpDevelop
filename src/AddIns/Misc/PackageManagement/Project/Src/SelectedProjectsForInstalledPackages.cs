// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement
{
	public class SelectedProjectsForInstalledPackages : PackageManagementSelectedProjects
	{
		public SelectedProjectsForInstalledPackages(IPackageManagementSolution solution)
			: base(solution)
		{
		}
		
		protected override bool IsProjectSelected(IPackageManagementProject project, IPackageFromRepository package)
		{
			return project.IsPackageInstalled(package);
		}
	}
}
