// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Util;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockProcessRunnerTestFixture
	{
		MockProcessRunner processRunner;
		
		[SetUp]
		public void Init()
		{
			processRunner = new MockProcessRunner();
		}
		
		[Test]
		public void LogStandardOutputAndErrorReturnsTrueByDefault()
		{
			Assert.IsTrue(processRunner.LogStandardOutputAndError);
		}
		
		[Test]
		public void LogStandardOutputAndErrorReturnsFalseIfSetToFalse()
		{
			processRunner.LogStandardOutputAndError = false;
			Assert.IsFalse(processRunner.LogStandardOutputAndError);
		}
		
		[Test]
		public void CommandPassedToStartMethodReturnsNullByDefault()
		{
			Assert.IsNull(processRunner.CommandPassedToStartMethod);
		}
		
		[Test]
		public void CommandArgumentsPassedToStartMethodReturnsNullByDefault()
		{
			Assert.IsNull(processRunner.CommandArgumentsPassedToStartMethod);
		}
		
		[Test]
		public void CommandPassedToStartMethodIsNUnitConsoleExeAfterStartMethodIsCalled()
		{
			string command = "nunit-console.exe";
			processRunner.Start(command, null);
			Assert.AreEqual(command, processRunner.CommandPassedToStartMethod);
		}
		
		[Test]
		public void CommandArgumentsPassedToStartMethodIsTestAfterStartMethodIsCalled()
		{
			string args = "test";
			processRunner.Start(null, args);
			Assert.AreEqual(args, processRunner.CommandArgumentsPassedToStartMethod);
		}
		
		[Test]
		public void IsKillMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(processRunner.IsKillMethodCalled);
		}
		
		[Test]
		public void IsKillMethodCalledReturnsTrueAfterKillMethodCalled()
		{
			processRunner.Kill();
			Assert.IsTrue(processRunner.IsKillMethodCalled);
		}
		
		[Test]
		public void FireProcessExitedEventFiresEvent()
		{
			bool fired = false;
			processRunner.ProcessExited += delegate (object o, EventArgs e) {
				fired = true;
			};
			processRunner.FireProcessExitedEvent();
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void FireOutputLineReceivedEventFiresEventAndReturnsExpectedLine()
		{
			LineReceivedEventArgs expectedEventArgs = null;
			processRunner.OutputLineReceived += delegate (object o, LineReceivedEventArgs e) {
				expectedEventArgs = e;
			};
			string line = "test";
			LineReceivedEventArgs eventArgs = new LineReceivedEventArgs(line);
			processRunner.FireOutputLineReceivedEvent(eventArgs);
			
			Assert.AreEqual(line, expectedEventArgs.Line);
		}
		
		[Test]
		public void FireErrorLineReceivedEventFiresEventAndReturnsExpectedLine()
		{
			LineReceivedEventArgs expectedEventArgs = null;
			processRunner.ErrorLineReceived += delegate (object o, LineReceivedEventArgs e) {
				expectedEventArgs = e;
			};
			string line = "test";
			LineReceivedEventArgs eventArgs = new LineReceivedEventArgs(line);
			processRunner.FireErrorLineReceivedEvent(eventArgs);
			
			Assert.AreEqual(line, expectedEventArgs.Line);
		}
	}
}
