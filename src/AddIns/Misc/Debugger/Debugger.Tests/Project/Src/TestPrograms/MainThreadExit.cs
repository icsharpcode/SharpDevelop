// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Threading;

namespace Debugger.Tests.TestPrograms
{
	public class MainThreadExit
	{
		static ManualResetEvent doSomething = new ManualResetEvent(false);
		
		public static void Main()
		{
			System.Threading.Thread t = new System.Threading.Thread(WaitForALongTime);
			t.Name = "Worker thread";
			t.Start();
			
			// RACE CONDITION: this does not guarantee that the worker thread will have started
			// when the debugger breaks.
			System.Threading.Thread.Sleep(0);
			
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
		[NUnit.Framework.Ignore("Ignoring test due to race condition")]
		public void MainThreadExit()
		{
			StartTest("MainThreadExit.cs");
			ObjectDump("ThreadsBeforeExit", process.Threads);
			process.AsyncContinue();
			System.Threading.Thread.Sleep(250);
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
    name="MainThreadExit.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>MainThreadExit.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break MainThreadExit.cs:23,4-23,40</DebuggingPaused>
    <ThreadsBeforeExit
      Count="2">
      <Item>
        <Thread
          CurrentException="null"
          CurrentExceptionIsUnhandled="False"
          CurrentExceptionType="0"
          HasExited="False"
          IsAtSafePoint="True"
          IsInValidState="True"
          IsMostRecentStackFrameNative="False"
          MostRecentStackFrame="Debugger.Tests.TestPrograms.MainThreadExit.Main"
          MostRecentStackFrameWithLoadedSymbols="Debugger.Tests.TestPrograms.MainThreadExit.Main"
          Name=""
          OldestStackFrame="Debugger.Tests.TestPrograms.MainThreadExit.Main"
          Priority="Normal"
          RuntimeValue="? = {System.Threading.Thread}"
          SelectedStackFrame="Debugger.Tests.TestPrograms.MainThreadExit.Main"
          Suspended="False" />
      </Item>
      <Item>
        <Thread
          CurrentException="null"
          CurrentExceptionIsUnhandled="False"
          CurrentExceptionType="0"
          HasExited="False"
          IsAtSafePoint="True"
          IsInValidState="True"
          IsMostRecentStackFrameNative="False"
          MostRecentStackFrame="System.Threading.WaitHandle.WaitOne"
          MostRecentStackFrameWithLoadedSymbols="Debugger.Tests.TestPrograms.MainThreadExit.WaitForALongTime"
          Name="Worker thread"
          OldestStackFrame="System.Threading.ThreadHelper.ThreadStart"
          Priority="Normal"
          RuntimeValue="? = {System.Threading.Thread}"
          SelectedStackFrame="null"
          Suspended="False" />
      </Item>
    </ThreadsBeforeExit>
    <DebuggingPaused>ForcedBreak MainThreadExit.cs:28,4-28,26</DebuggingPaused>
    <ThreadsAfterExit
      Count="2">
      <Item>
        <Thread
          CurrentException="null"
          CurrentExceptionIsUnhandled="False"
          CurrentExceptionType="0"
          HasExited="False"
          IsAtSafePoint="False"
          IsInValidState="False"
          IsMostRecentStackFrameNative="False"
          MostRecentStackFrame="null"
          MostRecentStackFrameWithLoadedSymbols="null"
          Name=""
          OldestStackFrame="null"
          Priority="Normal"
          RuntimeValue="{Exception: The state of the thread is invalid. (Exception from HRESULT: 0x8013132D)}"
          SelectedStackFrame="null"
          Suspended="False" />
      </Item>
      <Item>
        <Thread
          CurrentException="null"
          CurrentExceptionIsUnhandled="False"
          CurrentExceptionType="0"
          HasExited="False"
          IsAtSafePoint="True"
          IsInValidState="True"
          IsMostRecentStackFrameNative="False"
          MostRecentStackFrame="System.Threading.WaitHandle.WaitOne"
          MostRecentStackFrameWithLoadedSymbols="Debugger.Tests.TestPrograms.MainThreadExit.WaitForALongTime"
          Name="Worker thread"
          OldestStackFrame="System.Threading.ThreadHelper.ThreadStart"
          Priority="Normal"
          RuntimeValue="? = {System.Threading.Thread}"
          SelectedStackFrame="Debugger.Tests.TestPrograms.MainThreadExit.WaitForALongTime"
          Suspended="False" />
      </Item>
    </ThreadsAfterExit>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT