// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SelectedProjectsForUpdatedPackages : PackageManagementSelectedProjects
	{
		public SelectedProjectsForUpdatedPackages(IPackageManagementSolution solution)
			: base(solution)
		{
		}
		
		protected override bool IsProjectSelected(IPackageManagementProject project, IPackageFromRepository package)
		{
			return IsProjectEnabled(project, package);
		}
		
		protected override bool IsProjectEnabled(IPackageManagementProject project, IPackageFromRepository package)
		{
			return project.GetPackages()
				.Where(p => IsPackageIdMatch(p.Id, package.Id))
				.Any(p => p.Version < package.Version);
		}
		
		bool IsPackageIdMatch(string id1, string id2)
		{
			return String.Equals(id1, id2, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
