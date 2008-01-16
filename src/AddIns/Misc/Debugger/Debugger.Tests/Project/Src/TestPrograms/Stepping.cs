// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class Stepping
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			System.Diagnostics.Debug.WriteLine("1"); // Step over external code
			Sub(); // Step in internal code
			Sub2(); // Step over internal code
		}
		
		public static void Sub()
		{ // Step in noop
			System.Diagnostics.Debug.WriteLine("2"); // Step in external code
			System.Diagnostics.Debug.WriteLine("3"); // Step out
			System.Diagnostics.Debug.WriteLine("4");
		}
		
		public static void Sub2()
		{
			System.Diagnostics.Debug.WriteLine("5");
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Stepping()
		{
			StartTest("Stepping.cs");
			WaitForPause();
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.StepOver(); // Debugger.Break
			WaitForPause();
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.StepOver(); // Debug.WriteLine 1
			WaitForPause();
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.StepInto(); // Method Sub
			WaitForPause();
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.StepInto(); // '{'
			WaitForPause();
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.StepInto(); // Debug.WriteLine 2
			WaitForPause();
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.StepOut(); // Method Sub
			WaitForPause();
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.StepOver(); // Method Sub
			WaitForPause();
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.StepOver(); // Method Sub2
			WaitForPause();
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test name="Stepping.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">Stepping.exe</ModuleLoaded>
    <ModuleLoaded symbols="False">System.dll</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <Object name="NextStatement">Start=16,4 End=16,40</Object>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <Object name="NextStatement">Start=17,4 End=17,44</Object>
    <ModuleLoaded symbols="False">System.Configuration.dll</ModuleLoaded>
    <ModuleLoaded symbols="False">System.Xml.dll</ModuleLoaded>
    <LogMessage>1\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <Object name="NextStatement">Start=18,4 End=18,10</Object>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <Object name="NextStatement">Start=23,3 End=23,4</Object>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <Object name="NextStatement">Start=24,4 End=24,44</Object>
    <LogMessage>2\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <Object name="NextStatement">Start=25,4 End=25,44</Object>
    <LogMessage>3\r\n</LogMessage>
    <LogMessage>4\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <Object name="NextStatement">Start=18,4 End=18,10</Object>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <Object name="NextStatement">Start=19,4 End=19,11</Object>
    <LogMessage>5\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <Object name="NextStatement">Start=20,3 End=20,4</Object>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT