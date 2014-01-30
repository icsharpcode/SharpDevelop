// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
