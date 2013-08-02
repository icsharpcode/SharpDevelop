// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class LicenseAcceptanceViewModelTests
	{
		LicenseAcceptanceViewModel viewModel;
		List<FakePackage> packages;
		
		[SetUp]
		public void Init()
		{
			packages = new List<FakePackage>();
		}
		
		void CreateViewModel(IEnumerable<IPackage> packages)
		{
			viewModel = new LicenseAcceptanceViewModel(packages);
		}
		
		void CreateViewModelWithNoPackages()
		{
			CreateViewModel(packages);
		}
		
		void CreateViewModelWithOnePackage()
		{
			AddOneFakePackage();
			CreateViewModel(packages);
		}
		
		void CreateViewModelWithTwoPackages()
		{
			AddOneFakePackage();
			AddOneFakePackage();
			CreateViewModel(packages);
		}
		
		void AddOneFakePackage()
		{
			FakePackage package = CreatePackage();
			packages.Add(package);
		}
		
		FakePackage CreatePackage()
		{
			return new FakePackage() {
				Id = "Test",
				Description = "Description",
				LicenseUrl = new Uri("http://sharpdevelop.codeplex.com")
			};
		}
		
		[Test]
		public void Packages_NewInstanceCreatedWithOnePackage_ContainsOnePackage()
		{
			CreateViewModelWithOnePackage();
			
			List<PackageLicenseViewModel> packageViewModels = viewModel.Packages.ToList();
			PackageLicenseViewModel firstPackageViewModel = packageViewModels.FirstOrDefault();
			
			FakePackage expectedPackage = packages[0];
			Assert.AreEqual(1, packageViewModels.Count);
			Assert.AreEqual(expectedPackage.Id, firstPackageViewModel.Id);
			Assert.AreEqual(expectedPackage.LicenseUrl, firstPackageViewModel.LicenseUrl);
			Assert.AreEqual(expectedPackage.SummaryOrDescription(), firstPackageViewModel.Summary);
		}
		
		[Test]
		public void HasOnePackage_OnePackage_ReturnsTrue()
		{
			CreateViewModelWithOnePackage();
			
			Assert.IsTrue(viewModel.HasOnePackage);
		}
		
		[Test]
		public void HasOnePackage_NoPackages_ReturnsFalse()
		{
			CreateViewModelWithNoPackages();
			
			Assert.IsFalse(viewModel.HasOnePackage);
		}
		
		[Test]
		public void HasOnePackage_TwoPackages_ReturnsFalse()
		{
			CreateViewModelWithTwoPackages();
			
			Assert.IsFalse(viewModel.HasOnePackage);
		}
		
		[Test]
		public void HasMultiplePackages_TwoPackages_ReturnsTrue()
		{
			CreateViewModelWithTwoPackages();
			
			Assert.IsTrue(viewModel.HasMultiplePackages);
		}
		
		[Test]
		public void HasMultiplePackages_OnePackage_ReturnsFalse()
		{
			CreateViewModelWithOnePackage();
			
			Assert.IsFalse(viewModel.HasMultiplePackages);
		}
	}
}
