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
			PackageRepositoryPaths repositoryPaths = new PackageRepositoryPaths(project, options);
			IProjectSystem projectSystem = CreateProjectSystem(project);
			ISharedPackageRepository sharedRepository = CreateSharedRepository(repositoryPaths.SolutionPackagesPath);
			return new SharpDevelopPackageManager(packageRepository, projectSystem, sharedRepository, repositoryPaths);
		}
		
		IProjectSystem CreateProjectSystem(MSBuildBasedProject project)
		{
			return projectSystemFactory.CreateProjectSystem(project);
		}
		
		ISharedPackageRepository CreateSharedRepository(string path)
		{
			return packageRepositoryFactory.CreateSharedRepository(path);
		}
	}
}
