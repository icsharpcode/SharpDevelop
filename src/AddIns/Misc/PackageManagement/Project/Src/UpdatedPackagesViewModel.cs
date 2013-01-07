// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdatedPackagesViewModel : PackagesViewModel
	{
		PackageManagementSelectedProjects selectedProjects;
		UpdatedPackages updatedPackages;
		string errorMessage = String.Empty;
		
		public UpdatedPackagesViewModel(
			IPackageManagementSolution solution,
			IRegisteredPackageRepositories registeredPackageRepositories,
			UpdatedPackageViewModelFactory packageViewModelFactory,
			ITaskFactory taskFactory)
			: base(
				registeredPackageRepositories,
				packageViewModelFactory,
				taskFactory)
		{
			this.selectedProjects = new PackageManagementSelectedProjects(solution);
			ShowPackageSources = true;
		}
		
		protected override void UpdateRepositoryBeforeReadPackagesTaskStarts()
		{
			try {
				IPackageRepository repository = RegisteredPackageRepositories.ActiveRepository;
				IQueryable<IPackage> installedPackages = GetInstalledPackages(repository);
				updatedPackages = new UpdatedPackages(installedPackages, repository);
			} catch (Exception ex) {
				errorMessage = ex.Message;
			}
		}
		
		IQueryable<IPackage> GetInstalledPackages(IPackageRepository aggregateRepository)
		{
			return selectedProjects.GetInstalledPackages(aggregateRepository);
		}
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			if (updatedPackages == null) {
				ThrowSavedException();
			}
			return GetUpdatedPackages();
		}
		
		void ThrowSavedException()
		{
			throw new ApplicationException(errorMessage);
		}
		
		IQueryable<IPackage> GetUpdatedPackages()
		{
			return updatedPackages.GetUpdatedPackages().AsQueryable();
		}
	}
}
