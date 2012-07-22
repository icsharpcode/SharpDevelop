// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
    <Paused>Thread_Tests.cs:12,4-12,40</Paused>
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
    <Paused>Thread_Tests.cs:15,4-15,40</Paused>
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
    <Paused>Thread_Tests.cs:17,4-17,40</Paused>
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
