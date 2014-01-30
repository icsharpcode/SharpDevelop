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
    <Paused>StackFrame_Lifetime.cs:33,4-33,40</Paused>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="1"
        ChainIndex="1"
        FrameIndex="1"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.Function(i:System.Int32):System.Void]"
        NextStatement="StackFrame_Lifetime.cs:33,4-33,40"
        Thread="Thread Name =  Suspended = False" />
    </SelectedStackFrame>
    <Paused>StackFrame_Lifetime.cs:40,4-40,40</Paused>
    <Old_StackFrame>
      <StackFrame
        ArgumentCount="1"
        ChainIndex="1"
        FrameIndex="1"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.Function(i:System.Int32):System.Void]"
        NextStatement="StackFrame_Lifetime.cs:34,4-34,18"
        Thread="Thread Name =  Suspended = False" />
    </Old_StackFrame>
    <SelectedStackFrame>
      <StackFrame
        ChainIndex="1"
        FrameIndex="2"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.SubFunction():System.Void]"
        NextStatement="StackFrame_Lifetime.cs:40,4-40,40"
        Thread="Thread Name =  Suspended = False" />
    </SelectedStackFrame>
    <Paused>StackFrame_Lifetime.cs:35,4-35,40</Paused>
    <Old_StackFrame>
      <StackFrame
        ArgumentCount="1"
        ChainIndex="1"
        FrameIndex="1"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.Function(i:System.Int32):System.Void]"
        NextStatement="StackFrame_Lifetime.cs:35,4-35,40"
        Thread="Thread Name =  Suspended = False" />
    </Old_StackFrame>
    <SelectedStackFrame>
      <StackFrame
        ArgumentCount="1"
        ChainIndex="1"
        FrameIndex="1"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.Function(i:System.Int32):System.Void]"
        NextStatement="StackFrame_Lifetime.cs:35,4-35,40"
        Thread="Thread Name =  Suspended = False" />
    </SelectedStackFrame>
    <Paused>StackFrame_Lifetime.cs:28,4-28,40</Paused>
    <Main>
      <StackFrame
        ChainIndex="1"
        MethodInfo="[Method Debugger.Tests.StackFrame_Lifetime.Main():System.Void]"
        NextStatement="StackFrame_Lifetime.cs:28,4-28,40"
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
        NextStatement="StackFrame_Lifetime.cs:28,4-28,40"
        Thread="Thread Name =  Suspended = False" />
    </SelectedStackFrame>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
