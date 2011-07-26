// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Debugger.Tests
{
	public class StackFrame_Lifetime
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
		public void StackFrame_Lifetime()
		{
			StartTest();
			
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
  <Test
    name="StackFrame_Lifetime.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>StackFrame_Lifetime.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break StackFrame_Lifetime.cs:18,4-18,40</DebuggingPaused>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="1"
        ChainIndex="1"
        FrameIndex="1"
        HasSymbols="True"
        MethodInfo="static System.Void Debugger.Tests.StackFrame_Lifetime.Function(System.Int32 i)"
        NextStatement="StackFrame_Lifetime.cs:18,4-18,40"
        Thread="Thread Name =  Suspended = False" />
    </SelectedStackFrame>
    <DebuggingPaused>Break StackFrame_Lifetime.cs:25,4-25,40</DebuggingPaused>
    <Old_StackFrame>
      <StackFrame
        ArgumentCount="1"
        ChainIndex="1"
        FrameIndex="1"
        HasSymbols="True"
        MethodInfo="static System.Void Debugger.Tests.StackFrame_Lifetime.Function(System.Int32 i)"
        NextStatement="StackFrame_Lifetime.cs:19,4-19,18"
        Thread="Thread Name =  Suspended = False" />
    </Old_StackFrame>
    <SelectedStackFrame>
      <StackFrame
        ChainIndex="1"
        FrameIndex="2"
        HasSymbols="True"
        MethodInfo="static System.Void Debugger.Tests.StackFrame_Lifetime.SubFunction()"
        NextStatement="StackFrame_Lifetime.cs:25,4-25,40"
        Thread="Thread Name =  Suspended = False" />
    </SelectedStackFrame>
    <DebuggingPaused>Break StackFrame_Lifetime.cs:20,4-20,40</DebuggingPaused>
    <Old_StackFrame>
      <StackFrame
        ArgumentCount="1"
        ChainIndex="1"
        FrameIndex="1"
        HasSymbols="True"
        MethodInfo="static System.Void Debugger.Tests.StackFrame_Lifetime.Function(System.Int32 i)"
        NextStatement="StackFrame_Lifetime.cs:20,4-20,40"
        Thread="Thread Name =  Suspended = False" />
    </Old_StackFrame>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="1"
        ChainIndex="1"
        FrameIndex="1"
        HasSymbols="True"
        MethodInfo="static System.Void Debugger.Tests.StackFrame_Lifetime.Function(System.Int32 i)"
        NextStatement="StackFrame_Lifetime.cs:20,4-20,40"
        Thread="Thread Name =  Suspended = False" />
    </SelectedStackFrame>
    <DebuggingPaused>Break StackFrame_Lifetime.cs:13,4-13,40</DebuggingPaused>
    <Main>
      <StackFrame
        ChainIndex="1"
        HasSymbols="True"
        MethodInfo="static System.Void Debugger.Tests.StackFrame_Lifetime.Main()"
        NextStatement="StackFrame_Lifetime.cs:13,4-13,40"
        Thread="Thread Name =  Suspended = False" />
    </Main>
    <Old_StackFrame>
      <StackFrame
        ArgumentCount="{Exception: The requested frame index is too big}"
        ChainIndex="1"
        FrameIndex="1"
        HasSymbols="True"
        IsInvalid="True"
        MethodInfo="static System.Void Debugger.Tests.StackFrame_Lifetime.Function(System.Int32 i)"
        NextStatement="{Exception: The requested frame index is too big}"
        Thread="Thread Name =  Suspended = False" />
    </Old_StackFrame>
    <SelectedStackFrame>
      <StackFrame
        ChainIndex="1"
        HasSymbols="True"
        MethodInfo="static System.Void Debugger.Tests.StackFrame_Lifetime.Main()"
        NextStatement="StackFrame_Lifetime.cs:13,4-13,40"
        Thread="Thread Name =  Suspended = False" />
    </SelectedStackFrame>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
