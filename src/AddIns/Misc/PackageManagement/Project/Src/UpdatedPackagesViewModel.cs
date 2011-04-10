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
		IPackageManagementService packageManagementService;
		UpdatedPackages updatedPackages;
		string errorMessage = String.Empty;
		
		public UpdatedPackagesViewModel(
			IPackageManagementService packageManagementService,
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageViewModelFactory packageViewModelFactory,
			ITaskFactory taskFactory)
			: this(
				packageManagementService,
				registeredPackageRepositories,
				new UpdatedPackageViewModelFactory(packageViewModelFactory),
				taskFactory)
		{
		}
		
		public UpdatedPackagesViewModel(
			IPackageManagementService packageManagementService,
			IRegisteredPackageRepositories registeredPackageRepositories,
			UpdatedPackageViewModelFactory packageViewModelFactory,			
			ITaskFactory taskFactory)
			: base(
				registeredPackageRepositories,
				packageViewModelFactory,
				taskFactory)
		{
			this.packageManagementService = packageManagementService;
		}
		
		protected override void UpdateRepositoryBeforeReadPackagesTaskStarts()
		{
			try {
				IPackageRepository aggregateRepository = RegisteredPackageRepositories.CreateAggregateRepository();
				updatedPackages = new UpdatedPackages(packageManagementService, aggregateRepository);
			} catch (Exception ex) {
				errorMessage = ex.Message;
			}
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
