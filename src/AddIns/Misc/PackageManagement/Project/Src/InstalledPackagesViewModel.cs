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
		IPackageManagementSolution solution;
		IPackageManagementEvents packageManagementEvents;
		IPackageManagementProject project;
		string errorMessage;

		public InstalledPackagesViewModel(
			IPackageManagementSolution solution,
			IPackageManagementEvents packageManagementEvents,
			IRegisteredPackageRepositories registeredPackageRepositories,
			InstalledPackageViewModelFactory packageViewModelFactory,
			ITaskFactory taskFactory)
			: base(registeredPackageRepositories, packageViewModelFactory, taskFactory)
		{
			this.solution = solution;
			this.packageManagementEvents = packageManagementEvents;
			packageManagementEvents.ParentPackageInstalled += InstalledPackagesChanged;
			packageManagementEvents.ParentPackageUninstalled += InstalledPackagesChanged;
			
			TryGetActiveProject();
		}
		
		void TryGetActiveProject()
		{
			try {
				project = solution.GetActiveProject();
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
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			if (errorMessage != null) {
				ThrowOriginalExceptionWhenTryingToGetProjectManager();
			}
			if (project != null) {
				return project.GetPackages();
			}
			return solution.GetPackages();
		}
		
		void ThrowOriginalExceptionWhenTryingToGetProjectManager()
		{
			throw new ApplicationException(errorMessage);
		}
	}
}
