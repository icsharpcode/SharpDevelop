// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO.Packaging;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageRepositoryCacheTests
	{
		PackageRepositoryCache cache;
		FakePackageRepositoryFactory fakePackageRepositoryFactory;
		PackageSource nuGetPackageSource;
		OneRegisteredPackageSourceHelper packageSourcesHelper;
		RecentPackageInfo[] recentPackagesPassedToCreateRecentPackageRepository;
		FakePackageRepository fakeAggregateRepositoryPassedToCreateRecentPackageRepository;

		void CreateCache()
		{
			CreatePackageSources();
			CreateCacheUsingPackageSources();
		}
		
		void CreatePackageSources()
		{
			packageSourcesHelper = new OneRegisteredPackageSourceHelper();
		}
		
		void CreateCacheUsingPackageSources()
		{
			nuGetPackageSource = new PackageSource("http://nuget.org", "NuGet");
			fakePackageRepositoryFactory = new FakePackageRepositoryFactory();
			var packageSources = packageSourcesHelper.Options.PackageSources;
			var recentPackages = packageSourcesHelper.Options.RecentPackages;
			cache = new PackageRepositoryCache(fakePackageRepositoryFactory, packageSources, recentPackages);
		}
		
		FakePackageRepository AddFakePackageRepositoryForPackageSource(string source)
		{
			return fakePackageRepositoryFactory.AddFakePackageRepositoryForPackageSource(source);
		}
		
		IPackageRepository CreateRecentPackageRepositoryPassingAggregateRepository()
		{
			recentPackagesPassedToCreateRecentPackageRepository = new RecentPackageInfo[0];
			fakeAggregateRepositoryPassedToCreateRecentPackageRepository = new FakePackageRepository();
			
			return cache.CreateRecentPackageRepository(
				recentPackagesPassedToCreateRecentPackageRepository,
				fakeAggregateRepositoryPassedToCreateRecentPackageRepository);
		}
		
		RecentPackageInfo AddOneRecentPackage()
		{
			var recentPackage = new RecentPackageInfo("Id", new Version("1.0"));
			packageSourcesHelper.Options.RecentPackages.Add(recentPackage);
			return recentPackage;
		}

		[Test]
		public void CreateRepository_CacheCastToISharpDevelopPackageRepositoryFactory_CreatesPackageRepositoryUsingPackageRepositoryFactoryPassedInConstructor()
		{
			CreateCache();
			var factory = cache as ISharpDevelopPackageRepositoryFactory;
			IPackageRepository repository = factory.CreateRepository(nuGetPackageSource);
			
			Assert.AreEqual(fakePackageRepositoryFactory.FakePackageRepository, repository);
		}
		
		[Test]
		public void CreateRepository_PackageSourcePassed_PackageSourceUsedToCreateRepository()
		{
			CreateCache();
			cache.CreateRepository(nuGetPackageSource);
			
			var actualPackageSource = fakePackageRepositoryFactory.FirstPackageSourcePassedToCreateRepository;
			Assert.AreEqual(nuGetPackageSource, actualPackageSource);
		}
		
		[Test]
		public void CreateRepository_RepositoryAlreadyCreatedForPackageSource_NoRepositoryCreated()
		{
			CreateCache();
			cache.CreateRepository(nuGetPackageSource);
			fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.Clear();
			
			cache.CreateRepository(nuGetPackageSource);
			
			Assert.AreEqual(0, fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.Count);
		}
		
		[Test]
		public void CreateRepository_RepositoryAlreadyCreatedForPackageSource_RepositoryOriginallyCreatedIsReturned()
		{
			CreateCache();
			IPackageRepository originallyCreatedRepository = cache.CreateRepository(nuGetPackageSource);
			fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.Clear();
			
			IPackageRepository repository = cache.CreateRepository(nuGetPackageSource);
			
			Assert.AreSame(originallyCreatedRepository, repository);
		}
		
		[Test]
		public void CreatedSharedRepository_PathResolverPassed_PathResolverUsedToCreatedSharedRepository()
		{
			CreateCache();
			FakePackagePathResolver resolver = new FakePackagePathResolver();
			FakeSharedPackageRepository repository = cache.CreateSharedRepository(resolver, null) as FakeSharedPackageRepository;
			
			Assert.AreEqual(resolver, repository.PackagePathResolverPassedToConstructor);
		}
		
		[Test]
		public void CreatedSharedRepository_FileSystemPassed_FileSystemUsedToCreatedSharedRepository()
		{
			CreateCache();
			FakeFileSystem fileSystem = new FakeFileSystem();
			FakeSharedPackageRepository repository = cache.CreateSharedRepository(null, fileSystem) as FakeSharedPackageRepository;
			
			Assert.AreEqual(fileSystem, repository.FileSystemPassedToConstructor);
		}
		
		[Test]
		public void CreateAggregatePackageRepository_TwoRegisteredPackageRepositories_ReturnsAggregateRepositoryFromFactory()
		{
			CreatePackageSources();
			packageSourcesHelper.AddTwoPackageSources("Source1", "Source2");
			CreateCacheUsingPackageSources();
			
			var aggregateRepository = cache.CreateAggregateRepository();
			var expectedRepository = fakePackageRepositoryFactory.FakeAggregateRepository;
			
			Assert.AreEqual(expectedRepository, aggregateRepository);
		}
		
		[Test]
		public void CreateAggregatePackageRepository_TwoRegisteredPackageRepositories_AllRegisteredRepositoriesUsedToCreateAggregateRepositoryFromFactory()
		{
			CreatePackageSources();
			packageSourcesHelper.AddTwoPackageSources("Source1", "Source2");
			CreateCacheUsingPackageSources();
			
			var repository1 = AddFakePackageRepositoryForPackageSource("Source1");
			var repository2 = AddFakePackageRepositoryForPackageSource("Source2");
			var expectedRepositories = new FakePackageRepository[] {
				repository1,
				repository2
			};
			
			cache.CreateAggregateRepository();
			
			var repositoriesUsedToCreateAggregateRepository = 
				fakePackageRepositoryFactory.RepositoriesPassedToCreateAggregateRepository;
			
			var actualRepositoriesAsList = new List<IPackageRepository>(repositoriesUsedToCreateAggregateRepository);
			var actualRepositories = actualRepositoriesAsList.ToArray();
			
			CollectionAssert.AreEqual(expectedRepositories, actualRepositories);
		}
		
		[Test]
		public void CreateAggregatePackageRepository_OnePackageRepositoryPassed_ReturnsAggregateRepositoryFromFactory()
		{
			CreateCache();
			
			var repositories = new FakePackageRepository[] {
				new FakePackageRepository()
			};
			var aggregateRepository = cache.CreateAggregateRepository(repositories);
			
			var expectedRepository = fakePackageRepositoryFactory.FakeAggregateRepository;
			
			Assert.AreEqual(expectedRepository, aggregateRepository);
		}
		
		[Test]
		public void CreateAggregatePackageRepository_OnePackageRepositoryPassed_RepositoryUsedToCreateAggregateRepository()
		{
			CreateCache();
			
			var repositories = new FakePackageRepository[] {
				new FakePackageRepository()
			};
			cache.CreateAggregateRepository(repositories);
			
			var repositoriesUsedToCreateAggregateRepository = 
				fakePackageRepositoryFactory.RepositoriesPassedToCreateAggregateRepository;
			
			Assert.AreEqual(repositories, repositoriesUsedToCreateAggregateRepository);
		}
		
		[Test]
		public void RecentPackageRepository_NoRecentPackages_ReturnsRecentRepositoryCreatedByFactory()
		{
			CreateCache();
			var repository = cache.RecentPackageRepository;
			var expectedRepository = fakePackageRepositoryFactory.FakeRecentPackageRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void RecentPackageRepository_NoRecentPackages_CreatedWithAggregateRepository()
		{
			CreateCache();
			var repository = cache.RecentPackageRepository;
			
			var expectedRepository = fakePackageRepositoryFactory.FakeAggregateRepository;
			var actualRepository = fakePackageRepositoryFactory.AggregateRepositoryPassedToCreateRecentPackageRepository;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void RecentPackageRepository_OneRecentPackage_RecentPackageUsedToCreateRecentPackageRepository()
		{
			CreateCache();
			var recentPackage = AddOneRecentPackage();
			
			var repository = cache.RecentPackageRepository;
			
			var actualRecentPackages = fakePackageRepositoryFactory.RecentPackagesPassedToCreateRecentPackageRepository;
			
			var expectedRecentPackages = new RecentPackageInfo[] {
				recentPackage
			};
			
			Assert.AreEqual(expectedRecentPackages, actualRecentPackages);
		}
		
		[Test]
		public void RecentPackageRepository_OnePackageSource_OneRepositoryCreatedForPackageSourceAndUsedToCreateAggregateRepository()
		{
			CreatePackageSources();
			packageSourcesHelper.AddOnePackageSource("Source1");
			CreateCacheUsingPackageSources();
			
			var repository = AddFakePackageRepositoryForPackageSource("Source1");
			var expectedRepositories = new FakePackageRepository[] {
				repository
			};
			
			var recentRepository = cache.RecentPackageRepository;
			
			var repositoriesUsedToCreateAggregateRepository = 
				fakePackageRepositoryFactory.RepositoriesPassedToCreateAggregateRepository;
			
			var actualRepositoriesAsList = new List<IPackageRepository>(repositoriesUsedToCreateAggregateRepository);
			var actualRepositories = actualRepositoriesAsList.ToArray();
			
			CollectionAssert.AreEqual(expectedRepositories, actualRepositories);
		}
		
		[Test]
		public void RecentPackageRepository_PropertyAccessedTwice_AggregateRepositoryCreatedOnce()
		{
			CreateCache();
			var repository = cache.RecentPackageRepository;
			fakePackageRepositoryFactory.RepositoriesPassedToCreateAggregateRepository = null;
			repository = cache.RecentPackageRepository;
			
			Assert.IsNull(fakePackageRepositoryFactory.RepositoriesPassedToCreateAggregateRepository);
		}
			
		[Test]
		public void CreateRecentPackageRepository_AggregateRepositoryPassedAndNoRecentPackagesPassed_UsesFactoryToCreateRepository()
		{
			CreateCache();
			var repository = CreateRecentPackageRepositoryPassingAggregateRepository();
			
			var expectedRepository = fakePackageRepositoryFactory.FakeRecentPackageRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void CreateRecentPackageRepository_AggregateRepositoryPassedAndNoRecentPackagesPassed_AggregateIsUsedToCreateRepository()
		{
			CreateCache();
			CreateRecentPackageRepositoryPassingAggregateRepository();
			
			var actualRepository = fakePackageRepositoryFactory.AggregateRepositoryPassedToCreateRecentPackageRepository;
			
			Assert.AreEqual(fakeAggregateRepositoryPassedToCreateRecentPackageRepository, actualRepository);
		}
		
		[Test]
		public void CreateRecentPackageRepository_AggregateRepositoryPassedAndNoRecentPackagesPassed_RecentPackagesUsedToCreateRepository()
		{
			CreateCache();
			CreateRecentPackageRepositoryPassingAggregateRepository();
			
			var recentPackages = fakePackageRepositoryFactory.RecentPackagesPassedToCreateRecentPackageRepository;
			
			Assert.AreEqual(recentPackagesPassedToCreateRecentPackageRepository, recentPackages);
		}
		
		[Test]
		public void CreateRecentPackageRepository_MethodCalledTwice_RecentPackageRepositoryCreatedOnce()
		{
			CreateCache();
			CreateRecentPackageRepositoryPassingAggregateRepository();
			fakePackageRepositoryFactory.AggregateRepositoryPassedToCreateRecentPackageRepository = null;
			CreateRecentPackageRepositoryPassingAggregateRepository();
			
			Assert.IsNull(fakePackageRepositoryFactory.AggregateRepositoryPassedToCreateRecentPackageRepository);
		}
	}
}
