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
		public List<string> PackageSourcesPassedToCreateRepository
			= new List<string>();
		
		public string FirstPackageSourcePassedToCreateRepository {
			get { return PackageSourcesPassedToCreateRepository[0]; }
		}
		
		public FakePackageRepository FakePackageRepository = new FakePackageRepository();
		
		public Dictionary<string, FakePackageRepository> FakePackageRepositories =
			new Dictionary<string, FakePackageRepository>();
	
		public IPackageRepository CreateRepository(string packageSource)
		{
			PackageSourcesPassedToCreateRepository.Add(packageSource);
			
			FakePackageRepository repository = null;
			if (FakePackageRepositories.TryGetValue(packageSource, out repository)) {
				return repository;
			}
			
			return FakePackageRepository;
		}
		
		public IPackagePathResolver PathResolverPassedToCreateSharedRepository;
		public IFileSystem FileSystemPassedToCreateSharedRepository;
		public IFileSystem ConfigSettingsFileSystemPassedToCreateSharedRepository;
		public FakeSharedPackageRepository FakeSharedRepository = new FakeSharedPackageRepository();
		
		public ISharedPackageRepository CreateSharedRepository(IPackagePathResolver pathResolver, IFileSystem fileSystem, IFileSystem configSettingsFileSystem)
		{
			PathResolverPassedToCreateSharedRepository = pathResolver;
			FileSystemPassedToCreateSharedRepository = fileSystem;
			ConfigSettingsFileSystemPassedToCreateSharedRepository = configSettingsFileSystem;
			return FakeSharedRepository;
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
			var repository = new FakePackageRepository();			
			FakePackageRepositories.Add(source, repository);
			return repository;
		}
		
		public IRecentPackageRepository RecentPackageRepository {
			get { return FakeRecentPackageRepository; }
		}
	}
}
