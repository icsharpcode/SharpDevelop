// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;
using Rhino.Mocks;
using ICSharpCode.SharpDevelop;

namespace UnitTesting.Tests.Service
{
	[TestFixture]
	public class TestFrameworkDescriptorTests : SDTestFixtureBase
	{
		TestFrameworkDescriptor descriptor;
		ITestFramework fakeTestFramework;
		
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
			
			descriptor = new TestFrameworkDescriptor(properties, _ => fakeTestFramework);
		}
		
		MockCSharpProject CreateCSharpProjectNotSupportedByTestFramework()
		{
			var project = new MockCSharpProject();
			project.FileName = FileName.Create(@"d:\projects\MyProject\MyProject.csproj");
			fakeTestFramework = MockRepository.GenerateStrictMock<ITestFramework>();
			fakeTestFramework.Stub(f => f.IsTestProject(project)).Return(false);
			return project;
		}
		
		MockCSharpProject CreateCSharpProjectSupportedByTestFramework()
		{
			MockCSharpProject project = CreateCSharpProjectNotSupportedByTestFramework();
			fakeTestFramework = MockRepository.GenerateStrictMock<ITestFramework>();
			fakeTestFramework.Stub(f => f.IsTestProject(project)).Return(true);
			return project;
		}
		
		MockCSharpProject CreateVisualBasicProjectSupportedByTestFramework()
		{
			MockCSharpProject project = CreateCSharpProjectSupportedByTestFramework();
			project.FileName = FileName.Create(@"d:\projects\MyProject\MyProject.vbproj");
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
		public void IsSupportedProject_CSharpAndVisualBasicProjectsSupportedByDescriptor_ReturnsTrueForVBProject()
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
		public void IsSupportedProject_SupportedProjectFileExtensionsInDescriptorContainWhitespace_ReturnsTrueForVBProject()
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
		public void IsSupportedProject_SupportedProjectFileExtensionInDescriptorAreInUpperCase_ReturnsTrueForVBProject()
		{
			CreateTestFrameworkDescriptorToSupportProjectFileExtensions(".CSPROJ;.VBPROJ");
			MockCSharpProject project = CreateVisualBasicProjectSupportedByTestFramework();
			
			bool supported = descriptor.IsSupportedProject(project);
			
			Assert.IsTrue(supported);
		}
		
		[Test]
		public void IsSupportedProject_DescriptorSupportsCSharpProjects_ReturnsFalseForVBProject()
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
			project.FileName = FileName.Create(@"d:\projects\MyProject\MyProject.CSPROJ");
			
			bool supported = descriptor.IsSupportedProject(project);
			
			Assert.IsTrue(supported);
		}
	}
}
