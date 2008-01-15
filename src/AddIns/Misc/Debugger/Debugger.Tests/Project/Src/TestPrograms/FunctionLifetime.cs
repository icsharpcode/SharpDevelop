// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class FunctionLifetime
	{
		public static void Main()
		{
			Function(1);
			System.Diagnostics.Debugger.Break(); // 4
		}
		
		static void Function(int i)
		{
			System.Diagnostics.Debugger.Break(); // 1
			SubFunction();
			System.Diagnostics.Debugger.Break(); // 3
		}
		
		static void SubFunction()
		{
			System.Diagnostics.Debugger.Break(); // 2
		}
	}
}

#if TESTS
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void FunctionLifetime()
		{
			StartTest("FunctionLifetime");
			WaitForPause();
			StackFrame stackFrame = process.SelectedStackFrame;
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.Continue(); // Go to the SubFunction
			WaitForPause();
			ObjectDump("Old StackFrame", stackFrame);
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.Continue(); // Go back to Function
			WaitForPause();
			ObjectDump("Old StackFrame", stackFrame);
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.Continue(); // Setp out of function
			WaitForPause();
			ObjectDump("Main", process.SelectedStackFrame);
			ObjectDump("Old StackFrame", stackFrame);
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif