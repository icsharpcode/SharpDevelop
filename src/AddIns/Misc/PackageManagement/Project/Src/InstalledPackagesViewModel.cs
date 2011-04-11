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
		IPackageManagementService packageManagementService;
		IPackageManagementEvents packageManagementEvents;
		IProjectManager projectManager;
		IPackageRepository repository;
		string errorMessage = String.Empty;

		public InstalledPackagesViewModel(
			IPackageManagementService packageManagementService,
			IPackageManagementEvents packageManagementEvents,
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageViewModelFactory packageViewModelFactory,
			ITaskFactory taskFactory)
			: base(registeredPackageRepositories, packageViewModelFactory, taskFactory)
		{
			this.packageManagementService = packageManagementService;
			this.packageManagementEvents = packageManagementEvents;
			packageManagementEvents.ParentPackageInstalled += InstalledPackagesChanged;
			packageManagementEvents.ParentPackageUninstalled += InstalledPackagesChanged;
			
			GetActiveProjectManager();
		}
		
		void GetActiveProjectManager()
		{
			try {
				this.projectManager = packageManagementService.ActiveProjectManager;
			} catch (Exception ex) {
				errorMessage = ex.Message;
			}
		}

		void InstalledPackagesChanged(object sender, EventArgs e)
		{
			ReadPackages();
		}
		
		protected override void OnDispose()
		{
			packageManagementEvents.ParentPackageInstalled -= InstalledPackagesChanged;
			packageManagementEvents.ParentPackageUninstalled -= InstalledPackagesChanged;
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
