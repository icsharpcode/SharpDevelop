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
			properties["supportedProjects"] = fileExtensions;
			
			fakeTestFrameworkFactory = new MockTestFrameworkFactory();
			fakeTestFramework = new MockTestFramework();
			fakeTestFrameworkFactory.Add("NUnitTestFramework", fakeTestFramework);
			
			descriptor = new TestFrameworkDescriptor(properties, fakeTestFrameworkFactory);
		}
		
		MockCSharpProject CreateCSharpProjectNotSupportedByTestFramework()
		{
			var project = new MockCSharpProject();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			return project;
		}
		
		MockCSharpProject CreateCSharpProjectSupportedByTestFramework()
		{
			MockCSharpProject project = CreateCSharpProjectNotSupportedByTestFramework();
			fakeTestFramework.AddTestProject(project);
			return project;
		}
		
		MockCSharpProject CreateVisualBasicProjectSupportedByTestFramework()
		{
			MockCSharpProject project = CreateCSharpProjectSupportedByTestFramework();
			project.FileName = @"d:\projects\MyProject\MyProject.vbproj";
			return project;
		}
		
		[Test]
		public void IsSupportedProject_CSharpAndVisualBasicProjectsSupportedByDescriptor_ReturnsTrueForCSharpProject()
		{
			CreateTestFrameworkDescriptorToSupportProjectFileExtensions(".csproj;.vbproj");
			MockCSharpProject project = CreateCSharpProjectSupportedByTestFramework();
			
			bool supported = descriptor.IsSupportedProject(project);
			
			Assert.IsTrue(supported);
		}
		
		[Test]
		public void IsSupportedProject_CSharpAndVisualBasicProjectsSupportedByDescriptor_ReturnsTrueForVBNetProject()
		{
			CreateTestFrameworkDescriptorToSupportProjectFileExtensions(".csproj;.vbproj");
			MockCSharpProject project = CreateVisualBasicProjectSupportedByTestFramework();
			
			bool supported = descriptor.IsSupportedProject(project);
			
			Assert.IsTrue(supported);
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
		
		[Test]
		public void IsSupportedProject_SupportedProjectFileExtensionsInDescriptorContainWhitespace_ReturnsTrueForCSharpProject()
		{
			CreateTestFrameworkDescriptorToSupportProjectFileExtensions("  .csproj;  .vbproj  ");
			MockCSharpProject project = CreateCSharpProjectSupportedByTestFramework();
			
			bool supported = descriptor.IsSupportedProject(project);
			
			Assert.IsTrue(supported);
		}
		
		[Test]
		public void IsSupportedProject_SupportedProjectFileExtensionsInDescriptorContainWhitespace_ReturnsTrueForVBNetProject()
		{
			CreateTestFrameworkDescriptorToSupportProjectFileExtensions("  .csproj;  .vbproj  ");
			MockCSharpProject project = CreateVisualBasicProjectSupportedByTestFramework();
			
			bool supported = descriptor.IsSupportedProject(project);
			
			Assert.IsTrue(supported);
		}
		
		[Test]
		public void IsSupportedProject_SupportedProjectFileExtensionsInDescriptorAreInUpperCase_ReturnsTrueForCSharpProject()
		{
			CreateTestFrameworkDescriptorToSupportProjectFileExtensions(".CSPROJ;.VBPROJ");
			MockCSharpProject project = CreateCSharpProjectSupportedByTestFramework();
			
			bool supported = descriptor.IsSupportedProject(project);
			
			Assert.IsTrue(supported);
		}
		
		[Test]
		public void IsSupportedProject_SupportedProjectFileExtensionInDescriptorAreInUpperCase_ReturnsTrueForVBNetProject()
		{
			CreateTestFrameworkDescriptorToSupportProjectFileExtensions(".CSPROJ;.VBPROJ");
			MockCSharpProject project = CreateVisualBasicProjectSupportedByTestFramework();
			
			bool supported = descriptor.IsSupportedProject(project);
			
			Assert.IsTrue(supported);
		}
		
		[Test]
		public void IsSupportedProject_DescriptorSupportsCSharpProjects_ReturnsFalseForVBNetProject()
		{
			CreateTestFrameworkDescriptorToSupportProjectFileExtensions(".csproj");
			MockCSharpProject project = CreateVisualBasicProjectSupportedByTestFramework();
			
			bool supported = descriptor.IsSupportedProject(project);
			
			Assert.IsFalse(supported);
		}
		
		[Test]
		public void IsSupportedProject_NullProjectPassed_ReturnsFalse()
		{
			CreateTestFrameworkDescriptorToSupportProjectFileExtensions(".csproj");
			bool supported = descriptor.IsSupportedProject(null);
			
			Assert.IsFalse(supported);
		}
		
		[Test]
		public void IsSupportedProject_DescriptorSupportsCSharpProjects_ReturnsTrueForCSharpProjectFileExtensionInUpperCase()
		{
			CreateTestFrameworkDescriptorToSupportProjectFileExtensions(".csproj");
			MockCSharpProject project = CreateCSharpProjectSupportedByTestFramework();
			project.FileName = @"d:\projects\MyProject\MyProject.CSPROJ";
			
			bool supported = descriptor.IsSupportedProject(project);
			
			Assert.IsTrue(supported);
		}
	}
}
