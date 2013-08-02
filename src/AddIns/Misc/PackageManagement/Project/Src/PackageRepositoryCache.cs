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
		ConcurrentDictionary<string, IPackageRepository> repositories =
			new ConcurrentDictionary<string, IPackageRepository>();
		
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
		
		public IPackageRepository CreateRepository(string packageSource)
		{
			IPackageRepository repository = GetExistingRepository(packageSource);
			if (repository != null) {
				return repository;
			}
			return CreateNewCachedRepository(packageSource);
		}
		
		IPackageRepository GetExistingRepository(string packageSource)
		{
			IPackageRepository repository = null;
			if (repositories.TryGetValue(packageSource, out repository)) {
				return repository;
			}
			return null;
		}
		
		IPackageRepository CreateNewCachedRepository(string packageSource)
		{
			IPackageRepository repository = factory.CreateRepository(packageSource);
			repositories.TryAdd(packageSource, repository);
			return repository;
		}
		
		public ISharedPackageRepository CreateSharedRepository(IPackagePathResolver pathResolver, IFileSystem fileSystem, IFileSystem configSettingsFileSystem)
		{
			return factory.CreateSharedRepository(pathResolver, fileSystem, configSettingsFileSystem);
		}
		
		public IPackageRepository CreateAggregateRepository()
		{
			IEnumerable<IPackageRepository> allRepositories = CreateAllEnabledRepositories();
			return CreateAggregateRepository(allRepositories);
		}
		
		IEnumerable<IPackageRepository> CreateAllEnabledRepositories()
		{
			foreach (PackageSource source in registeredPackageSources.GetEnabledPackageSources()) {
				yield return CreateRepository(source.Source);
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
