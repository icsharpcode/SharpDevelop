// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockTestFrameworkTestFixture
	{
		MockTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new MockTestFramework();
		}
		
		[Test]
		public void IsTestMethodReturnsFalseByDefault()
		{
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			Assert.IsFalse(testFramework.IsTestMember(method));
		}
		
		[Test]
		public void IsTestMethodIMemberParameterIsInitiallyNull()
		{
			Assert.IsNull(testFramework.IsTestMemberParameterUsed);
		}
		
		[Test]
		public void IsTestMethodCallRecorded()
		{
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			testFramework.IsTestMember(method);
			Assert.AreEqual(method, testFramework.IsTestMemberParameterUsed);
		}
		
		[Test]
		public void IsTestMethodReturnsTrueIfMethodMatchesMethodPreviouslySpecified()
		{
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			testFramework.AddTestMember(method);
			
			Assert.IsTrue(testFramework.IsTestMember(method));
		}
		
		[Test]
		public void IsTestClassReturnsFalseByDefault()
		{
			MockClass c = new MockClass();
			Assert.IsFalse(testFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassReturnsTrueIfClassMatchesClassPreviouslySpecified()
		{
			MockClass c = new MockClass();
			testFramework.AddTestClass(c);
			
			Assert.IsTrue(testFramework.IsTestClass(c));
		}
		
		[Test]
		public void IsTestClassIClassParameterIsInitiallyNull()
		{
			Assert.IsNull(testFramework.IsTestClassParameterUsed);
		}
		
		[Test]
		public void IsTestClassCallRecorded()
		{
			MockClass c = new MockClass();
			testFramework.IsTestClass(c);
			Assert.AreEqual(c, testFramework.IsTestClassParameterUsed);
		}
		
		[Test]
		public void IsTestProjectReturnsFalseByDefault()
		{
			MockCSharpProject project = new MockCSharpProject();
			Assert.IsFalse(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsTrueIfProjectMatchesProjectPreviouslySpecified()
		{
			MockCSharpProject project = new MockCSharpProject();
			testFramework.AddTestProject(project);
			
			Assert.IsTrue(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectIProjectParameterIsInitiallyNull()
		{
			Assert.IsNull(testFramework.IsTestProjectParameterUsed);
		}
		
		[Test]
		public void IsTestProjectallRecorded()
		{
			MockCSharpProject project = new MockCSharpProject();
			testFramework.IsTestProject(project);
			Assert.AreEqual(project, testFramework.IsTestProjectParameterUsed);
		}
		
		[Test]
		public void IsTestClassReturnsFalseAfterTestClassRemovedFromTestFramework()
		{
			MockClass c = new MockClass();
			testFramework.AddTestClass(c);
			testFramework.RemoveTestClass(c);
			
			Assert.IsFalse(testFramework.IsTestClass(c));
		}
		
		[Test]
		public void CreateTestRunnerReturnsNewMockTestRunner()
		{
			Assert.IsInstanceOf(typeof(MockTestRunner), testFramework.CreateTestRunner());
		}
		
		[Test]
		public void TestRunnersCreatedReturnsTestRunnersCreatedByCallingCreateTestRunnerMethod()
		{
			List<ITestRunner> expectedRunners = new List<ITestRunner>();
			expectedRunners.Add(testFramework.CreateTestRunner());
			expectedRunners.Add(testFramework.CreateTestRunner());
			
			Assert.AreEqual(expectedRunners.ToArray(), testFramework.TestRunnersCreated.ToArray());
		}
		
		[Test]
		public void TestDebuggersCreatedReturnsTestRunnersCreatedByCallingCreateTestDebuggerMethod()
		{
			List<ITestRunner> expectedRunners = new List<ITestRunner>();
			expectedRunners.Add(testFramework.CreateTestDebugger());
			expectedRunners.Add(testFramework.CreateTestDebugger());
			
			Assert.AreEqual(expectedRunners.ToArray(), testFramework.TestDebuggersCreated.ToArray());
		}
		
		[Test]
		public void IsBuildNeededBeforeTestRunReturnsTrueByDefault()
		{
			Assert.IsTrue(testFramework.IsBuildNeededBeforeTestRun);
		}
	}
}
