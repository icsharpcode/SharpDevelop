// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class RegisteredTestFrameworksTestFixture
	{
		RegisteredTestFrameworks testFrameworks;
		MockTestFramework nunitTestFramework;
		MockTestFramework mbUnitTestFramework;
		
		[SetUp]
		public void Init()
		{
			List<TestFrameworkDescriptor> descriptors = new List<TestFrameworkDescriptor>();
			
			MockTestFrameworkFactory factory = new MockTestFrameworkFactory();
			
			Properties mbUnitProperties = new Properties();
			mbUnitProperties["id"] = "mbunit";
			mbUnitProperties["class"] = "MBUnitTestFramework";
			mbUnitProperties["supportedProjects"] = ".vbproj";
			mbUnitTestFramework = new MockTestFramework();
			factory.Add("MBUnitTestFramework", mbUnitTestFramework);
			
			Properties nunitProperties = new Properties();
			nunitProperties["id"] = "nunit";
			nunitProperties["class"] = "NUnitTestFramework";
			nunitProperties["supportedProjects"] = ".csproj";
			nunitTestFramework = new MockTestFramework();
			factory.Add("NUnitTestFramework", nunitTestFramework);
			
			TestFrameworkDescriptor mbUnitDescriptor = new TestFrameworkDescriptor(mbUnitProperties, factory);
			TestFrameworkDescriptor nunitDescriptor = new TestFrameworkDescriptor(nunitProperties, factory);
			
			descriptors.Add(mbUnitDescriptor);
			descriptors.Add(nunitDescriptor);
			
			MockAddInTree addinTree = new MockAddInTree();
			addinTree.AddItems("/SharpDevelop/UnitTesting/TestFrameworks", descriptors);
			
			testFrameworks = new RegisteredTestFrameworks(addinTree);
		}
		
		[Test]
		public void NUnitTestFrameworkRegisteredForUseWithProjectsWithCSharpProjectFileExtension()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test\MyProj.csproj";
			
			Assert.AreEqual(nunitTestFramework, testFrameworks.GetTestFrameworkForProject(project));
		}
		
		[Test]
		public void MbUnitTestFrameworkRegisteredForUseWithProjectsWithVBNetProjectFileExtension()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test\MyProj.vbproj";
			
			Assert.AreEqual(mbUnitTestFramework, testFrameworks.GetTestFrameworkForProject(project));
		}
		
		[Test]
		public void IsTestMethodReturnsFalseForUnknownMbUnitFrameworkTestMethod()
		{
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			method.MockDeclaringType.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.vbproj";
			
			Assert.IsFalse(testFrameworks.IsTestMethod(method));
		}
		
		[Test]
		public void IsTestMethodReturnsTrueForKnownMbUnitFrameworkTestMethod()
		{
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			method.MockDeclaringType.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.vbproj";
			
			mbUnitTestFramework.AddTestMethod(method);
			
			Assert.IsTrue(testFrameworks.IsTestMethod(method));
		}
		
		[Test]
		public void IsTestMethodDoesNotThrowNullReferenceExceptionWhenNoTestFrameworkSupportsProject()
		{
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			method.MockDeclaringType.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.unknown";
			
			Assert.IsFalse(testFrameworks.IsTestMethod(method));
		}
		
		[Test]
		public void IsTestMethodDoesNotThrowNullReferenceWhenNullPassedToMethod()
		{
			Assert.IsFalse(testFrameworks.IsTestMethod(null));
		}
		
		[Test]
		public void IsTestClassReturnsFalseForUnknownMbUnitFrameworkTestClass()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			c.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.vbproj";
			
			Assert.IsFalse(testFrameworks.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsTrueForKnownMbUnitFrameworkTestClass()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			c.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.vbproj";
			
			mbUnitTestFramework.AddTestClass(c);
			
			Assert.IsTrue(testFrameworks.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassDoesNotThrowNullReferenceExceptionWhenNoTestFrameworkSupportsProject()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			c.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.unknown";
			
			Assert.IsFalse(testFrameworks.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassDoesNotThrowNullReferenceWhenNullPassedToMethod()
		{
			Assert.IsFalse(testFrameworks.IsTestClass(null));
		}
		
		[Test]
		public void IsTestProjectReturnsFalseForUnknownMbUnitFrameworkTestProject()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.vbproj";
			
			Assert.IsFalse(testFrameworks.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsTrueForKnownMbUnitFrameworkTestProject()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.vbproj";
			
			mbUnitTestFramework.AddTestProject(project);
			
			Assert.IsTrue(testFrameworks.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectDoesNotThrowNullReferenceExceptionWhenNoTestFrameworkSupportsProject()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.unknown";
			
			Assert.IsFalse(testFrameworks.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectDoesNotThrowNullReferenceWhenNullPassedToMethod()
		{
			Assert.IsFalse(testFrameworks.IsTestProject(null));
		}
		
		[Test]
		public void CreateTestRunnerReturnsNewTestRunnerFromCorrectTestFramework()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.csproj";
			
			ITestRunner testRunner = testFrameworks.CreateTestRunner(project);
			ITestRunner[] expectedTestRunners = new ITestRunner[] { testRunner };
			Assert.AreEqual(expectedTestRunners, nunitTestFramework.TestRunnersCreated.ToArray());
		}
		
		[Test]
		public void CreateTestRunnerDoesNotThrowNullRefExceptionWhenUnknownProjectPassedToCreateTestRunnerMethod()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.unknown";
			
			Assert.IsNull(testFrameworks.CreateTestRunner(project));
		}
		
		[Test]
		public void CreateTestDebuggerReturnsNewTestRunnerFromCorrectTestFramework()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.csproj";
			
			ITestRunner testDebugger = testFrameworks.CreateTestDebugger(project);
			ITestRunner[] expectedTestRunners = new ITestRunner[] { testDebugger };
			Assert.AreEqual(expectedTestRunners, nunitTestFramework.TestDebuggersCreated.ToArray());
		}
		
		[Test]
		public void CreateTestDebuggerDoesNotThrowNullRefExceptionWhenUnknownProjectPassedToCreateTestRunnerMethod()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.unknown";
			
			Assert.IsNull(testFrameworks.CreateTestDebugger(project));
		}
		
		[Test]
		public void IsBuildNeededBeforeTestRunReturnsTrueWhenTestFrameworkIsBuildNeededBeforeTestRunSetToTrue()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.csproj";
			nunitTestFramework.IsBuildNeededBeforeTestRun = true;
			
			Assert.IsTrue(testFrameworks.IsBuildNeededBeforeTestRunForProject(project));
		}
		
		[Test]
		public void IsBuildNeededBeforeTestRunReturnsFalseWhenTestFrameworkIsBuildNeededBeforeTestRunSetToFalse()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.csproj";
			nunitTestFramework.IsBuildNeededBeforeTestRun = false;
			
			Assert.IsFalse(testFrameworks.IsBuildNeededBeforeTestRunForProject(project));
		}
	}
}
