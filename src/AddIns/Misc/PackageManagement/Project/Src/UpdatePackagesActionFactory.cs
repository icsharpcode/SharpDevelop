// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdatePackagesActionFactory
	{
		ILogger logger;
		IPackageManagementEvents packageManagementEvents;
		bool singleProjectSelected;
		IPackageManagementProject project;
		PackageManagementSelectedProjects selectedProjects;
		IEnumerable<IPackageFromRepository> packages;
		
		public UpdatePackagesActionFactory(ILogger logger, IPackageManagementEvents packageManagementEvents)
		{
			this.logger = logger;
			this.packageManagementEvents = packageManagementEvents;
		}
		
		public IUpdatePackagesAction CreateAction(
			PackageManagementSelectedProjects selectedProjects,
			IEnumerable<IPackageFromRepository> packages)
		{
			this.selectedProjects = selectedProjects;
			this.packages = packages;
			
			singleProjectSelected = selectedProjects.HasSingleProjectSelected();
			
			CreateProjectForDetermingPackageOperations();
			IUpdatePackagesAction action = CreateActionInternal();
			action.AddPackages(packages);
			action.Logger = logger;
			
			IEnumerable<PackageOperation> operations = GetPackageOperations(action);
			action.AddOperations(operations);
			
			return action;
		}
		
		IEnumerable<PackageOperation> GetPackageOperations(IUpdatePackagesAction action)
		{
			return project.GetUpdatePackagesOperations(packages, action);
		}
		
		IUpdatePackagesAction CreateActionInternal()
		{
			if (singleProjectSelected) {
				return project.CreateUpdatePackagesAction();
			} else {
				return new UpdateSolutionPackagesAction(selectedProjects.Solution, packageManagementEvents);
			}
		}
		
		void CreateProjectForDetermingPackageOperations()
		{
			IPackageRepository repository = packages.First().Repository;
			if (singleProjectSelected) {
				project = selectedProjects.GetSingleProjectSelected(repository);
			} else {
				project = selectedProjects.Solution.GetProjects(repository).First();
			}
			project.Logger = logger;
		}
	}
}
