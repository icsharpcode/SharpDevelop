// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageRepositoryCache : IPackageRepositoryCache
	{
		ISharpDevelopPackageRepositoryFactory factory;
		Dictionary<PackageSource, IPackageRepository> repositories =
			new Dictionary<PackageSource, IPackageRepository>();
		
		public PackageRepositoryCache(ISharpDevelopPackageRepositoryFactory factory)
		{
			this.factory = factory;
		}
		
		public PackageRepositoryCache()
			: this(new SharpDevelopPackageRepositoryFactory())
		{
		}
		
		public IPackageRepository CreateRepository(PackageSource packageSource)
		{
			IPackageRepository repository = GetExistingRepository(packageSource);
			if (repository != null) {
				return repository;
			}
			return CreateNewCachedRepository(packageSource);
		}
		
		IPackageRepository GetExistingRepository(PackageSource packageSource)
		{
			IPackageRepository repository = null;
			if (repositories.TryGetValue(packageSource, out repository)) {
				return repository;
			}
			return null;
		}
		
		IPackageRepository CreateNewCachedRepository(PackageSource packageSource)
		{
			IPackageRepository repository = factory.CreateRepository(packageSource);
			repositories.Add(packageSource, repository);
			return repository;
		}
		
		public ISharedPackageRepository CreateSharedRepository(IPackagePathResolver pathResolver, IFileSystem fileSystem)
		{
			return factory.CreateSharedRepository(pathResolver, fileSystem);
		}
	}
}
