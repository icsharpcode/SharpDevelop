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
		
		public RecentPackagesViewModel(IPackageManagementService packageManagementService, ITaskFactory taskFactory)
			: base(packageManagementService, taskFactory)
		{
			recentPackageRepository = packageManagementService.RecentPackageRepository;
			packageManagementService.PackageInstalled += PackageInstalled;
			packageManagementService.PackageUninstalled += PackageUninstalled;
		}
		
		void PackageInstalled(object sender, EventArgs e)
		{
			ReadPackages();
		}
		
		void PackageUninstalled(object sender, EventArgs e)
		{
			ReadPackages();
		}
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			return recentPackageRepository.GetPackages();
		}
	}
}
