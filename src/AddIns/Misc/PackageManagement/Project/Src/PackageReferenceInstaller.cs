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
			IPackageManagementProject packageManagementProject = CreatePackageManagementProject(project);
			foreach (PackageReference packageReference in packageReferences) {
				InstallPackageAction action = CreateInstallPackageAction(packageManagementProject, packageReference);
				packageActionRunner.Run(action);
			}
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
