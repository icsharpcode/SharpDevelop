// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement
{
	public class InstalledPackageViewModelFactory : PackageViewModelFactory
	{
		SelectedProjectsForInstalledPackages selectedProjectsForInstalledPackages;
		
		public InstalledPackageViewModelFactory(IPackageViewModelFactory packageViewModelFactory)
			: base(packageViewModelFactory)
		{
			selectedProjectsForInstalledPackages = new SelectedProjectsForInstalledPackages(Solution);
		}
		
		public override PackageViewModel CreatePackageViewModel(IPackageFromRepository package)
		{
			return new InstalledPackageViewModel(
				package,
				selectedProjectsForInstalledPackages,
				PackageManagementEvents,
				PackageActionRunner,
				Logger);
		}
	}
}
