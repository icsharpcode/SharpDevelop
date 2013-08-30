// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using ICSharpCode.PackageManagement;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class UpdatedPackagesTests
	{
		UpdatedPackages updatedPackages;
		IServiceBasedRepository sourceRepository;
		List<IPackage> installedPackages;
		List<IPackage> sourceRepositoryPackages;
		List<IPackage> packagesUsedWhenCheckingForUpdates;
		bool includePreleaseUsedWhenCheckingForUpdates;
		
		[SetUp]
		public void Init()
		{
			installedPackages = new List<IPackage>();
			sourceRepositoryPackages = new List<IPackage>();
			packagesUsedWhenCheckingForUpdates = new List<IPackage>();
			sourceRepository = MockRepository.GenerateStub<IServiceBasedRepository>();
		}
		
		void CreateUpdatedPackages()
		{
			sourceRepository
				.Stub(repository => repository.GetPackages())
				.Return(sourceRepositoryPackages.AsQueryable());
			
			sourceRepository
				.Stub(repository => repository.GetUpdates(
					Arg<IEnumerable<IPackage>>.Is.Anything,
					Arg<bool>.Is.Anything,
					Arg<bool>.Is.Anything,
					Arg<IEnumerable<FrameworkName>>.Is.Anything,
					Arg<IEnumerable<IVersionSpec>>.Is.Anything))
				.WhenCalled(call => {
					includePreleaseUsedWhenCheckingForUpdates = (bool)call.Arguments[1];
					packagesUsedWhenCheckingForUpdates.AddRange(call.Arguments[0] as IEnumerable<IPackage>);
				 })
				.Return(sourceRepositoryPackages.AsQueryable());
			
			updatedPackages = new UpdatedPackages(installedPackages.AsQueryable(), sourceRepository);
		}
		
		IPackage AddPackageToSourceRepository(string id, string version)
		{
			IPackage package = CreatePackage(id, version);
			sourceRepositoryPackages.Add(package);
			return package;
		}
		
		IPackage CreatePackage(string id, string version)
		{
			var helper = new TestPackageHelper(id, version);
			helper.IsLatestVersion();
			helper.Listed();
			return helper.Package;
		}
		
		IPackage AddInstalledPackage(string id, string version)
		{
			IPackage package = CreatePackage(id, version);
			installedPackages.Add(package);
			return package;
		}
		
		[Test]
		public void GetUpdatedPackages_OnePackageInstalledAndUpdateAvailable_UpdatedPackageReturned()
		{
			AddInstalledPackage("Test", "1.0");
			IPackage expectedPackage = AddPackageToSourceRepository("Test", "1.1");
			var expectedPackages = new IPackage[] { expectedPackage };
			CreateUpdatedPackages();
			
			IEnumerable<IPackage> packages = updatedPackages.GetUpdatedPackages();
			
			PackageCollectionAssert.AreEqual(expectedPackages, packages);
		}
		
		[Test]
		public void GetUpdatedPackages_OnePackageInstalledAndUpdateAvailable_InstalledPackageUsedToCheckIfSourceRepositoryHasAnyUpdates()
		{
			IPackage expectedPackage = AddInstalledPackage("Test", "1.0");
			var expectedPackages = new IPackage[] { expectedPackage };
			AddPackageToSourceRepository("Test", "1.1");
			CreateUpdatedPackages();
			
			IEnumerable<IPackage> packages = updatedPackages.GetUpdatedPackages();
			
			PackageCollectionAssert.AreEqual(expectedPackages, packagesUsedWhenCheckingForUpdates);
		}
		
		[Test]
		public void GetUpdatedPackages_JQueryPackageInstalledTwiceWithDifferentVersions_OnlyOlderJQueryPackageUsedToDetermineUpdatedPackages()
		{
			IPackage expectedPackage = AddInstalledPackage("jquery", "1.6");
			var expectedPackages = new IPackage[] { expectedPackage };
			AddInstalledPackage("jquery", "1.7");
			AddPackageToSourceRepository("jquery", "2.1");
			CreateUpdatedPackages();
			
			updatedPackages.GetUpdatedPackages();
			
			PackageCollectionAssert.AreEqual(expectedPackages, packagesUsedWhenCheckingForUpdates);
		}
		
		[Test]
		public void GetUpdatedPackages_JQueryPackageInstalledTwiceWithDifferentVersionsAndNewerVersionsFirst_OnlyOlderJQueryPackageUsedToDetermineUpdatedPackages()
		{
			AddInstalledPackage("jquery", "1.7");
			IPackage expectedPackage =  AddInstalledPackage("jquery", "1.6");
			var expectedPackages = new IPackage[] { expectedPackage };
			AddPackageToSourceRepository("jquery", "2.1");
			CreateUpdatedPackages();
			
			updatedPackages.GetUpdatedPackages();
			
			PackageCollectionAssert.AreEqual(expectedPackages, packagesUsedWhenCheckingForUpdates);
		}
		
		[Test]
		public void GetUpdatedPackages_AllowPrereleaseIsTrue_PrereleasePackagesAllowedForUpdates()
		{
			AddInstalledPackage("Test", "1.0");
			CreateUpdatedPackages();
			
			updatedPackages.GetUpdatedPackages(includePrerelease: true);
			
			Assert.IsTrue(includePreleaseUsedWhenCheckingForUpdates);
		}
		
		[Test]
		public void GetUpdatedPackages_AllowPrereleaseIsFalse_PrereleasePackagesNotAllowedForUpdates()
		{
			AddInstalledPackage("Test", "1.0");
			CreateUpdatedPackages();
			
			updatedPackages.GetUpdatedPackages(includePrerelease: false);
			
			Assert.IsFalse(includePreleaseUsedWhenCheckingForUpdates);
		}
	}
}
