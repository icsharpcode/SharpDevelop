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
			ShowPackageSources = true;
			ShowUpdateAllPackages = true;
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
		
		protected override void TryUpdatingAllPackages()
		{
			if (selectedProjects.HasSingleProjectSelected()) {
				IEnumerable<IPackageFromRepository> packages = GetPackagesFromViewModels();
				IPackageRepository repository = packages.First().Repository;
				IPackageManagementProject project = selectedProjects.GetSingleProjectSelected(repository);
				project.Logger = logger;
				
				UpdatePackagesAction action = project.CreateUpdatePackagesAction();
				action.AddPackages(packages);
				
				IEnumerable<PackageOperation> operations = project.GetUpdatePackagesOperations(packages, action);
				action.AddOperations(operations);
				
				ActionRunner.Run(action);
			} else {
				IEnumerable<IPackageFromRepository> packages = GetPackagesFromViewModels();
				IPackageRepository repository = packages.First().Repository;
				
				var action = new UpdateSolutionPackagesAction(selectedProjects.Solution);
				action.Logger = logger;
				action.AddPackages(packages);
				
				IPackageManagementProject project = selectedProjects.Solution.GetProjects(repository).First();
				project.Logger = logger;
				
				IEnumerable<PackageOperation> operations = project.GetUpdatePackagesOperations(packages, action);
				action.AddOperations(operations);
				
				ActionRunner.Run(action);
			}
		}
		
		IEnumerable<IPackageFromRepository> GetPackagesFromViewModels()
		{
			return PackageViewModels.Select(viewModel => viewModel.GetPackage() as IPackageFromRepository);
		}
	}
}
