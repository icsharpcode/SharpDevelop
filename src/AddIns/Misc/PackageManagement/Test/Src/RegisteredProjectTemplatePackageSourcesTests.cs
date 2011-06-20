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
