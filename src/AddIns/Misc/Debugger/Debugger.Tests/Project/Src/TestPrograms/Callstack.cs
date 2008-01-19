// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class Callstack
	{
		public static void Main()
		{
			Sub1();
		}
		
		static void Sub1()
		{
			Sub2();
		}
		
		static void Sub2()
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
		public void Callstack()
		{
			StartTest("Callstack.cs");
			WaitForPause();
			ObjectDump("Callstack", process.SelectedThread.GetCallstack());
			
			process.StepOut();
			WaitForPause();
			ObjectDump("Callstack", process.SelectedThread.GetCallstack());
			
			process.StepOut();
			WaitForPause();
			ObjectDump("Callstack", process.SelectedThread.GetCallstack());
			
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
  <Test name="Callstack.cs">
    <ProcessStarted />
    <ModuleLoaded symbols="False">mscorlib.dll</ModuleLoaded>
    <ModuleLoaded symbols="True">Callstack.exe</ModuleLoaded>
    <DebuggingPaused>Break</DebuggingPaused>
    <Callstack Type="StackFrame[]" ToString="Debugger.StackFrame[]">
      <Item Type="StackFrame" ToString="Debugger.Tests.TestPrograms.Callstack.Sub2">
        <ArgumentCount>0</ArgumentCount>
        <Depth>0</Depth>
        <HasExpired>False</HasExpired>
        <HasSymbols>True</HasSymbols>
        <MethodInfo>Sub2</MethodInfo>
        <NextStatement>Start=26,4 End=26,40</NextStatement>
      </Item>
      <Item Type="StackFrame" ToString="Debugger.Tests.TestPrograms.Callstack.Sub1">
        <ArgumentCount>0</ArgumentCount>
        <Depth>1</Depth>
        <HasExpired>False</HasExpired>
        <HasSymbols>True</HasSymbols>
        <MethodInfo>Sub1</MethodInfo>
        <NextStatement>Start=21,4 End=21,11</NextStatement>
      </Item>
      <Item Type="StackFrame" ToString="Debugger.Tests.TestPrograms.Callstack.Main">
        <ArgumentCount>0</ArgumentCount>
        <Depth>2</Depth>
        <HasExpired>False</HasExpired>
        <HasSymbols>True</HasSymbols>
        <MethodInfo>Main</MethodInfo>
        <NextStatement>Start=16,4 End=16,11</NextStatement>
      </Item>
    </Callstack>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <Callstack Type="StackFrame[]" ToString="Debugger.StackFrame[]">
      <Item Type="StackFrame" ToString="Debugger.Tests.TestPrograms.Callstack.Sub1">
        <ArgumentCount>0</ArgumentCount>
        <Depth>0</Depth>
        <HasExpired>False</HasExpired>
        <HasSymbols>True</HasSymbols>
        <MethodInfo>Sub1</MethodInfo>
        <NextStatement>Start=21,4 End=21,11</NextStatement>
      </Item>
      <Item Type="StackFrame" ToString="Debugger.Tests.TestPrograms.Callstack.Main">
        <ArgumentCount>0</ArgumentCount>
        <Depth>1</Depth>
        <HasExpired>False</HasExpired>
        <HasSymbols>True</HasSymbols>
        <MethodInfo>Main</MethodInfo>
        <NextStatement>Start=16,4 End=16,11</NextStatement>
      </Item>
    </Callstack>
    <DebuggingPaused>StepComplete</DebuggingPaused>
    <Callstack Type="StackFrame[]" ToString="Debugger.StackFrame[]">
      <Item Type="StackFrame" ToString="Debugger.Tests.TestPrograms.Callstack.Main">
        <ArgumentCount>0</ArgumentCount>
        <Depth>0</Depth>
        <HasExpired>False</HasExpired>
        <HasSymbols>True</HasSymbols>
        <MethodInfo>Main</MethodInfo>
        <NextStatement>Start=16,4 End=16,11</NextStatement>
      </Item>
    </Callstack>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT