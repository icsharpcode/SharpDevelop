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
	public class AvailablePackagesViewModelTests
	{
		AvailablePackagesViewModel viewModel;
		FakePackageManagementService packageManagementService;
		FakeTaskFactory taskFactory = new FakeTaskFactory();
		ExceptionThrowingPackageManagementService exceptionThrowingPackageManagementService;
		
		void CreateViewModel()
		{
			CreatePackageManagementService();
			CreateViewModel(packageManagementService);
		}
		
		void CreatePackageManagementService()
		{
			packageManagementService = new FakePackageManagementService();
		}
		
		void CreateViewModel(IPackageManagementService packageManagementService)
		{
			taskFactory = new FakeTaskFactory();
			var messageReporter = new FakeMessageReporter();
			viewModel = new AvailablePackagesViewModel(packageManagementService, messageReporter, taskFactory);
		}
		
		void CreateExceptionThrowingPackageManagementService()
		{
			exceptionThrowingPackageManagementService = new ExceptionThrowingPackageManagementService();
		}
		
		void CompleteReadPackagesTask()
		{
			taskFactory.ExecuteAllFakeTasks();
		}
		
		void ClearReadPackagesTasks()
		{
			taskFactory.ClearAllFakeTasks();
		}

		void AddOnePackageSourceToRegisteredSources()
		{
			packageManagementService.ClearPackageSources();
			packageManagementService.AddOnePackageSource();
			packageManagementService.HasMultiplePackageSources = false;
		}
		
		void AddTwoPackageSourcesToRegisteredSources()
		{
			var expectedPackageSources = new PackageSource[] {
				new PackageSource("http://first.com", "First"),
				new PackageSource("http://second.com", "Second")
			};
			AddPackageSourcesToRegisteredSources(expectedPackageSources);
			packageManagementService.HasMultiplePackageSources = true;
		}
				
		void AddPackageSourcesToRegisteredSources(PackageSource[] sources)
		{
			packageManagementService.ClearPackageSources();
			packageManagementService.AddPackageSources(sources);
		}
		
		void CreateNewActiveRepositoryWithDifferentPackages()
		{
			var package = new FakePackage("NewRepositoryPackageId");
			var newRepository = new FakePackageRepository();
			newRepository.FakePackages.Add(package);
			packageManagementService.FakeActivePackageRepository = newRepository;
		}
		
		void SetUpTwoPackageSourcesAndViewModelHasReadPackages()
		{
			CreatePackageManagementService();
			AddTwoPackageSourcesToRegisteredSources();
			CreateViewModel(packageManagementService);
			packageManagementService.ActivePackageSource = packageManagementService.Options.PackageSources[0];
			viewModel.ReadPackages();
			CompleteReadPackagesTask();
			CreateNewActiveRepositoryWithDifferentPackages();
		}
		
		void ChangeSelectedPackageSourceToSecondSource()
		{
			var secondPackageSource = packageManagementService.Options.PackageSources[1];
			viewModel.SelectedPackageSource = secondPackageSource;
		}
		
		void ChangeSelectedPackageSourceToFirstSource()
		{
			var firstPackageSource = packageManagementService.Options.PackageSources[0];
			viewModel.SelectedPackageSource = firstPackageSource;
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
			CompleteReadPackagesTask();
			
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
			CompleteReadPackagesTask();
			
			ClearReadPackagesTasks();
			viewModel.SearchTerms = "NotAMatch";
			viewModel.Search();
			CompleteReadPackagesTask();
			
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
			CompleteReadPackagesTask();
			
			ClearReadPackagesTasks();
			bool collectionChangedEventFired = false;
			viewModel.Pages.CollectionChanged += (sender, e) => collectionChangedEventFired = true;
			viewModel.ShowNextPage();
			CompleteReadPackagesTask();
			
			var expectedPackages = new FakePackage[] {
				package4
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
			Assert.IsTrue(collectionChangedEventFired);
		}
		
		[Test]
		public void ShowSources_TwoPackageSources_ReturnsTrue()
		{
			CreatePackageManagementService();
			AddTwoPackageSourcesToRegisteredSources();
			CreateViewModel(packageManagementService);
			
			Assert.IsTrue(viewModel.ShowPackageSources);
		}
		
		[Test]
		public void ShowPackageSources_OnePackageSources_ReturnsFalse()
		{
			CreatePackageManagementService();
			AddOnePackageSourceToRegisteredSources();
			CreateViewModel(packageManagementService);
			
			Assert.IsFalse(viewModel.ShowPackageSources);
		}
		
		[Test]
		public void PackageSources_TwoPackageSourcesInOptions_HasTwoRepositoriesInCollection()
		{
			CreatePackageManagementService();
			AddTwoPackageSourcesToRegisteredSources();
			CreateViewModel(packageManagementService);
			
			var expectedPackageSources = packageManagementService.Options.PackageSources;
			
			PackageSourceCollectionAssert.AreEqual(expectedPackageSources, viewModel.PackageSources);
		}
		
		[Test]
		public void SelectedPackageSource_TwoPackageSourcesInOptionsAndActivePackageSourceIsFirstSource_IsFirstPackageSource()
		{
			CreatePackageManagementService();
			AddTwoPackageSourcesToRegisteredSources();
			CreateViewModel(packageManagementService);
			
			var expectedPackageSource = packageManagementService.Options.PackageSources[0];
			packageManagementService.ActivePackageSource = expectedPackageSource;
			
			Assert.AreEqual(expectedPackageSource, viewModel.SelectedPackageSource);
		}
		
		[Test]
		public void SelectedPackageSource_TwoPackageSourcesInOptionsAndActivePackageSourceIsSecondSource_IsSecondPackageSource()
		{
			CreatePackageManagementService();
			AddTwoPackageSourcesToRegisteredSources();
			CreateViewModel(packageManagementService);
			
			var expectedPackageSource = packageManagementService.Options.PackageSources[1];
			packageManagementService.ActivePackageSource = expectedPackageSource;
			
			Assert.AreEqual(expectedPackageSource, viewModel.SelectedPackageSource);
		}
		
		[Test]
		public void SelectedPackageSource_Changed_PackageManagementServiceActivatePackageSourceChanged()
		{
			CreatePackageManagementService();
			AddTwoPackageSourcesToRegisteredSources();
			CreateViewModel(packageManagementService);
			
			packageManagementService.ActivePackageSource = packageManagementService.Options.PackageSources[0];
			var expectedPackageSource = packageManagementService.Options.PackageSources[1];
			viewModel.SelectedPackageSource = expectedPackageSource;
			
			Assert.AreEqual(expectedPackageSource, packageManagementService.ActivePackageSource);
		}
		
		[Test]
		public void SelectedPackageSource_PackageSourceChangedAfterReadingPackages_PackagesReadFromNewPackageSourceAndDisplayed()
		{
			SetUpTwoPackageSourcesAndViewModelHasReadPackages();
			ClearReadPackagesTasks();
			ChangeSelectedPackageSourceToSecondSource();
			CompleteReadPackagesTask();
			
			var expectedPackages = packageManagementService.FakeActivePackageRepository.FakePackages;
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void SelectedPackageSource_PackageSourceChangedAfterReadingPackages_PropertyChangedEventFiredAfterPackagesAreRead()
		{
			SetUpTwoPackageSourcesAndViewModelHasReadPackages();
			
			int packageCountWhenPropertyChangedEventFired = -1;
			viewModel.PropertyChanged += (sender, e) => packageCountWhenPropertyChangedEventFired = viewModel.PackageViewModels.Count;
			ClearReadPackagesTasks();
			ChangeSelectedPackageSourceToSecondSource();
			CompleteReadPackagesTask();
			
			Assert.AreEqual(1, packageCountWhenPropertyChangedEventFired);
		}
		
		[Test]
		public void SelectedPackageSource_PackageSourceChangedButToSameSelectedPackageSource_PackagesAreNotRead()
		{
			SetUpTwoPackageSourcesAndViewModelHasReadPackages();
			ChangeSelectedPackageSourceToFirstSource();
			
			Assert.AreEqual(0, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void SelectedPackageSource_PackageSourceChangedButToSameSelectedPackageSource_PropertyChangedEventNotFired()
		{
			SetUpTwoPackageSourcesAndViewModelHasReadPackages();
			
			bool fired = false;
			viewModel.PropertyChanged += (sender, e) => fired = true;
			ChangeSelectedPackageSourceToFirstSource();
			
			Assert.IsFalse(fired);
		}
		
		[Test]
		public void GetAllPackages_OnePackageInRepository_RepositoryNotCreatedByBackgroundThread()
		{
			CreatePackageManagementService();
			AddOnePackageSourceToRegisteredSources();
			packageManagementService.FakeActivePackageRepository.FakePackages.Add(new FakePackage());
			CreateViewModel(packageManagementService);
			viewModel.ReadPackages();
			
			packageManagementService.FakeActivePackageRepository = null;
			CompleteReadPackagesTask();
			
			Assert.AreEqual(1, viewModel.PackageViewModels.Count);
		}
		
		[Test]
		public void ReadPackages_ExceptionThrownWhenAccessingActiveRepository_ErrorMessageFromExceptionNotOverriddenByReadPackagesCall()
		{
			CreateExceptionThrowingPackageManagementService();
			exceptionThrowingPackageManagementService.ExeptionToThrowWhenActiveRepositoryAccessed = 
				new Exception("Test");
			CreateViewModel(exceptionThrowingPackageManagementService);
			viewModel.ReadPackages();
			
			ApplicationException ex = Assert.Throws<ApplicationException>(() => CompleteReadPackagesTask());
			Assert.AreEqual("Test", ex.Message);
		}
	}
}
