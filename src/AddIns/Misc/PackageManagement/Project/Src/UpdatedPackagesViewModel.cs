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
		IPackageManagementSolution solution;
		UpdatedPackages updatedPackages;
		string errorMessage = String.Empty;
		
		public UpdatedPackagesViewModel(
			IPackageManagementSolution solution,
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageViewModelFactory packageViewModelFactory,
			ITaskFactory taskFactory)
			: this(
				solution,
				registeredPackageRepositories,
				new UpdatedPackageViewModelFactory(packageViewModelFactory),
				taskFactory)
		{
		}
		
		public UpdatedPackagesViewModel(
			IPackageManagementSolution packageManagementService,
			IRegisteredPackageRepositories registeredPackageRepositories,
			UpdatedPackageViewModelFactory packageViewModelFactory,			
			ITaskFactory taskFactory)
			: base(
				registeredPackageRepositories,
				packageViewModelFactory,
				taskFactory)
		{
			this.solution = packageManagementService;
		}
		
		protected override void UpdateRepositoryBeforeReadPackagesTaskStarts()
		{
			try {
				IPackageRepository aggregateRepository = RegisteredPackageRepositories.CreateAggregateRepository();
				IQueryable<IPackage> installedPackages = GetInstalledPackages(aggregateRepository);
				updatedPackages = new UpdatedPackages(installedPackages, aggregateRepository);
			} catch (Exception ex) {
				errorMessage = ex.Message;
			}
		}
		
		IQueryable<IPackage> GetInstalledPackages(IPackageRepository aggregateRepository)
		{
			var selectedProjects = new PackageManagementSelectedProjects(solution);
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
