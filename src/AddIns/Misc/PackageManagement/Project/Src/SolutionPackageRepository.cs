// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SolutionPackageRepository : ISolutionPackageRepository
	{
		SolutionPackageRepositoryPath repositoryPath;
		ISharpDevelopPackageRepositoryFactory repositoryFactory;
		DefaultPackagePathResolver packagePathResolver;
		PhysicalFileSystem fileSystem;
		ISharedPackageRepository repository;
		
		public SolutionPackageRepository(Solution solution)
			: this(
				solution,
				new SharpDevelopPackageRepositoryFactory(),
				PackageManagementServices.Options)
		{
		}
		
		public SolutionPackageRepository(
			Solution solution,
			ISharpDevelopPackageRepositoryFactory repositoryFactory,
			PackageManagementOptions options)
		{
			this.repositoryFactory = repositoryFactory;
			repositoryPath = new SolutionPackageRepositoryPath(solution, options);
			CreatePackagePathResolver();
			CreateFileSystem();
			CreateRepository(ConfigSettingsFileSystem.CreateConfigSettingsFileSystem(solution));
		}
		
		void CreatePackagePathResolver()
		{
			packagePathResolver = new DefaultPackagePathResolver(repositoryPath.PackageRepositoryPath);
		}
		
		void CreateFileSystem()
		{
			fileSystem = new PhysicalFileSystem(repositoryPath.PackageRepositoryPath);
		}
		
		void CreateRepository(ConfigSettingsFileSystem configSettingsFileSystem)
		{
			repository = repositoryFactory.CreateSharedRepository(packagePathResolver, fileSystem, configSettingsFileSystem);			
		}
		
		public ISharedPackageRepository Repository {
			get { return repository; }
		}
		
		public IFileSystem FileSystem {
			get { return fileSystem; }
		}
		
		public IPackagePathResolver PackagePathResolver {
			get { return packagePathResolver; }
		}
		
		public string GetInstallPath(IPackage package)
		{
			return repositoryPath.GetInstallPath(package);
		}
		
		public IEnumerable<IPackage> GetPackagesByDependencyOrder()
		{
			var packageSorter = new PackageSorter(null);
			return packageSorter.GetPackagesByDependencyOrder(repository);
		}
		
		public IEnumerable<IPackage> GetPackagesByReverseDependencyOrder()
		{
			return GetPackagesByDependencyOrder().Reverse();
		}
		
		public bool IsInstalled(IPackage package)
		{
			return repository.Exists(package);
		}
		
		public IQueryable<IPackage> GetPackages()
		{
			return repository.GetPackages();
		}
	}
}
