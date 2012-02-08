// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class RegisteredTestFrameworksTests
	{
		MockAddInTree addinTree;
		RegisteredTestFrameworks testFrameworks;
		MockTestFramework nunitTestFramework;
		MockTestFramework mbUnitTestFramework;
		List<TestFrameworkDescriptor> testFrameworkDescriptors;
		MockTestFrameworkFactory testFrameworkFactory;
		
		[SetUp]
		public void Init()
		{
			CreateAddInTree();
			RegisterMbUnitFrameworkSupportingVisualBasicProjects();
			RegisterNUnitFrameworkSupportingCSharpProjects();
			CreateRegisteredTestFrameworks();
		}

		void CreateAddInTree()
		{
			testFrameworkFactory = new MockTestFrameworkFactory();
			
			addinTree = new MockAddInTree();
			testFrameworkDescriptors = new List<TestFrameworkDescriptor>();
			addinTree.AddItems("/SharpDevelop/UnitTesting/TestFrameworks", testFrameworkDescriptors);			
		}
		
		void RegisterMbUnitFrameworkSupportingVisualBasicProjects()
		{
			mbUnitTestFramework = RegisterTestFramework(
				className: "MBUnitTestFramework",
				supportedProjects: ".vbproj");
		}
			
		void RegisterNUnitFrameworkSupportingCSharpProjects()
		{
			nunitTestFramework = RegisterTestFramework(
				className: "NUnitTestFramework",
				supportedProjects: ".csproj");
		}
		
		MockTestFramework RegisterTestFramework(string className, string supportedProjects)
		{
			MockTestFramework testFramework = testFrameworkFactory.AddFakeTestFramework(className);
			
			Properties properties = new Properties();
			properties["class"] = className;
			properties["supportedProjects"] = supportedProjects;
			
			TestFrameworkDescriptor descriptor = new TestFrameworkDescriptor(properties, testFrameworkFactory);
			testFrameworkDescriptors.Add(descriptor);
			
			return testFramework;
		}
		
		void CreateRegisteredTestFrameworks()
		{
			testFrameworks = new RegisteredTestFrameworks(addinTree);
		}
		
		[Test]
		public void GetTestFrameworkForProject_NUnitTestFrameworkRegisteredForUseWithProjectsWithCSharpProjectFileExtension_ReturnsNUnitTestFramework()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test\MyProj.csproj";
			nunitTestFramework.AddTestProject(project);
			
			ITestFramework testFramework = testFrameworks.GetTestFrameworkForProject(project);
			
			Assert.AreEqual(nunitTestFramework, testFramework);
		}
		
		[Test]
		public void GetTestFrameworkForProject_MbUnitTestFrameworkRegisteredForUseWithProjectsWithVBNetProjectFileExtension_ReturnsMBUnitTestFramework()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test\MyProj.vbproj";
			mbUnitTestFramework.AddTestProject(project);
			
			ITestFramework testFramework = testFrameworks.GetTestFrameworkForProject(project);
			
			Assert.AreEqual(mbUnitTestFramework, testFramework);
		}
		
		[Test]
		public void IsTestMember_UnknownMbUnitFrameworkTestMethod_ReturnsFalse()
		{
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			IProject project = method.MockDeclaringType.Project;
			mbUnitTestFramework.AddTestProject(project);
			project.FileName = @"d:\projects\test.vbproj";
			
			bool result = testFrameworks.IsTestMember(method);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_KnownMbUnitFrameworkTestMethod_ReturnsTrue()
		{
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			IProject project = method.MockDeclaringType.Project;
			mbUnitTestFramework.AddTestProject(project);
			method.MockDeclaringType.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.vbproj";
			
			mbUnitTestFramework.AddTestMember(method);
			
			bool result = testFrameworks.IsTestMember(method);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestMember_NoTestFrameworkSupportsProject_DoesNotThrowNullReferenceException()
		{
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			method.MockDeclaringType.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.unknown";
			
			bool result = testFrameworks.IsTestMember(method);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_NullPassedToMethod_DoesNotThrowException()
		{
			bool result = testFrameworks.IsTestMember(null);

			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestClass_UnknownMbUnitFrameworkTestClass_ReturnsFalse()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			mbUnitTestFramework.AddTestProject(c.Project);
			c.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.vbproj";
			
			bool result = testFrameworks.IsTestClass(c);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestClass_KnownMbUnitFrameworkTestClass_ReturnsTrue()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			
			c.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.vbproj";
			
			mbUnitTestFramework.AddTestProject(c.Project);
			mbUnitTestFramework.AddTestClass(c);
			
			bool result = testFrameworks.IsTestClass(c);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestClass_NoTestFrameworkSupportsProject_DoesNotThrowNullReferenceException()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			c.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.unknown";
			
			bool result = testFrameworks.IsTestClass(c);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestClass_NullPassedToMethod_DoesNotThrowNullReference()
		{
			bool result = testFrameworks.IsTestClass(null);
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestProject_UnknownMbUnitFrameworkTestProject_ReturnsFalse()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.vbproj";
			
			bool result = testFrameworks.IsTestProject(project);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestProject_KnownMbUnitFrameworkTestProject_ReturnsTrue()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.vbproj";
			
			mbUnitTestFramework.AddTestProject(project);
			
			bool result = testFrameworks.IsTestProject(project);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestProject_NoTestFrameworkSupportsProject_DoesNotThrowNullReferenceException()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.unknown";
			
			bool result = testFrameworks.IsTestProject(project);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestProject_NullPassedToMethod_DoesNotThrowNullReference()
		{
			bool result = testFrameworks.IsTestProject(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void CreateTestRunner_CSharpProject_ReturnsNewTestRunnerFromNUnitTestFramework()
		{
			MockCSharpProject project = new MockCSharpProject();
			nunitTestFramework.AddTestProject(project);
			mbUnitTestFramework.AddTestProject(project);
			project.FileName = @"d:\projects\test.csproj";
			
			ITestRunner testRunner = testFrameworks.CreateTestRunner(project);
			ITestRunner[] expectedTestRunners = new ITestRunner[] { testRunner };
			
			MockTestRunner[] testRunnersCreated = nunitTestFramework.TestRunnersCreated.ToArray();
			
			Assert.AreEqual(expectedTestRunners, testRunnersCreated);
		}
		
		[Test]
		public void CreateTestRunner_UnknownProjectPassedToCreateTestRunnerMethod_DoesNotThrowNullReferenceException()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.unknown";
			
			ITestRunner testRunner = testFrameworks.CreateTestRunner(project);
			
			Assert.IsNull(testRunner);
		}
		
		[Test]
		public void CreateTestDebugger_CSharpProject_ReturnsNewTestRunnerFromNUnitTestFramework()
		{
			MockCSharpProject project = new MockCSharpProject();
			nunitTestFramework.AddTestProject(project);
			mbUnitTestFramework.AddTestProject(project);
			project.FileName = @"d:\projects\test.csproj";
			
			ITestRunner testDebugger = testFrameworks.CreateTestDebugger(project);
			ITestRunner[] expectedTestRunners = new ITestRunner[] { testDebugger };
			
			MockTestRunner[] testRunnersCreated = nunitTestFramework.TestDebuggersCreated.ToArray();
			
			Assert.AreEqual(expectedTestRunners, testRunnersCreated);
		}
		
		[Test]
		public void CreateTestDebugger_UnknownProjectPassedToCreateTestRunnerMethod_DoesNotThrowNullReferenceException()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.unknown";
			
			ITestRunner testRunner = testFrameworks.CreateTestDebugger(project);
			
			Assert.IsNull(testRunner);
		}
		
		[Test]
		public void IsBuildNeededBeforeTestRun_TestFrameworkNeedsBuildBeforeTestRun_ReturnsTrue()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.csproj";
			nunitTestFramework.IsBuildNeededBeforeTestRun = true;
			
			bool result = testFrameworks.IsBuildNeededBeforeTestRunForProject(project);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsBuildNeededBeforeTestRun_TestFrameworkDoesRequireBuildBeforeTestRun_ReturnsFalse()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.csproj";
			nunitTestFramework.IsBuildNeededBeforeTestRun = false;
			nunitTestFramework.AddTestProject(project);
			
			bool result = testFrameworks.IsBuildNeededBeforeTestRunForProject(project);
			
			Assert.IsFalse(result);
		}
	}
}
