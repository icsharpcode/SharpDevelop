// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class Thread_Name
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
		public void Thread_Name()
		{
			debugger.Processes.Added += debugger_ProcessStarted;
			StartTest("Thread_Name.cs");
			debugger.Processes.Added -= debugger_ProcessStarted;
			ObjectDump("Thread", process.SelectedThread);
			process.Continue();
			ObjectDump("Thread", process.SelectedThread);
			EndTest();
		}

		void debugger_ProcessStarted(object sender, CollectionItemEventArgs<Process> e)
		{
			e.Item.Threads.Added += delegate(object sender2, CollectionItemEventArgs<Thread> f) {
				ObjectDump("ThreadStartedEvent", f.Item);
			};
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="Thread_Name.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ThreadStartedEvent>
      <Thread
        CurrentExceptionType="0"
        GetCallstack="{void System.AppDomain.SetupDomain(Boolean allowRedirects, String path, String configFile)}"
        IsAtSafePoint="True"
        IsInValidState="True"
        MostRecentStackFrame="void System.AppDomain.SetupDomain(Boolean allowRedirects, String path, String configFile)"
        Name=""
        OldestStackFrame="void System.AppDomain.SetupDomain(Boolean allowRedirects, String path, String configFile)"
        Priority="Normal"
        RuntimeValue="null" />
    </ThreadStartedEvent>
    <ModuleLoaded>Thread_Name.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break Thread_Name.cs:17,4-17,40</DebuggingPaused>
    <Thread>
      <Thread
        CurrentExceptionType="0"
        GetCallstack="{static void Debugger.Tests.TestPrograms.Thread_Name.Main()}"
        IsAtSafePoint="True"
        IsInValidState="True"
        MostRecentStackFrame="static void Debugger.Tests.TestPrograms.Thread_Name.Main()"
        MostRecentStackFrameWithLoadedSymbols="static void Debugger.Tests.TestPrograms.Thread_Name.Main()"
        Name=""
        OldestStackFrame="static void Debugger.Tests.TestPrograms.Thread_Name.Main()"
        Priority="AboveNormal"
        RuntimeValue="{System.Threading.Thread}"
        SelectedStackFrame="static void Debugger.Tests.TestPrograms.Thread_Name.Main()" />
    </Thread>
    <DebuggingPaused>Break Thread_Name.cs:19,4-19,40</DebuggingPaused>
    <Thread>
      <Thread
        CurrentExceptionType="0"
        GetCallstack="{static void Debugger.Tests.TestPrograms.Thread_Name.Main()}"
        IsAtSafePoint="True"
        IsInValidState="True"
        MostRecentStackFrame="static void Debugger.Tests.TestPrograms.Thread_Name.Main()"
        MostRecentStackFrameWithLoadedSymbols="static void Debugger.Tests.TestPrograms.Thread_Name.Main()"
        Name="ThreadName"
        OldestStackFrame="static void Debugger.Tests.TestPrograms.Thread_Name.Main()"
        Priority="AboveNormal"
        RuntimeValue="{System.Threading.Thread}"
        SelectedStackFrame="static void Debugger.Tests.TestPrograms.Thread_Name.Main()" />
    </Thread>
    <ThreadStartedEvent>
      <Thread
        CurrentExceptionType="0"
        GetCallstack="{void System.Threading.ReaderWriterLock.Finalize()}"
        IsAtSafePoint="True"
        IsInValidState="True"
        MostRecentStackFrame="void System.Threading.ReaderWriterLock.Finalize()"
        Name=""
        OldestStackFrame="void System.Threading.ReaderWriterLock.Finalize()"
        Priority="Normal"
        RuntimeValue="null" />
    </ThreadStartedEvent>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT