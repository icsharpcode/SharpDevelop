// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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
    <DebuggingPaused>Break ControlFlow_TerminatePausedProcess.cs:16,4-16,40</DebuggingPaused>
    <ProcessExited />
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>ControlFlow_TerminatePausedProcess.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break ControlFlow_TerminatePausedProcess.cs:16,4-16,40</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT