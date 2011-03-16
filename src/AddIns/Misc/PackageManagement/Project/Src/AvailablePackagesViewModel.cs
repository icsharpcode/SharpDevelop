// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class AvailablePackagesViewModel : PackagesViewModel
	{
		IPackageRepository repository;
		
		public AvailablePackagesViewModel(
			IPackageManagementService packageManagementService,
			IMessageReporter messageReporter,
			ITaskFactory taskFactory)
			: base(packageManagementService, messageReporter, taskFactory)
		{
			IsSearchable = true;
			ShowPackageSources = packageManagementService.HasMultiplePackageSources;
		}
		
		protected override void UpdateRepositoryBeforeReadPackagesTaskStarts()
		{
			repository = PackageManagementService.ActivePackageRepository;
		}
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			return repository.GetPackages();
		}
		
		protected override IEnumerable<IPackage> GetFilteredPackagesBeforePagingResults(IQueryable<IPackage> allPackages)
		{
			IEnumerable<IPackage> filteredPackages = base.GetFilteredPackagesBeforePagingResults(allPackages);
			return GetDistinctPackagesById(filteredPackages);
		}
		
		IEnumerable<IPackage> GetDistinctPackagesById(IEnumerable<IPackage> allPackages)
		{
			return allPackages.DistinctLast<IPackage>(PackageEqualityComparer.Id);
		}
	}
}
