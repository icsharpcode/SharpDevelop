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
			
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.SelectedStackFrame.StepOver(); // Debugger.Break
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.SelectedStackFrame.StepOver(); // Debug.WriteLine 1
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.SelectedStackFrame.StepInto(); // Method Sub
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.SelectedStackFrame.StepInto(); // '{'
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.SelectedStackFrame.StepInto(); // Debug.WriteLine 2
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.SelectedStackFrame.StepOut(); // Method Sub
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.SelectedStackFrame.StepOver(); // Method Sub
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			process.SelectedStackFrame.StepOver(); // Method Sub2
			ObjectDumpToString("NextStatement", process.SelectedStackFrame.NextStatement);
			
			EndTest();
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
    <NextStatement>Start=16,4 End=16,40</NextStatement>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <NextStatement>Start=17,4 End=17,44</NextStatement>
    <ModuleLoaded symbols="False">System.Configuration.dll</ModuleLoaded>
    <ModuleLoaded symbols="False">System.Xml.dll</ModuleLoaded>
    <LogMessage>1\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <NextStatement>Start=18,4 End=18,10</NextStatement>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <NextStatement>Start=23,3 End=23,4</NextStatement>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <NextStatement>Start=24,4 End=24,44</NextStatement>
    <LogMessage>2\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <NextStatement>Start=25,4 End=25,44</NextStatement>
    <LogMessage>3\r\n</LogMessage>
    <LogMessage>4\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <NextStatement>Start=18,4 End=18,10</NextStatement>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <NextStatement>Start=19,4 End=19,11</NextStatement>
    <LogMessage>5\r\n</LogMessage>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <NextStatement>Start=20,3 End=20,4</NextStatement>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT