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
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockRegisteredTestFrameworksTestFixture
	{
		MockRegisteredTestFrameworks testFrameworks;
		MockTestFramework testFramework;
		MockCSharpProject project;
		
		[SetUp]
		public void Init()
		{
			testFrameworks = new MockRegisteredTestFrameworks();
			
			testFramework = new MockTestFramework();
			project = new MockCSharpProject();
			testFrameworks.AddTestFrameworkForProject(project, testFramework);
		}
		
		[Test]
		public void CreateTestRunnerForKnownProjectCreatesTestRunnerInKnownTestFramework()
		{
			ITestRunner testRunner = testFrameworks.CreateTestRunner(project);
			ITestRunner expectedTestRunner = testFramework.TestRunnersCreated[0];
			Assert.AreEqual(expectedTestRunner, testRunner);
		}
		
		[Test]
		public void CreateTestRunnerReturnsNullForUnknownProject()
		{
			MockCSharpProject unknownProject = new MockCSharpProject();
			Assert.IsNull(testFrameworks.CreateTestRunner(unknownProject));
		}
		
		[Test]
		public void GetTestFrameworkForProjectReturnsTestFrameworkForKnownProject()
		{
			Assert.AreEqual(testFramework, testFrameworks.GetTestFrameworkForProject(project));
		}
		
		[Test]
		public void GetTestFrameworkForProjectReturnsTestFrameworkForUnknownProject()
		{
			MockCSharpProject unknownProject = new MockCSharpProject();
			Assert.IsNull(testFrameworks.GetTestFrameworkForProject(unknownProject));
		}
		
		[Test]
		public void CreateTestDebuggerForKnownProjectCreatesTestDebuggerInKnownTestFramework()
		{
			ITestRunner testRunner = testFrameworks.CreateTestDebugger(project);
			ITestRunner expectedTestRunner = testFramework.TestDebuggersCreated[0];
			Assert.AreEqual(expectedTestRunner, testRunner);
		}

		[Test]
		public void CreateTestDebuggerReturnsNullForUnknownProject()
		{
			MockCSharpProject unknownProject = new MockCSharpProject();
			Assert.IsNull(testFrameworks.CreateTestDebugger(unknownProject));
		}
	}
}
