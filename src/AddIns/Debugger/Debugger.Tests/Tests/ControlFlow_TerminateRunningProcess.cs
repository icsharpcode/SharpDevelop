// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;

namespace Debugger.Tests
{
	public class ControlFlow_TerminateRunningProcess
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
		public void ControlFlow_TerminateRunningProcess()
		{
			for(int i = 0; i < 2; i++) {
				StartTest();
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
    name="ControlFlow_TerminateRunningProcess.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ControlFlow_TerminateRunningProcess.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break ControlFlow_TerminateRunningProcess.cs:16,4-16,40</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_TerminateRunningProcess.cs:17,4-17,26</DebuggingPaused>
    <Log>Calling terminate</Log>
    <ProcessExited />
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ControlFlow_TerminateRunningProcess.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break ControlFlow_TerminateRunningProcess.cs:16,4-16,40</DebuggingPaused>
    <DebuggingPaused>StepComplete ControlFlow_TerminateRunningProcess.cs:17,4-17,26</DebuggingPaused>
    <Log>Calling terminate</Log>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
