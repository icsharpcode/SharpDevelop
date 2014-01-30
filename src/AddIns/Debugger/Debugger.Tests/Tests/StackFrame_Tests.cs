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
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>StackFrame_Tests.exe (Has symbols)</ModuleLoaded>
    <Paused>StackFrame_Tests.cs:28,5-28,41</Paused>
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
    <Paused>StackFrame_Tests.cs:31,5-31,41</Paused>
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
    <Paused>StackFrame_Tests.cs:34,4-34,40</Paused>
    <None>
      <Item>
        <LocalVariable
          Name="j"
          Type="System.Int32"
          Value="0" />
      </Item>
    </None>
    <Paused>StackFrame_Tests.cs:38,4-38,40</Paused>
    <NewVarDefined>
      <Item>
        <LocalVariable
          Name="j"
          Type="System.Int32"
          Value="30" />
      </Item>
    </NewVarDefined>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
