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
		IPackageRepository repository;
		string errorMessage = String.Empty;

		public InstalledPackagesViewModel(
			IPackageManagementService packageManagementService,
			IMessageReporter messageReporter,
			ITaskFactory taskFactory)
			: base(packageManagementService, messageReporter, taskFactory)
		{
			packageManagementService.ParentPackageInstalled += ParentPackageInstalled;
			packageManagementService.ParentPackageUninstalled += ParentPackageUninstalled;
			
			GetActiveProjectManager();
		}
		
		void GetActiveProjectManager()
		{
			try {
				this.projectManager = PackageManagementService.ActiveProjectManager;
			} catch (Exception ex) {
				errorMessage = ex.Message;
			}
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
		
		protected override void UpdateRepositoryBeforeReadPackagesTaskStarts()
		{
			if (projectManager != null) {
				repository = projectManager.LocalRepository;
			}
		}
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			if (projectManager == null) {
				ThrowOriginalExceptionWhenTryingToGetProjectManager();
			}
			return repository.GetPackages();
		}
		
		void ThrowOriginalExceptionWhenTryingToGetProjectManager()
		{
			throw new ApplicationException(errorMessage);
		}
	}
}
