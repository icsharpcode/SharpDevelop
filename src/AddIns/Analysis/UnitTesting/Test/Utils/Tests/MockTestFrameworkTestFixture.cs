// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
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
			MockMethod method = MockMethod.CreateResolvedMethod();
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
			MockMethod method = MockMethod.CreateResolvedMethod();
			testFramework.IsTestMember(method);
			Assert.AreEqual(method, testFramework.IsTestMemberParameterUsed);
		}
		
		[Test]
		public void IsTestMethodReturnsTrueIfMethodMatchesMethodPreviouslySpecified()
		{
			MockMethod method = MockMethod.CreateResolvedMethod();
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
