// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests
{
	public class Thread_Tests
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
		public void Thread_Tests()
		{
			debugger.Processes.Added += debugger_ProcessStarted;
			StartTest();
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
    name="Thread_Tests.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ThreadStartedEvent>
      <Thread
        Callstack="{void System.AppDomain.SetupDomain(Boolean allowRedirects, String path, String configFile, String[] propertyNames, String[] propertyValues)}"
        CurrentExceptionType="0"
        IsAtSafePoint="True"
        IsInValidState="True"
        MostRecentStackFrame="void System.AppDomain.SetupDomain(Boolean allowRedirects, String path, String configFile, String[] propertyNames, String[] propertyValues)"
        Name=""
        Priority="Normal"
        RuntimeValue="null" />
    </ThreadStartedEvent>
    <ModuleLoaded>Thread_Tests.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break Thread_Tests.cs:17,4-17,40</DebuggingPaused>
    <Thread>
      <Thread
        Callstack="{static void Debugger.Tests.Thread_Tests.Main()}"
        CurrentExceptionType="0"
        IsAtSafePoint="True"
        IsInValidState="True"
        MostRecentStackFrame="static void Debugger.Tests.Thread_Tests.Main()"
        Name=""
        Priority="AboveNormal"
        RuntimeValue="{System.Threading.Thread}"
        SelectedStackFrame="static void Debugger.Tests.Thread_Tests.Main()" />
    </Thread>
    <DebuggingPaused>Break Thread_Tests.cs:19,4-19,40</DebuggingPaused>
    <Thread>
      <Thread
        Callstack="{static void Debugger.Tests.Thread_Tests.Main()}"
        CurrentExceptionType="0"
        IsAtSafePoint="True"
        IsInValidState="True"
        MostRecentStackFrame="static void Debugger.Tests.Thread_Tests.Main()"
        Name="ThreadName"
        Priority="AboveNormal"
        RuntimeValue="{System.Threading.Thread}"
        SelectedStackFrame="static void Debugger.Tests.Thread_Tests.Main()" />
    </Thread>
    <ThreadStartedEvent>
      <Thread
        Callstack="{void System.Threading.ReaderWriterLock.Finalize()}"
        CurrentExceptionType="0"
        IsAtSafePoint="True"
        IsInValidState="True"
        MostRecentStackFrame="void System.Threading.ReaderWriterLock.Finalize()"
        Name=""
        Priority="Normal"
        RuntimeValue="null" />
    </ThreadStartedEvent>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT