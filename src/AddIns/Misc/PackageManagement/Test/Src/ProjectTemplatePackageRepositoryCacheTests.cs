// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
