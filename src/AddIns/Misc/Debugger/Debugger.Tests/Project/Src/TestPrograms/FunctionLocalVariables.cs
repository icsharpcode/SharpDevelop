// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class FunctionLocalVariables
	{
		public static void Main()
		{
			int i = 0;
			string s = "S";
			string[] args = new string[] {"p1"};
			object n = null;
			object o = new object();
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void FunctionLocalVariables()
		{
			StartTest("FunctionLocalVariables.cs");
			WaitForPause();
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
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
  <Test name="FunctionLocalVariables.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">FunctionLocalVariables.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <ObjectDump name="SelectedStackFrame" Type="StackFrame">
      <MethodInfo>Main</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=21,4 End=21,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount>0</ArgumentCount>
      <Arguments>[ValueCollection Count=0]</Arguments>
      <LocalVariables>[ValueCollection Count=5]</LocalVariables>
    </ObjectDump>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT