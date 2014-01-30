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
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class RegisteredProjectTemplatePackageSourcesTests
	{
		RegisteredProjectTemplatePackageSources registeredPackageSources;
		FakeSettingsFactory fakeSettingsFactory;
		
		FakePropertyService CreatePropertyService()
		{
			return new FakePropertyService();
		}
		
		void CreateRegisteredPackageSources(List<PackageSource> packageSources, FakePropertyService propertyService)
		{
			fakeSettingsFactory = new FakeSettingsFactory();
			fakeSettingsFactory.FakeSettings.AddFakePackageSources(packageSources);
			registeredPackageSources = 
				new RegisteredProjectTemplatePackageSources(
					propertyService,
					fakeSettingsFactory);
		}
		
		[Test]
		public void PackageSources_NoPredefinedPackageSources_DefaultPackageSourceCreated()
		{
			FakePropertyService propertyService = CreatePropertyService();
			propertyService.DataDirectory = @"d:\sharpdevelop\data";
			
			var packageSources = new List<PackageSource>();
			CreateRegisteredPackageSources(packageSources, propertyService);
			
			RegisteredPackageSources actualPackageSources = 
				registeredPackageSources.PackageSources;
			
			var expectedPackageSources = new PackageSource[] {
				new PackageSource(@"d:\sharpdevelop\data\templates\packages", "Default")
			};
			
			PackageSourceCollectionAssert.AreEqual(expectedPackageSources, actualPackageSources);
		}
		
		[Test]
		public void PackageSources_OnePredefinedPackageSource_RegisteredPackageSourceIsPredefinedPackageSource()
		{
			FakePropertyService propertyService = CreatePropertyService();
			propertyService.DataDirectory = @"d:\sharpdevelop\data";
			var expectedPackageSources = new List<PackageSource>();
			expectedPackageSources.Add(new PackageSource("http://sharpdevelop", "Test"));
			CreateRegisteredPackageSources(expectedPackageSources, propertyService);
			
			RegisteredPackageSources actualPackageSources = 
				registeredPackageSources.PackageSources;
				
			PackageSourceCollectionAssert.AreEqual(expectedPackageSources, actualPackageSources);
		}
		
		[Test]
		public void PackageSources_NoPredefinedPackageSources_PackageSourceConfigLookedForInUserFolder()
		{
			FakePropertyService propertyService = CreatePropertyService();
			propertyService.DataDirectory = @"d:\sharpdevelop\data";
			propertyService.ConfigDirectory = @"c:\Users\test\AppData\ICSharpCode\SharpDevelop4.1";
			
			var packageSources = new List<PackageSource>();
			CreateRegisteredPackageSources(packageSources, propertyService);
			
			IEnumerable<PackageSource> actualPackageSources =
				registeredPackageSources.PackageSources;
			
			string directory = fakeSettingsFactory.DirectoryPassedToCreateSettings;
			string expectedDirectory = @"c:\Users\test\AppData\ICSharpCode\SharpDevelop4.1\templates";
			
			Assert.AreEqual(expectedDirectory, directory);
		}
	}
}
