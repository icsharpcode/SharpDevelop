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
    <DebuggingPaused>Break ControlFlow_MainThreadExit.cs:20,4-20,40</DebuggingPaused>
    <ThreadsBeforeExit
      Count="2"
      Selected="Thread Name =  Suspended = False">
      <Item>
        <Thread
          Callstack="{static System.Void Debugger.Tests.ControlFlow_MainThreadExit.Main()}"
          CurrentExceptionType="0"
          IsAtSafePoint="True"
          IsInValidState="True"
          MostRecentStackFrame="static System.Void Debugger.Tests.ControlFlow_MainThreadExit.Main()"
          Name=""
          Priority="Normal"
          RuntimeValue="{System.Threading.Thread}"
          SelectedStackFrame="static System.Void Debugger.Tests.ControlFlow_MainThreadExit.Main()" />
      </Item>
      <Item>
        <Thread
          Callstack="{static Boolean System.Threading.WaitHandle.InternalWaitOne(System.Runtime.InteropServices.SafeHandle waitableSafeHandle, System.Int64 millisecondsTimeout, System.Boolean hasThreadAffinity, System.Boolean exitContext), Boolean System.Threading.WaitHandle.WaitOne(System.Int32 millisecondsTimeout, System.Boolean exitContext), Boolean System.Threading.WaitHandle.WaitOne(), static System.Void Debugger.Tests.ControlFlow_MainThreadExit.WaitForALongTime(), static System.Void System.Threading.ThreadHelper.ThreadStart_Context(System.Object state), static System.Void System.Threading.ExecutionContext.Run(System.Threading.ExecutionContext executionContext, System.Threading.ContextCallback callback, System.Object state, System.Boolean ignoreSyncCtx), static System.Void System.Threading.ExecutionContext.Run(System.Threading.ExecutionContext executionContext, System.Threading.ContextCallback callback, System.Object state), System.Void System.Threading.ThreadHelper.ThreadStart()}"
          CurrentExceptionType="0"
          IsAtSafePoint="True"
          IsInValidState="True"
          MostRecentStackFrame="static Boolean System.Threading.WaitHandle.InternalWaitOne(System.Runtime.InteropServices.SafeHandle waitableSafeHandle, System.Int64 millisecondsTimeout, System.Boolean hasThreadAffinity, System.Boolean exitContext)"
          Name="Worker thread"
          Priority="Normal"
          RuntimeValue="{System.Threading.Thread}" />
      </Item>
    </ThreadsBeforeExit>
    <DebuggingPaused>ForcedBreak </DebuggingPaused>
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
          Callstack="{static Boolean System.Threading.WaitHandle.InternalWaitOne(System.Runtime.InteropServices.SafeHandle waitableSafeHandle, System.Int64 millisecondsTimeout, System.Boolean hasThreadAffinity, System.Boolean exitContext), Boolean System.Threading.WaitHandle.WaitOne(System.Int32 millisecondsTimeout, System.Boolean exitContext), Boolean System.Threading.WaitHandle.WaitOne(), static System.Void Debugger.Tests.ControlFlow_MainThreadExit.WaitForALongTime(), static System.Void System.Threading.ThreadHelper.ThreadStart_Context(System.Object state), static System.Void System.Threading.ExecutionContext.Run(System.Threading.ExecutionContext executionContext, System.Threading.ContextCallback callback, System.Object state, System.Boolean ignoreSyncCtx), static System.Void System.Threading.ExecutionContext.Run(System.Threading.ExecutionContext executionContext, System.Threading.ContextCallback callback, System.Object state), System.Void System.Threading.ThreadHelper.ThreadStart()}"
          CurrentExceptionType="0"
          IsAtSafePoint="True"
          IsInValidState="True"
          MostRecentStackFrame="static Boolean System.Threading.WaitHandle.InternalWaitOne(System.Runtime.InteropServices.SafeHandle waitableSafeHandle, System.Int64 millisecondsTimeout, System.Boolean hasThreadAffinity, System.Boolean exitContext)"
          Name="Worker thread"
          Priority="Normal"
          RuntimeValue="{System.Threading.Thread}"
          SelectedStackFrame="static Boolean System.Threading.WaitHandle.InternalWaitOne(System.Runtime.InteropServices.SafeHandle waitableSafeHandle, System.Int64 millisecondsTimeout, System.Boolean hasThreadAffinity, System.Boolean exitContext)" />
      </Item>
    </ThreadsAfterExit>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
