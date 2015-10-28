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
using System.Linq;
using System.Runtime.Versioning;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
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
		FakePackageManagementProject project;
		
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
			
			updatedPackages = new UpdatedPackages(installedPackages.AsQueryable(), sourceRepository, NullConstraintProvider.Instance);
		}
		
		void CreateProject()
		{
			project = new FakePackageManagementProject();
		}
		
		void CreateUpdatedPackages(IPackageRepository repository)
		{
			updatedPackages = new UpdatedPackages(project, repository);
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
		
		[Test]
		public void GetUpdatedPackages_OnePackageReferencedWithConstraintAndUpdatesAvailable_LatestVersionReturnedBasedOnConstraint()
		{
			CreateProject();
			project.FakePackages.Add(new FakePackage("Test", "1.0"));
			var sourceRepository = new FakePackageRepository();
			FakePackage packageVersion2 = sourceRepository.AddFakePackageWithVersion("Test", "2.0");
			FakePackage [] expectedPackages = new [] {
				packageVersion2
			};
			sourceRepository.AddFakePackageWithVersion("Test", "3.0");
			var versionSpec = new VersionSpec();
			versionSpec.MinVersion = new SemanticVersion("1.0");
			versionSpec.IsMinInclusive = true;
			versionSpec.MaxVersion = new SemanticVersion("2.0");
			versionSpec.IsMaxInclusive = true;
			var constraintProvider = new DefaultConstraintProvider();
			constraintProvider.AddConstraint("Test", versionSpec);
			project.ConstraintProvider = constraintProvider;
			CreateUpdatedPackages(sourceRepository);

			IEnumerable<IPackage> packages = updatedPackages.GetUpdatedPackages();

			PackageCollectionAssert.AreEqual(expectedPackages, packages);
		}
	}
}
