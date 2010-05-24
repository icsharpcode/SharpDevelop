// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using Gallio.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace Gallio.SharpDevelop.Tests
{
	[TestFixture]
	public class GallioTestFrameworkIsTestProjectTests
	{
		GallioTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new GallioTestFramework();
		}
		
		[Test]
		public void GallioTestFrameworkImplementsITestFramework()
		{
			Assert.IsNotNull(testFramework as ITestFramework);
		}
		
		[Test]
		public void IsTestProjectReturnsFalseForNullProject()
		{
			Assert.IsFalse(testFramework.IsTestProject(null));
		}
		
		[Test]
		public void IsTestProjectReturnsTrueForProjectWithMbUnitFrameworkAssemblyReference()
		{
			MockCSharpProject project = new MockCSharpProject();
			
			ReferenceProjectItem systemRef = new ReferenceProjectItem(project, "System");
			ProjectService.AddProjectItem(project, systemRef);
			
			ReferenceProjectItem nunitFrameworkRef = new ReferenceProjectItem(project, "MbUnit");
			ProjectService.AddProjectItem(project, nunitFrameworkRef);
			
			Assert.IsTrue(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsFalseForProjectWithoutMbUnitFrameworkAssemblyReference()
		{
			MockCSharpProject project = new MockCSharpProject();
			Assert.IsFalse(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsTrueForProjectWithMbUnitFrameworkAssemblyReferenceIgnoringCase()
		{
			MockCSharpProject project = new MockCSharpProject();
			
			ReferenceProjectItem nunitFrameworkRef = new ReferenceProjectItem(project, "MBUNIT");
			ProjectService.AddProjectItem(project, nunitFrameworkRef);
			
			Assert.IsTrue(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsTrueForProjectWithMbUnitFrameworkAssemblyReferenceIgnoringNonReferenceProjectItems()
		{
			MockCSharpProject project = new MockCSharpProject();
			
			FileProjectItem fileItem = new FileProjectItem(project, ItemType.Compile, "test.cs");
			ProjectService.AddProjectItem(project, fileItem);
			
			ReferenceProjectItem nunitFrameworkRef = new ReferenceProjectItem(project, "mbunit");
			ProjectService.AddProjectItem(project, nunitFrameworkRef);
			
			Assert.IsTrue(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsTrueForProjectWithMbUnitFrameworkAssemblyReferenceUsingFullName()
		{
			MockCSharpProject project = new MockCSharpProject();
			string assemblyName = "mbunit, Version=2.5.3.9345, Culture=neutral, PublicKeyToken=96d09a1eb7f44a77";
			ReferenceProjectItem mbunitFrameworkRef = new ReferenceProjectItem(project, assemblyName);
			ProjectService.AddProjectItem(project, mbunitFrameworkRef);
			
			Assert.IsTrue(testFramework.IsTestProject(project));
		}
	}
}
