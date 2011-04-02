// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageUpdatesViewModel : PackagesViewModel
	{
		List<IPackage> packages = new List<IPackage>();
		IPackageManagementService packageManagementService;
		IPackageRepository localRepository;
		IPackageRepository sourceRepository;
		string errorMessage = String.Empty;
		
		public PackageUpdatesViewModel(
			IPackageManagementService packageManagementService,
			IMessageReporter messageReporter,
			ITaskFactory taskFactory)
			: this(
				packageManagementService,
				messageReporter,
				new LicenseAcceptanceService(),
				taskFactory)
		{
		}
		
		public PackageUpdatesViewModel(
			IPackageManagementService packageManagementService,
			IMessageReporter messageReporter,
			ILicenseAcceptanceService licenseAcceptanceService,
			ITaskFactory taskFactory)
			: base(
				packageManagementService,
				new UpdatedPackageViewModelFactory(packageManagementService, licenseAcceptanceService, messageReporter),
				taskFactory)
		{
			this.packageManagementService = packageManagementService;
		}
		
		protected override void UpdateRepositoryBeforeReadPackagesTaskStarts()
		{
			try {
				IProjectManager projectManager = packageManagementService.ActiveProjectManager;
				localRepository = projectManager.LocalRepository;
			} catch (Exception ex) {
				errorMessage = ex.Message;
			}
			sourceRepository = packageManagementService.CreateAggregatePackageRepository();
		}
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			if (localRepository == null) {
				ThrowSavedException();
			}
			IQueryable<IPackage> localPackages = localRepository.GetPackages();
			return GetUpdatedPackages(localPackages);
		}
		
		IQueryable<IPackage> GetUpdatedPackages(IQueryable<IPackage> localPackages)
		{
			return sourceRepository.GetUpdates(localPackages).AsQueryable();
		}
		
		void ThrowSavedException()
		{
			throw new ApplicationException(errorMessage);
		}
	}
}
