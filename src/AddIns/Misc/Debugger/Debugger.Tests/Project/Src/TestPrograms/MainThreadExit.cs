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
  <Test name="MainThreadExit.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">MainThreadExit.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <ThreadsBeforeExit Type="ReadOnlyCollection`1" ToString="System.Collections.ObjectModel.ReadOnlyCollection`1[Debugger.Thread]">
      <Count>2</Count>
      <Item Type="Thread" ToString="Thread Name =  Suspended = False">
        <CurrentException>null</CurrentException>
        <CurrentExceptionIsUnhandled>False</CurrentExceptionIsUnhandled>
        <CurrentExceptionType>0</CurrentExceptionType>
        <HasExited>False</HasExited>
        <IsAtSafePoint>True</IsAtSafePoint>
        <IsInValidState>True</IsInValidState>
        <IsMostRecentStackFrameNative>False</IsMostRecentStackFrameNative>
        <MostRecentStackFrame>Debugger.Tests.TestPrograms.MainThreadExit.Main</MostRecentStackFrame>
        <MostRecentStackFrameWithLoadedSymbols>Debugger.Tests.TestPrograms.MainThreadExit.Main</MostRecentStackFrameWithLoadedSymbols>
        <Name>
        </Name>
        <OldestStackFrame>Debugger.Tests.TestPrograms.MainThreadExit.Main</OldestStackFrame>
        <Priority>Normal</Priority>
        <RuntimeValue>? = {System.Threading.Thread}</RuntimeValue>
        <SelectedStackFrame>Debugger.Tests.TestPrograms.MainThreadExit.Main</SelectedStackFrame>
        <Suspended>False</Suspended>
      </Item>
      <Item Type="Thread" ToString="Thread Name = Worker thread Suspended = False">
        <CurrentException>null</CurrentException>
        <CurrentExceptionIsUnhandled>False</CurrentExceptionIsUnhandled>
        <CurrentExceptionType>0</CurrentExceptionType>
        <HasExited>False</HasExited>
        <IsAtSafePoint>False</IsAtSafePoint>
        <IsInValidState>True</IsInValidState>
        <IsMostRecentStackFrameNative>False</IsMostRecentStackFrameNative>
        <MostRecentStackFrame>System.Threading.ThreadHelper.ThreadStart</MostRecentStackFrame>
        <MostRecentStackFrameWithLoadedSymbols>null</MostRecentStackFrameWithLoadedSymbols>
        <Name>Worker thread</Name>
        <OldestStackFrame>System.Threading.ThreadHelper.ThreadStart</OldestStackFrame>
        <Priority>Normal</Priority>
        <RuntimeValue>? = {System.Threading.Thread}</RuntimeValue>
        <SelectedStackFrame>null</SelectedStackFrame>
        <Suspended>False</Suspended>
      </Item>
    </ThreadsBeforeExit>
    <DebuggingPaused>ForcedBreak</DebuggingPaused>
    <ThreadsAfterExit Type="ReadOnlyCollection`1" ToString="System.Collections.ObjectModel.ReadOnlyCollection`1[Debugger.Thread]">
      <Count>2</Count>
      <Item Type="Thread" ToString="Thread Name =  Suspended = False">
        <CurrentException>null</CurrentException>
        <CurrentExceptionIsUnhandled>False</CurrentExceptionIsUnhandled>
        <CurrentExceptionType>0</CurrentExceptionType>
        <HasExited>False</HasExited>
        <IsAtSafePoint>False</IsAtSafePoint>
        <IsInValidState>False</IsInValidState>
        <IsMostRecentStackFrameNative>False</IsMostRecentStackFrameNative>
        <MostRecentStackFrame>null</MostRecentStackFrame>
        <MostRecentStackFrameWithLoadedSymbols>null</MostRecentStackFrameWithLoadedSymbols>
        <Name>
        </Name>
        <OldestStackFrame>null</OldestStackFrame>
        <Priority>Normal</Priority>
        <RuntimeValue exception="The state of the thread is invalid. (Exception from HRESULT: 0x8013132D)" />
        <SelectedStackFrame>null</SelectedStackFrame>
        <Suspended>False</Suspended>
      </Item>
      <Item Type="Thread" ToString="Thread Name = Worker thread Suspended = False">
        <CurrentException>null</CurrentException>
        <CurrentExceptionIsUnhandled>False</CurrentExceptionIsUnhandled>
        <CurrentExceptionType>0</CurrentExceptionType>
        <HasExited>False</HasExited>
        <IsAtSafePoint>True</IsAtSafePoint>
        <IsInValidState>True</IsInValidState>
        <IsMostRecentStackFrameNative>False</IsMostRecentStackFrameNative>
        <MostRecentStackFrame>System.Threading.WaitHandle.WaitOne</MostRecentStackFrame>
        <MostRecentStackFrameWithLoadedSymbols>Debugger.Tests.TestPrograms.MainThreadExit.WaitForALongTime</MostRecentStackFrameWithLoadedSymbols>
        <Name>Worker thread</Name>
        <OldestStackFrame>System.Threading.ThreadHelper.ThreadStart</OldestStackFrame>
        <Priority>Normal</Priority>
        <RuntimeValue>? = {System.Threading.Thread}</RuntimeValue>
        <SelectedStackFrame>Debugger.Tests.TestPrograms.MainThreadExit.WaitForALongTime</SelectedStackFrame>
        <Suspended>False</Suspended>
      </Item>
    </ThreadsAfterExit>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT