// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
    <Paused>StackFrame_Callstack.cs:37,4-37,40</Paused>
    <Callstack>
      <Item>
        <StackFrame
          ChainIndex="1"
          FrameIndex="2"
          MethodInfo="[Method Debugger.Tests.StackFrame_Callstack.Sub2():System.Void]"
          NextStatement="StackFrame_Callstack.cs:37,4-37,40"
          Thread="Thread Name =  Suspended = False" />
      </Item>
      <Item>
        <StackFrame
          ChainIndex="1"
          FrameIndex="1"
          MethodInfo="[Method Debugger.Tests.StackFrame_Callstack.Sub1():System.Void]"
          NextStatement="StackFrame_Callstack.cs:32,4-32,11"
          Thread="Thread Name =  Suspended = False" />
      </Item>
      <Item>
        <StackFrame
          ChainIndex="1"
          MethodInfo="[Method Debugger.Tests.StackFrame_Callstack.Main():System.Void]"
          NextStatement="StackFrame_Callstack.cs:27,4-27,11"
          Thread="Thread Name =  Suspended = False" />
      </Item>
    </Callstack>
    <Paused>StackFrame_Callstack.cs:32,4-32,11</Paused>
    <Callstack>
      <Item>
        <StackFrame
          ChainIndex="1"
          FrameIndex="1"
          MethodInfo="[Method Debugger.Tests.StackFrame_Callstack.Sub1():System.Void]"
          NextStatement="StackFrame_Callstack.cs:32,4-32,11"
          Thread="Thread Name =  Suspended = False" />
      </Item>
      <Item>
        <StackFrame
          ChainIndex="1"
          MethodInfo="[Method Debugger.Tests.StackFrame_Callstack.Main():System.Void]"
          NextStatement="StackFrame_Callstack.cs:27,4-27,11"
          Thread="Thread Name =  Suspended = False" />
      </Item>
    </Callstack>
    <Paused>StackFrame_Callstack.cs:27,4-27,11</Paused>
    <Callstack>
      <Item>
        <StackFrame
          ChainIndex="1"
          MethodInfo="[Method Debugger.Tests.StackFrame_Callstack.Main():System.Void]"
          NextStatement="StackFrame_Callstack.cs:27,4-27,11"
          Thread="Thread Name =  Suspended = False" />
      </Item>
    </Callstack>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
