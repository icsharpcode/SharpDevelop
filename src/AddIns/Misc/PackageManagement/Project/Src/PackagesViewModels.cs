// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement
{
	public class PackagesViewModels : IDisposable
	{
		public PackagesViewModels(
			IPackageManagementSolution solution,
			IRegisteredPackageRepositories registeredPackageRepositories,
			IThreadSafePackageManagementEvents packageManagementEvents,
			IPackageActionRunner actionRunner,
			ITaskFactory taskFactory)
		{
			var packageViewModelFactory = new PackageViewModelFactory(solution, packageManagementEvents, actionRunner);
			var updatedPackageViewModelFactory = new UpdatedPackageViewModelFactory(packageViewModelFactory);
			var installedPackageViewModelFactory = new InstalledPackageViewModelFactory(packageViewModelFactory);
			
			AvailablePackagesViewModel = new AvailablePackagesViewModel(registeredPackageRepositories, packageViewModelFactory, taskFactory);
			InstalledPackagesViewModel = new InstalledPackagesViewModel(solution, packageManagementEvents, registeredPackageRepositories, installedPackageViewModelFactory, taskFactory);
			UpdatedPackagesViewModel = new UpdatedPackagesViewModel(solution, registeredPackageRepositories, updatedPackageViewModelFactory, taskFactory);
			RecentPackagesViewModel = new RecentPackagesViewModel(packageManagementEvents, registeredPackageRepositories, packageViewModelFactory, taskFactory);
		}
		
		public AvailablePackagesViewModel AvailablePackagesViewModel { get; private set; }
		public InstalledPackagesViewModel InstalledPackagesViewModel { get; private set; }
		public RecentPackagesViewModel RecentPackagesViewModel { get; private set; }
		public UpdatedPackagesViewModel UpdatedPackagesViewModel { get; private set; }
		
		public void ReadPackages()
		{
			AvailablePackagesViewModel.ReadPackages();
			InstalledPackagesViewModel.ReadPackages();
			UpdatedPackagesViewModel.ReadPackages();
			RecentPackagesViewModel.ReadPackages();
		}
		
		public void Dispose()
		{
			AvailablePackagesViewModel.Dispose();
			InstalledPackagesViewModel.Dispose();
			RecentPackagesViewModel.Dispose();
			UpdatedPackagesViewModel.Dispose();
		}
	}
}
