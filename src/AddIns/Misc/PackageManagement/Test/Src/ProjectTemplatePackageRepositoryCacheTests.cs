// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class ProjectTemplatePackageRepositoryCacheTests
	{
		ProjectTemplatePackageRepositoryCache cache;
		FakePackageRepositoryFactory fakeMainCache;
		RegisteredProjectTemplatePackageSources registeredPackageSources;
		FakeSettingsFactory fakeSettingsFactory;
		
		void CreateCache()
		{
			fakeMainCache = new FakePackageRepositoryFactory();
			var propertyService = new FakePropertyService();
			fakeSettingsFactory = new FakeSettingsFactory();
			registeredPackageSources = new RegisteredProjectTemplatePackageSources(propertyService, fakeSettingsFactory);
			cache = new ProjectTemplatePackageRepositoryCache(fakeMainCache, registeredPackageSources);
		}
		
		void ClearRegisteredPackageSources()
		{
			registeredPackageSources.PackageSources.Clear();
		}
		
		void AddRegisteredPackageSource(PackageSource packageSource)
		{
			registeredPackageSources.PackageSources.Add(packageSource);
		}
		
		void AddRegisteredPackageSource(string url, string name)
		{
			var packageSource = new PackageSource(url, name);
			AddRegisteredPackageSource(packageSource);
		}
		
		FakePackageRepository AddRegisteredPackageRepository(string packageSourceUrl, string packageSourceName)
		{
			var packageSource = new PackageSource(packageSourceUrl, packageSourceName);
			AddRegisteredPackageSource(packageSource);
			FakePackageRepository fakeRepository = new FakePackageRepository();
			fakeMainCache.FakePackageRepositories.Add(packageSource.Source, fakeRepository);
			return fakeRepository;
		}
		
		[Test]
		public void CreateAggregateRepository_OneRegisteredPackageSource_CreatesAggregrateRepositoryUsingMainCache()
		{
			CreateCache();
			ClearRegisteredPackageSources();
			AddRegisteredPackageSource("http://sharpdevelop.com", "Test");
			
			IPackageRepository repository = cache.CreateAggregateRepository();
			
			IPackageRepository expectedRepository = fakeMainCache.FakeAggregateRepository;
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void CreateAggregateRepository_TwoRegisteredPackageSources_CreatesRepositoriesForRegisteredPackageSources()
		{
			CreateCache();
			ClearRegisteredPackageSources();
			FakePackageRepository fakeRepository1 = AddRegisteredPackageRepository("http://sharpdevelop.com", "Test");
			FakePackageRepository fakeRepository2 = AddRegisteredPackageRepository("http://test", "Test2");
			
			IPackageRepository repository = cache.CreateAggregateRepository();
			
			IEnumerable<IPackageRepository> repositories = fakeMainCache.RepositoriesPassedToCreateAggregateRepository;
			var expectedRepositories = new List<IPackageRepository>();
			expectedRepositories.Add(fakeRepository1);
			expectedRepositories.Add(fakeRepository2);
			
			Assert.AreEqual(expectedRepositories, repositories);
		}
		
		[Test]
		public void CreateAggregatePackageRepository_TwoRegisteredPackageSourcesButOneDisabled_ReturnsAggregateRepositoryCreatedWithOnlyEnabledPackageSource()
		{
			CreateCache();
			ClearRegisteredPackageSources();
			FakePackageRepository fakeRepository1 = AddRegisteredPackageRepository("http://sharpdevelop.com", "Test");
			FakePackageRepository fakeRepository2 = AddRegisteredPackageRepository("http://test", "Test2");
			registeredPackageSources.PackageSources[0].IsEnabled = false;
			
			IPackageRepository repository = cache.CreateAggregateRepository();
			
			IEnumerable<IPackageRepository> repositories = fakeMainCache.RepositoriesPassedToCreateAggregateRepository;
			var expectedRepositories = new List<IPackageRepository>();
			expectedRepositories.Add(fakeRepository2);
			
			Assert.AreEqual(expectedRepositories, repositories);
		}
	}
}
