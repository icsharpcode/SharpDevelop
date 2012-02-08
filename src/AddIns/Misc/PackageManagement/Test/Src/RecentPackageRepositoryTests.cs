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
		FakePackageRepository aggregateRepository;
		PackageManagementEvents packageManagementEvents;
		List<RecentPackageInfo> recentPackages;
		
		void CreateRepository()
		{
			CreateRecentPackages();
			CreateRepository(recentPackages);
		}
		
		void CreateRecentPackages()
		{
			recentPackages =  new List<RecentPackageInfo>();
			aggregateRepository = new FakePackageRepository();
		}
		
		void CreateRepository(IList<RecentPackageInfo> recentPackages)
		{
			packageManagementEvents = new PackageManagementEvents();
			repository = new RecentPackageRepository(recentPackages, aggregateRepository, packageManagementEvents);
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
			CreateRecentPackages();
			var package = new FakePackage("Test");
			aggregateRepository.FakePackages.Add(package);
			recentPackages.Add(new RecentPackageInfo(package));
			CreateRepository(recentPackages);
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
			package2.Version = new SemanticVersion(2, 0, 0, 0);
			aggregateRepository.FakePackages.Add(package2);
			
			var recentPackages = repository.GetPackages();
			
			var expectedPackages = new FakePackage[] {
				package1
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, recentPackages);
		}
		
		[Test]
		public void ParentPackageInstalled_EventFires_PackageAddedToRecentPackages()
		{
			CreateRepository();
			
			var package = new FakePackage("Test");
			packageManagementEvents.OnParentPackageInstalled(package);
			
			var recentPackages = repository.GetPackages();
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, recentPackages);
		}

		public void Clear_OneRecentPackage_PackagesRemoved()
		{
			CreateRepository();
			AddOnePackageToRepository("Test1");
			
			repository.Clear();
			
			int count = repository.GetPackages().Count();
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void Clear_OneRecentPackageInOptions_RecentPackagesAreRemovedFromOptions()
		{
			CreateRepositoryWithOneRecentPackageSavedInOptions();
			
			repository.Clear();
			
			int count = recentPackages.Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void HasRecentPackages_NoSavedRecentPackages_ReturnsFalse()
		{
			CreateRepository();
			
			bool hasRecentPackages = repository.HasRecentPackages;
			
			Assert.IsFalse(hasRecentPackages);
		}
		
		[Test]
		public void HasRecentPackages_OneSavedRecentPackages_ReturnsTrue()
		{
			CreateRepositoryWithOneRecentPackageSavedInOptions();
			
			bool hasRecentPackages = repository.HasRecentPackages;
			
			Assert.IsTrue(hasRecentPackages);
		}
	}
}
