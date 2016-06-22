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
using System.IO;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.AddInManager2.Tests.Fakes;
using NuGet;
using NUnit.Framework;

namespace ICSharpCode.AddInManager2.Tests
{
	[TestFixture]
	[Category("NuGetPackageManagerTests")]
	public class NuGetPackageManagerTests
	{
		AddInManagerEvents _events;
		FakePackageRepositories _packageRepositories;
		FakeSDAddInManagement _sdAddInManagement;
		NuGetPackageManager _nuGetPackageManager;
		
		public NuGetPackageManagerTests()
		{
		}
		
		[SetUp]
		public void SetUp()
		{
			_events = new AddInManagerEvents();
			_packageRepositories = new FakePackageRepositories();
			_sdAddInManagement = new FakeSDAddInManagement();
		}
			
		private void SetupNuGetPackageManager()
		{
			_nuGetPackageManager = new NuGetPackageManager(_packageRepositories, _events, _sdAddInManagement);
		}
		
		[Test]
		public void GetLocalPackageDirectoryForFullVersion()
		{
			// Create a fake package
			FakePackage fakePackage = new FakePackage()
			{
				Id = "NuGet.TestPackage",
				Version = new SemanticVersion("1.0.0.0")
			};
			
			_sdAddInManagement.ConfigDirectory = @"C:\ConfigDir";
			SetupNuGetPackageManager();
			
			string expectedPackageDir = @"C:\ConfigDir\NuGet\NuGet.TestPackage.1.0.0.0";
			
			Assert.That(_nuGetPackageManager.GetLocalPackageDirectory(fakePackage), Is.EqualTo(expectedPackageDir));
		}
		
		[Test]
		public void GetLocalPackageDirectoryForShortVersion()
		{
			// Create a fake package
			FakePackage fakePackage = new FakePackage()
			{
				Id = "NuGet.TestPackage",
				Version = new SemanticVersion("1.0")
			};
			
			_sdAddInManagement.ConfigDirectory = @"C:\ConfigDir";
			SetupNuGetPackageManager();
			
			string expectedPackageDir = @"C:\ConfigDir\NuGet\NuGet.TestPackage.1.0";
			
			Assert.That(_nuGetPackageManager.GetLocalPackageDirectory(fakePackage), Is.EqualTo(expectedPackageDir));
		}
	}
}
