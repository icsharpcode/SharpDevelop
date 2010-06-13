// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class TestResultFailureTaskWithNoMessageTestFixture
	{
		Task task;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			ResourceManager.Initialize();
		}
		
		[SetUp]
		public void Init()
		{
			TestResult testResult = new TestResult("MyNamespace.MyTests");
			testResult.ResultType = TestResultType.Failure;
			task = TestResultTask.Create(testResult, null);
		}
		
		[Test]
		public void TaskMessageEqualsIsTakenFromStringResource()
		{
			Assert.AreEqual("Test case 'MyNamespace.MyTests' failed.", task.Description);
		}
	}
}
