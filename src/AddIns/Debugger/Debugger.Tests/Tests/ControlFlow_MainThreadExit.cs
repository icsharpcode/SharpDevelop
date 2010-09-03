// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;

namespace Debugger.Tests
{
	public class ControlFlow_MainThreadExit
	{
		static ManualResetEvent doSomething = new ManualResetEvent(false);
		
		public static void Main()
		{
			System.Threading.Thread t = new System.Threading.Thread(WaitForALongTime);
			t.Name = "Worker thread";
			t.Start();
			// Wait for the thread to start
			System.Threading.Thread.Sleep(500);
			System.Diagnostics.Debugger.Break();
		}
		
		static void WaitForALongTime()
		{
			doSomething.WaitOne();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void ControlFlow_MainThreadExit()
		{
			StartTest();
			ObjectDump("ThreadsBeforeExit", process.Threads);
			process.AsyncContinue();
			// Wait for the main thread to exit
			System.Threading.Thread.Sleep(500);
			process.Break();
			ObjectDump("ThreadsAfterExit", process.Threads);
			process.Terminate();
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="ControlFlow_MainThreadExit.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ControlFlow_MainThreadExit.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break ControlFlow_MainThreadExit.cs:24,4-24,40</DebuggingPaused>
    <ThreadsBeforeExit
      Count="2"
      Selected="Thread Name =  Suspended = False">
      <Item>
        <Thread
          Callstack="{static void Debugger.Tests.ControlFlow_MainThreadExit.Main()}"
          CurrentExceptionType="0"
          IsAtSafePoint="True"
          IsInValidState="True"
          MostRecentStackFrame="static void Debugger.Tests.ControlFlow_MainThreadExit.Main()"
          Name=""
          Priority="Normal"
          RuntimeValue="{System.Threading.Thread}"
          SelectedStackFrame="static void Debugger.Tests.ControlFlow_MainThreadExit.Main()" />
      </Item>
      <Item>
        <Thread
          Callstack="{static Boolean System.Threading.WaitHandle.InternalWaitOne(SafeHandle waitableSafeHandle, Int64 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext), Boolean System.Threading.WaitHandle.WaitOne(Int32 millisecondsTimeout, Boolean exitContext), Boolean System.Threading.WaitHandle.WaitOne(), static void Debugger.Tests.ControlFlow_MainThreadExit.WaitForALongTime(), static void System.Threading.ThreadHelper.ThreadStart_Context(Object state), static void System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean ignoreSyncCtx), static void System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state), void System.Threading.ThreadHelper.ThreadStart()}"
          CurrentExceptionType="0"
          IsAtSafePoint="True"
          IsInValidState="True"
          MostRecentStackFrame="static Boolean System.Threading.WaitHandle.InternalWaitOne(SafeHandle waitableSafeHandle, Int64 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)"
          Name="Worker thread"
          Priority="Normal"
          RuntimeValue="{System.Threading.Thread}" />
      </Item>
    </ThreadsBeforeExit>
    <DebuggingPaused>ForcedBreak ControlFlow_MainThreadExit.cs:29,4-29,26</DebuggingPaused>
    <ThreadsAfterExit
      Count="2"
      Selected="Thread Name = Worker thread Suspended = False">
      <Item>
        <Thread
          CurrentExceptionType="0"
          Name=""
          Priority="Normal"
          RuntimeValue="{Exception: The state of the thread is invalid. (Exception from HRESULT: 0x8013132D)}" />
      </Item>
      <Item>
        <Thread
          Callstack="{static Boolean System.Threading.WaitHandle.InternalWaitOne(SafeHandle waitableSafeHandle, Int64 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext), Boolean System.Threading.WaitHandle.WaitOne(Int32 millisecondsTimeout, Boolean exitContext), Boolean System.Threading.WaitHandle.WaitOne(), static void Debugger.Tests.ControlFlow_MainThreadExit.WaitForALongTime(), static void System.Threading.ThreadHelper.ThreadStart_Context(Object state), static void System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean ignoreSyncCtx), static void System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state), void System.Threading.ThreadHelper.ThreadStart()}"
          CurrentExceptionType="0"
          IsAtSafePoint="True"
          IsInValidState="True"
          MostRecentStackFrame="static Boolean System.Threading.WaitHandle.InternalWaitOne(SafeHandle waitableSafeHandle, Int64 millisecondsTimeout, Boolean hasThreadAffinity, Boolean exitContext)"
          Name="Worker thread"
          Priority="Normal"
          RuntimeValue="{System.Threading.Thread}"
          SelectedStackFrame="static void Debugger.Tests.ControlFlow_MainThreadExit.WaitForALongTime()" />
      </Item>
    </ThreadsAfterExit>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
