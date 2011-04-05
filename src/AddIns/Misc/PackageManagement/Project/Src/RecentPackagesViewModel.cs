// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RecentPackagesViewModel : PackagesViewModel
	{
		IPackageRepository recentPackageRepository;
		
		public RecentPackagesViewModel(
			IPackageManagementService packageManagementService,
			IMessageReporter messageReporter,
			ITaskFactory taskFactory)
			: base(packageManagementService, messageReporter, taskFactory)
		{
			recentPackageRepository = packageManagementService.RecentPackageRepository;
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
			PackageManagementService.ParentPackageInstalled -= ParentPackageInstalled;
			PackageManagementService.ParentPackageUninstalled -= ParentPackageUninstalled;
		}
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			return recentPackageRepository.GetPackages();
		}
	}
}
