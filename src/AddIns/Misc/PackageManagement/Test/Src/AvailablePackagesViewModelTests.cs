// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class AvailablePackagesViewModelTests
	{
		AvailablePackagesViewModel viewModel;
		FakePackageManagementService packageManagementService;
		
		void CreateViewModel()
		{
			packageManagementService = new FakePackageManagementService();
			viewModel = new AvailablePackagesViewModel(packageManagementService);
		}
		
		[Test]
		public void ReadPackages_RepositoryHasThreePackagesWithSameIdButDifferentVersions_HasLatestPackageVersionOnly()
		{
			CreateViewModel();
			
			var package1 = new FakePackage() {
             	Id = "Test",
             	Version = new Version(0, 1, 0, 0)
            };
			var package2 = new FakePackage() {
				Id = "Test",
				Version = new Version(0, 2, 0, 0)
			};
			var package3 = new FakePackage() {
				Id = "Test",
				Version = new Version(0, 3, 0, 0)
			};
			var packages = new FakePackage[] {
				package1, package2, package3
			};
			
			packageManagementService.FakeActivePackageRepository.FakePackages.AddRange(packages);
			
			viewModel.ReadPackages();
			
			var expectedPackages = new FakePackage[] {
				package3
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void IsSearchable_ByDefault_ReturnsTrue()
		{
			CreateViewModel();
			Assert.IsTrue(viewModel.IsSearchable);
		}
	
		[Test]
		public void Search_RepositoryHasThreePackagesWithSameIdButSearchTermsMatchNoPackageIds_ReturnsNoPackages()
		{
			CreateViewModel();
			
			var package1 = new FakePackage() {
             	Id = "Test",
             	Description = "",
             	Version = new Version(0, 1, 0, 0)
            };
			var package2 = new FakePackage() {
				Id = "Test",
             	Description = "",
				Version = new Version(0, 2, 0, 0)
			};
			var package3 = new FakePackage() {
				Id = "Test",
             	Description = "",
				Version = new Version(0, 3, 0, 0)
			};
			var packages = new FakePackage[] {
				package1, package2, package3
			};
			
			packageManagementService.FakeActivePackageRepository.FakePackages.AddRange(packages);
			
			viewModel.ReadPackages();
			
			viewModel.SearchTerms = "NotAMatch";
			viewModel.Search();
			
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void ShowNextPage_TwoObjectsWatchingForPagesCollectionChangedEventAndUserMovesToPageTwoAndFilteredPackagesReturnsLessThanExpectedPackagesDueToMatchingVersions_InvalidOperationExceptionNotThrownWhen()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			
			var package1 = new FakePackage() {
             	Id = "First",
             	Description = "",
             	Version = new Version(0, 1, 0, 0)
            };
			var package2 = new FakePackage() {
				Id = "Secon",
             	Description = "",
				Version = new Version(0, 2, 0, 0)
			};
			var package3 = new FakePackage() {
				Id = "Test",
             	Description = "",
				Version = new Version(0, 3, 0, 0)
			};
			var package4 = new FakePackage() {
				Id = "Test",
             	Description = "",
				Version = new Version(0, 4, 0, 0)
			};
			var packages = new FakePackage[] {
				package1, package2, package3, package4
			};
			
			packageManagementService.FakeActivePackageRepository.FakePackages.AddRange(packages);
			
			viewModel.ReadPackages();
			
			bool collectionChangedEventFired = false;
			viewModel.Pages.CollectionChanged += (sender, e) => collectionChangedEventFired = true;
			viewModel.ShowNextPage();
			
			var expectedPackages = new FakePackage[] {
				package4
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
			Assert.IsTrue(collectionChangedEventFired);
		}
	}
}
