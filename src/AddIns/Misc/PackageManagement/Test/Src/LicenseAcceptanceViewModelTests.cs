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
