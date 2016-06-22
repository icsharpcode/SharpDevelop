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
