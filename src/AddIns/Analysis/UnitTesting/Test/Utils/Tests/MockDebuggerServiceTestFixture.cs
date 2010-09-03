// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
