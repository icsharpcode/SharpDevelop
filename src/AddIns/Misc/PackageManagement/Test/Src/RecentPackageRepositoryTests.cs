// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class RecentPackageRepositoryTests
	{
		RecentPackageRepository repository;
		PackageManagementOptions options;
		FakePackageRepository aggregateRepository;
		FakePackageManagementService packageManagementService;
		
		void CreateRepository()
		{
			CreatePackageManagementService();
			CreateRepository(packageManagementService);
		}
		
		void CreatePackageManagementService()
		{
			packageManagementService = new FakePackageManagementService();
			options = packageManagementService.Options;
			aggregateRepository = packageManagementService.FakeAggregateRepository;
		}
		
		void CreateRepository(IPackageManagementService packageManagementService)
		{
			repository = new RecentPackageRepository(packageManagementService);
		}
		
		FakePackage AddOnePackageToRepository(string id)
		{
			var package = new FakePackage(id);
			repository.AddPackage(package);
			return package;
		}
		
		IEnumerable<IPackage> AddTwoDifferentPackagesToRepository()
		{
			yield return AddOnePackageToRepository("Test.Package.1");
			yield return AddOnePackageToRepository("Test.Package.2");
		}

		IEnumerable<IPackage> AddFourDifferentPackagesToRepository()
		{
			yield return AddOnePackageToRepository("Test.Package.1");
			yield return AddOnePackageToRepository("Test.Package.2");
			yield return AddOnePackageToRepository("Test.Package.3");
			yield return AddOnePackageToRepository("Test.Package.4");
		}
		
		FakePackage CreateRepositoryWithOneRecentPackageSavedInOptions()
		{
			CreatePackageManagementService();
			var package = new FakePackage("Test");
			aggregateRepository.FakePackages.Add(package);
			options.RecentPackages.Add(new RecentPackageInfo(package));
			CreateRepository(packageManagementService);
			return package;
		}
		
		[Test]
		public void Source_NewRecentRepositoryCreated_IsRecentPackages()
		{
			CreateRepository();
			Assert.AreEqual("RecentPackages", repository.Source);
		}
		
		[Test]
		public void GetPackages_RepositoryIsEmptyAndOnePackageAdded_ReturnsPackageAdded()
		{
			CreateRepository();
			var package = AddOnePackageToRepository("Test.Package");
			
			var packages = repository.GetPackages();
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, packages);
		}
		
		[Test]
		public void GetPackages_RepositoryIsEmptyAndTwoDifferentPackagesAdded_ReturnsPackagesInReverseOrderWithLastAddedFirst()
		{
			CreateRepository();
			var packagesAdded = AddTwoDifferentPackagesToRepository();
			
			var packages = repository.GetPackages();
			
			var expectedPackages = packagesAdded.Reverse();
			
			PackageCollectionAssert.AreEqual(expectedPackages, packages);
		}
		
		[Test]
		public void GetPackages_RepositoryCanHoldThreePackagesAndFourPackagesAdded_ReturnsLastThreePackagesAddedInReverseOrder()
		{
			CreateRepository();
			repository.MaximumPackagesCount = 3;
			var packagesAdded = AddFourDifferentPackagesToRepository();
			
			var packages = repository.GetPackages();
			
			var expectedPackages = packagesAdded.Reverse().Take(3);
			
			PackageCollectionAssert.AreEqual(expectedPackages, packages);
		}
		
		[Test]
		public void GetPackages_RepositoryIsEmptyAndSamePackageIsAddedTwice_OnePackageReturned()
		{
			CreateRepository();
			AddOnePackageToRepository("Test");
			var package = AddOnePackageToRepository("Test");
			
			var packages = repository.GetPackages();
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, packages);
		}
		
		[Test]
		public void AddPackage_RepositoryIsEmptyAndOnePackageAdded_RecentPackageAddedToOptions()
		{
			CreateRepository();
			var package = AddOnePackageToRepository("Test");
			
			var recentPackages = options.RecentPackages;
			
			var expectedPackages = new RecentPackageInfo[] {
				new RecentPackageInfo(package)
			};
			
			RecentPackageInfoCollectionAssert.AreEqual(expectedPackages, recentPackages);
		}
		
		[Test]
		public void AddPackage_RepositoryIsEmptyAndTwoPackagesAddedFromDifferentSources_BothRecentPackagesAddedToOptions()
		{
			CreateRepository();
			var package1 = AddOnePackageToRepository("Test1");
			var package2 = AddOnePackageToRepository("Test2");
			
			var recentPackages = options.RecentPackages;
			
			var expectedPackages = new RecentPackageInfo[] {
				new RecentPackageInfo(package2),
				new RecentPackageInfo(package1)
			};
			
			RecentPackageInfoCollectionAssert.AreEqual(expectedPackages, recentPackages);
		}
		
		[Test]
		public void GetPackages_SavedOptionsHasOneRecentPackage_ContainsPackageTakenFromAggregateRepositoryMatchingSavedRecentPackageInfo()
		{
			var package = CreateRepositoryWithOneRecentPackageSavedInOptions();
			
			var recentPackages = repository.GetPackages();
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, recentPackages);
		}
		
		[Test]
		public void GetPackages_SavedOptionsHasOneRecentPackageAndGetPackagesCalledTwice_OnePackageReturned()
		{
			var package = CreateRepositoryWithOneRecentPackageSavedInOptions();
			
			repository.GetPackages();
			var recentPackages = repository.GetPackages();
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, recentPackages);
		}
		
		[Test]
		public void GetPackages_OneRecentPackageAndAggregrateRepositoryHasTwoPackagesWithSameIdButDifferentVersions_OnePackageReturnedWithMatchingVersion()
		{
			var package1 = CreateRepositoryWithOneRecentPackageSavedInOptions();
			var package2 = new FakePackage(package1.Id);
			package2.Version = new Version(2, 0);
			aggregateRepository.FakePackages.Add(package2);
			
			var recentPackages = repository.GetPackages();
			
			var expectedPackages = new FakePackage[] {
				package1
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, recentPackages);
		}
	}
}
