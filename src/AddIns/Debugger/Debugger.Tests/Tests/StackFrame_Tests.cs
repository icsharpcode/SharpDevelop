// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Debugger.Tests
{
	public class StackFrame_Tests
	{
		public static void Main()
		{
			for (int i = 10; i < 11; i++) {
				System.Diagnostics.Debugger.Break();
			}
			for (short i = 20; i < 21; i++) {
				System.Diagnostics.Debugger.Break();
			}
			
			System.Diagnostics.Debugger.Break();
			
			int j = 30;
			
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void StackFrame_Tests()
		{
			StartTest();
			
			// Test scoping
			DumpLocalVariables("Loop1");
			Continue();
			DumpLocalVariables("Loop2");
			Continue();
			DumpLocalVariables("None");
			Continue();
			DumpLocalVariables("NewVarDefined");
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="StackFrame_Tests.cs">
    <ProcessStarted />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>StackFrame_Tests.exe (Has symbols)</ModuleLoaded>
    <DebuggingPaused>Break StackFrame_Tests.cs:13,5-13,41</DebuggingPaused>
    <Loop1>
      <Item>
        <LocalVariable
          Name="j"
          Type="System.Int32"
          Value="0" />
      </Item>
      <Item>
        <LocalVariable
          Name="i"
          Type="System.Int32"
          Value="10" />
      </Item>
    </Loop1>
    <DebuggingPaused>Break StackFrame_Tests.cs:16,5-16,41</DebuggingPaused>
    <Loop2>
      <Item>
        <LocalVariable
          Name="j"
          Type="System.Int32"
          Value="0" />
      </Item>
      <Item>
        <LocalVariable
          Name="i"
          Type="System.Int16"
          Value="20" />
      </Item>
    </Loop2>
    <DebuggingPaused>Break StackFrame_Tests.cs:19,4-19,40</DebuggingPaused>
    <None>
      <Item>
        <LocalVariable
          Name="j"
          Type="System.Int32"
          Value="0" />
      </Item>
    </None>
    <DebuggingPaused>Break StackFrame_Tests.cs:23,4-23,40</DebuggingPaused>
    <NewVarDefined>
      <Item>
        <LocalVariable
          Name="j"
          Type="System.Int32"
          Value="30" />
      </Item>
    </NewVarDefined>
    <ProcessExited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
