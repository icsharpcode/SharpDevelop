// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class StackFrame_Callstack
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
		public void StackFrame_Callstack()
		{
			StartTest("StackFrame_Callstack.cs");
			
			ObjectDump("Callstack", process.SelectedThread.GetCallstack());
			process.SelectedStackFrame.StepOut();
			ObjectDump("Callstack", process.SelectedThread.GetCallstack());
			process.SelectedStackFrame.StepOut();
			ObjectDump("Callstack", process.SelectedThread.GetCallstack());
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="StackFrame_Callstack.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>StackFrame_Callstack.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break StackFrame_Callstack.cs:26,4-26,40</DebuggingPaused>
    <Callstack>
      <Item>
        <StackFrame
          ChainIndex="1"
          FrameIndex="2"
          HasSymbols="True"
          MethodInfo="Sub2"
          NextStatement="StackFrame_Callstack.cs:26,4-26,40" />
      </Item>
      <Item>
        <StackFrame
          ChainIndex="1"
          FrameIndex="1"
          HasSymbols="True"
          MethodInfo="Sub1"
          NextStatement="StackFrame_Callstack.cs:21,4-21,11" />
      </Item>
      <Item>
        <StackFrame
          ChainIndex="1"
          HasSymbols="True"
          MethodInfo="Main"
          NextStatement="StackFrame_Callstack.cs:16,4-16,11" />
      </Item>
    </Callstack>
    <DebuggingPaused>StepComplete StackFrame_Callstack.cs:21,4-21,11</DebuggingPaused>
    <Callstack>
      <Item>
        <StackFrame
          ChainIndex="1"
          FrameIndex="1"
          HasSymbols="True"
          MethodInfo="Sub1"
          NextStatement="StackFrame_Callstack.cs:21,4-21,11" />
      </Item>
      <Item>
        <StackFrame
          ChainIndex="1"
          HasSymbols="True"
          MethodInfo="Main"
          NextStatement="StackFrame_Callstack.cs:16,4-16,11" />
      </Item>
    </Callstack>
    <DebuggingPaused>StepComplete StackFrame_Callstack.cs:16,4-16,11</DebuggingPaused>
    <Callstack>
      <Item>
        <StackFrame
          ChainIndex="1"
          HasSymbols="True"
          MethodInfo="Main"
          NextStatement="StackFrame_Callstack.cs:16,4-16,11" />
      </Item>
    </Callstack>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT