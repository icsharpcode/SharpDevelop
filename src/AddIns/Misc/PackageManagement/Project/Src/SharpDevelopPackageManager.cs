// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SharpDevelopPackageManager : PackageManager, ISharpDevelopPackageManager
	{
		IProjectSystem projectSystem;
		
		public SharpDevelopPackageManager(
			IPackageRepository sourceRepository,
			MSBuildBasedProject project,
			ISharedPackageRepository localRepository,
			PackageRepositoryPaths repositoryPaths)
			: this(
				sourceRepository,
				new SharpDevelopProjectSystem(project),
				localRepository,
				repositoryPaths)
		{
		}
		
		public SharpDevelopPackageManager(
			IPackageRepository sourceRepository,
			IProjectSystem projectSystem,
			ISharedPackageRepository localRepository,
			PackageRepositoryPaths repositoryPaths)
			: this(
				sourceRepository,
				projectSystem,
				localRepository,
				repositoryPaths,
				repositoryPaths.SolutionPackagesPath)
		{
		}
		
		SharpDevelopPackageManager(
			IPackageRepository sourceRepository,
			IProjectSystem projectSystem,
			ISharedPackageRepository localRepository,
			PackageRepositoryPaths repositoryPaths,
			string solutionPackagesPath)
			: base(
				sourceRepository,
				new DefaultPackagePathResolver(solutionPackagesPath),
				new PhysicalFileSystem(solutionPackagesPath),
				localRepository)
		{
			this.projectSystem = projectSystem;
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
			InstallPackage(package, ignoreDependencies);
		}
		
		public void InstallPackage(IPackage package, IEnumerable<PackageOperation> operations)
		{
			foreach (PackageOperation operation in operations) {
				Execute(operation);
			}
			AddPackageReference(package);
		}

		void AddPackageReference(IPackage package)
		{
			bool ignoreDependencies = false;
			AddPackageReference(package, ignoreDependencies);
		}
		
		void AddPackageReference(IPackage package, bool ignoreDependencies)
		{
			ProjectManager.AddPackageReference(package.Id, package.Version, ignoreDependencies);			
		}
		
		public override void InstallPackage(IPackage package, bool ignoreDependencies)
		{
			base.InstallPackage(package, ignoreDependencies);
			AddPackageReference(package, ignoreDependencies);
		}
		
		public override void UninstallPackage(IPackage package, bool forceRemove, bool removeDependencies)
		{
			ProjectManager.RemovePackageReference(package.Id, forceRemove, removeDependencies);
			base.UninstallPackage(package, forceRemove, removeDependencies);
		}
	}
}
