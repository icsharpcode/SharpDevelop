// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SharpDevelopPackageManagerFactory : IPackageManagerFactory
	{
		PackageManagementOptions options;
		ISharpDevelopPackageRepositoryFactory packageRepositoryFactory;
		ISharpDevelopProjectSystemFactory projectSystemFactory;
		
		public SharpDevelopPackageManagerFactory()
			: this(new SharpDevelopPackageRepositoryFactory(),
			       new SharpDevelopProjectSystemFactory(),
			       new PackageManagementOptions())
		{
		}
		
		public SharpDevelopPackageManagerFactory(ISharpDevelopPackageRepositoryFactory packageRepositoryFactory,
		                                         ISharpDevelopProjectSystemFactory projectSystemFactory,
		                                         PackageManagementOptions options)
		{
			this.packageRepositoryFactory = packageRepositoryFactory;
			this.projectSystemFactory = projectSystemFactory;
			this.options = options;
		}
		
		public ISharpDevelopPackageManager CreatePackageManager(IPackageRepository packageRepository, MSBuildBasedProject project)
		{
			IFileSystem fileSystem = CreateFileSystemThatWillContainPackages(project);
			return CreatePackageManager(fileSystem, packageRepository, project);
		}
		
		IFileSystem CreateFileSystemThatWillContainPackages(MSBuildBasedProject project)
		{
			var repositoryPaths = new PackageRepositoryPaths(project, options);
			return new PhysicalFileSystem(repositoryPaths.SolutionPackagesPath);
		}
		
		ISharpDevelopPackageManager CreatePackageManager(
			IFileSystem fileSystem, 
			IPackageRepository packageRepository, 
			MSBuildBasedProject project)
		{
			DefaultPackagePathResolver pathResolver = new DefaultPackagePathResolver(fileSystem);
			ISharedPackageRepository sharedRepository = CreateSharedRepository(pathResolver, fileSystem);
			IProjectSystem projectSystem = CreateProjectSystem(project);
			return new SharpDevelopPackageManager(packageRepository, projectSystem, fileSystem, sharedRepository, pathResolver);
		}
		
		IProjectSystem CreateProjectSystem(MSBuildBasedProject project)
		{
			return projectSystemFactory.CreateProjectSystem(project);
		}
		
		ISharedPackageRepository CreateSharedRepository(IPackagePathResolver pathResolver, IFileSystem fileSystem)
		{
			return packageRepositoryFactory.CreateSharedRepository(pathResolver, fileSystem);
		}
	}
}
