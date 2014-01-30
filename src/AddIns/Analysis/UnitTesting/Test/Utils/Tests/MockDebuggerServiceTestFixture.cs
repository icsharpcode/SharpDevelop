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
using System.Diagnostics;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockDebuggerServiceTestFixture
	{
		MockDebuggerService debuggerService;
		MockDebugger debugger;
		
		[SetUp]
		public void Init()
		{
			debuggerService = new MockDebuggerService();
			debugger = debuggerService.CurrentDebugger as MockDebugger;
		}
		
		[Test]
		public void IsDebuggerLoadedReturnsFalseByDefault()
		{
			Assert.IsFalse(debuggerService.IsDebuggerLoaded);
		}
		
		[Test]
		public void IsDebuggerLoadedReturnsTrueAfterBeingSetToTrue()
		{
			debuggerService.IsDebuggerLoaded = true;
			Assert.IsTrue(debuggerService.IsDebuggerLoaded);
		}
		
		[Test]
		public void DebuggerIsDebuggingReturnsFalseByDefault()
		{
			Assert.IsFalse(debugger.IsDebugging);
		}
		
		[Test]
		public void MockDebuggerIsSameAsCurrentDebugger()
		{
			Assert.AreEqual(debuggerService.CurrentDebugger, debuggerService.MockDebugger);
		}
		
		[Test]
		public void DebuggerIsDebuggingReturnsTrueAfterBeingSetToTrue()
		{
			debugger.IsDebugging = true;
			Assert.IsTrue(debugger.IsDebugging);
		}
		
		[Test]
		public void DebuggerIsStopCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(debugger.IsStopCalled);
		}
		
		[Test]
		public void DebuggerIsStopCalledReturnsTrueAfterStopMethodCalled()
		{
			debugger.Stop();
			Assert.IsTrue(debugger.IsStopCalled);
		}
		
		[Test]
		public void DebuggerFireDebugStoppedEventFiresDebugStoppedEvent()
		{
			bool fired = false;
			debugger.DebugStopped += delegate {
				fired = true;
			};
			debugger.FireDebugStoppedEvent();
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void DebuggerProcessStartInfoReturnsNullByDefault()
		{
			Assert.IsNull(debugger.ProcessStartInfo);
		}
		
		[Test]
		public void DebuggerProcessStartInfoSavedAfterStartMethodCalled()
		{
			ProcessStartInfo startInfo = new ProcessStartInfo();
			debugger.Start(startInfo);
			
			Assert.AreEqual(startInfo, debugger.ProcessStartInfo);
		}
		
		[Test]
		public void DebuggerWillThrowExceptionIfConfiguredWhenStartMethodCalled()
		{
			ApplicationException expectedException = new ApplicationException();
			debugger.ThrowExceptionOnStart = expectedException;
			
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			ApplicationException actualException = 
				Assert.Throws<ApplicationException>(delegate { debugger.Start(processStartInfo); });
			Assert.AreEqual(expectedException, actualException);
		}
	}
}
