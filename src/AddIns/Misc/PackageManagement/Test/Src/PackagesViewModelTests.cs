// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackagesViewModelTests
	{
		TestablePackagesViewModel viewModel;
		FakePackageManagementService packageManagementService;
		
		void CreateViewModel()
		{
			viewModel = new TestablePackagesViewModel();
			packageManagementService = viewModel.FakePackageManagementService;
		}
		
		[Test]
		public void IsPaged_OnePackageAndPageSizeIsFive_ReturnsFalse()
		{
			CreateViewModel();
			viewModel.PageSize = 5;
			viewModel.AddOneFakePackage();
			viewModel.ReadPackages();
			
			bool paged = viewModel.IsPaged;
			
			Assert.IsFalse(paged);
		}
		
		[Test]
		public void IsPaged_SixPackagesAndPageSizeIsFive_ReturnsTrue()
		{
			CreateViewModel();
			viewModel.PageSize = 5;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			
			bool paged = viewModel.IsPaged;
			
			Assert.IsTrue(paged);
		}
		
		[Test]
		public void SelectedPageNumber_ByDefault_ReturnsOne()
		{
			CreateViewModel();
			
			int pageNumber = viewModel.SelectedPageNumber;
			
			Assert.AreEqual(1, pageNumber);
		}
		
		[Test]
		public void HasPreviousPage_SixPackagesSelectedPageNumberIsOneAndPageSizeIsFive_ReturnsFalse()
		{
			CreateViewModel();
			viewModel.PageSize = 5;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 1;
			
			Assert.IsFalse(viewModel.HasPreviousPage);
		}
		
		[Test]
		public void HasPreviousPage_SixPackagesSelectedPageNumberIsTwoAndPageSizeIsFive_ReturnsTrue()
		{
			CreateViewModel();
			viewModel.PageSize = 5;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 2;
			
			Assert.IsTrue(viewModel.HasPreviousPage);
		}
		
		[Test]
		public void HasPreviousPage_SelectedPagesChangesFromFirstPageToSecond_PropertyChangedEventFiredForAllProperties()
		{
			CreateViewModel();
			viewModel.PageSize = 5;
			viewModel.AddSixFakePackages();
			viewModel.SelectedPageNumber = 1;
			viewModel.ReadPackages();
			
			PropertyChangedEventArgs propertyChangedEvent = null;
			viewModel.PropertyChanged += (sender, e) => propertyChangedEvent = e;
			viewModel.SelectedPageNumber = 2;
			
			string propertyName = propertyChangedEvent.PropertyName;
			
			Assert.IsNull(propertyName);
		}
		
		[Test]
		public void HasNextPage_SixPackagesSelectedPageNumberIsOneAndPageSizeIsFive_ReturnsTrue()
		{
			CreateViewModel();
			viewModel.PageSize = 5;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 1;
			
			Assert.IsTrue(viewModel.HasNextPage);
		}
		
		[Test]
		public void HasNextPage_SixPackagesSelectedPageNumberIsTwoAndPageSizeIsFive_ReturnsFalse()
		{
			CreateViewModel();
			viewModel.PageSize = 5;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 2;
			
			Assert.IsFalse(viewModel.HasNextPage);
		}
		
		[Test]
		public void HasNextPage_SixPackagesSelectedPageNumberIsTwoAndPageSizeIsTwo_ReturnsTrue()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 2;
			
			Assert.IsTrue(viewModel.HasNextPage);
		}
		
		[Test]
		public void Pages_SixPackagesSelectedPageNumberIsTwoAndPageSizeIsFive_ReturnsTwoPagesWithSecondOneSelected()
		{
			CreateViewModel();
			viewModel.PageSize = 5;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 2;
			
			Page[] expectedPages = new Page[] {
				new Page() { Number = 1 },
				new Page() { Number = 2, IsSelected = true }
			};
			
			var actualPages = viewModel.Pages;
			
			PageCollectionAssert.AreEqual(expectedPages, actualPages);
		}
		
		[Test]
		public void Pages_SixPackagesSelectedPageNumberIsOneAndPageSizeIsFive_ReturnsTwoPagesWithFirstOneSelected()
		{
			CreateViewModel();
			viewModel.PageSize = 5;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 1;
			
			Page[] expectedPages = new Page[] {
				new Page() { Number = 1, IsSelected = true },
				new Page() { Number = 2 }
			};
			
			var actualPages = viewModel.Pages;
			
			PageCollectionAssert.AreEqual(expectedPages, actualPages);
		}
		
		[Test]
		public void Pages_SixPackagesSelectedPageNumberIsOneAndPageSizeIsTwo_ReturnsThreePagesWithFirstOneSelected()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 1;
			
			Page[] expectedPages = new Page[] {
				new Page() { Number = 1, IsSelected = true },
				new Page() { Number = 2 },
				new Page() { Number = 3 }
			};
			
			var actualPages = viewModel.Pages;
			
			PageCollectionAssert.AreEqual(expectedPages, actualPages);
		}
		
		[Test]
		public void Pages_SixPackagesSelectedPageNumberIsOneAndPageSizeIsTwoAndMaximumSelectablePagesIsTwo_ReturnsTwoPagesWithFirstOneSelected()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 1;
			viewModel.MaximumSelectablePages = 2;
			
			Page[] expectedPages = new Page[] {
				new Page() { Number = 1, IsSelected = true },
				new Page() { Number = 2 }
			};
			
			var actualPages = viewModel.Pages;
			
			PageCollectionAssert.AreEqual(expectedPages, actualPages);
		}
		
		[Test]
		public void Pages_SixPackagesSelectedPageNumberIsOneAndPageSizeIsFiveGetPagesTwice_ReturnsTwoPagesWithFirstOneSelected()
		{
			CreateViewModel();
			viewModel.PageSize = 5;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 1;
			
			Page[] expectedPages = new Page[] {
				new Page() { Number = 1, IsSelected = true },
				new Page() { Number = 2 }
			};
			
			var actualPages = viewModel.Pages;
			actualPages = viewModel.Pages;
			
			PageCollectionAssert.AreEqual(expectedPages, actualPages);
		}
		
		[Test]
		public void Pages_SixPackagesSelectedPageNumberIsThreeAndPageSizeIsTwoAndMaximumSelectablePagesIsTwo_ReturnsPagesTwoAndThreeWithPageThreeSelected()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 3;
			viewModel.MaximumSelectablePages = 2;
			
			Page[] expectedPages = new Page[] {
				new Page() { Number = 2 },
				new Page() { Number = 3, IsSelected = true }
			};
			
			var actualPages = viewModel.Pages;
			
			PageCollectionAssert.AreEqual(expectedPages, actualPages);
		}

		[Test]
		public void Pages_TenPackagesSelectedPageNumberIsFiveAndPageSizeIsTwoAndMaximumSelectablePagesIsThree_ReturnsPagesThreeAndFourAndFive()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.AddTenFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 5;
			viewModel.MaximumSelectablePages = 3;
			
			Page[] expectedPages = new Page[] {
				new Page() { Number = 3 },
				new Page() { Number = 4 },
				new Page() { Number = 5, IsSelected = true }
			};
			
			var actualPages = viewModel.Pages;
			
			PageCollectionAssert.AreEqual(expectedPages, actualPages);
		}
		
		[Test]
		public void ReadPackages_RepositoryHasThreePackagesWhenSelectedPageIsOneAndPageSizeIsTwo_TwoPackageViewModelsCreatedForFirstTwoPackages()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.SelectedPageNumber = 1;
			viewModel.AddThreeFakePackages();
			viewModel.ReadPackages();
			
			var expectedPackages = new List<FakePackage>();
			expectedPackages.Add(viewModel.FakePackages[0]);
			expectedPackages.Add(viewModel.FakePackages[1]);
			
			viewModel.ReadPackages();
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void ReadPackages_RepositoryHasSixPackagesWhenSelectedPageIsOneAndPageSizeIsThree_ThreePackageViewModelsCreatedForFirstThreePackages()
		{
			CreateViewModel();
			viewModel.PageSize = 3;
			viewModel.SelectedPageNumber = 1;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			
			var expectedPackages = new List<FakePackage>();
			expectedPackages.Add(viewModel.FakePackages[0]);
			expectedPackages.Add(viewModel.FakePackages[1]);
			expectedPackages.Add(viewModel.FakePackages[2]);
			
			viewModel.ReadPackages();
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void Pages_PageSizeChanged_PagesRecalcuatedBasedOnNewPageSize()
		{
			CreateViewModel();
			viewModel.PageSize = 10;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			
			int oldPageCount = viewModel.Pages.Count;
			viewModel.PageSize = 5;
			int newPageCount = viewModel.Pages.Count;
			
			Assert.AreEqual(2, newPageCount);
			Assert.AreEqual(1, oldPageCount);
		}

		[Test]
		public void Pages_SelectedPageNumberChanged_PagesRecalculatedBasedOnNewSelectedPage()
		{
			CreateViewModel();
			viewModel.PageSize = 3;
			viewModel.SelectedPageNumber = 1;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			
			var oldPages = viewModel.Pages;
			viewModel.SelectedPageNumber = 2;
			var newPages = viewModel.Pages;
			
			Page[] expectedPages = new Page[] {
				new Page() { Number = 1 },
				new Page() { Number = 2, IsSelected = true }
			};
			
			PageCollectionAssert.AreEqual(expectedPages, newPages);
		}
		
		[Test]
		public void ShowNextPageCommand_TwoPagesAndFirstPageSelectedWhenCommandExecuted_PageTwoIsSelected()
		{
			CreateViewModel();
			viewModel.PageSize = 3;
			viewModel.SelectedPageNumber = 1;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			
			viewModel.ShowNextPageCommand.Execute(null);
			
			int selectedPage = viewModel.SelectedPageNumber;
			
			Assert.AreEqual(2, selectedPage);
		}
		
		[Test]
		public void ShowNextPageCommand_TwoPagesAndFirstPageSelectedWhenCommandExecuted_SecondPageOfPackagesDisplayed()
		{
			CreateViewModel();
			viewModel.AddThreeFakePackages();
			viewModel.PageSize = 2;
			viewModel.SelectedPageNumber = 1;
			viewModel.ReadPackages();
			
			viewModel.ShowNextPageCommand.Execute(null);
			
			var expectedPackages = new List<FakePackage>();
			expectedPackages.Add(viewModel.FakePackages[2]);
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void ShowPreviousPageCommand_TwoPagesAndSecondPageSelectedWhenCommandExecuted_PageOneIsSelected()
		{
			CreateViewModel();
			viewModel.PageSize = 3;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 2;
			
			viewModel.ShowPreviousPageCommand.Execute(null);
			
			int selectedPage = viewModel.SelectedPageNumber;
			
			Assert.AreEqual(1, selectedPage);
		}
		
		[Test]
		public void ShowPreviousPageCommand_TwoPagesAndSecondPageSelectedWhenCommandExecuted_FirstPageOfPackagesDisplayed()
		{
			CreateViewModel();
			viewModel.AddThreeFakePackages();
			viewModel.PageSize = 2;
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 2;
			
			viewModel.ShowPreviousPageCommand.Execute(null);
			
			var expectedPackages = new List<FakePackage>();
			expectedPackages.Add(viewModel.FakePackages[0]);
			expectedPackages.Add(viewModel.FakePackages[1]);
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void ShowPageCommand_PageNumberOneToBeShownWhenCurrentlySelectedPageIsTwo_PageOneIsSelected()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.SelectedPageNumber = 2;
			
			int pageNumber = 1;
			viewModel.ShowPageCommand.Execute(pageNumber);
			
			int selectedPage = viewModel.SelectedPageNumber;
			
			Assert.AreEqual(1, selectedPage);
		}
		
		[Test]
		public void Pages_ReadPackagesAndIsPagedCalled_PackagesReadFromRepositoryOnlyOnce()
		{
			CreateViewModel();
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			bool result = viewModel.IsPaged;
			int count = viewModel.Pages.Count;
			
			Assert.AreEqual(1, viewModel.GetAllPackagesCallCount);
		}
		
		[Test]
		public void ReadPackages_CalledThreeTimesAndThenSelectedPageChanged_ViewModelPropertiesChangedEventFiresFourTimesWhenSelectedPageChanged()
		{
			CreateViewModel();
			viewModel.PageSize = 3;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			viewModel.ReadPackages();
			viewModel.ReadPackages();
			
			int count = 0;
			viewModel.PropertyChanged += (sender, e) => count++;
			viewModel.SelectedPageNumber = 2;
			
			// PropertyChanged fired for clearing the pages. 
			int propertyChangedEventFiredForClearingPagesCount = 1;
			
			// PropertyChanged fired once for each page.
			int propertyChangedEventFiredForAddingPagesCount = 2;
			
			int totalExpectedPropertyChangedFiredCount =
				propertyChangedEventFiredForClearingPagesCount + propertyChangedEventFiredForAddingPagesCount;
			
			Assert.AreEqual(totalExpectedPropertyChangedFiredCount, count);
		}
		
		[Test]
		public void IsSearchable_ByDefault_ReturnsFalse()
		{
			CreateViewModel();
			
			Assert.IsFalse(viewModel.IsSearchable);
		}
		
		[Test]
		public void SearchCommand_SearchTextEntered_PackageViewModelsFilteredBySearchCriteria()
		{
			CreateViewModel();
			viewModel.IsSearchable = true;
			viewModel.AddSixFakePackages();
			
			var package = new FakePackage() {
				Id = "SearchedForId",
				Description = "Test"
			};
			viewModel.FakePackages.Add(package);
			
			viewModel.ReadPackages();
			
			viewModel.SearchTerms = "SearchedForId";
			viewModel.SearchCommand.Execute(null);
			
			var expectedPackages = new FakePackage[] {
				package
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, viewModel.PackageViewModels);
		}
		
		[Test]
		public void PackageExtensionsFind_TwoPackagesInCollection_FindsOnePackageId()
		{
			List<IPackage> packages = new List<IPackage>();
			var package1 = new FakePackage() {
				Id = "Test"
			};
			var package2 = new FakePackage() {
				Id = "Another"
			};
			packages.Add(package1);
			packages.Add(package2);
			
			IQueryable<IPackage> query = packages.AsQueryable();
			
			IQueryable<IPackage> filteredResults = query.Find("Test");
			
			IPackage foundPackage = filteredResults.First();
			
			Assert.AreEqual("Test", foundPackage.Id);
		}
		
		[Test]
		public void PackageExtensionsFind_TwoPackagesInCollectionAndQueryableResultsPutInBufferedEnumerable_OnePackageInBufferedEnumerable()
		{
			List<IPackage> packages = new List<IPackage>();
			
			// Need to add descriptiosn otherwise we get a null reference when enumerating results 
			// in BufferedEnumerable
			var package1 = new FakePackage() {
				Id = "Test", Description = "b"
			};
			var package2 = new FakePackage() {
				Id = "Another", Description = "a"
			};
			packages.Add(package1);
			packages.Add(package2);
			
			IQueryable<IPackage> query = packages.AsQueryable();
			
			IQueryable<IPackage> filteredResults = query.Find("Test");
			
			var collection = new BufferedEnumerable<IPackage>(filteredResults, 10);
			IPackage foundPackage = collection.First();
			
			Assert.AreEqual("Test", foundPackage.Id);
		}
		
		[Test]
		public void Search_SearchTextChangedAndPackagesWerePagedBeforeSearch_PagesUpdatedAfterFilteringBySearchCriteria()
		{
			CreateViewModel();
			viewModel.IsSearchable = true;
			viewModel.PageSize = 2;
			viewModel.MaximumSelectablePages = 5;
			viewModel.AddSixFakePackages();
			
			var package = new FakePackage() {
				Id = "SearchedForId",
				Description = "Test"
			};
			viewModel.FakePackages.Add(package);
			
			viewModel.ReadPackages();
			
			ObservableCollection<Page> pages = viewModel.Pages;
			
			viewModel.SearchTerms = "SearchedForId";
			viewModel.Search();
			
			var expectedPages = new Page[] {
				new Page() { Number = 1, IsSelected = true }
			};
			
			PageCollectionAssert.AreEqual(expectedPages, pages);
		}
		
		[Test]
		public void Pages_SixPackagesButPackagesNotRead_HasNoPages()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.AddSixFakePackages();
			
			Assert.AreEqual(0, viewModel.Pages.Count);
		}
		
		[Test]
		public void HasPreviousPage_SixPackagesAndSecondPageSelectedButPackagesNotRead_ReturnsFalse()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.SelectedPageNumber = 2;
			viewModel.AddSixFakePackages();
			
			Assert.IsFalse(viewModel.HasPreviousPage);
		}
		
		[Test]
		public void HasNextPage_SixPackagesAndFirstPageSelectedButPackagesNotRead_ReturnsFalse()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.SelectedPageNumber = 1;
			viewModel.AddSixFakePackages();
			
			Assert.IsFalse(viewModel.HasNextPage);
		}
		
		[Test]
		public void IsPaged_SixPackagesAndFirstPageSelectedButPackagesNotRead_ReturnsFalse()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.SelectedPageNumber = 1;
			viewModel.AddSixFakePackages();
			
			Assert.IsFalse(viewModel.IsPaged);
		}
		
		[Test]
		public void Search_SelectedPageInitiallyIsPageTwoAndThenUserSearches_SelectedPageNumberIsSetToPageOne()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.AddSixFakePackages();
			
			var package = new FakePackage() {
				Id = "SearchedForId",
				Description = "Test"
			};
			viewModel.FakePackages.Add(package);
			viewModel.ReadPackages();

			viewModel.SelectedPageNumber = 2;
			
			viewModel.SearchTerms = "SearchedForId";
			viewModel.Search();
			
			Assert.AreEqual(1, viewModel.SelectedPageNumber);
		}
		
		/// <summary>
		/// Ensures that the total number of packages is determined from all packages and not
		/// the filtered set. All packages will be retrieved from the repository
		/// if this is not done when we only want 30 retrieved in one go.
		/// </summary>
		[Test]
		public void ReadPackages_SixPackagesInRepository_TotalPagesSetBeforePackagesFiltered()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			
			int expectedPages = 3;
			Assert.AreEqual(expectedPages, viewModel.PageCountBeforePackagesFiltered);
		}
		
		[Test]
		public void Search_ThreePagesOfPackagesBeforeSearchReturnsNoPackages_IsPagedIsFalseWhenPropertyChangedEventFired()
		{
			CreateViewModel();
			viewModel.PageSize = 2;
			viewModel.AddSixFakePackages();
			viewModel.ReadPackages();
			
			viewModel.SearchTerms = "SearchedForId";
			
			bool paged = true;
			viewModel.PropertyChanged += (sender, e) => paged = viewModel.IsPaged;
			viewModel.Search();
			
			Assert.IsFalse(paged);
		}
	}
}
