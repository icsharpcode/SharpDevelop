// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageRepositoryFactory : IPackageRepositoryCache
	{
		public List<PackageSource> PackageSourcesPassedToCreateRepository
			= new List<PackageSource>();
		
		public PackageSource FirstPackageSourcePassedToCreateRepository {
			get { return PackageSourcesPassedToCreateRepository[0]; }
		}
		
		public FakePackageRepository FakePackageRepository = new FakePackageRepository();
		
		public Dictionary<PackageSource, FakePackageRepository> FakePackageRepositories =
			new Dictionary<PackageSource, FakePackageRepository>();
	
		public IPackageRepository CreateRepository(PackageSource packageSource)
		{
			PackageSourcesPassedToCreateRepository.Add(packageSource);
			
			FakePackageRepository repository = null;
			if (FakePackageRepositories.TryGetValue(packageSource, out repository)) {
				return repository;
			}
			
			return FakePackageRepository;
		}
		
		public ISharedPackageRepository CreateSharedRepository(IPackagePathResolver pathResolver, IFileSystem fileSystem)
		{
			return new FakeSharedPackageRepository(pathResolver, fileSystem);
		}
		
		public FakePackageRepository FakeAggregateRepository = new FakePackageRepository();
		
		public IPackageRepository CreateAggregateRepository()
		{
			return FakeAggregateRepository;
		}
		
		public FakeRecentPackageRepository FakeRecentPackageRepository = new FakeRecentPackageRepository();
		public IList<RecentPackageInfo> RecentPackagesPassedToCreateRecentPackageRepository;
		public IPackageRepository AggregateRepositoryPassedToCreateRecentPackageRepository;
		
		public IRecentPackageRepository CreateRecentPackageRepository(
			IList<RecentPackageInfo> recentPackages,
			IPackageRepository aggregateRepository)
		{
			RecentPackagesPassedToCreateRecentPackageRepository = recentPackages;
			AggregateRepositoryPassedToCreateRecentPackageRepository = aggregateRepository;
			return FakeRecentPackageRepository;
		}
		
		public IEnumerable<IPackageRepository> RepositoriesPassedToCreateAggregateRepository;
		
		public IPackageRepository CreateAggregateRepository(IEnumerable<IPackageRepository> repositories)
		{
			RepositoriesPassedToCreateAggregateRepository = repositories;
			return FakeAggregateRepository;
		}
		
		public FakePackageRepository AddFakePackageRepositoryForPackageSource(string source)
		{
			var packageSource = new PackageSource(source);
			var repository = new FakePackageRepository();			
			FakePackageRepositories.Add(packageSource, repository);
			return repository;
		}
		
		public IRecentPackageRepository RecentPackageRepository {
			get { return FakeRecentPackageRepository; }
		}
	}
}
