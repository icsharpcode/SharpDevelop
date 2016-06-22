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
