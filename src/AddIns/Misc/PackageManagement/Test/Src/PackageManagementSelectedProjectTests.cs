// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageManagementSelectedProjectTests
	{
		PackageManagementSelectedProject selectedProject;
		FakePackageManagementProject fakeProject;
		
		void CreateFakePackageManagementProject()
		{
			fakeProject = new FakePackageManagementProject();
		}
		
		void CreateSelectedProject(FakePackageManagementProject fakeProject)
		{
			selectedProject = new PackageManagementSelectedProject(fakeProject);
		}
		
		[Test]
		public void Name_PackageManagementProjectNameIsTest_ReturnsTest()
		{
			CreateFakePackageManagementProject();
			CreateSelectedProject(fakeProject);
			fakeProject.Name = "Test";
			
			string name = selectedProject.Name;
			
			Assert.AreEqual("Test", name);
		}		
	}
}
