// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		Task task;
		
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
