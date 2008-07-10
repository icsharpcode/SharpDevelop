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
		public void ThreadName()
		{
			debugger.ProcessStarted += delegate(object sender, ProcessEventArgs e) {
				e.Process.ThreadStarted += delegate(object sender2, ThreadEventArgs f) {
					ObjectDump("ThreadStartedEvent", f.Thread);
				};
			};
			StartTest("ThreadName.cs");
			ObjectDump("Thread", process.SelectedThread);
			process.Continue();
			ObjectDump("Thread", process.SelectedThread);
			EndTest();
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
    <DebuggingPaused>Break</DebuggingPaused>
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
        Priority="Normal"
        RuntimeValue="? = {System.Threading.Thread}"
        SelectedStackFrame="Debugger.Tests.TestPrograms.ThreadName.Main"
        Suspended="False" />
    </Thread>
    <DebuggingPaused>Break</DebuggingPaused>
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
        Priority="Normal"
        RuntimeValue="? = {System.Threading.Thread}"
        SelectedStackFrame="Debugger.Tests.TestPrograms.ThreadName.Main"
        Suspended="False" />
    </Thread>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT