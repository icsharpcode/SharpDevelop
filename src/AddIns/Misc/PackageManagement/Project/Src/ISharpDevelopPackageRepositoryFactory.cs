// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface ISharpDevelopPackageRepositoryFactory : IPackageRepositoryFactory
	{
		ISharedPackageRepository CreateSharedRepository(
			IPackagePathResolver pathResolver,
			IFileSystem fileSystem,
			IFileSystem configSettingsFileSystem);
		
		IRecentPackageRepository CreateRecentPackageRepository(
			IList<RecentPackageInfo> recentPackages,
			IPackageRepository aggregateRepository);
		
		IPackageRepository CreateAggregateRepository(IEnumerable<IPackageRepository> repositories);
	}
}
