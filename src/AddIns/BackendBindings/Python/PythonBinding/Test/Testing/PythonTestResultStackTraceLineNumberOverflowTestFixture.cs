// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace PythonBinding.Tests.Testing
{
	[TestFixture]
	public class PythonTestResultLineNumberOverflowTestFixture
	{
		PythonTestResult pythonTestResult;
		string stackTraceText;
		
		[SetUp]
		public void Init()
		{
			TestResult testResult = new TestResult("MyTest");
			testResult.ResultType = TestResultType.Failure;
			testResult.Message = "test failed";
			
			stackTraceText = 
				"Traceback (most recent call last):\r\n" +
				"  File \"d:\\temp\\test\\PyTests\\Tests\\MyClassTest.py\", line 4294967296, in testAssertEquals\r\n" +
				"    self.assertEqual(10, 15, 'wrong size after resize')\r\n" +
				"AssertionError: wrong size after resize";
			
			testResult.StackTrace = stackTraceText;
			pythonTestResult = new PythonTestResult(testResult);
		}
		
		[Test]
		public void StackTraceFilePositionIsEmpty()
		{
			Assert.IsTrue(pythonTestResult.StackTraceFilePosition.IsEmpty);
		}
	}
}
