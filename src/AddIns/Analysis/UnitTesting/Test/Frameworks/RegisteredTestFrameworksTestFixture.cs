// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		MockCSharpProject project;
		MockMethod method;
		MockClass clazz;
		
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
			
			project = new MockCSharpProject();
			nunitTestFramework.AddTestProject(project);
			mbUnitTestFramework.AddTestProject(project);

			method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			method.MockDeclaringType.MockProjectContent.Project = project;
			
			clazz = MockClass.CreateMockClassWithoutAnyAttributes();
			clazz.MockProjectContent.Project = project;
		}
		
		[Test]
		public void NUnitTestFrameworkRegisteredForUseWithProjectsWithCSharpProjectFileExtension()
		{
			project.FileName = @"d:\projects\test\MyProj.csproj";
			
			Assert.AreEqual(nunitTestFramework, testFrameworks.GetTestFrameworkForProject(project));
		}
		
		[Test]
		public void MbUnitTestFrameworkRegisteredForUseWithProjectsWithVBNetProjectFileExtension()
		{
			project.FileName = @"d:\projects\test\MyProj.vbproj";
			
			Assert.AreEqual(mbUnitTestFramework, testFrameworks.GetTestFrameworkForProject(project));
		}
		
		[Test]
		public void IsTestMethodReturnsFalseForUnknownMbUnitFrameworkTestMethod()
		{
			method.MockDeclaringType.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.vbproj";
			
			Assert.IsFalse(testFrameworks.IsTestMethod(method));
		}
		
		[Test]
		public void IsTestMethodReturnsTrueForKnownMbUnitFrameworkTestMethod()
		{
			method.MockDeclaringType.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.vbproj";
			
			mbUnitTestFramework.AddTestMethod(method);
			
			Assert.IsTrue(testFrameworks.IsTestMethod(method));
		}
		
		[Test]
		public void IsTestMethodDoesNotThrowNullReferenceExceptionWhenNoTestFrameworkSupportsProject()
		{
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
			clazz.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.vbproj";
			
			Assert.IsFalse(testFrameworks.IsTestClass(clazz));
		}
		
		[Test]
		public void IsTestClassReturnsTrueForKnownMbUnitFrameworkTestClass()
		{
			clazz.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.vbproj";
			
			mbUnitTestFramework.AddTestClass(clazz);
			
			Assert.IsTrue(testFrameworks.IsTestClass(clazz));
		}
		
		[Test]
		public void IsTestClassDoesNotThrowNullReferenceExceptionWhenNoTestFrameworkSupportsProject()
		{
			clazz.MockProjectContent.ProjectAsIProject.FileName = @"d:\projects\test.unknown";
			
			Assert.IsFalse(testFrameworks.IsTestClass(clazz));
		}
		
		[Test]
		public void IsTestClassDoesNotThrowNullReferenceWhenNullPassedToMethod()
		{
			Assert.IsFalse(testFrameworks.IsTestClass(null));
		}
		
		[Test]
		public void IsTestProjectReturnsFalseForUnknownMbUnitFrameworkTestProject()
		{
			project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.vbproj";
			
			Assert.IsFalse(testFrameworks.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsTrueForKnownMbUnitFrameworkTestProject()
		{
			project = new MockCSharpProject();
			project.FileName = @"d:\projects\test.vbproj";
			
			mbUnitTestFramework.AddTestProject(project);
			
			Assert.IsTrue(testFrameworks.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectDoesNotThrowNullReferenceExceptionWhenNoTestFrameworkSupportsProject()
		{
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
			project.FileName = @"d:\projects\test.csproj";
			
			ITestRunner testRunner = testFrameworks.CreateTestRunner(project);
			ITestRunner[] expectedTestRunners = new ITestRunner[] { testRunner };
			Assert.AreEqual(expectedTestRunners, nunitTestFramework.TestRunnersCreated.ToArray());
		}
		
		[Test]
		public void CreateTestRunnerDoesNotThrowNullRefExceptionWhenUnknownProjectPassedToCreateTestRunnerMethod()
		{
			project.FileName = @"d:\projects\test.unknown";
			
			Assert.IsNull(testFrameworks.CreateTestRunner(project));
		}
		
		[Test]
		public void CreateTestDebuggerReturnsNewTestRunnerFromCorrectTestFramework()
		{
			project.FileName = @"d:\projects\test.csproj";
			
			ITestRunner testDebugger = testFrameworks.CreateTestDebugger(project);
			ITestRunner[] expectedTestRunners = new ITestRunner[] { testDebugger };
			Assert.AreEqual(expectedTestRunners, nunitTestFramework.TestDebuggersCreated.ToArray());
		}
		
		[Test]
		public void CreateTestDebuggerDoesNotThrowNullRefExceptionWhenUnknownProjectPassedToCreateTestRunnerMethod()
		{
			project.FileName = @"d:\projects\test.unknown";
			
			Assert.IsNull(testFrameworks.CreateTestDebugger(project));
		}
		
		[Test]
		public void IsBuildNeededBeforeTestRunReturnsTrueWhenTestFrameworkIsBuildNeededBeforeTestRunSetToTrue()
		{
			project.FileName = @"d:\projects\test.csproj";
			nunitTestFramework.IsBuildNeededBeforeTestRun = true;
			
			Assert.IsTrue(testFrameworks.IsBuildNeededBeforeTestRunForProject(project));
		}
		
		[Test]
		public void IsBuildNeededBeforeTestRunReturnsFalseWhenTestFrameworkIsBuildNeededBeforeTestRunSetToFalse()
		{
			project.FileName = @"d:\projects\test.csproj";
			nunitTestFramework.IsBuildNeededBeforeTestRun = false;
			
			Assert.IsFalse(testFrameworks.IsBuildNeededBeforeTestRunForProject(project));
		}
	}
	
	[TestFixture]
    public class RegisteredTestFrameworksWithTwoFrameworksForTheSameProjectType
    {
        MockCSharpProject nunitTestProject;
        MockTestFramework nunitTestFramework;
        MockCSharpProject mspecTestProject;
        MockTestFramework mspecTestFramework;
        RegisteredTestFrameworks testFrameworks;

        [SetUp]
        public void Setup()
        {
			var factory = new MockTestFrameworkFactory();

            Properties nunitProperties = new Properties();
            nunitProperties["id"] = "nunit";
            nunitProperties["class"] = "NUnitTestFramework";
            nunitProperties["supportedProjects"] = ".csproj";
            nunitTestFramework = new MockTestFramework();
            factory.Add("NUnitTestFramework", nunitTestFramework);
            
            var mspecProperties = new Properties();
			mspecProperties["id"] = "mspec";
			mspecProperties["class"] = "MSpecTestFramework";
			mspecProperties["supportedProjects"] = ".csproj";
            mspecTestFramework = new MockTestFramework();
            factory.Add("MSpecTestFramework", mspecTestFramework);
			
			TestFrameworkDescriptor mspecDescriptor = new TestFrameworkDescriptor(mspecProperties, factory);
			TestFrameworkDescriptor nunitDescriptor = new TestFrameworkDescriptor(nunitProperties, factory);

            var descriptors = new List<TestFrameworkDescriptor> { mspecDescriptor, nunitDescriptor };
			
			MockAddInTree addinTree = new MockAddInTree();
            addinTree.AddItems("/SharpDevelop/UnitTesting/TestFrameworks", descriptors);
			
			testFrameworks = new RegisteredTestFrameworks(addinTree);

            nunitTestProject = new MockCSharpProject();
            nunitTestProject.FileName = @"d:\projects\nunitTestProject.csproj";
            nunitTestFramework.AddTestProject(nunitTestProject);

            mspecTestProject = new MockCSharpProject();
            mspecTestProject.FileName = @"d:\projects\mspecTestProject.csproj";
            mspecTestFramework.AddTestProject(mspecTestProject);
        }

        [Test]
        public void ItShouldReturnMSpecTestFrameworkForMSpecTestProject()
        {
            Assert.AreSame(mspecTestFramework, testFrameworks.GetTestFrameworkForProject(mspecTestProject), "Expected mspec test framework.");
        }

        [Test]
        public void ItShouldReturnNUnitTestFrameworkForNUnitTestProject()
        {
            Assert.AreSame(nunitTestFramework, testFrameworks.GetTestFrameworkForProject(nunitTestProject), "Expected nunit test framework.");
        }
    }
}
