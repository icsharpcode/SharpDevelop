// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RecentPackagesViewModel : PackagesViewModel
	{
		IPackageManagementService packageManagementService;
		IPackageRepository recentPackageRepository;
		
		public RecentPackagesViewModel(
			IPackageManagementService packageManagementService,
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageViewModelFactory packageViewModelFactory,
			ITaskFactory taskFactory)
			: base(registeredPackageRepositories, packageViewModelFactory, taskFactory)
		{
			this.packageManagementService = packageManagementService;
			recentPackageRepository = registeredPackageRepositories.RecentPackageRepository;
			packageManagementService.ParentPackageInstalled += ParentPackageInstalled;
			packageManagementService.ParentPackageUninstalled += ParentPackageUninstalled;
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
			packageManagementService.ParentPackageInstalled -= ParentPackageInstalled;
			packageManagementService.ParentPackageUninstalled -= ParentPackageUninstalled;
		}
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			return recentPackageRepository.GetPackages();
		}
	}
}
