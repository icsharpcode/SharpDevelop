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
	public class TerminateRunningProcess
	{
		static ManualResetEvent doSomething = new ManualResetEvent(false);
		
		public static void Main()
		{
			int i = 42;
			System.Diagnostics.Debugger.Break();
			doSomething.WaitOne();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using NUnit.Framework;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void TerminateRunningProcess()
		{
			for(int i = 0; i < 2; i++) {
				StartTest("TerminateRunningProcess.cs");
				process.SelectedStackFrame.StepOver();
				process.Paused += delegate {
					Assert.Fail("Should not have received any callbacks after Terminate");
				};
				process.SelectedStackFrame.AsyncStepOver();
				ObjectDump("Log", "Calling terminate");
				process.Terminate();
			}
			
			CheckXmlOutput();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="TerminateRunningProcess.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>TerminateRunningProcess.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break TerminateRunningProcess.cs:20,4-20,40</DebuggingPaused>
    <DebuggingPaused>StepComplete TerminateRunningProcess.cs:21,4-21,26</DebuggingPaused>
    <Log>Calling terminate</Log>
    <ProcessExited />
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>TerminateRunningProcess.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break TerminateRunningProcess.cs:20,4-20,40</DebuggingPaused>
    <DebuggingPaused>StepComplete TerminateRunningProcess.cs:21,4-21,26</DebuggingPaused>
    <Log>Calling terminate</Log>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT