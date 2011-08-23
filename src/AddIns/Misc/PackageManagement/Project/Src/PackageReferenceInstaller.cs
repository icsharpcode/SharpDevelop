// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageReferenceInstaller : IPackageReferenceInstaller
	{
		IPackageActionRunner packageActionRunner;
		IPackageManagementProjectFactory projectFactory;
		IPackageRepositoryCache packageRepositoryCache;
		
		public PackageReferenceInstaller(IPackageRepositoryCache packageRepositoryCache)
			: this(
				packageRepositoryCache,
				PackageManagementServices.PackageActionRunner,
				new PackageManagementProjectFactory(PackageManagementServices.PackageManagementEvents))
		{
		}
		
		public PackageReferenceInstaller(
			IPackageRepositoryCache packageRepositoryCache,
			IPackageActionRunner packageActionRunner,
			IPackageManagementProjectFactory projectFactory)
		{
			this.packageRepositoryCache = packageRepositoryCache;
			this.packageActionRunner = packageActionRunner;
			this.projectFactory = projectFactory;
		}
		
		public void InstallPackages(
			IEnumerable<PackageReference> packageReferences,
			MSBuildBasedProject project)
		{
			List<InstallPackageAction> actions = GetInstallPackageActions(packageReferences, project);
			packageActionRunner.Run(actions);
		}
		
		List<InstallPackageAction> GetInstallPackageActions(
			IEnumerable<PackageReference> packageReferences,
			MSBuildBasedProject project)
		{
			var actions = new List<InstallPackageAction>();
			
			IPackageManagementProject packageManagementProject = CreatePackageManagementProject(project);
			foreach (PackageReference packageReference in packageReferences) {
				InstallPackageAction action = CreateInstallPackageAction(packageManagementProject, packageReference);
				actions.Add(action);
			}
			
			return actions;
		}
		
		IPackageManagementProject CreatePackageManagementProject(MSBuildBasedProject project)
		{
			IPackageRepository repository = packageRepositoryCache.CreateAggregateRepository();
			return projectFactory.CreateProject(repository, project);
		}
		
		InstallPackageAction CreateInstallPackageAction(
			IPackageManagementProject project,
			PackageReference packageReference)
		{
			InstallPackageAction action = project.CreateInstallPackageAction();
			action.PackageId = packageReference.Id;
			action.PackageVersion = packageReference.Version;
			return action;
		}
	}
}
