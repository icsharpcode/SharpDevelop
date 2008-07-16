// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class StackOverflow
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			new StackOverflow().Fun(0);
		}
		
		public int Fun(int i)
		{
			return Fun(i + 1);
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void StackOverflow()
		{
			StartTest("StackOverflow.cs");
			
			process.Continue();
			//ObjectDump("LastStackFrame", process.SelectedThread.MostRecentStackFrame);
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="StackOverflow.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>StackOverflow.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break StackOverflow.cs:16,4-16,40</DebuggingPaused>
    <ExceptionThrown>Could not intercept: System.StackOverflowException</ExceptionThrown>
    <DebuggingPaused>Exception StackOverflow.cs:21,3-21,4</DebuggingPaused>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT