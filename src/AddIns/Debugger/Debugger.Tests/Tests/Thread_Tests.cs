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
	public class Thread_Tests
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			// This line forces the internal thread object to be created
			System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.AboveNormal;
			System.Diagnostics.Debugger.Break();
			System.Threading.Thread.CurrentThread.Name = "ThreadName";
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Thread_Tests()
		{
			StartTest();
			ObjectDump("Thread", this.CurrentThread);
			process.Continue();
			ObjectDump("Thread", this.CurrentThread);
			process.Continue();
			ObjectDump("Thread", this.CurrentThread);
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="Thread_Tests.cs">
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>Thread_Tests.exe (Has symbols)</ModuleLoaded>
    <Paused>Thread_Tests.cs:27,4-27,40</Paused>
    <Thread>
      <Thread
        Callstack="{[Method Debugger.Tests.Thread_Tests.Main():System.Void]}"
        IsAtSafePoint="True"
        IsInValidState="True"
        MostRecentStackFrame="[Method Debugger.Tests.Thread_Tests.Main():System.Void]"
        Name=""
        Priority="Normal"
        RuntimeValue="null" />
    </Thread>
    <Paused>Thread_Tests.cs:30,4-30,40</Paused>
    <Thread>
      <Thread
        Callstack="{[Method Debugger.Tests.Thread_Tests.Main():System.Void]}"
        IsAtSafePoint="True"
        IsInValidState="True"
        MostRecentStackFrame="[Method Debugger.Tests.Thread_Tests.Main():System.Void]"
        Name=""
        Priority="AboveNormal"
        RuntimeValue="{System.Threading.Thread}" />
    </Thread>
    <Paused>Thread_Tests.cs:32,4-32,40</Paused>
    <Thread>
      <Thread
        Callstack="{[Method Debugger.Tests.Thread_Tests.Main():System.Void]}"
        IsAtSafePoint="True"
        IsInValidState="True"
        MostRecentStackFrame="[Method Debugger.Tests.Thread_Tests.Main():System.Void]"
        Name="ThreadName"
        Priority="AboveNormal"
        RuntimeValue="{System.Threading.Thread}" />
    </Thread>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
