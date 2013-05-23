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
			
			StackFrame stackFrame = this.CurrentStackFrame;
			ObjectDump("SelectedStackFrame", this.CurrentStackFrame);
			
			process.Continue(); // Go to the SubFunction
			ObjectDump("Old StackFrame", stackFrame);
			ObjectDump("SelectedStackFrame", this.CurrentStackFrame);
			
			process.Continue(); // Go back to Function
			ObjectDump("Old StackFrame", stackFrame);
			ObjectDump("SelectedStackFrame", this.CurrentStackFrame);
			
			process.Continue(); // Setp out of function
			ObjectDump("Main", this.CurrentStackFrame);
			ObjectDump("Old StackFrame", stackFrame);
			ObjectDump("SelectedStackFrame", this.CurrentStackFrame);
			
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
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>StackFrame_Lifetime.exe (Has symbols)</ModuleLoaded>
    <Paused>StackFrame_Lifetime.cs:18,4-18,40</Paused>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="1"
        ChainIndex="1"
        FrameIndex="1"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.Function(i:System.Int32):System.Void]"
        NextStatement="StackFrame_Lifetime.cs:18,4-18,40"
        Thread="Thread Name =  Suspended = False" />
    </SelectedStackFrame>
    <Paused>StackFrame_Lifetime.cs:25,4-25,40</Paused>
    <Old_StackFrame>
      <StackFrame
        ArgumentCount="1"
        ChainIndex="1"
        FrameIndex="1"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.Function(i:System.Int32):System.Void]"
        NextStatement="StackFrame_Lifetime.cs:19,4-19,18"
        Thread="Thread Name =  Suspended = False" />
    </Old_StackFrame>
    <SelectedStackFrame>
      <StackFrame
        ChainIndex="1"
        FrameIndex="2"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.SubFunction():System.Void]"
        NextStatement="StackFrame_Lifetime.cs:25,4-25,40"
        Thread="Thread Name =  Suspended = False" />
    </SelectedStackFrame>
    <Paused>StackFrame_Lifetime.cs:20,4-20,40</Paused>
    <Old_StackFrame>
      <StackFrame
        ArgumentCount="1"
        ChainIndex="1"
        FrameIndex="1"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.Function(i:System.Int32):System.Void]"
        NextStatement="StackFrame_Lifetime.cs:20,4-20,40"
        Thread="Thread Name =  Suspended = False" />
    </Old_StackFrame>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="1"
        ChainIndex="1"
        FrameIndex="1"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.Function(i:System.Int32):System.Void]"
        NextStatement="StackFrame_Lifetime.cs:20,4-20,40"
        Thread="Thread Name =  Suspended = False" />
    </SelectedStackFrame>
    <Paused>StackFrame_Lifetime.cs:13,4-13,40</Paused>
    <Main>
      <StackFrame
        ChainIndex="1"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.Main():System.Void]"
        NextStatement="StackFrame_Lifetime.cs:13,4-13,40"
        Thread="Thread Name =  Suspended = False" />
    </Main>
    <Old_StackFrame>
      <StackFrame
        ArgumentCount="{Exception: The requested frame index is too big}"
        ChainIndex="1"
        FrameIndex="1"
        IsInvalid="True"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.Function(i:System.Int32):System.Void]"
        NextStatement="{Exception: The requested frame index is too big}"
        Thread="Thread Name =  Suspended = False" />
    </Old_StackFrame>
    <SelectedStackFrame>
      <StackFrame
        ChainIndex="1"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.Main():System.Void]"
        NextStatement="StackFrame_Lifetime.cs:13,4-13,40"
        Thread="Thread Name =  Suspended = False" />
    </SelectedStackFrame>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
