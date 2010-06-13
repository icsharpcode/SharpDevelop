// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
