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

#if TESTS
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Stepping()
		{
			StartTest("Stepping");
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepOver(); // Debugger.Break
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepOver(); // Debug.WriteLine 1
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepInto(); // Method Sub
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepInto(); // '{'
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepInto(); // Debug.WriteLine 2
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepOut(); // Method Sub
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepOver(); // Method Sub
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.StepOver(); // Method Sub2
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif