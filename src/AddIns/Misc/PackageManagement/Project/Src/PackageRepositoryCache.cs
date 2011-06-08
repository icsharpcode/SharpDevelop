// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageRepositoryCache : IPackageRepositoryCache
	{
		ISharpDevelopPackageRepositoryFactory factory;
		RegisteredPackageSources registeredPackageSources;
		IList<RecentPackageInfo> recentPackages;
		IRecentPackageRepository recentPackageRepository;
		ConcurrentDictionary<PackageSource, IPackageRepository> repositories =
			new ConcurrentDictionary<PackageSource, IPackageRepository>();
		
		public PackageRepositoryCache(
			ISharpDevelopPackageRepositoryFactory factory,
			RegisteredPackageSources registeredPackageSources,
			IList<RecentPackageInfo> recentPackages)
		{
			this.factory = factory;
			this.registeredPackageSources = registeredPackageSources;
			this.recentPackages = recentPackages;
		}
		
		public PackageRepositoryCache(
			RegisteredPackageSources registeredPackageSources,
			IList<RecentPackageInfo> recentPackages)
			: this(
				new SharpDevelopPackageRepositoryFactory(),
				registeredPackageSources,
				recentPackages)
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
			repositories.TryAdd(packageSource, repository);
			return repository;
		}
		
		public ISharedPackageRepository CreateSharedRepository(IPackagePathResolver pathResolver, IFileSystem fileSystem)
		{
			return factory.CreateSharedRepository(pathResolver, fileSystem);
		}
		
		public IPackageRepository CreateAggregateRepository()
		{
			IEnumerable<IPackageRepository> allRepositories = CreateAllRepositories();
			return CreateAggregateRepository(allRepositories);
		}
		
		IEnumerable<IPackageRepository> CreateAllRepositories()
		{
			foreach (PackageSource source in registeredPackageSources) {
				yield return CreateRepository(source);
			}
		}
		
		public IPackageRepository CreateAggregateRepository(IEnumerable<IPackageRepository> repositories)
		{
			return factory.CreateAggregateRepository(repositories);
		}
		
		public IRecentPackageRepository RecentPackageRepository {
			get {
				CreateRecentPackageRepository();
				return recentPackageRepository;
			}
		}
		
		void CreateRecentPackageRepository()
		{
			if (recentPackageRepository == null) {
				IPackageRepository aggregateRepository = CreateAggregateRepository();
				CreateRecentPackageRepository(recentPackages, aggregateRepository);
			}
		}
		
		public IRecentPackageRepository CreateRecentPackageRepository(
			IList<RecentPackageInfo> recentPackages,
			IPackageRepository aggregateRepository)
		{
			if (recentPackageRepository == null) {
				recentPackageRepository = factory.CreateRecentPackageRepository(recentPackages, aggregateRepository);
			}
			return recentPackageRepository;
		}
	}
}
