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
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ControlFlow_MainThreadExit.exe (Has symbols)</ModuleLoaded>
    <Paused>ControlFlow_MainThreadExit.cs:20,4-20,40</Paused>
    <ThreadsBeforeExit
      Capacity="4"
      Count="2">
      <Item>
        <Thread
          Callstack="{[Method Debugger.Tests.ControlFlow_MainThreadExit.Main]}"
          IsAtSafePoint="True"
          IsInValidState="True"
          MostRecentStackFrame="[Method Debugger.Tests.ControlFlow_MainThreadExit.Main]"
          Name=""
          Priority="Normal"
          RuntimeValue="{System.Threading.Thread}" />
      </Item>
      <Item>
        <Thread
          Callstack="{[Method System.Threading.WaitHandle.InternalWaitOne], [Method System.Threading.WaitHandle.WaitOne], [Method System.Threading.WaitHandle.WaitOne], [Method Debugger.Tests.ControlFlow_MainThreadExit.WaitForALongTime], [Method System.Threading.ThreadHelper.ThreadStart_Context], [Method System.Threading.ExecutionContext.RunInternal], [Method System.Threading.ExecutionContext.Run], [Method System.Threading.ExecutionContext.Run], [Method System.Threading.ThreadHelper.ThreadStart]}"
          IsAtSafePoint="True"
          IsInValidState="True"
          MostRecentStackFrame="[Method System.Threading.WaitHandle.InternalWaitOne]"
          Name="Worker thread"
          Priority="Normal"
          RuntimeValue="{System.Threading.Thread}" />
      </Item>
    </ThreadsBeforeExit>
    <Paused></Paused>
    <ThreadsAfterExit
      Capacity="4"
      Count="2">
      <Item>
        <Thread
          Name=""
          Priority="Normal"
          RuntimeValue="{Exception: The state of the thread is invalid. (Exception from HRESULT: 0x8013132D)}" />
      </Item>
      <Item>
        <Thread
          Callstack="{[Method System.Threading.WaitHandle.InternalWaitOne], [Method System.Threading.WaitHandle.WaitOne], [Method System.Threading.WaitHandle.WaitOne], [Method Debugger.Tests.ControlFlow_MainThreadExit.WaitForALongTime], [Method System.Threading.ThreadHelper.ThreadStart_Context], [Method System.Threading.ExecutionContext.RunInternal], [Method System.Threading.ExecutionContext.Run], [Method System.Threading.ExecutionContext.Run], [Method System.Threading.ThreadHelper.ThreadStart]}"
          IsAtSafePoint="True"
          IsInValidState="True"
          MostRecentStackFrame="[Method System.Threading.WaitHandle.InternalWaitOne]"
          Name="Worker thread"
          Priority="Normal"
          RuntimeValue="{System.Threading.Thread}" />
      </Item>
    </ThreadsAfterExit>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
