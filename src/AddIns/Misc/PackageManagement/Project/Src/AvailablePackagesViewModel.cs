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
		string errorMessage;
		
		public AvailablePackagesViewModel(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageViewModelFactory packageViewModelFactory,
			ITaskFactory taskFactory)
			: base(registeredPackageRepositories, packageViewModelFactory, taskFactory)
		{
			IsSearchable = true;
			ShowPackageSources = true;
			ShowPrerelease = true;
		}
		
		protected override void UpdateRepositoryBeforeReadPackagesTaskStarts()
		{
			try {
				repository = RegisteredPackageRepositories.ActiveRepository;
			} catch (Exception ex) {
				errorMessage = ex.Message;
			}
		}
		
		protected override IQueryable<IPackage> GetAllPackages()
		{
			if (repository == null) {
				throw new ApplicationException(errorMessage);
			}
			if (IncludePrerelease) {
				return repository.GetPackages();
			}
			return repository.GetPackages().Where(package => package.IsLatestVersion);
		}
		
		/// <summary>
		/// Order packages by most downloaded first.
		/// </summary>
		protected override IQueryable<IPackage> OrderPackages(IQueryable<IPackage> packages)
		{
			return packages.OrderByDescending(package => package.DownloadCount);
		}
		
		protected override IEnumerable<IPackage> GetFilteredPackagesBeforePagingResults(IQueryable<IPackage> allPackages)
		{
			if (IncludePrerelease) {
				return base.GetFilteredPackagesBeforePagingResults(allPackages)
					.DistinctLast(PackageEqualityComparer.Id);
			}
			return base.GetFilteredPackagesBeforePagingResults(allPackages)
				.Where(package => package.IsReleaseVersion())
				.DistinctLast(PackageEqualityComparer.Id);
		}
	}
}
