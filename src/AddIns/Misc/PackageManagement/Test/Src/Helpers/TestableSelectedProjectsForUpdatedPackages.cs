// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class TestableSelectedProjectsForUpdatedPackages : SelectedProjectsForUpdatedPackages
	{
		public TestableSelectedProjectsForUpdatedPackages(IPackageManagementSolution solution)
			: base(solution)
		{
		}
		
		public bool CallIsProjectEnabled(IPackageManagementProject project, IPackageFromRepository package)
		{
			return base.IsProjectEnabled(project, package);
		}
		
		public bool CallIsProjectSelected(IPackageManagementProject project, IPackageFromRepository package)
		{
			return base.IsProjectSelected(project, package);
		}
	}
}
