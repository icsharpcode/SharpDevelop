// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class DefaultProjectTemplatePackageSourceTests
	{
		DefaultProjectTemplatePackageSource defaultPackageSource;
		
		FakePropertyService CreateFakePropertyService()
		{
			return new FakePropertyService();
		}
		
		void CreateDefaultPackageSource(FakePropertyService propertyService)
		{
			defaultPackageSource = new DefaultProjectTemplatePackageSource(propertyService);
		}
		
		[Test]
		public void PackageSource_PropertyServiceDefinesDataFolder_ReturnsDataTemplatePackagesFolder()
		{
			FakePropertyService propertyService = CreateFakePropertyService();
			propertyService.DataDirectory = @"d:\sharpdevelop\data";
			CreateDefaultPackageSource(propertyService);
			
			var packageSource = defaultPackageSource.PackageSource;
			
			string sourceDirectory = @"d:\sharpdevelop\data\templates\packages";
			var expectedPackageSource = new PackageSource(sourceDirectory, "Default");
			
			Assert.AreEqual(expectedPackageSource, packageSource);
		}
	}
}
