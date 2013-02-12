// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Debugger.Tests
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
			StartTest();
			
			ObjectDump("Callstack", this.CurrentThread.GetCallstack());
			this.CurrentStackFrame.StepOut();
			ObjectDump("Callstack", this.CurrentThread.GetCallstack());
			this.CurrentStackFrame.StepOut();
			ObjectDump("Callstack", this.CurrentThread.GetCallstack());
			
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
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>StackFrame_Callstack.exe (Has symbols)</ModuleLoaded>
    <Paused>StackFrame_Callstack.cs:22,4-22,40</Paused>
    <Callstack>
      <Item>
        <StackFrame
          ChainIndex="1"
          FrameIndex="2"
          MethodInfo="[Method Debugger.Tests.StackFrame_Callstack.Sub2():System.Void]"
          NextStatement="StackFrame_Callstack.cs:22,4-22,40"
          Thread="Thread Name =  Suspended = False" />
      </Item>
      <Item>
        <StackFrame
          ChainIndex="1"
          FrameIndex="1"
          MethodInfo="[Method Debugger.Tests.StackFrame_Callstack.Sub1():System.Void]"
          NextStatement="StackFrame_Callstack.cs:17,4-17,11"
          Thread="Thread Name =  Suspended = False" />
      </Item>
      <Item>
        <StackFrame
          ChainIndex="1"
          MethodInfo="[Method Debugger.Tests.StackFrame_Callstack.Main():System.Void]"
          NextStatement="StackFrame_Callstack.cs:12,4-12,11"
          Thread="Thread Name =  Suspended = False" />
      </Item>
    </Callstack>
    <Paused>StackFrame_Callstack.cs:17,4-17,11</Paused>
    <Callstack>
      <Item>
        <StackFrame
          ChainIndex="1"
          FrameIndex="1"
          MethodInfo="[Method Debugger.Tests.StackFrame_Callstack.Sub1():System.Void]"
          NextStatement="StackFrame_Callstack.cs:17,4-17,11"
          Thread="Thread Name =  Suspended = False" />
      </Item>
      <Item>
        <StackFrame
          ChainIndex="1"
          MethodInfo="[Method Debugger.Tests.StackFrame_Callstack.Main():System.Void]"
          NextStatement="StackFrame_Callstack.cs:12,4-12,11"
          Thread="Thread Name =  Suspended = False" />
      </Item>
    </Callstack>
    <Paused>StackFrame_Callstack.cs:12,4-12,11</Paused>
    <Callstack>
      <Item>
        <StackFrame
          ChainIndex="1"
          MethodInfo="[Method Debugger.Tests.StackFrame_Callstack.Main():System.Void]"
          NextStatement="StackFrame_Callstack.cs:12,4-12,11"
          Thread="Thread Name =  Suspended = False" />
      </Item>
    </Callstack>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
