// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class RegisteredPackageRepositoriesTests
	{
		RegisteredPackageRepositories registeredRepositories;
		OneRegisteredPackageSourceHelper packageSourcesHelper;
		FakePackageRepositoryFactory fakeRepositoryCache;
		
		void CreateRegisteredPackageRepositories()
		{
			CreatePackageSourcesHelper();
			CreateRegisteredPackageRepositoriesWithExistingPackageSourcesHelper();
		}
		
		void CreatePackageSourcesHelper()
		{
			packageSourcesHelper = new OneRegisteredPackageSourceHelper();
		}
		
		void CreateRegisteredPackageRepositoriesWithExistingPackageSourcesHelper()
		{
			fakeRepositoryCache = new FakePackageRepositoryFactory();
			registeredRepositories = new RegisteredPackageRepositories(fakeRepositoryCache, packageSourcesHelper.Options);	
		}
		
		[Test]
		public void RecentPackageRepository_PropertyAccessed_ReturnsRecentPackageRepositoryFromCache()
		{
			CreateRegisteredPackageRepositories();
			var recentRepository = registeredRepositories.RecentPackageRepository;
			var expectedRepository = fakeRepositoryCache.FakeRecentPackageRepository;
			
			Assert.AreEqual(expectedRepository, recentRepository);
		}
		
		[Test]
		public void CreateRepository_PackageSourceSpecified_CreatesRepositoryFromCache()
		{
			CreateRegisteredPackageRepositories();
			var repository = registeredRepositories.CreateRepository(new PackageSource("a"));
			var expectedRepository = fakeRepositoryCache.FakePackageRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void CreateRepository_PackageSourceSpecified_PackageSourcePassedToCache()
		{
			CreateRegisteredPackageRepositories();
			var source = new PackageSource("Test");
			var repository = registeredRepositories.CreateRepository(source);
			var actualSource = fakeRepositoryCache.FirstPackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(source, actualSource);
		}
		
		[Test]
		public void CreateAggregateRepository_MethodCalled_ReturnsAggregateRepositoryCreatedFromCache()
		{
			CreateRegisteredPackageRepositories();
			var repository = registeredRepositories.CreateAggregateRepository();
			var expectedRepository = fakeRepositoryCache.FakeAggregateRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void HasMultiplePackageSources_OnePackageSource_ReturnsFalse()
		{
			CreateRegisteredPackageRepositories();
			packageSourcesHelper.AddOnePackageSource();
			bool result = registeredRepositories.HasMultiplePackageSources;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void HasMultiplePackageSources_TwoPackageSources_ReturnsTrue()
		{
			CreateRegisteredPackageRepositories();
			packageSourcesHelper.AddTwoPackageSources();
			bool result = registeredRepositories.HasMultiplePackageSources;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ActivePackageSource_TwoPackageSources_ByDefaultReturnsFirstPackageSource()
		{
			CreateRegisteredPackageRepositories();
			packageSourcesHelper.AddTwoPackageSources();
			
			var expectedPackageSource = packageSourcesHelper.RegisteredPackageSources[0];
			var packageSource = registeredRepositories.ActivePackageSource;
			
			Assert.AreEqual(expectedPackageSource, packageSource);
		}
		
		[Test]
		public void ActivePackageSource_ChangedToSecondRegisteredPackageSources_ReturnsSecondPackageSource()
		{
			CreateRegisteredPackageRepositories();
			packageSourcesHelper.AddTwoPackageSources();
			
			var expectedPackageSource = packageSourcesHelper.RegisteredPackageSources[1];
			registeredRepositories.ActivePackageSource = expectedPackageSource;
			var packageSource = registeredRepositories.ActivePackageSource;
			
			Assert.AreEqual(expectedPackageSource, packageSource);
		}
		
		[Test]
		public void ActivePackageSource_ChangedToNonNullPackageSource_SavedInOptions()
		{
			CreateRegisteredPackageRepositories();
			packageSourcesHelper.Options.ActivePackageSource = null;
			var packageSource = new PackageSource("http://source-url", "Test");
			packageSourcesHelper.Options.PackageSources.Add(packageSource);
			
			registeredRepositories.ActivePackageSource = packageSource;
			
			var actualPackageSource = packageSourcesHelper.Options.ActivePackageSource;
			
			Assert.AreEqual(packageSource, actualPackageSource);
		}
		
		[Test]
		public void ActivePackageSource_ActivePackageSourceNonNullInOptionsBeforeInstanceCreated_ActivePackageSourceReadFromOptions()
		{
			CreatePackageSourcesHelper();
			var packageSource = new PackageSource("http://source-url", "Test");
			packageSourcesHelper.Options.PackageSources.Add(packageSource);
			packageSourcesHelper.Options.ActivePackageSource = packageSource;
			CreateRegisteredPackageRepositoriesWithExistingPackageSourcesHelper();
			
			var actualPackageSource = registeredRepositories.ActivePackageSource;
			
			Assert.AreEqual(packageSource, actualPackageSource);
		}
		
		[Test]
		public void ActiveRepository_OneRegisteredSource_RepositoryCreatedFromRegisteredSource()
		{
			CreateRegisteredPackageRepositories();
			IPackageRepository activeRepository = registeredRepositories.ActiveRepository;
			
			var actualPackageSource = fakeRepositoryCache.FirstPackageSourcePassedToCreateRepository;
			var expectedPackageSource = packageSourcesHelper.PackageSource;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void ActiveRepository_CalledTwice_RepositoryCreatedOnce()
		{
			CreateRegisteredPackageRepositories();
			IPackageRepository activeRepository = registeredRepositories.ActiveRepository;
			activeRepository = registeredRepositories.ActiveRepository;
			
			int count = fakeRepositoryCache.PackageSourcesPassedToCreateRepository.Count;
			
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void ActiveRepository_OneRegisteredSource_ReturnsPackageCreatedFromCache()
		{
			CreateRegisteredPackageRepositories();
			IPackageRepository activeRepository = registeredRepositories.ActiveRepository;
			
			IPackageRepository expectedRepository = fakeRepositoryCache.FakePackageRepository;
			
			Assert.AreEqual(expectedRepository, activeRepository);
		}
		
		[Test]
		public void ActivePackageRepository_ActivePackageSourceChangedToSecondRegisteredPackageSource_CreatesRepositoryUsingSecondPackageSource()
		{
			CreateRegisteredPackageRepositories();
			packageSourcesHelper.AddTwoPackageSources();
			
			var expectedPackageSource = packageSourcesHelper.Options.PackageSources[1];
			registeredRepositories.ActivePackageSource = expectedPackageSource;
			
			IPackageRepository repository = registeredRepositories.ActiveRepository;
			var packageSource = fakeRepositoryCache.FirstPackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(expectedPackageSource, packageSource);
		}
		
		[Test]
		public void ActiveRepository_ActivePackageSourceChangedAfterActivePackageRepositoryCreated_CreatesNewRepositoryUsingActivePackageSource()
		{
			CreateRegisteredPackageRepositories();
			packageSourcesHelper.AddTwoPackageSources();
			
			IPackageRepository initialRepository = registeredRepositories.ActiveRepository;
			fakeRepositoryCache.PackageSourcesPassedToCreateRepository.Clear();
			
			var expectedPackageSource = packageSourcesHelper.Options.PackageSources[1];
			registeredRepositories.ActivePackageSource = expectedPackageSource;
			
			IPackageRepository repository = registeredRepositories.ActiveRepository;
			var packageSource = fakeRepositoryCache.FirstPackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(expectedPackageSource, packageSource);
		}
		
		[Test]
		public void ActiveRepository_ActivePackageSourceSetToSameValueAfterActivePackageRepositoryCreated_NewRepositoryNotCreated()
		{
			CreateRegisteredPackageRepositories();
			packageSourcesHelper.AddOnePackageSource();
			
			IPackageRepository initialRepository = registeredRepositories.ActiveRepository;
			fakeRepositoryCache.PackageSourcesPassedToCreateRepository.Clear();
			
			var expectedPackageSource = packageSourcesHelper.Options.PackageSources[0];
			registeredRepositories.ActivePackageSource = expectedPackageSource;
			
			IPackageRepository repository = registeredRepositories.ActiveRepository;
			
			int count = fakeRepositoryCache.PackageSourcesPassedToCreateRepository.Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void PackageSources_OnePackageSourceInOptions_ReturnsOnePackageSource()
		{
			CreateRegisteredPackageRepositories();
			packageSourcesHelper.AddOnePackageSource();
			
			var packageSources = registeredRepositories.PackageSources;
			var expectedPackageSources = packageSourcesHelper.Options.PackageSources;
			
			Assert.AreEqual(expectedPackageSources, packageSources);
		}
		
		[Test]
		public void ActivePackageRepository_ActivePackageSourceIsAggregate_ReturnsAggregatePackageRepository()
		{
			CreateRegisteredPackageRepositories();
			packageSourcesHelper.AddTwoPackageSources();
			
			registeredRepositories.ActivePackageSource = RegisteredPackageSourceSettings.AggregatePackageSource;
						
			var repository = registeredRepositories.ActiveRepository;
			var expectedRepository = fakeRepositoryCache.FakeAggregateRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
	}
}

