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
			
			StackFrame stackFrame = process.SelectedStackFrame;
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.Continue(); // Go to the SubFunction
			ObjectDump("Old StackFrame", stackFrame);
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.Continue(); // Go back to Function
			ObjectDump("Old StackFrame", stackFrame);
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			process.Continue(); // Setp out of function
			ObjectDump("Main", process.SelectedStackFrame);
			ObjectDump("Old StackFrame", stackFrame);
			ObjectDump("SelectedStackFrame", process.SelectedStackFrame);
			
			EndTest();
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
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Function">
      <ArgumentCount>1</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo>Function</MethodInfo>
      <NextStatement>Start=22,4 End=22,40</NextStatement>
    </SelectedStackFrame>
    <DebuggingPaused>Break</DebuggingPaused>
    <Old_StackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Function">
      <ArgumentCount exception="StackFrame has expired" />
      <Depth>0</Depth>
      <HasExpired>True</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo>Function</MethodInfo>
      <NextStatement exception="StackFrame has expired" />
    </Old_StackFrame>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.SubFunction">
      <ArgumentCount>0</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo>SubFunction</MethodInfo>
      <NextStatement>Start=29,4 End=29,40</NextStatement>
    </SelectedStackFrame>
    <DebuggingPaused>Break</DebuggingPaused>
    <Old_StackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Function">
      <ArgumentCount exception="StackFrame has expired" />
      <Depth>0</Depth>
      <HasExpired>True</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo>Function</MethodInfo>
      <NextStatement exception="StackFrame has expired" />
    </Old_StackFrame>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Function">
      <ArgumentCount>1</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo>Function</MethodInfo>
      <NextStatement>Start=24,4 End=24,40</NextStatement>
    </SelectedStackFrame>
    <DebuggingPaused>Break</DebuggingPaused>
    <Main Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Main">
      <ArgumentCount>0</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo>Main</MethodInfo>
      <NextStatement>Start=17,4 End=17,40</NextStatement>
    </Main>
    <Old_StackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Function">
      <ArgumentCount exception="StackFrame has expired" />
      <Depth>0</Depth>
      <HasExpired>True</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo>Function</MethodInfo>
      <NextStatement exception="StackFrame has expired" />
    </Old_StackFrame>
    <SelectedStackFrame Type="StackFrame" ToString="Debugger.Tests.TestPrograms.FunctionLifetime.Main">
      <ArgumentCount>0</ArgumentCount>
      <Depth>0</Depth>
      <HasExpired>False</HasExpired>
      <HasSymbols>True</HasSymbols>
      <MethodInfo>Main</MethodInfo>
      <NextStatement>Start=17,4 End=17,40</NextStatement>
    </SelectedStackFrame>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT