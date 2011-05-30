// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RecentPackagesViewModel : PackagesViewModel
	{
		IPackageManagementEvents packageManagementEvents;
		IPackageRepository recentPackageRepository;
		
		public RecentPackagesViewModel(
			IPackageManagementEvents packageManagementEvents,
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageViewModelFactory packageViewModelFactory,
			ITaskFactory taskFactory)
			: base(registeredPackageRepositories, packageViewModelFactory, taskFactory)
		{
			this.packageManagementEvents = packageManagementEvents;
			
			recentPackageRepository = registeredPackageRepositories.RecentPackageRepository;
			
			packageManagementEvents.ParentPackageInstalled += ParentPackageInstalled;
			packageManagementEvents.ParentPackageUninstalled += ParentPackageUninstalled;
		}
		
		void ParentPackageInstalled(object sender, EventArgs e)
		{
			ReadPackages();
		}
		
		void ParentPackageUninstalled(object sender, EventArgs e)
		{
			ReadPackages();
		}
		
		protected override void OnDispose()
		{
			packageManagementEvents.ParentPackageInstalled -= ParentPackageInstalled;
			packageManagementEvents.ParentPackageUninstalled -= ParentPackageUninstalled;
		}
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			return recentPackageRepository.GetPackages();
		}
	}
}
