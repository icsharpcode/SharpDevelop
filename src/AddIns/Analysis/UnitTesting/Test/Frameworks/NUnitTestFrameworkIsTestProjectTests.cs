// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class NUnitTestFrameworkIsTestProjectTests
	{
		NUnitTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new NUnitTestFramework();
		}
		
		[Test]
		public void NUnitTestFrameworkImplementsITestFramework()
		{
			Assert.IsNotNull(testFramework as ITestFramework);
		}
		
		[Test]
		public void IsTestProjectReturnsFalseForNullProject()
		{
			Assert.IsFalse(testFramework.IsTestProject(null));
		}
		
		[Test]
		public void IsTestProjectReturnsTrueForProjectWithNUnitFrameworkAssemblyReference()
		{
			MockCSharpProject project = new MockCSharpProject();
			
			ReferenceProjectItem systemRef = new ReferenceProjectItem(project, "System");
			ProjectService.AddProjectItem(project, systemRef);
			
			ReferenceProjectItem nunitFrameworkRef = new ReferenceProjectItem(project, "NUnit.Framework");
			ProjectService.AddProjectItem(project, nunitFrameworkRef);
			
			Assert.IsTrue(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsFalseForProjectWithoutNUnitFrameworkAssemblyReference()
		{
			MockCSharpProject project = new MockCSharpProject();
			Assert.IsFalse(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsTrueForProjectWithNUnitFrameworkAssemblyReferenceIgnoringCase()
		{
			MockCSharpProject project = new MockCSharpProject();
			
			ReferenceProjectItem nunitFrameworkRef = new ReferenceProjectItem(project, "NUNIT.FRAMEWORK");
			ProjectService.AddProjectItem(project, nunitFrameworkRef);
			
			Assert.IsTrue(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsTrueForProjectWithNUnitFrameworkAssemblyReferenceIgnoringNonReferenceProjectItems()
		{
			MockCSharpProject project = new MockCSharpProject();
			
			FileProjectItem fileItem = new FileProjectItem(project, ItemType.Compile, "test.cs");
			ProjectService.AddProjectItem(project, fileItem);
			
			ReferenceProjectItem nunitFrameworkRef = new ReferenceProjectItem(project, "nunit.framework");
			ProjectService.AddProjectItem(project, nunitFrameworkRef);
			
			Assert.IsTrue(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsTrueForProjectWithNUnitFrameworkAssemblyReferenceUsingFullName()
		{
			MockCSharpProject project = new MockCSharpProject();
			string assemblyName = "nunit.framework, Version=2.5.3.9345, Culture=neutral, PublicKeyToken=96d09a1eb7f44a77";
			ReferenceProjectItem nunitFrameworkRef = new ReferenceProjectItem(project, assemblyName);
			ProjectService.AddProjectItem(project, nunitFrameworkRef);
			
			Assert.IsTrue(testFramework.IsTestProject(project));
		}
	}
}
