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
		IProjectManager projectManager;
		string errorMessage = String.Empty;

		public InstalledPackagesViewModel(
			IPackageManagementService packageManagementService,
			ITaskFactory taskFactory)
			: base(packageManagementService, taskFactory)
		{
			packageManagementService.PackageInstalled += PackageInstalled;
			packageManagementService.PackageUninstalled += PackageUninstalled;
			
			GetActiveProjectManager();
		}
		
		void GetActiveProjectManager()
		{
			try {
				this.projectManager = PackageManagementService.ActiveProjectManager;
			} catch (Exception ex) {
				SaveError(ex);
				errorMessage = ErrorMessage;
			}
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
			if (projectManager == null) {
				ThrowOriginalExceptionWhenTryingToGetProjectManager();
			}
			return projectManager.LocalRepository;
		}
		
		void ThrowOriginalExceptionWhenTryingToGetProjectManager()
		{
			throw new ApplicationException(errorMessage);
		}
	}
}
