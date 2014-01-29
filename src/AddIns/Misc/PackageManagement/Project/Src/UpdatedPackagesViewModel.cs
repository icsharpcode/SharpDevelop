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
		ILogger logger;
		IPackageManagementEvents packageManagementEvents;
		
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
			this.logger = packageViewModelFactory.Logger;
			this.packageManagementEvents = packageViewModelFactory.PackageManagementEvents;
			
			packageManagementEvents.ParentPackagesUpdated += PackagesUpdated;
			
			ShowPackageSources = true;
			ShowUpdateAllPackages = true;
			ShowPrerelease = true;
		}
		
		void PackagesUpdated(object sender, EventArgs e)
		{
			ReadPackages();
		}
		
		protected override void OnDispose()
		{
			packageManagementEvents.ParentPackagesUpdated -= PackagesUpdated;
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
			return updatedPackages.GetUpdatedPackages(IncludePrerelease).AsQueryable();
		}
		
		protected override void TryUpdatingAllPackages()
		{
			List<IPackageFromRepository> packages = GetPackagesFromViewModels().ToList();
			using (IDisposable operation = StartUpdateOperation(packages.First())) {
				var factory = new UpdatePackagesActionFactory(logger, packageManagementEvents);
				IUpdatePackagesAction action = factory.CreateAction(selectedProjects, packages);
				ActionRunner.Run(action);
			}
		}
		
		IDisposable StartUpdateOperation(IPackageFromRepository package)
		{
			return package.Repository.StartUpdateOperation();
		}
		
		IEnumerable<IPackageFromRepository> GetPackagesFromViewModels()
		{
			return PackageViewModels.Select(viewModel => viewModel.GetPackage() as IPackageFromRepository);
		}
	}
}
