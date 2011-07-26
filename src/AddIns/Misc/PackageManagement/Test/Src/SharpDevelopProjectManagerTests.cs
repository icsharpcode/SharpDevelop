// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class SharpDevelopProjectManagerTests
	{
		TestableProjectManager projectManager;
		
		void CreateProjectManager()
		{
			projectManager = new TestableProjectManager();
		}
		
		void AddFakePackageToProjectLocalRepository(string packageId, string version)
		{
			projectManager.AddFakePackageToProjectLocalRepository(packageId, version);
		}
		
		[Test]
		public void IsInstalled_PackageIdPassedThatDoesNotExistInProjectLocalRepository_ReturnsFalse()
		{
			CreateProjectManager();
		
			bool installed = projectManager.IsInstalled("Test");
			
			Assert.IsFalse(installed);
		}
		
		[Test]
		public void IsInstalled_PackageIdPassedExistsInProjectLocalRepository_ReturnsTrue()
		{
			CreateProjectManager();
			projectManager.AddFakePackageToProjectLocalRepository("Test", "1.0.2");
		
			bool installed = projectManager.IsInstalled("Test");
			
			Assert.IsTrue(installed);
		}
	}
}
