// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SharpDevelopPackageManager : PackageManager, ISharpDevelopPackageManager
	{
		IProjectSystem projectSystem;
		IPackageOperationResolverFactory packageOperationResolverFactory;
		
		public SharpDevelopPackageManager(
			IPackageRepository sourceRepository,
			IProjectSystem projectSystem,
			ISolutionPackageRepository solutionPackageRepository,
			IPackageOperationResolverFactory packageOperationResolverFactory)
			: base(
				sourceRepository,
				solutionPackageRepository.PackagePathResolver,
				solutionPackageRepository.FileSystem,
				solutionPackageRepository.Repository)
		{
			this.projectSystem = projectSystem;
			this.packageOperationResolverFactory = packageOperationResolverFactory;
			CreateProjectManager();
		}
		
		// <summary>
		/// project manager should be created with:
		/// 	local repo = PackageReferenceRepository(projectSystem, sharedRepo)
		///     packageRefRepo should have its RegisterIfNecessary() method called before creating the project manager.
		/// 	source repo = sharedRepository
		/// </summary>
		void CreateProjectManager()
		{
			var packageRefRepository = CreatePackageReferenceRepository();
			ProjectManager = CreateProjectManager(packageRefRepository);
		}
		
		PackageReferenceRepository CreatePackageReferenceRepository()
		{
			var sharedRepository = LocalRepository as ISharedPackageRepository;
			var packageRefRepository = new PackageReferenceRepository(projectSystem, sharedRepository);
			packageRefRepository.RegisterIfNecessary();
			return packageRefRepository;
		}
		
		public ISharpDevelopProjectManager ProjectManager { get; set; }
		
		SharpDevelopProjectManager CreateProjectManager(PackageReferenceRepository packageRefRepository)
		{
			return new SharpDevelopProjectManager(LocalRepository, PathResolver, projectSystem, packageRefRepository);
		}
		
		public void InstallPackage(IPackage package)
		{
			bool ignoreDependencies = false;
			bool allowPreleaseVersions = false;
			InstallPackage(package, ignoreDependencies, allowPreleaseVersions);
		}
		
		public void InstallPackage(IPackage package, IEnumerable<PackageOperation> operations, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			foreach (PackageOperation operation in operations) {
				Execute(operation);
			}
			AddPackageReference(package, ignoreDependencies, allowPrereleaseVersions);
		}
		
		void AddPackageReference(IPackage package, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			ProjectManager.AddPackageReference(package.Id, package.Version, ignoreDependencies, allowPrereleaseVersions);			
		}
		
		public override void InstallPackage(IPackage package, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			base.InstallPackage(package, ignoreDependencies, allowPrereleaseVersions);
			AddPackageReference(package, ignoreDependencies, allowPrereleaseVersions);
		}
		
		public override void UninstallPackage(IPackage package, bool forceRemove, bool removeDependencies)
		{
			ProjectManager.RemovePackageReference(package.Id, forceRemove, removeDependencies);
			if (!IsPackageReferencedByOtherProjects(package)) {
				base.UninstallPackage(package, forceRemove, removeDependencies);
			}
		}
		
		bool IsPackageReferencedByOtherProjects(IPackage package)
		{
			var sharedRepository = LocalRepository as ISharedPackageRepository;
			return sharedRepository.IsReferenced(package.Id, package.Version);
		}
		
		public IEnumerable<PackageOperation> GetInstallPackageOperations(IPackage package, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			IPackageOperationResolver resolver = CreateInstallPackageOperationResolver(ignoreDependencies, allowPrereleaseVersions);
			return resolver.ResolveOperations(package);
		}
		
		IPackageOperationResolver CreateInstallPackageOperationResolver(bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			return packageOperationResolverFactory.CreateInstallPackageOperationResolver(
				LocalRepository,
				SourceRepository,
				Logger,
				ignoreDependencies,
				allowPrereleaseVersions);
		}
		
		public void UpdatePackage(IPackage package, IEnumerable<PackageOperation> operations, bool updateDependencies, bool allowPrereleaseVersions)
		{
			foreach (PackageOperation operation in operations) {
				Execute(operation);
			}
			UpdatePackageReference(package, updateDependencies, allowPrereleaseVersions);
		}
		
		void UpdatePackageReference(IPackage package, bool updateDependencies, bool allowPrereleaseVersions)
		{
			ProjectManager.UpdatePackageReference(package.Id, package.Version, updateDependencies, allowPrereleaseVersions);		
		}
	}
}
