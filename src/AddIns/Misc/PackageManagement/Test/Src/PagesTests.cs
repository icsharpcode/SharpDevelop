// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PagesTests
	{
		Pages pages;
		
		void CreatePages()
		{
			pages = new Pages();
		}
		
		[Test]
		public void Pages_TotalItemsChanged_PagesUpdatedAfterChangeInTotalItems()
		{
			CreatePages();
			pages.PageSize = 2;
			pages.TotalItems = 2;
			
			int oldPageCount = pages.Count;
			pages.TotalItems = 3;
			int newPageCount = pages.Count;
			
			Assert.AreEqual(2, newPageCount);
			Assert.AreEqual(1, oldPageCount);
		}
		
		[Test]
		public void Pages_SelectedPageNumberChanged_PagesUpdatedAfterChange()
		{
			CreatePages();
			pages.PageSize = 1;
			pages.TotalItems = 2;
			pages.SelectedPageNumber = 1;
			
			pages.SelectedPageNumber = 2;
			
			Page[] expectedPages = new Page[] {
				new Page() { Number = 1 },
				new Page() { Number = 2, IsSelected = true }
			};
			
			PageCollectionAssert.AreEqual(expectedPages, pages);
		}
		
		[Test]
		public void Pages_PageSizeChanged_PagesUpdatedAfterChange()
		{
			CreatePages();
			pages.PageSize = 2;
			pages.SelectedPageNumber = 1;
			pages.TotalItems = 2;
			
			pages.PageSize = 1;
			
			Page[] expectedPages = new Page[] {
				new Page() { Number = 1, IsSelected = true },
				new Page() { Number = 2 }
			};
			
			PageCollectionAssert.AreEqual(expectedPages, pages);
		}
		
		[Test]
		public void Pages_MaximumSelectablePagesChanged_PagesUpdatedAfterChange()
		{
			CreatePages();
			pages.MaximumSelectablePages = 5;
			pages.PageSize = 2;
			pages.SelectedPageNumber = 1;
			pages.TotalItems = 10;
			
			pages.MaximumSelectablePages = 2;
			
			Page[] expectedPages = new Page[] {
				new Page() { Number = 1, IsSelected = true },
				new Page() { Number = 2 }
			};
			
			PageCollectionAssert.AreEqual(expectedPages, pages);
		}
		
		[Test]
		public void Pages_MaximumSelectablePagesSetToSameValue_PagesCollectionNotChanged()
		{
			CreatePages();
			pages.TotalItems = 1;
			pages.MaximumSelectablePages = 3;
			
			bool collectionChanged = false;
			pages.CollectionChanged += (sender, e) => collectionChanged = true;
			
			pages.MaximumSelectablePages = 3;
			
			Assert.IsFalse(collectionChanged);
		}
		
		[Test]
		public void Pages_PageSizeSetToSameValue_PagesCollectionNotChanged()
		{
			CreatePages();
			pages.TotalItems = 1;
			pages.PageSize = 3;
			
			bool collectionChanged = false;
			pages.CollectionChanged += (sender, e) => collectionChanged = true;
			
			pages.PageSize = 3;
			
			Assert.IsFalse(collectionChanged);
		}
		
		[Test]
		public void Pages_SelectedPageNumberSetToSameValue_PagesCollectionNotChanged()
		{
			CreatePages();
			pages.TotalItems = 1;
			pages.SelectedPageNumber = 1;
			
			bool collectionChanged = false;
			pages.CollectionChanged += (sender, e) => collectionChanged = true;
			
			pages.SelectedPageNumber = 1;
			
			Assert.IsFalse(collectionChanged);
		}
		
		[Test]
		public void Pages_TotalItemsSetToSameValue_PagesCollectionNotChanged()
		{
			CreatePages();
			pages.TotalItems = 1;
			
			bool collectionChanged = false;
			pages.CollectionChanged += (sender, e) => collectionChanged = true;
			
			pages.TotalItems = 1;
			
			Assert.IsFalse(collectionChanged);
		}
		
		[Test]
		public void ItemsBeforeFirstPage_OnePage_ReturnsZero()
		{
			CreatePages();
			pages.TotalItems = 1;
			int count = pages.ItemsBeforeFirstPage;
						
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void ItemsBeforeFirstPage_PageSizeIsTwoAndFourItemsAndSecondPageSelected_ReturnsTwo()
		{
			CreatePages();
			pages.PageSize = 2;
			pages.TotalItems = 4;
			pages.SelectedPageNumber = 2;
			int count = pages.ItemsBeforeFirstPage;
						
			Assert.AreEqual(2, count);
		}
		
		[Test]
		public void Pages_OneHundredItemsAndPageSizeIsTenAndSelectablePagesIsFiveWithPageNineSelected_PagesSixToTenDisplayed()
		{
			CreatePages();
			pages.PageSize = 10;
			pages.MaximumSelectablePages = 5;
			pages.SelectedPageNumber = 9;
			pages.TotalItems = 100;
			
			Page[] expectedPages = new Page[] {
				new Page() { Number = 6 },
				new Page() { Number = 7 },
				new Page() { Number = 8 },
				new Page() { Number = 9, IsSelected = true },
				new Page() { Number = 10 },
			};
			
			PageCollectionAssert.AreEqual(expectedPages, pages);
		}
		
		[Test]
		public void HasPreviousPage_NoPagesAndSelectedPageIsPageTwo_ReturnsFalse()
		{
			CreatePages();
			pages.PageSize = 2;
			pages.TotalItems = 0;
			pages.SelectedPageNumber = 2;
			
			Assert.IsFalse(pages.HasPreviousPage);
		}
		
		[Test]
		public void HasNextPage_NoPagesAndSelectedPageIsPageOne_ReturnsFalse()
		{
			CreatePages();
			pages.PageSize = 2;
			pages.TotalItems = 0;
			pages.SelectedPageNumber = 1;
			
			Assert.IsFalse(pages.HasNextPage);
		}
	}
}
