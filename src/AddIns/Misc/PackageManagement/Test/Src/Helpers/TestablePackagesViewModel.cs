// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class TestablePackagesViewModel : PackagesViewModel
	{
		public FakePackageManagementService FakePackageManagementService;
		public FakeTaskFactory FakeTaskFactory;
		public List<FakePackage> FakePackages = new List<FakePackage>();
		public int GetAllPackagesCallCount;
		public int PageCountBeforePackagesFiltered;
		public string SearchCriteriaPassedToFilterPackagesBySearchCriteria;
		
		public TestablePackagesViewModel()
			: this(new FakePackageManagementService())
		{
		}
		
		public TestablePackagesViewModel(FakePackageManagementService packageManagementService)
			: this(packageManagementService, new FakeTaskFactory())
		{
		}
		
		public TestablePackagesViewModel(FakePackageManagementService packageManagementService, FakeTaskFactory taskFactory)
			: base(packageManagementService, new FakeMessageReporter(), taskFactory)
		{
			FakePackageManagementService = packageManagementService;
			FakeTaskFactory = taskFactory;
		}
		
		public void AddOneFakePackage()
		{
			AddFakePackage("Test");
		}
		
		public void AddFakePackage(string packageId)
		{
			FakePackage package = CreateFakePackage(packageId);
			FakePackages.Add(package);
		}
		
		FakePackage CreateFakePackage(string packageId)
		{
			return new FakePackage() {
				Id = packageId,
				Description = ""
			};
		}
		
		public void AddSixFakePackages()
		{
			AddFakePackages(howMany: 6);
		}
		
		void AddFakePackages(int howMany)
		{
			for (int i = 1; i <= howMany; ++i) {
				AddFakePackage("Test " + i);
			}
		}
		
		protected override IQueryable<NuGet.IPackage> GetAllPackages()
		{
			GetAllPackagesCallCount++;
			return FakePackages.AsQueryable();
		}
		
		protected override IEnumerable<IPackage> GetFilteredPackagesBeforePagingResults(IQueryable<IPackage> packages)
		{
			PageCountBeforePackagesFiltered = Pages.Count;
			return base.GetFilteredPackagesBeforePagingResults(packages);
		}
		
		public void AddTenFakePackages()
		{
			AddFakePackages(howMany: 10);
		}
		
		public void AddThreeFakePackages()
		{
			AddFakePackages(howMany: 3);
		}
		
		protected override IQueryable<IPackage> FilterPackagesBySearchCriteria(IQueryable<IPackage> packages, string searchTerms)
		{
			SearchCriteriaPassedToFilterPackagesBySearchCriteria = searchTerms;
			return base.FilterPackagesBySearchCriteria(packages, searchTerms);
		}
	}
}
