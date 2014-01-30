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
using System.Resources;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class TestResultIgnoreTaskWithNoMessageTestFixture
	{
		SDTask task;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			ResourceManager rm = new ResourceManager("UnitTesting.Tests.Strings", GetType().Assembly);
			ResourceService.RegisterNeutralStrings(rm);
		}
		
		[SetUp]
		public void Init()
		{
			TestResult testResult = new TestResult("MyNamespace.MyTests");
			testResult.ResultType = TestResultType.Ignored;
			task = TestResultTask.Create(testResult, null);
		}
		
		[Test]
		public void TaskMessageEqualsIsTakenFromStringResource()
		{
			Assert.AreEqual("Test case 'MyNamespace.MyTests' was not executed.", task.Description);
		}
	}
}
