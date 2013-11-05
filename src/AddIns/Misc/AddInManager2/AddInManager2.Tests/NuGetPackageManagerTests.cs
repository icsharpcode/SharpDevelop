// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
