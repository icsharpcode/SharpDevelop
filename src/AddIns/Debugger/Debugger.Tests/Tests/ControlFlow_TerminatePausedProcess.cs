// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Debugger.Tests
{
	public class ControlFlow_TerminatePausedProcess
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void ControlFlow_TerminatePausedProcess()
		{
			StartTest();
			process.Terminate();
			
			StartTest();
			process.Terminate();
			
			CheckXmlOutput();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="ControlFlow_TerminatePausedProcess.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ControlFlow_TerminatePausedProcess.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break ControlFlow_TerminatePausedProcess.cs:12,4-12,40</DebuggingPaused>
    <ProcessExited />
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ControlFlow_TerminatePausedProcess.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break ControlFlow_TerminatePausedProcess.cs:12,4-12,40</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
