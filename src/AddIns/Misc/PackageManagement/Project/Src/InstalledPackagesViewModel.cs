// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class InstalledPackagesViewModel : PackagesViewModel
	{
		public InstalledPackagesViewModel(
			IPackageManagementService packageManagementService,
			ITaskFactory taskFactory)
			: base(packageManagementService, taskFactory)
		{
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
			IPackageRepository repository = GetRepository();
			return repository.GetPackages();
		}
		
		IPackageRepository GetRepository()
		{
			return ProjectManager.LocalRepository;
		}
	}
}
