// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SharpDevelopPackageRepositoryFactory : PackageRepositoryFactory, ISharpDevelopPackageRepositoryFactory
	{
		IPackageManagementEvents packageManagementEvents;
		
		public SharpDevelopPackageRepositoryFactory()
			: this(PackageManagementServices.PackageManagementEvents)
		{
		}
		
		public SharpDevelopPackageRepositoryFactory(IPackageManagementEvents packageManagementEvents)
		{
			this.packageManagementEvents = packageManagementEvents;
		}
		
		public ISharedPackageRepository CreateSharedRepository(
			IPackagePathResolver pathResolver,
			IFileSystem fileSystem,
			IFileSystem configSettingsFileSystem)
		{
			return new SharedPackageRepository(pathResolver, fileSystem, configSettingsFileSystem);
		}
		
		public IRecentPackageRepository CreateRecentPackageRepository(
			IList<RecentPackageInfo> recentPackages,
			IPackageRepository aggregateRepository)
		{
			return new RecentPackageRepository(recentPackages, aggregateRepository, packageManagementEvents);
		}
		
		public IPackageRepository CreateAggregateRepository(IEnumerable<IPackageRepository> repositories)
		{
			return new AggregateRepository(repositories);
		}
	}
}
