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

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void FunctionLifetime()
		{
			StartTest("FunctionLifetime.cs");
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

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test name="FunctionLifetime.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">FunctionLifetime.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <Object name="SelectedStackFrame" Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Function">
      <MethodInfo>Function</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=22,4 End=22,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount>1</ArgumentCount>
      <Arguments>[ValueCollection Count=1]</Arguments>
      <LocalVariables>[ValueCollection Count=0]</LocalVariables>
    </Object>
    <DebuggingPaused>Break</DebuggingPaused>
    <Object name="Old StackFrame" Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Function">
      <MethodInfo>Function</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>True</HasExpired>
      <NextStatement exception="StackFrame has expired" />
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount exception="StackFrame has expired" />
      <Arguments exception="StackFrame has expired" />
      <LocalVariables>[ValueCollection Count=0]</LocalVariables>
    </Object>
    <Object name="SelectedStackFrame" Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.SubFunction">
      <MethodInfo>SubFunction</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=29,4 End=29,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount>0</ArgumentCount>
      <Arguments>[ValueCollection Count=0]</Arguments>
      <LocalVariables>[ValueCollection Count=0]</LocalVariables>
    </Object>
    <DebuggingPaused>Break</DebuggingPaused>
    <Object name="Old StackFrame" Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Function">
      <MethodInfo>Function</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>True</HasExpired>
      <NextStatement exception="StackFrame has expired" />
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount exception="StackFrame has expired" />
      <Arguments exception="StackFrame has expired" />
      <LocalVariables>[ValueCollection Count=0]</LocalVariables>
    </Object>
    <Object name="SelectedStackFrame" Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Function">
      <MethodInfo>Function</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=24,4 End=24,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount>1</ArgumentCount>
      <Arguments>[ValueCollection Count=1]</Arguments>
      <LocalVariables>[ValueCollection Count=0]</LocalVariables>
    </Object>
    <DebuggingPaused>Break</DebuggingPaused>
    <Object name="Main" Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Main">
      <MethodInfo>Main</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=17,4 End=17,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount>0</ArgumentCount>
      <Arguments>[ValueCollection Count=0]</Arguments>
      <LocalVariables>[ValueCollection Count=0]</LocalVariables>
    </Object>
    <Object name="Old StackFrame" Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Function">
      <MethodInfo>Function</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>True</HasExpired>
      <NextStatement exception="StackFrame has expired" />
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount exception="StackFrame has expired" />
      <Arguments exception="StackFrame has expired" />
      <LocalVariables>[ValueCollection Count=0]</LocalVariables>
    </Object>
    <Object name="SelectedStackFrame" Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Main">
      <MethodInfo>Main</MethodInfo>
      <HasSymbols>True</HasSymbols>
      <HasExpired>False</HasExpired>
      <NextStatement>Start=17,4 End=17,40</NextStatement>
      <ThisValue exception="Static method does not have 'this'." />
      <ContaingClassVariables>[ValueCollection Count=0]</ContaingClassVariables>
      <ArgumentCount>0</ArgumentCount>
      <Arguments>[ValueCollection Count=0]</Arguments>
      <LocalVariables>[ValueCollection Count=0]</LocalVariables>
    </Object>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT