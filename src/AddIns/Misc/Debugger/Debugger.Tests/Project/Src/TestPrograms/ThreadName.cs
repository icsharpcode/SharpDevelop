// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class ThreadName
	{
		public static void Main()
		{
			System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.AboveNormal;
			System.Diagnostics.Debugger.Break();
			System.Threading.Thread.CurrentThread.Name = "ThreadName";
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore("Test fails on build server")]
		public void ThreadName()
		{
			debugger.ProcessStarted += debugger_ProcessStarted;
			StartTest("ThreadName.cs");
			debugger.ProcessStarted -= debugger_ProcessStarted;
			ObjectDump("Thread", process.SelectedThread);
			process.Continue();
			ObjectDump("Thread", process.SelectedThread);
			EndTest();
		}

		void debugger_ProcessStarted(object sender, ProcessEventArgs e)
		{
			e.Process.ThreadStarted += delegate(object sender2, ThreadEventArgs f) {
				ObjectDump("ThreadStartedEvent", f.Thread);
			};
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="ThreadName.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ThreadStartedEvent>
      <Thread
        CurrentException="null"
        CurrentExceptionIsUnhandled="False"
        CurrentExceptionType="0"
        HasExited="False"
        IsAtSafePoint="True"
        IsInValidState="True"
        IsMostRecentStackFrameNative="False"
        MostRecentStackFrame="System.AppDomain.SetupDomain"
        MostRecentStackFrameWithLoadedSymbols="null"
        Name=""
        OldestStackFrame="System.AppDomain.SetupDomain"
        Priority="Normal"
        RuntimeValue="? = null"
        SelectedStackFrame="null"
        Suspended="False" />
    </ThreadStartedEvent>
    <ModuleLoaded>ThreadName.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break ThreadName.cs:17,4-17,40</DebuggingPaused>
    <Thread>
      <Thread
        CurrentException="null"
        CurrentExceptionIsUnhandled="False"
        CurrentExceptionType="0"
        HasExited="False"
        IsAtSafePoint="True"
        IsInValidState="True"
        IsMostRecentStackFrameNative="False"
        MostRecentStackFrame="Debugger.Tests.TestPrograms.ThreadName.Main"
        MostRecentStackFrameWithLoadedSymbols="Debugger.Tests.TestPrograms.ThreadName.Main"
        Name=""
        OldestStackFrame="Debugger.Tests.TestPrograms.ThreadName.Main"
        Priority="AboveNormal"
        RuntimeValue="? = {System.Threading.Thread}"
        SelectedStackFrame="Debugger.Tests.TestPrograms.ThreadName.Main"
        Suspended="False" />
    </Thread>
    <DebuggingPaused>Break ThreadName.cs:19,4-19,40</DebuggingPaused>
    <Thread>
      <Thread
        CurrentException="null"
        CurrentExceptionIsUnhandled="False"
        CurrentExceptionType="0"
        HasExited="False"
        IsAtSafePoint="True"
        IsInValidState="True"
        IsMostRecentStackFrameNative="False"
        MostRecentStackFrame="Debugger.Tests.TestPrograms.ThreadName.Main"
        MostRecentStackFrameWithLoadedSymbols="Debugger.Tests.TestPrograms.ThreadName.Main"
        Name="ThreadName"
        OldestStackFrame="Debugger.Tests.TestPrograms.ThreadName.Main"
        Priority="AboveNormal"
        RuntimeValue="? = {System.Threading.Thread}"
        SelectedStackFrame="Debugger.Tests.TestPrograms.ThreadName.Main"
        Suspended="False" />
    </Thread>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT