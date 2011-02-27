// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
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
		
		void CreateRepository()
		{
			repository = new RecentPackageRepository();
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
	}
}
