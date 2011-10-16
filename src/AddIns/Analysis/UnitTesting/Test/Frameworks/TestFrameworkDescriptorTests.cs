// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class TestFrameworkDescriptorTests
	{
		TestFrameworkDescriptor descriptor;
		MockTestFrameworkFactory fakeTestFrameworkFactory;
		MockTestFramework fakeTestFramework;
		
		void CreateTestFrameworkDescriptorToSupportCSharpProjects()
		{
			CreateTestFrameworkDescriptorToSupportProjectFileExtensions(".csproj");
		}
		
		void CreateTestFrameworkDescriptorToSupportProjectFileExtensions(string fileExtensions)
		{
			Properties properties = new Properties();
			properties["id"] = "nunit";
			properties["class"] = "NUnitTestFramework";
			properties["supportedProjects"] = ".csproj;.vbproj";
			
			fakeTestFrameworkFactory = new MockTestFrameworkFactory();
			fakeTestFramework = new MockTestFramework();
			fakeTestFrameworkFactory.Add("NUnitTestFramework", fakeTestFramework);
			
			descriptor = new TestFrameworkDescriptor(properties, fakeTestFrameworkFactory);
		}
		
		MockCSharpProject CreateCSharpProjectNotSupportedByTestFramework()
		{
			return new MockCSharpProject();
		}
		
		MockCSharpProject CreateCSharpProjectSupportedByTestFramework()
		{
			var project = new MockCSharpProject();
			fakeTestFramework.AddTestProject(project);
			return project;
		}
		
		[Test]
		public void IsSupportedProject_CSharpProjectIsSupportedByFileExtensionButNotByTestFramework_ReturnsFalse()
		{
			CreateTestFrameworkDescriptorToSupportCSharpProjects();
			MockCSharpProject project = CreateCSharpProjectNotSupportedByTestFramework();
			bool supported = descriptor.IsSupportedProject(project);
			
			Assert.IsFalse(supported);
		}
		
		[Test]
		public void IsSupportedProject_CSharpProjectIsSupportedByFileExtensionAndByTestFramework_ReturnsTrue()
		{
			CreateTestFrameworkDescriptorToSupportCSharpProjects();
			MockCSharpProject project = CreateCSharpProjectSupportedByTestFramework();
			bool supported = descriptor.IsSupportedProject(project);
			
			Assert.IsTrue(supported);
		}
	}
}
